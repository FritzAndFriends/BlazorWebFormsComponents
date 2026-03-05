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

### @rendermode Scaffold Fix (2026-03-05)

**Fix applied:** Removed `@rendermode InteractiveServer` standalone directive from _Imports.razor scaffold in both migration-toolkit/scripts/bwfc-migrate.ps1 and scripts/bwfc-migrate.ps1. The `@using static Microsoft.AspNetCore.Components.Web.RenderMode` using directive was kept (correct — enables shorthand `InteractiveServer`). App.razor scaffold already had the correct pattern: `<Routes @rendermode="InteractiveServer" />` and `<HeadOutlet @rendermode="InteractiveServer" />`.

**Lesson:** `@rendermode` is a directive *attribute* that goes on component instances (e.g., `<Routes @rendermode="InteractiveServer" />`), NOT a standalone Razor directive. Placing it as a bare directive in _Imports.razor causes build errors. For global interactivity, apply it to `<Routes>` and `<HeadOutlet>` in App.razor. The `@using static` import in _Imports.razor is the correct way to make `InteractiveServer` available as a shorthand across all pages.
 Team update (2026-03-04): @rendermode InteractiveServer belongs in App.razor, not _Imports.razor  consolidated from Forge, Cyclops, Jeffrey T. Fritz (PR #419)


 Team update (2026-03-04): EF Core must use 10.0.3 (latest .NET 10)  directed by Jeff

### WebFormsPageBase Implementation (2026-03-05)

**WebFormsPageBase:** Created `src/BlazorWebFormsComponents/WebFormsPageBase.cs` — abstract base class inheriting `ComponentBase` (not `BaseWebFormsComponent`). Injects `IPageService` privately, exposes `Title`, `MetaDescription`, `MetaKeywords` as delegate properties. `IsPostBack => false` so `if (!IsPostBack)` compiles and always enters. `Page => this` self-reference enables `Page.Title = "X"` to compile unchanged from Web Forms code-behind. Converted pages use `@inherits WebFormsPageBase` (one line in `_Imports.razor`). Build verified clean (63 pre-existing warnings, 0 errors). Lesson: Pages are top-level containers, not child controls — inheriting `ComponentBase` directly avoids the CascadingValue wrapping and control-tree logic in `BaseWebFormsComponent`.

 Team update (2026-03-04): WebFormsPageBase implemented  decided by Forge, approved by Jeff

### WebFormsPage IPageService Consolidation (2026-03-05)

**WebFormsPage enhanced:** Merged `Page.razor` head-rendering capability into `WebFormsPage`. Added optional `IPageService` resolution via `ServiceProvider.GetService<IPageService>()` — WebFormsPage still works for naming/theming when IPageService is not registered. Subscribes to `TitleChanged`, `MetaDescriptionChanged`, `MetaKeywordsChanged` events in `OnInitialized()`. Renders `<PageTitle>` and `<HeadContent>` before the existing CascadingValue wrapper. Added `RenderPageHead` parameter (default true) to allow opting out. Implements `IDisposable` to unsubscribe from events. `Page.razor` left untouched as standalone option. Build verified clean (70 pre-existing warnings, 0 errors). Lesson: `IServiceProvider` in `BaseWebFormsComponent` is private — child classes needing it must inject their own via `[Inject]`.

� Team update (2026-03-05): WebFormsPage now includes IPageService head rendering (title + meta tags), merging Page.razor capability per Option B consolidation. Layout simplified to single <WebFormsPage> component. Page.razor remains standalone.  decided by Forge, implemented by Cyclops


 Team update (2026-03-05): Event handler audit complete  ~50 naming mismatches found, On-prefix aliases recommended  decided by Forge, Rogue

### On-Prefix EventCallback Aliases (2026-03-05)

**What:** Added 50 On-prefixed `[Parameter] EventCallback` aliases across 7 data components for Web Forms migration compatibility. Pattern: add a new `[Parameter]` property with the `On` prefix alongside each existing property. At invocation sites, coalesce with `var handler = Original.HasDelegate ? Original : OnOriginal;` so whichever name the consumer uses in markup works.

**Components modified:** GridView (9 aliases), DetailsView (11), FormView (6), ListView (16), DataGrid (5), Menu (2), TreeView (1).

**Key details:**
- Existing properties untouched (non-breaking). Only new `On`-prefixed aliases added.
- Invocation sites updated to prefer the original property if it has a delegate, falling back to the alias.
- `HasDelegate` guard checks updated where present (GridView.ShowCommandColumn, Menu.NotifyItemClicked, ListView.RaiseItemCreated) to check both original and alias.
- FormView already had `On`-prefixed events for delete/insert/update (OnItemDeleting, etc.). Only added aliases for the 6 events that lacked them (ModeChanging, ModeChanged, ItemCommand, ItemCreated, PageIndexChanging, PageIndexChanged).
- Build verified clean: 0 errors, 70 pre-existing warnings.

**Lesson:** Blazor sets `[Parameter]` properties independently by name during diffing. You cannot use a C# property getter/setter that delegates to another property — the framework won't see changes. Two independent properties with coalescing at invocation is the correct pattern.


 Team update (2026-03-05): ShoppingCart.aspx added as Layer 1 regression test case  migration output must contain <GridView not <table class=  decided by Forge

 Team update (2026-03-05): BWFC control preservation is mandatory  all asp: controls must be preserved as BWFC components in migration output, never flattened to raw HTML. Test-BwfcControlPreservation verifies automatically.  decided by Jeffrey T. Fritz, implemented by Forge

