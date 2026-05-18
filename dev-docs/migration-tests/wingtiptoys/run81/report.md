# WingtipToys Migration Benchmark — Run 81

**Date:** 2026-05-15  
**Branch:** `feature/cli-optimizations`  
**Commit:** `d008ea8b` (ComponentRef regex fix)  
**Operator:** Copilot (CLI) + Jeff (csharpfritz)  
**Result:** ✅ SUCCESS — 25/25 acceptance tests passing

## Summary

Run 81 is the first benchmark after the ComponentRef regex fix (`d008ea8b`). Initial build errors dropped from **28 (Run 80) to 14** — a 50% reduction. The `@ref` backing field generation now works correctly when classes inherit from `WebFormsPageBase`.

**Headline metric — L1+L2 migration time: 4 minutes 18 seconds (258s).**

## Timing

| Phase | Duration | Notes |
|-------|----------|-------|
| **L1 Migration** | **26s** | 204 files written, 0 errors |
| **L2 Build Repair** | **232s (3:52)** | 14 initial errors → 0 in 6 build iterations |
| **L1+L2 Combined** | **258s (4:18)** | ⬇️ from 266s in Run 80 |
| Startup Triage | 37s | 0 fixes needed, 7/7 routes HTTP 200 |
| Acceptance Tests | 130s | 24/25 → 25/25 after 1 fix (EF eager loading) |
| Screenshots | 14s | 7 screenshots captured |
| Session total | ~10 min | Includes conversation overhead between phases |

> **Note:** Session total includes human interaction latency (message round-trips, review pauses). The L1+L2 time of **4:18** is the clean automated metric — measured between tool calls with no conversation interruption.

## Run 80 → Run 81 Comparison

| Metric | Run 80 | Run 81 | Change |
|--------|--------|--------|--------|
| Initial build errors | 28 | 14 | **-50%** |
| L1 time | 28s | 26s | -2s |
| L2 build repair time | 238s | 232s | -6s |
| L1+L2 combined | 266s | 258s | **-8s** |
| Startup fixes needed | 0 | 0 | — |
| Acceptance test fixes | 1 | 1 | — |
| Final test result | 25/25 | 25/25 | ✅ |
| Build iterations | ~8 | 6 | -2 |

## CLI Fixes Validated

### ComponentRef Regex Fix (d008ea8b) — VALIDATED ✅

The `ClassOpenRegex` in `ComponentRefCodeBehindTransform` was changed from `\s*\{` to `[^{]*\{`. This allows the regex to match class declarations that include base class inheritance (`: WebFormsPageBase`) or interface lists.

**Impact:** Eliminated 14 CS0103 errors that were present in Run 80. The transform now correctly generates `@ref` backing fields (`CartList`, `CheckoutImageBtn`, `UpdateBtn`) in `ShoppingCart.razor.cs`.

### Template @ref Skip (85ec8cc8) — VALIDATED ✅

Controls inside `<ItemTemplate>` correctly have NO `@ref` attributes. `ShoppingCart.razor` PurchaseQuantity and Remove controls are clean.

### Variable Name Preservation (85ec8cc8) — VALIDATED ✅

`ShoppingCartActions.cs` preserves the original `_db` field name instead of generating `_productContext`.

## Errors Fixed During L2 Repair

### Build Errors (14 → 0)

| # | Error | File | Fix | CLI Gap? |
|---|-------|------|-----|----------|
| 1 | CS0103: `ShoppingCartTitle` | ShoppingCart.razor.cs | Added string field + bound in razor | Yes — HTML id→field |
| 2 | CS0103: `actions` | ShoppingCart.razor.cs | Changed to `_shoppingCartActions` | Yes — stale var ref |
| 3 | CS0191: readonly field assignment | ShoppingCartActions.cs | Removed `readonly` from `_db` | Yes — Dispose pattern |
| 4 | CS7036: parameterless constructor | ShoppingCartActions.cs | Rewrote `GetCart()` to use `this` | Yes — static factory |
| 5 | CS1061: `Request.IsLocal` | ErrorPage.razor.cs | Replaced with `true` (dev default) | Yes — missing shim |
| 6 | CS0120: static/instance mismatch | ExceptionUtility.cs | Made `LogException` static again, removed instance ref | Yes — static detection |
| 7 | CS0103: `ProviderName` | RegisterExternalLogin.razor.cs | Added string field | Yes — stub incomplete |
| 8-9 | CS1503/CS1660: EventCallback | ShoppingCart.razor, RegisterExternalLogin.razor | EventCallback.Factory.Create wrappers | Yes — signature adapt |
| 10 | CS1503: int→string | ShoppingCart.razor | Added `.ToString()` | Yes — type mismatch |
| 11 | CS0103: `Transparent` | ShoppingCart.razor | Changed to `System.Drawing.Color.Transparent` | Yes — color enum |
| 12 | CS0103: `CategoryName` | ProductList.razor.cs | Changed to `categoryName` (existing parameter) | Yes — case sensitivity |
| 13 | RZ9981: stray `</p>` | ProductList.razor | Removed orphan closing tag | Yes — HTML cleanup |
| 14 | CS1729: ImageClickEventArgs | ShoppingCart.razor.cs | Changed to `EventArgs` | Yes — type adaptation |

### Acceptance Test Fix (1)

| Error | File | Fix |
|-------|------|-----|
| `AddItemToCart` timeout — cart table hidden | ShoppingCartActions.cs | Added `.Include(c => c.Product)` for EF Core eager loading |

## CLI Gaps Identified

### Recurring (from Run 80)

1. **Orphan HTML tag cleanup** — stray `</p>` in ProductList.razor (same as Run 80)
2. **EF Core eager loading** — `.Include()` needed for navigation properties (every run)
3. **EventCallback signature adaptation** — Web Forms `(object sender, EventArgs e)` → Blazor `EventCallback<EventArgs>`
4. **Static/instance method detection** — `ExceptionUtility.LogException` static but uses instance field

### New Observations

5. **HTML ID→field binding** — `ShoppingCartTitle` div's InnerText set from code-behind; needs string field + Razor binding
6. **Stale variable references** — `actions` should be `_shoppingCartActions` after DI transform
7. **Dispose pattern with readonly** — CLI marks DI fields as `readonly` but original code assigns `null` in `Dispose()`
8. **Color enum resolution** — `BackColor="Transparent"` should become `System.Drawing.Color.Transparent`
9. **Parameter case sensitivity** — `CategoryName` vs `categoryName` (parameter casing mismatch in SelectMethod wrapper)

## Screenshot Gallery

### Home Page
![Home Page](images/01-home.png)

### Products
![Products](images/02-products.png)

### Product Details
![Product Details](images/03-product-details.png)

### Shopping Cart (Empty)
![Shopping Cart](images/04-shopping-cart.png)

### Login
![Login](images/05-login.png)

### About
![About](images/06-about.png)

### Shopping Cart (With Item)
![Cart With Item](images/07-cart-with-item.png)

## Conclusion

Run 81 validates the ComponentRef regex fix as a significant improvement. The 50% reduction in initial build errors (28→14) translates to fewer repair iterations and faster L2 time. The recurring EF eager loading gap remains the most impactful single fix needed per run. The EventCallback signature adaptation pattern is the highest-value next CLI improvement — it affects 3 errors per run across multiple files.
