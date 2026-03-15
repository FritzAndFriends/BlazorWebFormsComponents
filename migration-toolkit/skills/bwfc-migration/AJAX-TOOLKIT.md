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

## See Also

- **[SKILL.md](SKILL.md)** — Web Forms → Blazor migration overview and two-layer pipeline
- **[CONTROL-REFERENCE.md](CONTROL-REFERENCE.md)** — All 58 BWFC controls and translation tables
- **[CODE-TRANSFORMS.md](CODE-TRANSFORMS.md)** — Code-behind lifecycle, event handlers, data binding
- **BlazorAjaxToolkitComponents GitHub:** <https://github.com/FritzAndFriends/BlazorAjaxToolkitComponents>
- **BlazorAjaxToolkitComponents NuGet:** <https://www.nuget.org/packages/BlazorAjaxToolkitComponents>
