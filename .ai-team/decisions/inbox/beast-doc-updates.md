# Beast Doc Updates — Issue #359

**By:** Beast
**Date:** 2026-02-25

## Decision: PagerSettings gets a dedicated doc page

**What:** Created `docs/DataControls/PagerSettings.md` as a standalone documentation page for the shared PagerSettings sub-component. Added to mkdocs.yml nav under Data Controls.

**Why:** PagerSettings is used by FormView, DetailsView, and GridView. Rather than duplicating its property reference in each parent control's doc, a single dedicated page is linked from all three. This is the first shared sub-component to get its own doc page.

**Impact:** Any future shared sub-components (if they reach similar complexity) should follow this pattern — own page, linked from parent control docs.

## Decision: Stale "NOT Supported" entries corrected

**What:** Removed PagerSettings and row styles from DetailsView's "NOT Supported" list. Removed Paging, Sorting, Selection, and Editing from DataGrid's "NOT Supported" list. All were implemented in M6-M8 but docs were never updated.

**Why:** Stale "NOT Supported" entries mislead migrators into thinking features are missing when they actually work. This is a documentation fidelity issue — docs must reflect current implementation status.

**Impact:** Future milestone work should include a doc review pass to catch similar drift.
