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
 - **ListViewItem and ListViewDataItem** for event handling
 - **CRUD Operations** (Edit, Delete, Insert, Update, Cancel)
   - EditItemTemplate
   - InsertItemTemplate
   - EditIndex
   - InsertItemPosition

##### [Back to top](#listview)

## ListViewItem and ListViewDataItem

The `OnItemDataBound` event provides a `ListViewItemEventArgs` object that contains a `ListViewItem` instance. For data items, this will be a `ListViewDataItem` object with the following properties:

- **ItemType** - The type of item (DataItem, InsertItem, or EmptyItem)
- **DisplayIndex** - The position of the item as displayed in the ListView
- **DataItemIndex** - The index of the data item in the underlying data source
- **DataItem** - The underlying data object bound to this item

**Example:**

```csharp
void ItemDataBound(ListViewItemEventArgs e)
{
    if (e.Item.ItemType == ListViewItemType.DataItem)
    {
        var dataItem = (ListViewDataItem)e.Item;
        var widget = (Widget)dataItem.DataItem;
        // Access widget.Name, widget.Price, etc.
    }
}
```

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

```razor
<ListView Context="Item"
          ItemType="Widget"
          EditIndex="@editIndex"
          InsertItemPosition="InsertItemPosition.LastItem"
          ItemEditing="OnItemEditing"
          ItemUpdating="OnItemUpdating"
          ItemCanceling="OnItemCanceling"
          ItemDeleting="OnItemDeleting"
          ItemInserting="OnItemInserting">
    <ItemTemplate>
        <tr>
            <td>@Item.Name</td>
            <td>
                <button @onclick="() => listView.HandleCommand("edit", null, index)">Edit</button>
                <button @onclick="() => listView.HandleCommand("delete", null, index)">Delete</button>
            </td>
        </tr>
    </ItemTemplate>
    <EditItemTemplate>
        <tr>
            <td><input @bind="editName" /></td>
            <td>
                <button @onclick="() => listView.HandleCommand("update", null, editIndex)">Update</button>
                <button @onclick="() => listView.HandleCommand("cancel", null, editIndex)">Cancel</button>
            </td>
        </tr>
    </EditItemTemplate>
    <InsertItemTemplate>
        <tr>
            <td><input @bind="newName" /></td>
            <td>
                <button @onclick="() => listView.HandleCommand("insert", null, 0)">Insert</button>
            </td>
        </tr>
    </InsertItemTemplate>
    <EmptyDataTemplate>No items available.</EmptyDataTemplate>
</ListView>
```

##### [Back to top](#listview)

## CRUD Operations

The ListView supports full Create, Read, Update, and Delete operations through a set of command events. Use `HandleCommand` on the ListView reference to trigger operations.

### Events

| Event | EventArgs | Description |
|-------|-----------|-------------|
| `ItemEditing` | `ListViewEditEventArgs` | Fires when an Edit command is requested. Set `NewEditIndex` to control which item enters edit mode. |
| `ItemUpdating` | `ListViewUpdateEventArgs` | Fires when an Update command is requested. `ItemIndex` identifies the item being updated. |
| `ItemCanceling` | `ListViewCancelEventArgs` | Fires when a Cancel command is requested. |
| `ItemDeleting` | `ListViewDeleteEventArgs` | Fires before an item is deleted. Set `Cancel = true` to prevent deletion. |
| `ItemDeleted` | `ListViewDeletedEventArgs` | Fires after a delete operation completes. |
| `ItemInserting` | `ListViewInsertEventArgs` | Fires before an item is inserted. Set `Cancel = true` to prevent insertion. |
| `ItemInserted` | `ListViewInsertedEventArgs` | Fires after an insert operation completes. |

### EditItemTemplate

When `EditIndex` is set to a valid item index, the `EditItemTemplate` is rendered for that item instead of the `ItemTemplate`. Set `EditIndex = -1` to exit edit mode.

### InsertItemTemplate

The `InsertItemTemplate` renders an insert row when `InsertItemPosition` is set to `FirstItem` or `LastItem`. Set to `None` (default) to hide the insert row.

### Migration Example

**Before (Web Forms):**
```html
<asp:ListView ID="lvItems" runat="server"
    OnItemEditing="lvItems_ItemEditing"
    OnItemUpdating="lvItems_ItemUpdating"
    OnItemDeleting="lvItems_ItemDeleting"
    OnItemInserting="lvItems_ItemInserting"
    InsertItemPosition="LastItem">
    <ItemTemplate>
        <tr>
            <td><%# Eval("Name") %></td>
            <td>
                <asp:Button runat="server" CommandName="Edit" Text="Edit" />
                <asp:Button runat="server" CommandName="Delete" Text="Delete" />
            </td>
        </tr>
    </ItemTemplate>
    <EditItemTemplate>
        <tr>
            <td><asp:TextBox ID="txtName" runat="server" Text='<%# Bind("Name") %>' /></td>
            <td>
                <asp:Button runat="server" CommandName="Update" Text="Update" />
                <asp:Button runat="server" CommandName="Cancel" Text="Cancel" />
            </td>
        </tr>
    </EditItemTemplate>
</asp:ListView>
```

**After (Blazor):**
```razor
<ListView @ref="listView" Context="Item" ItemType="Widget"
    EditIndex="@editIndex"
    InsertItemPosition="InsertItemPosition.LastItem"
    ItemEditing="OnItemEditing"
    ItemUpdating="OnItemUpdating"
    ItemDeleting="OnItemDeleting"
    ItemInserting="OnItemInserting">
    <ItemTemplate>
        <tr>
            <td>@Item.Name</td>
            <td>
                <button @onclick="() => listView.HandleCommand("edit", null, idx)">Edit</button>
                <button @onclick="() => listView.HandleCommand("delete", null, idx)">Delete</button>
            </td>
        </tr>
    </ItemTemplate>
    <EditItemTemplate>
        <tr>
            <td><input @bind="editName" /></td>
            <td>
                <button @onclick="() => listView.HandleCommand("update", null, editIndex)">Update</button>
                <button @onclick="() => listView.HandleCommand("cancel", null, editIndex)">Cancel</button>
            </td>
        </tr>
    </EditItemTemplate>
</ListView>
```

##### [Back to top](#listview)