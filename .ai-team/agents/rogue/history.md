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


<!-- Summarized 2026-03-03 by Scribe -- covers Issue #406 + #356 + FormView tests -->

### Issue #406, FormView & ListView CRUD Test Summary (2026-03-02 through 2026-03-03)

**ListView EditItemTemplate tests (Issue #406, 6 tests):** EditIndex matching/non-matching/negative, HandleCommand Edit/Cancel swap, null EditItemTemplate fallback. CSS class selectors (span.display vs span.edit) for template identification. TDD: 2 pass, 4 fail (confirming bug). HandleCommand tests use cut.InvokeAsync() for Blazor dispatcher context.

**FormView RenderOuterTable tests (8 tests):** Default/true/false rendering, template content, structural equivalence, empty data (text + template), edit mode. Compare element counts not raw Markup (Blazor generates unique event handler IDs per render). RenderOuterTable=false strips all table/tr/td wrappers.

**ListView CRUD Event tests (Issue #356, 43 tests):** All 16 events covered. Test categories: event firing + EventArgs data, cancellation (Cancel=true prevents downstream), event ordering (List<string> tracker), HandleCommand routing (case-insensitive, unknown->ItemCommand), modifier handlers, CancelMode detection, full CRUD lifecycle sequences, edge cases (empty items, zero paging).

Key test patterns: (1) CSS class selectors for template switching. (2) cut.InvokeAsync for HandleCommand. (3) FindAll().Count for structural comparison. (4) Event ordering via List<string> with index assertions. (5) ShouldBeGreaterThanOrEqualTo for bUnit double-render. (6) ItemCreated needs async + InvokeAsync for lifecycle callback.

Key file paths: `src/BlazorWebFormsComponents.Test/ListView/EditTemplateTests.razor`, `src/BlazorWebFormsComponents.Test/FormView/RenderOuterTable.razor`, `src/BlazorWebFormsComponents.Test/ListView/ListViewCrudEventTests.razor`.

Team updates (2026-03-02-03): Skins roadmap (Forge), M22 planned (Forge), project reframed as migration system (Jeff), ModelErrorMessage spec (Forge), themes last directive (Jeff Fritz), IOrderedDictionary on ListView EventArgs (Cyclops), WingtipToys 7-phase schedule (Forge).

 Team update (2026-03-04): PRs must target upstream FritzAndFriends/BlazorWebFormsComponents, not the fork  decided by Jeffrey T. Fritz

� Team update (2026-03-04): Run 6 improvement analysis  decided by Forge
