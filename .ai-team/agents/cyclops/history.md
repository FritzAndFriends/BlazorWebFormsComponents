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

<!--  Summarized 2026-03-03 by Scribe  covers M20 theming through CSS fixes -->

### M20 Theming, Release & WingtipToys Context (2026-03-01 through 2026-03-03)

**Issue #366 theme wiring:** CascadingParameter ThemeConfiguration moved to BaseWebFormsComponent (named CascadedTheme). ApplySkin>ApplyThemeSkin (virtual override chain). ThemeProvider uses @inherits ComponentBase. WebFormsPage cascades Theme ?? CascadedTheme. Lesson: _Imports.razor @inherits affects ALL .razor files.

**FontInfo auto-sync:** Name/Names backing-field properties with bidirectional sync. ApplyThemeSkin guards both Font.Name AND Font.Names. Lesson: paired Web Forms properties must replicate sync behavior.

**Unified release.yml:** Single workflow on release:published. Version from tag_name. NuGet override: -p:PackageVersion + -p:Version. version.json 3-segment SemVer. deploy-server-side.yml/nuget.yml to workflow_dispatch-only.

**Issue #406 ListView EditItemTemplate:** (1) Closure bug: moved template selection before CascadingValue. (2) Diffing bug: added @key. Lessons: @key in loops with template switching; never reference loop-external mutable vars in ChildContent.

**FormView RenderOuterTable:** [Parameter] bool RenderOuterTable=true. When false, suppresses table wrapper/header/footer/pager. Driven by WingtipToys ProductDetails.aspx.

**Original WingtipToys build:** Connection strings v11.0 to MSSQLLocalDB. Empty Directory.Build.props blocks NBGV. `nuget install` for packages.config. IIS Express port 5200.

**CSS fixes screenshot refresh (2026-03-03):** 7 visual fixes applied (Cerulean theme, 4-column grid, BoundField currency, Trucks category, Site.css, category IDs). 6 screenshots updated. Lessons: Playwright blocks file:// -- serve via HTTP; verify visual requirements before capturing; detached dotnet run needs Get-NetTCPConnection for PID cleanup.

📌 Team update (2026-03-02): Skins & Themes roadmap — 3 waves, 15 WIs — decided by Forge
📌 Team updates (2026-03-02): M22 planned (Forge), project reframed as migration system (Jeff), FormView RenderOuterTable resolved (Cyclops), ModelErrorMessage 29/29 coverage (Forge), WingtipToys pipeline validated — 28/29 controls covered.
📌 Team update (2026-03-03): WingtipToys CSS fidelity — 7 visual differences identified requiring fixes (Cerulean theme, 4-column grid, BoundField bug, Trucks category, Site.css, category IDs) — decided by Forge
<!-- Note: M20 Theming & Release Process summary (2026-03-02) removed — superseded by M20 Theming, Release & WingtipToys Context above -->
### Issue #406 — ListView EditItemTemplate Not Rendering (2026-03-02)
<!-- Archived 2026-03-03 by Scribe — M20 theming + release detail moved to summary above -->



 Team update (2026-03-03): Themes (#369) implementation last  ListView CRUD first, WingtipToys features second, themes last  directed by Jeff Fritz


 Team update (2026-03-03): WingtipToys 7-phase feature schedule established  26 work items, critical path through Data Foundation  Product Browsing  Shopping Cart  Checkout  Polish  decided by Forge


 Team update (2026-03-03): ListView CRUD test conventions established  43 tests, event ordering via List<string>, cancellation assertions, bUnit double-render handling  decided by Rogue

### ListView CRUD Events — Correctness Fixes (2026-03-03)

- **Issue #356 audit:** All 16 CRUD events were already declared (EventCallback parameters + EventArgs classes + HandleCommand routing) from M7 and M21. The issue was open because the work was done incrementally across milestones.
- **Bug 1 — ItemCreated firing wrong:** Was `EventCallback` (no type param) firing once in `OnAfterRenderAsync(firstRender)`. Web Forms fires `ItemCreated` with `ListViewItemEventArgs` per-item during data binding, BEFORE `ItemDataBound`. Fixed: changed to `EventCallback<ListViewItemEventArgs>`, added `RaiseItemCreated()` helper, wired per-item in both non-grouped and grouped rendering paths in ListView.razor.
- **Bug 2 — ItemCommand not firing for known commands:** Was only firing for unknown commands (in `default` case of switch). Web Forms fires `ItemCommand` for ALL commands first, then routes to the specific handler (ItemEditing, ItemDeleting, etc.). Fixed: moved `ItemCommand.InvokeAsync()` before the switch statement.
- **Pattern:** Web Forms event order for commands is: ItemCommand → specific event (ItemEditing/ItemDeleting/etc.). ItemCreated fires per-item before ItemDataBound. These are documented Web Forms lifecycle behaviors that must be matched.
- **EventArgs completeness:** Web Forms EventArgs have IOrderedDictionary properties (Keys, Values, NewValues, OldValues) tied to the DataSource control paradigm. These are deliberately omitted since Blazor has no DataSource controls — consumers work directly with typed objects via templates.

### Layer 1 Benchmark — WingtipToys Migration Scripts (2026-03-04)

- **bwfc-scan.ps1:** Ran against `samples/WingtipToys/WingtipToys/`. 0.9s for 32 files, found 230 control usages across 31 control types, 100% BWFC coverage score. Script parameters: `-Path` (mandatory), `-OutputFormat` (Console/Json/Markdown), `-OutputFile` (optional).
- **bwfc-migrate.ps1:** Ran against same source, output to `samples/FreshWingtipToys/`. 2.4s, 276 transforms applied, 32 files → 33 .razor + 32 .razor.cs + scaffolded .csproj/Program.cs/_Imports.razor. 79 static files copied. 18 items flagged for manual review (14 complex data binding expressions, 4 Register directive removals).
- **Build result:** 338 errors, all expected — code-behind files still reference `System.Web.UI.Page`, `Microsoft.AspNet.Identity`, `WingtipToys.Models/Logic`. These are Layer 2 work.
- **Scaffold fix needed:** Generated .csproj uses `PackageReference Include="Fritz.BlazorWebFormsComponents" Version="*"` and targets `net8.0`. For local dev, must change to `ProjectReference` and `net10.0`. Consider updating `bwfc-migrate.ps1` scaffold to detect local repo context.
- **Pattern:** Layer 1 handles ~40% of migration (markup transforms). Layer 2 (Copilot skill) needed for code-behind lifecycle, domain models, Identity migration, OWIN→Core middleware.

### Layer 2+3 Benchmark — WingtipToys Full Migration (2026-03-04)

- **Total time:** 563s (~9.4 min) for Layer 2+3 migration of 32-page WingtipToys app.
- **Phase breakdown:** Data infrastructure (121s), Core storefront (136s), Checkout+Admin (187s), Layout (20s), Build fix (99s).
- **Build result:** Clean build (0 errors, 0 warnings) after 3 rounds. Round 1 = NuGet restore. Round 2 = Account page stubs missing vars. Round 3 = clean.
- **Account pages:** Copied from AfterWingtipToys reference — Identity migration is boilerplate, not domain-specific. This is the pragmatic choice in a real migration.
- **Key transforms applied:** SelectMethod→Items, ItemType→TItem, Page_Load→OnInitializedAsync, Response.Redirect→NavigateTo, Session→scoped services, QueryString→SupplyParameterFromQuery, EF6→EF Core with IDbContextFactory.
- **Architecture:** SQLite, scoped CartStateService/CheckoutStateService, MockPayPalService, ASP.NET Core Identity with canEdit role.
- **Pattern:** Layer 2+3 takes ~9 min with Copilot vs estimated 4-8 hours manually. The migration skills provide reliable translation rules. Having a reference implementation (AfterWingtipToys) to validate against accelerates decisions significantly.


 Team update (2026-03-04): Migration test reports go in docs/migration-tests/  directed by Jeffrey T. Fritz
 Team update (2026-03-04): PRs must target upstream FritzAndFriends repo, not origin fork  directed by Jeffrey T. Fritz
 Team update (2026-03-04): migration-toolkit/ is the distribution package for external users; .github/skills/ is internal only  decided by Forge, Jeffrey T. Fritz
