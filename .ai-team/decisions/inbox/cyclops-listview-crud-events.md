### 2026-03-03: ListView CRUD events — correctness fixes for ItemCreated and ItemCommand

**By:** Cyclops
**What:** Fixed two Web Forms lifecycle deviations in ListView: (1) `ItemCreated` changed from `EventCallback` firing once on first render to `EventCallback<ListViewItemEventArgs>` firing per-item before `ItemDataBound` in both grouped and non-grouped paths; (2) `ItemCommand` now fires for ALL commands before routing to specific handlers (Edit, Delete, Update, etc.), not just for unknown commands.
**Why:** Web Forms fires `ItemCommand` first for every command, then the specific event. `ItemCreated` fires per-item during data binding. These are documented lifecycle behaviors that migration code depends on. The IOrderedDictionary properties (Keys, Values, NewValues, OldValues) from Web Forms EventArgs are deliberately omitted — they're tied to the DataSource control paradigm that doesn't exist in Blazor.
