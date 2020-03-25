# LinkButton

It may seem strange that we have a Image component when there already is an HTML image and Blazor has features that enable C# interactions with that image, but we need to activate other features that were once present in Web Forms.  Original Web Forms documentation is at:  https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.image?view=netframework-4.8

## Blazor Features Supported

- `AlternateText` alternate text displayed when the image is unavailable
- `DescriptionUrl` the location to a detailed description for the image
- `ImageAlign`  the alignment of the image in relation to other elements on the Web page
- `ImageUrl` The URL of the image

## WebForms Syntax

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
