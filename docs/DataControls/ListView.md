# ListView

The ListView component is meant to emulate the asp:ListView control in markup and is defined in the [System.Web.UI.WebControls.ListView class](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.listview?view=netframework-4.8)

[Usage Notes](#usage-notes) | [Web Forms Syntax](#web-forms-declarative-syntax) | [Blazor Syntax](#blazor-syntax)

## Features supported in Blazor
 - Alternating Item Templates
 - Alternating Item Styles
 - Empty Data Template
 - Empty Item Template
 - Grouping
 - Item Templates
 - Item Styles
 - Model Binding
   - OnSelect Method
 - LayoutTemplate
 - DataBinder within the ItemTemplate and AlternatingItemTemplate

##### [Back to top](#listview)

## Usage Notes

- **LayoutTemplate** - Requires a `Context` attribute that defines the placeholder for the items
- **Context attribute** - For Web Forms compatibility, use `Context="Item"` on the ListView to access the current item as `@Item` in ItemTemplate and AlternatingItemTemplate instead of Blazor's default `@context`
- **ItemType attribute** - Required to specify the type of items in the collection

##### [Back to top](#listview)

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

##### [Back to top](#listview)

## Blazor Syntax

##### [Back to top](#listview)