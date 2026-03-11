### 2026-03-11: Standardize generic type params to ItemType
**By:** Cyclops
**What:** Renamed TItemType and TItem to ItemType across all BWFC data-bound components
**Why:** Web Forms used DataBoundControl.ItemType — BWFC's drop-in replacement promise requires matching the original attribute names

**Files changed:**
- DataBoundComponent.cs: `TItemType` → `ItemType` (class + 10 internal refs)
- SelectHandler.cs: `TItemType` → `ItemType` (delegate)
- BaseListControl.cs: `TItem` → `ItemType` (class + 2 method refs)
- BulletedList.razor/.razor.cs: `TItem` → `ItemType`
- CheckBoxList.razor/.razor.cs: `TItem` → `ItemType`
- DropDownList.razor/.razor.cs: `TItem` → `ItemType`
- ListBox.razor/.razor.cs: `TItem` → `ItemType`
- RadioButtonList.razor/.razor.cs: `TItem` → `ItemType`

**Verified:** All Group 3 components (GridView, DataGrid, DataList, Repeater, ListView, DetailsView, FormView, all column/row types) already used `ItemType`. Zero remaining `TItemType`/`TItem` in library source. Build: 0 errors.
