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

<!-- Summarized 2026-03-04 by Scribe — covers team updates and script enhancements 2026-03-02 through 2026-03-04 -->

### Script & Toolkit Summary (2026-03-02 through 2026-03-04)

**Team context:** PRs target upstream (not fork). Migration toolkit restructured into self-contained migration-toolkit/ package. Migration Run 2 validated 11/11 features (PR #418 critical). Project reframed as migration system. M22 planned. ListView CRUD first, Themes last.

**Script enhancements (bwfc-migrate.ps1):** ConvertFrom-MasterPage (6 transforms: @inherits injection, document wrapper strip, ContentPlaceHolder→@Body, ScriptManager removal, HeadContent extraction, layout path remap). New-AppRazorScaffold (App.razor + Routes.razor). Eval format-string regex (`Eval("prop","{0:fmt}")` → `@context.prop.ToString("fmt")`). String.Format regex (`String.Format("{0:fmt}",Item.Prop)` → `@($"{context.Prop:fmt}")`). Regex ordering: specific patterns before general. ScriptManager uses `(?s)`, ContentPlaceHolder uses `(?si)`.

<!-- Summarized 2026-03-05 by Scribe -- covers GetRouteUrl through migration-toolkit sync -->

### GetRouteUrl, Run 5 & Toolkit Sync Summary (2026-03-04 through 2026-03-05)

**GetRouteUrl:** 4 extension method overloads on BaseWebFormsComponent (2 object, 2 RouteValueDictionary), all delegate to LinkGenerator.GetPathByRouteValues. WingtipToys uses anonymous-object overloads only.

**Run 5 migration:** 3.25s, 309 transforms, 32 files. 6 new enhancements confirmed (LoginView, GetRouteUrl hints, SelectMethod TODO, Register cleanup, ContentPlaceHolder, String.Format). Clean build after stubbing Account/Checkout. Gaps: static assets need wwwroot/ copy, csproj TFM still net8.0.

**Toolkit sync:** migration-toolkit/ is canonical home. Synced 47KB bwfc-migrate.ps1 over stale 29KB copy. PageService.Title already exists -- updated analysis and skill. .NET SDK prereq updated to 10.0+. Lesson: always check existing BWFC components before flagging as missing.

Team updates: Migration report 3-level traversal (Beast). Run 5 reports need Works/Doesn't-Work sections (Beast). Migration standards formalized -- EF Core, .NET 10, ASP.NET Core Identity, BWFC data controls preferred, migration-toolkit/ canonical (Jeff/Forge).

<!-- Summarized 2026-03-04 by Scribe -- covers Run 6 script enhancements -->


## Archived 2026-03-12 (entries from 2026-03-12 through 2025-07-25)

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
