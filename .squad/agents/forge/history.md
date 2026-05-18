# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents  Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Core Context

<!-- Summarized 2026-03-04 by Scribe — originals in history-archive.md -->


### 2026-04-28: Semantic Pattern Infrastructure Sprint - All Agents

**Task:** Complete semantic pattern infrastructure for BlazorWebFormsComponents semantic pattern catalog.

**Bishop:**
- Implemented pattern-query-details and pattern-action-pages infrastructure
- Wired production and test registration for all patterns
- Added isolated and pipeline regression tests

**Cyclops:**
- Implemented pattern-account-pages infrastructure
- Implemented pattern-master-content-contracts with helper logic
- Added focused concrete tests

**Forge:**
- Performed comprehensive reviewer safety pass
- Approved bounded semantics and manual TODO boundaries
- Special review of authentication and master/content section patterns

**Rogue:**
- QA audit identified missing default registration gap
- Recommended helper and integration test coverage
- Re-check confirmed gap was resolved by Bishop

**Coordinator:**
- Executed full test suite: 486 passed, 0 failed
- Verified all tests passing before archival

**Outcome:** All semantic pattern contracts approved and production-ready.

≡ Team update (2026-05-07): Inbox merged, decisions consolidated — Scribe

### 2026-05-16: DepartmentPortal Migration Readiness Analysis

**Task:** Full gap analysis of `samples/DepartmentPortal/` to identify what the CLI and BWFC library must add before this benchmark can migrate at WingtipToys/ContosoUniversity fidelity.

**Key findings:**

- DepartmentPortal has 14 pages (11 root + 3 Admin), 12 `.ascx` user controls, and 7 code-only server controls (no `.ascx` file).
- **Data layer is trivial** — no EF, no database, pure static in-memory `PortalDataProvider`. Simpler than both WT and CU on this axis.
- **Control infrastructure is the hardest thing we have ever attempted to migrate.** Seven code-only controls in `Code/Controls/` (`SectionPanel`, `EmployeeDataGrid`, `EmployeeCard`, `PollQuestion`, `StarRating`, `NotificationBell`, `DepartmentBreadcrumb`) inherit from `WebControl`, `CompositeControl`, `DataBoundControl`, `Control+IPostBackEventHandler` — patterns the CLI has never seen.
- **`local:` tag prefix** is a Web.config namespace-level registration, not a `src=` file path. The CLI currently only resolves file-path control registrations. This is a blocker for every page that uses code-only controls.
- **`SectionPanel` with `<ContentTemplate>`** is a `[ParseChildren(true)]` templated container control. Pages reach inside it via `EnsureChildControls()` + `FindControl()`. Used 4 times across two pages.
- **`BasePage` / `BaseUserControl` / `BaseMasterPage`** are custom intermediate base classes. CLI's `BaseClassStripTransform` only knows `System.Web.UI.Page`. Every page's `IsAdmin`, `CurrentUser`, `ShowMessage()` call will fail to compile without shims.
- **`FindControl` typed cast pattern** is pervasive — 5 pages use `(DepartmentPortal.Controls.TypeName)FindControl("id")` to set properties on user controls. This must become `@ref` field access.
- **`Dashboard.aspx` has no `MasterPageFile` in its page directive** — it's set dynamically in `BasePage.OnPreInit`. The page directive scanner will miss layout assignment for this page.
- **`HttpRuntime.Cache`** (not `Page.Cache`) is used in `BaseUserControl.CacheGet/Set`. The existing `CacheShim` doesn't cover this API.
- **`IPostBackEventHandler.RaisePostBackEvent` + `GetPostBackEventReference`** pattern in `PollQuestion` and `DepartmentBreadcrumb` — zero Blazor analog; must become `EventCallback` components.
- **Admin pages** are partially manageable. `ManageTraining.aspx` has a pre-existing bug — GridView binds `InstructorName` and `IsAvailable` which don't exist on `TrainingCourse`.
- **Five Tier 1 blockers** must all be resolved before any first-pass build: (1) code-only control scaffolding, (2) namespace-level tag prefix parsing, (3) `<ContentTemplate>` unwrapping, (4) custom base class shims, (5) `FindControl` cast transform.
- Estimated DepartmentPortal complexity: ★★★ vs WT/CU ★★☆. Harder structurally despite simpler data.

**Deliverable:** Full gap analysis written to `.squad/decisions/inbox/forge-deptportal-analysis.md`

