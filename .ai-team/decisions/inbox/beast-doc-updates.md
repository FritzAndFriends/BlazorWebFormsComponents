# Decision: M6-M8 doc pages updated (#359)

**Date:** 2026-02-27
**Author:** Beast (Technical Writer)
**Status:** Complete

**What:** Updated 3 of 5 doc pages for Issue #359. ChangePassword and PagerSettings were already complete from prior work. FormView gained explicit CRUD event docs and a "NOT Supported" section. DetailsView Web Forms syntax block now includes Caption/CaptionAlign attributes and all style sub-component/PagerSettings child elements. DataGrid paging docs refreshed — stale caveat removed, property table and PagerSettings comparison admonition added.

**Why:** The M9 audit (planning-docs) identified these 5 pages as having gaps relative to M6-M8 feature additions. Closing these gaps ensures developers migrating FormView CRUD workflows, DetailsView layouts, and DataGrid paging have accurate documentation.

**Impact:** Documentation only — no source code changes. All edits are additive (no content removed except one stale caveat in DataGrid). mkdocs.yml nav unchanged.

**Key finding:** DataGrid is the only pageable data control that does NOT support the `<PagerSettings>` sub-component. It renders a fixed numeric pager. This should be highlighted in any future "choosing a data control" migration guide.
