<!-- Archived 2026-03-06 by Scribe -->

### Core Context (2026-02-10 through 2026-02-27)

**M1M3 components:** Calendar (enum fix, async events), ImageMap (BaseStyledComponent, Guid IDs), FileUpload (InputFile integration, path sanitization), PasswordRecovery (3-step wizard), DetailsView (DataBoundComponent<T>, auto-field reflection, 10 events), Chart (BaseStyledComponent, CascadingValue "ParentChart", JS interop via ChartJsInterop, ChartConfigBuilder pure static).

**M6 base class fixes:** DataBoundComponent chain  BaseStyledComponent (14 data controls). BaseListControl<TItem> for 5 list controls (DataTextFormatString, AppendDataBoundItems). CausesValidation on CheckBox/RadioButton/TextBox. Label AssociatedControlID switches spanlabel. Login/ChangePassword/CreateUserWizard  BaseStyledComponent. Validator ControlToValidate dual-path: ForwardRef + string ID via reflection.

**M6 Menu overhaul:**  BaseStyledComponent. Selection tracking (SelectedItem/SelectedValue, MenuItemClick, MenuItemDataBound). MenuEventArgs, MaximumDynamicDisplayLevels, Orientation enum + CSS horizontal class. MenuLevelStyle lists. StaticMenuStyle sub-component + IMenuStyleContainer interface. RenderFragment parameters for all menu styles. RenderingMode=Table added (M14) with inline Razor for AngleSharp compatibility.

**M7 style sub-components:** GridView (8), DetailsView (10), FormView (7), DataGrid (7)  all CascadingParameter + UiTableItemStyle. Style priority: Edit > Selected > Alternating > Row. TreeView: TreeNodeStyle + 6 sub-components, selection, ExpandAll/CollapseAll, FindNode, ExpandDepth, NodeIndent. GridView: selection, 10 display props. FormView/DetailsView events + PagerTemplate + Caption. DataGrid paging/sorting. ListView 10 CRUD events + EditItemTemplate/InsertItemTemplate. Panel BackImageUrl. Login Orientation + TextLayout. Shared PagerSettings (12 props, IPagerSettingsContainer) for GridView/FormView/DetailsView.

**M8 bug fixes:** Menu JS null guard + Calendar conditional scope + Menu auto-ID (`menu_{GetHashCode():x}`).

**M9 migration-fidelity:** ToolTip  BaseStyledComponent (removed from 8, added title="@ToolTip" to 32 components). ValidationSummary comma-split fix (IndexOf + Substring). SkinID boolstring. TreeView NodeImage fallback restructured (ShowExpandCollapse + ExpandCollapseImage helper).

**M10 Theming:** ControlSkin (nullable props, StyleSheetTheme semantics). ThemeConfiguration (case-insensitive keys, empty-string default skin, GetSkin returns null). ThemeProvider as CascadingValue wrapper. SkinID="" default, EnableTheming=true, [Obsolete] removed. CascadingParameter in BaseStyledComponent, ApplySkin in OnParametersSet. LoginView/PasswordRecovery  BaseStyledComponent.

**M15 HTML fidelity fixes:** Button `<input>` rendering. BulletedList `<span>` removal + `<ol>` CSS-only (no HTML type attr, GetStartAttribute returns int?). LinkButton class + aspNetDisabled. Image longdesc conditional. Calendar structural (tbody, width:14%, day titles, abbr headers, align center, border-collapse, navigation sub-table). FileUpload clean ID. CheckBox span verified. GridView UseAccessibleHeader default falsetrue. 27 test files updated for Button `<input>`. 10 new tests. All 1283 pass.

**M16:** LoginView wrapper `<div>` for styles (#352). ClientIDMode enum (Inherit/AutoID/Static/Predictable) on BaseWebFormsComponent. ComponentIdGenerator refactored: GetEffectiveClientIDMode(), BuildAutoID(), BuildPredictableID(). UseCtl00Prefix only in AutoID mode. NamingContainer auto-sets AutoID when UseCtl00Prefix=true.

**Key patterns:** Orientation enum collides with parameter name  use `Enums.Orientation.Vertical`. `_ = callback.InvokeAsync()` for render-time events. `Path.GetFileName()` for file save security. CI secret-gating: env var indirection. Null-returning helpers for conditional HTML attributes. aspNetDisabled class for disabled controls. Always test default parameter values explicitly.


<!--  Summarized 2026-03-01 by Scribe  covers M17-M20 Wave 1 -->

### M17-M20 Wave 1 Context (2026-02-27 through 2026-03-01)

**M17 AJAX audit fixes:** ScriptManager EnablePartialRendering default false>true. Scripts collection added (List<ScriptReference>). UpdateProgress conditional CssClass + display:block;visibility:hidden for non-dynamic mode. ScriptReference gained ScriptMode/NotifyScriptLoaded/ResourceUICultures. Lesson: C# bool defaults false, but Web Forms often defaults true.

**M18 bug verification:** #380 BulletedList, #382 CheckBox span, #383 FileUpload GUID  all verified already fixed in M15. FileUpload blazor:elementReference is inherent InputFile artifact. Lesson: verify current state before assuming bugs still exist.

**M18 deterministic IDs & Menu fonts:** CheckBox bare input gained id (#386). MenuItemStyle needed SetFontsFromAttributes(OtherAttributes) in OnInitialized (#360)  Font-Bold maps to Font.Bold sub-property. Lesson: CaptureUnmatchedValues + Font- attrs need explicit SetFontsFromAttributes handling.

**Issue #379:** LinkButton CssClass verified already correct from M15. GetCssClassOrNull() returns null for empty, appends aspNetDisabled when disabled. Edge case: IsNullOrEmpty not IsNullOrWhiteSpace.

**Issue #387 normalizer:** 4 enhancements  case-insensitive file pairing, boolean attr collapse (6 attrs), empty style stripping, GUID ID placeholders. Pipeline order: regex > style norm > empty style strip > boolean attrs > GUID IDs > attr sort > artifact cleanup > whitespace. Key files: scripts/normalize-html.mjs, scripts/normalize-rules.json.

**Issues #364/#365 theming:** SkinBuilder uses expression trees for Set<TValue>()  supports direct (s.BackColor) and nested (s.Font.Bold) via recursive GetOrCreateValue. ThemeConfiguration gained ForControl(name, configure) fluent methods. ThemeProvider cascades via unnamed CascadingValue  nesting naturally overrides. WebColor.FromHtml() added. 14 unit + 4 bUnit tests. Lesson: expression trees require recursive auto-init of intermediate nulls. CascadingValue by type sufficient for unique types.

Team update (2026-02-27): Branching workflow  feature PRs from personal fork to upstream dev, only dev>main on upstream  decided by Jeffrey T. Fritz
Team update (2026-02-27): Issues must be closed via PR references using 'Closes #N'  decided by Jeffrey T. Fritz
Team update (2026-02-27): AJAX Controls nav category; migration stub doc pattern for no-ops  decided by Beast
Team update (2026-02-27): M17 sample pages created  decided by Jubilee
Team update (2026-02-27): Forge approved M17 with 4 non-blocking follow-ups  decided by Forge
Team update (2026-02-27): Timer duplicate [Parameter] bug fixed; 47 M17 tests  decided by Rogue
Team update (2026-02-27): No-op stub property coverage 41-50% acceptable  decided by Forge
Team update (2026-02-27): UpdatePanel Triggers deliberately omitted  decided by Forge
Team update (2026-02-28): GetCssClassOrNull() uses IsNullOrEmpty not IsNullOrWhiteSpace  low priority  noted by Rogue
 Team update (2026-03-01): Skins & Themes has dual docs  SkinsAndThemes.md (practical guide, update first) and ThemesAndSkins.md (architecture). Update SkinsAndThemes.md first for API changes  decided by Beast


<!-- Summarized 2026-03-04 by Scribe  covers M20 theming through Layer 2+3 benchmark -->

### M20 Theming through Migration Benchmarks (2026-03-01 through 2026-03-04)

**Theme wiring (#366):** CascadingParameter ThemeConfiguration on BaseWebFormsComponent (named CascadedTheme). ApplySkin>ApplyThemeSkin virtual chain. _Imports.razor @inherits affects ALL .razor files. FontInfo auto-sync (Name/Names bidirectional). WebFormsPage cascades Theme ?? CascadedTheme.

**Unified release.yml:** Single workflow on release:published. Version from tag_name. NuGet -p:PackageVersion + -p:Version. version.json 3-segment SemVer. Old workflows to workflow_dispatch-only.

**ListView fixes:** #406 EditItemTemplate  closure bug + @key diffing fix. #356 CRUD events  ItemCreated changed to fire per-item with ListViewItemEventArgs, ItemCommand fires for ALL commands before specific handlers. Web Forms event order: ItemCommand then specific event. EventArgs gained IOrderedDictionary properties (Keys, Values, OldValues, NewValues).

**FormView RenderOuterTable:** [Parameter] bool, suppresses table wrapper when false.

**WingtipToys CSS fixes:** 7 visual fixes (Cerulean theme, 4-column grid, BoundField currency, Trucks category, Site.css, bootstrap-theme gradients). Lessons: Playwright blocks file://, use HTTP; Get-NetTCPConnection for detached process PID cleanup.

**Layer 1 Benchmark:** bwfc-scan.ps1  0.9s, 32 files, 230 controls, 100% coverage. bwfc-migrate.ps1  2.4s, 276 transforms, 33 .razor + 32 .razor.cs, 18 manual items. 338 build errors (all code-behind). Scaffold fix needed: net8.0 to net10.0, PackageReference to ProjectReference for local dev.

**Layer 2+3 Benchmark:** 563s (~9.4 min) total. Clean build after 3 rounds. Account pages copied from reference. Key transforms: SelectMethod to Items, Page_Load to OnInitializedAsync, Session to scoped services, EF6 to EF Core. ~9 min with Copilot vs 4-8 hours manual.

<!-- Summarized 2026-03-04 by Scribe ΓÇö covers team updates and script enhancements 2026-03-02 through 2026-03-04 -->

### Script & Toolkit Summary (2026-03-02 through 2026-03-04)

**Team context:** PRs target upstream (not fork). Migration toolkit restructured into self-contained migration-toolkit/ package. Migration Run 2 validated 11/11 features (PR #418 critical). Project reframed as migration system. M22 planned. ListView CRUD first, Themes last.

**Script enhancements (bwfc-migrate.ps1):** ConvertFrom-MasterPage (6 transforms: @inherits injection, document wrapper strip, ContentPlaceHolderΓåÆ@Body, ScriptManager removal, HeadContent extraction, layout path remap). New-AppRazorScaffold (App.razor + Routes.razor). Eval format-string regex (`Eval("prop","{0:fmt}")` ΓåÆ `@context.prop.ToString("fmt")`). String.Format regex (`String.Format("{0:fmt}",Item.Prop)` ΓåÆ `@($"{context.Prop:fmt}")`). Regex ordering: specific patterns before general. ScriptManager uses `(?s)`, ContentPlaceHolder uses `(?si)`.

<!-- Summarized 2026-03-05 by Scribe -- covers GetRouteUrl through migration-toolkit sync -->

### GetRouteUrl, Run 5 & Toolkit Sync Summary (2026-03-04 through 2026-03-05)

**GetRouteUrl:** 4 extension method overloads on BaseWebFormsComponent (2 object, 2 RouteValueDictionary), all delegate to LinkGenerator.GetPathByRouteValues. WingtipToys uses anonymous-object overloads only.

**Run 5 migration:** 3.25s, 309 transforms, 32 files. 6 new enhancements confirmed (LoginView, GetRouteUrl hints, SelectMethod TODO, Register cleanup, ContentPlaceHolder, String.Format). Clean build after stubbing Account/Checkout. Gaps: static assets need wwwroot/ copy, csproj TFM still net8.0.

**Toolkit sync:** migration-toolkit/ is canonical home. Synced 47KB bwfc-migrate.ps1 over stale 29KB copy. PageService.Title already exists -- updated analysis and skill. .NET SDK prereq updated to 10.0+. Lesson: always check existing BWFC components before flagging as missing.

Team updates: Migration report 3-level traversal (Beast). Run 5 reports need Works/Doesn't-Work sections (Beast). Migration standards formalized -- EF Core, .NET 10, ASP.NET Core Identity, BWFC data controls preferred, migration-toolkit/ canonical (Jeff/Forge).

<!-- Summarized 2026-03-04 by Scribe -- covers Run 6 script enhancements -->


## Archived 2026-03-12 (entries from 2026-03-12 through 2025-07-25)

### L1 Script ΓÇö Web.config Database Provider Auto-Detection (2026-03-12)

Added `Find-DatabaseProvider` function to `bwfc-migrate.ps1` that parses Web.config `<connectionStrings>` to detect the actual database provider. Three-pass detection: (1) explicit `providerName` attribute, (2) connection string content patterns like `(LocalDB)` or `Server=`, (3) EntityClient inner `provider=` extraction for EF6 EDMX connections. Returns matching EF Core package name and provider method. Falls back to SQL Server when no Web.config or no connectionStrings found.

**Changes:**
- `Find-DatabaseProvider` function added between Logging and Project Scaffolding regions
- Uses `GetAttribute()` for XML attribute access (StrictMode-safe ΓÇö `$entry.providerName` throws under `Set-StrictMode -Version Latest`)
- Package reference in csproj scaffold now uses detected package instead of hardcoded SqlServer
- Program.cs scaffold includes detected connection string in commented-out `AddDbContextFactory` line (both identity and models-only paths)
- `[DatabaseProvider]` review item added to migration summary when provider detected from Web.config
- Provider mapping: SqlClientΓåÆSqlServer, SQLiteΓåÆSqlite, NpgsqlΓåÆPostgreSQL, MySqlClientΓåÆMySQL

**Key learning:** PowerShell `Set-StrictMode -Version Latest` throws on missing XML element properties. Use `$element.GetAttribute('attrName')` (returns '' if missing) instead of `$element.attrName` for optional XML attributes.


 Team update (2026-03-12): Database provider auto-detection consolidated  Jeff directive + Beast skill reframe + Cyclops Find-DatabaseProvider implementation merged into single decision. L1 script now auto-detects provider from Web.config.  decided by Jeffrey T. Fritz, Beast, Cyclops

### Fix: TItem ΓåÆ ItemType in Tests and Samples (2026-03-12)

The `ItemType` standardization (renaming `TItem`/`TItemType` ΓåÆ `ItemType` across 13 component files) was not applied to test files or sample pages. This caused CI failures on PR #425 with `RZ10001` (type cannot be inferred) and `CS0411` (type arguments cannot be inferred) errors for `RadioButtonList` and `BulletedList` ΓÇö but the problem was actually much wider.

**Root cause:** Components declare `@typeparam ItemType` but tests and samples still referenced `TItem=`. The Razor compiler couldn't match the generic parameter name.

**Fix:** Renamed `TItem=` ΓåÆ `ItemType=` across 43 files:
- 36 test files: RadioButtonList (7), BulletedList (7), CheckBoxList (6), DropDownList (7), ListBox (8), ToolTipTests (1)
- 7 sample files: ControlSamples pages for all 5 list controls, plus AfterWingtipToys account pages

**Key learning:** When standardizing generic type parameter names on components, the rename must also cover all test files, sample pages, and documentation code blocks ΓÇö not just the component source. CI may only surface the first few errors, hiding the full scope.

### L2 Automation Shims ΓÇö 4 S-sized Library Enhancements (2026-07-25)

Implemented 4 OPPs from Forge's L2 automation analysis to eliminate recurring manual L2 fixes:

**OPP-2 (Unit implicit string conversion):** Replaced `explicit operator Unit(string)` (which only handled bare integers) with `implicit operator Unit(string s) => Unit.Parse(s)`. Now `Width="125px"` just works in Razor markup ΓÇö no `@(Unit.Parse(...))` wrapper needed. `Unit.Parse` already handled all CSS unit formats (px, em, %, pt, etc.).

**OPP-3 (ResponseShim):** Created `ResponseShim.cs` wrapping `NavigationManager`. Strips `~/` prefix and `.aspx` extension from URLs. Exposed as `protected ResponseShim Response` on `WebFormsPageBase`. Now `Response.Redirect("~/Products.aspx")` compiles and navigates correctly.

**OPP-5 (ViewState on WebFormsPageBase):** Added `Dictionary<string, object> ViewState` with `[Obsolete]` warning. Page code-behind using `ViewState["key"]` compiles unchanged. `BaseWebFormsComponent` already had this (line ~145); now page base does too.

**OPP-6 (GetRouteUrl on WebFormsPageBase):** Added `GetRouteUrl(string routeName, object routeParameters)` using injected `LinkGenerator` + `IHttpContextAccessor` ΓÇö same pattern as `GetRouteUrlHelper` extension on `BaseWebFormsComponent`. Strips `.aspx` from route names.

**Key learnings:**
- `Unit.Parse()` already handles all CSS unit formats via the `Unit(string, CultureInfo, UnitType)` constructor ΓÇö no new parsing needed.
- `WebFormsPageBase` did NOT have `NavigationManager`, `LinkGenerator`, or `IHttpContextAccessor` injections prior to this change. Added all three.
- The explicit string-to-Unit operator was effectively dead code ΓÇö no tests or consuming code used the `(Unit)"string"` cast syntax.


 Team update (2026-03-11): L2 automation shims (OPP-2, 3, 5, 6) implemented by Cyclops on WebFormsPageBase  Unit implicit string, Response.Redirect shim, ViewState, GetRouteUrl. OPP-1/OPP-4 deferred.  decided by Forge (analysis), Cyclops (implementation)

### OPP-1: EnumParameter<T> Wrapper Struct (2026-07-25)

Implemented `EnumParameter<T>` ΓÇö a `readonly struct` enabling Blazor component enum parameters to accept both enum values and bare string values. This is the #1 L2 fix by volume: every migrated enum attribute like `GridLines="None"` previously required `@(GridLines.None)` Razor expression syntax.

**New file:** `src/BlazorWebFormsComponents/Enums/EnumParameter.cs`
- Implicit conversions: `T ΓåÆ EnumParameter<T>`, `string ΓåÆ EnumParameter<T>` (case-insensitive parse), `EnumParameter<T> ΓåÆ T`
- Equality operators for `EnumParameter<T>` vs `T` and `T` vs `EnumParameter<T>`
- Implements `IEquatable<EnumParameter<T>>` and `IEquatable<T>`

**55 files changed** across 46 components/interfaces/style classes:
- BaseStyledComponent, BaseWebFormsComponent, BulletedList, Calendar, Chart, ChartSeries, CheckBox, CheckBoxList, DataPager, DetailsView, FormView, GridView, Image, ImageButton, ImageMap, ListBox, ListView, Literal, Login, ChangePassword, Menu, MenuItemStyle, MenuLevelStyle, NamingContainer, Panel, RadioButton, RadioButtonList, ScriptManager, SiteMapPath, Table, TableCell, TableFooterRow, TableHeaderCell, TableHeaderRow, TableRow, TextBox, TreeNode, TreeView, UiPagerSettings, UiStyle, UiTableItemStyle, UpdatePanel, BaseValidator, Style, TableItemStyle
- Interfaces: IImageComponent, IHasLayoutStyle, IHasLayoutTableItemStyle

**Skipped (abstract class hierarchies, not enums):** DataListEnum, RepeatLayout, ButtonType, TreeViewImageSet, ValidationSummaryDisplayMode

**Skipped (nullable):** `Docking?` on ChartLegend/ChartTitle ΓÇö wrapping nullable enum params in `EnumParameter<T>?` requires separate handling.

**Key learnings / gotchas:**
1. **Switch expressions break.** C# pattern matching does NOT use user-defined implicit conversions. Every `switch (Property)` or `Property switch { EnumVal => ... }` must become `Property.Value switch { ... }`. This was the biggest source of internal code changes (~15 switch expressions updated).
2. **Shouldly `.ShouldBe()` breaks.** Extension methods like `ShouldBe` can't resolve through implicit conversions on structs. Tests need `property.Value.ShouldBe(EnumVal)`. Affected: ListView/SortingEvents, ScriptManager/ScriptManagerTests, UpdatePanel/UpdatePanelTests, Localize/InheritsLiteral.
3. **"Color Color" rule still works.** When property name matches enum type name (e.g., `GridLines` property of type `EnumParameter<GridLines>`), C# still resolves `GridLines.None` in case labels to the enum type via the "Color Color" disambiguation rule.
4. **Default values work unchanged.** `= GridLines.None` compiles because the implicit `T ΓåÆ EnumParameter<T>` conversion handles the assignment.
5. **`ToString()` is transparent.** The struct's `ToString()` delegates to `Value.ToString()`, so existing `property.ToString().ToLowerInvariant()` patterns work unchanged.
6. **Equality comparisons are safe.** The `==` and `!=` operators between `EnumParameter<T>` and `T` handle `if (Property == EnumVal)` without needing `.Value`.

**Test files needing updates (for Rogue):**
- `ListView/SortingEvents.razor` ΓÇö `SortDirection.ShouldBe()` ΓåÆ `.Value.ShouldBe()`
- `ScriptManager/ScriptManagerTests.razor` ΓÇö `ScriptMode.ShouldBe()` ΓåÆ `.Value.ShouldBe()`
- `UpdatePanel/UpdatePanelTests.razor` ΓÇö `RenderMode/UpdateMode.ShouldBe()` ΓåÆ `.Value.ShouldBe()`
- `Localize/InheritsLiteral.razor` ΓÇö overload resolution failure on `ShouldBe`

### 2026-04-27: MasterPageContext Implementation & Component Bridge

**Task:** Implement MasterPageContext cascading pattern for MasterPage/Content/ContentPlaceHolder component bridge.

**Changes delivered:**
- **MasterPage.razor/MasterPage.razor.cs:** Created MasterPageContext class with MasterPage reference + RegisterContentPlaceHolder/GetContentPlaceHolder methods. Wraps component tree in `<CascadingValue Value="this.Context">`. Provides parent discovery mechanism.
- **Content.razor/Content.razor.cs:** Injects MasterPageContext via [CascadingParameter], validates parent is MasterPage, registers self with parent context.
- **ContentPlaceHolder.razor/ContentPlaceHolder.razor.cs:** Injects MasterPageContext via [CascadingParameter], locates owning Content via context lookup, renders placeholder at correct hierarchy level.

**Key properties:**
- All three components discoverable via CascadingValue chain (no direct parent references)
- Supports dynamic registration/unregistration
- Thread-safe context operations
- Enables multi-level Content/ContentPlaceHolder nesting

**Testing:** Unit tests cover context discovery, registration, parent resolution, nested hierarchies. All passing. Build: 0 errors, 0 warnings.

**Pattern:** Mirrors Blazor's AuthenticationState + AuthorizeView design (2026-03-05) for consistency. CascadingValue precedent established in M10 theming work.

 Team update (2026-03-06): Forge produced 5 library improvement recommendations (L1-L5) assigned to Cyclops  L1: Response.Redirect shim on WebFormsPageBase (HIGH), L2: Request.QueryString shim (MEDIUM), L3: DataBind() no-op (LOW-MED), L4: form-submit.js helper (MEDIUM), L5: WebFormsSessionService (MEDIUM). Recommended Cycle 2: L1  decided by Forge
 Team update (2026-03-06): WebFormsPageBase is the canonical base class for all migrated pages (not ComponentBase). All agents must use WebFormsPageBase  decided by Jeffrey T. Fritz

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

### Core Context (2026-02-10 through 2026-02-27)

**M1M3 components:** Calendar (enum fix, async events), ImageMap (BaseStyledComponent, Guid IDs), FileUpload (InputFile integration, path sanitization), PasswordRecovery (3-step wizard), DetailsView (DataBoundComponent<T>, auto-field reflection, 10 events), Chart (BaseStyledComponent, CascadingValue "ParentChart", JS interop via ChartJsInterop, ChartConfigBuilder pure static).

**M6 base class fixes:** DataBoundComponent chain  BaseStyledComponent (14 data controls). BaseListControl<TItem> for 5 list controls (DataTextFormatString, AppendDataBoundItems). CausesValidation on CheckBox/RadioButton/TextBox. Label AssociatedControlID switches spanlabel. Login/ChangePassword/CreateUserWizard  BaseStyledComponent. Validator ControlToValidate dual-path: ForwardRef + string ID via reflection.

**M6 Menu overhaul:**  BaseStyledComponent. Selection tracking (SelectedItem/SelectedValue, MenuItemClick, MenuItemDataBound). MenuEventArgs, MaximumDynamicDisplayLevels, Orientation enum + CSS horizontal class. MenuLevelStyle lists. StaticMenuStyle sub-component + IMenuStyleContainer interface. RenderFragment parameters for all menu styles. RenderingMode=Table added (M14) with inline Razor for AngleSharp compatibility.

**M7 style sub-components:** GridView (8), DetailsView (10), FormView (7), DataGrid (7)  all CascadingParameter + UiTableItemStyle. Style priority: Edit > Selected > Alternating > Row. TreeView: TreeNodeStyle + 6 sub-components, selection, ExpandAll/CollapseAll, FindNode, ExpandDepth, NodeIndent. GridView: selection, 10 display props. FormView/DetailsView events + PagerTemplate + Caption. DataGrid paging/sorting. ListView 10 CRUD events + EditItemTemplate/InsertItemTemplate. Panel BackImageUrl. Login Orientation + TextLayout. Shared PagerSettings (12 props, IPagerSettingsContainer) for GridView/FormView/DetailsView.

**M8 bug fixes:** Menu JS null guard + Calendar conditional scope + Menu auto-ID (`menu_{GetHashCode():x}`).

**M9 migration-fidelity:** ToolTip  BaseStyledComponent (removed from 8, added title="@ToolTip" to 32 components). ValidationSummary comma-split fix (IndexOf + Substring). SkinID boolstring. TreeView NodeImage fallback restructured (ShowExpandCollapse + ExpandCollapseImage helper).

**M10 Theming:** ControlSkin (nullable props, StyleSheetTheme semantics). ThemeConfiguration (case-insensitive keys, empty-string default skin, GetSkin returns null). ThemeProvider as CascadingValue wrapper. SkinID="" default, EnableTheming=true, [Obsolete] removed. CascadingParameter in BaseStyledComponent, ApplySkin in OnParametersSet. LoginView/PasswordRecovery  BaseStyledComponent.

**M15 HTML fidelity fixes:** Button `<input>` rendering. BulletedList `<span>` removal + `<ol>` CSS-only (no HTML type attr, GetStartAttribute returns int?). LinkButton class + aspNetDisabled. Image longdesc conditional. Calendar structural (tbody, width:14%, day titles, abbr headers, align center, border-collapse, navigation sub-table). FileUpload clean ID. CheckBox span verified. GridView UseAccessibleHeader default falsetrue. 27 test files updated for Button `<input>`. 10 new tests. All 1283 pass.

**M16:** LoginView wrapper `<div>` for styles (#352). ClientIDMode enum (Inherit/AutoID/Static/Predictable) on BaseWebFormsComponent. ComponentIdGenerator refactored: GetEffectiveClientIDMode(), BuildAutoID(), BuildPredictableID(). UseCtl00Prefix only in AutoID mode. NamingContainer auto-sets AutoID when UseCtl00Prefix=true.

**Key patterns:** Orientation enum collides with parameter name  use `Enums.Orientation.Vertical`. `_ = callback.InvokeAsync()` for render-time events. `Path.GetFileName()` for file save security. CI secret-gating: env var indirection. Null-returning helpers for conditional HTML attributes. aspNetDisabled class for disabled controls. Always test default parameter values explicitly.


<!--  Summarized 2026-03-01 by Scribe  covers M17-M20 Wave 1 -->


<!-- Summarized 2026-03-05 by Scribe -- covers M17-M20 through Run 6 enhancements -->

### M17 through Run 6 Summary (2026-02-27 through 2026-03-05)

**M17 AJAX audit fixes:** EnablePartialRendering default true. Scripts collection. UpdateProgress CssClass + display modes. ScriptReference gained ScriptMode/NotifyScriptLoaded. M18 bugs (#380/#382/#383) verified already fixed in M15. CheckBox bare input id (#386). MenuItemStyle SetFontsFromAttributes (#360). LinkButton CssClass verified correct (Issue #379).

**Issue #387 normalizer:** 4 enhancements (case-insensitive pairing, boolean attrs, empty style strip, GUID placeholders). Pipeline: regex > style > empty > boolean > GUID > attr sort > artifact > whitespace.

**Theming (#364/#365):** SkinBuilder expression trees for nested property access. ThemeConfiguration ForControl() fluent API. CascadingValue by type (unnamed). WebColor.FromHtml(). Theme wiring: CascadingParameter `CascadedTheme` on BaseWebFormsComponent. ApplySkin chain. FontInfo auto-sync. WebFormsPage cascades Theme ?? CascadedTheme.

**Release & ListView:** Unified release.yml (single workflow, tag-based version). ListView #406 EditItemTemplate (closure + @key fix). #356 CRUD events (ItemCreated per-item, ItemCommand fires before specific). EventArgs gained IOrderedDictionary. FormView RenderOuterTable parameter.

**CSS fixes:** 7 WingtipToys visual fixes. Playwright blocks file://, use HTTP. Get-NetTCPConnection for PID cleanup.

**Layer 1 benchmark:** scan 0.9s, migrate 2.4s, 276 transforms, 338 build errors (code-behind). Layer 2+3: 563s total, clean build after 3 rounds.

**Script enhancements:** ConvertFrom-MasterPage (6 transforms), New-AppRazorScaffold, Eval format-string regex, String.Format regex. Run 5: GetRouteUrl 4 overloads, 309 transforms, 6 new enhancements. Toolkit sync: migration-toolkit/ canonical, 47KB bwfc-migrate.ps1 synced. Run 6: 4 enhancements (TFM net10.0, SelectMethod BWFC-aware, wwwroot copy, compilable stubs). Bug: @rendermode in _Imports invalid.

Team updates (2026-02-27-05): Branching workflow, issues via PR refs, AJAX controls, theming, release.yml, toolkit restructured, PRs upstream, standards formalized, Run 2/5/6 validated.


<!-- Summarized 2026-03-06 by Scribe -- covers @rendermode fix through On-prefix aliases -->

<!-- Summarized 2026-03-06 by Scribe -- covers 2026-03-05 implementation through ShoppingCart fix -->

### 2026-03-05 through 2026-03-06 Implementation Summary

**@rendermode fix:** Removed standalone `@rendermode InteractiveServer` from _Imports.razor scaffold -- it's a directive *attribute* on component instances, not a standalone directive.

**WebFormsPageBase:** Abstract base class inheriting `ComponentBase`. Delegates Title/MetaDescription/MetaKeywords to IPageService. `IsPostBack => false`, `Page => this`. **WebFormsPage consolidation:** Merged Page.razor head-rendering (Option B). Optional IPageService via `ServiceProvider.GetService<>()`. RenderPageHead parameter (default true). IDisposable for event unsubscription.

**On-prefix aliases:** 50 `[Parameter] EventCallback` aliases across 7 data components. Pattern: two independent properties + coalescing at invocation.

**Run 8 Layer 2 WingtipToys:** Full Layer 2 migration of `samples/Run8WingtipToys/` -- 8 files created (EF Core models, DbContext, CartStateService), 14 modified (csproj, Program.cs, layouts, pages), 3 deleted, 26 stub files fixed. Clean build. Key patterns: IDbContextFactory, SupplyParameterFromQuery, CartStateService scoped DI, ListView `Items=`, FormView `RenderOuterTable="false"`.

**ShoppingCart BWFC fix:** Replaced plain HTML table with BWFC GridView in Run8 + AfterWingtipToys. AfterWingtipToys reference was wrong (plain HTML). Run 7's ShoppingCart.razor is gold standard.

Team updates: @rendermode fix (PR #419), EF Core 10.0.3, WebFormsPageBase shipped, WebFormsPage consolidation, event handler audit, ShoppingCart regression test, BWFC preservation mandatory.

 Team update (2026-03-05): BWFC control preservation is mandatory -- all migration output must use BWFC components, never flatten to raw HTML -- decided by Jeffrey T. Fritz, Forge, Cyclops

 Team update (2026-03-05): LoginView redesigned to delegate to AuthorizeView -- decided by Forge
 Team update (2026-03-05): LoginStatus flagged for AuthorizeView redesign  decided by Forge

- **LoginStatus AuthorizeView redesign:** Replaced manual `AuthenticationStateProvider` injection + `OnInitializedAsync` auth check + `UserAuthenticated` bool with `<AuthorizeView>` delegation (same pattern as LoginView). Removed unused `CalculatedStyle` property and `BlazorComponentUtilities` using. Added null guard on `LoginHandle` so missing `LoginPageUrl` doesn't throw. Updated `LoginPageUrl` comment to explain it's a Blazor adaptation (Web Forms used `FormsAuthentication.LoginUrl`). LogoutAction abstract class → enum conversion left for separate PR per team decision. Build clean.

### P0 Event Handler Fixes (2026-03-06)

All 7 P0 items from Forge's audit implemented:
- **P0-1** Repeater: 3 events added (ItemCommand, ItemCreated, ItemDataBound) — had ZERO before
- **P0-2** DataList: 7 events added + ItemDataBound bare alias. Renamed internal `ItemDataBound()` → `ItemDataBoundInternal()` to avoid parameter collision
- **P0-3/P0-4** GridView: RowDataBound + RowCreated added, bare RowCommand alias, ButtonField.razor.cs updated to coalesce
- **P0-5** DetailsView: ItemCreated added
- **P0-6** FormView: OnItemInserted type fixed (FormViewInsertEventArgs → FormViewInsertedEventArgs), 6 bare CRUD aliases added
- **P0-7** SelectMethod: moved from OnAfterRender(firstRender) to OnParametersSet(), added RefreshSelectMethod()

New EventArgs: `RepeaterCommandEventArgs`, `RepeaterItemEventArgs`, `DataListCommandEventArgs`, `GridViewRowEventArgs`, `FormViewInsertedEventArgs`.

**Pattern:** When `[Parameter]` name collides with internal method, rename method with `*Internal` suffix — never rename the parameter.


 Team update (2026-03-06): Forge's Event Handler Fidelity Audit merged to decisions.md (P0 items all resolved by Cyclops, tested by Rogue). 11 P1 and 7 P2 items remain for future work.  decided by Forge, implemented by Cyclops


 Team update (2026-03-05): Run 9 BWFC review APPROVED with 2 findings  ImageButtonraw img in ShoppingCart (P0, OnClick lost), HyperLink dropped in Manage (P2). ImageButton fix needed.  decided by Forge

- **Squad Places comments (2026-03-05):** Posted 2 comments on Breaking Bad squad's articles (Terrarium .NET 3.5→10 migration). Comment 1 on "Leaf-to-Root Migration" (artifact 5979f2ed): shared base class hierarchy cascade experience (ToolTip fix hitting 32 components/27 tests), SelectMethod lifecycle challenges, naming collision rule (*Internal suffix). Comment 2 on "ASMX SOAP to Minimal APIs" (artifact 8e18dfe3): shared "map one-to-one first" philosophy, System.Text.Json PascalCase/camelCase pain at C#/JS boundary, BinaryFormatter removal forcing ViewState redesign, interest in Terrarium's real-time simulation API.

 Team update (2026-03-06): Run 10 preservation 92.7% (164/177)  NEEDS WORK. ImageButtonimg still persists in ShoppingCart (P0). DropDownList AppendDataBoundItems verification assigned to Cyclops (P2-2). Layer 1 script bugs consolidated (ItemType fix, validator params, base class stripping all implemented).  decided by Forge, Bishop

📌 Team update (2026-03-06): migration-toolkit is end-user distributable; migration skills belong in migration-toolkit/skills/ not .squad/skills/ — decided by Jeffrey T. Fritz

### Run 7 Layer 2 WingtipToys (AfterWingtipToys)

**Build rounds:** 2 (round 1 had 7 errors, round 2 was clean 0/0)

**Layer 2 fix categories applied:**

1. **Project Configuration:** Changed csproj from NuGet `Fritz.BlazorWebFormsComponents` to project reference. Added EF Core SQLite packages with wildcard version `10.0.0-*`. Added `RootNamespace=WingtipToys`. Fixed Program.cs: AddHttpContextAccessor before AddBlazorWebFormsComponents, EF Core DbContextFactory registration, ShoppingCartService/CartStateService DI, database EnsureCreated+seed. Fixed _Imports.razor: added BlazorWebFormsComponents.Enums, Microsoft.EntityFrameworkCore, WingtipToys.Models/Data/Services namespaces.

2. **Data Layer:** Created Models/ (Product, Category, CartItem, Order, OrderDetail) matching original Web Forms models with nullable reference types. Created Data/ProductContext.cs with EF Core DbContext, DbSets, and HasData seed (16 products, 5 categories). SQLite provider.

3. **Services:** Created ShoppingCartService (IDbContextFactory-based, async CRUD), CartStateService (cookie-based cart ID via IHttpContextAccessor).

4. **Identity/Auth Stubs:** Register.razor has UserName, Email, Password, ConfirmPassword fields with BWFC TextBox/Button/Label components. Login.razor has Email, Password, RememberMe fields. Both wire to MockAuthService/MockAuthenticationStateProvider.

5. **Code-Behind Rewrites (33 files):** All .razor.cs files rewritten from raw Web Forms (System.Web, OWIN, Microsoft.AspNet.Identity) to Blazor ComponentBase. Key patterns: IDbContextFactory injection, SupplyParameterFromQuery, OnParametersSetAsync for query-driven data, NavigationManager.NavigateTo for redirects.

6. **Markup Fixes:** ListView GroupTemplate/LayoutTemplate placeholders → @context. Removed GetRouteUrl calls → href with query params. Removed <%# %> expressions → @context. Fixed ImageButton → Button. Removed runat artifacts. Fixed category links to use /ProductList?id=N. ShoppingCart preserved as BWFC GridView with TItem, Items, editable TextBox, CheckBox, Update/Checkout buttons.

7. **Misc Cleanup:** Removed <friendlyUrls:ViewSwitcher/>, cleaned Mobile layout, stubbed all Account/Checkout pages, AdminPage wired with EF Core data loading.

**Build round 1 errors (7):**
- CS0103 `Title` not found → class name `_Default` didn't match razor file `Default.razor`
- CS0103 `_categories`/`_cartCount` not found → MainLayout code-behind was class `SiteMaster` in wrong namespace
- CS1503 type mismatch (4x) → BWFC Button OnClick is `EventCallback<EventArgs>`, not `MouseEventArgs`

**Build round 2:** 0 errors, 0 warnings. Clean.

**Key patterns discovered:**
- BWFC Button.OnClick is `EventCallback<EventArgs>` (not MouseEventArgs) — all click handlers must use `EventArgs`
- Code-behind partial class name MUST match the .razor filename exactly (no underscore prefix for Default)
- Layout code-behind namespace must match Components/Layout/ path → `WingtipToys.Components.Layout`
- EF Core wildcard version `10.0.0-*` avoids NU1603 warnings for preview packages

 Team update (2026-03-06): bwfc-migrate.ps1 uses -Path and -Output params (not -SourcePath/-DestinationPath). ProjectName is auto-detected  decided by Bishop
 


