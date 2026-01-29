# RadioButtonList

The RadioButtonList component renders a group of radio buttons, allowing users to select a single item from a list of mutually exclusive options. It supports both static items and data-bound scenarios.

Original Web Forms documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.radiobuttonlist?view=netframework-4.8

!!! warning "Not Yet Implemented"
    This component is not yet available in BlazorWebFormsComponents. This documentation is a placeholder for the planned implementation.

## Planned Features

- Static items via `StaticItems` parameter with `ListItem` collection
- Data binding via `Items` parameter with `DataTextField` and `DataValueField`
- Single selection with `SelectedValue` and `SelectedIndex`
- Two-way binding with `@bind-SelectedValue`
- `RepeatColumns` for multi-column layout
- `RepeatDirection` (Vertical or Horizontal)
- `RepeatLayout` (Table, Flow, OrderedList, UnorderedList)
- `TextAlign` for label positioning (Left or Right)
- `OnSelectedIndexChanged` event handler
- Style attributes and CssClass formatting

## Web Forms Declarative Syntax

```html
<asp:RadioButtonList
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
    
</asp:RadioButtonList>
```

## Proposed Blazor Syntax

### Basic Usage

```razor
<RadioButtonList TItem="object" 
                 StaticItems="items"
                 @bind-SelectedValue="selectedValue" />

@code {
    private string selectedValue = "";
    
    private ListItemCollection items = new()
    {
        new ListItem("Small", "S"),
        new ListItem("Medium", "M"),
        new ListItem("Large", "L"),
        new ListItem("Extra Large", "XL")
    };
}
```

### Horizontal Layout

```razor
<RadioButtonList TItem="object" 
                 StaticItems="items"
                 RepeatDirection="RepeatDirection.Horizontal"
                 @bind-SelectedValue="selectedValue" />
```

### Multi-Column Layout

```razor
<RadioButtonList TItem="object" 
                 StaticItems="items"
                 RepeatColumns="2"
                 RepeatDirection="RepeatDirection.Vertical"
                 @bind-SelectedValue="selectedValue" />
```

### Data Binding

```razor
<RadioButtonList TItem="ShippingMethod"
                 Items="shippingMethods"
                 DataTextField="Name"
                 DataValueField="Code"
                 @bind-SelectedValue="selectedShippingCode" />

@code {
    private string selectedShippingCode = "standard";
    
    private List<ShippingMethod> shippingMethods = new()
    {
        new ShippingMethod { Code = "standard", Name = "Standard Shipping (5-7 days)" },
        new ShippingMethod { Code = "express", Name = "Express Shipping (2-3 days)" },
        new ShippingMethod { Code = "overnight", Name = "Overnight Shipping" }
    };
    
    public class ShippingMethod
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
    private string selectedValue = "M";
    
    private void HandleSelectionChanged()
    {
        Console.WriteLine($"Selection changed to: {selectedValue}");
    }
}
```

## Key Differences from Individual RadioButtons

The RadioButtonList is preferred when:

- You have a dynamic list of options
- You want automatic grouping (no need for `GroupName`)
- You need layout control (columns, direction)
- You want data binding support

Use individual [RadioButton](RadioButton.md) components when:

- You have a fixed, small number of options
- You need custom layout or styling per button
- Options are spread across different parts of the UI

## Contributing

If you would like to help implement this component, please see our [contributing guide](https://github.com/FritzAndFriends/BlazorWebFormsComponents/blob/dev/CONTRIBUTING.md).

## See Also

- [RadioButton](RadioButton.md) - Single radio button control
- [CheckBoxList](CheckBoxList.md) - Multiple selection checkbox list
- [DropDownList](DropDownList.md) - Single selection dropdown
- [ListBox](ListBox.md) - Multiple selection list control
