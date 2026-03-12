# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents  Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!--  Summarized 2026-02-27 by Scribe  covers M1M16 -->

<!-- ⚠ Summarized 2026-03-06 by Scribe — older entries archived -->

### Archived Sessions

- Core Context (2026-02-10 through 2026-02-27)
- M17-M20 Wave 1 Context (2026-02-27 through 2026-03-01)
- M20 Theming through Migration Benchmarks (2026-03-01 through 2026-03-04)
- Script & Toolkit Summary (2026-03-02 through 2026-03-04)
- GetRouteUrl, Run 5 & Toolkit Sync Summary (2026-03-04 through 2026-03-05)

<!-- ⚠ Summarized 2026-03-07 by Scribe — entries from 2026-03-05 through 2026-03-07 (pre-Run 11) archived -->

- Run 6 Script Enhancements (2026-03-05)
- @rendermode Scaffold Fix (2026-03-05)
- WebFormsPageBase Implementation (2026-03-05)
- WebFormsPage IPageService Consolidation (2026-03-05)
- LoginView Migration Script Fix (2026-03-06)
- Run 9 Script Fixes — 9 RF items (2026-03-06)
- Layer 2 AfterWingtipToys Build Conversion (2026-03-06)

### Summary (2026-03-05 through 2026-03-07 pre-Run 11)

Run 6: 4 script enhancements (TFM, SelectMethod TODO, wwwroot copy, stubs). @rendermode fix: removed standalone directive from _Imports.razor scaffold — `@rendermode` is a directive *attribute* for component instances only. WebFormsPageBase: `ComponentBase` subclass with `Page => this`, Title/MetaDescription/MetaKeywords delegates, `IsPostBack => false`. WebFormsPage consolidation: merged Page.razor head rendering into WebFormsPage via Option B. LoginView script fix: `<asp:LoginView>` → `<LoginView>` (not AuthorizeView), preserve template names. Run 9: 9 script fixes (Models copy, DbContext transform, EF6→EF Core, redirect detection, Program.cs boilerplate, Page Title extraction, QueryString/RouteData annotations, ListView GroupItemCount, csproj packages). Layer 2: full AfterWingtipToys conversion — key pattern: layout code-behind class name MUST match .razor filename. Auth pages use plain HTML forms with HTTP endpoints.

<!-- summarized 2026-03-11 by Scribe -- entries from 2026-03-07 through 2026-03-08 (Runs 11-13, script fixes) archived -->

- Run 11 -- Complete WingtipToys Migration from Scratch (2026-03-07)
- Run 11 Script Fixes -- Fix 1 (Invoke-ScriptAutoDetection) & Fix 2 (Convert-TemplatePlaceholders) (2026-03-07)
- Run 12 -- Complete WingtipToys Migration from Scratch (2026-03-07)
- LoginView Namespace Fix (2026-03-07)
- Run 13 -- Full WingtipToys Migration Pipeline, 25/25 tests (2026-03-08)

### Summary (2026-03-07 through 2026-03-08)

Run 11: Fresh WingtipToys migration from scratch (105 files, 0 errors). Key patterns: root-level `_Imports.razor` for pages outside `Components/`, partial classes must NOT specify `: ComponentBase` with `@inherits WebFormsPageBase`, auth pages use plain HTML forms to HTTP endpoints. Run 11 script fixes: `Invoke-ScriptAutoDetection` (JS files to wwwroot/Scripts/ with correct dependency order) and `Convert-TemplatePlaceholders` (placeholder elements to `@context`). Run 12: Full pipeline with Layer 2, established dual DbContext pattern (later superseded by factory-only in Run 13). LoginView namespace fix: `@using BlazorWebFormsComponents.LoginControls` required in `_Imports.razor` -- added to script template. Run 13: 25/25 tests passed (100%). Confirmed patterns: SSR default, `data-enhance-nav="false"` for minimal API links, `data-enhance="false"` for auth forms, `AddDbContextFactory` only (no dual registration), middleware order `UseAuthentication -> UseAuthorization -> UseAntiforgery`, logout must use `<a>` not `<button>`.

Team updates (2026-03-07 through 2026-03-08): Coordinator must not perform domain work; FreshWingtipToys must not be committed; migration-standards updated with Static Asset Checklist/ListView Placeholder/Action Links; migration order: fresh project first; SSR default with InteractiveServer opt-in; enhanced nav bypass for minimal API; DbContext factory-only; middleware order confirmed; logout uses link not button.

<!-- ⚠ Summarized 2026-03-11 by Scribe — entries from 2026-03-11 (ServiceCollectionExtensions through L2 structural transform) archived -->

- ServiceCollectionExtensions Enhancement (2026-03-11)
- Run 18a/18b/18c — Test-UnconvertiblePage False-Positive Investigation (2026-03-11)
- P0 Migration Script Fixes (2026-03-11)
- Standardize Generic Type Params to ItemType (2026-03-11)
- Layer 2 Structural Transform - AfterWingtipToys (2026-03-11)

### Summary (2026-03-11 pre-Run 20)

ServiceCollectionExtensions: Added `AddHttpContextAccessor()` auto-registration, `BlazorWebFormsComponentsOptions` with `EnableAspxUrlRewriting`, `UseBlazorWebFormsComponents()` middleware, `AspxRewriteMiddleware` for `.aspx` → clean URL 301 redirects. Run 18: Fixed `Test-UnconvertiblePage` false positives — 'Checkout' matched button IDs, 'PayPal' matched image URLs. Both now path-based. ShoppingCart.razor restored to full GridView markup. P0 fixes: (1) Eliminated `Test-UnconvertiblePage` — always returns `$false`, pages get TODO annotations instead of stubs. (2) `[Parameter]` regex fix — TODO on separate line, eliminates 6 CS errors. ItemType standardization: renamed all `TItemType`/`TItem` → `ItemType` across 13 files, 0 build errors. L2 AfterWingtipToys: ~60 files transformed, 0 errors, 58 RZ10012 warnings. Key gotchas: class name must match .razor filename, `@inject`/`[Inject]` duplication, `#hexcolor` → `@("...")`, `LoggedInTemplate` has no typed context.

Team updates (2026-03-11): Migration tests reorganized to `project/runNN/`. Mandatory L1→L2 pipeline with no fixes between layers. All generics standardized to `ItemType`. Test-UnconvertiblePage eliminated. Run 18 improvements prioritized by Forge.

<!-- ⚠ Summarized 2026-03-12 by Scribe — Run 20 fixes, Run 21, CU L2, L1 SQLite fix archived -->

- Run 20 L1 Script Fixes — SelectMethod Preservation + Review Item Noise Reduction (2026-03-12)
- Run 21 — Layer 2 Structural Transform AfterWingtipToys (2026-03-11)
- Layer 2 Structural Transform — AfterContosoUniversity (2026-03-12)
- L1 Script SQLite → SQL Server Fix (2026-03-12)

### Summary (2026-03-11 through 2026-03-12)

Run 20 L1 fixes: `ConvertFrom-SelectMethod` was stripping SelectMethod from markup — fixed to preserve it with TODO for L2 delegate conversion. ContentPlaceHolder and GetRouteUrl review items updated to reference BWFC components. Run 21 WingtipToys L2: 44 files, 0 errors. SelectMethod delegate conversion on 4 data pages (`SelectMethod="@GetProducts"` etc.), all code-behinds to async lifecycle, enum/type Razor escaping (`@GridLines.Vertical`), account pages stubbed for Identity migration. CU L2: 0 errors, 5 models to EF Core, 5 BLL classes with IDbContextFactory, EDMX artifacts deleted, raw SQL → LINQ. L1 script: SQLite package/example replaced with SqlServer/LocalDB.

**Key gotchas (combined):** Code-behind class name MUST match .razor filename. Code-behind files need explicit usings (don't inherit _Imports.razor). Don't duplicate `@inject`/`[Inject]`. `#hexcolor` → `@("#hexcolor")`. `Unit` params are bare integers. BWFC uses `Sorting` EventCallback (not `OnSorting`). `CommandField` doesn't exist — use `ButtonField`. `TextBox Text` expects `string`. `TemplateField` in DetailsView needs `ItemType`. Style sub-components: `<RowStyleContent><GridViewRowStyle .../></RowStyleContent>`. `SortedAscendingCellStyle` not in BWFC. L1 must not strip attributes BWFC supports.


 Team update (2026-03-11): NEVER default to SQLite; SelectMethod MUST be preserved as SelectHandler<ItemType> delegates. All skill files hardened, L1 script now scaffolds SQL Server.  decided by Jeffrey T. Fritz, Beast, Cyclops


 Team update (2026-03-11): ContosoUniversity L2 confirmed SQL Server LocalDB exclusively. Earlier SQLite attempt was rejected. IDbContextFactory pattern with SqlServer provider is canonical.  decided by Cyclops


 Team update (2026-03-11): Database provider guidance reframed  lead with 'detect and match original provider' instead of 'NEVER substitute'. L1 auto-detection connects to L2 verification.  decided by Beast (requested by Jeffrey T. Fritz)

### L1 Script — Web.config Database Provider Auto-Detection (2026-03-12)

Added `Find-DatabaseProvider` function to `bwfc-migrate.ps1` that parses Web.config `<connectionStrings>` to detect the actual database provider. Three-pass detection: (1) explicit `providerName` attribute, (2) connection string content patterns like `(LocalDB)` or `Server=`, (3) EntityClient inner `provider=` extraction for EF6 EDMX connections. Returns matching EF Core package name and provider method. Falls back to SQL Server when no Web.config or no connectionStrings found.

**Changes:**
- `Find-DatabaseProvider` function added between Logging and Project Scaffolding regions
- Uses `GetAttribute()` for XML attribute access (StrictMode-safe — `$entry.providerName` throws under `Set-StrictMode -Version Latest`)
- Package reference in csproj scaffold now uses detected package instead of hardcoded SqlServer
- Program.cs scaffold includes detected connection string in commented-out `AddDbContextFactory` line (both identity and models-only paths)
- `[DatabaseProvider]` review item added to migration summary when provider detected from Web.config
- Provider mapping: SqlClient→SqlServer, SQLite→Sqlite, Npgsql→PostgreSQL, MySqlClient→MySQL

**Key learning:** PowerShell `Set-StrictMode -Version Latest` throws on missing XML element properties. Use `$element.GetAttribute('attrName')` (returns '' if missing) instead of `$element.attrName` for optional XML attributes.


 Team update (2026-03-12): Database provider auto-detection consolidated  Jeff directive + Beast skill reframe + Cyclops Find-DatabaseProvider implementation merged into single decision. L1 script now auto-detects provider from Web.config.  decided by Jeffrey T. Fritz, Beast, Cyclops

### Fix: TItem → ItemType in Tests and Samples (2026-03-12)

The `ItemType` standardization (renaming `TItem`/`TItemType` → `ItemType` across 13 component files) was not applied to test files or sample pages. This caused CI failures on PR #425 with `RZ10001` (type cannot be inferred) and `CS0411` (type arguments cannot be inferred) errors for `RadioButtonList` and `BulletedList` — but the problem was actually much wider.

**Root cause:** Components declare `@typeparam ItemType` but tests and samples still referenced `TItem=`. The Razor compiler couldn't match the generic parameter name.

**Fix:** Renamed `TItem=` → `ItemType=` across 43 files:
- 36 test files: RadioButtonList (7), BulletedList (7), CheckBoxList (6), DropDownList (7), ListBox (8), ToolTipTests (1)
- 7 sample files: ControlSamples pages for all 5 list controls, plus AfterWingtipToys account pages

**Key learning:** When standardizing generic type parameter names on components, the rename must also cover all test files, sample pages, and documentation code blocks — not just the component source. CI may only surface the first few errors, hiding the full scope.

### L2 Automation Shims — 4 S-sized Library Enhancements (2026-07-25)

Implemented 4 OPPs from Forge's L2 automation analysis to eliminate recurring manual L2 fixes:

**OPP-2 (Unit implicit string conversion):** Replaced `explicit operator Unit(string)` (which only handled bare integers) with `implicit operator Unit(string s) => Unit.Parse(s)`. Now `Width="125px"` just works in Razor markup — no `@(Unit.Parse(...))` wrapper needed. `Unit.Parse` already handled all CSS unit formats (px, em, %, pt, etc.).

**OPP-3 (ResponseShim):** Created `ResponseShim.cs` wrapping `NavigationManager`. Strips `~/` prefix and `.aspx` extension from URLs. Exposed as `protected ResponseShim Response` on `WebFormsPageBase`. Now `Response.Redirect("~/Products.aspx")` compiles and navigates correctly.

**OPP-5 (ViewState on WebFormsPageBase):** Added `Dictionary<string, object> ViewState` with `[Obsolete]` warning. Page code-behind using `ViewState["key"]` compiles unchanged. `BaseWebFormsComponent` already had this (line ~145); now page base does too.

**OPP-6 (GetRouteUrl on WebFormsPageBase):** Added `GetRouteUrl(string routeName, object routeParameters)` using injected `LinkGenerator` + `IHttpContextAccessor` — same pattern as `GetRouteUrlHelper` extension on `BaseWebFormsComponent`. Strips `.aspx` from route names.

**Key learnings:**
- `Unit.Parse()` already handles all CSS unit formats via the `Unit(string, CultureInfo, UnitType)` constructor — no new parsing needed.
- `WebFormsPageBase` did NOT have `NavigationManager`, `LinkGenerator`, or `IHttpContextAccessor` injections prior to this change. Added all three.
- The explicit string-to-Unit operator was effectively dead code — no tests or consuming code used the `(Unit)"string"` cast syntax.


 Team update (2026-03-11): L2 automation shims (OPP-2, 3, 5, 6) implemented by Cyclops on WebFormsPageBase  Unit implicit string, Response.Redirect shim, ViewState, GetRouteUrl. OPP-1/OPP-4 deferred.  decided by Forge (analysis), Cyclops (implementation)

### OPP-1: EnumParameter<T> Wrapper Struct (2026-07-25)

Implemented `EnumParameter<T>` — a `readonly struct` enabling Blazor component enum parameters to accept both enum values and bare string values. This is the #1 L2 fix by volume: every migrated enum attribute like `GridLines="None"` previously required `@(GridLines.None)` Razor expression syntax.

**New file:** `src/BlazorWebFormsComponents/Enums/EnumParameter.cs`
- Implicit conversions: `T → EnumParameter<T>`, `string → EnumParameter<T>` (case-insensitive parse), `EnumParameter<T> → T`
- Equality operators for `EnumParameter<T>` vs `T` and `T` vs `EnumParameter<T>`
- Implements `IEquatable<EnumParameter<T>>` and `IEquatable<T>`

**55 files changed** across 46 components/interfaces/style classes:
- BaseStyledComponent, BaseWebFormsComponent, BulletedList, Calendar, Chart, ChartSeries, CheckBox, CheckBoxList, DataPager, DetailsView, FormView, GridView, Image, ImageButton, ImageMap, ListBox, ListView, Literal, Login, ChangePassword, Menu, MenuItemStyle, MenuLevelStyle, NamingContainer, Panel, RadioButton, RadioButtonList, ScriptManager, SiteMapPath, Table, TableCell, TableFooterRow, TableHeaderCell, TableHeaderRow, TableRow, TextBox, TreeNode, TreeView, UiPagerSettings, UiStyle, UiTableItemStyle, UpdatePanel, BaseValidator, Style, TableItemStyle
- Interfaces: IImageComponent, IHasLayoutStyle, IHasLayoutTableItemStyle

**Skipped (abstract class hierarchies, not enums):** DataListEnum, RepeatLayout, ButtonType, TreeViewImageSet, ValidationSummaryDisplayMode

**Skipped (nullable):** `Docking?` on ChartLegend/ChartTitle — wrapping nullable enum params in `EnumParameter<T>?` requires separate handling.

**Key learnings / gotchas:**
1. **Switch expressions break.** C# pattern matching does NOT use user-defined implicit conversions. Every `switch (Property)` or `Property switch { EnumVal => ... }` must become `Property.Value switch { ... }`. This was the biggest source of internal code changes (~15 switch expressions updated).
2. **Shouldly `.ShouldBe()` breaks.** Extension methods like `ShouldBe` can't resolve through implicit conversions on structs. Tests need `property.Value.ShouldBe(EnumVal)`. Affected: ListView/SortingEvents, ScriptManager/ScriptManagerTests, UpdatePanel/UpdatePanelTests, Localize/InheritsLiteral.
3. **"Color Color" rule still works.** When property name matches enum type name (e.g., `GridLines` property of type `EnumParameter<GridLines>`), C# still resolves `GridLines.None` in case labels to the enum type via the "Color Color" disambiguation rule.
4. **Default values work unchanged.** `= GridLines.None` compiles because the implicit `T → EnumParameter<T>` conversion handles the assignment.
5. **`ToString()` is transparent.** The struct's `ToString()` delegates to `Value.ToString()`, so existing `property.ToString().ToLowerInvariant()` patterns work unchanged.
6. **Equality comparisons are safe.** The `==` and `!=` operators between `EnumParameter<T>` and `T` handle `if (Property == EnumVal)` without needing `.Value`.

**Test files needing updates (for Rogue):**
- `ListView/SortingEvents.razor` — `SortDirection.ShouldBe()` → `.Value.ShouldBe()`
- `ScriptManager/ScriptManagerTests.razor` — `ScriptMode.ShouldBe()` → `.Value.ShouldBe()`
- `UpdatePanel/UpdatePanelTests.razor` — `RenderMode/UpdateMode.ShouldBe()` → `.Value.ShouldBe()`
- `Localize/InheritsLiteral.razor` — overload resolution failure on `ShouldBe`



 Team update (2026-03-12): ResponseShim.Redirect null URL bug reported by Rogue — throws NullReferenceException instead of ArgumentNullException. Add null guard.  reported by Rogue (QA)
