# Panel

The Panel component is a container control that renders as a `<div>` element, or as a `<fieldset>` when the `GroupingText` property is set. It provides a way to group controls and apply common styling.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.panel?view=netframework-4.8

## Features Supported in Blazor

- Child content rendering
- `GroupingText` property for fieldset/legend rendering
- `Direction` property for text direction (LTR/RTL)
- `HorizontalAlign` property for text alignment
- `ScrollBars` property for overflow control
- `Wrap` property for content wrapping
- `BackImageUrl` property for background images
- All style properties (BackColor, ForeColor, CssClass, etc.)
- `Visible` property to show/hide the panel

## Web Forms Features NOT Supported

- `DefaultButton` - JavaScript-based button targeting is not implemented

## Web Forms Declarative Syntax

```html
<asp:Panel
    ID="string"
    BackColor="color name|#dddddd"
    BorderColor="color name|#dddddd"
    BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|Groove|Ridge|Inset|Outset"
    BorderWidth="size"
    CssClass="string"
    DefaultButton="string"
    Direction="NotSet|LeftToRight|RightToLeft"
    Enabled="True|False"
    Font-Bold="True|False"
    Font-Italic="True|False"
    Font-Names="string"
    Font-Size="string"
    ForeColor="color name|#dddddd"
    GroupingText="string"
    Height="size"
    HorizontalAlign="NotSet|Left|Center|Right|Justify"
    ScrollBars="None|Horizontal|Vertical|Both|Auto"
    Visible="True|False"
    Width="size"
    Wrap="True|False"
    runat="server">
    
    <!-- Child content here -->
    
</asp:Panel>
```

## Blazor Syntax

### Basic Panel

```razor
<Panel>
    <p>This is content inside a panel.</p>
</Panel>
```

### Panel with CSS Class

```razor
<Panel CssClass="card p-3">
    <h3>Card Title</h3>
    <p>Card content goes here.</p>
</Panel>
```

### Panel with GroupingText (Fieldset)

When `GroupingText` is set, the Panel renders as a `<fieldset>` with a `<legend>`:

```razor
<Panel GroupingText="Personal Information">
    <Label Text="Name:" />
    <TextBox @bind-Text="name" />
    <br />
    <Label Text="Email:" />
    <TextBox @bind-Text="email" TextMode="TextBoxMode.Email" />
</Panel>
```

**Renders as:**
```html
<fieldset>
    <legend>Personal Information</legend>
    <!-- content -->
</fieldset>
```

### Panel with ScrollBars

```razor
<Panel ScrollBars="ScrollBars.Auto" Height="Unit.Pixel(200)" Width="Unit.Pixel(300)">
    <p>This content can scroll if it overflows the panel dimensions.</p>
    <!-- lots of content -->
</Panel>
```

### Panel with Text Direction (RTL)

```razor
<Panel Direction="ContentDirection.RightToLeft">
    <p>This text will be right-to-left.</p>
</Panel>
```

### Panel with Horizontal Alignment

```razor
<Panel HorizontalAlign="HorizontalAlign.Center">
    <p>This content is centered.</p>
</Panel>
```

### Panel with No Wrap

```razor
<Panel Wrap="false">
    <p>This content will not wrap to a new line.</p>
</Panel>
```

### Styled Panel

```razor
@using static BlazorWebFormsComponents.WebColor

<Panel BackColor="LightGray" 
       ForeColor="DarkBlue"
       BorderColor="Navy"
       BorderWidth="Unit.Pixel(1)"
       BorderStyle="BorderStyle.Solid">
    <p>Styled panel content.</p>
</Panel>
```

### Conditionally Visible Panel

```razor
<CheckBox Text="Show Details" @bind-Checked="showDetails" />

<Panel Visible="@showDetails">
    <p>These are the details that can be shown or hidden.</p>
</Panel>

@code {
    private bool showDetails = false;
}
```

### Panel with BackImageUrl

The `BackImageUrl` property sets a CSS background image on the panel. This renders as an inline `background-image:url(...)` style.

```razor
<Panel BackImageUrl="https://example.com/images/background.png"
       Width="Unit.Pixel(500)"
       Height="Unit.Pixel(200)"
       BorderStyle="BorderStyle.Solid"
       BorderWidth="Unit.Pixel(1)">
    <p>Content displayed over the background image.</p>
</Panel>
```

**Renders as:**
```html
<div style="background-image:url(https://example.com/images/background.png);width:500px;height:200px;border-style:Solid;border-width:1px;">
    <p>Content displayed over the background image.</p>
</div>
```

## HTML Output

### Default Panel (div)

**Blazor:**
```razor
<Panel CssClass="my-panel">
    <p>Content</p>
</Panel>
```

**Rendered HTML:**
```html
<div class="my-panel">
    <p>Content</p>
</div>
```

### Panel with GroupingText (fieldset)

**Blazor:**
```razor
<Panel GroupingText="Settings" CssClass="settings-group">
    <p>Content</p>
</Panel>
```

**Rendered HTML:**
```html
<fieldset class="settings-group">
    <legend>Settings</legend>
    <p>Content</p>
</fieldset>
```

## ScrollBars Property Values

| Value | CSS Output |
|-------|------------|
| `ScrollBars.None` | No overflow style |
| `ScrollBars.Horizontal` | `overflow-x:scroll; overflow-y:hidden` |
| `ScrollBars.Vertical` | `overflow-x:hidden; overflow-y:scroll` |
| `ScrollBars.Both` | `overflow:scroll` |
| `ScrollBars.Auto` | `overflow:auto` |

## Migration Notes

When migrating from Web Forms to Blazor:

1. Remove the `asp:` prefix and `runat="server"` attribute
2. Replace `ID` with `@ref` if you need a component reference
3. The `DefaultButton` property is not implemented - use JavaScript or Blazor event handling instead
4. Use `ChildContent` pattern (content between tags) for child controls

### Before (Web Forms):
```html
<asp:Panel ID="pnlDetails" 
           CssClass="details-panel" 
           Visible="true" 
           runat="server">
    <p>Panel content</p>
</asp:Panel>
```

### After (Blazor):
```razor
<Panel CssClass="details-panel" Visible="true">
    <p>Panel content</p>
</Panel>
```

## See Also

- [PlaceHolder](PlaceHolder.md) - Container with no wrapper element
- [Label](Label.md) - Display text
