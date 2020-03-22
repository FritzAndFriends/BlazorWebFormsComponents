# ImageButton

It may seem strange that we have a ImageButton component when there already is an HTML input with image type and Blazor has features that enable C# interactions with that image, but we need to activate other features that were once present in Web Forms.  Original Web Forms documentation is at: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.imagebutton?view=netframework-4.8

## Blazor Features Supported

- `AlternateText` alternate text displayed when the image is unavailable
- `DescriptionUrl` the location to a detailed description for the image
- `ImageAlign`  the alignment of the image in relation to other elements on the Web page
- `ImageUrl` The URL of the image
- `OnClick` event handler
- `OnClientClick` JavaScript pointer
- `OnCommand` event handler with event bubbling
- `PostBackurl` The URL of the Web page to post to from the current page

## WebForms Syntax

```html
<asp:ImageButton  
    AccessKey="string"  
    AlternateText="string"  
    BackColor="color name|#dddddd"  
    BorderColor="color name|#dddddd"  
    BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|Groove|Ridge|  
        Inset|Outset"  
    BorderWidth="size"  
    CausesValidation="True|False"  
    CommandArgument="string"  
    CommandName="string"  
    CssClass="string"  
    DescriptionUrl="uri"  
    Enabled="True|False"  
    EnableTheming="True|False"  
    EnableViewState="True|False"  
    ForeColor="color name|#dddddd"  
    Height="size"  
    ID="string"  
    ImageAlign="NotSet|Left|Right|Baseline|Top|Middle|Bottom|  
        AbsBottom|AbsMiddle|TextTop"  
    ImageUrl="uri"  
    OnClick="Click event handler"  
    OnClientClick="string"  
    OnCommand="Command event handler"  
    OnDataBinding="DataBinding event handler"  
    OnDisposed="Disposed event handler"  
    OnInit="Init event handler"  
    OnLoad="Load event handler"  
    OnPreRender="PreRender event handler"  
    OnUnload="Unload event handler"  
    PostBackUrl="uri"  
    runat="server"  
    SkinID="string"  
    Style="string"  
    TabIndex="integer"  
    ToolTip="string"  
    ValidationGroup="string"  
    Visible="True|False"  
    Width="size"  
/>
```
