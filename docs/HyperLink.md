# LinkButton

It may seem strange that we have a HyperLink component when there already is an HTML anchor and Blazor has features that enable C# interactions with that link, but we need to activate other features that were once present in Web Forms.  Original Web Forms documentation is at:  https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.hyperlink?view=netframework-4.8

## Blazor Features Supported

- `NavigationUrl` the URL to link when HyperLink component is clicked
- `Text` the text content of the HyperLink component
- `Target` the target window or frame in which to display the Web page content linked to when the HyperLink component is clicked

## WebForms Syntax

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
