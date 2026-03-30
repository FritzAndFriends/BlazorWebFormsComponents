# Bishop Phase 2: GAP-05 + GAP-07 Code-Behind Transforms

**Date:** 2025-07-24
**Author:** Bishop (Migration Tooling Dev)
**Status:** Implemented

## Decisions Made

### D1: GAP-07 Event Handler — Type-name matching over inheritance check
**Decision:** Determine whether to strip both params or keep specialized EventArgs by checking if the type name is *exactly* `EventArgs` (strip both) vs. ends with `EventArgs` but is longer (keep specialized).
**Rationale:** PowerShell regex can't inspect the C# type hierarchy. String matching on `\w*EventArgs` is sufficient because all Web Forms EventArgs subtypes follow the naming convention `*EventArgs`. This avoids false positives on non-EventArgs types like `string` or `int`.
**Risk:** If a custom EventArgs subclass is named exactly `EventArgs` (impossible per C# rules) it would be incorrectly stripped. Extremely low risk.

### D2: GAP-05 Lifecycle — `await base.OnInitializedAsync()` injection
**Decision:** Inject `await base.OnInitializedAsync();` at the start of the converted `OnInitializedAsync` body.
**Rationale:** Blazor requires calling the base lifecycle method. Missing this call is a common source of bugs when components use `WebFormsPageBase` which has initialization logic in the base.

### D3: GAP-05 PreRender — `if (firstRender)` guard wrapping
**Decision:** Wrap the entire `Page_PreRender` body in `if (firstRender) { ... }` when converting to `OnAfterRenderAsync`.
**Rationale:** `Page_PreRender` runs once before the first render. `OnAfterRenderAsync` runs after *every* render. The `firstRender` guard preserves the original single-execution semantics.

### D4: Transform ordering — Lifecycle before Event Handlers
**Decision:** Run GAP-05 (lifecycle) before GAP-07 (event handlers) in the pipeline.
**Rationale:** Lifecycle conversion changes `Page_Load(object sender, EventArgs e)` to `OnInitializedAsync()`. If event handler conversion ran first, it would strip the lifecycle method's params but not rename it. Running lifecycle first ensures the method name is changed, and the params no longer match the event handler regex.

### D5: Test expected files updated in-place
**Decision:** Updated 6 existing expected test files (TC13–TC16, TC18, TC19) to reflect the new transforms rather than excluding lifecycle/event handler test assertions.
**Rationale:** The L1 pipeline is cumulative — all transforms run on every code-behind file. Expected outputs must reflect the full transform chain. Selective exclusion would be fragile and mask regressions.

## Validation
- Script parses cleanly (0 errors)
- All 21 L1 tests pass at 100% line accuracy
- TC19 (lifecycle), TC20 (standard handlers), TC21 (specialized handlers) are dedicated test cases
