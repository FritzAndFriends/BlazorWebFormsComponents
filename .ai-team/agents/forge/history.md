# Project Context

- **Owner:** Jeffrey T. Fritz (csharpfritz@users.noreply.github.com)
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-02-10 — PR Review & Sprint Planning Session

**PR #333 (Calendar):**
- Strongest of the 6 PRs. Table-based rendering matches Web Forms output. 19 tests. SelectionMode uses string instead of enum — Web Forms uses `CalendarSelectionMode` enum (None/Day/DayWeek/DayWeekMonth). Missing `CalendarSelectionMode` enum in Enums/. Style properties use CSS class strings (`TitleStyleCss`) instead of Web Forms `TableItemStyle` objects — acceptable pragmatic trade-off for Blazor. Missing: `UseAccessibleHeader` property, `Caption`/`CaptionAlign` properties. The `.GetAwaiter().GetResult()` call in `CreateDayRenderArgs` is a blocking anti-pattern but necessary for synchronous rendering. Overall quality is high.

**PR #335 (FileUpload):**
- Inherits `BaseStyledComponent` ✓. Uses `<input type="file">` — correct HTML output. `OnFileChangeInternal` uses raw `ChangeEventArgs` instead of Blazor `InputFile`/`IBrowserFile` pattern — the `@onchange` binding won't actually populate `_currentFile`. This is a broken data flow: files will never be loaded. Has security comments from GitHub Advanced Security about `_currentFiles` readonly and `Path.Combine` traversal risk. `Accept` and `AllowMultiple` attributes correct. Missing: `HasFiles` (plural) property from Web Forms.

**PR #337 (ImageMap):**
- Correctly renders `<img>` + `<map>` + `<area>` HTML structure matching Web Forms. HotSpot hierarchy (HotSpot → RectangleHotSpot/CircleHotSpot/PolygonHotSpot) matches Web Forms class hierarchy exactly. Implements `IImageComponent` interface. Uses `BaseWebFormsComponent` not `BaseStyledComponent` — this is wrong; Web Forms `ImageMap` inherits from `Image` which inherits `WebControl` which has style properties. Static `_mapIdCounter` with `Interlocked.Increment` is thread-safe but will leak across test runs. Missing: `Enabled` property handling for areas.

**PR #327 (PageService):**
- Novel approach — not a direct Web Forms control, but emulates `Page.Title`, `Page.MetaDescription`, `Page.MetaKeywords`. Uses DI service pattern (IPageService) — idiomatic Blazor. Renders `<PageTitle>` and `<HeadContent>` — correct for Blazor 6+. Generic catch clauses flagged by code scanning. Useless variable assignments in tests flagged. Solid architectural approach for the migration use case.

**PR #328 (ASCX CLI Tool):**
- Merge conflicts — NOT mergeable. Draft status. Converts `<%@ Control %>`, `<asp:*>`, `<%: %>`, `<%= %>`, `<%# %>`, `<% %>` blocks. Has `AiAssistant` stub class. No tests visible in the tool project itself. This is a companion tool, not a component — different review criteria. Needs conflict resolution and test coverage before merge.

**PR #309 (VS Snippets):**
- Merge conflicts — NOT mergeable. 13 VS 2022 snippets as VSIX. Not a component — tooling review. Snippets for static imports and component patterns. Useful but needs rebase to resolve conflicts.

**Key Patterns Discovered:**
- Copilot-authored PRs consistently use good XML doc comments
- Components generally follow the project's base class hierarchy correctly
- Calendar uses string-based SelectionMode instead of enum — inconsistent with project enum pattern
- FileUpload has a fundamental data flow bug with `@onchange` not populating file data
- ImageMap should inherit BaseStyledComponent, not BaseWebFormsComponent
- Two PRs (#328, #309) have merge conflicts blocking any merge

**Sprint Planning Decisions:**
- Sprint 1 should focus on landing Calendar (with SelectionMode enum fix) and PageService, plus fixing merge conflicts on tooling PRs
- Sprint 2 should tackle remaining Editor Controls (MultiView/View, Localize) and start Login Controls
- Sprint 3 should cover Data Controls gaps (DetailsView) and documentation/sample catch-up

📌 Team update (2026-02-10): PRs #328 (ASCX CLI) and #309 (VS Snippets) shelved indefinitely — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): Docs and samples must ship in the same sprint as the component — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): Sprint plan ratified — 3-sprint roadmap established — decided by Forge
📌 Team update (2026-02-10): Sprint 1 gate review — Calendar (#333) REJECTED (assigned Rogue), FileUpload (#335) REJECTED (assigned Jubilee), ImageMap (#337) APPROVED, PageService (#327) APPROVED — decided by Forge
📌 Team update (2026-02-10): Lockout protocol — Cyclops locked out of Calendar and FileUpload revisions — decided by Jeffrey T. Fritz
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

### 2026-02-12 — Chart Component JS Library Evaluation

**Requested by Jeffrey T. Fritz.** Performed full architectural evaluation of using a JavaScript charting library to implement the Web Forms `System.Web.UI.DataVisualization.Charting.Chart` control (previously deferred as "very high complexity").

**Key findings:**
- Web Forms Chart renders an `<img>` tag pointing to a server-generated PNG via `ChartImg.axd` — this is architecturally impossible to replicate in Blazor (no GDI+, no server-side image handler pipeline). Any implementation must accept a HTML output deviation.
- The `SeriesChartType` enum has 35 chart types (Point through Pyramid). Realistic Phase 1 subset: 8 types (Column, Bar, Line, Pie, Area, Doughnut, Scatter, StackedColumn) covering 90%+ of real-world usage.
- Chart's key sub-objects: Series, DataPoint, ChartArea, Axis, Legend, Title — must be mirrored as Blazor sub-components.

**Library evaluation results:**
- **D3.js — REJECTED.** Low-level SVG manipulation toolkit, not a charting library. Zero built-in chart types. No Blazor wrapper. Would require building an entire charting engine from scratch (XL effort). Jeffrey's suggestion, but wrong abstraction level.
- **Chart.js — RECOMMENDED.** MIT license, ~60KB gzipped, ~10 built-in chart types covering all Phase 1 needs, multiple Blazor wrappers exist (though we recommend bundling chart.min.js directly and writing thin interop, not depending on community wrapper NuGet packages).
- **ApexCharts — STRONG ALTERNATIVE.** MIT, ~120KB gz, 20+ chart types, official Blazor wrapper (Blazor-ApexCharts). Better coverage but double the bundle size and the wrapper has its own opinionated API.
- **Plotly.js — REJECTED.** 3-4MB gzipped bundle size is disqualifying. Scientific/3D features are overkill.

**Architecture recommendation:**
- Use Chart.js bundled as a static asset (~60KB)
- Thin JS interop layer (`chart-interop.js`) translating C# config → Chart.js config
- Blazor Chart component mirrors Web Forms property names (Series, ChartAreas, etc.)
- `<canvas>` output instead of `<img>` — documented as justified exception
- Inherit `DataBoundComponent<T>` (consistent with Web Forms Chart inheriting DataBoundControl)
- Effort: L (Large) — first component requiring JS interop, new testing challenges (canvas can't be bUnit-tested)

**Risks identified:**
1. JS interop is unprecedented in this project — new build/packaging complexity
2. Canvas content not testable via bUnit — Playwright becomes primary quality gate
3. Chart.js version pinning required — major version upgrades could break interop
4. SSR/prerendering needs special handling (canvas requires DOM)

**Recommendation: Proceed, but as Sprint 4** — not tacked onto Sprint 3. The JS interop pattern warrants dedicated design review and planning.

📌 Team update (2026-02-12): Chart component feasibility confirmed — Chart.js recommended via JS interop. HTML output deviation (`<canvas>` not `<img>`) justified and documented. Effort: L. Target Sprint 4. — decided by Forge
