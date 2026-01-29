# CheckBoxList

The CheckBoxList component renders a group of checkboxes, allowing users to select multiple items from a list. It supports both static items and data-bound scenarios.

Original Web Forms documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.checkboxlist?view=netframework-4.8

!!! warning "Not Yet Implemented"
    This component is not yet available in BlazorWebFormsComponents. This documentation is a placeholder for the planned implementation.

## Planned Features

- Static items via `StaticItems` parameter with `ListItem` collection
- Data binding via `Items` parameter with `DataTextField` and `DataValueField`
- Multiple selection tracking via `SelectedItems` or `SelectedValues`
- `RepeatColumns` for multi-column layout
- `RepeatDirection` (Vertical or Horizontal)
- `RepeatLayout` (Table, Flow, OrderedList, UnorderedList)
- `TextAlign` for label positioning (Left or Right)
- `OnSelectedIndexChanged` event handler
- Style attributes and CssClass formatting

## Web Forms Declarative Syntax

```html
<asp:CheckBoxList
    AccessKey="string"
    AutoPostBack="True|False"
    BackColor="color name|#dddddd"
    BorderColor="color name|#dddddd"
    BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|Groove|Ridge|Inset|Outset"
    BorderWidth="size"
    CellPadding="integer"
    CellSpacing="integer"
    CssClass="string"
    DataSourceID="string"
    DataTextField="string"
    DataValueField="string"
    Enabled="True|False"
    EnableTheming="True|False"
    EnableViewState="True|False"
    Font-Bold="True|False"
    Font-Italic="True|False"
    Font-Names="string"
    Font-Overline="True|False"
    Font-Size="string|Smaller|Larger|XX-Small|X-Small|Small|Medium|Large|X-Large|XX-Large"
    Font-Strikeout="True|False"
    Font-Underline="True|False"
    ForeColor="color name|#dddddd"
    Height="size"
    ID="string"
    OnDataBinding="DataBinding event handler"
    OnDisposed="Disposed event handler"
    OnInit="Init event handler"
    OnLoad="Load event handler"
    OnPreRender="PreRender event handler"
    OnSelectedIndexChanged="SelectedIndexChanged event handler"
    OnUnload="Unload event handler"
    RepeatColumns="integer"
    RepeatDirection="Horizontal|Vertical"
    RepeatLayout="Table|Flow|OrderedList|UnorderedList"
    runat="server"
    SelectedIndex="integer"
    SelectedValue="string"
    TabIndex="integer"
    TextAlign="Left|Right"
    ToolTip="string"
    Visible="True|False"
    Width="size">
    
    <asp:ListItem Value="value1" Text="Display Text 1" Selected="True|False" />
    <asp:ListItem Value="value2" Text="Display Text 2" />
    
</asp:CheckBoxList>
```

## Proposed Blazor Syntax

### Basic Usage

```razor
<CheckBoxList TItem="object" 
              StaticItems="items"
              OnSelectedIndexChanged="HandleSelectionChanged" />

@code {
    private ListItemCollection items = new()
    {
        new ListItem("Option One", "1"),
        new ListItem("Option Two", "2"),
        new ListItem("Option Three", "3")
    };
    
    private void HandleSelectionChanged()
    {
        // Handle selection change
    }
}
```

### Multi-Column Layout

```razor
<CheckBoxList TItem="object" 
              StaticItems="items"
              RepeatColumns="3"
              RepeatDirection="RepeatDirection.Horizontal" />
```

### Data Binding

```razor
<CheckBoxList TItem="Category"
              Items="categories"
              DataTextField="Name"
              DataValueField="Id" />

@code {
    private List<Category> categories = new()
    {
        new Category { Id = "1", Name = "Electronics" },
        new Category { Id = "2", Name = "Clothing" },
        new Category { Id = "3", Name = "Books" }
    };
    
    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
```

### Getting Selected Items

```razor
<CheckBoxList @ref="checkboxList" TItem="object" StaticItems="items" />
<Button Text="Submit" OnClick="HandleSubmit" />

@code {
    private CheckBoxList<object> checkboxList;
    
    private void HandleSubmit()
    {
        var selectedItems = checkboxList.SelectedItems;
        foreach (var item in selectedItems)
        {
            Console.WriteLine($"Selected: {item.Text} ({item.Value})");
        }
    }
}
```

## Contributing

If you would like to help implement this component, please see our [contributing guide](https://github.com/FritzAndFriends/BlazorWebFormsComponents/blob/dev/CONTRIBUTING.md).

## See Also

- [CheckBox](CheckBox.md) - Single checkbox control
- [RadioButtonList](RadioButtonList.md) - Radio button selection list
- [DropDownList](DropDownList.md) - Single selection dropdown
- [ListBox](ListBox.md) - Multiple selection list control
