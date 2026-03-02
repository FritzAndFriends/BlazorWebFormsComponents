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

### M20 Skins & Themes Pipeline Tests (Issue #368)

Wrote 13 bUnit end-to-end tests in `src/BlazorWebFormsComponents.Test/Theming/ThemingPipelineTests.razor` validating the ThemeProvider → BaseWebFormsComponent → BaseStyledComponent pipeline using real components (Button, Label, Panel):

1. `DefaultSkin_AppliesBackColor_ToButton` — default skin applies BackColor via `#FFDEAD`
2. `NamedSkin_AppliesVia_SkinID` — SkinID="highlight" selects named skin with BackColor + Font.Bold
3. `ExplicitValue_OverridesTheme_StyleSheetThemeSemantics` — explicit BackColor="Red" overrides theme Blue
4. `EnableThemingFalse_IgnoresTheme` — EnableTheming=false skips all theme application
5. `NoThemeProvider_WorksNormally` — Button renders normally without ThemeProvider
6. `MissingSkinID_DoesNotThrow_FallsBackGracefully` — SkinID="nonexistent" silently skips
7. `NestedThemeProvider_InnerOverridesOuter` — inner ThemeProvider wins for its children
8. `Theme_AppliesForeColor_ToPanel` — ForeColor on Panel div
9. `Theme_AppliesCssClass_ToLabel` — CssClass on Label span
10. `Theme_AppliesWidthAndHeight_ToButton` — Width/Height on Button input
11. `Theme_AppliesFontProperties_ToLabel` — Bold, Italic, Underline on Label span
12. `MultipleControlTypes_ThemedSimultaneously` — Button+Label+Panel in same ThemeProvider
13. `ExplicitCssClass_OverridesThemeCssClass` — explicit CssClass beats theme CssClass

Total: 1426 tests (1413 existing + 13 new), 0 failures.

📌 Bug found: `ApplyThemeSkin` sets `Font.Name` but the style builder reads `Font.Names` for `font-family` rendering. Theme font-family does not render. Not blocking — deferred to M11 skin implementation. — Rogue

📌 Test pattern: Button renders as `<input>`, Label as `<span>`, Panel as `<div>`. For theme pipeline tests, use `cut.Find("input")`, `cut.Find("span")`, `cut.Find("div")` respectively. Style attribute contains CSS properties like `background-color:Red`, `color:Blue`, `font-weight:bold`. — Rogue

📌 Test pattern: Missing SkinID (named skin not registered) returns null from `GetSkin()` — no skin applied at all, not even the default skin for that control type. This is by design per Jeff's decision. — Rogue

### FontInfo Name/Names Auto-Sync Tests

Wrote 11 tests (9 unit + 2 pipeline) validating the FontInfo Name/Names auto-sync fix by Cyclops:

**FontInfoSyncTests.cs (9 unit tests):**
1. `SettingName_UpdatesNames` — Name="Arial" → Names="Arial"
2. `SettingNames_UpdatesName_ToFirstFont` — Names="Verdana" → Name="Verdana"
3. `SettingNames_WithMultipleFonts_SetsNameToFirst` — Names="Arial, sans-serif" → Name="Arial"
4. `SettingName_ToNull_ClearsNames` — Name=null → Names is null/empty
5. `SettingName_ToEmpty_ClearsNames` — Name="" → Names is null/empty
6. `SettingNames_ToNull_ClearsName` — Names=null → Name is null/empty
7. `SettingNames_ToEmpty_ClearsName` — Names="" → Name is null/empty
8. `SettingNames_ThenName_NameWins` — Last-write-wins: Name overrides Names for both properties
9. `SettingName_ThenNames_NamesWins` — Last-write-wins: Names overrides Name for both properties

**ThemingPipelineTests.razor (2 pipeline tests):**
14. `Theme_FontName_RendersFontFamily_ViaAutoSync` — Button with theme Font.Name="Arial" renders font-family:Arial
15. `Theme_FontName_MultipleViaNames_RendersFontFamily` — Label with theme Font.Name="Verdana" renders font-family:Verdana

All 1437 tests pass (0 failures). Cyclops's auto-sync fix was already in place — the previously-documented bug (ApplyThemeSkin sets Font.Name but style builder reads Font.Names) is now resolved.

📌 Bug resolved: The Font.Name/Font.Names disconnect is fixed. ApplyThemeSkin sets Font.Name → auto-sync propagates to Font.Names → style builder reads Font.Names → font-family renders correctly. Full pipeline verified. — Rogue

📌 Test pattern: FontInfo sync tests are pure C# unit tests (no bUnit needed). Use `new FontInfo()` then set properties and assert the counterpart. Last-write-wins semantics: setting Name then Names means Names wins, and vice versa. — Rogue

 Team update (2026-03-02): Unified release process implemented  single release.yml triggered by GitHub Release publication coordinates all artifacts (NuGet, Docker, docs, demos). version.json now uses 3-segment SemVer (0.17.0). Existing nuget.yml and deploy-server-side.yml are workflow_dispatch-only escape hatches. PR #408  decided by Forge (audit), Cyclops (implementation)

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
