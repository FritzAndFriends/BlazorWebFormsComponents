# Run 9 — WingtipToys Migration Benchmark

**Date:** 2025-07-25
**Agent:** Bishop (Migration Tooling Dev)
**Source:** `samples/WingtipToys/WingtipToys` (ASP.NET Web Forms 4.5)
**Output:** `samples/Run9WingtipToys/` (Blazor Server .NET 9)

---

## Pipeline Timing

| Phase | Duration | Notes |
|-------|----------|-------|
| Layer 0 — Scan/Assessment | 0.66 s | bwfc-scan.ps1, Console format |
| Layer 1 — Automated Transforms | 4.49 s | bwfc-migrate.ps1, 667 operations |
| Layer 2 — Structural Transforms | ~45 min | Copilot-assisted (models, data, services, page rewrites) |
| Build Verification | 6.13 s | Final successful build (7 attempts total) |

## Layer 0 — Assessment

- **Readiness Score:** 100%
- **Controls Covered:** 230 / 230
- **Source Files:** 32 (28 .aspx, 2 .ascx, 2 .master)

## Layer 1 — Automated Transforms

- **Files Transformed:** 32
- **Transform Operations:** 667
- **Control Preservation Warnings:** 2 (Site.Master PlaceHolder)
- **Output Files:** 35 .razor, 33 .cs, 79 wwwroot assets, scaffold (csproj, Program.cs, _Imports.razor, App.razor, Routes.razor)

## Layer 2 — Structural Transforms

### Files Created
| File | Purpose |
|------|---------|
| Models/Product.cs | Product entity |
| Models/Category.cs | Category entity |
| Models/CartItem.cs | Cart item entity |
| Models/Order.cs | Order entity |
| Models/OrderDetail.cs | Order detail entity |
| Data/ProductContext.cs | EF Core DbContext + SQLite + seed data (5 categories, 16 products) |
| Services/CartStateService.cs | In-memory cart state (scoped per circuit) |

### Files Rewritten (Functional Pages)
| File | Transform |
|------|-----------|
| Program.cs | EF Core SQLite, CartStateService, auth, HttpContextAccessor, DB seeding |
| _Imports.razor | All BWFC usings + @inherits WebFormsPageBase |
| Components/App.razor | CSS links for bootstrap.css + Site.css |
| Components/Layout/MainLayout.razor + .cs | LoginView/LoginName/LoginStatus, category ListView, data loading |
| Default.razor + .cs | Clean Blazor page with PageTitle |
| About.razor, Contact.razor, ErrorPage.razor | Clean Blazor pages |
| ProductList.razor + .cs | ListView with Items binding + category filtering |
| ProductDetails.razor + .cs | FormView with Items wrapping |
| AddToCart.razor + .cs | Full cart-add implementation with DB lookup + cart service |
| ShoppingCart.razor + .cs | GridView with TextBox/CheckBox event handlers, cart total |

### Stub Pages Cleaned (26 files)
Account/*, Checkout/*, Admin/AdminPage, ViewSwitcher — markup compiles, code-behinds are minimal stubs.

## Build Verification

- **Final Result:** ✅ Build succeeded
- **Errors:** 0
- **Warnings:** 0 (from Run9; 70 warnings from BWFC library itself)
- **Build Attempts:** 7 (iterative fix cycle)

### Build Issues Resolved
| Issue | Fix |
|-------|-----|
| NuGet auth failure (GitHub Packages) | Changed to ProjectReference for local BWFC |
| CS0263 base class conflicts | Removed `: ComponentBase` from code-behinds (WebFormsPageBase via @inherits) |
| Wrong type param: `TItem` on GridView/ListView/FormView/DetailsView | Changed to `ItemType` (Layer 1 bug) |
| Missing Validations namespace | Added `@using BlazorWebFormsComponents.Validations` to _Imports.razor |
| Validators need explicit type params | Added `Type="string"` / `InputType="string"` to 26 validators |
| Unconverted `<% %>` expressions in stub pages | Replaced with Blazor @-expressions or static text |
| CS0103 undefined members in 17 stub files | Cleaned up OnClick handlers, ControlToValidate refs, @() expressions |

## Output Metrics

| Metric | Count |
|--------|-------|
| Total .razor files | 35 |
| Total .cs files | 46 |
| wwwroot assets | 79 |
| Routable pages (@page) | 28 |
| Unique BWFC control types used | 23 |
| Total BWFC control instances | 173 |

### BWFC Controls Preserved

| Control | Instances |
|---------|-----------|
| GridView | 2 |
| ListView | 4 |
| FormView | 1 |
| DetailsView | 1 |
| TextBox | 22 |
| Button | 17 |
| Label | 43 |
| HyperLink | 7 |
| Image | 1 |
| Panel | 1 |
| Literal | 7 |
| PlaceHolder | 14 |
| LoginView | 1 |
| LoginName | 1 |
| LoginStatus | 1 |
| DropDownList | 3 |
| CheckBox | 3 |
| RequiredFieldValidator | 21 |
| RegularExpressionValidator | 1 |
| CompareValidator | 4 |
| ValidationSummary | 7 |
| TemplateField | 4 |
| BoundField | 7 |

## Key Learnings

1. **Layer 1 bug — ItemType→TItem conversion:** `bwfc-migrate.ps1` converts `ItemType` to `TItem` for ALL data controls, but GridView/ListView/FormView/DetailsView use `ItemType` (only DropDownList uses `TItem`). This causes build failures every run.

2. **Validator type params required:** RequiredFieldValidator and RegularExpressionValidator need `Type="string"`, CompareValidator needs `InputType="string"`. Layer 1 does not add these.

3. **@inherits WebFormsPageBase conflicts with `: ComponentBase`:** When `_Imports.razor` has `@inherits WebFormsPageBase`, code-behind files must NOT specify `: ComponentBase`. Layout files are the exception — they need `: LayoutComponentBase`.

4. **Validations namespace not auto-included:** `BlazorWebFormsComponents.Validations` must be added to `_Imports.razor` manually.

5. **NuGet auth for GitHub Packages fails in CI-less environments:** Use ProjectReference for local development benchmarks.

6. **Stub page cleanup is the largest Layer 2 effort:** 17 of 35 razor files needed manual cleanup of unconverted code-behind references.
