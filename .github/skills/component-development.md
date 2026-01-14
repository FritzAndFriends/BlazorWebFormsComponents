---
description: Guidance for creating Blazor components that emulate ASP.NET Web Forms controls
applyTo: "**/src/BlazorWebFormsComponents/**"
tags:
  - blazor
  - components
  - web-forms
  - migration
---

# Component Development Skill for BlazorWebFormsComponents

This skill provides guidance for creating new Blazor components that emulate ASP.NET Web Forms controls. Use this when implementing new components or extending existing ones.

## Core Requirement

**Every component must faithfully recreate its Web Forms counterpart:**
1. **Same name** as the original Web Forms control
2. **Same attributes/properties** where possible
3. **Identical HTML output** to preserve CSS and JavaScript compatibility

## Before You Start

1. Find the original Web Forms control documentation:
   - https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.[controlname]?view=netframework-4.8
2. Identify the HTML output the control renders
3. List all attributes/properties from the declarative syntax
4. Determine which features can/cannot be supported in Blazor

## File Structure

Every component needs at minimum two files:

```
/src/BlazorWebFormsComponents/
  ComponentName.razor        # Markup template
  ComponentName.razor.cs     # Code-behind partial class
```

## Choosing the Right Base Class

### Simple Components (no styling)
```csharp
public partial class Label : BaseWebFormsComponent, ITextComponent
```
Use for: Literal, HiddenField, simple containers

### Styled Components (with CSS properties)
```csharp
public partial class Button : BaseStyledComponent
```
Use for: Button, Image, HyperLink, most visible controls

### Data-Bound Components
```csharp
public partial class GridView<ItemType> : DataBoundComponent<ItemType>
```
Use for: GridView, ListView, Repeater, DataList

### Validation Components
```csharp
public abstract partial class BaseValidator<Type> : BaseStyledComponent
```
Use for: RequiredFieldValidator, RegularExpressionValidator, etc.

## Component Code-Behind Template

```csharp
using Microsoft.AspNetCore.Components;
using System;

namespace BlazorWebFormsComponents
{
    public partial class ComponentName : BaseStyledComponent
    {
        #region Obsolete Web Forms Properties

        [Parameter, Obsolete("In Blazor [Feature] is not supported")]
        public string UnsupportedProperty { get; set; }

        #endregion

        #region Supported Properties

        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public string CssClass { get; set; }

        #endregion

        #region Calculated Properties

        internal string CalculatedCssClass => Enabled
            ? CssClass
            : string.Concat(CssClass, " aspNetDisabled").Trim();

        #endregion

        #region Events

        [Parameter]
        public EventCallback<EventArgs> OnClick { get; set; }

        protected void HandleClick()
        {
            OnClick.InvokeAsync(EventArgs.Empty);
        }

        #endregion
    }
}
```

## Component Razor Template

```razor
@inherits BaseStyledComponent

@if (Visible)
{
    <element-name
        class="@CalculatedCssClass"
        style="@Style"
        disabled="@(Enabled ? null : "disabled")"
        @onclick="HandleClick">
        @Text
    </element-name>
}
```

### Key Patterns

1. **Always wrap in `@if (Visible)`** - All Web Forms controls have Visible property
2. **Use `@inherits`** - Point to appropriate base class
3. **Conditional disabled** - Use `@(Enabled ? null : "disabled")` pattern
4. **Style binding** - Use `style="@Style"` from BaseStyledComponent

## Property Naming Conventions

| Pattern | Example | Usage |
|---------|---------|-------|
| Direct match | `Text`, `CssClass` | Properties that work identically |
| `Calculated*` | `CalculatedCssClass` | Computed properties for rendering |
| `On*` | `OnClick`, `OnCommand` | Event callbacks |

## Handling Obsolete Features

Mark unsupported Web Forms features as obsolete with helpful messages:

```csharp
/// <summary>
/// PostBackUrl is not supported in Blazor - use NavigationManager instead
/// </summary>
[Parameter, Obsolete("In Blazor PostBackUrl is not supported - use NavigationManager")]
public string PostBackUrl { get; set; }
```

### Common Obsolete Properties (already in base classes)

These are handled by `BaseWebFormsComponent`:
- `ID` → Use `@ref` instead
- `runat` → Not needed in Blazor
- `EnableViewState` → Syntax-only, does nothing
- `EnableTheming` → Not available in Blazor
- `SkinID` → Not available in Blazor
- `DataKeys` → Not used in Blazor

## Event Handling Patterns

### Simple Click Event
```csharp
[Parameter]
public EventCallback<MouseEventArgs> OnClick { get; set; }

protected void Click()
{
    OnClick.InvokeAsync(new MouseEventArgs());
}
```

### Command Event with Bubbling
```csharp
[Parameter]
public string CommandName { get; set; }

[Parameter]
public object CommandArgument { get; set; }

[Parameter]
public EventCallback<CommandEventArgs> OnCommand { get; set; }

protected void Click()
{
    if (!string.IsNullOrEmpty(CommandName))
    {
        var args = new CommandEventArgs(CommandName, CommandArgument);
        OnCommand.InvokeAsync(args);
        OnBubbledEvent(this, args);  // Bubble to parent
    }
    else
    {
        OnClick.InvokeAsync(new MouseEventArgs());
    }
}
```

## Style Implementation

### For Styled Components
BaseStyledComponent provides the `Style` property automatically:

```csharp
// In BaseStyledComponent
protected string Style => this.ToStyle().Build().NullIfEmpty();
```

### Custom Style Building
```csharp
protected StyleBuilder CalculatedStyle => this.ToStyle()
    .AddStyle("custom-property", "value", when: SomeCondition);
```

## Data-Bound Component Pattern

```razor
@typeparam ItemType
@inherits DataBoundComponent<ItemType>

@if (Items != null && Items.Any())
{
    @foreach (ItemType item in Items)
    {
        @ItemTemplate(item)
    }
}
else
{
    @EmptyDataTemplate
}
```

```csharp
public partial class Repeater<ItemType> : DataBoundComponent<ItemType>
{
    [Parameter]
    public RenderFragment<ItemType> ItemTemplate { get; set; }

    [Parameter]
    public RenderFragment<ItemType> AlternatingItemTemplate { get; set; }

    [Parameter]
    public RenderFragment HeaderTemplate { get; set; }

    [Parameter]
    public RenderFragment FooterTemplate { get; set; }

    [Parameter]
    public RenderFragment SeparatorTemplate { get; set; }
}
```

## Implementing Interfaces

Use interfaces for components that share behaviors:

```csharp
// For components with Text property
public partial class Label : BaseWebFormsComponent, ITextComponent

// For button-like components
public partial class Button : BaseStyledComponent, IButtonComponent

// For image components
public partial class Image : BaseStyledComponent, IImageComponent
```

## Validation Component Pattern

```csharp
public class RequiredFieldValidator : BaseValidator<string>
{
    [Parameter]
    public string InitialValue { get; set; } = string.Empty;

    public override bool Validate(string value)
    {
        return !string.IsNullOrEmpty(value) && value != InitialValue;
    }
}
```

## Checklist for New Components

- [ ] Created `ComponentName.razor` with proper `@inherits`
- [ ] Created `ComponentName.razor.cs` partial class
- [ ] Inherited from appropriate base class
- [ ] Added all supported Web Forms properties as `[Parameter]`
- [ ] Marked unsupported properties with `[Obsolete]`
- [ ] HTML output matches original Web Forms control
- [ ] Wrapped content in `@if (Visible)` check
- [ ] Used `CalculatedCssClass` pattern for disabled state
- [ ] Added unit tests in `/src/BlazorWebFormsComponents.Test/ComponentName/`
- [ ] Added sample page in `/samples/AfterBlazorServerSide/Pages/ControlSamples/`
- [ ] Created documentation in `/docs/` folder
- [ ] Updated `mkdocs.yml` navigation

## HTML Output Reference

Always verify your component renders the same HTML as Web Forms:

| Control | Web Forms Output |
|---------|------------------|
| Button | `<input type="submit">` or `<button>` |
| LinkButton | `<a href="javascript:__doPostBack(...)">` → `<a @onclick>` |
| HyperLink | `<a href="...">` |
| Image | `<img src="...">` |
| Label | `<span>text</span>` |
| Literal | Raw text (no wrapper) |
| Panel | `<div>` |
| GridView | `<table>` with thead/tbody |
| Repeater | No wrapper, just repeated content |

## Common Pitfalls

1. **Forgetting `Visible` check** - Always wrap in `@if (Visible)`
2. **Wrong disabled pattern** - Use `disabled="@(Enabled ? null : "disabled")"` not `disabled="@(!Enabled)"`
3. **Missing namespace** - All components must be in `BlazorWebFormsComponents` namespace
4. **Incorrect HTML** - Test against actual Web Forms output
5. **Missing event bubbling** - Command events should call `OnBubbledEvent`
