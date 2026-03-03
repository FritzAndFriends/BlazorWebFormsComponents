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

### M17 and Issue #379 Test Summary (2026-02-27 through 2026-02-28)

**M17 AJAX tests (47 tests):** 6 test files -- Timer (9), ScriptManager (9), ScriptManagerProxy (4), UpdatePanel (10), UpdateProgress (9), Substitution (6). Fixed Timer duplicate [Parameter] Enabled bug (shadowed base class). Timer tests use C# API Render<Timer>(p => p.Add(...)). No-op stubs tested via cut.Markup.Trim().ShouldBeEmpty(). ScriptReference properties tested as plain C# instantiation.

**M17 Audit fix tests (9 tests):** Covered all 5 fixes -- EnablePartialRendering default true, Scripts collection, CssClass rendering, display:block;visibility:hidden, ScriptReference properties. UpdateProgress CssClass uses conditional null pattern. All 29 ScriptManager/UpdateProgress tests pass.

**LinkButton CssClass tests (8 tests, Issue #379):** Dedicated CssClass.razor -- single class, multiple classes, no/empty CssClass omits attribute, CssClass+ID coexist, PostBackUrl branch, disabled aspNetDisabled, disabled+CssClass. Two render branches tested (PostBackUrl null vs non-null). Edge case: GetCssClassOrNull() uses IsNullOrEmpty not IsNullOrWhiteSpace.

Key patterns: Timer parameter inheritance -- use C# API, not Razor templates. No-op stubs -- test defaults + no-throw. UpdateProgress DynamicLayout=true > display:none, false > display:block;visibility:hidden. UpdatePanel Block > div, Inline > span. LinkButton has two render paths both sharing GetCssClassOrNull().

 Team update (2026-02-27): M17 audit fixes resolved -- 5 fidelity issues fixed. 9 new tests, 1367 total. PR #402 -- decided by Forge, Cyclops

 Team update (2026-02-28): Cyclops fixed MenuItemStyle Font- attributes (SetFontsFromAttributes call in OnInitialized) and CheckBox bare input missing id attribute  may warrant additional test coverage. Issue #379 confirmed already fixed in M15.

<!-- Summarized 2026-03-02 by Scribe -- covers M20 theming + FontInfo tests -->

### M20 Theming & FontInfo Test Summary (2026-03-01 through 2026-03-02)

**Skins & Themes pipeline tests (13 tests, Issue #368):** ThemingPipelineTests.razor validates ThemeProvider > BaseWebFormsComponent > BaseStyledComponent pipeline using Button/Label/Panel. Covers: default skin, named SkinID, explicit-overrides-theme, EnableTheming=false, no ThemeProvider, missing SkinID graceful, nested ThemeProvider override, ForeColor/CssClass/Width/Height/Font properties, multi-control theming. Total: 1426 tests.

**FontInfo sync tests (11 tests):** 9 unit tests (FontInfoSyncTests.cs) + 2 pipeline tests verifying Name/Names bidirectional sync. Setting Name updates Names, setting Names updates Name (first entry). Null/empty clears both. Last-write-wins semantics. Pipeline tests confirm theme Font.Name renders font-family via auto-sync. Total: 1437 tests.

Key patterns: Button=input, Label=span, Panel=div for theme tests. Missing SkinID returns null (no skin, not even default). FontInfo sync tests are pure C# (no bUnit). Theme font-family pipeline: ApplyThemeSkin sets Font.Name > auto-sync > Font.Names > style builder renders.

Team updates: Unified release process (PR #408), Skins & Themes roadmap (3 waves, 15 WIs).


### ListView EditItemTemplate Tests (Issue #406, 2026-03-02)

6 bUnit tests in ListView/EditTemplateTests.razor: EditIndex matching/non-matching/negative, HandleCommand Edit/Cancel swap, null EditItemTemplate fallback. TDD: 2 pass (negative/null), 4 fail (the bug). CSS class selectors (span.display vs span.edit) for template identification. HandleCommand tests use cut.InvokeAsync() for Blazor dispatcher context.

### FormView RenderOuterTable Tests (2026-03-02)

8 bUnit tests in FormView/RenderOuterTable.razor: default/true/false rendering, template content, structural equivalence, empty data (text + template), edit mode. All pass. Compare element counts not raw Markup (Blazor generates unique event handler IDs per render). RenderOuterTable=false: no table/tr/td wrappers, empty data path also strips wrappers.

📌 Test patterns: (1) CSS class selectors for template switching tests. (2) cut.InvokeAsync for HandleCommand. (3) FindAll().Count for structural comparison, not ShouldBe on Markup.

📌 Team updates (2026-03-02): Skins roadmap (3 waves, 15 WIs, Forge), M22 planned (Forge), project reframed as migration system (Jeff), ModelErrorMessage 29/29 (Forge), WingtipToys pipeline validated — three-layer pipeline (Script→Skill→Agent), 18-26 hours total, scaffolded Identity UI for Account pages (Forge).
 Team update (2026-03-02): Full Skins & Themes roadmap defined  3 waves, 15 work items. Wave 1: Theme mode, sub-component styles (41 slots across 6 controls), EnableTheming propagation, runtime switching. See decisions.md for full roadmap and agent assignments  decided by Forge
### ListView EditItemTemplate Rendering Tests (Issue #406)

Wrote 6 bUnit tests in `src/BlazorWebFormsComponents.Test/ListView/EditTemplateTests.razor` for the EditItemTemplate rendering fix:

1. `EditIndex_MatchingItem_RendersEditItemTemplate` — EditIndex=0 parameter, verify span.edit appears for item 0
2. `EditIndex_NonMatchingItems_StillUseItemTemplate` — EditIndex=0, verify items 1+ still use span.display
3. `EditIndexNegativeOne_AllItemsUseItemTemplate` — default EditIndex=-1, all items use ItemTemplate ✅
4. `HandleCommand_Edit_SwapsToEditItemTemplate` — HandleCommand("Edit") triggers template swap in DOM
5. `HandleCommand_Cancel_RestoresItemTemplate` — Start in edit mode, cancel returns to ItemTemplate
6. `EditItemTemplateNull_FallsBackToItemTemplate` — EditIndex=0 but no EditItemTemplate, falls back to ItemTemplate ✅

**TDD results:** 2 pass (negative/null cases), 4 fail (template swap behavior — the exact bug #406 describes). All 39 pre-existing ListView tests unaffected.

📌 Test pattern: ListView EditItemTemplate rendering tests use CSS class selectors (`span.display` vs `span.edit`) to distinguish which template rendered for each item. This avoids fragile markup matching and clearly shows template selection per row. — Rogue

📌 Edge case: ListView.razor line 59 has the correct template selection logic (`EditIndex >= 0 && dataItemIndex == EditIndex && EditItemTemplate != null`) but it doesn't produce the expected DOM output when EditIndex is set via parameter. The bug is in the rendering pipeline, not the conditional logic. Tests confirm this — `theListView.EditIndex` is correctly set but the rendered HTML doesn't reflect it. — Rogue

📌 Test pattern: For HandleCommand-based tests, use `cut.InvokeAsync(() => theListView.HandleCommand(...))` pattern (from CrudEvents.razor) to ensure Blazor dispatcher context. Verify DOM state with `cut.FindAll()` AFTER the invoke, not just property values. — Rogue


 Team update (2026-03-02): M22 Copilot-Led Migration Showcase planned  decided by Forge

 Team update (2026-03-02): WingtipToys migration analysis complete  36 work items across 5 phases, FormView RenderOuterTable is only blocking gap  decided by Forge

### FormView RenderOuterTable Tests

Wrote 8 bUnit tests in `src/BlazorWebFormsComponents.Test/FormView/RenderOuterTable.razor` for the RenderOuterTable parameter:

1. `FormView_Default_RendersOuterTable` — default behavior (RenderOuterTable=true), verifies `<table>` + border-collapse present
2. `FormView_RenderOuterTableTrue_RendersOuterTable` — explicit true produces outer table
3. `FormView_RenderOuterTableFalse_NoOuterTable` — false removes all `<table>` elements, template content still renders
4. `FormView_RenderOuterTableFalse_RendersTemplateContent` — false still renders actual data from ItemTemplate
5. `FormView_RenderOuterTableTrue_MatchesDefaultStructure` — explicit true matches default DOM structure (table/tr/td/a counts)
6. `FormView_RenderOuterTableFalse_EmptyData_RendersEmptyDataText` — false + empty data still shows EmptyDataText, no table
7. `FormView_RenderOuterTableFalse_EmptyData_RendersEmptyDataTemplate` — false + EmptyDataTemplate renders custom template, no table
8. `FormView_RenderOuterTableFalse_EditMode_RendersEditTemplate` — false in edit mode renders EditItemTemplate, no table

All 8 tests pass against the already-landed implementation.

📌 Test pattern: When comparing two separate component renders for structural equivalence, do NOT use `ShouldBe` on raw Markup — Blazor generates unique internal event handler IDs per render. Compare element counts (`FindAll("table").Count`) or structural features instead. — Rogue

📌 FormView RenderOuterTable convention: When `RenderOuterTable=false`, the `else` branch in FormView.razor renders template content directly without `<table>`/`<tr>`/`<td>` wrappers. Empty data path also strips table wrappers. The implementation already exists on the current branch. — Rogue

 Team update (2026-03-02): Project reframed  final product is a migration acceleration system (tool/skill/agent), not just a component library. WingtipToys is proof-of-concept.  decided by Jeffrey T. Fritz

 Team update (2026-03-02): ModelErrorMessage component spec consolidated  29/29 WingtipToys coverage, BaseStyledComponent, EditContext pattern  decided by Forge

