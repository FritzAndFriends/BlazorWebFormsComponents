# ListBox

The ListBox component renders an HTML `<select>` element with a `size` attribute that displays multiple rows, allowing users to select one or more items from a list. This component supports both static items and data-bound scenarios.

Original Web Forms documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.listbox?view=netframework-4.8

## Blazor Features Supported

- Static items via `StaticItems` parameter with `ListItem` collection
- Data binding via `Items` parameter with `DataTextField` and `DataValueField`
- Two-way binding with `@bind-SelectedValue` and `@bind-SelectedValues`
- Rows property controls visible row count (maps to `size` attribute)
- Single or Multiple selection modes via `SelectionMode` parameter
- Selected item tracking via `SelectedValue`, `SelectedValues`, and `SelectedIndex`
- OnSelectedIndexChanged event handler
- Disabled state via `Enabled` parameter
- Disabled items via `ListItem.Enabled` property
- Style attributes (BackColor, ForeColor, Font, etc.) and CssClass formatting
- Access to `SelectedItem` and `SelectedItems` properties

## WebForms Features Not Supported

- AutoPostBack is not supported in Blazor - use `OnSelectedIndexChanged` event instead
- AppendDataBoundItems is not implemented
- DataSourceID is not supported - bind directly to collections via `Items` parameter

## WebForms Syntax

```html
<asp:ListBox
    AccessKey="string"
    AutoPostBack="True|False"
    BackColor="color name|#dddddd"
    BorderColor="color name|#dddddd"
    BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|Groove|Ridge|Inset|Outset"
    BorderWidth="size"
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
    Rows="integer"
    runat="server"
    SelectionMode="Single|Multiple"
    SelectedIndex="integer"
    SelectedValue="string"
    TabIndex="integer"
    ToolTip="string"
    Visible="True|False"
    Width="size">
    
    <asp:ListItem Value="value1" Text="Display Text 1" Selected="True|False" />
    <asp:ListItem Value="value2" Text="Display Text 2" />
    
</asp:ListBox>
```

## Blazor Syntax

### Static Items (Single Selection)

```razor
<ListBox TItem="object" StaticItems="items" @bind-SelectedValue="selectedValue" />

@code {
    private string selectedValue = "";
    
    private ListItemCollection items = new()
    {
        new ListItem("Apple", "1"),
        new ListItem("Banana", "2"),
        new ListItem("Cherry", "3"),
        new ListItem("Date", "4")
    };
}
```

### Multiple Selection

```razor
@using BlazorWebFormsComponents.Enums

<ListBox TItem="object"
         StaticItems="items"
         SelectionMode="ListSelectionMode.Multiple"
         @bind-SelectedValues="selectedValues" />

@code {
    private List<string> selectedValues = new();
    
    private ListItemCollection items = new()
    {
        new ListItem("Red", "r"),
        new ListItem("Green", "g"),
        new ListItem("Blue", "b")
    };
}
```

### Custom Row Count

```razor
<ListBox TItem="object" StaticItems="items" Rows="6" @bind-SelectedValue="selectedValue" />
```

### Data Binding

```razor
<ListBox TItem="Product"
         Items="products"
         DataTextField="Name"
         DataValueField="Id"
         @bind-SelectedValue="selectedProductId" />

@code {
    private string selectedProductId = "";
    
    private List<Product> products = new()
    {
        new Product { Id = "1", Name = "Widget" },
        new Product { Id = "2", Name = "Gadget" },
        new Product { Id = "3", Name = "Gizmo" }
    };
    
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
```

### With Event Handler

```razor
<ListBox TItem="object"
         StaticItems="items"
         @bind-SelectedValue="selectedValue"
         OnSelectedIndexChanged="HandleSelectionChanged" />

@code {
    private string selectedValue = "";
    
    private void HandleSelectionChanged(ChangeEventArgs e)
    {
        var newValue = e.Value?.ToString();
        Console.WriteLine($"Selection changed to: {newValue}");
    }
}
```

### With Styling

```razor
<ListBox TItem="object"
         StaticItems="items"
         CssClass="form-select"
         BackColor="new WebColor(System.Drawing.Color.LightYellow)"
         Font="new FontInfo { Bold = true }" />
```

## Key Differences from Web Forms

1. **Type Parameter**: Blazor ListBox requires a `TItem` type parameter for data binding
2. **Property Names**: Use `StaticItems` for the item collection (not `Items`), as `Items` is reserved for data-bound scenarios
3. **Two-way Binding**: Use `@bind-SelectedValue` or `@bind-SelectedValues` for automatic two-way binding
4. **Events**: Use `OnSelectedIndexChanged` with `ChangeEventArgs` instead of specialized event args
5. **No AutoPostBack**: Blazor's event model doesn't require postback; events fire immediately
6. **Selection Mode Enum**: Use `ListSelectionMode.Single` or `ListSelectionMode.Multiple` from `BlazorWebFormsComponents.Enums`

## Key Differences from DropDownList

| Feature | DropDownList | ListBox |
|---------|--------------|---------|
| Display | Single visible row | Multiple rows (`size` attribute) |
| Selection | Single only | Single or Multiple |
| HTML | `<select>` | `<select size="N">` |
| Multiple | Not supported | `<select multiple>` via `SelectionMode` |
| Rows | N/A | Configurable via `Rows` property |

## ListItem and ListItemCollection

The `ListItem` class represents individual items in the listbox:

```csharp
public class ListItem
{
    public string Text { get; set; }
    public string Value { get; set; }
    public bool Selected { get; set; }
    public bool Enabled { get; set; } = true;
    
    public ListItem(string text)
    public ListItem(string text, string value)
    public ListItem(string text, string value, bool selected)
}
```

The `ListItemCollection` class provides helper methods:

```csharp
public class ListItemCollection : List<ListItem>
{
    public ListItem FindByValue(string value)
    public ListItem FindByText(string text)
}
```

## ListSelectionMode Enum

```csharp
public enum ListSelectionMode
{
    Single,    // Only one item can be selected
    Multiple   // Multiple items can be selected
}
```

## Common Patterns

### Pre-select an Item

```razor
<ListBox TItem="object" StaticItems="items" SelectedValue="2" />
```

### Pre-select Multiple Items

```razor
<ListBox TItem="object"
         StaticItems="items"
         SelectionMode="ListSelectionMode.Multiple"
         SelectedValues="@(new List<string> { "1", "3" })" />
```

### Disabled ListBox

```razor
<ListBox TItem="object" StaticItems="items" Enabled="false" />
```

### Disabled Individual Items

```razor
@code {
    private ListItemCollection items = new()
    {
        new ListItem("Available", "1") { Enabled = true },
        new ListItem("Unavailable", "2") { Enabled = false }
    };
}
```

### Get Selected Item Details

```csharp
var listbox = // reference to ListBox component

// Single selection
var selectedItem = listbox.SelectedItem;
var text = selectedItem?.Text;
var value = selectedItem?.Value;

// Multiple selection
foreach (var item in listbox.SelectedItems)
{
    Console.WriteLine($"{item.Text}: {item.Value}");
}
```

## Migration Tips

When migrating from Web Forms:

1. Replace `<asp:ListBox>` with `<ListBox TItem="object">`
2. Rename any `Items` parameter to `StaticItems`
3. Replace `AutoPostBack="true"` with `OnSelectedIndexChanged` event handler
4. Use `@bind-SelectedValue` or `@bind-SelectedValues` instead of manually managing selection
5. Remove `runat="server"` attribute
6. Remove `<asp:ListItem>` tags and define items in code-behind as `ListItemCollection`
7. Add `@using BlazorWebFormsComponents.Enums` if using `SelectionMode`

## See Also

- [DropDownList](DropDownList.md) - Single selection drop-down control
- [CheckBoxList](CheckBoxList.md) - Multiple checkboxes in a list
- [RadioButtonList](RadioButtonList.md) - Radio button selection list
