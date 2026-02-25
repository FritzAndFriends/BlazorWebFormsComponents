# Decision: Fix 19 Unreachable Sample Pages in ComponentCatalog.cs

**Date:** 2026-02-25
**Author:** Jubilee
**Issue:** #350

## Summary

Updated `ComponentCatalog.cs` to link all 19 previously unreachable sample pages.

## Changes Made

### 5 New Component Entries
| Component | Category | Route |
|-----------|----------|-------|
| DataBinder | Utility | /ControlSamples/DataBinder |
| ViewState | Utility | /ControlSamples/ViewState |
| DetailsView | Data | /ControlSamples/DetailsView |
| Menu | Navigation | /ControlSamples/Menu/Selection |
| PasswordRecovery | Login | /ControlSamples/PasswordRecovery |

### 15 New SubPage Entries
- **GridView:** DisplayProperties, InlineEditing, Paging, Selection, Sorting
- **TreeView:** ExpandCollapse, Selection
- **FormView:** Edit, Events, Styles
- **ListView:** CrudOperations
- **DataGrid:** Styles
- **Panel:** BackImageUrl

### Bug Fix
- **DataList:** SubPage name corrected from "Flow" to "SimpleFlow" to match actual file name.

## Rationale

- DetailsView was treated as a 5th missing component (not listed in the original issue's 4) because it had zero catalog presence despite having 3 sample pages on disk.
- Menu routes to `/ControlSamples/Menu/Selection` because no `Index.razor` exists — only `Selection.razor`.
- SubPages are kept alphabetically sorted within their arrays to match existing convention.
