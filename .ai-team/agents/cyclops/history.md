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

### 2026-03-05 Implementation Summary

**@rendermode fix:** Removed standalone `@rendermode InteractiveServer` from _Imports.razor scaffold. It's a directive *attribute* on component instances (App.razor `<Routes>`/`<HeadOutlet>`), not a standalone directive. `@using static` import enables shorthand.

**WebFormsPageBase:** Abstract base class inheriting `ComponentBase` (not `BaseWebFormsComponent`). Delegates Title/MetaDescription/MetaKeywords to IPageService. `IsPostBack => false`, `Page => this`.

**WebFormsPage consolidation:** Merged Page.razor head-rendering into WebFormsPage (Option B). Optional IPageService via `ServiceProvider.GetService<>()`. RenderPageHead parameter (default true). IDisposable for event unsubscription.

**On-prefix aliases:** 50 `[Parameter] EventCallback` aliases across 7 data components (GridView 9, DetailsView 11, FormView 6, ListView 16, DataGrid 5, Menu 2, TreeView 1). Pattern: two independent properties + coalescing at invocation. Blazor sets [Parameter] properties by name independently.

Team updates: @rendermode fix (PR #419), EF Core 10.0.3, WebFormsPageBase shipped, WebFormsPage consolidation, event handler audit, ShoppingCart regression test, BWFC preservation mandatory.
### Run 8 Layer 2 — WingtipToys Migration Implementation (2026-03-06)

**What:** Completed Layer 2 migration of `samples/Run8WingtipToys/` from non-functional Layer 1 scaffold to working end-to-end shopping flow, using `samples/AfterWingtipToys/` as the reference implementation.

**Files created (8):**
- `Models/Product.cs`, `Models/Category.cs`, `Models/CartItem.cs`, `Models/Order.cs`, `Models/OrderDetail.cs` — EF Core entity models in `WingtipToys.Models` namespace
- `Data/ProductContext.cs` — EF Core DbContext with seed data (5 categories, 16 products), uses SQLite
- `Services/CartStateService.cs` — in-memory cart state (scoped DI), tracks items/quantities/totals

**Files modified (14):**
- `WingtipToys.csproj` — NuGet ref → ProjectReference to BWFC + added EF Core SQLite 10.0.3
- `Program.cs` — added DbContextFactory, CartStateService, auth services, DB seed on startup
- `Components/App.razor` — added CSS links for bootstrap.css and Site.css from `/Content/`
- `_Imports.razor` — added usings for EntityFrameworkCore, WingtipToys.Models/Data/Services
- `Components/Layout/MainLayout.razor` — replaced broken master page with working layout: navbar, logo, category ListView from DB, removed broken `GetRouteUrl`/`LoginStatus`/`Page.Title` expressions
- `ProductList.razor` — replaced broken ListView (GetRouteUrl, .aspx links, TODO annotations) with working version using `Items="@_products"`, correct image/link paths
- `ProductList.razor.cs` — replaced Web Forms code-behind with ComponentBase + IDbContextFactory + SupplyParameterFromQuery
- `ProductDetails.razor` — replaced broken FormView (TODO, no Items) with working version using `Items="@_products"` + null guard
- `ProductDetails.razor.cs` — replaced Web Forms code-behind with ComponentBase + IDbContextFactory
- `AddToCart.razor` — replaced empty HTML shell with inline @code block using CartStateService
- `ShoppingCart.razor` — replaced broken GridView/Label/Button markup with clean foreach table using CartStateService
- `Default.razor` — replaced `@(Title)` with static text and added PageTitle
- `Default.razor.cs` — replaced Web Forms Page class with empty ComponentBase
- `About.razor`, `Contact.razor` — replaced `@(Title)` with static text

**Files deleted (3):**
- `AddToCart.razor.cs` — code now inline in .razor
- `ShoppingCart.razor.cs` — code now inline in .razor
- `Components/Layout/MainLayout.razor.cs` — code now inline in .razor via @code block

**Out-of-scope stubs fixed (26 files):** All Account/, Checkout/, Admin/, ViewSwitcher code-behind files replaced with empty `ComponentBase` partials. Several broken razor files (Manage, ManagePassword, TwoFactorAuthenticationSignIn, CheckoutReview, CheckoutComplete, CheckoutError, Register, ResetPassword, Forgot, ManageLogins, Login, VerifyPhoneNumber, OpenAuthProviders, RegisterExternalLogin, AddPhoneNumber, Confirm, Lockout, ResetPasswordConfirmation, AdminPage, ViewSwitcher) replaced with simple stub markup to eliminate compile errors from leftover Web Forms expressions.

**Build result:** 0 errors, 0 warnings. `dotnet build` clean.

**Key patterns used:**
- `IDbContextFactory<ProductContext>` injected via `[Inject]` for short-lived DbContext per operation
- `[SupplyParameterFromQuery]` for URL query string binding (replaces Web Forms `[QueryString]`)
- `CartStateService` as scoped service for in-memory cart state (replaces Web Forms Session-based cart)
- ListView `Items="@_products"` pattern for data binding (replaces SelectMethod)
- FormView with `RenderOuterTable="false"` for product details
- Inline `@code` blocks for simple pages (AddToCart, ShoppingCart, MainLayout)

### ShoppingCart BWFC Control Preservation Fix (2026-03-06)

**What:** Replaced plain HTML `<table>` + `@foreach` in ShoppingCart.razor with BWFC GridView in both Run8WingtipToys and AfterWingtipToys. Added `UpdateQuantity` method to both CartStateService implementations.

**Learnings:**
- ShoppingCart MUST use BWFC GridView, not HTML table — this is the core value proposition of the library
- AfterWingtipToys reference was wrong — it used plain HTML instead of BWFC components, causing the bug to propagate to Run 8
- All migration output should preserve Web Forms controls as BWFC equivalents (GridView, BoundField, TemplateField, TextBox, CheckBox, Button, Label)
- Run 7's ShoppingCart.razor is the gold standard pattern for data-bound BWFC pages with edit capabilities



 Team update (2026-03-05): BWFC control preservation is mandatory  all migration output must use BWFC components, never flatten to raw HTML. Cyclops's decision merged into consolidated block.  decided by Jeffrey T. Fritz, Forge, Cyclops
