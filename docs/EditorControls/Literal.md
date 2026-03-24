# Literal

The **Literal** component renders text or HTML content directly to the page without any wrapping element. It emulates the `asp:Literal` control, providing a way to output dynamic content with control over how that content is encoded. Unlike the [Label](Label.md) component, Literal does not render a `<span>` or any other container element.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.literal?view=netframework-4.8

## Features Supported in Blazor

- `Text` - The text or HTML content to render
- `Mode` - Specifies how the content is rendered:
    - `LiteralMode.PassThrough` - Content is rendered as-is (raw HTML)
    - `LiteralMode.Encode` - Content is HTML-encoded before rendering (safe for user input)
    - `LiteralMode.Transform` - Content is rendered with device-specific transformations (default)

## Web Forms Features NOT Supported

- **EnableTheming / SkinID** - ASP.NET theming not available in Blazor
- **EnableViewState** - Not needed; Blazor manages state differently
- **Visible** - Use conditional rendering with `@if` instead
- **Lifecycle events** (`OnDataBinding`, `OnInit`, `OnLoad`, etc.) - Use Blazor lifecycle methods instead

## Syntax Comparison

=== "Web Forms (Before)"

    ```html
    <asp:Literal
        EnableTheming="True|False"
        EnableViewState="True|False"
        ID="string"
        Mode="Transform|PassThrough|Encode"
        OnDataBinding="DataBinding event handler"
        OnDisposed="Disposed event handler"
        OnInit="Init event handler"
        OnLoad="Load event handler"
        OnPreRender="PreRender event handler"
        OnUnload="Unload event handler"
        runat="server"
        SkinID="string"
        Text="string"
        Visible="True|False"
    />
    ```

=== "Blazor (After)"

    ```razor
    <Literal Text="Hello, World!" />
    ```

!!! note "Key Difference"
    The Literal component renders content without any wrapping HTML element, just like the Web Forms version. Use `Mode` to control encoding: `Encode` for user-supplied text (XSS protection), `PassThrough` for trusted HTML content.

## Blazor Examples

### Basic Text

```razor
<Literal Text="Welcome to our site!" />
```

### With Encoded Mode (Safe for User Input)

```razor
<Literal Text="@userInput" Mode="LiteralMode.Encode" />

@code {
    private string userInput = "<script>alert('xss')</script>";
    // Renders as: &lt;script&gt;alert('xss')&lt;/script&gt;
}
```

### With PassThrough Mode (Raw HTML)

```razor
<Literal Text="@htmlContent" Mode="LiteralMode.PassThrough" />

@code {
    private string htmlContent = "<strong>Bold text</strong> and <em>italic text</em>";
}
```

### Dynamic Content

```razor
<Literal Text="@greeting" />

@code {
    private string greeting = "";

    protected override void OnInitialized()
    {
        greeting = $"Today is {DateTime.Now:dddd, MMMM d, yyyy}";
    }
}
```

## HTML Output

**Blazor Input:**
```razor
<Literal Text="Hello <b>World</b>" Mode="LiteralMode.PassThrough" />
```

**Rendered HTML (PassThrough):**
```html
Hello <b>World</b>
```

**Blazor Input:**
```razor
<Literal Text="Hello <b>World</b>" Mode="LiteralMode.Encode" />
```

**Rendered HTML (Encode):**
```html
Hello &lt;b&gt;World&lt;/b&gt;
```

## Migration Notes

When migrating from Web Forms to Blazor:

1. **Remove `asp:` prefix** - Change `<asp:Literal>` to `<Literal>`
2. **Remove `runat="server"`** - Not needed in Blazor
3. **Keep the `Mode` property** - It works the same way: `Encode`, `PassThrough`, and `Transform`
4. **Replace `Visible="false"`** - Use `@if` conditional rendering instead
5. **Data binding** - Replace `<%# Eval("Field") %>` with `@variable` Razor expressions

### Before / After

=== "Web Forms (Before)"

    ```html
    <asp:Literal ID="litMessage" runat="server"
        Text="Welcome back!" Mode="Encode" />
    ```

=== "Blazor (After)"

    ```razor
    <Literal Text="@message" Mode="LiteralMode.Encode" />

    @code {
        private string message = "Welcome back!";
    }
    ```

!!! tip "Migration Tip"
    In pure Blazor, you can often replace `<Literal>` with direct Razor expressions like `@myVariable` or `@((MarkupString)htmlContent)`. Use the Literal component when migrating existing markup to minimize changes, or when you need the `Mode` property for encoding control.

## See Also

- [Label](Label.md) - Renders text inside a `<span>` element
- [PlaceHolder](../LayoutControls/PlaceHolder.md) - Container for dynamically added content
