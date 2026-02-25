# Project Context

- **Owner:** Jeffrey T. Fritz (csharpfritz@users.noreply.github.com)
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- ⚠ Summarized 2026-02-25 by Scribe — older entries condensed into Core Context -->

### Core Context (2026-02-10 through 2026-02-25)

Established doc structure: title → intro (MS docs link) → Features Supported → NOT Supported → Web Forms syntax → Blazor syntax → HTML Output → Migration Notes → Examples → See Also. mkdocs.yml nav alphabetical within categories. Branch naming: `copilot/create-*`. Chart doc introduced JS interop "HTML Output Exception" pattern and multi-component (child) doc pattern. Chart Type Gallery at `docs/images/chart/chart-{type}.png`. Created PasswordRecovery doc (3-step wizard pattern), DetailsView doc (generic component pattern). Deferred controls use `docs/Migration/DeferredControls.md`. Feature audit: AccessKey/ToolTip base class gap, Substitution/Xml deferred, Style is computed.

**Key patterns:** Style migration: TableItemStyle → CSS class string parameters. DeferredControls.md has dual role (fully deferred + partially implemented). Chart screenshots at `docs/images/{component}/`. Shared sub-component docs linked from parent control docs.

 Team update (2026-02-23): Menu Orientation requires Razor local variable workaround  document this pattern  decided by Jubilee

- **Milestone 8 release-readiness docs polish:** Formally deferred Substitution and Xml controls in `status.md` (changed from 🔴 Not Started to ⏸️ Deferred with rationale). Added Deferred column to summary table. Updated `docs/Migration/DeferredControls.md` to mark Chart as fully implemented (removed "Phase 1"/"Partial" hedging). Removed all "Phase 1"/"Phase 2/3" hedging from `docs/DataControls/Chart.md`. Fixed duplicate `DeferredControls.md` entry in `mkdocs.yml` and re-alphabetized Migration nav. Fixed broken `ImageMap` link in `README.md` (pointed to EditorControls, should be NavigationControls). Added missing doc links in README for MultiView, View, ChangePassword, CreateUserWizard. Marked Xml as deferred in README component list.

 Team update (2026-02-24): Menu auto-ID pattern  components with JS interop should auto-generate IDs  decided by Cyclops
 Team update (2026-02-24): M8 scope excludes version bump to 1.0 and release  decided by Jeffrey T. Fritz
 Team update (2026-02-24): PagerSettings shared sub-component created  update docs when component stabilizes  decided by Cyclops

- **M9 Doc Gap Audit (WI-09):** Audited all docs against M6-M8 features. GridView, TreeView, Menu, Validators (ControlToValidate), and Login are fully documented. Gaps found in: FormView (ItemCommand event, styles, PagerSettings not in Blazor sections), DetailsView (Caption missing, styles/PagerSettings listed as unsupported but may be stale), DataGrid (paging listed as unsupported, needs verification), ChangePassword (Orientation and TextLayout not documented despite Login having them), and PagerSettings (no dedicated doc page exists). Full report in `.ai-team/decisions/inbox/beast-m9-doc-audit.md`.
- **M9 Planning-Docs Historical Headers (WI-10):** Added `> ⚠️ Historical Snapshot (Pre-Milestone 6)` header to all 54 per-control audit files and SUMMARY.md in `planning-docs/`. Excluded README.md and MILESTONE*-PLAN.md files (still current/active). This prevents future contributors from treating pre-M6 gap data as current.
- **ChangePassword/Login parity gap:** Login.md documents Orientation and TextLayout with full reference tables and migration examples, but ChangePassword.md has neither. Both controls should have identical coverage for these shared layout properties.
- **ToolTip universality documented (WI-04):** Added ToolTip to Features Supported in Label.md, TextBox.md, and GridView.md (Button.md already had it). Added "Common Properties on All Styled Controls" section to Migration/readme.md explaining that ToolTip (renders as `title` attribute) is universally available on all BaseStyledComponent-derived controls. Used `!!! tip` admonition and code examples. No dedicated Common Properties page created — kept it inline in the migration guide for minimal footprint.

 Team update (2026-02-25): ToolTip moved to BaseStyledComponent (28+ controls), ValidationSummary comma-split fixed, SkinID boolstring fixed  decided by Cyclops
 Team update (2026-02-25): M9 plan ratified  12 WIs across P0/P1/P2, migration fidelity theme  decided by Forge

- **M9 Consolidated Audit Report:** Created `planning-docs/AUDIT-REPORT-M9.md` combining findings from three M9 audits: Doc Gap Audit (5 findings → #359), Integration Test Coverage Audit (5 findings → #358), and Sample Navigation Audit (19 findings → #350). All 29 findings mapped to M10 GitHub Issues with 100% coverage. Report includes 6 additional post-M9 findings (component gaps and TreeView bug). Used the planning-docs historical snapshot header convention (`> ⚠️ Historical Snapshot (Milestone 9)`).
- **Audit report convention established:** Consolidated audit reports should live at `planning-docs/AUDIT-REPORT-M{N}.md` with the standard historical snapshot header, summary table, per-audit sections with findings + resolution status, and an appendix issue tracker. This pattern can be reused for future milestone audits.

 Team update (2026-02-25): TreeView NodeImage now checks ShowExpandCollapse independently of ShowLines; ExpandCollapseImage() helper added (#361)  decided by Cyclops


 Team update (2026-02-25): M12 introduces Migration Analysis Tool PoC (`bwfc-migrate` CLI, regex-based ASPX parsing, 3-phase roadmap)  decided by Forge

- **Issue #359 doc updates (M6-M8 features):** Updated 4 existing doc pages and created 1 new page to close gaps identified in the M9 Doc Gap Audit:
  1. **ChangePassword.md** — Added Orientation and TextLayout documentation with reference tables, enum usage examples, and migration Before/After, mirroring the Login.md pattern.
  2. **PagerSettings.md** (NEW) — Created dedicated doc page in DataControls/ covering all properties, PagerButtons/PagerPosition enums, usage with FormView/DetailsView/GridView, and migration notes. Added to mkdocs.yml nav alphabetically.
  3. **FormView.md** — Added ItemCommand, ItemCreated, PageIndexChanging/PageIndexChanged events to features and Blazor syntax. Added Caption/CaptionAlign, PagerSettings child element, PagerTemplate, and 7 style sub-components (RowStyle, EditRowStyle, InsertRowStyle, HeaderStyle, FooterStyle, EmptyDataRowStyle, PagerStyle) with reference table.
  4. **DetailsView.md** — Added Caption/CaptionAlign with reference table. Moved styles and PagerSettings from "NOT Supported" to "Supported" (were stale). Added 10-row style sub-components table and PagerSettings child element to Blazor syntax. Updated migration notes to reflect style child elements.
  5. **DataGrid.md** — Moved Paging, Sorting, Selection, and Editing from "NOT Supported" to "Supported" (all now implemented). Added AllowPaging/PageSize/CurrentPageIndex/PageIndexChanged, AllowSorting/SortCommand, SelectedIndex/EditItemIndex to Blazor syntax. Added paging and sorting examples with event handlers.
- **Documentation pattern: PagerSettings as shared sub-component doc:** PagerSettings is the first shared (non-control) sub-component to get its own dedicated doc page. It's referenced via `[PagerSettings](PagerSettings.md)` links from FormView, DetailsView, and GridView docs. Future shared sub-components (e.g., TableItemStyle if ever documented separately) should follow this pattern.


 Team update (2026-02-25): All login controls (Login, LoginView, ChangePassword, PasswordRecovery, CreateUserWizard) now inherit from BaseStyledComponent  decided by Cyclops

 Team update (2026-02-25): ComponentCatalog.cs now links all sample pages; new samples must be registered there  decided by Jubilee


 Team update (2026-02-25): Future milestone work should include a doc review pass to catch stale 'NOT Supported' entries  decided by Beast

 Team update (2026-02-25): Shared sub-components of sufficient complexity get their own doc page (e.g., PagerSettings)  decided by Beast
