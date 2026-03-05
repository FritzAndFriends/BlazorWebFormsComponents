# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents  Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!--  Summarized 2026-02-27 by Scribe  covers M1M16 -->

### Core Context (2026-02-10 through 2026-02-27)

**M1M3 QA:** Triaged PR #333 Calendar. 71 bUnit tests for Sprint 3 (42 DetailsView + 29 PasswordRecovery). DetailsView is generic DataBoundComponent<ItemType>, PasswordRecovery needs NavigationManager mock.

**M4 Chart QA:** 152 bUnit tests (BunitContext with JSInterop.Mode=Loose). ChartConfigBuilder most testable (pure static). GetPaletteColors internal  tested indirectly. ChartSeriesDataBindingHelper documents binding contract.

**M6 P0 QA:** 44 tests for base class changes (AccessKey, ToolTip, ImageStyle, LabelStyle, StyleInheritance, ValidatorDisplay, SetFocusOnError). Fixed DataList duplicate AccessKey bug. WebColor "LightGray""LightGrey" via ColorTranslator.

**M7 QA:** 24 GridView tests (Selection 7, StyleSubComponents 8, DisplayProperties 9). 41 P2 tests (ListView CRUD 12, DataGrid Styles 11 + Events 3, Menu LevelStyles 7, Panel BackImageUrl 3, Login Orientation 5).

**M9 QA:** 24 tests (ToolTip 20 across 9 controls, CommaSplit 4). Validation messages stored as `Text,ErrorMessage\x1F ValidationGroup`. ToolTip renders as title on outermost element.

**M16 QA:** 18 LoginView/PasswordRecovery OuterStyle tests. 12 ClientIDMode tests (Static 3, Predictable 3, AutoID 2, Inherit 2, Edge Cases 2). Found UseCtl00Prefix regression  fixed via NamingContainer auto-AutoID.

**Post-Bug-Fix Capture Pipeline (2026-02-26):** Re-ran full HTML capture after 14 bug fixes. 132131 divergences, 01 exact match (Literal-3). 11 targeted controls show improvements. Primary blocker: sample data parity, not component bugs.

### Key Test Patterns

- **Validator Display:** EditForm + InputText + RequiredFieldValidator. Static  visibility:hidden, Dynamic  display:none, None  always display:none. SetFocusOnError uses JSInterop.SetupVoid/VerifyInvoke.
- **BaseListControl:** GetItems() applies DataTextFormatString to both static and data-bound items. AppendDataBoundItems=false replaces static items.
- **Menu:** FindAll("li a") to exclude skip-link. JSInterop.Mode = Loose. `Orientation ori = Orientation.Horizontal;` variable pattern avoids Razor collision.
- **Login controls:** AuthenticationStateProvider + NavigationManager mocks. LoginView uses fully-qualified type to avoid ambiguity.
- **GridView styles:** Named RenderFragments (`<RowStyleContent>` etc.) with GridViewRowStyle. AlternatingRowStyle always initialized. AutoGenerateColumns=false renders in two passes.
- **DataGrid styles:** WebColor values must be variables, not string literals.
- **ListView:** HandleCommand via cut.InvokeAsync() for Blazor Dispatcher context.
- **PasswordRecovery:** outer table always includes border-collapse:collapse  use ShouldContain.
- **ClientIDMode:** Button as test component. `@using BlazorWebFormsComponents.Enums` required.
- **Pipeline:** normalize-html.mjs --compare uses RAW files. Normalize both sides first. Sample data alignment is prerequisite for accurate comparison.

 Team update (2026-02-27): Branching workflow directive  feature PRs from personal fork to upstream dev, only devmain on upstream  decided by Jeffrey T. Fritz

 Team update (2026-02-27): Issues must be closed via PR references using 'Closes #N' syntax, no manual closures  decided by Jeffrey T. Fritz


 Team update (2026-02-27): M17 AJAX controls implemented  ScriptManager/Proxy are no-op stubs, Timer shadows Enabled, UpdatePanel uses ChildContent, UpdateProgress renders hidden, Substitution uses Func callback, new AJAX/Migration Helper categories  decided by Cyclops


 Team update (2026-02-27): M17 sample pages created for Timer, UpdatePanel, UpdateProgress, ScriptManager, Substitution. Default.razor filenames. ComponentCatalog already populated  decided by Jubilee

<!-- Summarized 2026-03-02 by Scribe -- covers M17 + Issue #379 -->

<!-- Summarized 2026-03-05 by Scribe -- covers M17 tests through ListView CRUD -->

### M17-M22 Test Summary (2026-02-27 through 2026-03-03)

**M17 AJAX (47 tests, 6 files):** Timer (9), ScriptManager (9), ScriptManagerProxy (4), UpdatePanel (10), UpdateProgress (9), Substitution (6). Fixed Timer duplicate [Parameter] Enabled. M17 audit fix tests (9). LinkButton CssClass (8, Issue #379). Patterns: C# API for timer, no-op stubs test defaults/no-throw, DynamicLayout controls display/visibility.

**M20 Theming (13 tests, Issue #368):** ThemingPipelineTests.razor with Button/Label/Panel. Default skin, named SkinID, explicit-overrides-theme, EnableTheming=false, nested override, multi-property/multi-control. FontInfo sync (11 tests): Name/Names bidirectional, null/empty clears both, last-write-wins.

**Issue #406 + FormView + ListView CRUD:** EditItemTemplate (6 TDD tests, CSS class selectors). FormView RenderOuterTable (8 tests, element count comparison). ListView CRUD (43 tests, all 16 events): firing+EventArgs, cancellation, ordering, HandleCommand routing, CancelMode, lifecycle sequences. Key: CSS class selectors for template switching, cut.InvokeAsync for HandleCommand, FindAll().Count for structural.

Team updates (2026-02-27-03): M17 audit fixes (PR #402), MenuItemStyle Font-attrs, CheckBox bare id, Skins roadmap, unified release (PR #408), ModelErrorMessage, IOrderedDictionary on ListView EventArgs, migration toolkit restructured.
Team updates (2026-03-02-03): Skins roadmap (Forge), M22 planned (Forge), project reframed as migration system (Jeff), ModelErrorMessage spec (Forge), themes last directive (Jeff Fritz), IOrderedDictionary on ListView EventArgs (Cyclops), WingtipToys 7-phase schedule (Forge).


<!-- Summarized 2026-03-05 by Scribe -- covers WebFormsPageBase tests -->

<!-- Summarized 2026-03-06 by Scribe -- covers WebFormsPage head tests through LoginView test redesign -->

### WebFormsPage & Migration Script Test Summary (2026-03-04 through 2026-03-06)

**WebFormsPageBase (8 tests):** Title/MetaDescription/MetaKeywords delegate to IPageService, IsPostBack=false, Page=this. Concrete `TestPage` subclass, IPageService as scoped. **WebFormsPage head rendering (7 tests):** TDD tests for Option B consolidation (FindComponent<PageTitle>/HeadContent, RenderPageHead=false). 3 pass (existing behavior), 4 expected failures awaiting Cyclops.

**Event handler migration audit:** bwfc-migrate.ps1 does zero event handler transforms (defers to Layer 2). Accidental pass-through works for most BWFC EventCallback params. Gaps: AutoPostBack not stripped (6+ [Obsolete] warnings), Repeater has no event params, GridView PageIndexChanged name mismatch, inconsistent On-prefix naming, no ManualItem warnings for signature changes (Web Forms 2-param vs BWFC single-param).

**Migration script fixes:** AutoPostBack added to $StripAttributes + ManualItem emission. Event handler signature scan via `(On[A-Z]\w+)="[^"]*"` regex, summary-level ManualItem per file.

**LoginView AuthorizeView test redesign (8 files):** Replaced manual AuthenticationStateProvider mocking with bUnit `AddTestAuthorization()` API (SetNotAuthorized/SetAuthorized/SetRoles). Wrapper `<div>` assertions → `cut.Markup` assertions. `<ChildContent>` → `<RoleGroups>` parameter. OuterStyle.razor DELETED (10 tests removed — LoginView inherits Control, not WebControl, so no style properties).

**Key patterns:** `AddTestAuthorization()` for AuthorizeView-based components, `cut.Markup` for no-wrapper-element components, CSS class selectors for template switching.

📌 Team update (2026-03-05): WebFormsPage now includes IPageService head rendering — decided by Forge, implemented by Cyclops
📌 Team update (2026-03-05): AfterWingtipToys must only be produced by migration toolkit output, never hand-edited — decided by Jeffrey T. Fritz
📌 Team update (2026-03-05): Migration scripts must preserve GridView structure; ShoppingCart.aspx added as Layer 1 regression test — decided by Forge
📌 Team update (2026-03-05): BWFC control preservation is mandatory — all migration output must use BWFC components, never flatten to raw HTML — decided by Jeffrey T. Fritz, Forge, Cyclops
📌 Team update (2026-03-05): LoginView redesigned to delegate to AuthorizeView — decided by Forge

 Team update (2026-03-05): LoginStatus flagged for AuthorizeView redesign  decided by Forge
