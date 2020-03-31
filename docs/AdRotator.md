# AdRotator

The AdRotator component is meant to emulate the `asp:Rotator` control in markup and is defined in the [System.Web.UI.WebControls.AdRotator class](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.adrotator?view=netframework-4.8)

## Blazor Features Supported

- `AdvertismetFile` the path to an XML file that contains advertisement information.
- `Target` the name of the browser window or frame that displays the contents of the Web page linked to when the AdRotator component is clicked.

## Web Forms Declarative Syntax

```html
<asp:AdRotator  
    AccessKey="string"  
    AdvertisementFile="uri"  
    AlternateTextField="string"  
    BackColor="color name|#dddddd"  
    BorderColor="color name|#dddddd"  
    BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|Groove|Ridge|  
        Inset|Outset"  
    BorderWidth="size"  
    CssClass="string"  
    DataMember="string"  
    DataSource="string"  
    DataSourceID="string"  
    Enabled="True|False"  
    EnableTheming="True|False"  
    EnableViewState="True|False"  
    ForeColor="color name|#dddddd"  
    Height="size"  
    ID="string"  
    ImageUrlField="string"  
    KeywordFilter="string"  
    NavigateUrlField="string"  
    OnAdCreated="AdCreated event handler"  
    OnDataBinding="DataBinding event handler"  
    OnDataBound="DataBound event handler"  
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
    ToolTip="string"  
    Visible="True|False"  
    Width="size"  
/>
```
