# Project Context

- **Owner:** Jeffrey T. Fritz (csharpfritz@users.noreply.github.com)
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->
<!-- ⚠ Summarized 2026-02-12 by Scribe — original entries covered 2026-02-10 through 2026-02-12 -->

### Summary: Milestone 1 PR Review & Planning (2026-02-10)

Reviewed 6 PRs. Calendar (#333): strong but needed CalendarSelectionMode enum. FileUpload (#335): broken data flow — must use InputFile, not raw @onchange. ImageMap (#337): wrong base class (BaseWebFormsComponent → should be BaseStyledComponent), static ID counter leak. PageService (#327): solid DI/service approach. #328 (ASCX CLI) and #309 (VS Snippets): merge conflicts, shelved indefinitely.

Gate review: Calendar REJECTED (assigned Rogue), FileUpload REJECTED (assigned Jubilee, path sanitization needed), ImageMap APPROVED, PageService APPROVED. Cyclops locked out of Calendar/FileUpload. PR #333 closed — work already on dev.

### Summary: Milestone 2 & 3 Planning (2026-02-10–02-11)

- Milestone 2 shipped: Localize, MultiView+View, ChangePassword, CreateUserWizard. 41/53 (77%).
- Status.md reconciliation: actual count was 48/53 (91%) after accounting for merged work.
- Milestone 3 scope: DetailsView + PasswordRecovery. Chart/Substitution/Xml deferred.
- Colossus added as integration test engineer.

### Summary: Milestone 3 Gate Review (2026-02-11)

DetailsView APPROVED: DataBoundComponent<T> inheritance, 10 events, table HTML, DetailsViewMode enum, auto-field generation. PasswordRecovery APPROVED: BaseWebFormsComponent inheritance, 3-step wizard, 6 events, table HTML. Minor non-blocking issues noted. 50/53 components (94%), 797 tests.

**Key patterns confirmed:**
- Login Controls → BaseWebFormsComponent + cascading TableItemStyle/Style
- Data-bound controls → DataBoundComponent<T> with Items parameter
- Login Control events use `On` prefix (project convention)
- Docs + samples must ship with components

### Summary: Chart JS Library Evaluation (2026-02-12)

Evaluated 4 JS libraries for Chart component. D3 rejected (zero built-in charts, XL effort). Chart.js recommended (MIT, ~60KB, 10 types, Blazor wrappers exist). ApexCharts strong alternative (20+ types but 2x bundle). Plotly rejected (3-4MB). Architecture: bundle chart.min.js as static asset + thin JS interop layer. HTML output exception: `<canvas>` instead of `<img>` (justified). Inherit DataBoundComponent<T>. Effort: L. Risks: first JS interop in project, canvas not bUnit-testable, SSR needs special handling. Proceed as Milestone 4.

📌 Team update (2026-02-10): PRs #328 (ASCX CLI) and #309 (VS Snippets) shelved indefinitely — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): Docs and samples must ship in the same milestone as the component — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): Milestone 1 gate review — Calendar (#333) REJECTED, FileUpload (#335) REJECTED, ImageMap (#337) APPROVED, PageService (#327) APPROVED — decided by Forge
📌 Team update (2026-02-10): Lockout protocol — Cyclops locked out of Calendar and FileUpload revisions — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): Close PR #333 without merging — all Calendar work already on dev — decided by Rogue
📌 Team update (2026-02-10): Milestone 2 complete — 4 components shipped. 709 tests. 41/53 done. — decided by Squad
📌 Team update (2026-02-10): Milestone 3 plan ratified — DetailsView + PasswordRecovery. Chart/Substitution/Xml deferred. — decided by Forge
📌 Team update (2026-02-11): Colossus added as dedicated integration test engineer. — decided by Jeffrey T. Fritz
📌 Team update (2026-02-11): Milestone 3 gate review — DetailsView APPROVED, PasswordRecovery APPROVED. 50/53 (94%). — decided by Forge
📌 Team update (2026-02-12): Chart component feasibility confirmed — Chart.js recommended via JS interop. Effort: L. Target Milestone 4. — decided by Forge
📌 Team update (2026-02-12): Milestone 4 planned — Chart component with Chart.js via JS interop. 8 work items, design review required before implementation. — decided by Forge + Squad

### Summary: Feature Audit & Themes/Skins Exploration (2026-02-12)

Audited 12 data + navigation controls. Key findings: DataBoundComponent<T> chain lacks style properties (systematic gap across 5 controls). GridView weakest (no paging/sorting/editing). Recommended DataBoundStyledComponent<T> as fix. DetailsView strong on sprint3 branch but needs merge-forward. Evaluated 5 Themes/Skins approaches — recommended CascadingValue ThemeProvider. SkinID bug (bool→string). Implementation opt-in.

### Summary: Sprint 3 Planning & Gate Review (2026-02-10 through 2026-02-11)

Status.md reconciliation found actual 48/53 (not 41/53). Sprint 3: DetailsView + PasswordRecovery. Chart/Substitution/Xml deferred. DetailsView APPROVED (DataBoundComponent<T>, 10 events, auto-fields via reflection). PasswordRecovery APPROVED (BaseWebFormsComponent, 3-step wizard, 6 events). Key patterns: Login Controls → BaseWebFormsComponent, Data → DataBoundComponent<T> with Items, events use On prefix. 50/53 complete (94%), 797 tests.

### Summary: Chart Component Gate Review (2026-02-13)

Chart on milestone4 branch substantially complete. Architecture sound: Components→ConfigBuilder→JSON→JS interop. 140 tests. Data binding gap was ship-blocking (Items/XValueMember/YValueMembers ignored by ToConfig()). Conditionally approved pending fix. 27 unsupported chart types documented. Phase 2/3: per-point coloring, tooltips, IsValueShownAsLabel, MarkerStyle.

 Team update (2026-02-23): P2 features complete  all 1,065 tests pass, 0 build errors  decided by Squad
 Team update (2026-02-23): UI overhaul requested by Jeffrey T. Fritz  scope document created  decided by Jeffrey T. Fritz

### Summary: Milestone 7 Planning (2026-02-23)

Planned M7: "Control Depth & Navigation Overhaul" — 51 work items targeting ~138 gap closures. Key findings from post-M6 codebase audit:

- **GridView (~55% post-M6):** Has paging, sorting, editing from M6. Still missing selection (SelectedIndex/SelectedRow/SelectedRowStyle), 6 style sub-components, display props (ShowHeader/ShowFooter/Caption/GridLines/EmptyDataTemplate). Selection is the last major functional gap.
- **Menu (~42%):** Only got Orientation in M6. Still missing ~35 props — base style props (needs BaseStyledComponent upgrade), selection tracking, MenuItemClick/MenuItemDataBound events, level styles. JS interop complicates base class change.
- **TreeView (~60%):** Untouched in M6. Has solid core (nodes, data binding, checkboxes, expand/collapse, image sets). Missing node-level styles (TreeNodeStyle objects), functional selection, ExpandAll/CollapseAll, ExpandDepth, FindNode.
- **FormView (~50%):** M6 added header/footer/empty. Still missing style sub-components, paging events (PageIndexChanging/Changed), ModeChanged/ItemCommand events, PagerSettings.
- **DetailsView (~70%):** Has strong event coverage (10 CRUD/mode/paging events). Missing all 10 style sub-components, PagerSettings, Caption.
- **ListView (~42%):** Barely touched. Has excellent templates but missing all 16 CRUD events, editing templates, selection, sorting. Deferred to P2 due to L size.
- **DataGrid (~55%):** Has command events from original impl + style inheritance from M6. Missing style sub-components, paging/sorting events. Also P2.

**Key patterns confirmed:**
- Style sub-components are the single biggest systematic remaining gap across all data controls
- PagerSettings should be a shared type (GridView, FormView, DetailsView all need identical API)
- Validator ControlToValidate string ID is a migration-blocking mismatch — ForwardRef doesn't match the "paste and it works" migration story
- Diminishing returns are real: M6 closed ~345 gaps, M7 targets ~138 because remaining gaps require more work per gap (style sub-components, event pipelines vs. base class inheritance)
- Re-audit must open the milestone — all planning-docs/ files are stale (pre-M6 numbers)

📌 Team update (2026-02-23): Milestone 7 planned — 51 WIs, ~138 gaps, "Control Depth & Navigation Overhaul". P0: GridView completion + re-audit. P1: TreeView, Menu, DetailsView, FormView, Validators. P2: ListView CRUD, DataGrid, Menu levels. — decided by Forge

 Team update (2026-02-24): Menu auto-ID pattern established  components with JS interop should auto-generate IDs when none provided  decided by Cyclops
 Team update (2026-02-24): Substitution/Xml formally deferred in status.md and README  decided by Beast
 Team update (2026-02-24): M8 scope excludes version bump to 1.0 and release  decided by Jeffrey T. Fritz
 Team update (2026-02-24): PagerSettings shared sub-component created for GridView/FormView/DetailsView  decided by Cyclops
