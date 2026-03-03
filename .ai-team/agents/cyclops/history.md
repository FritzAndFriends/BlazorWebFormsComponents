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
<!-- Archived 2026-03-03 by Scribe — M20 theming + release detail moved to summary above -->



 Team update (2026-03-03): Themes (#369) implementation last  ListView CRUD first, WingtipToys features second, themes last  directed by Jeff Fritz


 Team update (2026-03-03): WingtipToys 7-phase feature schedule established  26 work items, critical path through Data Foundation  Product Browsing  Shopping Cart  Checkout  Polish  decided by Forge


 Team update (2026-03-03): ListView CRUD test conventions established  43 tests, event ordering via List<string>, cancellation assertions, bUnit double-render handling  decided by Rogue

