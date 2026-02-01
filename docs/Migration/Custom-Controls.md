# Migrating Custom Controls to Blazor

When migrating ASP.NET Web Forms applications to Blazor, you'll often encounter custom controls that inherit from `WebControl` or `CompositeControl`. The BlazorWebFormsComponents library provides shim classes that allow you to migrate these controls with minimal code changes.

## Overview

In Web Forms, custom controls are typically created by:
1. Inheriting from `WebControl` for simple controls with custom HTML rendering
2. Inheriting from `CompositeControl` for controls that contain child controls
3. Using `HtmlTextWriter` to generate HTML output

The BlazorWebFormsComponents library provides Blazor-compatible versions of these classes in the `BlazorWebFormsComponents.CustomControls` namespace.

## HtmlTextWriter

The `HtmlTextWriter` class provides a familiar API for rendering HTML, similar to the Web Forms version. It buffers HTML output that is then converted to Blazor's render tree.

### Key Methods

- `Write(string)` - Writes text to the output
- `WriteLine(string)` - Writes text followed by a line terminator
- `AddAttribute(string, string)` - Adds an attribute to the next tag
- `AddStyleAttribute(string, string)` - Adds a CSS style to the next tag  
- `RenderBeginTag(string)` - Opens an HTML tag
- `RenderEndTag()` - Closes the current HTML tag

### Supported Enums

- `HtmlTextWriterTag` - Common HTML tags (Div, Span, Button, etc.)
- `HtmlTextWriterAttribute` - Common HTML attributes (Id, Class, Style, etc.)
- `HtmlTextWriterStyle` - Common CSS styles (Color, Width, Height, etc.)

## WebControl Base Class

The `WebControl` class provides a base for simple custom controls. It inherits from `BaseStyledComponent`, giving you access to standard styling properties.

### Migration Example

**Original Web Forms Control:**

```csharp
using System.Web.UI;
using System.Web.UI.WebControls;

public class HelloLabel : WebControl
{
    public string Text { get; set; }
    public string Prefix { get; set; }

    protected override void Render(HtmlTextWriter writer)
    {
        writer.RenderBeginTag(HtmlTextWriterTag.Span);
        writer.Write($"{Prefix} {Text}");
        writer.RenderEndTag();
    }
}
```

**Blazor Version:**

```csharp
using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

public class HelloLabel : WebControl
{
    [Parameter]
    public string Text { get; set; }

    [Parameter]
    public string Prefix { get; set; }

    protected override void Render(HtmlTextWriter writer)
    {
        AddBaseAttributes(writer);  // Includes ID, CssClass, Style from base
        writer.RenderBeginTag(HtmlTextWriterTag.Span);
        writer.Write($"{Prefix} {Text}");
        writer.RenderEndTag();
    }
}
```

### Key Differences

1. **Add `[Parameter]` attributes** to public properties that should accept values from markup
2. **Call `AddBaseAttributes(writer)`** before `RenderBeginTag` to include base styling properties (ID, CssClass, Style)
3. **No `runat="server"`** needed in Blazor markup

### Usage in Blazor

```razor
<HelloLabel Text="World" Prefix="Hello" CssClass="greeting" />
```

Renders as:
```html
<span class="greeting">Hello World</span>
```

## CompositeControl Base Class

The `CompositeControl` class is for controls that contain child controls. It extends `WebControl` and provides child control management.

### Migration Example

**Original Web Forms Control:**

```csharp
using System.Web.UI;
using System.Web.UI.WebControls;

public class SearchBox : CompositeControl
{
    protected override void CreateChildControls()
    {
        Label label = new Label { Text = "Search:" };
        TextBox textBox = new TextBox { ID = "searchQuery" };
        Button button = new Button { Text = "Go" };

        Controls.Add(label);
        Controls.Add(textBox);
        Controls.Add(button);
    }

    protected override void Render(HtmlTextWriter writer)
    {
        writer.AddAttribute(HtmlTextWriterAttribute.Class, "search-box");
        writer.RenderBeginTag(HtmlTextWriterTag.Div);
        RenderChildren(writer);
        writer.RenderEndTag();
    }
}
```

**Blazor Version:**

For composite controls in Blazor, you have two approaches:

### Approach 1: Using WebControl Children (Recommended for simple cases)

```csharp
using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

// Simple label control
public class SimpleLabel : WebControl
{
    [Parameter]
    public string Text { get; set; }

    protected override void Render(HtmlTextWriter writer)
    {
        writer.RenderBeginTag(HtmlTextWriterTag.Label);
        writer.Write(Text);
        writer.RenderEndTag();
    }
}

// Simple button control  
public class SimpleButton : WebControl
{
    [Parameter]
    public string Text { get; set; }

    protected override void Render(HtmlTextWriter writer)
    {
        writer.RenderBeginTag(HtmlTextWriterTag.Button);
        writer.Write(Text);
        writer.RenderEndTag();
    }
}

// Composite control
public class SearchBox : CompositeControl
{
    protected override void CreateChildControls()
    {
        var label = new SimpleLabel { Text = "Search:" };
        var button = new SimpleButton { Text = "Go" };

        Controls.Add(label);
        Controls.Add(button);
    }

    protected override void Render(HtmlTextWriter writer)
    {
        AddBaseAttributes(writer);
        writer.AddAttribute(HtmlTextWriterAttribute.Class, "search-box");
        writer.RenderBeginTag(HtmlTextWriterTag.Div);
        RenderChildren(writer);
        writer.RenderEndTag();
    }
}
```

### Approach 2: Using Native Blazor Components (Recommended for complex cases)

For more complex scenarios, consider converting to a pure Blazor component:

```razor
@* SearchBox.razor *@
@inherits BaseStyledComponent

<div class="@CalculatedCssClass">
    <label>Search:</label>
    <input type="text" @bind="SearchQuery" />
    <button @onclick="OnSearchClick">Go</button>
</div>

@code {
    [Parameter]
    public string SearchQuery { get; set; }

    [Parameter]
    public EventCallback<string> OnSearch { get; set; }

    private async Task OnSearchClick()
    {
        await OnSearch.InvokeAsync(SearchQuery);
    }

    private string CalculatedCssClass => CssClassBuilder.Default("search-box")
        .AddClass(CssClass, when: !string.IsNullOrEmpty(CssClass))
        .Build();
}
```

## Best Practices

### 1. Use AddBaseAttributes() Consistently

Always call `AddBaseAttributes(writer)` before your first `RenderBeginTag()` to ensure that base styling properties (ID, CssClass, Style) are applied:

```csharp
protected override void Render(HtmlTextWriter writer)
{
    AddBaseAttributes(writer);  // Always first!
    writer.RenderBeginTag(HtmlTextWriterTag.Div);
    // ... rest of rendering
    writer.RenderEndTag();
}
```

### 2. Class Attribute Concatenation

The `HtmlTextWriter` automatically concatenates multiple class attributes:

```csharp
AddBaseAttributes(writer);  // Adds user's CssClass if present
writer.AddAttribute(HtmlTextWriterAttribute.Class, "my-control");  // Concatenated
writer.RenderBeginTag(HtmlTextWriterTag.Div);
// Results in: <div class="user-class my-control">
```

### 3. Mark Properties with [Parameter]

Don't forget to add `[Parameter]` to properties that should be settable from markup:

```csharp
[Parameter]  // Required for Blazor parameter binding
public string Text { get; set; }
```

### 4. Consider Pure Blazor for New Development

While the custom control shims are useful for migration, consider using pure Blazor components for new development:

- Better performance (no HTML string generation)
- Better tooling support
- More idiomatic Blazor code
- Easier to maintain

### 5. Child Control Limitations

The `CompositeControl.RenderChildren()` method only works with child controls that inherit from `WebControl`. For standard Blazor components, use the pure Blazor approach (Approach 2 above).

## Migration Strategy

1. **Start with simple controls** - Migrate WebControl-based controls first as they're simpler
2. **Test incrementally** - Migrate and test one control at a time
3. **Consider refactoring** - If a control is complex, consider rewriting it as a pure Blazor component
4. **Update child controls first** - For composite controls, migrate child controls before the parent
5. **Validate HTML output** - Compare rendered HTML between Web Forms and Blazor versions

## Common Pitfalls

### Missing Service Registration

Custom controls inherit from `BaseWebFormsComponent` which requires certain services. Make sure your Blazor app registers them:

```csharp
// In Program.cs
builder.Services.AddRouting();  // Provides LinkGenerator
builder.Services.AddHttpContextAccessor();  // Provides IHttpContextAccessor
```

For testing environments, you can use mocks:

```csharp
// In test setup
Services.AddSingleton<LinkGenerator>(new Mock<LinkGenerator>().Object);
Services.AddSingleton<IHttpContextAccessor>(new Mock<IHttpContextAccessor>().Object);
```

### Forgetting AddBaseAttributes

If your migrated controls don't respect `CssClass`, `ID`, or `Style` properties, you likely forgot to call `AddBaseAttributes(writer)`.

### ViewState Dependencies

Web Forms ViewState is syntax-only in Blazor. If your control relies heavily on ViewState, you'll need to refactor to use:
- Component parameters
- Blazor state management (@bind, cascading values)
- Browser storage (localStorage, sessionStorage)

## Examples in the Test Suite

The BlazorWebFormsComponents test suite includes several example custom controls:

- `HelloLabel` - Simple text rendering
- `CustomButton` - Button with custom attributes
- `StyledDiv` - Control with inline styles
- `SearchBox` - Composite control with multiple children
- `FormGroup` - Composite control with conditional children

See: `/src/BlazorWebFormsComponents.Test/CustomControls/TestComponents/`

## Further Reading

- [BaseWebFormsComponent API](../UtilityFeatures/IDRendering.md)
- [Blazor Component Basics](https://docs.microsoft.com/aspnet/core/blazor/components/)
- [Migration Strategies](Strategies.md)

