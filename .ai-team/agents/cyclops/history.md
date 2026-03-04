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

### Key Team Updates (2026-03-02 through 2026-03-04)

- Skins & Themes roadmap: 3 waves, 15 WIs (Forge)
- Project reframed as migration system (Jeff), M22 planned (Forge)
- FormView RenderOuterTable resolved, ModelErrorMessage 29/29 coverage
- WingtipToys CSS fidelity: 7 fixes identified (Forge)
- Themes (#369) last  ListView CRUD first, WingtipToys second (Jeff)
- WingtipToys 7-phase schedule: 26 work items (Forge)
- ListView CRUD test conventions: 43 tests (Rogue)

 Team update (2026-03-04): PRs must target upstream FritzAndFriends/BlazorWebFormsComponents, not the fork  decided by Jeffrey T. Fritz
 Team update (2026-03-04): Migration Run 2  11/11 features pass, PR #418 fixes confirmed critical, toolkit ready for docs  decided by Forge
 Team update (2026-03-04): Migration toolkit restructured into self-contained migration-toolkit/ package  decided by Jeffrey T. Fritz, Forge

 Team update (2026-03-04): Forge proposed 2 regex additions to bwfc-migrate.ps1 for Eval format-string and String.Format patterns (eval-regex-enhancement, status: Proposed)  decided by Forge

### Master Page & Expression Enhancements (2026-03-04)

**ConvertFrom-MasterPage function:** Added to bwfc-migrate.ps1 for .master-file-specific transforms. 6 mechanical transforms: (1) inject `@inherits LayoutComponentBase`, (2) strip document wrapper (DOCTYPE/html/head/body), (3) ContentPlaceHolder MainContent → @Body (other CPHs get TODO comments), (4) remove ScriptManager block (multiline (?s) regex), (5) extract meta/link/title from head into `<HeadContent>` block, (6) output path remap to `Components\Layout\{Name}Layout.razor` (Site.Master → MainLayout.razor). Called after ConvertFrom-MasterDirective in the pipeline. Flags LoginView and SelectMethod as manual items.

**New-AppRazorScaffold function:** Generates `Components/App.razor` (Blazor document shell with HeadOutlet + Routes @rendermode) and `Components/Routes.razor` (Router with DefaultLayout=MainLayout). Called alongside New-ProjectScaffold from entry point.

**Eval format-string regex:** `<%#: Eval("prop", "{0:fmt}") %>` → `@context.prop.ToString("fmt")`. Placed BEFORE existing single-arg Eval regex so specific pattern matches first.

**String.Format regex:** `<%#: String.Format("{0:fmt}", Item.Prop) %>` → `@($"{context.Prop:fmt}")`. Uses `$$` in replacement for literal `$` in .NET regex. Placed before existing Eval/Item regexes.

**Key decisions:** ScriptManager regex uses `(?s)` for multiline matching across `<Scripts>` block. ContentPlaceHolder regex uses `(?si)` for case-insensitive + singleline. Head metadata extraction happens before head section removal. Code-behind output path for .master files reuses computed `$razorRelPath + $cbSuffix`.

### GetRouteUrl Completion (2026-03-05)

- `GetRouteUrlHelper.cs` has 4 extension method overloads on `BaseWebFormsComponent`: two taking `object routeParameters` (working), two taking `RouteValueDictionary` (were stubbed, now complete). All delegate to `LinkGenerator.GetPathByRouteValues`.
- `RouteValueDictionary` is accepted by `LinkGenerator.GetPathByRouteValues` as the `object values` parameter — it's already the internal type, so no conversion needed.
- WingtipToys uses `GetRouteUrl("RouteName", new { param = value })` exclusively (anonymous objects) — the `object` overloads cover all WingtipToys scenarios. `RouteValueDictionary` overloads exist for completeness with the Web Forms API surface.
- `LinkGenerator` is auto-registered by ASP.NET Core routing. `IHttpContextAccessor` requires explicit `services.AddHttpContextAccessor()` — the sample already does this in Program.cs.
- In Web Forms, `GetRouteUrl` is an instance method on `Control`/`Page`. In Blazor, it's an extension method on `BaseWebFormsComponent`, so migrated code uses `this.GetRouteUrl(...)` in code-behind or `@(this.GetRouteUrl(...))` in Razor markup.
- Migration toolkit currently suggests inlining route URLs (e.g., `@($"/Products/{context.ID}")`) rather than using the extension method. Both approaches are valid — inlining is simpler, extension method is more faithful to route-name resolution.


 Team update (2026-03-05): Migration report image paths must use ../../../ (3-level traversal) for repo-root assets  decided by Beast
