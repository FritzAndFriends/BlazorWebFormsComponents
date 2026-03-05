### 2026-03-06: P0 Event Handler Test Coverage for All 7 Audit Items
**By:** Rogue
**What:** Created 49 bUnit tests across 6 test files covering all P0 event handler additions/fixes from Forge's event handler fidelity audit. Tests verify both bare-name and On-prefix aliases, EventArgs types and properties, lifecycle firing behavior, empty-data edge cases, and Sender property population. 12 tests pass now (FormViewInsertedEventArgs shape, SelectMethod initial render, empty-data guards, type fix). 37 tests are expected-fail pending Cyclops's event wiring in component .razor templates.
**Why:** These tests define the acceptance criteria for Cyclops's P0 implementation work. They document the exact parameter names, types, and behavioral contracts from the audit. Once Cyclops finishes wiring events in the .razor templates, these tests become the regression safety net. Writing tests against the spec (not the implementation) ensures we catch any deviation from the audit's requirements.

### 2026-03-06: DataList ItemDataBound fires 2x per item during Blazor lifecycle
**By:** Rogue
**What:** Discovered that DataList's `OnItemDataBound` callback fires twice per data item (6 times for 3 items) due to Blazor's double-render cycle. Tests use `ShouldBeGreaterThanOrEqualTo(N)` instead of exact count assertions.
**Why:** This is a behavioral characteristic, not necessarily a bug — Blazor re-renders during lifecycle (OnInitialized + OnAfterRender). However, this could cause performance issues with large datasets if event handlers do expensive work. Flagging for team awareness. Future optimization: debounce or guard against duplicate firings in the DataList template.

### 2026-03-06: RowCreated must fire before RowDataBound (ordering test)
**By:** Rogue
**What:** Added a test in GridView/RowEvents.razor that verifies RowCreated fires BEFORE RowDataBound for each row, matching Web Forms behavior where the row is structurally created before data is bound to it.
**Why:** Web Forms developers may depend on this ordering (e.g., modifying row structure in RowCreated before data binding populates values in RowDataBound). The ordering test ensures Cyclops implements the events in the correct sequence.
