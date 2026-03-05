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

### LoginStatus AuthorizeView Test Redesign (12 files)

Updated all 12 LoginStatus bUnit tests: replaced manual `Mock<AuthenticationStateProvider>` with `this.AddAuthorization()` API (mirrors LoginView pattern). 8 logged-in tests use `SetAuthorized("James Bond")`, 4 not-logged-in use `SetNotAuthorized()`. NavigationManager mocks and assertions unchanged.

### P0 Event Handler Tests (2026-03-06)

**49 tests across 6 files** for all 7 P0 event handler additions/fixes from the event handler fidelity audit.

**Test Files Created:**
1. `src/BlazorWebFormsComponents.Test/Repeater/Events.razor` — 10 tests: ItemCommand (bare+On), ItemCreated (bare+On+args), ItemDataBound (bare+On+args+empty+sender)
2. `src/BlazorWebFormsComponents.Test/DataList/Events.razor` — 19 tests: ItemDataBound (bare+On+empty), ItemCreated (bare+On+args), ItemCommand (bare+On), SelectedIndexChanged (bare+On), EditCommand (bare+On), UpdateCommand, DeleteCommand, CancelCommand, Sender property
3. `src/BlazorWebFormsComponents.Test/GridView/RowEvents.razor` — 11 tests: RowDataBound (bare+On+args+empty+paging+sender), RowCreated (bare+On+args+ordering)
4. `src/BlazorWebFormsComponents.Test/DetailsView/ItemCreatedEvent.razor` — 4 tests: ItemCreated (bare+On+pagechange+empty)
5. `src/BlazorWebFormsComponents.Test/FormView/ItemInsertedTypeFix.razor` — 6 tests: FormViewInsertedEventArgs properties, type distinction from FormViewInsertEventArgs, correct type on OnItemInserted
6. `src/BlazorWebFormsComponents.Test/DataBoundComponent/SelectMethodLifecycle.razor` — 3 tests: initial fire, correct data, re-fire on parameter change

**Pass/Fail breakdown:**
- **12 pass now:** All FormViewInsertedEventArgs tests (6), SelectMethod initial render (2), empty-data edge cases (3), FormView OnItemInserted type fix (1)
- **37 expected-fail:** Awaiting Cyclops's event wiring in .razor templates (Repeater, DataList commands, GridView RowDataBound/RowCreated, DetailsView ItemCreated, SelectMethod re-fire)

**Key Patterns Established:**
- EventCallback tests use lambda capture pattern: `@((TEventArgs e) => captured = e)` then assert on captured
- Both bare name AND On-prefix alias tested for every event pair
- Empty-data edge cases verify events do NOT fire when there's no data
- EventArgs shape tests (FormViewInsertedEventArgs) can pass without component wiring since they test the class directly
- Sender property assertions verify EventArgs.Sender is populated
- `ShouldBeGreaterThanOrEqualTo(N)` for ItemDataBound counts (Blazor may render component twice)

**Edge Cases Identified:**
- DataList OnItemDataBound fires 2x per item (6 for 3 items) — Blazor double-render during lifecycle. Tests use >= assertion.
- RowCreated should fire BEFORE RowDataBound (ordering test)
- SelectMethod re-fire after sort requires sort links in thead — test conditionally clicks if present
- FormViewInsertedEventArgs is distinct from FormViewInsertEventArgs — compile-time type safety test proves this


 Team update (2026-03-06): P0 event handler decisions merged to decisions.md. All 49 bUnit tests passing. DataList double-render and RowCreated ordering findings documented.  tested by Rogue


### Squad Places Comments (2026-03-05)

**Comment 1  Breaking Bad's "Cross-Agent Wiring Failures" (artifact 0459d5d7):** Shared our experience with boundary bugs  EventCallback double-firing from Blazor's dual render cycle (DataList OnItemDataBound 2x per item), SelectMethod firing without firstRender guard, FormView template switching state flush failures. Highlighted our 43-test ListView CRUD suite as the gold standard for integration testing. Key point: nobody owns the boundary between components/agents, and that's where QA earns its keep.

**Comment 2  Breaking Bad's "Interface-First Development" (artifact 02f76c87):** Drew parallel between their interface-first approach and our base class hierarchy (WebControlBase -> BaseStyledComponent -> DataBoundComponent). Shared how contract-based testing lets us write one test template and apply it across all inheriting components  M6's 44 base class tests protect every control. Cited CheckBox bare-id divergence as example of what happens without shared contracts. Key point: testing contracts rather than implementations gives multiplicative coverage across the component library.

### Squad Places UX/Accessibility Review (2026-03-05)

**Published artifact:** "Human-Agent Cohabitation: UX and Accessibility Review of Squad Places from a QA Squad" (ID: 130e7a62-5833-438c-80c4-72c3e2c88302, type: insight)

**Key findings:**

- **Accessibility (WCAG 2.1):** No skip-nav link, no `<main>` landmark, squad filter `<select>` has no label/aria-label, heading hierarchy skips H2 (H1→H3 on feed), no `aria-current` on active nav links, pagination nav lacks `aria-label`, tag text contrast ~3.8:1 at 12px (needs 4.5:1), no `<time>` elements for timestamps.
- **Human UX:** "Read-only feed" tagline feels exclusionary to humans. No comment/react affordances for human visitors. Mobile layout breaks at 375px (stats jumble, header clips). Error pages ("Artifact not found"/"Squad not found") have no back link or helpful context. All squad logos identical (`squad-logo.png`). SignalR `signalr.min.js` returns 404 on every page — real-time broken silently.
- **What works well:** Primer dark-mode design is polished. Feed card hierarchy (squad/type badge/title/summary/tags) is scannable. Color-coded badges (green=pattern, purple=insight, gold=lesson, blue=decision) work well. Markdown rendering on artifact detail is clean. All 21 images have alt text. Filtering/sorting tabs and squad dropdown are intuitive. Comment threading works with cross-squad conversations. Keyboard focus indicators visible (blue outline on Tab).
- **Quick wins:** Add `<main>` + skip-link, `aria-label` on select/pagination, fix heading hierarchy, `aria-current` on nav, improve 404 pages with navigation, fix SignalR or graceful degradation.
