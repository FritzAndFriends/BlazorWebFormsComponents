---
name: bwfc-ascx-migration
description: "Migrate ASP.NET Web Forms User Controls (.ascx) to Blazor components using BlazorWebFormsComponents. Covers ASCX-to-Razor conversion, code-behind preservation, tag prefix resolution, property/event mapping, and partial-class base class alignment. WHEN: 'migrate ascx', 'convert user control', 'ascx to blazor', 'user control migration'. FOR SINGLE OPERATIONS: use /bwfc-migration for full page migration, /bwfc-custom-control-migration for WebControl-based controls."
---

# ASCX User Control → Blazor Component Migration

## Overview

ASP.NET Web Forms User Controls (`.ascx` files) are reusable markup fragments with code-behind. They map directly to Blazor `.razor` components with `.razor.cs` code-behind files.

The `webforms-to-blazor` CLI tool handles L1 conversion automatically. This skill guides L2 repair for common patterns that require contextual understanding.

## How the CLI Handles ASCX Files

| Step | What Happens |
|------|--------------|
| 1. Directive removal | `<%@ Control ... %>` is stripped |
| 2. Markup transforms | `asp:` prefixes removed, expressions converted |
| 3. Code-behind preservation | Base class remapped, usings updated |
| 4. `@inherits` injection | Added to `.razor` when code-behind inherits a CustomControls type |
| 5. Output | `.razor` + `.razor.cs` pair in the output project |

## Critical Rules

### 1. Preserve the Code-Behind

ASCX code-behind files contain business logic that should compile unchanged. The CLI:
- Strips `System.Web.UI.*` usings
- Adds `using BlazorWebFormsComponents.CustomControls;`
- Preserves `UserControl` base class (now mapped to BWFC's `UserControl`)
- Injects `@inherits UserControl` into the `.razor` file

**Do NOT rewrite code-behind logic.** Fix compile errors by adding missing shims or stubs.

### 2. Properties Become Parameters

Web Forms exposes ASCX properties to parent pages. In Blazor, these become `[Parameter]` properties:

```csharp
// Web Forms (worked without attribute)
public string Title { get; set; }

// Blazor (requires [Parameter])
[Parameter]
public string Title { get; set; }
```

The CLI's `ParameterAttributeTransform` handles this automatically for public properties.

### 3. Events Become EventCallback

ASCX controls that raise events to parent pages:

```csharp
// Web Forms
public event EventHandler ItemSelected;
protected void OnItemSelected(EventArgs e) => ItemSelected?.Invoke(this, e);

// Blazor
[Parameter]
public EventCallback<EventArgs> ItemSelected { get; set; }
protected async Task OnItemSelected(EventArgs e) => await ItemSelected.InvokeAsync(e);
```

### 4. Tag Prefix Registration Is Eliminated

Web Forms requires tag prefix registration in `Web.config` or `<%@ Register %>`:

```xml
<!-- Web.config -->
<add tagPrefix="uc" src="~/Controls/StatusPanel.ascx" tagName="StatusPanel" />

<!-- Usage -->
<uc:StatusPanel runat="server" Title="Dashboard" />
```

In Blazor, components are referenced directly by name (no prefix, no registration):

```razor
<StatusPanel Title="Dashboard" />
```

The CLI strips `<%@ Register %>` directives and removes tag prefixes automatically.

### 5. Partial Class Base Must Match

The `.razor` file and `.razor.cs` file form a partial class. Their base class **must** agree:

```razor
@* In the .razor file *@
@inherits UserControl
```

```csharp
// In the .razor.cs file
public partial class MyControl : UserControl { }
```

If the CLI misses the `@inherits` directive, you'll get CS0263. Add it manually.

## Common L2 Repair Patterns

### Pattern: ASCX with LoadControl/Dynamic Loading

Web Forms can load user controls dynamically:

```csharp
var ctrl = (StatusPanel)LoadControl("~/Controls/StatusPanel.ascx");
ctrl.Title = "Dynamic";
PlaceHolder1.Controls.Add(ctrl);
```

**Blazor equivalent:** Use `RenderFragment` or `DynamicComponent`:

```razor
<DynamicComponent Type="typeof(StatusPanel)"
                  Parameters="@(new Dictionary<string, object> { ["Title"] = "Dynamic" })" />
```

### Pattern: ASCX with FindControl

Code-behind that uses `FindControl` to locate child controls works unchanged through the BWFC runtime:

```csharp
// This works in Blazor via BWFC's FindControl runtime
var lbl = (Label)FindControl("lblStatus");
lbl.Text = "Updated";
```

### Pattern: ASCX Exposing ChildControl Properties

Web Forms controls often wrap and expose child control properties:

```csharp
// Web Forms pattern
public string StatusText
{
    get { return lblStatus.Text; }
    set { lblStatus.Text = value; }
}
```

In Blazor, this still works if `lblStatus` is resolved via `FindControl` or `@ref`. The BWFC runtime supports both patterns.

### Pattern: Page_Load in User Controls

User control lifecycle methods auto-wire through BWFC's virtual methods:

```csharp
// Works unchanged — BaseWebFormsComponent provides virtual Page_Load
protected override void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        BindData();
    }
}
```

## File Structure After Migration

```
// Before (Web Forms)
Controls/
  StatusPanel.ascx          ← Markup
  StatusPanel.ascx.cs       ← Code-behind
  StatusPanel.ascx.designer.cs  ← Auto-generated (discarded)

// After (Blazor)
Components/Controls/        (or wherever the CLI places them)
  StatusPanel.razor         ← Converted markup + @inherits UserControl
  StatusPanel.razor.cs      ← Preserved code-behind (usings updated)
```

## Checklist

For each ASCX file in the migration:

- [ ] `.razor` file has `@inherits UserControl` (or appropriate base)
- [ ] `.razor` file has `@using BlazorWebFormsComponents.CustomControls`
- [ ] Code-behind compiles with updated usings
- [ ] Public properties have `[Parameter]` attribute
- [ ] Events converted to `EventCallback<T>`
- [ ] `Page_Load` / `Page_Init` overrides work (virtual methods)
- [ ] `FindControl` calls resolve at runtime
- [ ] Parent pages reference component by name (no tag prefix)
- [ ] Component renders expected HTML output

## Reference

- [Custom WebControl Migration Guide](../../docs/Migration/CustomWebControl.md)
- [FindControl Migration Guide](../../docs/Migration/FindControl-Migration.md)
- [User Controls Migration Guide](../../docs/Migration/User-Controls.md)
- [BWFC CustomControls namespace](../../src/BlazorWebFormsComponents/CustomControls/)
