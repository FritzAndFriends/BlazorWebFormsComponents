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

### WebFormsPageBase Test Summary (2026-03-04)

8 bUnit tests (all pass): Title/MetaDescription/MetaKeywords delegate to IPageService, IsPostBack always false, Page returns this. Used concrete `TestPage : WebFormsPageBase` with public accessors. Registered IPageService as scoped. Simpler test setup than most components (no JSInterop/LinkGenerator mocking needed).

Team updates: PRs upstream, Run 6 analysis.
### WebFormsPage Head Rendering Tests (2026-03-05)

**7 bUnit tests for WebFormsPage head rendering (3 pass, 4 expected failures):** Tests written ahead of Cyclops's enhancement to merge Page.razor head rendering into WebFormsPage (Option B from forge-page-consolidation decision). Test file: `src/BlazorWebFormsComponents.Test/WebFormsPage/WebFormsPageHeadRenderingTests.razor`.

Tests: (1) RendersPageTitle when IPageService registered + title set — uses `FindComponent<PageTitle>()`, (2) RendersMetaDescription — checks `FindComponent<HeadContent>()` markup for `<meta name="description">`, (3) RendersMetaKeywords — same pattern for keywords, (4) DoesNotRenderHeadContent when RenderPageHead=false — asserts `FindComponents<PageTitle>().Count.ShouldBe(0)`, (5) WorksWithoutIPageServiceRegistered — verifies naming still works without IPageService in DI, (6) CascadesTheme while also rendering head content — verifies theme + head coexistence, (7) ChildContent renders normally — basic child content pass-through.

**Currently passing (3):** Tests 4, 5, 7 pass — they test existing behavior or pass vacuously since head rendering isn't implemented yet. **Expected failures (4):** Tests 1, 2, 3, 6 fail because WebFormsPage doesn't yet have IPageService integration or RenderPageHead parameter — awaiting Cyclops's implementation. Existing 6 WebFormsPage tests unaffected.

Key patterns: `FindComponent<PageTitle>()` and `FindComponent<HeadContent>()` for asserting Blazor built-in head components. `RenderPageHead="false"` captured by `BaseWebFormsComponent`'s unmatched attributes until Cyclops adds the actual parameter. Pre-set `PageService` values before rendering (not after) for initial render assertions.

� Team update (2026-03-05): WebFormsPage now includes IPageService head rendering (title + meta tags), merging Page.razor capability per Option B consolidation. Layout simplified to single <WebFormsPage> component. Page.razor remains standalone.  decided by Forge, implemented by Cyclops

### Event Handler Migration Audit (2026-03-06)

**Migration script event handler handling:** bwfc-migrate.ps1 performs ZERO event handler transforms. Line 17 explicitly defers to Layer 2. Event handler attributes (`OnClick`, `OnSelectedIndexChanged`, etc.) pass through unchanged after `asp:` prefix removal. This accidental pass-through works for most BWFC components because BWFC defines matching `[Parameter] EventCallback` properties. Both `OnClick="Handler"` and `OnClick="@Handler"` compile in Blazor for EventCallback parameters.

**Gaps identified:**
1. `AutoPostBack` is NOT in `$StripAttributes` — passes through unchanged, triggers `[Obsolete]` warnings on 6+ BWFC components (DropDownList, CheckBox, RadioButton, CheckBoxList, RadioButtonList, ListBox). Should be stripped + ManualItem warning emitted.
2. Repeater has NO event parameters in BWFC — `OnItemCommand` silently lost.
3. GridView uses `PageIndexChanged` not `OnPageIndexChanging` — name mismatch causes silent failure.
4. BWFC naming is inconsistent: some events keep `On` prefix (OnRowCommand, OnItemCommand on DataGrid), others drop it (Sorting, RowEditing, ItemCommand on ListView/DetailsView). Web Forms always uses `On` prefix in markup.
5. No ManualItem warnings emitted for event handler attributes despite code-behind signature changes being required.
6. CommandArgument with data-binding expressions may lose `@` prefix needed for Blazor attribute expression evaluation.

**Web Forms vs Blazor event signature differences:** All Web Forms handlers use `(object sender, EventArgs e)` — two params. BWFC EventCallbacks accept only the event args (no sender). Every code-behind event handler will have compile errors. The code-behind TODO header mentions this generically but emits no per-handler ManualItem entries.

**Findings written to:** `.ai-team/decisions/inbox/rogue-event-handler-migration-gaps.md`

### Migration Script Fixes — AutoPostBack & Event Handler Gaps (2026-03-06)

**Fix 1 — AutoPostBack stripping:** Added `'AutoPostBack\s*=\s*"(true|false)"'` to `$StripAttributes` array (line 81). Added conditional ManualItem emission inside `Remove-WebFormsAttributes` loop — when `$friendlyName -eq 'AutoPostBack'`, emits category `AutoPostBack` warning about Blazor's immediate-fire event model vs Web Forms delayed postback.

**Fix 2 — Event handler signature warnings:** Added post-`Remove-WebFormsAttributes` scan in `Convert-WebFormsFile` pipeline (after line 1045). Uses `[regex]::Matches($content, '(On[A-Z]\w+)="[^"]*"')` to find event handler attributes in the already-transformed Blazor markup. Collects unique handler names via `Sort-Object -Unique`, emits ONE `EventHandler` category ManualItem per file listing all found handlers with sender-parameter signature guidance.

**Patterns used:**
- `$StripAttributes` array: regex patterns matching `attribute="value"` with `\s*=\s*` whitespace tolerance
- ManualItem emission: conditional check on friendly name extracted by the existing `$pattern -replace` chain
- Event handler detection: `(On[A-Z]\w+)="[^"]*"` — captures attribute name in group 1, excludes lowercase-after-On to avoid false positives
- Summary-level ManualItem: one per file with comma-joined unique handler list, not per-attribute


 Team update (2026-03-05): AfterWingtipToys must only be produced by migration toolkit output, never hand-edited  decided by Jeffrey T. Fritz
 Team update (2026-03-05): Migration scripts must preserve GridView structure (not decompose to raw HTML); ShoppingCart.aspx added as Layer 1 regression test case  decided by Forge

 Team update (2026-03-05): BWFC control preservation is mandatory  all asp: controls must be preserved as BWFC components in migration output, never flattened to raw HTML. Test-BwfcControlPreservation verifies automatically.  decided by Jeffrey T. Fritz, implemented by Forge

