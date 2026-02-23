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
📌 Team update (2026-02-10): Close PR #333 without merging — all Calendar work already on dev, PR branch has 0 unique commits — decided by Rogue
📌 Team update (2026-02-10): Sprint 2 complete — Localize, MultiView+View, ChangePassword, CreateUserWizard shipped with docs, samples, tests. 709 tests passing. 41/53 components done. — decided by Squad

### 2026-02-10 — Sprint 3 Planning & Status Reconciliation

**Status.md was significantly stale:**
- Calendar was merged to dev via commit d33e156 and PR #339 but still marked 🔴 Not Started
- FileUpload was merged via PRs #335 and #338 but still marked 🔴 Not Started
- Summary table said 41/53 (Editor: 20/27) but actual count of ✅ entries in the detailed section was already 23/27 for Editors (now 25/27 with Calendar + FileUpload fixed)
- The 27-count for Editor Controls groups MultiView and View as one logical component despite separate table rows
- Corrected total: 48/53 components complete (91%), 5 remaining

**Sprint 3 scope decision:**
- DetailsView and PasswordRecovery are the two buildable components
- Chart deferred: requires SVG/Canvas rendering engine, no Blazor primitive equivalent
- Substitution deferred: Web Forms output caching has no Blazor architectural equivalent
- Xml deferred: XSLT transforms are a dead-end pattern with near-zero migration demand
- Post-Sprint 3 state: 50/53 (94%), library effectively feature-complete for practical migration

**DetailsView design notes:**
- Must inherit BaseStyledComponent (Web Forms DetailsView → CompositeDataBoundControl → WebControl)
- Renders as `<table>` with one `<tr>` per field (vertical layout vs GridView's horizontal)
- Can reuse existing BoundField, TemplateField, CommandField, HyperLinkField, ButtonField from GridView
- Needs DetailsViewMode enum (ReadOnly=0, Edit=1, Insert=2)
- Needs 8 EventArgs classes for mode changes, CRUD operations

**PasswordRecovery design notes:**
- Must inherit BaseStyledComponent
- 3-step wizard flow: UserName → Question → Success (same pattern as CreateUserWizard's 2-step)
- Can reuse existing LoginControls style sub-components (TitleTextStyle, TextBoxStyle, LabelStyle, etc.)
- Table-based HTML output matching ChangePassword's render pattern

📌 Team update (2026-02-10): Sprint 3 plan ratified — DetailsView + PasswordRecovery. Chart/Substitution/Xml deferred indefinitely with migration docs. 48/53 → target 50/53. — decided by Forge
📌 Team update (2026-02-11): Colossus added as dedicated integration test engineer. Rogue retains bUnit unit tests. — decided by Jeffrey T. Fritz

### 2026-02-11 — Sprint 3 Gate Review

**DetailsView — APPROVED:**
- Inherits `DataBoundComponent<ItemType>` — correct for data-bound controls. Uses same `Items` property as GridView/ListView.
- All 10 Web Forms events implemented with correct `EventArgs` types. Pre-operation events support cancellation.
- `DetailsViewMode` enum (ReadOnly=0, Edit=1, Insert=2) matches Web Forms exactly.
- HTML output: `<table>` with one `<tr>` per field, command row with `<a>` links, nested-table numeric pager — all match Web Forms.
- Auto-generation via reflection correctly generates fields from `ItemType` properties.
- Minor issues (non-blocking): `CombinedStyle` has CellPadding/CellSpacing logic mismatch, `cellspacing` hardcoded to 0 in template, docs use `DataSource` but actual parameter is `Items`.
- DetailsView docs `DataSource`→`Items` fix assigned to Beast.

**PasswordRecovery — APPROVED:**
- Inherits `BaseWebFormsComponent` — consistent with ChangePassword and CreateUserWizard pattern.
- 3-step wizard flow (UserName → Question → Success) matches Web Forms exactly.
- Reuses existing `LoginCancelEventArgs`, `TableItemStyle`, `Style` cascading parameter pattern from other Login Controls.
- `SuccessTextStyle` sub-component added following existing `UiTableItemStyle` pattern.
- All 6 events implemented: `OnVerifyingUser`, `OnUserLookupError`, `OnVerifyingAnswer`, `OnAnswerLookupError`, `OnSendingMail`, `OnSendMailError`.
- `SetQuestion()` and `SkipToSuccess()` APIs provide developer control matching Web Forms extensibility.
- Table-based nested HTML output matches Web Forms PasswordRecovery output.
- Minor issues (non-blocking): `RenderOuterTable` declared but not used, `SubmitButtonType`/`SubmitButtonImageUrl` declared but not rendered, sample uses `e.Sender` casting instead of `@ref`.

**Key Patterns Confirmed:**
- Login Controls consistently inherit `BaseWebFormsComponent` (not `BaseStyledComponent`) and use cascading `TableItemStyle`/`Style` objects for styling — this is an established project convention.
- Data-bound controls inherit `DataBoundComponent<T>` which provides `Items` (not `DataSource`) as the primary binding parameter.
- Event naming in Login Controls uses `On` prefix (`OnVerifyingUser`, `OnChangingPassword`) — project convention, not Web Forms convention.
- Both components ship with docs, samples, and tests per Sprint 2 policy.

**Sprint 3 Status:**
- 50/53 components complete (94%)
- 797 tests passing, 0 build errors
- 3 remaining (Chart, Substitution, Xml) deferred indefinitely
- Library is effectively feature-complete for practical Web Forms migration

📌 Team update (2026-02-11): Sprint 3 gate review — DetailsView APPROVED, PasswordRecovery APPROVED. 50/53 complete (94%). — decided by Forge
