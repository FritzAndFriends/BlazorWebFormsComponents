# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents  Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!--  Summarized 2026-02-27 by Scribe  covers M1M16 -->

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

### Run 6 Script Enhancements (2026-03-05)

4 enhancements to bwfc-migrate.ps1: (1) TFM net8.0->net10.0 + RenderMode using (line 139), (2) SelectMethod TODO->BWFC Items guidance (-120s, line 756), (3) static files->wwwroot/ (line 1103), (4) compilable stubs for unconvertible pages (Test-UnconvertiblePage + New-CompilableStub, lines 907-988). Bug found: @rendermode InteractiveServer in _Imports.razor is invalid in .NET 10. Test-UnconvertiblePage must also scan .aspx.cs code-behind.

Team update (2026-03-04): Run 6 improvement analysis -> decided by Forge
Team update (2026-03-04): @rendermode InteractiveServer in _Imports.razor scaffold is invalid in .NET 10 -- must be removed from bwfc-migrate.ps1 line 164. Also: Test-UnconvertiblePage must scan .aspx.cs code-behind files. -- decided by Forge