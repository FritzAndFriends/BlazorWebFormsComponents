# Label

The **Label** component displays static text on a web page. It may seem strange that we have a Label component when there already is an HTML span and Blazor has features that enable C# interactions with that label, but we need to activate other features that were once present in Web Forms, such as style properties, `AssociatedControlID`, and `ToolTip` support.

Original Web Forms documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.label?view=netframework-4.8

## Features Supported in Blazor

- `Text` - the text content of the Label component
- `ToolTip` - tooltip text displayed on hover (renders as `title` attribute)
- `AssociatedControlID` - associates the label with a form control for accessibility
- All style properties (`BackColor`, `ForeColor`, `BorderColor`, `BorderStyle`, `BorderWidth`, `CssClass`, `Font`, `Width`, `Height`)
- `Visible` - controls label visibility
- `Enabled` - enables or disables the label

## Web Forms Features NOT Supported

- **EnableTheming / SkinID** - Blazor uses CSS for theming
- **EnableViewState** - Blazor doesn't use ViewState
- Lifecycle events (`OnInit`, `OnLoad`, `OnPreRender`, `OnUnload`) - Not part of the Blazor component model

## Syntax Comparison

=== "Web Forms (Before)"

    ```html
    <asp:Label
        AccessKey="string"
        AssociatedControlID="string"
        BackColor="color name|#dddddd"
        BorderColor="color name|#dddddd"
        BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|Groove|Ridge|
            Inset|Outset"
        BorderWidth="size"
        CssClass="string"
        Enabled="True|False"
        EnableTheming="True|False"
        EnableViewState="True|False"
        Font-Bold="True|False"
        Font-Italic="True|False"
        Font-Names="string"
        Font-Overline="True|False"
        Font-Size="string|Smaller|Larger|XX-Small|X-Small|Small|Medium|
            Large|X-Large|XX-Large"
        Font-Strikeout="True|False"
        Font-Underline="True|False"
        ForeColor="color name|#dddddd"
        Height="size"
        ID="string"
        OnDataBinding="DataBinding event handler"
        OnDisposed="Disposed event handler"
        OnInit="Init event handler"
        OnLoad="Load event handler"
        OnPreRender="PreRender event handler"
        OnUnload="Unload event handler"
        runat="server"
        SkinID="string"
        Style="string"
        TabIndex="integer"
        Text="string"
        ToolTip="string"
        Visible="True|False"
        Width="size"
    />
    ```

=== "Blazor (After)"

    ```razor
    <Label Text="Hello, World!" />
    ```

## Blazor Syntax

### Basic Label

```razor
<Label Text="Welcome to the application" />
```

### Label with Styling

```razor
<Label Text="Important Notice"
       ForeColor="WebColor.Red"
       Font_Bold="true"
       Font_Size="FontUnit.Large"
       CssClass="notice-label" />
```

### Label with ToolTip

```razor
<Label Text="Hover over me" ToolTip="This is additional information" />
```

### Label with BackColor and Border

```razor
<Label Text="Highlighted text"
       BackColor="WebColor.LightYellow"
       ForeColor="WebColor.Navy"
       BorderColor="WebColor.Gray"
       BorderWidth="Unit.Pixel(1)"
       BorderStyle="BorderStyle.Solid" />
```

## HTML Output

**Blazor Input:**
```razor
<Label Text="Hello" CssClass="info-label" ForeColor="WebColor.Blue" />
```

**Rendered HTML:**
```html
<span class="info-label" style="color:Blue;">Hello</span>
```

**With ToolTip:**
```razor
<Label Text="Help text" ToolTip="Click here for more info" />
```

**Rendered HTML:**
```html
<span title="Click here for more info">Help text</span>
```

## Migration Notes

When migrating from Web Forms to Blazor:

1. **Remove `asp:` prefix** - Change `<asp:Label>` to `<Label>`
2. **Remove `runat="server"`** - Not needed in Blazor
3. **Remove `ID` attribute** - Use `@ref` if you need a reference to the component
4. **Update style property syntax** - Use `WebColor.` prefix for colors, `Unit.Pixel()` for sizes

=== "Web Forms (Before)"

    ```html
    <asp:Label ID="lblMessage" 
               Text="Welcome" 
               ForeColor="Blue" 
               Font-Bold="true"
               runat="server" />
    ```

=== "Blazor (After)"

    ```razor
    <Label Text="Welcome" 
           ForeColor="WebColor.Blue" 
           Font_Bold="true" />
    ```

!!! tip "Migration Tip"
    The Label renders as a `<span>` element in both Web Forms and Blazor, so existing CSS targeting `span` elements will continue to work. Replace `Font-Bold` with `Font_Bold` (underscore instead of hyphen) in Blazor syntax.

## See Also

- [TextBox](TextBox.md) - Single-line and multi-line text input
- [Button](Button.md) - Trigger actions
- [Literal](Literal.md) - Render raw text without a wrapping element
