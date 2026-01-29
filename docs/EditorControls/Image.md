# Image

The **Image** component displays an image on a web page. It may seem strange that we have an Image component when there already is an HTML `<img>` element and Blazor has features that enable C# interactions with that image, but we need to activate other features that were once present in Web Forms, such as `DescriptionUrl` for accessibility, `ImageAlign` for legacy alignment, and `GenerateEmptyAlternateText` for accessibility compliance.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.image?view=netframework-4.8

## Features Supported in Blazor

- `AlternateText` - alternate text displayed when the image is unavailable (renders as `alt` attribute)
- `DescriptionUrl` - the location to a detailed description for the image (renders as `longdesc` attribute)
- `GenerateEmptyAlternateText` - when `true`, generates an empty `alt=""` attribute for decorative images (accessibility compliance)
- `ImageAlign` - the alignment of the image in relation to other elements on the Web page (renders as `align` attribute)
- `ImageUrl` - the URL of the image to display (renders as `src` attribute)
- `ToolTip` - tooltip text displayed on hover (renders as `title` attribute)
- `Visible` - controls image visibility

### Blazor Notes

- The `ImageUrl` property maps directly to the HTML `src` attribute. In Blazor, you can use relative paths from your `wwwroot` folder (e.g., `/images/logo.png`) or absolute URLs
- For static assets in Blazor, place images in the `wwwroot` folder and reference them with a leading `/`
- The `ImageAlign` property uses the `ImageAlign` enum with values: `NotSet`, `Left`, `Right`, `Baseline`, `Top`, `Middle`, `Bottom`, `AbsBottom`, `AbsMiddle`, `TextTop`
- The `longdesc` attribute (from `DescriptionUrl`) is considered obsolete in HTML5 but is rendered for backward compatibility. Consider using `aria-describedby` in new applications

## Web Forms Features NOT Supported

- **Style properties** (`BackColor`, `ForeColor`, `BorderColor`, `BorderStyle`, `BorderWidth`, `CssClass`, `Width`, `Height`, `Font`) - Not yet implemented; use standard HTML/CSS styling on a wrapper element
- **AccessKey** - Not supported; use HTML `accesskey` attribute directly if needed
- **TabIndex** - Not supported; use HTML `tabindex` attribute directly if needed
- **EnableTheming/SkinID** - Not supported; ASP.NET theming is not available in Blazor
- **EnableViewState** - Not needed; Blazor manages state differently
- **Lifecycle events** (`OnDataBinding`, `OnInit`, `OnLoad`, etc.) - Not supported; use Blazor lifecycle methods instead

## Web Forms Declarative Syntax

```html
<asp:Image
    AccessKey="string"
    AlternateText="string"
    BackColor="color name|#dddddd"
    BorderColor="color name|#dddddd"
    BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|Groove|Ridge|
        Inset|Outset"
    BorderWidth="size"
    CssClass="string"
    DescriptionUrl="uri"
    Enabled="True|False"
    EnableTheming="True|False"
    EnableViewState="True|False"
    ForeColor="color name|#dddddd"
    GenerateEmptyAlternateText="True|False"
    Height="size"
    ID="string"
    ImageAlign="NotSet|Left|Right|Baseline|Top|Middle|Bottom|
        AbsBottom|AbsMiddle|TextTop"
    ImageUrl="uri"
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
    ToolTip="string"
    Visible="True|False"
    Width="size"
/>
```

## Blazor Razor Syntax

### Basic Image

```razor
<Image ImageUrl="/images/photo.jpg" AlternateText="A beautiful photo" />
```

### Image with Tooltip

```razor
<Image ImageUrl="/images/logo.png" 
       AlternateText="Company Logo" 
       ToolTip="Our company logo" />
```

### Image with Alignment

```razor
@using BlazorWebFormsComponents.Enums

<Image ImageUrl="/images/icon.png" 
       AlternateText="Icon" 
       ImageAlign="ImageAlign.Left" />
<p>This text flows around the left-aligned image.</p>
```

### Decorative Image (Empty Alt Text)

```razor
<Image ImageUrl="/images/decorative-border.png" 
       GenerateEmptyAlternateText="true" />
```

### Image with Description URL

```razor
<Image ImageUrl="/images/complex-chart.png" 
       AlternateText="Sales Chart" 
       DescriptionUrl="/descriptions/sales-chart.html" />
```

## HTML Output

**Blazor Input:**
```razor
<Image ImageUrl="/images/photo.jpg" 
       AlternateText="A photo" 
       ToolTip="Click to enlarge" 
       ImageAlign="ImageAlign.Left" />
```

**Rendered HTML:**
```html
<img src="/images/photo.jpg" alt="A photo" longdesc="" title="Click to enlarge" align="left" />
```

## Migration Notes

When migrating from Web Forms to Blazor:

1. **Remove `asp:` prefix** - Change `<asp:Image>` to `<Image>`
2. **Remove `runat="server"`** - Not needed in Blazor
3. **Remove `ID` attribute** - Use `@ref` if you need a reference to the component
4. **Update image paths** - Place images in `wwwroot` folder and use paths like `/images/photo.jpg`
5. **Add `@using` for enums** - Add `@using BlazorWebFormsComponents.Enums` when using `ImageAlign`
6. **Style properties** - Wrap the Image in a `<div>` or `<span>` and apply CSS for styling not yet supported

### Before (Web Forms)

```html
<asp:Image ID="imgLogo" 
           ImageUrl="~/images/logo.png" 
           AlternateText="Company Logo"
           CssClass="logo-image"
           runat="server" />
```

### After (Blazor)

```razor
<div class="logo-image">
    <Image ImageUrl="/images/logo.png" 
           AlternateText="Company Logo" />
</div>
```

## Examples

### Basic Image Display

```razor
<Image ImageUrl="/images/sample.jpg" AlternateText="Sample Image" />
```

### Dynamic Image URL

```razor
<Image ImageUrl="@imageUrl" AlternateText="@imageAlt" />

@code {
    private string imageUrl = "/images/default.png";
    private string imageAlt = "Default image";
    
    void ChangeImage()
    {
        imageUrl = "/images/alternate.png";
        imageAlt = "Alternate image";
    }
}
```

### Conditional Visibility

```razor
<Image ImageUrl="/images/banner.jpg" 
       AlternateText="Promotional Banner" 
       Visible="@showBanner" />

<button @onclick="ToggleBanner">Toggle Banner</button>

@code {
    private bool showBanner = true;
    
    void ToggleBanner()
    {
        showBanner = !showBanner;
    }
}
```

### Text Wrapping with Aligned Image

```razor
@using BlazorWebFormsComponents.Enums

<Image ImageUrl="/images/author.jpg" 
       AlternateText="Author photo" 
       ImageAlign="ImageAlign.Left" />

<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
   This text wraps around the left-aligned image. The ImageAlign 
   property allows you to control how content flows around the image.</p>

<div style="clear: both;"></div>
```

## See Also

- [ImageButton](ImageButton.md) - A clickable image that acts as a button
- [HyperLink](../NavigationControls/HyperLink.md) - Wrap an Image in a HyperLink for clickable images
- [Live Demo](https://blazorwebformscomponents.azurewebsites.net/ControlSamples/Image) - Interactive Image samples
