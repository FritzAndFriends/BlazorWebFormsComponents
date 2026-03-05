### 2026-03-06: P0 Event Handler Implementation Decisions

**By:** Cyclops

**What:** Implemented all 7 P0 event handler fixes from Forge's audit. Key decisions:

1. **DataList `ItemDataBound` method→parameter collision:** Renamed the internal method from `ItemDataBound()` to `ItemDataBoundInternal()` rather than changing the `[Parameter]` name. The parameter name must match Web Forms markup for migration fidelity.

2. **GridView `RowCommand` bare alias:** The existing `OnRowCommand` had no bare-name alias. Added `RowCommand` and updated `ButtonField.razor.cs` to coalesce both, since `ButtonField` directly accessed the GridView's `OnRowCommand` property.

3. **FormView CRUD event aliases:** The 6 CRUD events (`OnItemDeleting`, `OnItemDeleted`, `OnItemInserting`, `OnItemInserted`, `OnItemUpdating`, `OnItemUpdated`) previously only had On-prefix forms. Added all bare-name aliases and updated `HandleCommandArgs` to coalesce consistently.

4. **FormView `OnItemInserted` type correction:** Changed from `FormViewInsertEventArgs` (present tense, for "Inserting") to new `FormViewInsertedEventArgs` (past tense, for "Inserted"). Follows the same Insert/Inserted pattern seen in `DetailsViewInsertEventArgs`/`DetailsViewInsertedEventArgs`.

5. **SelectMethod moved to `OnParametersSet`:** Was previously in `OnAfterRender(firstRender)` which only fires once. Now in `OnParametersSet()` so it re-evaluates whenever parameters change. Added `RefreshSelectMethod()` helper for explicit post-CRUD refreshes.

**Why:** Forge's audit identified these as P0 fidelity gaps — migrated Web Forms markup would fail silently because event handlers wouldn't bind. Every data control needs its full event surface to support real-world migration scenarios.
