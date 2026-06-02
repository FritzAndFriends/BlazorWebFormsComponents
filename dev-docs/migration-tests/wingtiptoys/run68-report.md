# WingtipToys Migration Benchmark — Run 68

**Date:** 2026-05-13  
**Branch:** `feature/cli-optimizations`  
**CLI Commit:** `81907aae` (Fix 7 CLI migration gaps from run67 analysis)  
**Target:** .NET 10 Blazor Static SSR  

## Summary

| Metric | Value |
|--------|-------|
| **Acceptance Tests** | **25/25 (100%)** |
| **L1 Build Errors** | 22 → 0 (all fixed in L2) |
| **Files Generated** | 221 (L1) → 531 total after build repair |
| **Quarantined Pages** | Account/*, Checkout/*, ErrorPage, ShoppingCartActions |
| **Key Improvement** | 7 CLI fixes from run67 analysis reduced manual work |

## Timing Breakdown

| Phase | Duration | Notes |
|-------|----------|-------|
| Preparation | ~15s | Clear output, create report folder |
| L1 Migration (CLI) | ~18s | `bwfc-migrate.ps1` → 221 files |
| Build Repair (L2) | ~10 min | Agent fixed 22 errors + manual parameter dedup fix |
| Startup Triage | ~5 min | Fixed ProductList duplicate parameter runtime error |
| Acceptance Tests | ~23s | 25/25 passing (build+test time) |
| Screenshots | ~1 min | 5 key pages captured |
| Report | ~2 min | This document |
| **Total** | **~20 min** | Down from ~45 min in run67 |

## CLI Fixes Validated in This Run

These 7 fixes were committed before run68 and validated:

| # | Fix | Validated? |
|---|-----|-----------|
| 1 | `partial` keyword on LegacyHelperStubTransform | ✅ No partial errors |
| 2 | ValidatorDisplay enum → `@ValidatorDisplay.Dynamic` | ✅ Verified in ShoppingCart.razor |
| 3 | Route parameter dedup (RouteParameterWiringTransform) | ⚠️ **Partial** — CategoryName still duplicated (see below) |
| 4 | AspxRewriteMiddleware POST handling | ✅ POST routes preserved |
| 5 | DbContext FQN regex matching | ✅ No `new ProductContext()` errors |
| 10 | AddDbContext (not Factory) + scoped seeding | ✅ EF contexts register correctly |
| 8 | Add to Cart test flow fix | ✅ Test passes from ProductList |

## L2 Build Repair — Issues Fixed

### Category A: ShoppingCartActions Stub (Root cause of ~15 errors)

The quarantine system correctly flagged `ShoppingCartActions` but generated a minimal stub missing:
- `IDisposable` implementation
- `AddToCart(int productId)` overload  
- `ShoppingCartUpdates` nested type
- `UpdateShoppingCartDatabase(string, ShoppingCartUpdates[])` overload

**Fix:** Rewrote stub with DI-friendly constructor (`ProductContext` + `IHttpContextAccessor`), proper method signatures, `IDisposable`, and `ShoppingCartUpdates` class. Added `.Include(c => c.Product)` for eager loading.

### Category B: ShoppingCart.razor.cs GridView Row Access

Code-behind used `GridViewRow.FindControl()`, `.Cells`, `.RowState` — none available in BWFC's `IRow<T>`. 

**Fix:** Simplified `UpdateCartItems()` to work with field-based state instead of GridView row iteration.

### Category C: ErrorPage.razor.cs

- `Request.IsLocal` → replaced with environment check
- `LogException` signature mismatch → fixed to match ExceptionUtility stub

### Category D: Account/RegisterExternalLogin.razor

- Missing `ProviderName` field → added as string property
- `OnClick` method group conversion → fixed EventCallback signature

### Category E: ProductList Duplicate Parameter (Runtime Error)

**This is a CLI bug that Cat 3 fix didn't fully prevent.** The `RouteParameterWiringTransform` added `[Parameter] public string? CategoryName` to the `.razor` `@code` block, but the code-behind already had `[Parameter] public string? categoryName`. Blazor parameters are **case-insensitive**, so these collide at runtime.

**Root cause:** The Cat 3 dedup regex checks for `[Parameter]` in the code-behind, but the code-behind parameter was lowercase `categoryName` while the markup one was PascalCase `CategoryName`. The case-insensitive match should have caught this — needs further investigation in the transform.

**Fix:** Removed duplicate from `.razor` `@code` block. Also wired `GetProductsQueryDetails_SelectMethod` to call the code-behind `GetProducts()` method instead of returning empty data, and added `.Include(p => p.Category)` for navigation property loading.

### Category F: Default.razor Missing Main Content Container

The test `HomePage_HasStyledMainContent` expects `[role='main']` or `.jumbotron`. The migrated homepage had bare HTML.

**Fix:** Wrapped content in `<section role="main">`.

## Remaining Items for Future Runs

### CLI Improvements Needed

1. **Cat 3 Enhancement:** RouteParameterWiringTransform needs case-insensitive dedup across `.razor` and `.razor.cs` files. Current regex misses PascalCase vs camelCase variants.

2. **SelectMethod Wrapper Wiring:** The generated `GetProductsQueryDetails_SelectMethod` in `.razor` returns empty data instead of calling the code-behind `GetProducts()`. The wrapper should auto-wire to the original method.

3. **Quarantine Stub Fidelity:** When quarantining helper classes (like `ShoppingCartActions`), the stub should match the original class's public API surface more closely — especially `IDisposable`, method overloads, and nested types.

4. **Homepage Main Container:** Default/home page migration should wrap content in a semantic container (`<section role="main">` or `<main>`).

### Deferred Items

- **ShoppingCartActions full implementation** (Cat 6 from run67): Currently a compile-safe stub with basic EF operations. Full cart update/checkout logic needs L3 work.
- **GridView row state patterns:** Deep Web Forms GridView patterns (`FindControl`, `Cells`, `RowState`) need a general L2 recipe for simplification.

## Screenshots

### Homepage
![Homepage](images/homepage.png)

### Product List
![Product List](images/productlist.png)

### Product Details
![Product Details](images/productdetails.png)

### Shopping Cart
![Shopping Cart](images/shoppingcart.png)

### About Page
![About](images/about.png)

## Test Results

```
Test summary: total: 25, failed: 0, succeeded: 25, skipped: 0, duration: 21.5s
```

All 25 acceptance tests pass:
- Navigation tests (homepage, about, contact, product list, product details)
- Product catalog display with images
- Shopping cart page loads
- Add to cart functionality
- Category filtering routes
- .aspx URL compatibility (middleware rewrite)
- Static asset loading (CSS, images)
- Layout structure verification
