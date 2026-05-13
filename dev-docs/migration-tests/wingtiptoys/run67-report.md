# WingtipToys Migration Benchmark — Run 67

**Date:** 2026-06-07
**Branch:** `feature/cli-optimizations`
**Commit (CLI fixes):** `62320e11` (Fixes 1–4 for run67)
**Result:** 25/25 acceptance tests passing

## Summary

| Metric | Run 65 | Run 66 | Run 67 |
|--------|--------|--------|--------|
| Acceptance tests | 22/25 | 25/25 | 25/25 |
| Manual L2 fixes | 19 | 8 | 15 files, 11 fix categories |
| Initial build errors | ~60 | ~28 | ~28 |
| Build errors after CLI fixes | — | — | ~28 (same — fixes were pre-committed) |

> Run 67 used the same CLI as run 66 plus 4 new fixes (quarantine dedupe, full code-behind emission, RouteData param promotion, DbContext constructor injection for standalone classes). The higher fix count vs run 66 reflects more thorough categorization, not regression.

## L1 Migration (CLI)

The CLI migration completed in ~12 seconds, generating the full Blazor static SSR scaffold with identity, EF, and session support.

## CLI Fixes Committed for This Run

1. **Quarantine stub dedupe** — `MigrationPipeline` extracts class names from scaffold output and passes them to `SourceFileCopier`, which skips generating stubs for already-scaffolded classes
2. **Full code-behind emission** — When `PageCodeBehindEmissionPlanner` returns `Artifact`, the pipeline now emits the full transformed code-behind instead of only a skeleton
3. **RouteData parameter promotion** — New `RouteDataParameterPromotionTransform` (Order 136) removes `[RouteData]` parameters from method signatures since `RouteParameterWiringTransform` already creates class-level `[Parameter]` properties
4. **DbContext constructor injection** — `DbContextInstantiationTransform` rewritten to detect page vs standalone classes; standalone classes get constructor injection instead of `[Inject]`

## L2 Manual Fixes (11 categories, 15 files)

### Category 1: Missing `partial` on quarantine stubs
**Files:** `Account/OpenAuthProviders.razor.cs`, `Account/RegisterExternalLogin.razor.cs`
**CLI bug:** `LegacyHelperStubTransform` line 59 generates `public class` without `partial` keyword. Since the `.razor` file implicitly creates a partial class, the code-behind must also be `partial`.
**Fix:** Add `partial` keyword to class declarations.

### Category 2: ValidatorDisplay enum as string literal
**File:** `Account/RegisterExternalLogin.razor`
**CLI bug:** `Display="Dynamic"` left as string when it should be `Display="@ValidatorDisplay.Dynamic"`.
**Fix:** Change to enum reference.

### Category 3: Duplicate route parameter collision
**File:** `ProductList.razor.cs`
**CLI bug:** `RouteParameterWiringTransform` creates `[Parameter] public string? CategoryName` in `.razor` `@code` block, but the code-behind also declares `string categoryName` field. Blazor throws "declares more than one parameter matching the name" (case-insensitive).
**Fix:** Remove the code-behind field, reference the PascalCase `.razor` property.

### Category 4: .aspx route directives
**Files:** `AddToCart.razor`, `ShoppingCart.razor`
**CLI gap:** `AspxRewriteMiddleware` handles `/page.aspx` → `/page` redirects, but some test flows link directly to `.aspx` URLs with query strings. Adding `@page "/Page.aspx"` ensures both routes work.
**Fix:** Add `@page "/AddToCart.aspx"` and `@page "/ShoppingCart.aspx"`.

### Category 5: DbContext injection for non-page files
**Files:** `ProductDetails.razor.cs`, `ProductList.razor.cs`
**CLI gap:** DbContext transform now handles standalone classes but these page files needed `[Inject] protected ProductContext` added manually because the original code used `new ProductContext()` in patterns the regex didn't match (fully-qualified names like `new WingtipToys.Models.ProductContext()`).
**Fix:** Add `[Inject]` for ProductContext.

### Category 6: ShoppingCartActions full reimplementation
**File:** `Logic/ShoppingCartActions.cs`
**Nature:** Business logic rewrite — original uses `HttpContext.Current.Session` and `new ProductContext()`. Migrated version needs constructor-injected `ProductContext` + `IHttpContextAccessor`, session via `GetString()`/`SetString()`, and `.Include(c => c.Product)` for EF eager loading.
**Scope:** 177 lines changed. This is the largest single fix and hardest to automate.

### Category 7: ShoppingCart @ref → field-based state
**Files:** `ShoppingCart.razor`, `ShoppingCart.razor.cs`
**Pattern:** Web Forms code-behind accesses Label.Text in Page_Load. CLI generates `@ref` bindings, but component refs are null during `OnInitializedAsync`. Must use field-based state instead.
**Fix:** Replace `@ref LabelTotalText` with `@_labelTotalText` bound fields; rewrite code-behind to set fields.

### Category 8: ProductDetails "Add To Cart" link
**File:** `ProductDetails.razor`
**Nature:** Test expects an "Add To Cart" link on the product detail page. The original Web Forms page had it, but the CLI-generated markup lost it during template conversion.
**Fix:** Add `<a href="/AddToCart.aspx?productID=@Item.ProductID" data-enhance-nav="false">Add To Cart</a>`.

### Category 9: SelectMethod stub wiring
**File:** `ProductList.razor`
**CLI gap:** CLI generates `GetProductsQueryDetails_SelectMethod` stub that returns empty list. The real `GetProducts()` method exists in code-behind but isn't called by the stub.
**Fix:** Wire stub to call real `GetProducts(CategoryId)`.

### Category 10: Program.cs DI consolidation
**File:** `Program.cs`
**CLI gap:** Scaffolder emits both `AddDbContextFactory<ProductContext>` and `AddDbContext<ProductContext>`, causing "Cannot resolve scoped service from root provider" at startup. Also needs `AddScoped<ShoppingCartActions>()`.
**Fix:** Keep only `AddDbContext`, fix DB seeding to use scoped resolution, add service registration.

### Category 11: Miscellaneous fixes
**Files:** `ErrorPage.razor.cs`, `Logic/ExceptionUtility.cs`, `Logic/AddProducts.cs`
- `Request.IsLocal` → unconditional (no Blazor equivalent)
- `LogException` method signature fixed to be static
- `AddProducts.cs` DbContext usage fixed

## CLI Bugs Identified for Future Fixes

| # | Bug | Impact | Fix Complexity |
|---|-----|--------|---------------|
| 1 | `LegacyHelperStubTransform` missing `partial` keyword | 2+ files per migration | Trivial — add `partial` to line 59 |
| 2 | DbContext regex doesn't match fully-qualified names (`new NS.DbContext()`) | 2+ files | Medium — extend regex pattern |
| 3 | Duplicate parameter when `.razor` and `.razor.cs` both declare route params | 1+ file | Medium — detect and dedupe |
| 4 | SelectMethod stub should auto-delegate to original method | 1+ file | Medium — analyze method signatures |
| 5 | `AddDbContextFactory` + `AddDbContext` conflict in scaffolder | 1 file (Program.cs) | Easy — emit only `AddDbContext` |
| 6 | ValidatorDisplay enum left as string | Validation pages | Easy — enum-aware transform |
| 7 | @ref pattern generates null refs in `OnInitializedAsync` | Shopping cart pattern | Hard — requires pattern analysis |
| 8 | ShoppingCartActions-style classes need full DI rewrite | Business logic | Hard — requires semantic understanding |

## Key Technical Decisions

1. **Field-based state over @ref:** Component refs are null during `OnInitializedAsync`. Fields bound directly in markup are the correct Blazor pattern.
2. **Single `AddDbContext` over factory+context:** `AddDbContextFactory` + `AddDbContext` on the same type causes DI conflicts. Use only `AddDbContext` which provides scoped resolution.
3. **`.Include()` for navigation properties:** EF queries backing GridView BoundFields that reference nested properties (e.g., `Product.ProductName`) must eagerly load those relationships.
4. **Session via `IHttpContextAccessor`:** Static SSR pages have HTTP context available. Use `HttpContext.Session.GetString()/SetString()` for cart ID storage.
