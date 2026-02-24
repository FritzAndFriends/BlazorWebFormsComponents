# Decision: ListView CRUD Events Pattern (WI-41)

**By:** Cyclops
**Date:** 2026-02-24

## What

ListView CRUD events follow the same dual-event pattern as GridView and FormView:
- Pre-events (ItemEditing, ItemDeleting, ItemUpdating, ItemInserting, ItemCanceling) support `Cancel` bool
- Post-events (ItemDeleted, ItemInserted, ItemUpdated) carry `AffectedRows` + `Exception`
- `ItemCommand` fires for unrecognized commands (catch-all)
- `HandleCommand(string, object, int)` is the public routing method

## Why

Consistent with GridView's `EditRow`/`UpdateRow`/`DeleteRow`/`CancelEdit` and FormView's `HandleCommandArgs` patterns. ListView event args are intentionally simpler than FormView's (no OrderedDictionary) because the task spec said "don't over-engineer dictionaries if simpler patterns work."

## Key Decisions

1. **EmptyItemTemplate vs EmptyDataTemplate:** `EmptyItemTemplate` takes precedence when both are set. `EmptyDataTemplate` was the original, `EmptyItemTemplate` is the Web Forms ListView-specific name.
2. **ListViewCancelMode enum:** Created in `Enums/ListViewCancelMode.cs` — `CancelingEdit` (0) and `CancelingInsert` (1). Follows project enum convention with explicit int values.
3. **GetItemTemplate helper:** Returns EditItemTemplate when itemIndex matches EditIndex, otherwise delegates to alternating template logic. Used in both grouped and non-grouped rendering paths.
4. **InsertItemTemplate positioning:** Renders at top (before items) or bottom (after items) based on InsertItemPosition enum, only in the non-grouped (GroupItemCount == 0) path.
