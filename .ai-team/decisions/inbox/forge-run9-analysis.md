### 2026-03-06: Run 9 preparation — post-mortem analysis of Run 8
**By:** Forge
**What:** Analyzed Run 8 migration results and identified 22 improvements for Run 9
**Why:** Each fix reduces manual Layer 2 work, making the migration more automated

---

## Run 8 Post-Mortem Analysis

### Summary

Run 8 achieved 14/14 acceptance tests in 1h 55m. Layer 1 (script) completed 366 transforms in 3.3s. Layer 2 (manual) took ~26 minutes plus ~1h 20m of test-fix iteration. The biggest time sink was the HTTP session / Interactive Server incompatibility requiring 6 minimal API endpoints and the onclick workaround — architectural issues the skills didn't warn about. The second biggest time sink was creating Models, Data, and Services from scratch when the original source has perfectly good models that could be auto-copied.

### Methodology

Compared every file in `samples/WingtipToys/WingtipToys/` against its equivalent in `samples/AfterWingtipToys/`. Read the full Run 8 REPORT.md including all phases, build issues, test iteration rounds, architecture decisions, and known limitations. Inspected the migration script (`bwfc-migrate.ps1`) transform pipeline and all 4 migration skill files for gaps.

---

## Prioritized Fix List

### P0 — Blocks Migration / Major Architectural Gap

---

#### RF-01: Skill — HTTP Session + Interactive Server Warning
**Category:** Skill fix
**Priority:** P0
**Description:** The `bwfc-identity-migration` and `bwfc-data-migration` skills do not warn that **all session-dependent operations fail silently under Blazor Interactive Server mode** because `HttpContext` is null during WebSocket circuits. Run 8 burned ~1h 20m in Phase 3 discovering this. The skills need a prominent "⚠️ Session-Dependent Operations" section explaining:
- Cookie auth (login/register) MUST use HTML form POST to minimal API endpoints
- Session-based cart operations MUST use minimal API endpoints
- Any `IHttpContextAccessor` usage is null during WebSocket rendering
- Pattern: `<form method="post" action="/endpoint">` + `app.MapPost(...).DisableAntiforgery()`
**Effort:** M
**Files affected:**
- `migration-toolkit/skills/bwfc-identity-migration/SKILL.md` (add "Cookie Auth Requires HTTP Endpoints" section — partially exists at line 326 but is buried and incomplete)
- `migration-toolkit/skills/bwfc-data-migration/SKILL.md` (add "Session State Under Interactive Server" section)
- `migration-toolkit/skills/migration-standards/SKILL.md` (add architectural pattern)

---

#### RF-02: Skill — Minimal API Endpoint Templates for Auth
**Category:** Skill fix
**Priority:** P0
**Description:** Run 8 required 5 minimal API endpoints (LoginHandler, RegisterHandler, Logout, AddToCart, Cart/Update, Cart/Remove). The identity skill should include **copy-paste-ready** Program.cs endpoint templates for Login, Register, and Logout. Currently the skill shows a single logout example (line 329) but not login or register. Layer 2 had to figure out the full pattern from scratch.
**Effort:** M
**Files affected:**
- `migration-toolkit/skills/bwfc-identity-migration/SKILL.md` (add complete Login/Register/Logout endpoint templates)

---

#### RF-03: Script — Auto-copy Models directory
**Category:** Script fix
**Priority:** P0
**Description:** Run 8 had to manually create `Models/Category.cs`, `Models/Product.cs`, `Models/CartItem.cs` — all three already existed in the original source at `Models/`. The migration script should detect and copy `Models/` directory files (`.cs` only), stripping EF6 namespaces (`using System.Data.Entity;`) and adding a TODO header. The original WingtipToys models use `System.ComponentModel.DataAnnotations` which works unchanged in .NET 10.
**Effort:** M
**Files affected:**
- `migration-toolkit/scripts/bwfc-migrate.ps1` (add Models directory copy logic after static files copy, ~line 1127)

---

### P1 — Significant Manual Work Reduction

---

#### RF-04: Script — Auto-copy and transform DbContext
**Category:** Script fix
**Priority:** P1
**Description:** The original `Models/ProductContext.cs` uses `System.Data.Entity.DbContext`. The script should copy it and: (a) replace `using System.Data.Entity;` with `using Microsoft.EntityFrameworkCore;`, (b) remove constructor with connection string name, (c) add `DbContextOptions` constructor, (d) flag with TODO for Identity integration. Run 8 manually created `Data/ProductContext.cs` from scratch.
**Effort:** M
**Files affected:**
- `migration-toolkit/scripts/bwfc-migrate.ps1` (add DbContext detection and transform in Models copy)

---

#### RF-05: Script — Detect LoginView and preserve as BWFC component
**Category:** Script fix
**Priority:** P1
**Description:** The Run 8 report shows `<asp:LoginView>` in Site.Master was manually converted to `<AuthorizeView>` with `<NotAuthorized>`/`<Authorized>` (see REPORT.md line 172-187). But per team decision, **LoginView is a native BWFC component** — the script already converts it correctly via `ConvertFrom-LoginView` (line 630-658). The Layer 2 agent overrode this and replaced it with `AuthorizeView`. The actual MainLayout.razor in the output does use `<LoginView>` correctly (line 28-41). The issue is that the **report's before/after example shows AuthorizeView**, which is misleading. This is a doc fix in the report template, plus a skill reinforcement.
**Effort:** S
**Files affected:**
- `migration-toolkit/skills/bwfc-identity-migration/SKILL.md` (add bold warning: "Do NOT replace LoginView with AuthorizeView — LoginView IS a BWFC component")
- `migration-toolkit/skills/migration-standards/SKILL.md` (reinforce LoginView preservation)

---

#### RF-06: Script — Generate EF Core package references in .csproj
**Category:** Script fix
**Priority:** P1
**Description:** The scaffolded `WingtipToys.csproj` only includes `Fritz.BlazorWebFormsComponents`. Run 8 had to manually add 5 NuGet packages (Identity.UI, EF Core Sqlite, Identity.EFC, EF Tools, Diagnostics.EFC). The script should detect the presence of `Models/` or `*.Models.*` files and automatically add common EF Core + Identity packages to the .csproj scaffold. Use the team-mandated EF Core version standard (latest .NET 10).
**Effort:** S
**Files affected:**
- `migration-toolkit/scripts/bwfc-migrate.ps1` (modify `New-ProjectScaffold`, ~line 128-148, to conditionally add EF Core/Identity packages)

---

#### RF-07: Script — Scaffold Program.cs with Identity and Session boilerplate
**Category:** Script fix
**Priority:** P1
**Description:** The generated Program.cs (line 166-192) is minimal. Run 8 manually added: `AddHttpContextAccessor()`, `AddDbContext()`, `AddDefaultIdentity()`, `AddDistributedMemoryCache()`, `AddSession()`, `AddCascadingAuthenticationState()`, `UseSession()`, `UseAuthentication()`, `UseAuthorization()`, and DB seed logic. When the script detects Identity-related files in the source (Login.aspx, Register.aspx, IdentityModels.cs), it should scaffold a more complete Program.cs with these registrations as TODO-annotated blocks.
**Effort:** M
**Files affected:**
- `migration-toolkit/scripts/bwfc-migrate.ps1` (enhance Program.cs scaffold with conditional Identity/Session blocks)

---

#### RF-08: Script — Convert AddToCart-style redirect pages to minimal API endpoints
**Category:** Script fix
**Priority:** P1
**Description:** `AddToCart.aspx` is a "headless" page — no UI, just `Page_Load` that reads a query string, performs an action, and `Response.Redirect`s. The script already detects it as unconvertible (it matches `Session\[` pattern in `Test-UnconvertiblePage`), but generates a dead stub. Instead, the script should detect the pattern (no Content/HTML, only code-behind with `Response.Redirect`) and generate a TODO comment in Program.cs: `// TODO: Convert AddToCart to a minimal API endpoint: app.MapGet("/AddToCart", ...)`. The stub page can remain but should note "this page was a redirect handler — see Program.cs for the minimal API endpoint".
**Effort:** M
**Files affected:**
- `migration-toolkit/scripts/bwfc-migrate.ps1` (enhance `Test-UnconvertiblePage` and `New-CompilableStub` to detect redirect-only pages)

---

#### RF-09: Skill — Enhanced Navigation Interception Warning
**Category:** Skill fix
**Priority:** P1
**Description:** Run 8 discovered that Blazor's enhanced navigation intercepts `<a>` tag clicks, preventing links from hitting server endpoints. The workaround was `onclick="window.location.href=this.href; return false;"`. The migration skill should document this pattern and when to use it: any `<a href>` that targets a minimal API endpoint (not a Blazor page) needs either (a) the onclick workaround, or (b) `data-enhance-nav="false"` attribute. This should be in the data-migration skill under the session/HTTP patterns.
**Effort:** S
**Files affected:**
- `migration-toolkit/skills/bwfc-data-migration/SKILL.md` (add "Blazor Enhanced Navigation" section)
- `migration-toolkit/skills/migration-standards/SKILL.md` (add pattern)

---

#### RF-10: Script — Extract Page Title from <%@ Page %> directive
**Category:** Script fix
**Priority:** P1
**Description:** Every `.aspx` file has `Title="Product Details"` in its `<%@ Page %>` directive. The script strips the entire directive and replaces with `@page "/route"`. It should also extract the Title value and generate `Page.Title = "Product Details";` in the code-behind stub, or add `@{ Page.Title = "Product Details"; }` to the top of the .razor file. Run 8 lost all page titles — the `<h2><%: Page.Title %></h2>` expressions on ProductList.aspx (line 8) became empty.
**Effort:** S
**Files affected:**
- `migration-toolkit/scripts/bwfc-migrate.ps1` (`ConvertFrom-PageDirective`, ~line 299-314)

---

#### RF-11: Script — Detect and flag `GetRouteUrl` patterns with inline href
**Category:** Script fix
**Priority:** P1
**Description:** ProductList.aspx uses `href="<%#: GetRouteUrl("ProductByNameRoute", ...) %>"` inside `<a>` tags within data-bound templates. The script converts `GetRouteUrl` calls to `GetRouteUrlHelper.GetRouteUrl(...)` but the expression is inside a data-binding context (`<%#: ... %>`) embedded in an HTML attribute. The converted output still needs significant manual rewriting because the route names ("ProductByNameRoute") don't exist in Blazor — they need to be replaced with direct URL patterns like `/ProductDetails?ProductID=@Item.ProductID`. The script should flag these specifically and suggest the replacement pattern.
**Effort:** S
**Files affected:**
- `migration-toolkit/scripts/bwfc-migrate.ps1` (`ConvertFrom-GetRouteUrl`, ~line 664-706)

---

#### RF-12: Script — Handle `[QueryString]` and `[RouteData]` parameter attributes
**Category:** Script fix
**Priority:** P1
**Description:** The original code-behind files use `[QueryString("id")] int? categoryId` and `[RouteData] string categoryName` parameter attributes on SelectMethod methods. These are Web Forms model binding attributes. The code-behind copy function should annotate these with TODO comments explaining the Blazor equivalent: `[SupplyParameterFromQuery(Name = "id")]` for query strings, `[Parameter]` for route data. Run 8 had to manually figure out and add `[SupplyParameterFromQuery]` on every page.
**Effort:** S
**Files affected:**
- `migration-toolkit/scripts/bwfc-migrate.ps1` (`Copy-CodeBehind`, ~line 831-873, add regex detection for `[QueryString]` and `[RouteData]`)

---

#### RF-13: Script — Convert ListView with GroupItemCount to manual loop pattern
**Category:** Script fix / Skill fix
**Priority:** P1
**Description:** ProductList.aspx uses `<asp:ListView GroupItemCount="4">` with `<GroupTemplate>`, `<LayoutTemplate>`, and `<ItemTemplate>` to create a 4-column grid. The migrated output expanded this into nested `@for` loops (ProductList.razor lines 12-56). The BWFC ListView component supports `GroupItemCount` and these templates natively, but Run 8 did not use the BWFC component — it inlined the loops. Either (a) the skill should show how to use `<ListView GroupItemCount="4" Items="products" TItem="Product">` with templates, or (b) the script should flag this pattern for Layer 2 with a clear TODO and the recommended approach.
**Effort:** M
**Files affected:**
- `migration-toolkit/skills/bwfc-migration/SKILL.md` (add ListView with GroupItemCount example)
- `migration-toolkit/scripts/bwfc-migrate.ps1` (improve SelectMethod TODO to include BWFC component usage hint)

---

#### RF-14: Skill — DisableAntiforgery() requirement for form POST endpoints
**Category:** Skill fix
**Priority:** P1
**Description:** Run 8 required `.DisableAntiforgery()` on every POST endpoint because Blazor-rendered HTML forms don't include antiforgery tokens. This is mentioned in REPORT.md line 477 but NOT documented in any skill. The identity and data migration skills should document this requirement whenever suggesting `<form method="post">` patterns.
**Effort:** S
**Files affected:**
- `migration-toolkit/skills/bwfc-identity-migration/SKILL.md` (add note on DisableAntiforgery)
- `migration-toolkit/skills/bwfc-data-migration/SKILL.md` (add note on DisableAntiforgery)

---

### P2 — Nice to Have / Polish

---

#### RF-15: Script — Detect and skip .designer.cs files
**Category:** Script fix
**Priority:** P2
**Description:** The original source contains `.designer.cs` files for every `.aspx`, `.ascx`, and `.master` file. These are auto-generated by Visual Studio and contain control field declarations (`protected global::System.Web.UI.WebControls.Button UpdateBtn;`). They have zero value in Blazor. The script should skip these entirely rather than copying them. Currently the code-behind handler only looks for `.cs` and `.vb` suffixes but `.designer.cs` files still exist in the source tree.
**Effort:** S
**Files affected:**
- `migration-toolkit/scripts/bwfc-migrate.ps1` (`Copy-CodeBehind` and file discovery, add `.designer.cs` exclusion)

---

#### RF-16: Script — Generate _Imports.razor with WebFormsPageBase and LoginControls using
**Category:** Script fix
**Priority:** P2
**Description:** The scaffolded `_Imports.razor` (line 152-163) does not include `@inherits BlazorWebFormsComponents.WebFormsPageBase` or `@using BlazorWebFormsComponents.LoginControls`. Run 8's final `_Imports.razor` has both (lines 9, 15). Since WebFormsPageBase is now the standard per migration-standards SKILL.md, the scaffold should include it. It also needs `@using Microsoft.AspNetCore.Components.Authorization` for `<AuthorizeView>` and auth-related components.
**Effort:** S
**Files affected:**
- `migration-toolkit/scripts/bwfc-migrate.ps1` (`New-ProjectScaffold`, ~line 152-163)

---

#### RF-17: Script — Copy Content/CSS files to wwwroot preserving paths
**Category:** Script fix
**Priority:** P2
**Description:** The static file copy logic (line 1127-1151) copies all static files into `wwwroot/` preserving their relative paths from the source root. This works for most files but the original WingtipToys has CSS in `Content/` (Bootstrap) which the App.razor references as `Content/bootstrap.min.css`. The script should handle `BundleConfig`/`Content/` patterns by copying to wwwroot and generating the correct `<link>` references in App.razor's `<head>`. Currently this is all manual.
**Effort:** M
**Files affected:**
- `migration-toolkit/scripts/bwfc-migrate.ps1` (enhance static file copy + App.razor scaffold)

---

#### RF-18: Doc — Update CONTROL-COVERAGE.md with session/auth limitations
**Category:** Doc fix
**Priority:** P2
**Description:** CONTROL-COVERAGE.md lists control coverage but doesn't mention architectural limitations. Add a "Migration Gotchas" section documenting: (1) Session-dependent operations need minimal API endpoints, (2) Login/Register forms need HTTP POST pattern, (3) Enhanced navigation interception on `<a>` tags, (4) DisableAntiforgery for Blazor-rendered forms. These are universal issues anyone migrating will hit.
**Effort:** S
**Files affected:**
- `migration-toolkit/CONTROL-COVERAGE.md` (add "Common Migration Gotchas" section)

---

#### RF-19: Skill — ShoppingCart/Session-Based Service Pattern
**Category:** Skill fix
**Priority:** P2
**Description:** Run 8 created `Services/ShoppingCartService.cs` from scratch. The data migration skill mentions scoped services (line 156-182) but doesn't show a complete session-based cart pattern using `IHttpContextAccessor`. Add a complete example showing: (a) the service class, (b) DI registration with `AddSession()` + `AddDistributedMemoryCache()`, (c) how `GetCartId()` works via session, (d) the limitation that it only works from HTTP pipeline (not WebSocket).
**Effort:** M
**Files affected:**
- `migration-toolkit/skills/bwfc-data-migration/SKILL.md` (add "Session-Based Service" complete example)

---

#### RF-20: Script — Detect `Logic/` directory and flag for service layer creation
**Category:** Script fix
**Priority:** P2
**Description:** WingtipToys has a `Logic/` directory containing business logic classes (`ShoppingCartActions.cs`, `AddProducts.cs`, etc.). The script doesn't scan this directory at all. It should copy `.cs` files from `Logic/` (and similar patterns like `Services/`, `Helpers/`, `BusinessLogic/`) with TODO annotations, or at minimum flag their existence in the manual review items list so Layer 2 knows they need service layer migration.
**Effort:** S
**Files affected:**
- `migration-toolkit/scripts/bwfc-migrate.ps1` (add business logic directory scanning after Models copy)

---

#### RF-21: Script — Handle `webopt:bundlereference` and `Scripts.Render` removal
**Category:** Script fix
**Priority:** P2
**Description:** Site.Master line 14 has `<webopt:bundlereference runat="server" path="~/Content/css" />` and line 12 has `<%: Scripts.Render("~/bundles/modernizr") %>`. The script converts the expressions via `ConvertFrom-Expressions` but `webopt:` is not an `asp:` prefix so it's not stripped. The script should remove `<webopt:bundlereference>` tags entirely and replace `Scripts.Render()` / `Styles.Render()` calls with a TODO comment listing the bundles that need manual `<link>` / `<script>` tags.
**Effort:** S
**Files affected:**
- `migration-toolkit/scripts/bwfc-migrate.ps1` (add `webopt:` tag stripping and bundle reference TODOs)

---

#### RF-22: Doc — Update component count in migration skill
**Category:** Doc fix
**Priority:** P2
**Description:** The `bwfc-migration/SKILL.md` still says "58 components across 6 categories" (line 20). Per the library audit, the correct count is "58 primary + 95 supporting (153 total Razor components)" across 9 categories. Update the skill to match CONTROL-COVERAGE.md.
**Effort:** S
**Files affected:**
- `migration-toolkit/skills/bwfc-migration/SKILL.md` (line 20)

---

## Summary Table

| ID | Category | Priority | Description | Effort |
|----|----------|----------|-------------|--------|
| RF-01 | Skill | P0 | HTTP Session + Interactive Server warning | M |
| RF-02 | Skill | P0 | Minimal API endpoint templates for auth | M |
| RF-03 | Script | P0 | Auto-copy Models directory | M |
| RF-04 | Script | P1 | Auto-copy and transform DbContext | M |
| RF-05 | Script + Skill | P1 | LoginView preservation reinforcement | S |
| RF-06 | Script | P1 | Generate EF Core + Identity package refs in .csproj | S |
| RF-07 | Script | P1 | Scaffold Program.cs with Identity/Session boilerplate | M |
| RF-08 | Script | P1 | Convert redirect-only pages to minimal API TODOs | M |
| RF-09 | Skill | P1 | Enhanced navigation interception warning | S |
| RF-10 | Script | P1 | Extract Page Title from directive | S |
| RF-11 | Script | P1 | Flag GetRouteUrl patterns with replacement hint | S |
| RF-12 | Script | P1 | Handle [QueryString]/[RouteData] annotations | S |
| RF-13 | Script + Skill | P1 | ListView GroupItemCount pattern | M |
| RF-14 | Skill | P1 | DisableAntiforgery() requirement | S |
| RF-15 | Script | P2 | Skip .designer.cs files | S |
| RF-16 | Script | P2 | _Imports.razor with WebFormsPageBase | S |
| RF-17 | Script | P2 | CSS bundle handling in static copy | M |
| RF-18 | Doc | P2 | CONTROL-COVERAGE.md gotchas section | S |
| RF-19 | Skill | P2 | Complete session-based service pattern | M |
| RF-20 | Script | P2 | Detect Logic/ directory for services | S |
| RF-21 | Script | P2 | Handle webopt:bundlereference removal | S |
| RF-22 | Doc | P2 | Update component count in skill | S |

### Estimated Impact

If all P0 + P1 fixes (14 items) are implemented, the projected Run 9 improvement:
- **Phase 2 (manual):** ~26 min → ~10 min (Models auto-copied, Program.cs pre-scaffolded, packages pre-added, page titles preserved)
- **Phase 3 (test/fix):** ~1h 20m → ~30 min (session/auth patterns documented upfront, minimal API templates available, enhanced nav workaround known)
- **Projected total:** ~1h 55m → ~50-60 min

### Category Breakdown

| Category | Count | Priority Split |
|----------|-------|---------------|
| Script fix | 13 | 1 P0, 8 P1, 4 P2 |
| Skill fix | 7 | 2 P0, 3 P1, 1 P2 |
| Doc fix | 2 | 0 P0, 0 P1, 2 P2 |
