# RadioButtonList

The RadioButtonList component renders a group of radio buttons that allow users to select a single item from a list. This component supports both static items and data-bound scenarios with various layout options.

Original Web Forms documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.radiobuttonlist?view=netframework-4.8

## Blazor Features Supported

- Static items via `StaticItems` parameter with `ListItem` collection
- Data binding via `Items` parameter with `DataTextField` and `DataValueField`
- Two-way binding with `@bind-SelectedValue`
- Selected item tracking via `SelectedValue` and `SelectedIndex`
- OnSelectedIndexChanged event handler
- RepeatLayout options: Table, Flow, OrderedList, UnorderedList
- RepeatDirection options: Horizontal, Vertical
- TextAlign for label positioning (Left or Right)
- Disabled state via `Enabled` parameter
- Per-item disabled state via `ListItem.Enabled`
- Style attributes (BackColor, ForeColor, Font, etc.) and CssClass formatting
- CellPadding and CellSpacing for table layout
- Access to `SelectedItem` property

## WebForms Features Not Supported

- AutoPostBack is not supported in Blazor - use `OnSelectedIndexChanged` event instead
- AppendDataBoundItems is not implemented
- DataSourceID is not supported - bind directly to collections via `Items` parameter
- RepeatColumns is not fully implemented

## WebForms Syntax

```html
<asp:RadioButtonList
    ID="rbl1"
    runat="server"
    RepeatDirection="Vertical|Horizontal"
    RepeatLayout="Table|Flow|OrderedList|UnorderedList"
    TextAlign="Left|Right"
    CellPadding="integer"
    CellSpacing="integer"
    CssClass="string"
    Enabled="True|False"
    Visible="True|False"
    SelectedValue="string"
    SelectedIndex="integer"
    AutoPostBack="True|False"
    OnSelectedIndexChanged="SelectedIndexChanged event handler"
    DataTextField="string"
    DataValueField="string">
    
    <asp:ListItem Value="S" Text="Small" />
    <asp:ListItem Value="M" Text="Medium" Selected="True" />
    <asp:ListItem Value="L" Text="Large" />
    
</asp:RadioButtonList>
```

**Rendered HTML (Table layout, Vertical direction):**
```html
<table id="rbl1">
    <tr>
        <td><input id="rbl1_0" type="radio" name="rbl1" value="S" /><label for="rbl1_0">Small</label></td>
    </tr>
    <tr>
        <td><input id="rbl1_1" type="radio" name="rbl1" value="M" checked="checked" /><label for="rbl1_1">Medium</label></td>
    </tr>
    <tr>
        <td><input id="rbl1_2" type="radio" name="rbl1" value="L" /><label for="rbl1_2">Large</label></td>
    </tr>
</table>
```

## Blazor Syntax

### Static Items

```razor
<RadioButtonList TItem="object" StaticItems="items" @bind-SelectedValue="selectedSize" />

@code {
    private string selectedSize = "M";
    
    private ListItemCollection items = new()
    {
        new ListItem("Small", "S"),
        new ListItem("Medium", "M"),
        new ListItem("Large", "L")
    };
}
```

### Data Binding

```razor
<RadioButtonList TItem="Size"
                 Items="sizes"
                 DataTextField="Name"
                 DataValueField="Code"
                 @bind-SelectedValue="selectedSizeCode" />

@code {
    private string selectedSizeCode = "";
    
    private List<Size> sizes = new()
    {
        new Size { Code = "S", Name = "Small" },
        new Size { Code = "M", Name = "Medium" },
        new Size { Code = "L", Name = "Large" },
        new Size { Code = "XL", Name = "Extra Large" }
    };
    
    public class Size
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
```

### With Event Handler

```razor
<RadioButtonList TItem="object"
                 StaticItems="items"
                 @bind-SelectedValue="selectedValue"
                 OnSelectedIndexChanged="HandleSelectionChanged" />

@code {
    private string selectedValue = "";
    
    private void HandleSelectionChanged(ChangeEventArgs e)
    {
        Console.WriteLine($"Selection changed to: {selectedValue}");
    }
}
```

### Different Layouts

```razor
@* Flow layout - inline radio buttons *@
<RadioButtonList TItem="object"
                 StaticItems="items"
                 RepeatLayout="RepeatLayout.Flow"
                 RepeatDirection="RepeatDirection.Horizontal" />

@* Unordered list layout *@
<RadioButtonList TItem="object"
                 StaticItems="items"
                 RepeatLayout="RepeatLayout.UnorderedList" />

@* Ordered list layout *@
<RadioButtonList TItem="object"
                 StaticItems="items"
                 RepeatLayout="RepeatLayout.OrderedList" />
```

### Label Position

```razor
@* Label before radio button (left-aligned text) *@
<RadioButtonList TItem="object"
                 StaticItems="items"
                 TextAlign="TextAlign.Left" />

@* Label after radio button (right-aligned text, default) *@
<RadioButtonList TItem="object"
                 StaticItems="items"
                 TextAlign="TextAlign.Right" />
```

### With Styling

```razor
<RadioButtonList TItem="object"
                 StaticItems="items"
                 CssClass="custom-radio-list"
                 BackColor="new WebColor(System.Drawing.Color.LightGray)"
                 CellPadding="5"
                 CellSpacing="2" />
```

## Key Differences from Web Forms

1. **Type Parameter**: Blazor RadioButtonList requires a `TItem` type parameter for data binding
2. **Property Names**: Use `StaticItems` for the item collection (not `Items`), as `Items` is reserved for data-bound scenarios
3. **Two-way Binding**: Use `@bind-SelectedValue` for automatic two-way binding
4. **Events**: Use `OnSelectedIndexChanged` with `ChangeEventArgs` instead of specialized event args
5. **No AutoPostBack**: Blazor's event model doesn't require postback; events fire immediately
6. **Mutual Exclusion**: All radio buttons automatically share the same `name` attribute for proper mutual exclusion

## Key Difference from CheckBoxList

RadioButtonList renders radio buttons with a **shared `name` attribute** for mutual exclusion (only one can be selected), while CheckBoxList uses checkboxes with unique names allowing multiple selections:

```html
<!-- RadioButtonList - all inputs have same name for mutual exclusion -->
<input type="radio" name="rbl1" value="S" />
<input type="radio" name="rbl1" value="M" checked />
<input type="radio" name="rbl1" value="L" />

<!-- CheckBoxList - each input has unique name -->
<input type="checkbox" name="cbl1$0" value="1" />
<input type="checkbox" name="cbl1$1" value="2" />
<input type="checkbox" name="cbl1$2" value="3" />
```

## Common Patterns

### Pre-select an Item

```razor
<RadioButtonList TItem="object" StaticItems="items" SelectedValue="M" />
```

### Disabled RadioButtonList

```razor
<RadioButtonList TItem="object" StaticItems="items" Enabled="false" />
```

### Disable Individual Items

```razor
@code {
    private ListItemCollection items = new()
    {
        new ListItem("Available", "1") { Enabled = true },
        new ListItem("Unavailable", "2") { Enabled = false },
        new ListItem("Available", "3") { Enabled = true }
    };
}
```

### Get Selected Item Details

```csharp
var radioList = // reference to RadioButtonList component
var selectedItem = radioList.SelectedItem;
var text = selectedItem?.Text;
var value = selectedItem?.Value;
```

## Migration Tips

When migrating from Web Forms:

1. Replace `<asp:RadioButtonList>` with `<RadioButtonList TItem="object">`
2. Rename any `Items` parameter to `StaticItems`
3. Replace `AutoPostBack="true"` with `OnSelectedIndexChanged` event handler
4. Use `@bind-SelectedValue` instead of manually managing `SelectedValue`
5. Remove `runat="server"` attribute
6. Remove `<asp:ListItem>` tags and define items in code-behind as `ListItemCollection`
7. Update `RepeatLayout` and `RepeatDirection` to use the enum syntax (e.g., `RepeatLayout.Table`)

## See Also

- [RadioButton](RadioButton.md) - Individual radio button control
- [DropDownList](DropDownList.md) - Single selection dropdown
- [CheckBox](CheckBox.md) - Individual checkbox control
