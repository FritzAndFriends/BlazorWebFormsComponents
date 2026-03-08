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

### Run 11 — Complete WingtipToys Migration from Scratch (2026-03-07)

**Completed:** Full fresh migration of WingtipToys from Web Forms to Blazor Server. Built from scratch (no reference to FreshWingtipToys). 0 errors, 0 warnings.

**Approach:**
1. Created fresh `dotnet new blazor --interactivity Server --framework net10.0` project
2. Added BWFC ProjectReference + EF Core/Identity NuGet packages
3. Ran `bwfc-migrate.ps1` to temp dir, cherry-picked converted .razor pages
4. Copied static content (CSS, images, fonts, favicon) from original source preserving paths
5. Built all Layer 2 content from scratch: Models, Data, Services, Program.cs, MainLayout, all code-behinds

**Key decisions & patterns:**
- Root-level `_Imports.razor` needed for pages outside `Components/` — the `Components/_Imports.razor` only applies within that folder. Both files must have identical usings + `@inherits WebFormsPageBase`.
- Code-behind partial classes must NOT specify `: ComponentBase` when `_Imports.razor` has `@inherits WebFormsPageBase` — causes CS0263 (different base classes in partial declarations).
- `@rendermode InteractiveServer` as a standalone directive in .razor files works for pages that need interactivity (ShoppingCart, AddToCart, AdminPage, Checkout pages). The `@using static RenderMode` in `_Imports.razor` enables the shorthand.
- Auth pages (Login, Register) use plain HTML forms posting to HTTP endpoints — SignInManager needs HTTP context, not SignalR.
- MainLayout inherits `LayoutComponentBase` (overrides `_Imports.razor` `@inherits`) and uses `<BlazorWebFormsComponents.Page />` for head rendering, `<LoginView>` with `<AnonymousTemplate>`/`<LoggedInTemplate>`, and code-based category list.
- Category.Description set to `string?` — seed data doesn't populate it.
- Product.UnitPrice converted from `double?` to `decimal?` for currency precision.
- CartStateService uses cookie-based cart ID instead of Session.
- Image paths preserved from source: `/Catalog/Images/Thumbs/` for list, `/Catalog/Images/` for details.

**File count:** 105 total (27 .razor, 23 .cs, 38 .png images, 5 .css, plus fonts/config)
**Build result:** 0 errors, 0 warnings

### Run 11 Script Fixes — Fix 1 & Fix 2 (2026-03-07)

**Fix 1: Scripts/ folder detection and copy (`Invoke-ScriptAutoDetection`)**

Added `Invoke-ScriptAutoDetection` function to `migration-toolkit/scripts/bwfc-migrate.ps1` (parallel to existing `Invoke-CssAutoDetection`). The function:
- Scans source project for `Scripts/` folder
- Filters out WebForms-specific JS (`*intellisense*`, `_references.js`, `WebForms/` subdir)
- Copies relevant JS files to `wwwroot/Scripts/` in output
- Injects `<script>` tags into App.razor before `</body>` (after `blazor.web.js`)
- Orders scripts correctly: jQuery → Modernizr → Respond → Bootstrap → remaining
- Prefers `.min.js` variants when both exist
- Scans `Site.Master` for `<webopt:bundlereference>` targeting Scripts and flags as `ScriptBundle` manual item
- Called from Entry Point section, right after `Invoke-CssAutoDetection`

**Fix 2: Convert ListView/DataPager placeholder elements to `@context` (`Convert-TemplatePlaceholders`)**

Added `Convert-TemplatePlaceholders` function in new `#region --- Template Placeholder Conversion (Fix 2) ---`. The function:
- Finds elements whose `id` attribute contains "Placeholder" (case-insensitive)
- Replaces self-closing tags: `<\w+\s+[^>]*?id\s*=\s*"[^"]*[Pp]laceholder[^"]*"[^>]*/>`
- Replaces open+close tags with whitespace-only content: `<(\w+)\s+[^>]*?id\s*=\s*"[^"]*[Pp]laceholder[^"]*"[^>]*>\s*</\1>`
- Both patterns replace with `@context`
- Called in `Convert-WebFormsFile` pipeline AFTER `ConvertFrom-UrlReferences` and BEFORE blank line cleanup
- Container elements (e.g., `itemPlaceholderContainer`) are preserved because they contain non-whitespace content after inner placeholder replacement

**Test results (WingtipToys):**
- ✅ `wwwroot/Scripts/` created with 7 filtered JS files (9 total including those from general static copy)
- ✅ App.razor contains 7 `<script>` tags in correct dependency order before `</body>`
- ✅ `ProductList.razor` line 25: `<td id="itemPlaceholder"></td>` → `@context` (inside GroupTemplate)
- ✅ `ProductList.razor` line 69: `<tr id="groupPlaceholder"></tr>` → `@context` (inside LayoutTemplate)
- ✅ Container elements (`itemPlaceholderContainer`, `groupPlaceholderContainer`) preserved correctly
- ✅ Migration ran clean: 32 files processed, 303 transforms, 79 static files copied


### Run 12 — Complete WingtipToys Migration from Scratch (2026-03-07)

**Completed:** Full migration of WingtipToys using bwfc-migrate.ps1 + Layer 2 manual fixes. Build: 0 errors, 0 warnings.

**Migration script output:**
- 32 files processed, 303 transforms, 79 static files, 8 model files copied
- Script correctly handled: JS files → wwwroot/Scripts/, CSS auto-detection, placeholder→@context conversion

**Layer 2 fixes applied:**
1. **csproj:** Changed BWFC from NuGet PackageReference to ProjectReference (`..\..\src\BlazorWebFormsComponents\BlazorWebFormsComponents.csproj`); removed Identity.UI package (not needed with manual auth)
2. **_Imports.razor:** Added `@inherits WebFormsPageBase`, model/data/services usings, `BlazorWebFormsComponents.Enums`
3. **Models:** Product.UnitPrice `double?`→`decimal?`, OrderDetail.UnitPrice same, ProductContext→`IdentityDbContext<IdentityUser>`, nullable reference types, file-scoped namespaces
4. **ProductDatabaseInitializer:** Rewrote as static `Seed(ProductContext)` method — EF Core pattern (check `if (context.Categories.Any()) return;`)
5. **IdentityModels.cs:** Gutted — using `IdentityUser` directly, no custom ApplicationUser
6. **Program.cs:** Full pipeline — `AddDbContextFactory<ProductContext>` + `AddDbContext` (dual registration for DI factory + scoped), `AddIdentity<IdentityUser, IdentityRole>`, `ConfigureApplicationCookie`, `AddCascadingAuthenticationState`, `MapPost` endpoints for register/login/logout handlers
7. **App.razor:** Deduplicated CSS (kept only `.min.css` variants), removed non-min JS duplicates
8. **MainLayout:** Rewrote entirely — categories via `IDbContextFactory`, `LoginView` with `AnonymousTemplate`/`LoggedInTemplate`, plain HTML logout form, `_userName` from `CascadingParameter Task<AuthenticationState>`, plain `<img>` instead of BWFC `<Image>` for logo
9. **Default.razor (RC-8):** Added featured products grid with `IDbContextFactory`, `role="main"` container, product images/links/prices — ensures >100px height
10. **ProductList.razor:** Changed `TItem` to `ItemType`, fixed GetRouteUrl links to `/ProductDetails?ProductID=@context.ProductID`, fixed AddToCart links from `.aspx` to `/AddToCart?productID=`, wired up `IDbContextFactory` data loading with category filter via `[SupplyParameterFromQuery]`
11. **ProductDetails.razor (RC-10):** Rewrote as direct data binding (not FormView), added "Add To Cart" link (`/AddToCart?productID=@_product.ProductID`), `IDbContextFactory` data loading
12. **ShoppingCart.razor:** Implemented full GridView equivalent with HTML table, `@rendermode InteractiveServer`, quantity update via `@onchange`, remove button, order total calculation, `CartStateService` integration
13. **AddToCart.razor:** Cart service integration — adds item and redirects to ShoppingCart via `NavigationManager`
14. **Register/Login (RC-11):** Plain HTML `<form method="post" action="/account/*-handler" data-enhance="false">` for full page reloads. No BWFC components. Error display via `[SupplyParameterFromQuery]`.
15. **All remaining Account/Admin/Checkout pages:** Stubbed as simple HTML placeholders to avoid build errors from old WebForms code-behinds
16. **All code-behinds:** Replaced old WebForms `System.Web.UI` code with stubs (pages use inline `@code` blocks)

**Key patterns established:**
- Dual DbContext registration: `AddDbContextFactory` for `IDbContextFactory<T>` (short-lived queries in components) + `AddDbContext` for Identity (which needs scoped)
- Auth forms use `data-enhance="false"` to force full HTTP POST + page reload — this ensures the Blazor circuit picks up the new cookie auth state
- `CascadingParameter Task<AuthenticationState>` in MainLayout for username display
- `LoginView` templates are `RenderFragment` (not generic) — no `@context` inside them; use cascading auth state instead
- `CartStateService` with `IDbContextFactory` and cookie-based cart ID — avoids Session dependency
- `[SupplyParameterFromQuery]` on page components for query string parameters (replaces WebForms Request.QueryString)

**File count:** 35 .razor, 42 .cs, 79 static assets

 Team update (2026-03-07): Coordinator must not perform domain work  all code changes must route through specialist agents  decided by Jeffrey T. Fritz, Beast
 Team update (2026-03-07): FreshWingtipToys must not be committed or referenced as template  decided by Jeffrey T. Fritz
 Team update (2026-03-07): migration-standards SKILL.md updated with Static Asset Checklist, ListView Placeholder Conversion, Preserving Action Links  decided by Beast
 Team update (2026-03-07): Migration order directive  fresh Blazor project first, then apply BWFC, then migrate content  decided by Jeffrey T. Fritz

### LoginView Namespace Fix (2026-03-07)

**Problem:** AfterWingtipToys had 3 RZ10012 warnings — Razor compiler didn't recognize `<LoginView>`, `<AnonymousTemplate>`, or `<LoggedInTemplate>` as components.

**Root cause:** `LoginView` lives in `BlazorWebFormsComponents.LoginControls` namespace. The sample's `_Imports.razor` had `@using BlazorWebFormsComponents` but was missing `@using BlazorWebFormsComponents.LoginControls`. The migration script (`bwfc-migrate.ps1`) also omitted this using from its generated `_Imports.razor` template.

**Fixes applied:**
1. `samples/AfterWingtipToys/_Imports.razor` — added `@using BlazorWebFormsComponents.LoginControls` (line 10)
2. `migration-toolkit/scripts/bwfc-migrate.ps1` — added `@using BlazorWebFormsComponents.LoginControls` to `$importsContent` template (line 187)

**Result:** AfterWingtipToys warnings dropped from 73 → 70 (3 RZ10012s eliminated). BWFC test project unaffected.

 Team update (2026-03-08): Default to SSR (Static Server Rendering) with per-component InteractiveServer opt-in; eliminates HttpContext/cookie/session problems  decided by Forge

 Team update (2026-03-08): Run 12 migration patterns: auth via plain HTML forms with data-enhance=false, dual DbContext, LoginView _userName from cascading auth state  decided by Cyclops
