It may seem strange that we have a HyperLink component when there already is an HTML anchor and Blazor has features that enable C# interactions with that link, but we need to activate other features that were once present in Web Forms.  Original Web Forms documentation is at:  https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.hyperlink?view=netframework-4.8

## Blazor Features Supported

### Core Properties
- `NavigationUrl` - the URL to link when HyperLink component is clicked
- `Text` - the text content of the HyperLink component
- `Target` - the target window or frame in which to display the Web page content linked to when the HyperLink component is clicked (e.g., "_blank", "_self", "_parent", "_top")
- `ToolTip` - displays a tooltip on hover (rendered as the `title` attribute)

### Styling Properties
- `BackColor` - background color of the hyperlink
- `ForeColor` - text color of the hyperlink
- `BorderColor` - border color
- `BorderStyle` - border style (NotSet, None, Dotted, Dashed, Solid, Double, Groove, Ridge, Inset, Outset)
- `BorderWidth` - border width
- `CssClass` - CSS class name to apply to the hyperlink
- `Height` - height of the hyperlink
- `Width` - width of the hyperlink

### Font Properties
- `Font-Bold` - bold text
- `Font-Italic` - italic text
- `Font-Names` - font family names
- `Font-Overline` - overline text decoration
- `Font-Size` - font size
- `Font-Strikeout` - strikethrough text decoration
- `Font-Underline` - underline text decoration

### Other Properties
- `Enabled` - whether the hyperlink is enabled (default: true)
- `Visible` - whether the hyperlink is visible (default: true)
- `TabIndex` - tab order for keyboard navigation

### Event Handlers
- `OnDataBinding` - event handler for data binding
- `OnInit` - event handler triggered at initialization
- `OnLoad` - event handler triggered after component loads
- `OnPreRender` - event handler triggered before rendering
- `OnUnload` - event handler triggered when component unloads
- `OnDisposed` - event handler triggered when component is disposed

## WebForms Features Not Supported

- `ImageUrl` - The Blazor HyperLink component does not support image links. Use the Image component wrapped in an anchor tag instead.
- `AccessKey` - Not supported in Blazor
- `EnableTheming` - Theming is not available in Blazor
- `EnableViewState` - ViewState is supported for compatibility but this parameter does nothing
- `SkinID` - Theming is not available in Blazor

## Usage Examples

### Web Forms Syntax
```html
<asp:HyperLink ID="lnkExample" 
    NavigateUrl="https://example.com" 
    Text="Visit Example" 
    Target="_blank"
    ToolTip="Click to visit example.com"
    CssClass="btn btn-link"
    runat="server" />
```

### Blazor Syntax
```html
<HyperLink NavigationUrl="https://example.com" 
    Text="Visit Example" 
    Target="_blank"
    ToolTip="Click to visit example.com"
    CssClass="btn btn-link" />
```

### Blazor with Styling
```html
<HyperLink NavigationUrl="/products" 
    Text="View Products" 
    ForeColor="#0066cc"
    Font-Bold="true"
    Font-Size="14px" />
```

### Blazor without NavigationUrl (renders as plain anchor)
```html
<HyperLink Text="Inactive Link" 
    ToolTip="This link has no URL" />
```

## WebForms Syntax Reference

```html
<asp:HyperLink
    AccessKey="string"
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
    ImageUrl="uri"
    NavigateUrl="uri"
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
    Target="string|_blank|_parent|_search|_self|_top"
    Text="string"
    ToolTip="string"
    Visible="True|False"
    Width="size"
/>
```
