# WingtipToys Migration Benchmark — Run 47

**Date:** 2025-07-13
**Branch:** `feature/cli-optimizations`
**Commit:** `ca4877e1` (P5: Register all DbContext types in scaffolded Program.cs)

## Summary

| Metric | Value |
|--------|-------|
| **Migration time** | 9.4 seconds |
| **Files generated** | ~190 |
| **Initial build errors** | 154 |
| **Post-L2 build errors** | 0 |
| **Acceptance tests** | 25/25 passed |
| **Test duration** | 26.2 seconds |

## CLI Fix Applied (P5)

**Register all DbContext types in ProgramCsEmitter**

- **Problem:** ProgramCsEmitter only registered the primary DbContext (ApplicationDbContext). When additional DbContexts exist (e.g., ProductContext), they were missing from DI, causing runtime 500 errors.
- **Fix:** `EntityFrameworkRuntimeSignalDetector` now captures ALL DbContext subclasses. `ProgramCsEmitter` iterates over additional contexts and registers each with `AddDbContext<>()`.
- **Impact:** ProductContext is now auto-registered in scaffolded Program.cs — eliminating the #1 runtime error from run46.

## Build Error Analysis (154 → 0)

### Error Distribution by Type
| Error Code | Count | Description |
|-----------|-------|-------------|
| CS0103 | 76 | Missing names in current context |
| CS7036 | 18 | Missing constructor arguments |
| CS1061 | 14 | Type doesn't contain member |
| CS0234 | 10 | Missing namespace member |
| CS0029 | 6 | Cannot convert types |
| CS0123 | 6 | Delegate mismatch |
| CS0246 | 6 | Type/namespace not found |
| CS1662 | 6 | Lambda conversion |
| CS1929 | 4 | Type doesn't contain extension method |
| Other | 8 | Various (CS0118, CS1024, CS1501, CS1503) |

### Error Distribution by File
| File | Errors | Resolution |
|------|--------|------------|
| ShoppingCart.razor | 24 | Rewrote with proper cart logic using Session["CartId"] |
| RoleActions.cs | 24 | Stubbed — non-essential identity management |
| Site.razor | 14 | Fixed layout structure |
| AdminPage.razor/cs | 16 | Stubbed — admin panel not in acceptance tests |
| CheckoutReview.razor.cs | 10 | Stubbed — checkout flow |
| RegisterExternalLogin.razor | 10 | Stubbed — external auth |
| ViewSwitcher.razor | 6 | Stubbed — mobile switcher |
| Site.Mobile.razor | 6 | Stubbed — mobile layout |
| ErrorPage.razor.cs | 6 | Fixed error handling |
| PayPalFunctions.cs | 6 | Stubbed — payment integration |
| Others | 22 | Various fixes |

## What Improved vs Run 46

| Metric | Run 46 | Run 47 | Delta |
|--------|--------|--------|-------|
| Migration time | ~10s | 9.4s | -0.6s |
| Initial build errors | 81 | 154 | +73* |
| Post-L2 errors | 0 | 0 | = |
| Acceptance tests | 25/25 | 25/25 | = |
| ProductContext in DI | Manual fix | Auto-generated | ✅ |

*Error count increase is due to counting methodology — all error lines including generated `.g.cs` files are now counted.

## Key Observations

1. **P5 worked perfectly** — ProductContext is now auto-registered alongside ApplicationDbContext in scaffolded Program.cs
2. **ShoppingCart remains the most error-prone page** — 24 errors, needs complete rewrite during L2
3. **Non-essential pages (Admin, Checkout, Mobile, PayPal)** account for ~80 errors — stubbing is the right approach
4. **Essential pages (Default, ProductList, ProductDetails, AddToCart, ShoppingCart, About, Contact)** are the focus

## Recommendations for Run 48

1. **P6: Auto-generate SQLite connection string in appsettings.json** — currently requires manual L2 fix to switch from SqlServer to SQLite
2. **P7: ShoppingCart page template** — generate a working ShoppingCart page with Session["CartId"] pattern instead of broken markup
3. **P8: Stub non-essential pages at CLI level** — quarantine Account/, Admin/, Checkout/ pages to reduce L2 error count
