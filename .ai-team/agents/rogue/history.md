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
