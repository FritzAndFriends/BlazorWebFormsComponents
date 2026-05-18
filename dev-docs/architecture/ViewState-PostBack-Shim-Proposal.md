# Enhanced ViewState & PostBack Shim — Architecture Proposal

**Author:** Forge (Lead / Web Forms Reviewer)  
**Date:** 2026-03-24  
**Status:** Proposal — Awaiting Jeffrey's Review  
**Requested by:** Jeffrey T. Fritz  

---

## Executive Summary

The current ViewState and IsPostBack implementations in BlazorWebFormsComponents are compile-time stubs: ViewState is an in-memory `Dictionary<string, object>` marked `[Obsolete]`, and `IsPostBack` is hardcoded to `false`. These stubs allow migrated code to *compile*, but they don't *behave correctly* — Web Forms code that relied on ViewState persistence or PostBack detection silently produces wrong results.

This proposal upgrades both features from "compiles but doesn't work" to "compiles AND works correctly" across both Blazor SSR and ServerInteractive rendering modes. The goal: **a Web Forms developer's ViewState-backed property pattern and `if (!IsPostBack)` guard should work unchanged in migrated code-behind**.

---

## Table of Contents

1. [Enhanced ViewState](#1-enhanced-viewstate)
2. [PostBack Shim](#2-postback-shim)
3. [Rendering Mode Awareness](#3-rendering-mode-awareness)
4. [Migration Impact](#4-migration-impact)
5. [API Surface Proposal](#5-api-surface-proposal)
6. [Risks & Trade-offs](#6-risks--trade-offs)
7. [Implementation Roadmap](#7-implementation-roadmap)

---

## 1. Enhanced ViewState

### 1.1 The Problem

Web Forms ViewState serialized control state into a hidden `__VIEWSTATE` field on every form POST. Developers used it constantly:

```csharp
// Classic Web Forms pattern — used in thousands of ASCX controls
public int SelectedDepartmentId
{
    get { object val = ViewState["SelectedDepartmentId"]; return val != null ? (int)val : 0; }
    set { ViewState["SelectedDepartmentId"] = value; }
}
```

Today in BWFC, this compiles but:
- The dictionary is in-memory only — values vanish on navigation
- In SSR mode, the dictionary is re-created on every request — values don't survive form POST
- The `[Obsolete]` warning tells developers to stop using it, but the migration path requires them to manually convert every ViewState-backed property to `[Parameter]`

### 1.2 Design: Mode-Adaptive ViewState

ViewState should behave differently based on rendering mode, matching the semantics that Web Forms developers expect:

| Mode | Persistence Mechanism | Lifetime | How It Works |
|------|----------------------|----------|--------------|
| **ServerInteractive** | Component instance memory | Circuit lifetime | Already works today — the dictionary lives as long as the component. No change needed. |
| **SSR (Static)** | Hidden form field (`__bwfc_viewstate`) | Request → Response → Next Request | Serialized to a protected hidden field on render, deserialized on form POST. Mirrors original Web Forms behavior. |

### 1.3 ServerInteractive Mode — No Changes Needed

In ServerInteractive mode, the Blazor component instance lives on the server for the duration of the SignalR circuit. The existing `Dictionary<string, object>` already persists across re-renders. ViewState-backed properties **already work correctly** in this mode.

The only change: **remove the `[Obsolete]` attribute**.

### 1.4 SSR Mode — Hidden Field Persistence

In SSR mode, each HTTP request creates a new component instance. ViewState must round-trip through the form, exactly as Web Forms did.

**Mechanism:**

1. **On Render:** Serialize the ViewState dictionary → encrypt/sign with `IDataProtector` → emit as `<input type="hidden" name="__bwfc_viewstate_{ComponentId}" value="..." />`
2. **On Form POST:** Deserialize the incoming hidden field → populate the ViewState dictionary before `OnInitialized` runs → developer code reads correct values

**Why `IDataProtector` and not raw serialization:**
Web Forms ViewState was infamously vulnerable to tampering until `ViewStateMAC` became mandatory in .NET 4.5.2. We should be secure by default — ASP.NET Core's Data Protection API handles key rotation, encryption, and anti-tampering.

**Serialization format:** JSON via `System.Text.Json`. Web Forms used `LosFormatter`/`ObjectStateFormatter` (binary). JSON is debuggable, smaller for typical use cases, and aligns with modern .NET.

**Type safety concern:** Web Forms ViewState stored `object` values requiring casts. We should support the same `object`-based API for migration compatibility, but also offer a type-safe overlay:

```csharp
// Migration-compatible (unchanged from Web Forms code-behind):
public int SelectedDepartmentId
{
    get { object val = ViewState["SelectedDepartmentId"]; return val != null ? (int)val : 0; }
    set { ViewState["SelectedDepartmentId"] = value; }
}

// NEW: Type-safe convenience (optional, for developers who want to improve their code):
public int SelectedDepartmentId
{
    get => ViewState.GetValueOrDefault<int>("SelectedDepartmentId");
    set => ViewState.Set("SelectedDepartmentId", value);
}
```

### 1.5 ViewState Location Decision

**Recommendation: Keep ViewState on `BaseWebFormsComponent`.**

Rationale:
- It's already there and thousands of migrated code-behinds reference `ViewState` as an instance member
- Moving it to a service would break the `this.ViewState["key"]` syntax that Web Forms code uses directly
- `WebFormsPageBase` should also keep its own `ViewState` property for page-level state (it already has one)

**Do NOT extract to a separate service.** The whole point is migration-compatibility — `ViewState["key"]` must resolve on `this` without any DI or constructor changes.

### 1.6 The `[Obsolete]` Question

**Remove `[Obsolete]` from ViewState once this ships.** It's no longer a stub — it's a real feature. The obsolete message `"ViewState is supported for compatibility and is discouraged for future use"` becomes misleading once ViewState actually works.

**BUT:** Add a code comment explaining that ViewState is a migration shim, not a recommended pattern for new Blazor code:

```csharp
/// <summary>
/// Dictionary-based state storage emulating ASP.NET Web Forms ViewState.
/// In ServerInteractive mode, persists for the component's lifetime.
/// In SSR mode, round-trips via a protected hidden form field.
/// 
/// <para><b>Migration note:</b> This enables Web Forms ViewState-backed property 
/// patterns to work unchanged. For new Blazor code, prefer [Parameter] properties
/// and component fields.</para>
/// </summary>
public ViewStateDictionary ViewState { get; }
```

### 1.7 ViewStateDictionary — Enhanced Type

Replace `Dictionary<string, object>` with a custom `ViewStateDictionary` that:

1. Implements `IDictionary<string, object>` (backward-compatible)
2. Adds type-safe convenience methods
3. Tracks dirty state for serialization optimization
4. Handles JSON deserialization type coercion (JSON numbers → int/long/double as needed)

```csharp
public class ViewStateDictionary : IDictionary<string, object>
{
    private readonly Dictionary<string, object> _store = new();
    
    // Migration-compatible indexer (unchanged Web Forms syntax)
    public object this[string key]
    {
        get => _store.TryGetValue(key, out var value) ? value : null;
        set { _store[key] = value; IsDirty = true; }
    }
    
    // Type-safe convenience methods (new)
    public T GetValueOrDefault<T>(string key, T defaultValue = default)
    {
        if (!_store.TryGetValue(key, out var value) || value is null)
            return defaultValue;
        
        // Handle JSON deserialization type coercion
        if (value is JsonElement element)
            return element.Deserialize<T>();
        
        return (T)Convert.ChangeType(value, typeof(T));
    }
    
    public void Set<T>(string key, T value)
    {
        _store[key] = value;
        IsDirty = true;
    }
    
    // Serialization support
    internal bool IsDirty { get; private set; }
    internal void MarkClean() => IsDirty = false;
    
    internal string Serialize(IDataProtector protector)
    {
        var json = JsonSerializer.Serialize(_store);
        return protector.Protect(json);
    }
    
    internal static ViewStateDictionary Deserialize(string protectedPayload, IDataProtector protector)
    {
        var json = protector.Unprotect(protectedPayload);
        var dict = new ViewStateDictionary();
        var values = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
        foreach (var kvp in values)
            dict._store[kvp.Key] = kvp.Value; // Store as JsonElement for lazy type coercion
        return dict;
    }
    
    // IDictionary<string, object> implementation delegated to _store...
}
```

---

## 2. PostBack Shim

### 2.1 The Problem

Web Forms developers universally wrote:

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        // Bind data, initialize controls — only on first load
        BindDepartments();
    }
}
```

Today, `IsPostBack` is hardcoded to `false`. The guarded block always executes. This is *accidentally correct* for initial renders but wrong for form submissions in SSR mode — the developer intended to skip re-binding on postback.

### 2.2 Design: Mode-Adaptive IsPostBack

| Mode | `IsPostBack` Behavior | Rationale |
|------|----------------------|-----------|
| **SSR — GET request** | `false` | Initial page load. Same as Web Forms first-load. |
| **SSR — POST request** | `true` | Form submission. This IS a postback. Exactly matches Web Forms semantics. |
| **ServerInteractive — `OnInitialized`** | `false` | First render of the component. Matches Page_Load on initial GET. |
| **ServerInteractive — subsequent renders** | `true` | After first render, any re-render is triggered by user interaction — conceptually a postback. |

### 2.3 SSR Implementation

In SSR mode, `HttpContext` is available. We can check the HTTP method:

```csharp
// In WebFormsPageBase (and BaseWebFormsComponent)
public bool IsPostBack
{
    get
    {
        // SSR mode: HttpContext is available
        if (_httpContextAccessor?.HttpContext is { } context)
            return HttpMethods.IsPost(context.Request.Method);
        
        // ServerInteractive mode: track initialization state
        return _hasInitialized;
    }
}
```

This directly mirrors Web Forms: a form submission (`POST`) is a postback; a navigation (`GET`) is not.

### 2.4 ServerInteractive Implementation

In ServerInteractive mode, there's no HTTP request on re-renders. We use component lifecycle:

```csharp
private bool _hasInitialized;

protected override void OnInitialized()
{
    base.OnInitialized();
    // After OnInitialized completes, subsequent renders are "postbacks"
    _hasInitialized = true;
}
```

This means:
- During `OnInitialized` → `IsPostBack == false` (first load)
- During `OnParametersSet` on first render → `IsPostBack == false` (still first load)
- After `OnInitialized` completes, any subsequent event-driven render → `IsPostBack == true`

**Key insight:** In Web Forms, `Page_Load` fires on every request. The `!IsPostBack` guard prevents re-initialization on postbacks. In Blazor ServerInteractive, `OnInitialized` already only fires once, so the `!IsPostBack` guard inside `OnInitialized` is redundant but harmless. The real value is in `OnParametersSet`, which fires on every render — here, `IsPostBack` correctly distinguishes first render from subsequent parameter changes.

### 2.5 AutoPostBack — Actual Behavior

`AutoPostBack` in Web Forms meant: "when the user changes this control's value, automatically submit the form to the server."

| Mode | AutoPostBack Behavior |
|------|----------------------|
| **SSR** | Emit JavaScript `onchange="this.form.submit()"` on the rendered `<select>` / `<input>`. This causes a real form POST, which the server handles as a new request. |
| **ServerInteractive** | Already the default behavior — Blazor's `@onchange` fires immediately over SignalR. `AutoPostBack` is redundant but should be respected. |

**Implementation for SSR:**

```razor
@* In DropDownList.razor *@
<select @attributes="AdditionalAttributes"
        name="@UniqueID"
        class="@CssClass"
        @onchange="HandleChange">
    @if (AutoPostBack && IsSSRMode)
    {
        <script>
            // Inline for SSR — triggers form submit on change
            document.currentScript.previousElementSibling.addEventListener('change', 
                function() { this.form.submit(); });
        </script>
    }
    @foreach (var item in GetItems())
    {
        <option value="@item.Value" selected="@(item.Value == SelectedValue)">@item.Text</option>
    }
</select>
```

**Better approach — avoid inline `<script>`:**

Use an `onchange` HTML attribute directly (no Blazor event wire-up in SSR mode):

```razor
@if (IsSSRMode && AutoPostBack)
{
    <select name="@UniqueID" class="@CssClass" onchange="this.form.submit()">
        @* items *@
    </select>
}
else
{
    <select @attributes="AdditionalAttributes" class="@CssClass" @onchange="HandleChange">
        @* items *@
    </select>
}
```

**Remove `[Obsolete]` from AutoPostBack** on controls where we implement this behavior. It's a real feature now.

### 2.6 IPostBackDataHandler / IPostBackEventHandler

**Recommendation: Do NOT shim these interfaces.**

These are deep Web Forms plumbing interfaces tied to the Page lifecycle and control tree. Shimming them would require recreating the entire Web Forms event pipeline, which is:
1. Enormous scope — the Page lifecycle has 20+ stages
2. Counter-productive — it would encourage keeping legacy patterns instead of migrating to Blazor idioms
3. Unnecessary — the ViewState + IsPostBack shims handle 95%+ of migration scenarios

The BWFC023 analyzer should continue recommending `EventCallback<T>` for these patterns. **These are the patterns that genuinely need manual migration.**

---

## 3. Rendering Mode Awareness

### 3.1 Current Detection Pattern

`WebFormsPageBase` already has the right pattern:

```csharp
protected bool IsHttpContextAvailable
    => _httpContextAccessor.HttpContext is not null;
```

- **SSR / Pre-render:** HttpContext is available → true
- **ServerInteractive (WebSocket):** HttpContext is null → false

### 3.2 Proposed Extension: `RenderMode` Property

Add an explicit enum for clarity:

```csharp
public enum WebFormsRenderMode
{
    /// <summary>SSR or pre-render — HttpContext is available, no circuit.</summary>
    StaticSSR,
    
    /// <summary>Interactive Server — SignalR circuit, no HttpContext.</summary>
    InteractiveServer
}

// On BaseWebFormsComponent:
protected WebFormsRenderMode CurrentRenderMode
    => IsHttpContextAvailable ? WebFormsRenderMode.StaticSSR : WebFormsRenderMode.InteractiveServer;
```

This is used internally by ViewState and PostBack to adapt behavior. **Developers should NOT need to check this** — the shims auto-detect.

### 3.3 Auto-Detection vs. Explicit Configuration

**Recommendation: Auto-detect. No configuration required.**

The `HttpContext` availability check is reliable and well-established in the ASP.NET Core ecosystem. Blazor's own `ComponentBase` uses the same pattern internally for render mode detection. Requiring explicit configuration would add migration friction for zero benefit.

### 3.4 Pre-rendering Edge Case

When a ServerInteractive component pre-renders, it goes through two phases:
1. **Pre-render (SSR):** HttpContext is available, component renders static HTML
2. **Interactive activation:** Component re-initializes on the circuit, HttpContext is null

**ViewState handling:**
- During pre-render: Serialize ViewState to hidden field (SSR behavior)
- During interactive activation: Ignore the hidden field; use in-memory dictionary
- On the interactive component: ViewState persists in memory for the circuit lifetime

**IsPostBack handling:**
- During pre-render: `IsPostBack` based on HTTP method (GET → false)
- During interactive activation: `_hasInitialized` is false → `IsPostBack == false` (correct — this is a fresh component)

This naturally does the right thing. No special pre-render handling needed.

---

## 4. Migration Impact

### 4.1 Zero-Change Patterns (New Shims Make These Just Work)

| Pattern | Before Shim | After Shim |
|---------|------------|------------|
| `ViewState["key"]` get/set | Compiles, doesn't persist in SSR | Works correctly in both modes |
| ViewState-backed properties | Compiles, loses state on form POST | Persists via hidden field (SSR) or memory (Interactive) |
| `if (!IsPostBack) { BindData(); }` | Always binds (IsPostBack=false) | Correctly skips on POST/re-render |
| `AutoPostBack = true` on DropDownList | Compiles, no effect | Submits form on change (SSR) or fires event (Interactive) |

### 4.2 Still Requires Manual Migration

| Pattern | Why It Can't Be Shimmed | Recommended Action |
|---------|------------------------|-------------------|
| `IPostBackEventHandler.RaisePostBackEvent` | Requires Page lifecycle pipeline | Convert to `EventCallback<T>` |
| `IPostBackDataHandler.LoadPostData` | Requires control tree traversal | Use Blazor `@bind` or `@onchange` |
| `Page.RegisterStartupScript` | No client-script registration in Blazor | Use `IJSRuntime.InvokeAsync` |
| `ViewState` with complex custom types | JSON serialization may not match `LosFormatter` | Add `[JsonSerializable]` or simplify types |
| Cross-control ViewState references | Web Forms ViewState was per-control in a tree | Each Blazor component has independent ViewState |

### 4.3 Analyzer Updates

| Analyzer | Current Guidance | New Guidance |
|----------|-----------------|-------------|
| **BWFC002** (ViewState usage) | "Use component state instead" | **Suppress when target inherits BaseWebFormsComponent** — ViewState works now. Add info-level suggestion: "Consider migrating to [Parameter] for new code." |
| **BWFC003** (IsPostBack usage) | "Use lifecycle methods instead" | **Suppress when target inherits WebFormsPageBase** — IsPostBack works now. Add info-level suggestion: "Consider using OnInitialized for one-time setup." |
| **BWFC020** (ViewState-backed property) | "Convert to [Parameter]" | **Change severity from Info to Suggestion.** The pattern works, but [Parameter] is still the Blazor-idiomatic approach. Message: "ViewState-backed property detected. This works with BWFC but consider converting to [Parameter] for idiomatic Blazor." |
| **BWFC023** (IPostBackEventHandler) | "Use EventCallback" | **No change.** These interfaces are NOT shimmed. Keep the warning. |

### 4.4 DepartmentFilter Migration Story — Before and After

#### Today: Manual Rewrite Required

```csharp
// Original Web Forms code-behind (DepartmentFilter.ascx.cs) — CANNOT be used as-is
public int SelectedDepartmentId
{
    get { object val = ViewState["SelectedDepartmentId"]; return val != null ? (int)val : 0; }
    set { ViewState["SelectedDepartmentId"] = value; }
}

protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        BindDepartments();
    }
}
```

```csharp
// Must be manually converted to:
[Parameter] public int SelectedDepartmentId { get; set; }

protected override void OnInitialized()
{
    BindDepartments();  // Always runs since IsPostBack is always false
}
```

#### With Enhanced Shims: Zero-Change Code-Behind

```csharp
// The ORIGINAL Web Forms code-behind works unchanged:
public int SelectedDepartmentId
{
    get { object val = ViewState["SelectedDepartmentId"]; return val != null ? (int)val : 0; }
    set { ViewState["SelectedDepartmentId"] = value; }
}

// Page_Load maps to OnInitialized — IsPostBack now returns correct values
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        BindDepartments();  // Correctly skips on POST (SSR) or re-render (Interactive)
    }
}
```

**The only changes needed are markup:**
- Remove `asp:` prefix and `runat="server"` from ASCX
- Add `@inherits WebFormsPageBase` or appropriate base class
- Wire events to Blazor syntax (`OnSelectedIndexChanged="..."` → `@onchange`)

**The code-behind is untouched.** This is a massive win for ASCX user control migration.

---

## 5. API Surface Proposal

### 5.1 ViewStateDictionary (New Class)

```csharp
namespace Fritz.BlazorWebFormsComponents;

/// <summary>
/// Dictionary-based state storage emulating Web Forms ViewState.
/// Adapts persistence mechanism based on rendering mode:
/// SSR → protected hidden form field; Interactive → in-memory.
/// </summary>
public class ViewStateDictionary : IDictionary<string, object>
{
    // Indexer — migration compatible
    public object this[string key] { get; set; }
    
    // Type-safe convenience
    public T GetValueOrDefault<T>(string key, T defaultValue = default);
    public void Set<T>(string key, T value);
    
    // Standard dictionary operations
    public bool ContainsKey(string key);
    public bool TryGetValue(string key, out object value);
    public void Add(string key, object value);
    public bool Remove(string key);
    public void Clear();
    public int Count { get; }
    
    // Internal serialization (used by rendering infrastructure)
    internal bool IsDirty { get; }
    internal string Serialize(IDataProtector protector);
    internal static ViewStateDictionary Deserialize(string payload, IDataProtector protector);
}
```

### 5.2 BaseWebFormsComponent Changes

```csharp
public abstract class BaseWebFormsComponent : ComponentBase
{
    [Inject] private IDataProtectionProvider DataProtectionProvider { get; set; }
    [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; }
    
    // CHANGED: Type upgraded from Dictionary<string, object> to ViewStateDictionary
    // CHANGED: [Obsolete] attribute REMOVED
    public ViewStateDictionary ViewState { get; } = new();
    
    // NEW: Rendering mode detection
    protected WebFormsRenderMode CurrentRenderMode { get; }
    
    // NEW: PostBack detection  
    public bool IsPostBack { get; }
    
    // EXISTING: (unchanged)
    [Parameter] public string ID { get; set; }
    [Parameter] public string CssClass { get; set; }
    [Parameter] public bool Visible { get; set; }
    // ... etc
    
    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        // SSR mode: Deserialize ViewState from form POST data
        if (CurrentRenderMode == WebFormsRenderMode.StaticSSR)
        {
            var context = HttpContextAccessor.HttpContext;
            if (HttpMethods.IsPost(context.Request.Method))
            {
                var fieldName = $"__bwfc_viewstate_{GetComponentId()}";
                if (context.Request.Form.TryGetValue(fieldName, out var payload))
                {
                    var protector = DataProtectionProvider.CreateProtector("BWFC.ViewState");
                    ViewState.LoadFrom(ViewStateDictionary.Deserialize(payload, protector));
                }
            }
        }
        
        _hasInitialized = true;
    }
}
```

### 5.3 ViewState Hidden Field Rendering

Components that need ViewState persistence in SSR mode render a hidden field. This is handled in the base component's rendering:

```csharp
// Added to BaseWebFormsComponent rendering pipeline
protected void RenderViewStateField(RenderTreeBuilder builder)
{
    if (CurrentRenderMode == WebFormsRenderMode.StaticSSR && ViewState.IsDirty)
    {
        var protector = DataProtectionProvider.CreateProtector("BWFC.ViewState");
        var payload = ViewState.Serialize(protector);
        var fieldName = $"__bwfc_viewstate_{GetComponentId()}";
        
        builder.OpenElement(0, "input");
        builder.AddAttribute(1, "type", "hidden");
        builder.AddAttribute(2, "name", fieldName);
        builder.AddAttribute(3, "value", payload);
        builder.CloseElement();
    }
}
```

### 5.4 WebFormsPageBase Changes

```csharp
public abstract class WebFormsPageBase : ComponentBase
{
    // CHANGED: [Obsolete] REMOVED, type upgraded
    public ViewStateDictionary ViewState { get; } = new();
    
    // CHANGED: No longer hardcoded to false
    public bool IsPostBack
    {
        get
        {
            if (_httpContextAccessor?.HttpContext is { } context)
                return HttpMethods.IsPost(context.Request.Method);
            return _hasInitialized;
        }
    }
    
    // EXISTING: (unchanged)
    protected bool IsHttpContextAvailable { get; }
    
    // ... rest unchanged
}
```

### 5.5 DepartmentFilter — Complete Migrated Example

#### SSR Mode

```razor
@* DepartmentFilter.razor — SSR Mode *@
@inherits WebFormsPageBase

<div class="department-filter">
    <Label AssociatedControlID="ddlDepartments" Text="Department:" CssClass="filter-label" />
    <DropDownList ID="ddlDepartments"
                  CssClass="filter-dropdown"
                  AutoPostBack="true"
                  OnSelectedIndexChanged="ddlDepartments_SelectedIndexChanged" />
</div>

@code {
    // UNCHANGED from Web Forms code-behind:
    public int SelectedDepartmentId
    {
        get { object val = ViewState["SelectedDepartmentId"]; return val != null ? (int)val : 0; }
        set { ViewState["SelectedDepartmentId"] = value; }
    }

    // Page_Load → OnInitializedAsync (or a lifecycle adapter calls this)
    protected void Page_Load(object sender, EventArgs e)
    {
        ddlDepartments.AutoPostBack = AutoPostBack;
        
        if (!IsPostBack)  // FALSE on GET, TRUE on POST — works correctly!
        {
            var departments = PortalDataProvider.GetDepartments();
            // ... bind data ...
        }
    }

    protected void ddlDepartments_SelectedIndexChanged(object sender, EventArgs e)
    {
        SelectedDepartmentId = int.Parse(ddlDepartments.SelectedValue);
        OnDepartmentChanged(EventArgs.Empty);
    }
}
```

**SSR flow:**
1. GET request → `IsPostBack == false` → data binds → ViewState serialized to hidden field
2. User changes dropdown → `AutoPostBack` triggers form submit
3. POST request → ViewState deserialized from hidden field → `IsPostBack == true` → data binding skipped → `SelectedIndexChanged` handler runs

#### ServerInteractive Mode

Same markup, different behavior under the hood:
1. First render → `IsPostBack == false` → data binds → ViewState lives in memory
2. User changes dropdown → Blazor `@onchange` fires over SignalR → `IsPostBack == true` → handler runs
3. ViewState persists in component instance memory for circuit lifetime

**No code changes between modes.** The shims auto-adapt.

---

## 6. Risks & Trade-offs

### 6.1 ViewState Size in SSR Mode

**Risk:** Large ViewState dictionaries create bloated hidden fields, just like Web Forms.

**Mitigation:** 
- Log a warning when serialized ViewState exceeds 4KB
- Document that ViewState should contain small scalar values only (IDs, flags, counts), not data collections
- Consider optional GZip compression for payloads > 1KB

### 6.2 Security — ViewState Tampering

**Risk:** Tampered hidden field data could inject malicious values.

**Mitigation:** `IDataProtector` handles encryption AND authentication (HMAC). Tampered payloads throw `CryptographicException` and result in an empty ViewState (fail-safe to default values).

### 6.3 JSON Serialization Type Fidelity

**Risk:** `System.Text.Json` may not roundtrip all types that `object`-boxing handles. For example, `ViewState["count"] = 42` serializes as JSON number `42`, but deserializes as `JsonElement` or `long`, not `int`. Code doing `(int)ViewState["count"]` would fail.

**Mitigation:** The `ViewStateDictionary` indexer performs `Convert.ChangeType` coercion. The `GetValueOrDefault<T>` method handles `JsonElement` → T conversion explicitly. Document supported types: primitives, strings, enums, simple DTOs. Complex types require `[JsonSerializable]`.

### 6.4 Component ID Stability

**Risk:** Hidden field names use component IDs (`__bwfc_viewstate_{id}`). If the component ID changes between renders, ViewState is lost.

**Mitigation:** Use the existing `ID` parameter (which developers explicitly set) as the component identifier. If no ID is set, generate a deterministic ID from the component's position in the render tree. Document that SSR ViewState requires stable component IDs.

### 6.5 Migration Confusion — Two State Models

**Risk:** Developers may be confused about when to use ViewState vs. `[Parameter]` vs. component fields.

**Mitigation:** Clear documentation:
- **ViewState:** For migrated code-behind that already uses it. Do not use for new code.
- **`[Parameter]`:** For new Blazor code. Parent passes values down.
- **Component fields:** For internal state. Preferred for new code.

### 6.6 IsPostBack Semantic Gap in ServerInteractive

**Risk:** In Web Forms, `IsPostBack` was strictly tied to HTTP POST. In ServerInteractive mode, we're overloading it to mean "after first render." This isn't perfectly equivalent — a parent re-rendering a child with new parameters isn't quite the same as a user-initiated postback.

**Mitigation:** Accept the semantic gap. The important behavior is correct: `if (!IsPostBack)` runs initialization code exactly once. In ServerInteractive, `OnInitialized` already ensures this, so the guard is redundant but correct. In `OnParametersSet`, the guard correctly prevents re-initialization on parameter changes.

---

## 7. Implementation Roadmap

### Phase 1: Core Infrastructure (2 weeks)

- [ ] Implement `ViewStateDictionary` class with serialization support
- [ ] Update `BaseWebFormsComponent.ViewState` → `ViewStateDictionary`
- [ ] Update `WebFormsPageBase.ViewState` → `ViewStateDictionary`
- [ ] Implement `IsPostBack` mode-adaptive logic on both base classes
- [ ] Remove `[Obsolete]` from ViewState and IsPostBack
- [ ] Add `WebFormsRenderMode` enum and `CurrentRenderMode` property
- [ ] Unit tests for ViewStateDictionary (serialize/deserialize/type coercion)
- [ ] Unit tests for IsPostBack (SSR GET/POST, Interactive init/re-render)

### Phase 2: SSR ViewState Persistence (2 weeks)

- [ ] Implement hidden field rendering in BaseWebFormsComponent
- [ ] Implement ViewState deserialization from form POST data
- [ ] Data Protection integration (encryption + signing)
- [ ] Component ID resolution strategy
- [ ] Integration tests — ViewState round-trip through form POST
- [ ] Size limit warnings and optional compression

### Phase 3: AutoPostBack Behavior (1 week)

- [ ] SSR: Emit `onchange="this.form.submit()"` on DropDownList, CheckBox, TextBox, RadioButton
- [ ] ServerInteractive: Verify existing `@onchange` behavior suffices
- [ ] Remove `[Obsolete]` from AutoPostBack on all controls
- [ ] Update BulletedList (the one control missing `[Obsolete]` — add consistent behavior)

### Phase 4: Analyzer Updates (1 week)

- [ ] BWFC002: Reduce to Info severity, update message
- [ ] BWFC003: Reduce to Info severity, update message
- [ ] BWFC020: Change to Suggestion, update message
- [ ] BWFC023: No change (IPostBackEventHandler still not shimmed)
- [ ] Add new analyzer: BWFC025 — "ViewState used with non-serializable type" (Warning)

### Phase 5: Documentation & Samples (1 week)

- [ ] Update ViewState.md docs with new behavior
- [ ] Update migration guide with "zero-change code-behind" examples
- [ ] Add DepartmentFilter SSR sample demonstrating full ViewState round-trip
- [ ] Update User-Controls.md with ASCX migration workflow using shims

**Total estimated effort: 7 weeks**

---

## Appendix A: Comparison with Web Forms ViewState

| Aspect | Web Forms | BWFC Enhanced ViewState |
|--------|-----------|------------------------|
| Storage format | Binary (`LosFormatter`) | JSON (`System.Text.Json`) |
| Transport | Hidden `__VIEWSTATE` field | Hidden `__bwfc_viewstate_{id}` field |
| Protection | ViewStateMAC + optional encryption | `IDataProtector` (encrypt + sign always) |
| Scope | Per-control in control tree | Per-component instance |
| Supported types | Anything `LosFormatter` handles | Primitives, strings, enums, simple DTOs |
| Compression | Built-in (> 500 bytes) | Optional (> 1KB threshold) |
| Performance | Notorious for bloat | Smaller payloads (JSON, per-component) |

## Appendix B: Supported ViewState Value Types

The following types serialize/deserialize correctly via `System.Text.Json`:

- `bool`, `int`, `long`, `double`, `float`, `decimal`
- `string`, `char`
- `DateTime`, `DateTimeOffset`, `TimeSpan`, `Guid`
- `enum` types (serialized as underlying integer)
- `List<T>` and `T[]` where T is a supported type
- Simple POCOs with public properties (no circular references)

Types that require explicit handling:
- Custom classes → must be JSON-serializable
- `DataTable`, `DataSet` → **not supported** (too heavy, use DTOs)
- Delegates, events → **not supported** (not serializable)
