# GridView Style Sub-Components Pattern

**By:** Cyclops  
**Date:** 2026-02-24  
**Work Item:** WI-05

## Decision

GridView style sub-components follow the same `IXxxStyleContainer` + `UiTableItemStyle` + `CascadingParameter` pattern used by Calendar and DataList.

## Details

- **Interface:** `IGridViewStyleContainer` with 8 TableItemStyle properties (RowStyle, AlternatingRowStyle, HeaderStyle, FooterStyle, EmptyDataRowStyle, PagerStyle, EditRowStyle, SelectedRowStyle)
- **CascadingValue name:** `"ParentGridView"` (matching Calendar's `"ParentCalendar"` and DataList's `"ParentDataList"` convention)
- **Style priority in GetRowStyle:** EditRowStyle > SelectedRowStyle > AlternatingRowStyle > RowStyle (matches Web Forms precedence)
- **Style application:** Inline `style` attribute via `TableItemStyle.ToString()` on `<tr>` elements, not CSS classes
- **EditRowStyle migration:** Changed from `[Parameter]` to `IGridViewStyleContainer` property with `internal set`, to be consistent with all other style properties and enable sub-component setting

## Why

This maintains consistency with the existing Calendar and DataList style patterns. The `CascadingParameter` + interface approach allows style sub-components to be declared as child elements in markup, exactly matching Web Forms `<asp:GridView><RowStyle .../></asp:GridView>` syntax.
