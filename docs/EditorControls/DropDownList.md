# DropDownList

The DropDownList component renders an HTML `<select>` element that allows users to select a single item from a drop-down list. This component supports both static items and data-bound scenarios.

Original Web Forms documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.dropdownlist?view=netframework-4.8

## Blazor Features Supported

- Static items via `StaticItems` parameter with `ListItem` collection
- Data binding via `Items` parameter with `DataTextField` and `DataValueField`
- Two-way binding with `@bind-SelectedValue`
- Selected item tracking via `SelectedValue` and `SelectedIndex`
- OnSelectedIndexChanged event handler
- Disabled state via `Enabled` parameter
- Style attributes (BackColor, ForeColor, Font, etc.) and CssClass formatting
- Access to `SelectedItem` property

## WebForms Features Not Supported

- AutoPostBack is not supported in Blazor - use `OnSelectedIndexChanged` event instead
- AppendDataBoundItems is not implemented
- DataSourceID is not supported - bind directly to collections via `Items` parameter

## WebForms Syntax

```html
<asp:DropDownList
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
    runat="server"
    SelectedIndex="number"
    SelectedValue="string"
    TabIndex="integer"
    ToolTip="string"
    Visible="True|False"
    Width="size">
    
    <asp:ListItem Value="value1" Text="Display Text 1" Selected="True|False" />
    <asp:ListItem Value="value2" Text="Display Text 2" />
    
</asp:DropDownList>
```

## Blazor Syntax

### Static Items

```razor
<DropDownList TItem="object" StaticItems="items" @bind-SelectedValue="selectedValue" />

@code {
    private string selectedValue = "";
    
    private ListItemCollection items = new()
    {
        new ListItem("Select...", ""),
        new ListItem("Option One", "1"),
        new ListItem("Option Two", "2"),
        new ListItem("Option Three", "3")
    };
}
```

### Data Binding

```razor
<DropDownList TItem="Product"
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
<DropDownList TItem="object"
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
<DropDownList TItem="object"
              StaticItems="items"
              CssClass="form-select"
              BackColor="new WebColor(System.Drawing.Color.LightYellow)"
              Font="new FontInfo { Bold = true }" />
```

## Key Differences from Web Forms

1. **Type Parameter**: Blazor DropDownList requires a `TItem` type parameter for data binding
2. **Property Names**: Use `StaticItems` for the item collection (not `Items`), as `Items` is reserved for data-bound scenarios
3. **Two-way Binding**: Use `@bind-SelectedValue` for automatic two-way binding
4. **Events**: Use `OnSelectedIndexChanged` with `ChangeEventArgs` instead of specialized event args
5. **No AutoPostBack**: Blazor's event model doesn't require postback; events fire immediately

## ListItem and ListItemCollection

The `ListItem` class represents individual items in the dropdown:

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

## Common Patterns

### Pre-select an Item

```razor
<DropDownList TItem="object" StaticItems="items" SelectedValue="2" />
```

### Disabled DropDownList

```razor
<DropDownList TItem="object" StaticItems="items" Enabled="false" />
```

### Get Selected Item Details

```csharp
var dropdown = // reference to DropDownList component
var selectedItem = dropdown.SelectedItem;
var text = selectedItem?.Text;
var value = selectedItem?.Value;
```

## Migration Tips

When migrating from Web Forms:

1. Replace `<asp:DropDownList>` with `<DropDownList TItem="object">`
2. Rename any `Items` parameter to `StaticItems`
3. Replace `AutoPostBack="true"` with `OnSelectedIndexChanged` event handler
4. Use `@bind-SelectedValue` instead of manually managing `SelectedValue`
5. Remove `runat="server"` attribute
6. Remove `<asp:ListItem>` tags and define items in code-behind as `ListItemCollection`

## See Also

- [ListBox](ListBox.md) - Multiple selection list control
- [CheckBoxList](CheckBoxList.md) - Multiple checkboxes in a list
- [RadioButtonList](RadioButtonList.md) - Radio button selection list
