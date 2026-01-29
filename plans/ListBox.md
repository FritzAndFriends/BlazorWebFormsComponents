# ListBox Component Implementation Plan

## Overview
Implement the ListBox component that emulates ASP.NET Web Forms `<asp:ListBox>` control. ListBox renders an HTML `<select>` element with the `size` attribute set to display multiple rows, and optionally allows multiple selection.

## Web Forms Reference
- **Documentation**: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.listbox?view=netframework-4.8
- **Namespace**: `System.Web.UI.WebControls`

## HTML Output

**Web Forms (Single Select):**
```html
<asp:ListBox ID="lb1" Rows="4" runat="server">
    <asp:ListItem Text="Apple" Value="1" />
    <asp:ListItem Text="Banana" Value="2" />
    <asp:ListItem Text="Cherry" Value="3" />
    <asp:ListItem Text="Date" Value="4" />
</asp:ListBox>
```

**Rendered HTML:**
```html
<select id="lb1" name="lb1" size="4">
    <option value="1">Apple</option>
    <option value="2">Banana</option>
    <option value="3">Cherry</option>
    <option value="4">Date</option>
</select>
```

**Web Forms (Multiple Select):**
```html
<asp:ListBox ID="lb1" Rows="4" SelectionMode="Multiple" runat="server">
    ...
</asp:ListBox>
```

**Rendered HTML:**
```html
<select id="lb1" name="lb1" size="4" multiple="multiple">
    ...
</select>
```

## Key Properties to Implement

| Property | Type | Description |
|----------|------|-------------|
| `StaticItems` | `ListItemCollection` | Static list items |
| `Items` | `IEnumerable<TItem>` | Data-bound items |
| `DataTextField` | `string` | Property for display text |
| `DataValueField` | `string` | Property for value |
| `Rows` | `int` | Number of visible rows (maps to `size` attribute) |
| `SelectionMode` | `ListSelectionMode` | Single or Multiple |
| `SelectedValue` | `string` | First selected value |
| `SelectedValues` | `List<string>` | All selected values (for multiple mode) |
| `SelectedIndex` | `int` | First selected index |
| All `IStyle` properties | various | BackColor, ForeColor, CssClass, etc. |

## Enum Needed

### ListSelectionMode
```csharp
public enum ListSelectionMode
{
    Single,
    Multiple
}
```

## Implementation Tasks

- [ ] Create `src/BlazorWebFormsComponents/ListBox.razor`
- [ ] Create `src/BlazorWebFormsComponents/ListBox.razor.cs`
- [ ] Add `ListSelectionMode` enum in `Enums/` folder
- [ ] Create unit tests in `src/BlazorWebFormsComponents.Test/ListBox/`
  - [ ] StaticItems.razor - static item rendering
  - [ ] DataBinding.razor - data-bound items
  - [ ] Rows.razor - size attribute
  - [ ] SingleSelect.razor - single selection mode
  - [ ] MultipleSelect.razor - multiple selection mode
  - [ ] SelectedValue.razor - selection tracking
  - [ ] Style.razor - styling
  - [ ] Disabled.razor - enabled/disabled state
- [ ] Create documentation in `docs/EditorControls/ListBox.md`
- [ ] Update `mkdocs.yml` nav section

## Code Structure

### ListBox.razor
```razor
@using BlazorWebFormsComponents.DataBinding
@typeparam TItem
@inherits DataBoundComponent<TItem>

@if (Visible)
{
    <select class="@CssClass" 
            style="@Style"
            size="@Rows"
            multiple="@(SelectionMode == ListSelectionMode.Multiple)"
            disabled="@(!Enabled)" 
            @onchange="HandleChange">
        @foreach (var item in GetItems())
        {
            <option value="@item.Value" 
                    selected="@IsSelected(item.Value)"
                    disabled="@(!item.Enabled)">
                @item.Text
            </option>
        }
    </select>
}
```

### ListBox.razor.cs (key parts)
```csharp
public partial class ListBox<TItem> : DataBoundComponent<TItem>, IStyle
{
    [Parameter]
    public ListItemCollection StaticItems { get; set; } = new();

    [Parameter]
    public string DataTextField { get; set; }

    [Parameter]
    public string DataValueField { get; set; }

    [Parameter]
    public int Rows { get; set; } = 4;

    [Parameter]
    public ListSelectionMode SelectionMode { get; set; } = ListSelectionMode.Single;

    [Parameter]
    public string SelectedValue { get; set; }

    [Parameter]
    public EventCallback<string> SelectedValueChanged { get; set; }

    [Parameter]
    public List<string> SelectedValues { get; set; } = new();

    [Parameter]
    public EventCallback<List<string>> SelectedValuesChanged { get; set; }

    [Parameter]
    public int SelectedIndex { get; set; } = -1;

    [Parameter]
    public EventCallback<int> SelectedIndexChanged { get; set; }

    [Parameter]
    public EventCallback<ChangeEventArgs> OnSelectedIndexChanged { get; set; }

    // IStyle properties (same as DropDownList)
    [Parameter] public WebColor BackColor { get; set; }
    [Parameter] public WebColor BorderColor { get; set; }
    [Parameter] public BorderStyle BorderStyle { get; set; }
    [Parameter] public Unit BorderWidth { get; set; }
    [Parameter] public string CssClass { get; set; }
    [Parameter] public FontInfo Font { get; set; } = new FontInfo();
    [Parameter] public WebColor ForeColor { get; set; }
    [Parameter] public Unit Height { get; set; }
    [Parameter] public Unit Width { get; set; }

    protected string Style => this.ToStyle().NullIfEmpty();

    public ListItem SelectedItem => GetItems().FirstOrDefault(i => i.Value == SelectedValue);

    public IEnumerable<ListItem> SelectedItems => 
        GetItems().Where(i => SelectedValues.Contains(i.Value));

    private bool IsSelected(string value)
    {
        if (SelectionMode == ListSelectionMode.Multiple)
            return SelectedValues.Contains(value);
        return value == SelectedValue;
    }

    private async Task HandleChange(ChangeEventArgs e)
    {
        if (SelectionMode == ListSelectionMode.Multiple)
        {
            // For multiple selection, get all selected values from JS interop
            // or use Blazor's built-in multiple select handling
            var selectedValues = e.Value as string[] ?? new[] { e.Value?.ToString() };
            SelectedValues = selectedValues.ToList();
            await SelectedValuesChanged.InvokeAsync(SelectedValues);
            SelectedValue = SelectedValues.FirstOrDefault();
        }
        else
        {
            SelectedValue = e.Value?.ToString();
            SelectedValues = string.IsNullOrEmpty(SelectedValue) 
                ? new List<string>() 
                : new List<string> { SelectedValue };
        }
        
        await SelectedValueChanged.InvokeAsync(SelectedValue);

        var items = GetItems().ToList();
        SelectedIndex = items.FindIndex(i => i.Value == SelectedValue);
        await SelectedIndexChanged.InvokeAsync(SelectedIndex);
        
        await OnSelectedIndexChanged.InvokeAsync(e);
    }

    private IEnumerable<ListItem> GetItems()
    {
        foreach (var item in StaticItems)
            yield return item;

        if (Items != null)
        {
            foreach (var dataItem in Items)
            {
                yield return new ListItem
                {
                    Text = GetPropertyValue(dataItem, DataTextField),
                    Value = GetPropertyValue(dataItem, DataValueField)
                };
            }
        }
    }

    private string GetPropertyValue(TItem item, string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
            return item?.ToString() ?? string.Empty;

        var prop = typeof(TItem).GetProperty(propertyName);
        return prop?.GetValue(item)?.ToString() ?? string.Empty;
    }
}
```

## Key Differences from DropDownList

| Feature | DropDownList | ListBox |
|---------|--------------|---------|
| Display | Single visible row | Multiple rows (`size` attribute) |
| Selection | Single only | Single or Multiple |
| HTML | `<select>` | `<select size="N">` |
| Multiple | Not supported | `<select multiple>` |

## Estimated Effort
- Complexity: **Medium**
- Estimated time: **3-5 hours**

## Acceptance Criteria
1. Renders as `<select>` with `size` attribute
2. Rows property controls visible row count
3. Single selection mode allows one selection
4. Multiple selection mode allows multiple selections
5. Static items work via StaticItems property
6. Data binding works via Items, DataTextField, DataValueField
7. SelectedValue tracks first selected value
8. SelectedValues tracks all selections in Multiple mode
9. Two-way binding works with @bind-SelectedValue and @bind-SelectedValues
10. Disabled items render with disabled attribute
11. All style properties work
12. All unit tests pass

## Related Components
- **DropDownList** - Similar but single row, single select (already complete)
- **CheckBoxList** - Multi-select with checkbox UI
- **RadioButtonList** - Single-select with radio button UI
