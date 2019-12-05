# ListView

The ListView component is meant to emulate the asp:ListView control in markup and is defined in the [System.Web.UI.WebControls.ListView class](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.listview?view=netframework-4.8)

## Web Forms Declarative Syntax

```html
<asp:ListView  
    ConvertEmptyStringToNull="True|False"  
    DataKeyNames="string"  
    DataMember="string"  
    DataSource="string"  
    DataSourceID="string"  
    EditIndex="integer"  
    Enabled="True|False"  
    EnableTheming="True|False"  
    EnableViewState="True|False"  
    GroupPlaceholderID="string"  
    GroupItemCount="integer"  
    ID="string"  
    InsertItemPosition="None|FirstItem|LastItem"  
    ItemPlaceholderID="string"  
    OnDataBinding="DataBinding event handler"  
    OnDataBound="DataBound event handler"  
    OnDisposed="Disposed event handler"  
    OnInit="Init event handler"  
    OnItemCanceling="ItemCanceling event handler"  
    OnItemCommand="ItemCommand event handler"  
    OnItemCreated="ItemCreated event handler"  
    OnItemDataBound="ItemDataBound event handler"  
    OnItemDeleted="ItemDeleted event handler"  
    OnItemDeleting="ItemDeleting event handler"  
    OnItemEditing="ItemEditing event handler"  
    OnItemInserted="ItemInserted event handler"  
    OnItemInserting="ItemInserting event handler"  
    OnItemUpdated="ItemUpdated event handler"  
    OnItemUpdating="ItemUpdating event handler"  
    OnLayoutCreated="LayoutCreated event handler"  
    OnLoad="Load event handler"  
    OnPagePropertiesChanged="PagePropertiesChanged event handler"  
    OnPagePropertiesChanging="PagePropertiesChanging event handler"  
    OnPreRender="PreRender event handler"  
    OnSelectedIndexChanged="SelectedIndexChanged event handler"  
    OnSelectedIndexChanging="SelectedIndexChanging event handler"  
    OnSorted="Sorted event handler"  
    OnSorting="Sorting event handler"  
    OnUnload="Unload event handler"  
    runat="server"  
    SelectedIndex="integer"  
    SkinID="string"  
    Style="string"  
    Visible="True|False">  
    <AlternatingItemTemplate>  
        <!-- child controls -->  
    </AlternatingItemTemplate>  
    <EditItemTemplate>  
        <!-- child controls -->  
    </EditItemTemplate>  
    <EmptyDataTemplate>  
        <!-- child controls -->  
    </EmptyDataTemplate>  
    <EmptyItemTemplate>  
        <!-- child controls -->  
    </EmptyItemTemplate>  
    <GroupSeparatorTemplate>  
        <!-- child controls -->  
    </GroupSeparatorTemplate>  
    <GroupTemplate>  
        <!-- child controls -->  
    </GroupTemplate>  
    <InsertItemTemplate>  
        <!-- child controls -->  
    </InsertItemTemplate>  
    <ItemSeparatorTemplate>  
        <!-- child controls -->  
    </ItemSeparatorTemplate>  
    <ItemTemplate>  
        <!-- child controls -->  
    </ItemTemplate>  
    <LayoutTemplate>  
            <!-- child controls -->  
    </LayoutTemplate>  
    <SelectedItemTemplate>  
        <!-- child controls -->  
    </SelectedItemTemplate>  
</asp:ListView>
```