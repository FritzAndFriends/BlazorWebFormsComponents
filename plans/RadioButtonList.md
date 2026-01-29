# RadioButtonList Component Implementation Plan

## Overview
Implement the RadioButtonList component that emulates ASP.NET Web Forms `<asp:RadioButtonList>` control. RadioButtonList renders a group of mutually exclusive radio buttons for single-select scenarios.

## Web Forms Reference
- **Documentation**: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.radiobuttonlist?view=netframework-4.8
- **Namespace**: `System.Web.UI.WebControls`

## HTML Output

**Web Forms:**
```html
<asp:RadioButtonList ID="rbl1" runat="server" RepeatDirection="Vertical">
    <asp:ListItem Text="Small" Value="S" />
    <asp:ListItem Text="Medium" Value="M" Selected="True" />
    <asp:ListItem Text="Large" Value="L" />
</asp:RadioButtonList>
```

**Rendered HTML (RepeatLayout=Table, default):**
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

**Key difference from CheckBoxList:** All radio inputs share the same `name` attribute for mutual exclusion.

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
| `SelectedValue` | `string` | Currently selected value |
| `SelectedIndex` | `int` | Currently selected index |
| `CellPadding` | `int` | Padding in table layout |
| `CellSpacing` | `int` | Spacing in table layout |
| All `IStyle` properties | various | BackColor, ForeColor, CssClass, etc. |

## Implementation Tasks

- [ ] Create `src/BlazorWebFormsComponents/RadioButtonList.razor`
- [ ] Create `src/BlazorWebFormsComponents/RadioButtonList.razor.cs`
- [ ] Ensure `RepeatDirection` enum exists
- [ ] Ensure `RepeatLayout` enum exists
- [ ] Create unit tests in `src/BlazorWebFormsComponents.Test/RadioButtonList/`
  - [ ] StaticItems.razor - static item rendering
  - [ ] DataBinding.razor - data-bound items
  - [ ] SelectedValue.razor - selection tracking
  - [ ] MutualExclusion.razor - only one selected at a time
  - [ ] RepeatLayout.razor - different layouts
  - [ ] Style.razor - styling
  - [ ] TextAlign.razor - label positioning
- [ ] Create documentation in `docs/EditorControls/RadioButtonList.md`
- [ ] Update `mkdocs.yml` nav section

## Code Structure

### RadioButtonList.razor
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

### RadioButtonList.razor.cs (key parts)
```csharp
public partial class RadioButtonList<TItem> : DataBoundComponent<TItem>, IStyle
{
    private string _groupName = Guid.NewGuid().ToString("N");

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
    public string SelectedValue { get; set; }

    [Parameter]
    public EventCallback<string> SelectedValueChanged { get; set; }

    [Parameter]
    public int SelectedIndex { get; set; } = -1;

    [Parameter]
    public EventCallback<int> SelectedIndexChanged { get; set; }

    [Parameter]
    public EventCallback<ChangeEventArgs> OnSelectedIndexChanged { get; set; }

    [Parameter]
    public int CellPadding { get; set; } = -1;

    [Parameter]
    public int CellSpacing { get; set; } = -1;

    public ListItem SelectedItem => GetItems().FirstOrDefault(i => i.Value == SelectedValue);

    private async Task HandleChange(ListItem item, ChangeEventArgs e)
    {
        SelectedValue = item.Value;
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
}
```

## Key Difference from CheckBoxList

The critical difference is that **all radio inputs share the same `name` attribute**:

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

## Estimated Effort
- Complexity: **Medium**
- Estimated time: **4-6 hours**

## Acceptance Criteria
1. Renders radio buttons with shared name for mutual exclusion
2. Only one item can be selected at a time
3. Static items work via StaticItems property
4. Data binding works via Items, DataTextField, DataValueField
5. SelectedValue tracks the selected item
6. Two-way binding works with @bind-SelectedValue
7. SelectedIndex tracks index position
8. TextAlign positions labels correctly
9. All layout modes work (Table, Flow, OrderedList, UnorderedList)
10. All style properties work
11. All unit tests pass

## Related Components
- **RadioButton** - Individual radio button (already complete)
- **CheckBoxList** - Similar but multi-select
- **DropDownList** - Similar single-select with different UI
