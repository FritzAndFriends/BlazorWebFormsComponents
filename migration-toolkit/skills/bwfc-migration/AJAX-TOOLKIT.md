# Ajax Control Toolkit Migration

Patterns for migrating ASP.NET **Ajax Control Toolkit** extenders and containers to Blazor equivalents using **BlazorAjaxToolkitComponents**.

**Parent skill:** `SKILL.md` (bwfc-migration)

---

## Overview

### What is the Ajax Control Toolkit?

The Ajax Control Toolkit is a library of reusable ASP.NET Web Forms components that add rich client-side JavaScript behaviors — modals, popups, autocomplete, tabs, accordions, sliders, and more. In Web Forms markup, they appear as `<ajaxToolkit:*>` prefixed components.

**Examples:**
- `<ajaxToolkit:ConfirmButtonExtender>` — Browser confirmation dialog
- `<ajaxToolkit:AutoCompleteExtender>` — Typeahead suggestions
- `<ajaxToolkit:ModalPopupExtender>` — Modal dialog with overlay
- `<ajaxToolkit:TabContainer>` — Tabbed content panes
- `<ajaxToolkit:Accordion>` — Collapsible accordion panels

### Ajax Control Toolkit is Now Supported in Blazor

Many Web Forms applications depend on Ajax Control Toolkit controls. Rather than rewrite them from scratch in Blazor, the **BlazorAjaxToolkitComponents** package provides drop-in Blazor replacements for all 14 commonly-used Ajax Control Toolkit controls.

- **Same component names** (minus the prefix)
- **Same property names** (e.g., `TargetControlID`, `ConfirmText`)
- **Same rendered behavior** — CSS and JavaScript targeting the HTML remain compatible
- **Automatic JS module loading** — No manual `<script>` tags needed

Migration is **purely mechanical**: strip the `ajaxToolkit:` prefix and remove `runat="server"`.

---

## Installation

### Step 1: Add the NuGet Package

```bash
dotnet add package BlazorAjaxToolkitComponents
```

BlazorAjaxToolkitComponents depends on BlazorWebFormsComponents, which installs automatically.

### Step 2: Update `_Imports.razor`

Add these `@using` directives to your Blazor project's `_Imports.razor` file:

```razor
@using BlazorAjaxToolkitComponents
@using BlazorAjaxToolkitComponents.Enums
```

### Step 3: Ensure InteractiveServer Render Mode

Ajax Control Toolkit extenders depend on JavaScript interoperability, which requires **InteractiveServer** render mode. Verify your `App.razor` contains:

```razor
@rendermode InteractiveServer
```

Or add `@rendermode InteractiveServer` to individual pages that use extenders.

### Step 4: JavaScript Auto-Loading

No manual `<script>` tags are required. Extender components load their JavaScript modules automatically using ES module imports from the `_content/BlazorAjaxToolkitComponents/js/` static assets path.

---

## Detection: How to Identify Ajax Control Toolkit Usage

### 1. Assembly Registration Directives

Look for `<%@ Register %>` directives that reference AjaxControlToolkit:

```html
<%@ Register Assembly="AjaxControlToolkit" 
             Namespace="AjaxControlToolkit" 
             TagPrefix="ajaxToolkit" %>
```

**Action:** Remove this directive entirely in Blazor (no equivalent needed).

### 2. ToolkitScriptManager

Look for this pattern:

```html
<ajaxToolkit:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" />
```

**Action:** Delete it. Blazor handles script loading automatically.

### 3. Extender and Container Components

Any component with the `ajaxToolkit:` prefix indicates Ajax Control Toolkit usage:

```html
<ajaxToolkit:ConfirmButtonExtender ID="cbe1" runat="server"
    TargetControlID="btnDelete"
    ConfirmText="Are you sure?" />

<ajaxToolkit:TabContainer ID="tabs" runat="server">
    <ajaxToolkit:TabPanel HeaderText="Tab 1">...</ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel HeaderText="Tab 2">...</ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
```

---

## Control Translation Table

All 14 supported Ajax Control Toolkit components and their Blazor equivalents:

| Web Forms | BWFC Blazor | Type | Notes |
|-----------|-------------|------|-------|
| `<ajaxToolkit:Accordion>` | `<Accordion>` | Container | Vertically stacked collapsible panes; requires `AccordionPane` children |
| `<ajaxToolkit:AccordionPane>` | `<AccordionPane>` | Child | Use inside `<Accordion>`; pairs with `HeaderTemplate` and `ContentTemplate` |
| `<ajaxToolkit:AutoCompleteExtender>` | `<AutoCompleteExtender>` | Extender | Typeahead autocomplete for TextBox; requires ServiceMethod callback |
| `<ajaxToolkit:CalendarExtender>` | `<CalendarExtender>` | Extender | Popup date picker for TextBox; supports date format, range constraints |
| `<ajaxToolkit:CollapsiblePanelExtender>` | `<CollapsiblePanelExtender>` | Extender | Collapse/expand panel with animations |
| `<ajaxToolkit:ConfirmButtonExtender>` | `<ConfirmButtonExtender>` | Extender | Browser confirmation dialog; simplest extender |
| `<ajaxToolkit:FilteredTextBoxExtender>` | `<FilteredTextBoxExtender>` | Extender | Character filtering for TextBox |
| `<ajaxToolkit:HoverMenuExtender>` | `<HoverMenuExtender>` | Extender | Hover-triggered popup menu |
| `<ajaxToolkit:MaskedEditExtender>` | `<MaskedEditExtender>` | Extender | Input mask formatting (dates, times, numbers) |
| `<ajaxToolkit:ModalPopupExtender>` | `<ModalPopupExtender>` | Extender | Modal dialog with overlay and focus trap |
| `<ajaxToolkit:NumericUpDownExtender>` | `<NumericUpDownExtender>` | Extender | Numeric spinner with up/down buttons |
| `<ajaxToolkit:PopupControlExtender>` | `<PopupControlExtender>` | Extender | Click-triggered popup (non-modal) |
| `<ajaxToolkit:SliderExtender>` | `<SliderExtender>` | Extender | Range slider control |
| `<ajaxToolkit:TabContainer>` | `<TabContainer>` | Container | Tabbed content panels; requires `TabPanel` children |
| `<ajaxToolkit:TabPanel>` | `<TabPanel>` | Child | Use inside `<TabContainer>`; pairs with `HeaderText` and content |
| `<ajaxToolkit:ToggleButtonExtender>` | `<ToggleButtonExtender>` | Extender | Image-based checkbox toggle |

---

## Migration Pattern

The L1 script (`bwfc-migrate.ps1`) handles Ajax Control Toolkit components mechanically. Here's what happens:

### Layer 1 — Automated Script Transforms

The script performs these transformations automatically:

1. **Strip `ajaxToolkit:` prefix** on all 14 supported components
2. **Remove ToolkitScriptManager** entirely
3. **Remove Register directives** for AjaxControlToolkit
4. **Remove `ID` attributes** from extenders (no longer needed)
5. **Remove `runat="server"`** from all components
6. **Preserve all other attributes** (TargetControlID, properties, etc.)

**Before (Web Forms):**
```html
<%@ Register Assembly="AjaxControlToolkit" 
             Namespace="AjaxControlToolkit" 
             TagPrefix="ajaxToolkit" %>

<ajaxToolkit:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" />

<ajaxToolkit:ConfirmButtonExtender
    ID="cbe1"
    runat="server"
    TargetControlID="btnDelete"
    ConfirmText="Are you sure?" />

<asp:Button ID="btnDelete" Text="Delete" runat="server" />
```

**After (Layer 1 Output):**
```razor
@* Register directive removed *@

@* ToolkitScriptManager removed *@

<ConfirmButtonExtender
    TargetControlID="btnDelete"
    ConfirmText="Are you sure?" />

<Button ID="btnDelete" Text="Delete" />
```

---

## Before/After Examples

### Example 1: ConfirmButtonExtender (Simplest)

**Web Forms:**
```html
<%@ Register Assembly="AjaxControlToolkit" 
             Namespace="AjaxControlToolkit" 
             TagPrefix="ajaxToolkit" %>

<ajaxToolkit:ToolkitScriptManager runat="server" />

<asp:Button ID="btnDelete" Text="Delete Record" OnClick="DeleteRecord" runat="server" />

<ajaxToolkit:ConfirmButtonExtender ID="cbe1" runat="server"
    TargetControlID="btnDelete"
    ConfirmText="Delete this record permanently? This cannot be undone." />
```

**Blazor (After Layer 1):**
```razor
@* After Layer 1: Script output *@

<Button ID="btnDelete" Text="Delete Record" OnClick="DeleteRecord" />

<ConfirmButtonExtender
    TargetControlID="btnDelete"
    ConfirmText="Delete this record permanently? This cannot be undone." />
```

**Layer 2 Manual Work:**
- No additional work needed — the pattern is complete!
- The `OnClick` event handler signature may need conversion (Web Forms: `protected void DeleteRecord(object sender, EventArgs e)` → Blazor: `private void DeleteRecord()`)

---

### Example 2: AutoCompleteExtender (Common in Real Apps)

**Web Forms:**
```html
<%@ Register Assembly="AjaxControlToolkit" 
             Namespace="AjaxControlToolkit" 
             TagPrefix="ajaxToolkit" %>

<ajaxToolkit:ToolkitScriptManager runat="server" />

<asp:TextBox ID="searchBox" runat="server" />

<ajaxToolkit:AutoCompleteExtender ID="ace1" runat="server"
    TargetControlID="searchBox"
    ServiceMethod="GetProductSuggestions"
    MinimumPrefixLength="2"
    CompletionListCssClass="ac-list"
    ServicePath="AutoComplete.asmx" />
```

**Blazor (After Layer 1):**
```razor
<TextBox ID="searchBox" />

<AutoCompleteExtender
    TargetControlID="searchBox"
    ServiceMethod="GetProductSuggestions"
    MinimumPrefixLength="2"
    CompletionListCssClass="ac-list"
    ServicePath="AutoComplete.asmx" />
```

**Layer 2 Manual Work:**
- **Replace `ServicePath` with Blazor callback**: Web Forms uses `ServicePath="AutoComplete.asmx"` (a URL). Blazor uses a C# method. Update:

```razor
<AutoCompleteExtender
    TargetControlID="searchBox"
    ServiceMethod="@GetProductSuggestions"
    MinimumPrefixLength="2"
    CompletionListCssClass="ac-list" />

@code {
    private async Task<IEnumerable<AutoCompleteItem>> GetProductSuggestions(string prefixText, int count)
    {
        return await _productService.SearchAsync(prefixText, count);
    }
}
```

The `ServiceMethod` property now points to a C# method instead of a web service URL.

---

### Example 3: TabContainer with TabPanels (Container Pattern)

**Web Forms:**
```html
<%@ Register Assembly="AjaxControlToolkit" 
             Namespace="AjaxControlToolkit" 
             TagPrefix="ajaxToolkit" %>

<ajaxToolkit:TabContainer ID="tabs" runat="server">
    <ajaxToolkit:TabPanel HeaderText="Customer Info">
        <asp:Label Text="Name: John Doe" runat="server" />
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel HeaderText="Orders">
        <asp:GridView ID="OrderGrid" SelectMethod="GetOrders" runat="server" />
    </ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel HeaderText="Contacts">
        <asp:Label Text="Email: john@example.com" runat="server" />
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
```

**Blazor (After Layer 1):**
```razor
<TabContainer>
    <TabPanel HeaderText="Customer Info">
        <Label Text="Name: John Doe" />
    </TabPanel>
    <TabPanel HeaderText="Orders">
        <GridView SelectMethod="@GetOrders" />
    </TabPanel>
    <TabPanel HeaderText="Contacts">
        <Label Text="Email: john@example.com" />
    </TabPanel>
</TabContainer>
```

**Layer 2 Manual Work:**
- No additional work for simple tab structure
- If tabs contain data-bound controls (like `GridView`), ensure `SelectMethod` is wired as a delegate (see `/bwfc-migration` for data binding patterns)

---

## Key Concept: TargetControlID and ID Rendering

### How Extenders Find Their Targets

Extender components enhance other controls through a `TargetControlID` property — the ID of the HTML element to enhance:

```razor
<TextBox ID="searchBox" />
<AutoCompleteExtender TargetControlID="searchBox" ... />
```

The extender's JavaScript looks for `document.getElementById(TargetControlID)` and attaches behavior to it.

### ID Rendering in BWFC

When a BWFC control has an `ID` attribute, it renders that ID on the resulting HTML element:

```razor
<TextBox ID="myTextBox" />
<!-- renders as -->
<input type="text" id="myTextBox" ... />
```

**This means:** If your target control has `ID="searchBox"`, the extender's `TargetControlID="searchBox"` will find it. **No additional work needed.**

### Careful: If Target Control Doesn't Render an ID

If you use a BWFC component without an `ID`, it won't render an HTML `id` attribute, and the extender won't find it. Always ensure the target control has an `ID` set.

---

## Layer 1 Script Automation

The `bwfc-migrate.ps1` PowerShell script handles Ajax Control Toolkit components automatically:

### What Layer 1 Does

1. **Identifies all Ajax Control Toolkit components** by the `ajaxToolkit:` prefix
2. **Strips the prefix** for all 14 supported controls
3. **Removes ToolkitScriptManager** (entire tag)
4. **Removes Register directives** for AjaxControlToolkit
5. **Removes `ID` attributes** from extenders (no longer needed in Blazor)
6. **Removes `runat="server"`** from all components
7. **Preserves all properties** (TargetControlID, ConfirmText, FilterType, etc.)
8. **Flags unsupported controls** as `<!-- TODO: Unsupported ACT Control: [ControlName] -->`

### Supported vs. Unsupported Controls

**Supported (14 total):**
- Accordion, AccordionPane
- AutoCompleteExtender, CalendarExtender, CollapsiblePanelExtender, ConfirmButtonExtender
- FilteredTextBoxExtender, HoverMenuExtender, MaskedEditExtender, ModalPopupExtender
- NumericUpDownExtender, PopupControlExtender, SliderExtender
- TabContainer, TabPanel, ToggleButtonExtender

**Not Supported** (need manual JS interop):
- DragPanelExtender, ResizableControlExtender, ValidatorCalloutExtender, etc. (Flagged with `<!-- TODO: Unsupported -->`comments)

---

## Layer 2 Manual Work

After Layer 1 completes, Layer 2 (Copilot-assisted) must handle these tasks:

### 1. **Add NuGet Package Reference**

In the Blazor project `.csproj`:

```xml
<ItemGroup>
    <PackageReference Include="BlazorAjaxToolkitComponents" Version="1.0.0" />
</ItemGroup>
```

Or use: `dotnet add package BlazorAjaxToolkitComponents`

### 2. **Update `_Imports.razor`**

Add:
```razor
@using BlazorAjaxToolkitComponents
@using BlazorAjaxToolkitComponents.Enums
```

### 3. **For AutoCompleteExtender Only: Wire ServiceMethod**

Layer 1 preserves the `ServiceMethod` property as-is. If it points to a Web Forms `.asmx` web service, replace it with a Blazor callback method:

**Before:**
```razor
<AutoCompleteExtender
    TargetControlID="searchBox"
    ServiceMethod="GetProductSuggestions"
    ServicePath="AutoComplete.asmx" />
```

**After:**
```razor
<AutoCompleteExtender
    TargetControlID="searchBox"
    ServiceMethod="@GetProductSuggestions"
    MinimumPrefixLength="2" />

@code {
    private async Task<IEnumerable<AutoCompleteItem>> GetProductSuggestions(string prefixText, int count)
    {
        // Call your Blazor service
        return await _productService.SearchAsync(prefixText, count);
    }
}
```

### 4. **Verify TargetControlID References**

Ensure the `TargetControlID` value matches an actual control's `ID` attribute on the page:

```razor
<TextBox ID="mySearch" />
<AutoCompleteExtender TargetControlID="mySearch" ... />  ✅ Correct

<TextBox ID="searchInput" />
<AutoCompleteExtender TargetControlID="mySearch" ... />  ❌ Mismatch
```

### 5. **Handle Any Unsupported Controls**

If Layer 1 flagged controls as `<!-- TODO: Unsupported ACT Control: ... -->`, they require manual replacement:

- **DragPanelExtender** → Use CSS `position: absolute` + `onmousedown` JS interop, or Blazor component library (e.g., for drag/drop)
- **ResizableControlExtender** → Use CSS `resize: both` or Blazor component
- **ValidatorCalloutExtender** → Use Blazor's built-in validation summary with custom CSS

---

## Common Scenarios

### Scenario 1: Simple Confirmation Dialog

**Layer 1 Output:**
```razor
<Button ID="btnDelete" Text="Delete" OnClick="Delete_Click" />
<ConfirmButtonExtender
    TargetControlID="btnDelete"
    ConfirmText="Are you sure?" />
```

**Layer 2 Work:** Convert the event handler signature:

```csharp
// Web Forms: protected void Delete_Click(object sender, EventArgs e)
// Blazor:    private void Delete_Click()
private void Delete_Click()
{
    // Delete logic here
}
```

**No additional work needed.** The extender is ready to use.

---

### Scenario 2: Masked Input for Phone Number

**Layer 1 Output:**
```razor
<TextBox ID="phoneBox" />
<MaskedEditExtender
    TargetControlID="phoneBox"
    Mask="(999) 999-9999" />
```

**Layer 2 Work:** None — the pattern is complete. The mask applies automatically when the user focuses the TextBox.

---

### Scenario 3: Modal Popup with Form

**Layer 1 Output:**
```razor
<Button ID="btnOpenDialog" Text="Open" OnClick="OpenDialog_Click" />
<ModalPopupExtender
    TargetControlID="btnOpenDialog"
    PopupControlID="dialogPanel" />

<Panel ID="dialogPanel">
    <TextBox ID="dialogInput" />
    <Button ID="btnOK" Text="OK" OnClick="OK_Click" />
    <Button ID="btnCancel" Text="Cancel" OnClick="Cancel_Click" />
</Panel>
```

**Layer 2 Work:** None — the pattern is ready. Clicking "Open" shows the modal; clicking "OK" or "Cancel" hides it.

---

## Unsupported Controls and Alternatives

If your Web Forms app uses Ajax Control Toolkit controls NOT in the 14-component list, you'll need manual replacement strategies:

| Unsupported Control | BWFC Alternative | Difficulty |
|---|---|---|
| `DragPanelExtender` | CSS + JS interop for drag behavior | Medium |
| `ResizableControlExtender` | CSS `resize: both` property | Easy |
| `ValidatorCalloutExtender` | Blazor `ValidationSummary` + custom CSS | Easy |
| `RoundedCornersExtender` | CSS `border-radius` | Trivial |
| `AnimationExtender` | CSS animations or Blazor animation library | Medium |
| `ColorPickerExtender` | HTML5 `<input type="color">` or Blazor component | Easy |
| `CascadingDropdownExtender` | Blazor data binding + event handlers | Medium |

Most unsupported controls have straightforward CSS or JavaScript interop replacements. Open the issue if you need guidance on specific controls.

---

## Render Mode and JavaScript

### Why InteractiveServer is Required

Extenders depend on JavaScript interoperability:
- They import ES modules from `_content/BlazorAjaxToolkitComponents/js/`
- They communicate with the Blazor runtime
- They need `IJSRuntime` to be available

**Static (non-interactive) components cannot use extenders.** Extenders will silently skip initialization if `IJSRuntime` is unavailable.

### Recommended Pattern

```razor
@* App.razor *@
<Routes @rendermode="InteractiveServer" />
```

This enables InteractiveServer globally. Alternatively, add `@rendermode InteractiveServer` to individual pages that use extenders.

---

## Troubleshooting

### Extender Not Activating

**Problem:** The extender renders but doesn't apply its behavior (e.g., ConfirmButtonExtender doesn't show a confirmation dialog).

**Diagnosis:**
1. Open browser DevTools → Console; check for JavaScript errors
2. Verify `TargetControlID` matches an actual control ID on the page
3. Verify render mode is `InteractiveServer` (not Static/SSR)
4. Check that `document.getElementById(TargetControlID)` returns the target element (try in browser console)

**Solutions:**
- Fix mismatched IDs
- Add `@rendermode InteractiveServer` to the page
- Verify browser allows dynamic module imports (check network tab for `.js` module loads)

### Target Control Not Found

**Problem:** The extender initializes but can't find its target control.

**Cause:** Extenders initialize after the first render, but the target control might not have rendered yet.

**Solution:** Ensure the target control renders before the extender. In `.razor` files, this is usually automatic (top-to-bottom rendering), but check for conditional rendering:

```razor
@if (showTarget)
{
    <TextBox ID="myInput" />
    <AutoCompleteExtender TargetControlID="myInput" ... />  ✅ Correct
}

<AutoCompleteExtender TargetControlID="myInput" ... />
@if (showTarget)
{
    <TextBox ID="myInput" />  ❌ Wrong: target renders after extender
}
```

---

---

## Extended Extender Reference

The following extenders require manual implementation or can be implemented using CSS and Blazor interop. Each entry shows the migration pattern.

### TextBoxWatermarkExtender — Placeholder Text

**Purpose:** Display placeholder text that disappears when the user focuses the input.

**Web Forms:**
```html
<ajaxToolkit:TextBoxWatermarkExtender ID="twme1" runat="server"
    TargetControlID="searchBox"
    WatermarkText="Enter search term..."
    WatermarkCssClass="watermark-style" />
```

**Blazor Equivalent (CSS + HTML5):**
```razor
<TextBox ID="searchBox" placeholder="Enter search term..." />
```

**Key Property Mappings:**
| Web Forms | Blazor | Notes |
|-----------|--------|-------|
| `WatermarkText` | `placeholder` | HTML5 standard |
| `WatermarkCssClass` | `class` | Apply CSS to TextBox directly |

**Migration Notes:**
- HTML5 `placeholder` attribute replaces `WatermarkText` — no JavaScript needed
- For custom styling, use CSS classes on the TextBox or `::placeholder` pseudo-element
- Modern browsers fully support this; no fallback needed

---

### DragPanelExtender — Makes Panels Draggable

**Purpose:** Enable mouse-drag repositioning of a Panel.

**Web Forms:**
```html
<ajaxToolkit:DragPanelExtender ID="dpe1" runat="server"
    TargetControlID="dragPanel"
    HandleControlID="dragHandle" />

<asp:Panel ID="dragPanel" runat="server">
    <div id="dragHandle" style="cursor: move; background: #ccc;">Drag here</div>
    <p>Draggable content...</p>
</asp:Panel>
```

**Blazor Equivalent (CSS + JS Interop):**
```razor
<div @ref="dragPanelRef" style="position: absolute; cursor: move; user-select: none;">
    <div @onmousedown="StartDrag" style="cursor: move; background: #ccc;">Drag here</div>
    <p>Draggable content...</p>
</div>

@code {
    private ElementReference dragPanelRef;
    private int offsetX = 0, offsetY = 0;
    
    private void StartDrag(MouseEventArgs e)
    {
        offsetX = (int)e.ClientX - _currentX;
        offsetY = (int)e.ClientY - _currentY;
        // Wire onmousemove and onmouseup via JS interop
    }
}
```

**Key Property Mappings:**
| Web Forms | Blazor | Notes |
|-----------|--------|-------|
| `TargetControlID` | `@ref` | Reference the div element |
| `HandleControlID` | Event handler on handle | Attach `@onmousedown` to the drag handle |

**Migration Notes:**
- Requires manual JavaScript interop or a Blazor drag-drop library (e.g., Telerik, DevExpress)
- Consider third-party libraries to avoid complex JS integration
- Use CSS `position: absolute` for the draggable element

---

### ResizableControlExtender — Drag-to-Resize

**Purpose:** Enable resizing a control by dragging its corner.

**Web Forms:**
```html
<ajaxToolkit:ResizableControlExtender ID="rce1" runat="server"
    TargetControlID="resizePanel"
    ResizableCssClass="resize-handle"
    OnClientResize="OnResize" />
```

**Blazor Equivalent (CSS Only):**
```razor
<div style="position: relative; width: 300px; height: 200px; resize: both; overflow: auto; border: 1px solid #ccc;">
    Resizable content
</div>
```

**Key Property Mappings:**
| Web Forms | Blazor | Notes |
|-----------|--------|-------|
| `TargetControlID` | Container `div` | Apply CSS `resize: both` |
| `ResizableCssClass` | CSS `resize` property | Built into modern CSS |
| `OnClientResize` | CSS-only solution | No callback needed for basic resizing |

**Migration Notes:**
- CSS3 `resize: both` property eliminates the need for JavaScript
- Requires `overflow: auto` and explicit `width`/`height`
- Works in all modern browsers (IE 9+)
- For advanced resize events, use a library (e.g., Interact.js)

---

### DropShadowExtender — CSS Box-Shadow

**Purpose:** Add a drop shadow to a control.

**Web Forms:**
```html
<ajaxToolkit:DropShadowExtender ID="dse1" runat="server"
    TargetControlID="shadowPanel"
    Width="5"
    Opacity="0.5" />
```

**Blazor Equivalent (CSS):**
```razor
<div style="box-shadow: 5px 5px 10px rgba(0, 0, 0, 0.5); padding: 10px; background: white;">
    Panel with shadow
</div>
```

**Key Property Mappings:**
| Web Forms | Blazor | Notes |
|-----------|--------|-------|
| `Width` | `box-shadow` blur radius | Map to CSS blur radius |
| `Opacity` | `rgba` alpha channel | Use CSS `rgba()` color |

**Migration Notes:**
- CSS3 `box-shadow` replaces the extender entirely
- Syntax: `box-shadow: offset-x offset-y blur-radius color`
- Much simpler than JavaScript; works in all modern browsers

---

### AlwaysVisibleControlExtender — Fixed Positioning

**Purpose:** Keep a control visible (fixed) as the page scrolls.

**Web Forms:**
```html
<ajaxToolkit:AlwaysVisibleControlExtender ID="avce1" runat="server"
    TargetControlID="fixedPanel"
    VerticalSide="Top"
    HorizontalSide="Right" />
```

**Blazor Equivalent (CSS):**
```razor
<div style="position: fixed; top: 10px; right: 10px; background: white; border: 1px solid #ccc; padding: 10px;">
    Fixed content
</div>
```

**Key Property Mappings:**
| Web Forms | Blazor | Notes |
|-----------|--------|-------|
| `VerticalSide` | `top` or `bottom` CSS property | Direct CSS mapping |
| `HorizontalSide` | `left` or `right` CSS property | Direct CSS mapping |

**Migration Notes:**
- CSS `position: fixed` eliminates the need for JavaScript
- Pair with `top`, `right`, `bottom`, `left` properties
- Consider `z-index` for layering with other elements
- Modern browsers fully support this

---

### RoundedCornersExtender — Border-Radius

**Purpose:** Add rounded corners to a control.

**Web Forms:**
```html
<ajaxToolkit:RoundedCornersExtender ID="rce1" runat="server"
    TargetControlID="roundedPanel"
    Radius="10" />
```

**Blazor Equivalent (CSS):**
```razor
<div style="border-radius: 10px; overflow: hidden; background: white; border: 1px solid #ccc; padding: 10px;">
    Rounded corner panel
</div>
```

**Key Property Mappings:**
| Web Forms | Blazor | Notes |
|-----------|--------|-------|
| `Radius` | `border-radius` CSS property | Direct pixel mapping |

**Migration Notes:**
- CSS3 `border-radius` replaces the extender
- Pair with `overflow: hidden` to clip content at corners
- Specify in pixels (e.g., `border-radius: 10px`)
- Works in all modern browsers (IE 9+)

---

### UpdatePanelAnimationExtender — Loading Animations

**Purpose:** Play animations when an UpdatePanel posts back.

**Web Forms:**
```html
<ajaxToolkit:UpdatePanelAnimationExtender ID="upae1" runat="server"
    TargetControlID="animationPanel">
    <Animations>
        <OnUpdating>
            <FadeOut Duration="0.5" />
        </OnUpdating>
        <OnUpdated>
            <FadeIn Duration="0.5" />
        </OnUpdated>
    </Animations>
</ajaxToolkit:UpdatePanelAnimationExtender>
```

**Blazor Equivalent (CSS Animations + State):**
```razor
<div style="@fadeStyle">
    Loading panel content...
</div>

@code {
    private string fadeStyle = "";
    
    private async Task OnPostBack()
    {
        fadeStyle = "opacity: 0; transition: opacity 0.5s ease;";
        await Task.Delay(500);
        // Update content
        fadeStyle = "opacity: 1; transition: opacity 0.5s ease;";
    }
}
```

**Key Property Mappings:**
| Web Forms | Blazor | Notes |
|-----------|--------|-------|
| `OnUpdating` | Before state change | Set style or trigger animation |
| `OnUpdated` | After state change | Restore style after update |
| `FadeOut/FadeIn` | CSS `opacity` transition | Use CSS `transition` property |

**Migration Notes:**
- Blazor's reactivity makes animations simpler — change state, let CSS handle the transition
- Use CSS `transition` property for smooth effects
- Consider CSS keyframes for complex animations
- No JavaScript required for basic fade effects

---

### PasswordStrength — Password Quality Indicator

**Purpose:** Display visual feedback on password strength.

**Web Forms:**
```html
<ajaxToolkit:PasswordStrength ID="ps1" runat="server"
    TargetControlID="passwordBox"
    StrengthIndicatorType="BarIndicator"
    PreferredPasswordLength="8" />
```

**Blazor Equivalent (Blazor Component + CSS):**
```razor
<TextBox ID="passwordBox" @onchange="CheckPasswordStrength" Type="password" />

<div style="@GetStrengthStyle()">
    <div style="width: @strengthPercentage%; height: 10px; background: @strengthColor; transition: width 0.3s;"></div>
</div>
<p>Strength: @strengthText</p>

@code {
    private int strengthPercentage = 0;
    private string strengthColor = "red";
    private string strengthText = "Weak";
    
    private void CheckPasswordStrength(ChangeEventArgs e)
    {
        string password = e.Value?.ToString() ?? "";
        // Calculate strength...
        strengthPercentage = CalculateStrength(password);
        strengthText = GetStrengthText(strengthPercentage);
    }
    
    private int CalculateStrength(string pwd)
    {
        int strength = 0;
        if (pwd.Length >= 8) strength += 25;
        if (System.Text.RegularExpressions.Regex.IsMatch(pwd, @"[A-Z]")) strength += 25;
        if (System.Text.RegularExpressions.Regex.IsMatch(pwd, @"[0-9]")) strength += 25;
        if (System.Text.RegularExpressions.Regex.IsMatch(pwd, @"[!@#$%^&*]")) strength += 25;
        return strength;
    }
}
```

**Key Property Mappings:**
| Web Forms | Blazor | Notes |
|-----------|--------|-------|
| `TargetControlID` | `@onchange` event | Wire change handler |
| `StrengthIndicatorType` | CSS bars or text | Render visually with CSS |
| `PreferredPasswordLength` | Component logic | Check in C# handler |

**Migration Notes:**
- Blazor makes this a data-binding exercise instead of JavaScript
- Validate password strength in `@code` block
- Update visual indicator with reactive state changes
- No third-party library needed for basic strength checking

---

### ValidatorCalloutExtender — Validation Callouts

**Purpose:** Display validation errors in a styled callout popup.

**Web Forms:**
```html
<asp:TextBox ID="emailBox" runat="server" />
<asp:RequiredFieldValidator ControlToValidate="emailBox" runat="server" />

<ajaxToolkit:ValidatorCalloutExtender ID="vce1" runat="server"
    TargetControlID="emailBox"
    CssClass="validator-callout" />
```

**Blazor Equivalent (ValidationMessage + CSS):**
```razor
<InputText @bind-Value="model.Email" />
<ValidationMessage For="@(() => model.Email)" />

<style>
    .field-validation-error {
        position: absolute;
        background: #fff3cd;
        border: 1px solid #ffc107;
        padding: 8px;
        border-radius: 4px;
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
        z-index: 1000;
    }
</style>
```

**Key Property Mappings:**
| Web Forms | Blazor | Notes |
|-----------|--------|-------|
| `TargetControlID` | Associated control | Use `@bind` with validator |
| `CssClass` | `.field-validation-error` | Apply custom CSS styling |

**Migration Notes:**
- Use Blazor's built-in `ValidationMessage` component
- Style with CSS for the callout appearance
- No JavaScript needed — validation is handled by Blazor data annotations
- Supports async validation with EditContext

---

### SlideShowExtender — Image Carousel

**Purpose:** Display a rotating slideshow of images.

**Web Forms:**
```html
<ajaxToolkit:SlideShowExtender ID="sse1" runat="server"
    TargetControlID="slideShowPanel"
    Loop="true"
    AutoPlay="true"
    PlayInterval="3000" />
```

**Blazor Equivalent (Component + Timer):**
```razor
<div style="width: 400px; height: 300px; position: relative; overflow: hidden;">
    <img src="@images[currentIndex]" style="width: 100%; height: 100%;" />
    
    <button @onclick="PreviousSlide" style="position: absolute; left: 10px; top: 50%;">❮</button>
    <button @onclick="NextSlide" style="position: absolute; right: 10px; top: 50%;">❯</button>
</div>

@code {
    private List<string> images = new() { "img1.jpg", "img2.jpg", "img3.jpg" };
    private int currentIndex = 0;
    private System.Timers.Timer autoPlayTimer;
    
    protected override void OnInitialized()
    {
        autoPlayTimer = new System.Timers.Timer(3000);
        autoPlayTimer.Elapsed += (s, e) => NextSlide();
        autoPlayTimer.AutoReset = true;
        autoPlayTimer.Start();
    }
    
    private void NextSlide()
    {
        currentIndex = (currentIndex + 1) % images.Count;
        StateHasChanged();
    }
    
    private void PreviousSlide()
    {
        currentIndex = (currentIndex - 1 + images.Count) % images.Count;
        StateHasChanged();
    }
    
    public void Dispose() => autoPlayTimer?.Dispose();
}
```

**Key Property Mappings:**
| Web Forms | Blazor | Notes |
|-----------|--------|-------|
| `AutoPlay` | Timer initialization | Set up in `OnInitialized()` |
| `PlayInterval` | `Timer(interval)` | Pass milliseconds to Timer |
| `Loop` | Modulo logic | Use `%` operator for wrapping |

**Migration Notes:**
- Blazor components with state management replace the extender
- Use `System.Timers.Timer` for auto-play
- Implement Previous/Next buttons with index logic
- Consider CSS transitions for smooth image changes

---

### ListSearchExtender — Type-to-Filter Lists

**Purpose:** Filter list items by typing in a search box.

**Web Forms:**
```html
<asp:TextBox ID="searchBox" runat="server" />
<ajaxToolkit:ListSearchExtender ID="lse1" runat="server"
    TargetControlID="itemList"
    PromptPosition="Top" />

<asp:ListBox ID="itemList" runat="server">
    <asp:ListItem>Apple</asp:ListItem>
    <asp:ListItem>Banana</asp:ListItem>
    <asp:ListItem>Cherry</asp:ListItem>
</asp:ListBox>
```

**Blazor Equivalent (Data Binding + Filtering):**
```razor
<input type="text" @oninput="SearchItems" placeholder="Type to search..." />

<ul>
@foreach (var item in filteredItems)
{
    <li>@item</li>
}
</ul>

@code {
    private List<string> allItems = new() { "Apple", "Banana", "Cherry", "Date", "Elderberry" };
    private List<string> filteredItems;
    
    protected override void OnInitialized()
    {
        filteredItems = allItems;
    }
    
    private void SearchItems(ChangeEventArgs e)
    {
        string searchTerm = e.Value?.ToString() ?? "";
        filteredItems = allItems
            .Where(item => item.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}
```

**Key Property Mappings:**
| Web Forms | Blazor | Notes |
|-----------|--------|-------|
| `TargetControlID` | ListBox/dropdown items | Data-bind to filtered list |
| `PromptPosition` | CSS or HTML placement | Render input above/below list |

**Migration Notes:**
- Replace with simple C# filtering logic
- Use `@oninput` event for real-time search
- Use `string.Contains()` with case-insensitive comparison
- Consider using `System.Linq` for complex filtering

---

### BalloonPopupExtender — Styled Tooltips

**Purpose:** Display a styled balloon-style tooltip on hover.

**Web Forms:**
```html
<asp:Label ID="lblInfo" Text="Hover me" runat="server" />

<ajaxToolkit:BalloonPopupExtender ID="bpe1" runat="server"
    TargetControlID="lblInfo"
    BalloonPopupControlID="balloon"
    Position="TopRight"
    OffsetX="0"
    OffsetY="10" />

<asp:Panel ID="balloon" runat="server">
    This is the tooltip content
</asp:Panel>
```

**Blazor Equivalent (CSS + Hover):**
```razor
<div style="position: relative; display: inline-block;">
    <span @onmouseenter="ShowTooltip" @onmouseleave="HideTooltip">Hover me</span>
    
    @if (showTooltip)
    {
        <div style="@TooltipStyle()">
            This is the tooltip content
            <div style="position: absolute; bottom: -8px; left: 50%; transform: translateX(-50%); width: 0; height: 0; border-left: 8px solid transparent; border-right: 8px solid transparent; border-top: 8px solid #333;"></div>
        </div>
    }
</div>

@code {
    private bool showTooltip = false;
    
    private void ShowTooltip() => showTooltip = true;
    private void HideTooltip() => showTooltip = false;
    
    private string TooltipStyle() => "position: absolute; top: -50px; right: 0; background: #333; color: white; padding: 8px; border-radius: 4px; white-space: nowrap; z-index: 1000;";
}
```

**Key Property Mappings:**
| Web Forms | Blazor | Notes |
|-----------|--------|-------|
| `BalloonPopupControlID` | Inline div | Render tooltip inline |
| `Position` | CSS `top`/`right`/`bottom`/`left` | Control positioning |
| `OffsetX`/`OffsetY` | CSS positioning | Use `top`, `left` properties |

**Migration Notes:**
- Use `@onmouseenter` and `@onmouseleave` for hover events
- Style the balloon with CSS (border, background, shadow)
- Use CSS triangles (border tricks) for arrow/pointer
- Consider `title` attribute for simpler tooltips without custom styling

---

## See Also

- **[SKILL.md](SKILL.md)** — Web Forms → Blazor migration overview and two-layer pipeline
- **[CONTROL-REFERENCE.md](CONTROL-REFERENCE.md)** — All 58 BWFC controls and translation tables
- **[CODE-TRANSFORMS.md](CODE-TRANSFORMS.md)** — Code-behind lifecycle, event handlers, data binding
- **BlazorAjaxToolkitComponents GitHub:** <https://github.com/FritzAndFriends/BlazorAjaxToolkitComponents>
- **BlazorAjaxToolkitComponents NuGet:** <https://www.nuget.org/packages/BlazorAjaxToolkitComponents>
