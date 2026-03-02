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

### Milestone 17: AJAX Control Tests

Wrote 47 bUnit tests across 6 new test files for the M17 AJAX/migration-helper controls:

**TimerTests.razor (9 tests):** Default Interval=60000, default Enabled=true, renders no visible output, custom Interval accepted, Enabled=false accepted, OnTick callback invoked after interval, Enabled=false prevents ticking, IDisposable cleanup, renders without errors. Uses `Render<Timer>(p => p.Add(...))` C# API because Timer's `new Enabled` shadows BaseWebFormsComponent.Enabled — fixed by removing duplicate `[Parameter]` from Timer.razor.cs.

**ScriptManagerTests.razor (9 tests):** Renders no visible output, defaults for all 7 properties verified (EnablePartialRendering=false, EnablePageMethods=false, ScriptMode=Auto, AsyncPostBackTimeout=90, EnableCdn=false, EnableScriptGlobalization=false, EnableScriptLocalization=false), setting all properties does not throw.

**ScriptManagerProxyTests.razor (4 tests):** Renders no visible output, renders without errors, default Scripts is empty list, default Services is empty list.

**UpdatePanelTests.razor (10 tests):** Default RenderMode=Block renders `<div>`, Inline mode renders `<span>`, ChildContent rendered, default UpdateMode=Always, Conditional mode accepted, default ChildrenAsTriggers=true, ChildrenAsTriggers=false accepted, ID attribute on div wrapper, ID attribute on span in Inline mode.

**UpdateProgressTests.razor (9 tests):** Renders `<div>` with ProgressTemplate content, DynamicLayout=true uses `display:none`, DynamicLayout=false uses `visibility:hidden`, default DynamicLayout=true, default DisplayAfter=500, custom DisplayAfter accepted, AssociatedUpdatePanelID accepted, default AssociatedUpdatePanelID is null, ProgressTemplate content rendered.

**SubstitutionTests.razor (6 tests):** Callback output rendered as raw markup, null callback renders nothing, MethodName property accepted, renders no wrapper element (raw text), HTML from callback rendered as markup, empty callback string renders empty.

📌 Test pattern: Timer has `new bool Enabled` shadowing BaseWebFormsComponent.Enabled. Both are `[Parameter]`, causing Blazor duplicate-parameter error. Fixed by removing duplicate from Timer.razor.cs and using inherited Enabled. Timer tests must use `Render<Timer>(p => p.Add(...))` C# API, not Razor templates. — Rogue

📌 Test pattern: ScriptManager and ScriptManagerProxy are no-op stubs that render no visible HTML. Test default property values and that rendering doesn't throw. Use `cut.Markup.Trim().ShouldBeEmpty()` to verify no output. — Rogue

📌 Test pattern: UpdateProgress DynamicLayout=true renders `display:none`, DynamicLayout=false renders `visibility:hidden`. Both use `<div>` wrapper. UpdatePanel Block mode renders `<div>`, Inline mode renders `<span>`. Both use `id="@ClientID"`. — Rogue

📌 Bug fix: Removed `new bool Enabled` `[Parameter]` from Timer.razor.cs — it shadowed BaseWebFormsComponent.Enabled causing a runtime InvalidOperationException (duplicate parameter name). Timer now uses the inherited Enabled property which has the same default (true). — Rogue

### M17 Audit Fix Tests

Added 9 new tests covering the 5 audit fixes from Forge's M17 audit:

**ScriptManagerTests.razor (6 new/updated tests):**
- Updated `ScriptManager_DefaultEnablePartialRendering_IsTrue` — changed from `ShouldBeFalse()` to `ShouldBeTrue()` (Fix 1: default now matches Web Forms)
- `ScriptManager_DefaultScripts_IsInitialized` — verifies Scripts collection is non-null and empty (Fix 2)
- `ScriptManager_Scripts_CanHoldScriptReferences` — passes List<ScriptReference> with 2 items, verifies Name/Path (Fix 2)
- `ScriptReference_DefaultScriptMode_IsAuto` — verifies ScriptMode defaults to Auto (Fix 5)
- `ScriptReference_DefaultNotifyScriptLoaded_IsTrue` — verifies NotifyScriptLoaded defaults to true (Fix 5)
- `ScriptReference_DefaultResourceUICultures_IsNull` — verifies ResourceUICultures defaults to null (Fix 5)

**UpdateProgressTests.razor (3 new/updated tests):**
- Updated `UpdateProgress_DynamicLayoutFalse_UsesDisplayBlockVisibilityHidden` — renamed from `UsesVisibilityHidden`, now asserts both `display:block` and `visibility:hidden` (Fix 4)
- `UpdateProgress_CssClass_AppliedToDiv` — sets CssClass="progress-overlay", verifies class attribute on div (Fix 3)
- `UpdateProgress_NoCssClass_NoClassAttribute` — no CssClass set, verifies class attribute is null (Fix 3)

All 29 ScriptManager/UpdateProgress/ScriptReference tests pass (0 failures). Build has 60 pre-existing warnings (none from new tests).

📌 Test pattern: ScriptReference properties tested as plain C# object instantiation (no bUnit render needed) — `new ScriptReference()` then assert defaults. Same pattern as ScriptManagerProxy Scripts/Services tests. — Rogue

📌 Test pattern: UpdateProgress CssClass uses `class="@(string.IsNullOrEmpty(CssClass) ? null : CssClass)"` — when CssClass is empty/null, AngleSharp returns null for `GetAttribute("class")`, matching Web Forms behavior of omitting the class attribute entirely. — Rogue

 Team update (2026-02-27): M17 audit fixes resolved  5 fidelity issues fixed (EnablePartialRendering default, Scripts collection, CssClass rendering, display:block style, ScriptReference properties). 9 new tests, 1367 total. PR #402  decided by Forge, Cyclops

### LinkButton CssClass Tests (Issue #379)

Wrote 8 bUnit tests in `src/BlazorWebFormsComponents.Test/LinkButton/CssClass.razor` verifying CssClass pass-through on the `<a>` element:

1. `SingleClass_RendersAsClassAttributeOnAnchor` — single class renders correctly
2. `MultipleClasses_RendersSpaceSeparatedOnAnchor` — space-separated classes preserved
3. `NoCssClass_OmitsClassAttribute` — no CssClass param omits class attribute entirely
4. `EmptyCssClass_OmitsClassAttribute` — empty string CssClass also omits attribute
5. `CssClass_WithIdAttribute_BothRenderOnAnchor` — CssClass + ID coexist
6. `CssClass_WithPostBackUrl_RendersOnAnchor` — PostBackUrl branch also renders class
7. `Disabled_NoCssClass_RendersAspNetDisabledOnly` — disabled state adds aspNetDisabled
8. `Disabled_WithCssClass_RendersBothClasses` — disabled + CssClass combines both

📌 Test pattern: LinkButton has two render branches (PostBackUrl null vs non-null). Both share `GetCssClassOrNull()` which returns null for empty/null CssClass (AngleSharp `HasAttribute("class")` returns false) and appends "aspNetDisabled" when Enabled=false. Must test both branches for CssClass. — Rogue

📌 Edge case: `GetCssClassOrNull()` uses `string.IsNullOrEmpty()` not `IsNullOrWhiteSpace()` — whitespace-only CssClass like " " would render `class=" "`. Not a blocker for #379 but noted for future audit. — Rogue

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
