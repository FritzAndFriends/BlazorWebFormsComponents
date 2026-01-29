# ListBox

The ListBox component renders an HTML `<select>` element with the `multiple` attribute or `size` attribute, allowing users to view and select one or more items from a scrollable list.

Original Web Forms documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.listbox?view=netframework-4.8

!!! warning "Not Yet Implemented"
    This component is not yet available in BlazorWebFormsComponents. This documentation is a placeholder for the planned implementation.

## Planned Features

- Static items via `StaticItems` parameter with `ListItem` collection
- Data binding via `Items` parameter with `DataTextField` and `DataValueField`
- Single and multiple selection modes via `SelectionMode` property
- `Rows` property to control visible height
- Two-way binding with `@bind-SelectedValue` (single selection)
- Selected items collection for multiple selection
- `OnSelectedIndexChanged` event handler
- Style attributes and CssClass formatting

## Web Forms Declarative Syntax

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
    SelectedIndex="integer"
    SelectedValue="string"
    SelectionMode="Single|Multiple"
    TabIndex="integer"
    ToolTip="string"
    Visible="True|False"
    Width="size">
    
    <asp:ListItem Value="value1" Text="Display Text 1" Selected="True|False" />
    <asp:ListItem Value="value2" Text="Display Text 2" />
    
</asp:ListBox>
```

## Proposed Blazor Syntax

### Single Selection

```razor
<ListBox TItem="object" 
         StaticItems="items" 
         Rows="5"
         @bind-SelectedValue="selectedValue" />

@code {
    private string selectedValue = "";
    
    private ListItemCollection items = new()
    {
        new ListItem("Option One", "1"),
        new ListItem("Option Two", "2"),
        new ListItem("Option Three", "3"),
        new ListItem("Option Four", "4"),
        new ListItem("Option Five", "5")
    };
}
```

### Multiple Selection

```razor
<ListBox TItem="object" 
         StaticItems="items" 
         SelectionMode="ListSelectionMode.Multiple"
         Rows="5"
         SelectedValues="selectedValues" />

@code {
    private List<string> selectedValues = new();
}
```

## Contributing

If you would like to help implement this component, please see our [contributing guide](https://github.com/FritzAndFriends/BlazorWebFormsComponents/blob/dev/CONTRIBUTING.md).

## See Also

- [DropDownList](DropDownList.md) - Single selection dropdown
- [CheckBoxList](CheckBoxList.md) - Multiple checkboxes in a list
- [RadioButtonList](RadioButtonList.md) - Radio button selection list
