### GridView Selection Support — Pattern Decisions

**By:** Cyclops  
**Date:** 2026-02-24  
**WI:** WI-02

**What:**
- `SelectedIndex` (int, default -1) follows the same pattern as `EditIndex`
- `SelectedRow` and `SelectedValue` are computed read-only properties (not parameters)
- `SelectedValue` uses reflection on `DataKeyNames` first key field, matching Web Forms behavior
- `AutoGenerateSelectButton` adds a "Select" link to the command column, rendered before Edit/Delete links
- `ShowCommandColumn` now includes `AutoGenerateSelectButton` in its check
- `GetRowStyle()` priority: EditRowStyle > SelectedRowStyle > AlternatingRowStyle > RowStyle (edit takes precedence over selection)
- `GridViewSelectedRowStyle` follows the existing `IGridViewStyleContainer` + CascadingParameter pattern (same as `GridViewEditRowStyle`, etc.)
- `GridViewSelectEventArgs` follows the same pattern as `GridViewEditEventArgs` (NewSelectedIndex + Cancel)

**Why:**
- Selection mirrors the existing edit-mode pattern for consistency
- Edit takes priority over selection in styling because a row being edited is an active operation
- The `SelectedRowStyle` child component reuses the established `IGridViewStyleContainer` interface rather than creating a new one
