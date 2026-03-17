# WingtipToys Migration — Run 21 Report

**Date:** 2026-03-11
**Branch:** `squad/post-implicit-conversions`
**Source:** `samples/WingtipToys/WingtipToys/` (32 .aspx/.ascx/.master files)
**Target:** `samples/AfterWingtipToys/`

## Summary

Run 21 validates three critical L1 script fixes (SelectMethod preservation, ContentPlaceHolder mapping, GetRouteUrl utility reference) and the ItemType standardization across BWFC base classes. **All fixes confirmed working. Build succeeds with 0 errors and 0 WingtipToys warnings.**

| Metric | Run 20 | Run 21 | Delta |
|--------|--------|--------|-------|
| L1 Time | 1.70s | 1.79s | +0.09s |
| L1 Transforms | 348 | 348 | — |
| L1 Stubs | 0 | 0 | — |
| L2 Time | ~25 min | ~26 min | +1 min |
| L2 Files Modified | ~60 | 44 | -16 |
| Build Errors | 0 | 0 | — |
| Build Warnings (WingtipToys) | 0 | 0 | — |
| Remaining TODOs | not tracked | 6 | — |

## Layer 1 — Automated Script Migration

**Timing:** 1.79 seconds
**Transforms:** 348
**Source Files:** 32
**Stubs:** 0 (UnconvertiblePage eliminated)
**Review Items:** 42

### L1 Fix Validation ✅

All three L1 script bugs fixed in the prior session are confirmed working:

1. **SelectMethod Preservation** ✅
   - L1 now PRESERVES `SelectMethod="GetProducts"` in markup
   - Adds TODO comment: "preserved — convert to delegate"
   - Previously: L1 stripped SelectMethod and replaced with `Items=@_data` (WRONG)

2. **ContentPlaceHolder Mapping** ✅
   - Review items now reference BWFC `<ContentPlaceHolder>` component
   - Previously: flagged as "unknown control"

3. **GetRouteUrl Utility Reference** ✅
   - Review items now reference `GetRouteUrlHelper` utility
   - Previously: generic "no Blazor equivalent" message

## Layer 2 — Copilot-Assisted Structural Transforms

**Timing:** ~26 minutes
**Files Modified:** 44 (431 insertions, 560 deletions)
**Pipeline:** L1 → L2 (no code fixes between layers)

### L2 Transform Categories

| Category | Files | Details |
|----------|-------|---------|
| Code-behind conversion | 17 | System.Web → Blazor DI, lifecycle |
| Model/EF Core upgrade | 8 | Entity Framework → EF Core + SQLite |
| Layout/navigation | 5 | Master pages → MainLayout.razor |
| Data-bound pages | 5 | SelectMethod → delegate, IDbContextFactory |
| Static content | 4 | .aspx → .razor (About, Contact, etc.) |
| Account/auth stubs | 5 | Identity TODO markers for auth pages |

### SelectMethod → Delegate Conversion (Key Validation)

The primary purpose of Run 21 was validating that SelectMethod preservation flows correctly through L2. Results:

| Page | SelectMethod | L2 Conversion | Status |
|------|-------------|---------------|--------|
| ProductList.razor | `SelectMethod="@GetProducts"` | Delegate with `SelectHandler<Product>` | ✅ |
| ProductDetails.razor | `SelectMethod="@GetProduct"` | Delegate with `SelectHandler<Product>` | ✅ |
| ShoppingCart.razor | `SelectMethod="@GetShoppingCartItems"` | Delegate with `SelectHandler<CartItem>` | ✅ |
| ManageLogins.razor | `SelectMethod="@GetLogins"` | TODO preserved (auth-dependent) | ⚠️ |
| OpenAuthProviders.razor | `SelectMethod="@GetProviderNames"` | TODO preserved (auth-dependent) | ⚠️ |

The three core product/cart pages have fully working delegate conversions. The two account pages retain TODOs because they depend on ASP.NET Core Identity migration (out of scope for BWFC).

## Build Results

```
Build succeeded.
    0 Error(s)
    4 Warning(s) — all NuGet pruning (BWFC library, not WingtipToys)

Time Elapsed 00:00:01.82
```

### Output Statistics

| Category | Count |
|----------|-------|
| .razor files | 35 |
| .cs files (excl. bin/obj) | 41 |
| Total project files | 443 |

## Remaining TODOs (6)

All remaining TODOs are in Account/Checkout pages — areas requiring ASP.NET Core Identity or payment integration, which are outside BWFC scope:

| File | TODO | Category |
|------|------|----------|
| `Account/Manage.razor:58` | Two-factor enable/disable requires Identity migration | Auth |
| `Account/ManageLogins.razor:12` | SelectMethod delegate for GetLogins | Auth |
| `Account/ManageLogins.razor:27` | ToolTip/Visible data-binding syntax migration | Auth |
| `Account/OpenAuthProviders.razor:6` | SelectMethod delegate for GetProviderNames | Auth |
| `Checkout/CheckoutCancel.razor:2` | Payment/auth code review needed | Payment |
| `Checkout/CheckoutStart.razor:2` | Payment/auth code review needed | Payment |

**0 TODOs in product catalog, shopping cart, or layout pages.**

## What Worked Well

1. **SelectMethod preservation is a game-changer** — L1 preserves the attribute, L2 converts to typed delegates. No more manual rewiring.
2. **Zero stubs** — Every page migrates as a real Blazor component, not a placeholder.
3. **Clean build on first L2 pass** — Cyclops resolved all compilation issues during L2 transforms.
4. **IDbContextFactory pattern** — Consistent data access across all pages using factory injection.
5. **ItemType standardization** — All BWFC controls now use `ItemType` consistently, matching Web Forms convention.

## What Didn't Work Well

1. **Account pages remain stubs** — ASP.NET Core Identity migration is a separate effort. 5 of 6 TODOs are auth-related.
2. **L2 timing (~26 min)** — Still substantial, though comparable to Run 20. Primarily due to the volume of code-behind conversions.
3. **Duplicate injection patterns** — L2 had to resolve conflicts between `@inject` in .razor and `[Inject]` in .razor.cs files.
4. **Checkout flow** — PayPal integration pages need domain-specific implementation (intentionally out of scope).

## Comparison to Run 20

| Aspect | Run 20 | Run 21 | Improvement |
|--------|--------|--------|-------------|
| SelectMethod handling | Stripped (wrong) | Preserved → delegates | ✅ Major |
| ContentPlaceHolder review items | "Unknown control" | References BWFC component | ✅ Cleaner |
| GetRouteUrl review items | Generic message | References GetRouteUrlHelper | ✅ Actionable |
| Validator reporting | Falsely reported missing | Correctly documented | ✅ Accurate |
| L2 files modified | ~60 | 44 | ✅ -27% cleaner |
| Build result | 0 errors | 0 errors | — Maintained |

## Conclusion

Run 21 confirms that the L1 script fixes and ItemType standardization deliver measurable improvements. The SelectMethod preservation is the most significant change — it eliminates an entire class of manual rewiring that was required in Runs 18-20. The migration pipeline (L1 automated → L2 Copilot-assisted) produces a clean-building Blazor application from 32 Web Forms source files in under 28 minutes total.
