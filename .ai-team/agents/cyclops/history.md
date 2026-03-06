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

📌 Team update (2026-03-06): migration-toolkit is end-user distributable; migration skills belong in migration-toolkit/skills/ not .ai-team/skills/ — decided by Jeffrey T. Fritz
