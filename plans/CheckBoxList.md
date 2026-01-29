# CheckBoxList Component Implementation Plan

## Overview
Implement the CheckBoxList component that emulates ASP.NET Web Forms `<asp:CheckBoxList>` control. CheckBoxList renders a group of checkboxes for multi-select scenarios.

## Web Forms Reference
- **Documentation**: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.checkboxlist?view=netframework-4.8
- **Namespace**: `System.Web.UI.WebControls`

## HTML Output

**Web Forms:**
```html
<asp:CheckBoxList ID="cbl1" runat="server" RepeatDirection="Vertical">
    <asp:ListItem Text="Option 1" Value="1" />
    <asp:ListItem Text="Option 2" Value="2" Selected="True" />
    <asp:ListItem Text="Option 3" Value="3" />
</asp:CheckBoxList>
```

**Rendered HTML (RepeatLayout=Table, default):**
```html
<table id="cbl1">
    <tr>
        <td><input id="cbl1_0" type="checkbox" name="cbl1$0" value="1" /><label for="cbl1_0">Option 1</label></td>
    </tr>
    <tr>
        <td><input id="cbl1_1" type="checkbox" name="cbl1$1" value="2" checked="checked" /><label for="cbl1_1">Option 2</label></td>
    </tr>
    <tr>
        <td><input id="cbl1_2" type="checkbox" name="cbl1$2" value="3" /><label for="cbl1_2">Option 3</label></td>
    </tr>
</table>
```

**Rendered HTML (RepeatLayout=Flow):**
```html
<span id="cbl1">
    <input id="cbl1_0" type="checkbox" name="cbl1$0" value="1" /><label for="cbl1_0">Option 1</label><br />
    <input id="cbl1_1" type="checkbox" name="cbl1$1" value="2" checked="checked" /><label for="cbl1_1">Option 2</label><br />
    <input id="cbl1_2" type="checkbox" name="cbl1$2" value="3" /><label for="cbl1_2">Option 3</label>
</span>
```

## Key Properties to Implement

| Property | Type | Description |
|----------|------|-------------|
| `StaticItems` | `ListItemCollection` | Static list items |
| `Items` | `IEnumerable<TItem>` | Data-bound items |
| `DataTextField` | `string` | Property for display text |
| `DataValueField` | `string` | Property for value |
| `RepeatColumns` | `int` | Number of columns |
| `RepeatDirection` | `RepeatDirection` | Vertical or Horizontal |
| `RepeatLayout` | `RepeatLayout` | Table, Flow, OrderedList, UnorderedList |
| `TextAlign` | `TextAlign` | Label position (Left or Right) |
| `SelectedValue` | `string` | First selected value |
| `SelectedValues` | `List<string>` | All selected values |
| `CellPadding` | `int` | Padding in table layout |
| `CellSpacing` | `int` | Spacing in table layout |
| All `IStyle` properties | various | BackColor, ForeColor, CssClass, etc. |

## Enums Needed

### RepeatDirection (may exist)
```csharp
public enum RepeatDirection
{
    Horizontal,
    Vertical
}
```

### RepeatLayout (may exist)
```csharp
public enum RepeatLayout
{
    Table,
    Flow,
    OrderedList,
    UnorderedList
}
```

## Implementation Tasks

- [ ] Create `src/BlazorWebFormsComponents/CheckBoxList.razor`
- [ ] Create `src/BlazorWebFormsComponents/CheckBoxList.razor.cs`
- [ ] Add `RepeatDirection` enum if not exists
- [ ] Add `RepeatLayout` enum if not exists
- [ ] Create unit tests in `src/BlazorWebFormsComponents.Test/CheckBoxList/`
  - [ ] StaticItems.razor - static item rendering
  - [ ] DataBinding.razor - data-bound items
  - [ ] SelectedValues.razor - selection tracking
  - [ ] RepeatLayout.razor - different layouts
  - [ ] Style.razor - styling
  - [ ] TextAlign.razor - label positioning
- [ ] Create documentation in `docs/EditorControls/CheckBoxList.md`
- [ ] Update `mkdocs.yml` nav section

## Code Structure

### CheckBoxList.razor
```razor
@typeparam TItem
@inherits DataBoundComponent<TItem>

@if (Visible)
{
    @switch (RepeatLayout)
    {
        case Enums.RepeatLayout.Table:
            <table class="@CssClass" style="@Style" cellpadding="@CellPadding" cellspacing="@CellSpacing">
                @RenderTableItems()
            </table>
            break;
        case Enums.RepeatLayout.Flow:
            <span class="@CssClass" style="@Style">
                @RenderFlowItems()
            </span>
            break;
        case Enums.RepeatLayout.OrderedList:
            <ol class="@CssClass" style="@Style">
                @RenderListItems()
            </ol>
            break;
        case Enums.RepeatLayout.UnorderedList:
            <ul class="@CssClass" style="@Style">
                @RenderListItems()
            </ul>
            break;
    }
}
```

### CheckBoxList.razor.cs (key parts)
```csharp
public partial class CheckBoxList<TItem> : DataBoundComponent<TItem>, IStyle
{
    private string _baseId = Guid.NewGuid().ToString("N");

    [Parameter]
    public ListItemCollection StaticItems { get; set; } = new();

    [Parameter]
    public string DataTextField { get; set; }

    [Parameter]
    public string DataValueField { get; set; }

    [Parameter]
    public int RepeatColumns { get; set; } = 0;

    [Parameter]
    public RepeatDirection RepeatDirection { get; set; } = RepeatDirection.Vertical;

    [Parameter]
    public RepeatLayout RepeatLayout { get; set; } = RepeatLayout.Table;

    [Parameter]
    public TextAlign TextAlign { get; set; } = TextAlign.Right;

    [Parameter]
    public List<string> SelectedValues { get; set; } = new();

    [Parameter]
    public EventCallback<List<string>> SelectedValuesChanged { get; set; }

    [Parameter]
    public int CellPadding { get; set; } = -1;

    [Parameter]
    public int CellSpacing { get; set; } = -1;

    public string SelectedValue => SelectedValues.FirstOrDefault();

    public IEnumerable<ListItem> SelectedItems => 
        GetItems().Where(i => SelectedValues.Contains(i.Value));

    private async Task HandleChange(ListItem item, ChangeEventArgs e)
    {
        var isChecked = (bool)e.Value;
        if (isChecked && !SelectedValues.Contains(item.Value))
        {
            SelectedValues.Add(item.Value);
        }
        else if (!isChecked && SelectedValues.Contains(item.Value))
        {
            SelectedValues.Remove(item.Value);
        }
        await SelectedValuesChanged.InvokeAsync(SelectedValues);
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
}
```

## Estimated Effort
- Complexity: **Medium**
- Estimated time: **4-6 hours**

## Acceptance Criteria
1. Renders checkboxes with correct layout (Table, Flow, OrderedList, UnorderedList)
2. Static items work via StaticItems property
3. Data binding works via Items, DataTextField, DataValueField
4. SelectedValues tracks all checked items
5. Two-way binding works with @bind-SelectedValues
6. TextAlign positions labels correctly
7. RepeatDirection controls vertical/horizontal layout
8. RepeatColumns controls multi-column layout
9. All style properties work
10. All unit tests pass

## Related Components
- **CheckBox** - Individual checkbox (already complete)
- **RadioButtonList** - Similar but single-select
- **ListBox** - Similar multi-select with different UI
