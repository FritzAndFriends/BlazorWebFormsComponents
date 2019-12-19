# Repeater

The Repeater component is meant to emulate the asp:Repeater control in markup and is defined in the [System.Web.UI.WebControls.Repeater class](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.repeater?view=netframework-4.8)

## Web Forms Declarative Syntax

```html
<asp:Repeater  
    DataMember="string"  
    DataSource="string"  
    DataSourceID="string"  
    EnableTheming="True|False"  
    EnableViewState="True|False"  
    ID="string"  
    OnDataBinding="DataBinding event handler"  
    OnDisposed="Disposed event handler"  
    OnInit="Init event handler"  
    OnItemCommand="ItemCommand event handler"  
    OnItemCreated="ItemCreated event handler"  
    OnItemDataBound="ItemDataBound event handler"  
    OnLoad="Load event handler"  
    OnPreRender="PreRender event handler"  
    OnUnload="Unload event handler"  
    runat="server"  
    Visible="True|False"  
>  
        <AlternatingItemTemplate>  
            <!-- child controls -->  
        </AlternatingItemTemplate>  
        <FooterTemplate>  
            <!-- child controls -->  
        </FooterTemplate>  
        <HeaderTemplate>  
            <!-- child controls -->  
        </HeaderTemplate>  
        <ItemTemplate>  
            <!-- child controls -->  
        </ItemTemplate>  
        <SeparatorTemplate>  
            <!-- child controls -->  
        </SeparatorTemplate>  
</asp:Repeater>
```