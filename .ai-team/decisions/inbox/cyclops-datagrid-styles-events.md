# DataGrid Style Sub-Components + Paging/Sorting Events

**By:** Cyclops  
**Date:** 2026-02-24  
**Work Items:** WI-44, WI-45

## Decision

DataGrid style sub-components follow the exact same `IXxxStyleContainer` + `UiTableItemStyle` + `CascadingParameter` pattern used by GridView, DetailsView, and FormView. Paging and sorting events follow Web Forms DataGrid naming conventions (not GridView conventions).

## Details — WI-44 (Style Sub-Components)

- **Interface:** `IDataGridStyleContainer` with 7 TableItemStyle properties (AlternatingItemStyle, ItemStyle, HeaderStyle, FooterStyle, PagerStyle, SelectedItemStyle, EditItemStyle)
- **CascadingValue name:** `"ParentDataGrid"` (matching GridView's `"ParentGridView"` convention)
- **7 sub-component pairs:** DataGridAlternatingItemStyle, DataGridItemStyle, DataGridHeaderStyle, DataGridFooterStyle, DataGridPagerStyle, DataGridSelectedItemStyle, DataGridEditItemStyle
- **Style priority in GetRowStyle:** EditItemStyle > SelectedItemStyle > AlternatingItemStyle > ItemStyle (matches Web Forms precedence)
- **Display properties added:** Caption, CaptionAlign, CellPadding, CellSpacing, GridLines, UseAccessibleHeader
- **Template enhanced:** Paging UI (page links in tfoot), footer row, caption element, grid lines rules attribute, sortable header links

## Details — WI-45 (Paging + Sorting Events)

- **Events:** PageIndexChanged (DataGridPageChangedEventArgs), SortCommand (DataGridSortCommandEventArgs), ItemCreated (DataGridItemEventArgs), ItemDataBound (DataGridItemEventArgs), SelectedIndexChanged (EventCallback)
- **Event args:** DataGridPageChangedEventArgs (NewPageIndex), DataGridSortCommandEventArgs (SortExpression, CommandSource), DataGridItemEventArgs (Item)
- **Paging:** GoToPage(int) updates CurrentPageIndex and fires PageIndexChanged
- **Sorting:** Sort(string) fires SortCommand when AllowSorting is enabled via header links

## Key Naming Difference: DataGrid vs GridView

DataGrid uses Web Forms DataGrid naming (ItemStyle, AlternatingItemStyle, EditItemIndex, CurrentPageIndex) rather than GridView naming (RowStyle, AlternatingRowStyle, EditIndex, PageIndex). This matches the original ASP.NET Web Forms distinction between the two controls.

## Why

Consistency with existing GridView style pattern ensures predictable API. DataGrid-specific naming preserves Web Forms migration fidelity — developers migrating `<asp:DataGrid>` markup expect `ItemStyle` not `RowStyle`.
