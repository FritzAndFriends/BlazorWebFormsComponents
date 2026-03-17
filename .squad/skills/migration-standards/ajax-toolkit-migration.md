---
name: "Ajax Control Toolkit Migration — L1 Automation"
description: "L1 script handling for Ajax Control Toolkit migration during Layer 1 conversion"
domain: "migration"
confidence: "high"
source: "earned"
---

## Overview

This document describes how the L1 migration script (`bwfc-migrate.ps1`) handles **Ajax Control Toolkit** (ACT) components during automated migration. It is a companion to the main [`SKILL.md`](SKILL.md) migration standards document.

For comprehensive, per-component migration guidance and L2 manual work, see:
- **[Ajax Control Toolkit Overview](../../docs/AjaxToolkit/index.md)** — Component reference and usage patterns
- **[Ajax Control Toolkit Migration Guide](../../docs/AjaxToolkit/migration-guide.md)** — Step-by-step migration walkthrough with before/after examples

---

## What is the Ajax Control Toolkit?

The **Ajax Control Toolkit** (from the DevExpress Ajax Control Toolkit NuGet package) provides a set of client-side enhancement components for ASP.NET Web Forms:

- **Extender controls** — Non-rendering components that attach JavaScript behavior to existing controls (e.g., `<ajaxToolkit:ConfirmButtonExtender>`)
- **Container controls** — Rendering components that hold child content (e.g., `<ajaxToolkit:Accordion>`, `<ajaxToolkit:TabContainer>`)

These controls are identified by the `ajaxToolkit:` XML namespace prefix.

---

## Detection

When auditing a Web Forms source for Ajax Control Toolkit usage, look for:

### 1. Register Directive

```aspx
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
```

This declaration binds the `ajaxToolkit:` prefix to the Ajax Control Toolkit assembly.

### 2. ToolkitScriptManager

```aspx
<ajaxToolkit:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" />
```

This control is a special script manager required by Web Forms to load Ajax Control Toolkit JavaScript. It must be removed during migration (Blazor handles script loading natively).

### 3. Control Usage

Extender examples:
- `<ajaxToolkit:ConfirmButtonExtender TargetControlID="btnDelete" ConfirmText="Are you sure?" />`
- `<ajaxToolkit:FilteredTextBoxExtender TargetControlID="txtPhone" FilterType="Numbers" />`
- `<ajaxToolkit:ModalPopupExtender TargetControlID="btnOpen" PopupControlID="pnlModal" />`

Container examples:
- `<ajaxToolkit:Accordion ID="acc1" runat="server">...` with `<ajaxToolkit:AccordionPane>` children
- `<ajaxToolkit:TabContainer ID="tabs1" runat="server">...` with `<ajaxToolkit:TabPanel>` children

### Supported Controls

The L1 script has built-in support for 14 components:

| Component | Type | Layer 1 Behavior |
|---|---|---|
| `Accordion` | Container | Strip prefix → `<Accordion>` |
| `AccordionPane` | Container | Strip prefix → `<AccordionPane>` |
| `AutoCompleteExtender` | Extender | Strip prefix → `<AutoCompleteExtender>` |
| `CalendarExtender` | Extender | Strip prefix → `<CalendarExtender>` |
| `CollapsiblePanelExtender` | Extender | Strip prefix → `<CollapsiblePanelExtender>` |
| `ConfirmButtonExtender` | Extender | Strip prefix → `<ConfirmButtonExtender>` |
| `FilteredTextBoxExtender` | Extender | Strip prefix → `<FilteredTextBoxExtender>` |
| `HoverMenuExtender` | Extender | Strip prefix → `<HoverMenuExtender>` |
| `MaskedEditExtender` | Extender | Strip prefix → `<MaskedEditExtender>` |
| `ModalPopupExtender` | Extender | Strip prefix → `<ModalPopupExtender>` |
| `NumericUpDownExtender` | Extender | Strip prefix → `<NumericUpDownExtender>` |
| `PopupControlExtender` | Extender | Strip prefix → `<PopupControlExtender>` |
| `SliderExtender` | Extender | Strip prefix → `<SliderExtender>` |
| `TabContainer` | Container | Strip prefix → `<TabContainer>` |
| `TabPanel` | Container | Strip prefix → `<TabPanel>` |
| `ToggleButtonExtender` | Extender | Strip prefix → `<ToggleButtonExtender>` |
| `ToolkitScriptManager` | Special | Remove entirely (not needed in Blazor) |

---

## Layer 1 (Automated) Script Handling

The L1 script (`bwfc-migrate.ps1`) applies the following transforms in the `ConvertFrom-AjaxToolkitPrefix` function:

### 1. Remove ToolkitScriptManager

**Transform:** Removes all occurrences of `<ajaxToolkit:ToolkitScriptManager>` (both block and self-closing forms).

```diff
- <ajaxToolkit:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" />
```

**Logging:** Logs `AjaxToolkit` transform: `"Removed <ajaxToolkit:ToolkitScriptManager> (not needed in Blazor)"`

### 2. Strip Prefix on Known Controls

**Transform:** Converts `<ajaxToolkit:*>` to `<*>` for all 16 known components (listed above).

```diff
- <ajaxToolkit:ConfirmButtonExtender TargetControlID="btnDelete" ConfirmText="Are you sure?" />
+ <ConfirmButtonExtender TargetControlID="btnDelete" ConfirmText="Are you sure?" />
```

**Preserve:** All attributes and properties remain unchanged:
- `TargetControlID` stays as-is
- `ConfirmText`, `OnCancelScript`, etc. unchanged
- Nested elements (e.g., `<AccordionPane>` inside `<Accordion>`) preserved

**Logging:** Logs `AjaxToolkit` transform with match count per file (e.g., `"Converted ajaxToolkit: prefix on 3 opening tag(s)"`)

### 3. Handle Unrecognized Controls → TODO Comment

**Transform:** Any `<ajaxToolkit:UnknownControl>` (not in the 16 known list) is replaced with a TODO comment.

```diff
- <ajaxToolkit:CustomExtender TargetControlID="ctrl1" />
+ @* TODO: Convert ajaxToolkit:CustomExtender — no BWFC equivalent yet *@
```

**Logging:** 
- Logs manual item in category `AjaxToolkit-Unknown`
- Logs `AjaxToolkit` transform: `"Replaced N unrecognized ajaxToolkit control(s) with TODO"`

---

## Blazor Project Setup for Ajax Control Toolkit

After Layer 1 conversion, the migrated Blazor project must be configured to support Ajax Control Toolkit components:

### 1. Add NuGet Package

```bash
dotnet add package BlazorAjaxToolkitComponents
```

**Why:** This package provides the Blazor implementations of all 16 components, plus JavaScript interop modules for extenders.

### 2. Update `_Imports.razor`

Add these `@using` directives:

```razor
@using BlazorAjaxToolkitComponents
@using BlazorAjaxToolkitComponents.Enums
```

**Why:** Makes component names and enum types available without fully qualifying them (e.g., `<Accordion>` instead of `<BlazorAjaxToolkitComponents.Accordion>`).

### 3. Set Render Mode

All pages/components using Ajax Control Toolkit extenders must use `@rendermode InteractiveServer`:

```razor
@rendermode InteractiveServer
```

**Why:** Extenders depend on JavaScript interoperability via `IJSRuntime`. Static render modes (SSR or Static) do not support this. Containers (Accordion, TabContainer) also require InteractiveServer for client-side tab switching.

**Graceful Degradation:** If an extender is accidentally placed in a Static component, it silently skips initialization (no exceptions thrown).

### 4. No Manual Script Tags Required

**Note:** Do NOT add manual `<script>` tags for Ajax Control Toolkit JavaScript. The BWFC components load their JS modules automatically via ES module imports from `_content/BlazorAjaxToolkitComponents/js/`.

---

## Migration Example

### Before (Web Forms with Ajax Control Toolkit)

```aspx
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
...
<ajaxToolkit:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" />

<asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="Delete_Click" />
<ajaxToolkit:ConfirmButtonExtender 
    ID="ext1" runat="server" 
    TargetControlID="btnDelete" 
    ConfirmText="Are you sure you want to delete this item?" />
```

### After Layer 1 (Script Conversion)

```razor
@* Register directive removed; ToolkitScriptManager removed *@

<Button ID="btnDelete" Text="Delete" OnClick="Delete_Click" />
<ConfirmButtonExtender 
    TargetControlID="btnDelete" 
    ConfirmText="Are you sure you want to delete this item?" />
```

**What Changed:**
- ✅ `<ajaxToolkit:ToolkitScriptManager>` removed entirely
- ✅ `<asp:Button>` → `<Button>` (standard BWFC conversion)
- ✅ `<ajaxToolkit:ConfirmButtonExtender>` → `<ConfirmButtonExtender>` (prefix stripped)
- ✅ All attributes preserved (`TargetControlID`, `ConfirmText`)

### Blazor Project Setup

The migrated project's `_Imports.razor` must include:

```razor
@using BlazorWebFormsComponents
@using BlazorAjaxToolkitComponents
@using BlazorAjaxToolkitComponents.Enums
@using static Microsoft.AspNetCore.Components.Web.RenderMode
@inherits WebFormsPageBase
```

And the page itself must have:

```razor
@page "/some-page"
@rendermode InteractiveServer
```

---

## TargetControlID Resolution

Extenders locate their target control by matching the `TargetControlID` property against the target control's `ID` attribute:

1. **Rendering phase:** The target control (e.g., `<Button ID="btnDelete">`) renders to HTML with an `id` attribute (e.g., `<button id="btnDelete">`).
2. **JavaScript initialization:** After first render, the extender's JavaScript calls `document.getElementById(TargetControlID)` to find the DOM element.
3. **Enhancement:** If found, the extender attaches event listeners or other behavior to that element.

**BWFC controls handle this automatically** — when you set `ID="btnDelete"` on a Button component, it renders to a `<button id="btnDelete">` HTML element. Extenders find it via standard DOM lookup.

**Key requirement:** The target control must have an `ID` attribute. Extenders without valid targets will fail silently (no JavaScript errors; the behavior simply doesn't initialize).

---

## What's NOT Supported

If your Web Forms application uses Ajax Control Toolkit components **not in the 16-component list above**, the Layer 1 script will flag them as TODO items and log them as manual work:

**Example:**
```aspx
<ajaxToolkit:CustomRichTextEditor TargetControlID="txt1" />
```

**Layer 1 Output:**
```razor
@* TODO: Convert ajaxToolkit:CustomRichTextEditor — no BWFC equivalent yet *@
```

**Manual Item Logged:** Category `AjaxToolkit-Unknown`, details: `"Unrecognized ajaxToolkit control: CustomRichTextEditor — no BWFC equivalent available"`

**Layer 2 Action:** You will need to manually replace unsupported controls with alternative approaches (custom Blazor components, third-party components, or plain HTML + JavaScript).

---

## Link to Comprehensive Documentation

For detailed usage, properties, examples, and troubleshooting for each component:

- **[Ajax Control Toolkit Overview](../../docs/AjaxToolkit/index.md)** — Component catalog and render mode requirements
- **[Ajax Control Toolkit Migration Guide](../../docs/AjaxToolkit/migration-guide.md)** — Complete migration walkthrough with 14 before/after component examples
- **Per-component docs** (e.g., `ConfirmButtonExtender.md`, `ModalPopupExtender.md`, etc.) — Detailed properties, enums, realistic code examples

---

## Troubleshooting

### Extender Not Initializing After Migration

**Symptoms:** Extender renders without JavaScript errors, but behavior doesn't work (e.g., click doesn't show confirmation dialog).

**Common causes:**
1. Missing `@rendermode InteractiveServer` on the page
2. `TargetControlID` doesn't match the target control's `ID` attribute
3. Browser console shows module loading error (check network tab)

**Fix:** Verify render mode is set and TargetControlID is correct. See [Ajax Control Toolkit Index](../../docs/AjaxToolkit/index.md) troubleshooting section.

### Unrecognized Ajax Control Toolkit Control

**Symptoms:** Layer 1 logs `AjaxToolkit-Unknown` manual item.

**Cause:** You are using an Ajax Control Toolkit component that BWFC doesn't support (not in the 16-component list).

**Fix:** Replace the control manually in Layer 2 with a supported alternative or custom Blazor component.

---

## See Also

- [Main Migration Standards (SKILL.md)](SKILL.md) — Core migration patterns and architecture
- [Ajax Control Toolkit Migration Guide](../../docs/AjaxToolkit/migration-guide.md) — Comprehensive per-component walkthrough
- [Ajax Control Toolkit Overview](../../docs/AjaxToolkit/index.md) — Component reference, render mode requirements, troubleshooting
