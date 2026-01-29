# CheckBoxList

The CheckBoxList component renders a group of checkboxes that allow users to select multiple items from a list. This component supports both static items and data-bound scenarios.

Original Web Forms documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.checkboxlist?view=netframework-4.8

## Blazor Features Supported

- Static items via `StaticItems` parameter with `ListItemCollection`
- Data binding via `Items` parameter with `DataTextField` and `DataValueField`
- Two-way binding with `@bind-SelectedValues` for multiple selections
- Layout options: Table (default), Flow, OrderedList, UnorderedList
- Repeat direction: Vertical (default) or Horizontal
- `TextAlign` property (Left or Right) for label positioning
- `CellPadding` and `CellSpacing` for table layout
- `OnSelectedIndexChanged` event handler
- Disabled state via `Enabled` parameter
- Style attributes (BackColor, ForeColor, Font, etc.) and CssClass formatting

## WebForms Features Not Supported

- `AutoPostBack` is not supported in Blazor - use `OnSelectedIndexChanged` event instead
- `DataSourceID` is not supported - bind directly to collections via `Items` parameter

## WebForms Syntax

```html
<asp:CheckBoxList
    ID="string"
    runat="server"
    AutoPostBack="True|False"
    CellPadding="integer"
    CellSpacing="integer"
    CssClass="string"
    DataSourceID="string"
    DataTextField="string"
    DataValueField="string"
    Enabled="True|False"
    RepeatColumns="integer"
    RepeatDirection="Horizontal|Vertical"
    RepeatLayout="Table|Flow|OrderedList|UnorderedList"
    TextAlign="Left|Right"
    Visible="True|False"
    OnSelectedIndexChanged="SelectedIndexChanged event handler">
    
    <asp:ListItem Value="value1" Text="Option 1" Selected="True|False" />
    <asp:ListItem Value="value2" Text="Option 2" />
    
</asp:CheckBoxList>
```

## Blazor Syntax

### Static Items

```razor
<CheckBoxList TItem="object" StaticItems="items" @bind-SelectedValues="selectedValues" />

@code {
    private List<string> selectedValues = new();
    
    private ListItemCollection items = new()
    {
        new ListItem("Option One", "1"),
        new ListItem("Option Two", "2"),
        new ListItem("Option Three", "3")
    };
}
```

### Data Binding

```razor
<CheckBoxList TItem="Category"
              Items="categories"
              DataTextField="Name"
              DataValueField="Id"
              @bind-SelectedValues="selectedCategoryIds" />

@code {
    private List<string> selectedCategoryIds = new();
    
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

### With Event Handler

```razor
<CheckBoxList TItem="object"
              StaticItems="items"
              @bind-SelectedValues="selectedValues"
              OnSelectedIndexChanged="HandleSelectionChanged" />

@code {
    private List<string> selectedValues = new();
    
    private void HandleSelectionChanged(ChangeEventArgs e)
    {
        Console.WriteLine($"Selection changed. Total selected: {selectedValues.Count}");
    }
}
```

### Text Alignment

By default, the label appears to the right of the checkbox. Use `TextAlign` to position it on the left:

```razor
<CheckBoxList TItem="object" StaticItems="items" TextAlign="TextAlign.Left" />
```

### Layout Options

#### Flow Layout (no table, items in a span)

```razor
<CheckBoxList TItem="object" 
              StaticItems="items" 
              RepeatLayout="RepeatLayout.Flow"
              RepeatDirection="DataListEnum.Vertical" />
```

#### Unordered List Layout

```razor
<CheckBoxList TItem="object" 
              StaticItems="items" 
              RepeatLayout="RepeatLayout.UnorderedList" />
```

#### Horizontal Direction with Multiple Columns

```razor
<CheckBoxList TItem="object" 
              StaticItems="items" 
              RepeatDirection="DataListEnum.Horizontal"
              RepeatColumns="3" />
```

### With Styling

```razor
<CheckBoxList TItem="object"
              StaticItems="items"
              CssClass="checkbox-list"
              CellPadding="5"
              CellSpacing="2" />
```

## HTML Output

### Table Layout (default, vertical direction)

```html
<table class="custom-class">
    <tr>
        <td><input id="abc_0" type="checkbox" name="abc$0" value="1" /><label for="abc_0">Option 1</label></td>
    </tr>
    <tr>
        <td><input id="abc_1" type="checkbox" name="abc$1" value="2" checked /><label for="abc_1">Option 2</label></td>
    </tr>
</table>
```

### Flow Layout (vertical direction)

```html
<span class="custom-class">
    <input id="abc_0" type="checkbox" name="abc$0" value="1" /><label for="abc_0">Option 1</label><br />
    <input id="abc_1" type="checkbox" name="abc$1" value="2" /><label for="abc_1">Option 2</label>
</span>
```

### Unordered List Layout

```html
<ul class="custom-class">
    <li><input id="abc_0" type="checkbox" name="abc$0" value="1" /><label for="abc_0">Option 1</label></li>
    <li><input id="abc_1" type="checkbox" name="abc$1" value="2" /><label for="abc_1">Option 2</label></li>
</ul>
```

## Key Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `StaticItems` | `ListItemCollection` | empty | Static list items |
| `Items` | `IEnumerable<TItem>` | null | Data-bound items |
| `DataTextField` | `string` | null | Property for display text |
| `DataValueField` | `string` | null | Property for value |
| `SelectedValues` | `List<string>` | empty | All selected values |
| `RepeatColumns` | `int` | 0 | Number of columns (0 = auto) |
| `RepeatDirection` | `DataListEnum` | Vertical | Vertical or Horizontal |
| `RepeatLayout` | `RepeatLayout` | Table | Table, Flow, OrderedList, UnorderedList |
| `TextAlign` | `TextAlign` | Right | Label position (Left or Right) |
| `CellPadding` | `int` | -1 | Padding in table layout |
| `CellSpacing` | `int` | -1 | Spacing in table layout |

## Key Differences from Web Forms

1. **Type Parameter**: Blazor CheckBoxList requires a `TItem` type parameter for data binding
2. **Property Names**: Use `StaticItems` for the item collection (not `Items`), as `Items` is reserved for data-bound scenarios
3. **Two-way Binding**: Use `@bind-SelectedValues` (returns `List<string>`) for automatic two-way binding
4. **RepeatDirection**: Uses `DataListEnum.Horizontal` / `DataListEnum.Vertical` instead of enum
5. **No AutoPostBack**: Blazor's event model doesn't require postback; events fire immediately

## Accessing Selected Items

```csharp
var checkboxList = // reference to CheckBoxList component

// Get all selected values
List<string> values = checkboxList.SelectedValues;

// Get first selected value
string firstValue = checkboxList.SelectedValue;

// Get all selected items
IEnumerable<ListItem> selectedItems = checkboxList.SelectedItems;

// Get first selected item
ListItem firstItem = checkboxList.SelectedItem;

// Get first selected index
int index = checkboxList.SelectedIndex;
```

## Migration Notes

When migrating from Web Forms to Blazor:

1. Remove the `asp:` prefix and `runat="server"` attribute
2. Add the `TItem="object"` type parameter
3. Replace `AutoPostBack="true"` with the `OnSelectedIndexChanged` event handler
4. Use `@bind-SelectedValues` for two-way data binding
5. Remove `<asp:ListItem>` tags and define items in code-behind as `ListItemCollection`

### Before (Web Forms):

```html
<asp:CheckBoxList ID="cblCategories" 
                  runat="server"
                  RepeatDirection="Vertical"
                  AutoPostBack="true"
                  OnSelectedIndexChanged="Categories_Changed">
    <asp:ListItem Value="1" Text="Electronics" />
    <asp:ListItem Value="2" Text="Clothing" Selected="True" />
    <asp:ListItem Value="3" Text="Books" />
</asp:CheckBoxList>
```

### After (Blazor):

```razor
<CheckBoxList TItem="object"
              StaticItems="categoryItems"
              @bind-SelectedValues="selectedCategories"
              OnSelectedIndexChanged="HandleCategoryChange" />

@code {
    private List<string> selectedCategories = new() { "2" };
    
    private ListItemCollection categoryItems = new()
    {
        new ListItem("Electronics", "1"),
        new ListItem("Clothing", "2"),
        new ListItem("Books", "3")
    };
    
    private void HandleCategoryChange(ChangeEventArgs e)
    {
        // Handle the change
    }
}
```

## See Also

- [CheckBox](CheckBox.md) - Single checkbox control
- [DropDownList](DropDownList.md) - Single-select dropdown
- [RadioButtonList](RadioButtonList.md) - Single-select radio buttons (planned)
- [ListBox](ListBox.md) - Multiple selection list control (planned)
