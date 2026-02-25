# Project Context

- **Owner:** Jeffrey T. Fritz (csharpfritz@users.noreply.github.com)
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

<!-- ⚠ Summarized 2026-02-25 by Scribe — original entries covered 2026-02-10 through 2026-02-12 -->

### Summary: Documentation Conventions (2026-02-10 through 2026-02-12)

**Doc structure:** title → intro (MS docs link) → Features Supported → NOT Supported → Web Forms syntax → Blazor syntax → HTML Output → Migration Notes (Before/After) → Examples → See Also. Admonitions for gotchas. mkdocs.yml nav is alphabetical within category sections. Migration section keeps "Getting started" and "Migration Strategies" at top.

**Key patterns:** Style migration: Web Forms `TableItemStyle` child elements → Blazor CSS class string parameters. Deferred controls use `docs/Migration/DeferredControls.md` with What/Why/Alternatives/Before-After (no Features sections). ImageMap is in Navigation Controls. Branch naming: `copilot/create-*` on upstream. Chart doc introduces JS interop "HTML Output Exception" pattern and multi-component (child) doc pattern. Chart Type Gallery at `docs/images/chart/chart-{type}.png`.

📌 Team update (2026-02-10): Docs and samples must ship in the same sprint as the component — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): PRs #328 (ASCX CLI) and #309 (VS Snippets) shelved indefinitely — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): Sprint 1 gate review — ImageMap (#337) APPROVED, PageService (#327) APPROVED, ready to merge — decided by Forge
📌 Team update (2026-02-10): Sprint 2 complete — Localize, MultiView+View, ChangePassword, CreateUserWizard shipped with docs, samples, tests. 709 tests passing. 41/53 components done. — decided by Squad
📌 Team update (2026-02-11): Sprint 3 scope: DetailsView + PasswordRecovery. Chart/Substitution/Xml deferred. 48/53 → target 50/53. — decided by Forge
📌 Team update (2026-02-11): Colossus added as dedicated integration test engineer. Rogue retains bUnit unit tests. — decided by Jeffrey T. Fritz
- **PasswordRecovery doc pattern follows ChangePassword:** The PasswordRecovery doc mirrors the ChangePassword.md structure — same "Authentication Integration" warning admonition, same style migration guidance (TableItemStyle → CSS classes via cascading parameters), same emphasis on event-driven architecture. This three-step wizard pattern (UserName → Question → Success) with `@ref` for calling component methods (SetQuestion, SkipToSuccess) is unique among login controls and should be noted for any future wizard-style components.
- **DetailsView doc covers generic component:** DetailsView is generic (`DetailsView<ItemType>`), unlike most other data controls. The doc explicitly calls out the `ItemType` requirement and the reflection-based auto-field generation. The Fields child content pattern with CascadingValue registration is worth noting for any future components that use child component registration.
- **Sprint 3 docs delivered:** DetailsView and PasswordRecovery documentation created with full structure (features, Web Forms syntax, Blazor syntax, HTML output, migration notes, examples, See Also). Added to mkdocs.yml nav (alphabetical) and linked in README.md.

📌 Team update (2026-02-12): Sprint 3 gate review — DetailsView and PasswordRecovery APPROVED. Action item: fix DetailsView docs to replace `DataSource=` with `Items=` in Blazor code samples. — decided by Forge

 Team update (2026-02-12): Milestone 4 planned  Chart component with Chart.js via JS interop. 8 work items, design review required before implementation.  decided by Forge + Squad

- **Chart doc is first JS interop component:** The Chart component is unique in the library — it's the first to use JavaScript interop (Chart.js via ES module import). The doc template needed a new "HTML Output Exception" admonition pattern to explain why `<canvas>` replaces `<img>`. This pattern should be reused for any future components that deviate from identical HTML output.
- **DeferredControls.md updated for partial implementation:** Chart moved from fully-deferred to partially-implemented. The DeferredControls page now has a dual role: documenting controls not implemented at all (Substitution, Xml) AND documenting unsupported sub-features of implemented controls (27 unsupported chart types). This "partially implemented" pattern may apply to future controls.
- **Child component docs pattern:** Chart introduces a multi-component documentation pattern (Chart, ChartSeries, ChartArea, ChartLegend, ChartTitle) with separate parameter tables for each. This nested-component doc approach should be used for any future components with required child components.
- **Chart Type Gallery added:** Added a "Chart Type Gallery" section to `docs/DataControls/Chart.md` between "Chart Palettes" and "Web Forms Features NOT Supported". Contains 8 subsections (Column, Line, Bar, Pie, Doughnut, Area, Scatter, Stacked Column) each with a screenshot, `SeriesChartType` enum value, and 1-2 sentence usage guidance. Includes `!!! warning` admonitions on Pie and Doughnut for the Phase 1 palette limitation (single series color instead of per-segment colors).
- **Chart image path convention:** Chart screenshots live at `docs/images/chart/chart-{type}.png` (lowercase, hyphenated). Referenced from Chart.md using relative paths: `../images/chart/chart-{type}.png`. This `docs/images/{component}/` pattern should be used for any future component screenshots.
### Summary: Feature Audit Findings (2026-02-23)

AccessKey/ToolTip missing from base classes (universal gap). Label needs BaseStyledComponent. ListControl-derived components share common gaps (AppendDataBoundItems, DataTextFormatString, CausesValidation). Literal/Localize/PlaceHolder/View/MultiView near-complete. Substitution/Xml permanently deferred. Style property is computed (not directly settable). Panel is most feature-complete styled control.


 Team update (2026-02-23): AccessKey/ToolTip must be added to BaseStyledComponent  decided by Beast, Cyclops
 Team update (2026-02-23): Chart implementation architecture consolidated (10 decisions)  decided by Cyclops, Forge
 Team update (2026-02-23): DetailsView/PasswordRecovery branch (sprint3) must be merged forward  decided by Forge

 Team update (2026-02-23): BaseListControl<TItem> introduced  docs should reflect shared base for list controls  decided by Cyclops
 Team update (2026-02-23): Label AssociatedControlID switches rendered element  document accessibility benefit  decided by Cyclops
 Team update (2026-02-23): Login controls now inherit BaseStyledComponent  update docs for outer style support  decided by Rogue, Cyclops
 Team update (2026-02-23): Milestone 6 Work Plan ratified  54 WIs, Beast assigned branding (UI-11) and docs (UI-12)  decided by Forge
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

