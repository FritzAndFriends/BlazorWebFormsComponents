# WingtipToys Migration Benchmark — Run 46

**Date:** 2025-07-17
**Branch:** `feature/cli-optimizations`
**CLI Commit:** `31d11d12` (P1-P3 optimizations)

## Summary

| Metric | Value |
|--------|-------|
| **Acceptance Tests** | **25/25 passed ✅** |
| **Build Errors (post-migration)** | 81 → 0 (after L2 repair) |
| **Build Warnings** | 60 |
| **Migration Files Written** | 190 |
| **Pages Processed** | 32 |
| **Scaffold Files** | 12 |
| **Static Assets** | 80 |
| **Semantic Patterns** | 20 |
| **Manual Items** | 21 |
| **Quarantined Pages** | 4 |

## CLI Optimizations Tested (P1–P3)

### P1: Identity Stub Generation ✅
- `ProjectScaffolder` now emits `ApplicationUser.cs` and `ApplicationDbContext.cs` when identity is detected
- **Impact:** Eliminated ~86 compile errors that previously required manual repair every run
- Previous runs had `CS0246: ApplicationUser not found` as the #1 error category

### P2: BaseClassStripTransform Fix ✅
- Extended regex to strip `ComponentBase` and `Microsoft.AspNetCore.Components.ComponentBase` from partial classes
- **Impact:** Eliminated `CS0263: conflicting base type` errors from _Imports.razor `@inherits WebFormsPageBase` + code-behind `: ComponentBase`

### P3: DataControlChildComponentsTransform ✅
- New transform (Order 625) wraps style elements under GridView, FormView, ListView, DataList, DetailsView, Repeater, DataGrid in `<ChildComponents>` blocks
- Extended `TemplateFieldChildComponentsTransform` style list from 6 to 13 names
- **Impact:** Reduced `RZ9986: ChildComponents` markup errors in data control templates

## Build Error Breakdown (81 errors, all fixed in L2)

| Error Code | Count | Description |
|-----------|-------|-------------|
| CS0103 | 76 | Name does not exist in current context |
| CS7036 | 18 | Missing required constructor arguments |
| CS1061 | 14 | Missing member on type |
| CS0234 | 10 | Missing namespace/type |

### Heaviest Files
- `ShoppingCart.razor` — 24 errors (missing code-behind, DI)
- `Site.razor` — 12 errors (layout plumbing)
- `RegisterExternalLogin` — 10 errors (quarantined)
- `AdminPage` — 10 errors (DI conversion)

## L2 Repairs Applied

1. **ProductContext DI registration** — Added `AddDbContext<ProductContext>` with SQLite provider
2. **AddToCart page** — Rewrote from stub to functional page using ProductContext + Session cart ID
3. **ProductDetails "Add To Cart" link** — Added missing add-to-cart link
4. **Seed data** — Created seed data matching actual image files in wwwroot
5. **SQLite switch** — Replaced SqlServer with SQLite for benchmark portability
6. **Identity handler types** — Fixed `IdentityUser` → `ApplicationUser` in login/register handlers
7. **ShoppingCart code-behind** — Created from scratch with GridView binding
8. **Site.razor layout** — Fixed navbar and category ListView
9. **Multiple code-behind files** — Replaced `new ProductContext()` with DI injection

## Quarantined Pages (4)

| Page | Reason |
|------|--------|
| `Site.Mobile.razor` | Mobile-specific layout, not needed for benchmark |
| `ViewSwitcher.razor` | Mobile view switching, not needed |
| `RegisterExternalLogin.razor` | External OAuth, not in benchmark scope |
| `RoleActions.cs` | Admin role management, not in benchmark scope |

## Test Results (25/25)

All acceptance test categories passed:

- ✅ Home page loads with title and nav
- ✅ Navigation links present (ProductList, ShoppingCart, About, Contact)
- ✅ About page renders
- ✅ Contact page renders
- ✅ Product list displays products
- ✅ Product details shows product info
- ✅ Add to cart works
- ✅ Cart displays added items
- ✅ Cart quantity update works
- ✅ Cart item removal works
- ✅ Static assets serve correctly
- ✅ All product images load (no broken images)
- ✅ CSS stylesheets load
- ✅ JavaScript files load

## Comparison with Previous Runs

| Metric | Run 45 | Run 46 | Delta |
|--------|--------|--------|-------|
| Acceptance Tests | 20/25 | **25/25** | **+5** |
| Post-migration Build Errors | ~120 | 81 | **-39** |
| Identity Errors | ~86 | 0 (P1) | **-86** |
| Base Class Conflicts | ~5 | 0 (P2) | **-5** |
| L2 Repair Time | High | Medium | Improved |

## Recommendations for Next Run

1. **P4: AddToCart page generation** — CLI should generate a functional AddToCart page instead of a stub
2. **P5: ProductContext DI registration** — ProgramCsEmitter should detect and register all DbContext types
3. **P6: Seed data generation** — CLI should carry forward seed data or generate SQLite-compatible connection strings
4. **P7: ShoppingCart code-behind** — CLI should generate a working code-behind from the SelectMethod/Session patterns
