# WingtipToys Migration Benchmark — CLI Tool (2026-04-20)

## 1. Executive Summary

This run benchmarks the **CLI tool** (`webforms-to-blazor migrate`) against the WingtipToys e-commerce sample (32 Web Forms files, 230 control usages). Previous runs (1–6) used the PowerShell script (`bwfc-migrate.ps1`); this is the first CLI-tool benchmark and tests the full L1 code-behind transformation pipeline with 18 markup + 16 code-behind transforms.

**Key results:**
- **Layer 0 scan**: 100% readiness score — all 230 controls covered by BWFC
- **Layer 1 CLI**: 32 files processed → 74 files written + 9 scaffold files in **23.6 seconds**, zero migration-tool errors
- **Layer 1 build**: 156 errors — all from missing business logic (Models/Logic not auto-copied) and identity namespace differences
- **After L2 fixes** (copy Models/Logic, add `using`): **72 errors across 14 of 32 code-behind files**; **18 files (56%) compile clean**
- **680 TODO markers** generated — structured migration guidance embedded in every code-behind

The CLI tool produces significantly richer output than the PS1 script — including scaffolded `Program.cs`, `WebFormsShims.cs`, `IdentityShims.cs`, `App.razor`, `Routes.razor`, `_Imports.razor`, and `appsettings.json` with connection strings extracted from `Web.config`. Code-behind files include comprehensive TODO markers categorized by migration concern.

## 2. Quick-Reference Metrics

| Metric | PS1 Run 6 (Mar 2026) | CLI Run (Apr 2026) |
|--------|----------------------|---------------------|
| Tool | `bwfc-migrate.ps1` | `webforms-to-blazor migrate` |
| Scan time | — | **0.97 s** |
| Migration time | 4.58 s | **23.6 s** (includes build of CLI) |
| Files processed | 32 | **32** |
| Files written | ~100+ | **74** (+ 9 scaffold) |
| Transforms applied | 269 | **18 markup + 16 code-behind** |
| Scaffold files | 6 | **9** |
| Static files copied | 79 | **0** (not yet implemented in CLI) |
| Build errors (L1 raw) | — | **156** |
| Build errors (after L2 copy) | — | **72** |
| Clean code-behinds | — | **18 of 32 (56%)** |
| TODO markers generated | ~30 | **680** |
| Manual review items | 28 | **1** (Web.config connection strings) |

## 3. What Worked Well

### 3.1 Layer 0 — Readiness Scan (0.97s)

The scan correctly identified all 230 control usages across 32 files and confirmed 100% BWFC coverage. Control inventory:

| Control | Count | | Control | Count |
|---------|-------|-|---------|-------|
| Label | 44 | | Button | 17 |
| Content | 27 | | TextBox | 22 |
| RequiredFieldValidator | 21 | | PlaceHolder | 15 |
| ScriptReference | 13 | | HyperLink | 9 |
| BoundField | 7 | | Literal | 7 |
| ValidationSummary | 7 | | TemplateField | 4 |
| CompareValidator | 4 | | ListView | 4 |
| DropDownList | 3 | | CheckBox | 3 |
| LinkButton | 3 | | GridView | 2 |
| HiddenField | 2 | | ContentPlaceHolder | 4 |

### 3.2 Layer 1 — CLI Automated Transforms

The CLI tool excels at:

1. **Markup conversion** — All 28 .aspx + 2 .ascx + 2 .master files converted to .razor with correct `@page` routes
2. **Code-behind generation** — Every page gets a `.razor.cs` partial class with:
   - Namespace preserved from original
   - Using directives mapped (System.Web → Microsoft.AspNetCore)
   - Lifecycle methods transformed (`Page_Load` → `OnInitializedAsync`)
   - IsPostBack guards unwrapped with explanatory comments
   - Event handlers preserved
3. **Scaffold generation** — 9 files auto-created:
   - `WingtipToys.csproj` (net10.0, BWFC reference, EF Core packages)
   - `Program.cs` (Razor components, BWFC services, commented-out DB/Identity/Session setup)
   - `_Imports.razor` (all standard Blazor + BWFC usings)
   - `Components/App.razor`, `Components/Routes.razor`
   - `GlobalUsings.cs`, `WebFormsShims.cs`, `IdentityShims.cs`
   - `appsettings.json` (with extracted connection strings)
   - `Properties/launchSettings.json`
4. **Master page → Layout** — `Site.Master` converted to `Site.razor` with `@inherits LayoutComponentBase` and `@Body`
5. **TODO markers** — 680 structured TODOs across 13 categories:

| Category | Count | Description |
|----------|-------|-------------|
| `bwfc-general` | 226 | General migration notes, ClientScript, UpdatePanel |
| `bwfc-lifecycle` | 92 | Page_Load → OnInitializedAsync guidance |
| `bwfc-form` | 64 | Request.Form → FormShim/WebFormsForm |
| `bwfc-config` | 64 | ConfigurationManager → BWFC shim |
| `bwfc-navigation` | 48 | Response.Redirect → ResponseShim |
| `bwfc-server` | 39 | Server.MapPath → ServerShim |
| `bwfc-session-state` | 37 | Session/Cache → SessionShim/CacheShim |
| `bwfc-datasource` | 33 | Data binding → component parameters |
| `bwfc-ispostback` | 33 | IsPostBack guard removal |
| `bwfc-viewstate` | 32 | ViewState → component state |
| `bwfc-select-method` | 8 | SelectMethod → service injection |
| `bwfc-master-page` | 2 | Head content extraction |
| `bwfc-identity` | 2 | Identity model migration |

### 3.3 Files That Compile Clean (18 of 32)

These code-behinds required zero additional work after L1:

| File | Controls |
|------|----------|
| About.razor.cs | Content |
| AddToCart.razor.cs | — |
| Contact.razor.cs | Content |
| CheckoutCancel.razor.cs | Content |
| CheckoutStart.razor.cs | Content |
| ErrorPage.razor.cs | Label, Panel |
| Account/AddPhoneNumber.razor.cs | Button, Label, TextBox, Validator |
| Account/Confirm.razor.cs | HyperLink, PlaceHolder |
| Account/Lockout.razor.cs | Content |
| Account/Login.razor.cs | Button, CheckBox, TextBox, HyperLink |
| Account/Register.razor.cs | Button, Label, TextBox, Validators |
| Account/RegisterExternalLogin.razor.cs | Button, Label, TextBox, Validator |
| Account/ResetPassword.razor.cs | Button, TextBox, Validators |
| Account/ResetPasswordConfirmation.razor.cs | HyperLink |
| Account/VerifyPhoneNumber.razor.cs | Button, HiddenField, TextBox |
| ViewSwitcher.razor.cs | — |
| Account/OpenAuthProviders.razor.cs | ListView |
| Site.Mobile.razor.cs | ContentPlaceHolder |

## 4. What Needs Help — Remaining Errors

### 4.1 Error Breakdown (72 errors across 14 files)

| Category | Errors | Root Cause | Fix Required |
|----------|--------|------------|--------------|
| **EF6 → EF Core** | 24 | `System.Data.Entity` namespace, `DbContext`/`DbSet<>` missing, `DropCreateDatabaseIfModelChanges<>` | Change `using System.Data.Entity` → `using Microsoft.EntityFrameworkCore`; replace `DropCreateDatabaseIfModelChanges` with `EnsureCreated()` or migrations |
| **ASP.NET Identity 2.0** | 18 | `ApplicationUserManager`, `ApplicationSignInManager`, `LoginCancelEventArgs`, `Microsoft.AspNet.Identity.*`, `Microsoft.Owin.*` | Rewrite Identity model from OWIN/Identity 2.0 → ASP.NET Core Identity |
| **Class name mismatch** | 10 | Razor filename → class name differs from code-behind: `Site`≠`SiteMaster`, `Default`≠`_Default`, `Forgot`≠`ForgotPassword`, `Site.Mobile`≠`Site_Mobile` | Rename classes in code-behind or rename .razor files |
| **RouteData ambiguity** | 4 | `Microsoft.AspNetCore.Components.RouteData` vs `Microsoft.AspNetCore.Routing.RouteData` | Add explicit `using` alias or fully qualify |
| **QueryString attribute** | 4 | `[QueryString]` used as parameter attribute (not an attribute class) | Replace with `[SupplyParameterFromQuery]` |
| **Missing Web Forms types** | 4 | `GridViewRow`, `ImageClickEventArgs` | Use BWFC type aliases or rewrite event handlers |

### 4.2 Files with Errors — Detail

| File | Errors | Primary Issue |
|------|--------|---------------|
| Site.razor.cs | 14 | Identity (ApplicationUserManager), class mismatch (SiteMaster≠Site), OWIN |
| ShoppingCart.razor.cs | 12 | QueryString attr, RouteData, GridViewRow, ImageClickEventArgs |
| ProductList.razor.cs | 8 | RouteData, QueryString attr |
| ProductDetails.razor.cs | 8 | RouteData, QueryString attr |
| Account/TwoFactorAuthenticationSignIn.razor.cs | 6 | ApplicationSignInManager, ApplicationUserManager |
| Models/IdentityModels.cs | 10 | Full Identity 2.0 model (Microsoft.AspNet.Identity.*) |
| Models/ProductContext.cs | 10 | System.Data.Entity namespace |
| Models/ProductDatabaseInitializer.cs | 4 | DropCreateDatabaseIfModelChanges, System.Data.Entity |
| Logic/RoleActions.cs | 4 | Microsoft.AspNet.Identity.* |
| Account/Manage.razor.cs | 4 | ApplicationUserManager |
| Account/ManageLogins.razor.cs | 4 | ApplicationUserManager |
| Account/ManagePassword.razor.cs | 4 | ApplicationUserManager |
| Default.razor.cs | 2 | Class mismatch (_Default≠Default) |
| Account/Forgot.razor.cs | 2 | Class mismatch (ForgotPassword≠Forgot) |

## 5. CLI Tool vs PS1 Script Comparison

| Aspect | PS1 Script | CLI Tool |
|--------|-----------|----------|
| **Speed** | Faster (~4.5s) | Slower (~23.6s, includes compilation) |
| **Code-behind handling** | Copies files, adds basic TODO headers | Full AST-level transforms (lifecycle, usings, events) |
| **Scaffold quality** | 6 files (basic) | 9 files (includes WebFormsShims.cs, IdentityShims.cs, appsettings.json) |
| **Static file copy** | ✅ 79 files to wwwroot/ | ❌ Not yet implemented |
| **TODO markers** | ~30 manual items | 680 structured TODOs across 13 categories |
| **Business logic copy** | Not copied | Not copied (by design — requires manual migration) |
| **Build success (L1)** | 0/0 after stubs | 72 errors (no stubs, real code) |
| **Identity handling** | Stub files generated | IdentityShims.cs + TODO guidance |
| **Best for** | Quick MVP, demo builds | Production migration with guided L2 path |

## 6. Recommendations

### Immediate CLI Tool Improvements

1. **Fix class name mismatches** (10 errors) — The CLI should generate code-behind class names matching the .razor filename, not the original Web Forms class name
2. **Add static file copy** — Mirror the PS1 script's wwwroot/ copy behavior
3. **Copy Models/Logic** — Business logic files should be copied (not transformed) to the output
4. **Handle EF6 namespace** — Auto-replace `System.Data.Entity` → `Microsoft.EntityFrameworkCore` in copied .cs files

### Layer 2 Work Remaining (Architectural)

1. **Identity migration** — Rewrite `IdentityModels.cs` from OWIN/Identity 2.0 to ASP.NET Core Identity (~2 hours)
2. **EF Core migration** — Update `ProductContext` to EF Core, replace `DropCreateDatabaseIfModelChanges` with migrations (~30 min)
3. **RouteData disambiguation** — Add explicit using aliases in pages that use routing (~5 min)
4. **QueryString → SupplyParameterFromQuery** — Attribute replacement (~5 min)
5. **Data binding wiring** — Connect `SelectMethod` references to service injection (~1 hour)

### Estimated Total Migration Time

| Phase | Time | Notes |
|-------|------|-------|
| L0 Scan | 1 sec | Automated |
| L1 CLI | 24 sec | Automated |
| L2 Mechanical fixes | ~15 min | Class names, EF namespace, RouteData, QueryString |
| L2 Identity rewrite | ~2 hours | Manual architectural work |
| L2 Data binding | ~1 hour | Service injection, SelectMethod wiring |
| L3 Testing & polish | ~2 hours | Runtime verification, CSS, edge cases |
| **Total** | **~5.5 hours** | For 32-page e-commerce app |

## 7. Raw Data

- Scan output: 230 controls, 32 files, 100% coverage
- CLI output: 74 files written, 9 scaffold, 0 migration errors, 1 manual item
- L1 build: 156 errors → 72 after copying business logic
- TODO markers: 680 across 13 categories
- Clean code-behinds: 18 of 32 (56%)
- Previous runs: 6 runs with PS1 script (March 2026), best result was 0/0 errors in ~4.5 minutes
