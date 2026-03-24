# Label

The **Label** component displays a text label. While HTML has native `<label>` and `<span>` elements, the BWFC Label component emulates the ASP.NET Web Forms `<asp:Label>` control, enabling features like styling, the `AssociatedControlID` property for accessibility, and a familiar API for Web Forms developers.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.label?view=netframework-4.8

## Features Supported in Blazor

- `Text` — The text content displayed by the label
- `AssociatedControlID` — Associates the label with a form control using the `for` attribute (improves accessibility)
- `ToolTip` — Tooltip text displayed on hover (renders as `title` attribute)
- `Visible` — Controls whether the label is rendered
- `Enabled` — Enable or disable the label
- All style properties (`BackColor`, `ForeColor`, `BorderColor`, `BorderStyle`, `BorderWidth`, `CssClass`, `Width`, `Height`, `Font`)
- `AccessKey` — Keyboard shortcut key
- `TabIndex` — Tab order
- `ID` — Control identifier rendered in HTML

## Web Forms Features NOT Supported

- **EnableViewState** — Not applicable in Blazor
- **EnableTheming** — Partially supported; theme system works differently in Blazor
- **SkinID** — Limited support compared to Web Forms themes
- **OnDataBinding, OnInit, OnLoad, OnPreRender, OnUnload** — Blazor lifecycle differs from Web Forms

## Syntax Comparison

=== "Web Forms"

    ```html
    <asp:Label
        AccessKey="string"
        AssociatedControlID="string"
        BackColor="color name|#dddddd"
        BorderColor="color name|#dddddd"
        BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|Groove|Ridge|Inset|Outset"
        BorderWidth="size"
        CssClass="string"
        Enabled="True|False"
        Font-Bold="True|False"
        Font-Italic="True|False"
        Font-Names="string"
        Font-Size="string"
        ForeColor="color name|#dddddd"
        Height="size"
        ID="string"
        runat="server"
        TabIndex="integer"
        Text="string"
        ToolTip="string"
        Visible="True|False"
        Width="size"
    />
    ```

=== "Blazor"

    ```razor
    <Label
        AccessKey="string"
        AssociatedControlID="string"
        BackColor="WebColor.Color"
        BorderColor="WebColor.Color"
        BorderStyle="BorderStyle.Solid"
        BorderWidth="1px"
        CssClass="string"
        Enabled="true"
        Font="new FontInfo() { Bold = true }"
        ForeColor="WebColor.Black"
        Height="20px"
        ID="string"
        TabIndex="0"
        Text="string"
        ToolTip="string"
        Visible="true"
        Width="200px"
    />
    ```

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Text` | `string` | `null` | Text content of the label |
| `AssociatedControlID` | `string` | `null` | ID of the control to associate with (renders `<label for="">`) |
| `ToolTip` | `string` | `null` | Hover text |
| `Visible` | `bool` | `true` | Show or hide the label |
| `Enabled` | `bool` | `true` | Enable or disable the label |
| `BackColor` | `WebColor` | `default` | Background color |
| `ForeColor` | `WebColor` | `default` | Text color |
| `BorderColor` | `WebColor` | `default` | Border color |
| `BorderStyle` | `BorderStyle` | `NotSet` | Border style |
| `BorderWidth` | `Unit` | `default` | Border width |
| `CssClass` | `string` | `null` | CSS class(es) |
| `Width` | `Unit` | `default` | Control width |
| `Height` | `Unit` | `default` | Control height |
| `Font` | `FontInfo` | `new FontInfo()` | Font properties (Bold, Italic, Name, Size, etc.) |
| `AccessKey` | `string` | `null` | Keyboard shortcut key |
| `TabIndex` | `short` | `0` | Tab order |
| `ID` | `string` | `null` | Control identifier |

## HTML Output

When `AssociatedControlID` is set, the label renders as an HTML `<label>` tag with a `for` attribute:

```html
<!-- Blazor Input -->
<Label Text="Email Address" AssociatedControlID="emailInput" />

<!-- Rendered HTML -->
<label for="emailInput">Email Address</label>
```

When `AssociatedControlID` is not set, it renders as a `<span>`:

```html
<!-- Blazor Input -->
<Label Text="Welcome" />

<!-- Rendered HTML -->
<span>Welcome</span>
```

With styling:

```html
<!-- Blazor Input -->
<Label Text="Important"
        BackColor="WebColor.Yellow"
        ForeColor="WebColor.Red"
        Font_Bold="true" />

<!-- Rendered HTML -->
<span style="background-color:Yellow;color:Red;font-weight:bold;">Important</span>
```

## Examples

### Basic Label

```razor
<Label Text="Full Name:" />
```

### Label Associated with Input

Improves accessibility by linking the label to a form control:

```razor
<Label Text="Email Address:" AssociatedControlID="emailInput" />
<InputText id="emailInput" @bind-Value="model.Email" />
```

### Styled Label

```razor
<Label Text="Warning"
        ForeColor="WebColor.Red"
        Font_Bold="true"
        CssClass="warning-label" />
```

### Label with Tooltip

```razor
<Label Text="Username"
        ToolTip="Your login username (case-sensitive)" />
```

### Conditional Visibility

```razor
<Label Text="Validation Error" 
        ForeColor="WebColor.Red"
        Visible="@showError" />

@code {
    private bool showError = false;
}
```

### Label with Border

```razor
<Label Text="Important Notice"
        BorderColor="WebColor.Black"
        BorderStyle="BorderStyle.Solid"
        BorderWidth="2px"
        BackColor="WebColor.LightYellow"
        CssClass="notice-box" />
```

## Migration Notes

When migrating from Web Forms to Blazor:

1. **Remove `asp:` prefix** — Change `<asp:Label>` to `<Label>`
2. **Remove `runat="server"`** — Not needed in Blazor
3. **`Text` property name stays the same** — Direct property instead of inline content
4. **`AssociatedControlID` works the same** — Maintains accessibility pattern
5. **Font properties use `FontInfo` object** — Instead of hyphenated attributes like `Font-Bold`
6. **Style properties use enums** — e.g., `BorderStyle.Solid` instead of string values
7. **Colors use `WebColor` enum** — Instead of color name strings

### Migration Example

=== "Web Forms"

    ```html
    <asp:Label ID="lblEmail"
               Text="Email Address:"
               AssociatedControlID="txtEmail"
               ForeColor="Red"
               Font-Bold="true"
               runat="server" />
    <asp:TextBox ID="txtEmail" runat="server" />
    ```

=== "Blazor"

    ```razor
    <Label ID="lblEmail"
           Text="Email Address:"
           AssociatedControlID="txtEmail"
           ForeColor="WebColor.Red"
           Font="new FontInfo() { Bold = true }" />
    <InputText id="txtEmail" @bind-Value="model.Email" />
    ```

## See Also

- [Button](Button.md) — Interactive button control
- [LinkButton](LinkButton.md) — Button that renders as a hyperlink
- [TextBox](TextBox.md) — Text input control
- [Panel](Panel.md) — Container control
- [HyperLink](HyperLink.md) — Navigate to a URL
- [Live Demo](https://blazorwebformscomponents.azurewebsites.net/ControlSamples/Label) — Interactive Label samples
