# Decision: Ajax Control Toolkit Extender Pattern for Blazor

**By:** Forge (Lead / Web Forms Reviewer)
**Date:** 2026-03-15
**Issue:** #442 — "Design extender pattern for Ajax Toolkit controls"
**Milestone:** M24: Ajax Toolkit Components
**Status:** PROPOSED — awaiting Jeff's review

---

## Problem Statement

The Ajax Control Toolkit (ACT) uses an "extender" pattern where controls attach JavaScript behavior to existing controls rather than rendering their own HTML. For example:

```xml
<asp:TextBox ID="txtDate" runat="server" />
<ajaxToolkit:CalendarExtender TargetControlID="txtDate" Format="MM/dd/yyyy" />
```

The extender resolves `TargetControlID` to the target's `ClientID` at render time, emits a `<script>` block that creates a JavaScript behavior object (`Sys.Extended.UI.CalendarBehavior`), and attaches it to the DOM element. The extender itself renders **no HTML**.

We need a Blazor-native pattern that:
1. Preserves the `<CalendarExtender TargetControlID="txtDate" />` migration markup
2. Works within Blazor's component model and render lifecycle
3. Handles the TargetControlID → DOM element mapping without `@ref` (migration can't add `@ref` to every target)
4. Supports both extenders (no HTML) and standalone controls (Accordion, TabContainer)
5. Integrates with the existing BWFC base class hierarchy

---

## How ACT Extenders Work in Web Forms

### Architecture

```
ExtenderControlBase : Control, IExtenderControl
├── TargetControlID (string) — references sibling control by server ID
├── BehaviorID (string) — optional JS behavior identifier
├── GetScriptDescriptors() — returns ScriptBehaviorDescriptor objects
├── GetScriptReferences() — returns ScriptReference objects (JS files to load)
└── Render() — emits nothing; ScriptManager handles script injection
```

### Lifecycle

1. Page parses markup, creates `ExtenderControlBase` instance with `TargetControlID`
2. During `OnPreRender`, `ScriptManager` calls `GetScriptReferences()` to register JS files
3. During `Render`, `ScriptManager` calls `GetScriptDescriptors()` to get behavior config
4. ScriptManager resolves `TargetControlID` → `FindControl(TargetControlID).ClientID`
5. ScriptManager emits `$create(Sys.Extended.UI.CalendarBehavior, {properties}, null, null, $get('ctl00_MainContent_txtDate'))`
6. Microsoft AJAX Library creates the behavior instance and attaches to the DOM element

### Key Observations

- **Extenders render zero HTML.** All behavior is JS-attached.
- **TargetControlID is a sibling control ID**, resolved via `Control.FindControl()` within the same naming container.
- **BehaviorID defaults to** `TargetControlID + "_" + ExtenderID` (used by JS to find the behavior instance).
- **The ScriptManager is the orchestrator** — it collects all extender registrations and emits script blocks in proper order.
- **JavaScript behaviors extend `Sys.UI.Behavior`** — they have `initialize()`, `dispose()`, and property getters/setters.

---

## Existing BWFC Architecture (Relevant Patterns)

### Base Class Hierarchy

```
ComponentBase (Blazor)
└── BaseWebFormsComponent
    ├── ID, ClientID, Parent, FindControl(), Controls[]
    ├── CascadingValue<BaseWebFormsComponent> for parent-child discovery
    ├── IJSRuntime injection
    ├── BlazorWebFormsJsInterop (lazy-resolved service)
    └── BaseStyledComponent (adds CssClass, BackColor, ForeColor, etc.)
        └── BaseDataBoundComponent → DataBoundComponent<T>
```

### JS Interop Precedent (Chart.js)

```csharp
// ChartJsInterop.cs — ES module pattern (established in M17)
public sealed class ChartJsInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    public ChartJsInterop(IJSRuntime jsRuntime)
    {
        _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/Fritz.BlazorWebFormsComponents/js/chart-interop.js").AsTask());
    }
}
```

### ComponentIdGenerator

Already resolves `ClientID` by walking the parent `NamingContainer` chain. Supports `Static`, `AutoID`, and `Predictable` modes. This is exactly what extenders need.

---

## Design Decision: Hybrid Approach (Option 2 from #442 + wrapper support)

### Chosen Pattern: `@ref` + JS Interop with TargetControlID String Fallback

After analyzing all three options from the issue:

| Option | Pros | Cons |
|--------|------|------|
| (1) Wrapper components | Clean Blazor pattern | Breaks migration markup completely |
| (2) Blazor @ref + JS interop | Blazor-native | Requires adding @ref to targets |
| (3) Pure CSS/JS | Simplest | Can't handle complex behaviors |

**Decision: Modified Option 2 — string-based TargetControlID with JS-side element resolution.**

The extender accepts `TargetControlID` as a string (matching Web Forms), resolves it to a `ClientID` by walking the BWFC component tree, then passes that `ClientID` to a JavaScript ES module that finds the DOM element via `document.getElementById()`. No `@ref` required on the target control.

This preserves the migration markup pattern **exactly**:

```razor
@* Web Forms original: *@
@* <asp:TextBox ID="txtDate" runat="server" /> *@
@* <ajaxToolkit:CalendarExtender TargetControlID="txtDate" Format="MM/dd/yyyy" /> *@

@* Blazor migration (remove asp:/ajaxToolkit: prefix and runat): *@
<TextBox ID="txtDate" />
<CalendarExtender TargetControlID="txtDate" Format="MM/dd/yyyy" />
```

---

## Architecture

### 1. Base Class: `BaseExtenderComponent`

```csharp
namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Base class for Ajax Control Toolkit extender controls.
/// Extenders attach JavaScript behavior to a target control
/// without rendering their own HTML.
/// </summary>
public abstract class BaseExtenderComponent : BaseWebFormsComponent
{
    private IJSObjectReference _module;
    private IJSObjectReference _behaviorInstance;
    private bool _initialized;

    /// <summary>
    /// The ID of the target control to extend. Resolved via
    /// the BWFC component tree (Parent.FindControl) to get
    /// the target's ClientID for DOM lookup.
    /// </summary>
    [Parameter]
    public string TargetControlID { get; set; }

    /// <summary>
    /// Optional behavior identifier for JS-side lookup.
    /// Defaults to "{TargetControlID}_{ID}" if not set.
    /// </summary>
    [Parameter]
    public string BehaviorID { get; set; }

    /// <summary>
    /// Whether the extender's behavior is currently active.
    /// Setting to false disposes the JS behavior without
    /// removing the component.
    /// </summary>
    [Parameter]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Returns the relative path to the JS module for this extender.
    /// Each extender provides its own module.
    /// Example: "./_content/Fritz.BlazorAjaxToolkitComponents/js/calendar-extender.js"
    /// </summary>
    protected abstract string JsModulePath { get; }

    /// <summary>
    /// Returns the JS function name to call for initialization.
    /// Example: "createCalendarBehavior"
    /// </summary>
    protected abstract string JsCreateFunction { get; }

    /// <summary>
    /// Builds the configuration object passed to the JS create function.
    /// Each extender defines its own properties.
    /// </summary>
    protected abstract object GetBehaviorProperties();

    /// <summary>
    /// Resolves TargetControlID to the actual DOM element ID
    /// by walking the BWFC component tree.
    /// </summary>
    protected string ResolveTargetClientID()
    {
        if (string.IsNullOrEmpty(TargetControlID))
            throw new InvalidOperationException(
                $"{GetType().Name} requires TargetControlID to be set.");

        // Walk up to find a parent with FindControl capability
        var container = Parent;
        while (container != null)
        {
            var target = container.FindControl(TargetControlID);
            if (target != null)
            {
                return target.ClientID
                    ?? throw new InvalidOperationException(
                        $"Target control '{TargetControlID}' has no ClientID. " +
                        $"Ensure it has an ID parameter set.");
            }
            container = container.Parent;
        }

        // Fallback: use TargetControlID as literal DOM ID
        // (covers cases where the target is a plain HTML element
        // or a non-BWFC Blazor component with a manual id attribute)
        return TargetControlID;
    }

    protected string ResolvedBehaviorID =>
        BehaviorID ?? $"{TargetControlID}_{ID}";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender && Enabled)
        {
            await InitializeBehaviorAsync();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (_initialized && Enabled)
        {
            await UpdateBehaviorAsync();
        }
        else if (_initialized && !Enabled)
        {
            await DisposeBehaviorAsync();
        }
    }

    private async Task InitializeBehaviorAsync()
    {
        try
        {
            _module = await JsRuntime.InvokeAsync<IJSObjectReference>(
                "import", JsModulePath);

            var targetClientId = ResolveTargetClientID();
            var config = new
            {
                targetId = targetClientId,
                behaviorId = ResolvedBehaviorID,
                properties = GetBehaviorProperties()
            };

            _behaviorInstance = await _module.InvokeAsync<IJSObjectReference>(
                JsCreateFunction, config);

            _initialized = true;
        }
        catch (JSException ex)
        {
            // SSR/prerender — JS interop not available. Silently degrade.
            System.Diagnostics.Debug.WriteLine(
                $"[{GetType().Name}] JS init failed (expected during SSR): {ex.Message}");
        }
    }

    private async Task UpdateBehaviorAsync()
    {
        if (_behaviorInstance == null) return;

        try
        {
            await _module.InvokeVoidAsync("updateBehavior",
                ResolvedBehaviorID, GetBehaviorProperties());
        }
        catch (JSException)
        {
            // Silently degrade during SSR
        }
    }

    private async Task DisposeBehaviorAsync()
    {
        if (_behaviorInstance != null)
        {
            try
            {
                await _module.InvokeVoidAsync("disposeBehavior",
                    ResolvedBehaviorID);
                await _behaviorInstance.DisposeAsync();
            }
            catch (JSDisconnectedException) { }
            catch (ObjectDisposedException) { }

            _behaviorInstance = null;
            _initialized = false;
        }
    }

    protected override async ValueTask Dispose(bool disposing)
    {
        if (disposing)
        {
            await DisposeBehaviorAsync();
            if (_module != null)
            {
                await _module.DisposeAsync();
                _module = null;
            }
        }
        await base.Dispose(disposing);
    }
}
```

### 2. Base Class: `BaseStandaloneToolkitComponent`

For ACT controls that render their own HTML (Accordion, TabContainer, Rating):

```csharp
namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Base class for standalone Ajax Control Toolkit controls that render
/// their own HTML and manage their own JavaScript behavior.
/// Unlike extenders, these have visible output and style properties.
/// </summary>
public abstract class BaseStandaloneToolkitComponent : BaseStyledComponent
{
    private IJSObjectReference _module;
    private IJSObjectReference _componentInstance;
    private bool _initialized;
    protected ElementReference ComponentRef;

    /// <summary>
    /// Returns the relative path to the JS module for this component.
    /// </summary>
    protected abstract string JsModulePath { get; }

    /// <summary>
    /// Returns the JS function name to call for initialization.
    /// </summary>
    protected abstract string JsCreateFunction { get; }

    /// <summary>
    /// Builds the configuration object for JS initialization.
    /// </summary>
    protected abstract object GetComponentConfig();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            try
            {
                _module = await JsRuntime.InvokeAsync<IJSObjectReference>(
                    "import", JsModulePath);

                _componentInstance = await _module.InvokeAsync<IJSObjectReference>(
                    JsCreateFunction,
                    ComponentRef,
                    GetComponentConfig());

                _initialized = true;
            }
            catch (JSException)
            {
                // SSR — degrade gracefully. HTML structure still renders.
            }
        }
    }

    protected async Task UpdateComponentAsync()
    {
        if (!_initialized || _componentInstance == null) return;

        try
        {
            await _module.InvokeVoidAsync("updateComponent",
                ComponentRef, GetComponentConfig());
        }
        catch (JSException) { }
    }

    protected override async ValueTask Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_componentInstance != null)
            {
                try
                {
                    await _componentInstance.InvokeVoidAsync("dispose");
                    await _componentInstance.DisposeAsync();
                }
                catch (JSDisconnectedException) { }
                catch (ObjectDisposedException) { }
            }
            if (_module != null)
            {
                await _module.DisposeAsync();
            }
        }
        await base.Dispose(disposing);
    }
}
```

### 3. Sample Extender: CalendarExtender

```csharp
// CalendarExtender.razor.cs
namespace BlazorAjaxToolkitComponents;

public partial class CalendarExtender : BaseExtenderComponent
{
    private const string MODULE_PATH =
        "./_content/Fritz.BlazorAjaxToolkitComponents/js/calendar-extender.js";

    [Parameter] public string Format { get; set; } = "d";
    [Parameter] public DateTime? SelectedDate { get; set; }
    [Parameter] public EventCallback<DateTime> SelectedDateChanged { get; set; }
    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; } = DayOfWeek.Sunday;
    [Parameter] public string CssClass { get; set; } = "ajax__calendar";
    [Parameter] public DateTime? StartDate { get; set; }
    [Parameter] public DateTime? EndDate { get; set; }
    [Parameter] public bool DefaultView { get; set; } = true;
    [Parameter] public string PopupPosition { get; set; } = "BottomLeft";
    [Parameter] public EventCallback<EventArgs> OnClientDateSelectionChanged { get; set; }

    protected override string JsModulePath => MODULE_PATH;
    protected override string JsCreateFunction => "createCalendarBehavior";

    protected override object GetBehaviorProperties() => new
    {
        format = Format,
        selectedDate = SelectedDate?.ToString("o"),
        firstDayOfWeek = (int)FirstDayOfWeek,
        cssClass = CssClass,
        startDate = StartDate?.ToString("o"),
        endDate = EndDate?.ToString("o"),
        defaultView = DefaultView,
        popupPosition = PopupPosition
    };
}
```

```razor
@* CalendarExtender.razor — extenders render nothing *@
@inherits BaseExtenderComponent
```

### 4. Sample Standalone: Accordion

```csharp
// Accordion.razor.cs
namespace BlazorAjaxToolkitComponents;

public partial class Accordion : BaseStandaloneToolkitComponent
{
    private const string MODULE_PATH =
        "./_content/Fritz.BlazorAjaxToolkitComponents/js/accordion.js";

    [Parameter] public RenderFragment ChildContent { get; set; }
    [Parameter] public RenderFragment<AccordionPane> Panes { get; set; }
    [Parameter] public int SelectedIndex { get; set; } = 0;
    [Parameter] public EventCallback<int> SelectedIndexChanged { get; set; }
    [Parameter] public string HeaderCssClass { get; set; } = "ajax__accordion_header";
    [Parameter] public string HeaderSelectedCssClass { get; set; } = "ajax__accordion_selected";
    [Parameter] public string ContentCssClass { get; set; } = "ajax__accordion_content";
    [Parameter] public bool FadeTransitions { get; set; } = true;
    [Parameter] public int TransitionDuration { get; set; } = 250;
    [Parameter] public bool RequireOpenedPane { get; set; } = true;
    [Parameter] public bool AutoSize { get; set; } = false;
    [Parameter] public bool SuppressHeaderPostbacks { get; set; } = true;

    protected override string JsModulePath => MODULE_PATH;
    protected override string JsCreateFunction => "createAccordion";

    protected override object GetComponentConfig() => new
    {
        selectedIndex = SelectedIndex,
        fadeTransitions = FadeTransitions,
        transitionDuration = TransitionDuration,
        requireOpenedPane = RequireOpenedPane,
        autoSize = AutoSize
    };
}
```

```razor
@* Accordion.razor *@
@inherits BaseStandaloneToolkitComponent

@if (Visible)
{
    <div id="@ClientID" class="@($"ajax__accordion {CssClass}".Trim())"
         style="@Style" @ref="ComponentRef">
        @ChildContent
    </div>
}
```

---

## TargetControlID Resolution — Detailed Design

### The Problem

In Web Forms:
```xml
<asp:TextBox ID="txtDate" runat="server" />
<ajaxToolkit:CalendarExtender TargetControlID="txtDate" />
```

`FindControl("txtDate")` walks the naming container and returns the `TextBox` server control. The ScriptManager then gets `TextBox.ClientID` (e.g., `"ctl00_MainContent_txtDate"`) and emits `$get('ctl00_MainContent_txtDate')` in JavaScript.

### The Blazor Solution

BWFC already has this infrastructure:
- **`BaseWebFormsComponent.Parent`** — cascading parameter providing parent-child tree
- **`BaseWebFormsComponent.Controls`** — list of registered child controls
- **`BaseWebFormsComponent.FindControl(string id)`** — searches children by ID
- **`ComponentIdGenerator.GetClientID(component)`** — resolves ClientID with naming container chain

The `ResolveTargetClientID()` method in `BaseExtenderComponent` (shown above) walks up the parent chain calling `FindControl()` until it finds the target, then returns `target.ClientID`.

### Fallback Strategy

If `FindControl` fails (target is a plain HTML element or non-BWFC component), the extender falls back to using `TargetControlID` as a literal DOM element ID. JavaScript uses `document.getElementById(targetId)` which handles both cases identically.

### Important Constraint

The target control **must render before the extender**. This is guaranteed by Blazor's top-to-bottom render order, matching Web Forms' behavior. The extender's JS initialization runs in `OnAfterRenderAsync(firstRender: true)`, by which time the target element exists in the DOM.

---

## JS Interop Strategy

### Pattern: One ES Module Per Component

Following the established Chart.js precedent (`chart-interop.js`):

```
wwwroot/js/
├── calendar-extender.js      # CalendarExtender behavior
├── modal-popup-extender.js   # ModalPopupExtender behavior
├── collapsible-panel.js      # CollapsiblePanelExtender behavior
├── accordion.js              # Accordion standalone
├── tab-container.js          # TabContainer standalone
├── rating.js                 # Rating standalone
├── confirm-button.js         # ConfirmButtonExtender behavior
├── autocomplete-extender.js  # AutoCompleteExtender behavior
├── masked-edit-extender.js   # MaskedEditExtender behavior
├── numeric-updown.js         # NumericUpDownExtender behavior
├── filtered-textbox.js       # FilteredTextBoxExtender behavior
├── watermark-extender.js     # TextBoxWatermarkExtender behavior
└── _shared/
    └── behavior-base.js      # Shared behavior lifecycle utilities
```

### Module Contract

Every extender JS module exports three functions:

```javascript
// calendar-extender.js

import { createBehaviorBase, disposeBehaviorBase } from './_shared/behavior-base.js';

const behaviors = new Map();

/**
 * Creates and attaches behavior to the target element.
 * @param {object} config - { targetId, behaviorId, properties }
 * @returns {object} - JS object reference for .NET to hold
 */
export function createCalendarBehavior(config) {
    const target = document.getElementById(config.targetId);
    if (!target) {
        console.warn(`[CalendarExtender] Target '${config.targetId}' not found in DOM.`);
        return null;
    }

    const behavior = createBehaviorBase(target, config.behaviorId, config.properties);
    // ... calendar-specific initialization (popup, date picker UI, event handlers)
    behaviors.set(config.behaviorId, behavior);
    return behavior;
}

/**
 * Updates an existing behavior's properties.
 */
export function updateBehavior(behaviorId, properties) {
    const behavior = behaviors.get(behaviorId);
    if (behavior) {
        // Apply property changes
    }
}

/**
 * Disposes a behavior and cleans up DOM/event listeners.
 */
export function disposeBehavior(behaviorId) {
    const behavior = behaviors.get(behaviorId);
    if (behavior) {
        disposeBehaviorBase(behavior);
        behaviors.delete(behaviorId);
    }
}
```

### Shared Behavior Base (`_shared/behavior-base.js`)

```javascript
/**
 * Shared behavior lifecycle utilities.
 * Provides element lookup, event cleanup, and dispose pattern.
 */
export function createBehaviorBase(element, behaviorId, properties) {
    return {
        element,
        behaviorId,
        properties: { ...properties },
        _eventCleanups: [],

        addEventListener(eventName, handler) {
            element.addEventListener(eventName, handler);
            this._eventCleanups.push(() =>
                element.removeEventListener(eventName, handler));
        }
    };
}

export function disposeBehaviorBase(behavior) {
    for (const cleanup of behavior._eventCleanups) {
        cleanup();
    }
    behavior._eventCleanups = [];
    behavior.element = null;
}
```

### Why One Module Per Component (Not Bundled)

1. **Tree-shaking by usage:** If a migration only uses CalendarExtender, only `calendar-extender.js` loads. Bundling would force 100+ KB for 13 controls.
2. **Lazy loading:** ES `import()` in Blazor's JS interop loads modules on demand. First `OnAfterRenderAsync` triggers the import.
3. **Established pattern:** Chart.js interop already works this way. No new infrastructure needed.
4. **Independent development:** Each team member can work on their extender's JS without merge conflicts.

---

## Project Structure

### Separate NuGet Package: `Fritz.BlazorAjaxToolkitComponents`

```
src/
├── BlazorWebFormsComponents/               # Existing — core BWFC
│   └── BlazorWebFormsComponents.csproj     # Fritz.BlazorWebFormsComponents
│
├── BlazorAjaxToolkitComponents/            # NEW — ACT companion library
│   ├── BlazorAjaxToolkitComponents.csproj  # Fritz.BlazorAjaxToolkitComponents
│   ├── _Imports.razor
│   ├── BaseExtenderComponent.cs
│   ├── BaseStandaloneToolkitComponent.cs
│   │
│   ├── Extenders/
│   │   ├── CalendarExtender.razor
│   │   ├── CalendarExtender.razor.cs
│   │   ├── ModalPopupExtender.razor
│   │   ├── ModalPopupExtender.razor.cs
│   │   ├── CollapsiblePanelExtender.razor
│   │   ├── CollapsiblePanelExtender.razor.cs
│   │   ├── ConfirmButtonExtender.razor
│   │   ├── ConfirmButtonExtender.razor.cs
│   │   ├── AutoCompleteExtender.razor
│   │   ├── AutoCompleteExtender.razor.cs
│   │   ├── MaskedEditExtender.razor
│   │   ├── MaskedEditExtender.razor.cs
│   │   ├── NumericUpDownExtender.razor
│   │   ├── NumericUpDownExtender.razor.cs
│   │   ├── FilteredTextBoxExtender.razor
│   │   ├── FilteredTextBoxExtender.razor.cs
│   │   ├── TextBoxWatermarkExtender.razor
│   │   └── TextBoxWatermarkExtender.razor.cs
│   │
│   ├── Standalone/
│   │   ├── Accordion.razor
│   │   ├── Accordion.razor.cs
│   │   ├── AccordionPane.razor
│   │   ├── AccordionPane.razor.cs
│   │   ├── TabContainer.razor
│   │   ├── TabContainer.razor.cs
│   │   ├── TabPanel.razor
│   │   ├── TabPanel.razor.cs
│   │   ├── Rating.razor
│   │   └── Rating.razor.cs
│   │
│   ├── Enums/
│   │   ├── AccordionAutoSize.cs
│   │   ├── MaskedEditType.cs
│   │   └── ... (ACT-specific enums)
│   │
│   └── wwwroot/
│       └── js/
│           ├── _shared/
│           │   └── behavior-base.js
│           ├── calendar-extender.js
│           ├── modal-popup-extender.js
│           ├── collapsible-panel.js
│           ├── accordion.js
│           ├── tab-container.js
│           ├── rating.js
│           ├── confirm-button.js
│           ├── autocomplete-extender.js
│           ├── masked-edit-extender.js
│           ├── numeric-updown.js
│           ├── filtered-textbox.js
│           └── watermark-extender.js
│
├── BlazorAjaxToolkitComponents.Test/       # NEW — bUnit tests
│   └── BlazorAjaxToolkitComponents.Test.csproj
```

### .csproj Reference

```xml
<Project Sdk="Microsoft.NET.Sdk.Razor">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <PackageId>Fritz.BlazorAjaxToolkitComponents</PackageId>
    <Authors>Jeffrey T. Fritz</Authors>
    <Description>Blazor components emulating Ajax Control Toolkit controls for Web Forms migration</Description>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\BlazorWebFormsComponents\BlazorWebFormsComponents.csproj" />
  </ItemGroup>
</Project>
```

### Why Separate Package

1. **Optional dependency:** Not every Web Forms app used ACT. The core BWFC package shouldn't carry ACT JS overhead.
2. **Different release cadence:** ACT components are greenfield (M24+), while core BWFC is stabilizing (M20-M23).
3. **Clear boundary:** ACT controls have different base classes, different test patterns (JS interop mocking), and different documentation.
4. **Installation story:** `dotnet add package Fritz.BlazorAjaxToolkitComponents` — one command, pulls core BWFC transitively.

---

## Component Classification: Extender vs. Standalone

### Extenders (10 components — render no HTML, attach JS behavior)

| # | Component | Target Control | Behavior |
|---|-----------|---------------|----------|
| 1 | **CalendarExtender** | TextBox | Popup date picker |
| 2 | **ModalPopupExtender** | Panel/Button | Modal dialog + backdrop |
| 3 | **CollapsiblePanelExtender** | Panel | Collapse/expand with animation |
| 4 | **ConfirmButtonExtender** | Button/LinkButton | Confirmation dialog before postback |
| 5 | **AutoCompleteExtender** | TextBox | Dropdown suggestions from service |
| 6 | **MaskedEditExtender** | TextBox | Input masking (phone, date, etc.) |
| 7 | **NumericUpDownExtender** | TextBox | Up/down increment buttons |
| 8 | **FilteredTextBoxExtender** | TextBox | Restrict input characters |
| 9 | **TextBoxWatermarkExtender** | TextBox | Placeholder text (legacy — HTML5 has `placeholder`) |
| 10 | **ValidatorCalloutExtender** | Validator | Callout-style validation messages |

### Standalone Controls (3 components — render their own HTML + JS)

| # | Component | HTML Output | Behavior |
|---|-----------|-------------|----------|
| 11 | **Accordion** | `<div>` with panes | Expand/collapse panes |
| 12 | **TabContainer** + **TabPanel** | `<div>` with tabs | Tab switching |
| 13 | **Rating** | `<span>` with stars | Star rating widget |

### Notes

- **TextBoxWatermarkExtender**: Consider implementing as a thin wrapper that simply sets the HTML5 `placeholder` attribute on the target. This is a semantic upgrade — the behavior is identical, but no JS is needed. Mark as `[Obsolete("Use HTML5 placeholder attribute instead")]` with automatic fallback.
- **ValidatorCalloutExtender**: Depends on the BWFC Validators infrastructure. Implement last.
- **TabContainer + TabPanel**: Two components, but counted as one logical unit. TabPanel is a child component of TabContainer (like AccordionPane inside Accordion).

---

## Recommended Implementation Order

### Phase 1: Foundation + Simplest Extenders (2 sprints)

| Priority | Component | Rationale |
|----------|-----------|-----------|
| **P0** | `BaseExtenderComponent` | Foundation — everything depends on this |
| **P0** | `BaseStandaloneToolkitComponent` | Foundation for standalone controls |
| **P0** | `behavior-base.js` | Shared JS infrastructure |
| **P1** | `CollapsiblePanelExtender` | Simplest extender — CSS transitions, no complex UI. Validates the pattern. |
| **P1** | `ConfirmButtonExtender` | Simple — `window.confirm()` interception. Second pattern validation. |

### Phase 2: High-Demand Controls (2 sprints)

| Priority | Component | Rationale |
|----------|-----------|-----------|
| **P1** | `CalendarExtender` | Highest migration demand. Date pickers in every LOB app. |
| **P1** | `ModalPopupExtender` | Second highest demand. Modals are everywhere. |
| **P1** | `Accordion` | Most used standalone. Tests standalone base class. |

### Phase 3: TextBox Extenders (2 sprints)

| Priority | Component | Rationale |
|----------|-----------|-----------|
| **P2** | `MaskedEditExtender` | Medium complexity, high value for form-heavy apps |
| **P2** | `NumericUpDownExtender` | Medium complexity, pairs with MaskedEdit |
| **P2** | `FilteredTextBoxExtender` | Simple, quick implementation |
| **P2** | `TextBoxWatermarkExtender` | Trivial — HTML5 placeholder. May be obsolete-only stub. |

### Phase 4: Complex + Remaining (2 sprints)

| Priority | Component | Rationale |
|----------|-----------|-----------|
| **P2** | `TabContainer` / `TabPanel` | Standalone, medium complexity |
| **P2** | `Rating` | Standalone, self-contained |
| **P3** | `AutoCompleteExtender` | Complex — requires service endpoint integration |
| **P3** | `ValidatorCalloutExtender` | Depends on Validators, implement last |

### Total: ~8 sprints for all 13 components

---

## SSR Compatibility

All extenders and standalone controls **must degrade gracefully under SSR**:

- **Extenders:** Render nothing (they render nothing anyway). JS fails silently during SSR — the `try/catch` in `InitializeBehaviorAsync()` handles this. When the page transitions to InteractiveServer, `OnAfterRenderAsync(firstRender: true)` fires and JS initializes.
- **Standalone:** Render their HTML structure (Accordion panes, tab panels, star containers). Without JS, all panes/tabs are visible (progressive enhancement). JS initialization adds hide/show behavior after hydration.

This matches our existing SSR strategy: default to SSR, opt-in to InteractiveServer per page.

---

## .NET Aspire / Service Registration

No DI registration needed for extenders — they resolve `IJSRuntime` via the existing `BaseWebFormsComponent` injection. The separate package needs only a `@using` directive:

```razor
@* _Imports.razor in consuming project *@
@using BlazorAjaxToolkitComponents
```

If we later need shared services (e.g., AutoComplete endpoint registry), add:

```csharp
services.AddBlazorAjaxToolkitComponents(); // future, only if needed
```

---

## Open Questions for Jeff

1. **Package naming:** `Fritz.BlazorAjaxToolkitComponents` or `Fritz.BlazorWebFormsComponents.AjaxToolkit`? The latter chains the namespace but is longer.
2. **TextBoxWatermarkExtender:** Implement as full JS extender or just set `placeholder` attribute? HTML5 `placeholder` is identical behavior with zero JS.
3. **ValidatorCalloutExtender scope:** Include in M24 or defer? It has a dependency on Validator improvements (Display property, #442 sibling issues).
4. **AutoComplete service contract:** The original ACT AutoComplete called a `[WebMethod]` or WCF service. In Blazor, should we support a callback delegate (`Func<string, Task<IEnumerable<string>>>`) or require a REST endpoint URL? Callback is more Blazor-native; URL is more migration-compatible.

---

## Summary

This design gives us:
- **Zero-change migration markup** for extenders (`<CalendarExtender TargetControlID="txtDate" />`)
- **Proven JS interop pattern** (ES modules, matches Chart.js precedent)
- **Clean separation** (separate NuGet, own base classes, no pollution of core BWFC)
- **Progressive enhancement** (SSR renders HTML, JS adds behavior on hydration)
- **Incremental delivery** (4 phases, simplest first, validates pattern early)

Every future ACT component follows the same pattern: inherit `BaseExtenderComponent` or `BaseStandaloneToolkitComponent`, implement 3 abstract members, write one JS module. The framework does the rest.
