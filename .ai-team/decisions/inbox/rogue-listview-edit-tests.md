### 2026-03-01: ListView EditItemTemplate TDD tests use CSS class selectors for template identification
**By:** Rogue
**What:** Created `EditTemplateTests.razor` with 6 tests for Issue #406. Tests use `span.display` and `span.edit` CSS classes to identify which template (ItemTemplate vs EditItemTemplate) rendered for each row. 4 tests intentionally fail pre-fix (TDD), 2 pass for negative/null edge cases.
**Why:** Previous CrudEvents.razor tests only verified `EditIndex` property values, not the actual rendered DOM. The new tests verify the **visual template swap** — which is the actual user-facing bug. CSS class selectors are stable, readable, and clearly communicate which template owns each row. Cyclops should ensure the fix makes all 6 tests pass.
