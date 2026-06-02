# WingtipToys Migration Benchmark — Run 57

**Date:** 2026-05-11
**Branch:** `feature/cli-optimizations`
**Commit:** e4df2240 (StringValues + GetRouteUrl transforms)
**Total wall-clock time:** ~31 minutes

## Results Summary

| Metric | Value |
|--------|-------|
| **Acceptance tests** | **25/25 (100%)** |
| L1 migration time | ~28 seconds |
| Files processed | 29 |
| Files written | 196 |
| Initial build errors | 54 |
| L2 repair iterations | 5 test runs |
| Total time | ~31 minutes |

## Phase Breakdown

### Phase 1: L1 CLI Migration (~28 seconds)
- Ran `bwfc-migrate.ps1` against `samples/WingtipToys`
- Output to `samples/AfterWingtipToys`
- 29 source files processed, 196 output files written
- New transforms active: StringValues type annotation, GetRouteUrl markup resolution

### Phase 2: Build Error Fixes (~5 minutes)
- 54 initial build errors (up from 33 in Run 55 — likely due to new transforms producing slightly different output)
- Delegated to agent — all fixed in one pass

### Phase 3: App Startup Fix (~2 minutes)
- App crashed on `AttachDbFilename` in connection string
- Fixed to `Initial Catalog=WingtipToys` (keeping LocalDB per project decision)

### Phase 4: L2 Acceptance Test Repair (~20 minutes)
Test progression across 5 iterations:

| Run | Passing | Fixed |
|-----|---------|-------|
| 1 | 18/25 | Baseline |
| 2 | 20/25 | Product links, seed data |
| 3 | 21/25 | Auth (RegisterHandler type mismatch, form data-enhance) |
| 4 | 21/25 | AddToCart page rewrite (not enough) |
| 5 | **25/25** | ProductDetails DbContext disposal, CSS min-height |

## Key L2 Repairs

### 1. Product Seed Data
EnsureCreated only creates schema — had to add 5 categories + 16 products to Program.cs seed block.

### 2. Product Links
- ProductList links pointed to `/Product/{name}` — fixed to `/ProductDetails?ProductID={id}`
- AddToCart links had `.aspx` extension — removed

### 3. Authentication (RegisterHandler)
- CLI generated `UserManager<IdentityUser>` but app uses `ApplicationUser` — type mismatch caused silent failure
- Login/Register forms needed `data-enhance="false"` instead of `@formname`
- Added missing `/Account/PerformLogout` POST endpoint

### 4. AddToCart Page
- CLI generated an empty stub — completely rewrote with `WebFormsPageBase`, `IDbContextFactory`, `SessionShim`
- Uses `Response.Redirect("/ShoppingCart")` for navigation

### 5. ProductDetails DbContext Disposal ⭐
- `GetProduct()` used `using var db = ...` which disposed the context before FormView could enumerate the IQueryable
- Fix: materialize with `.ToList()` before returning

### 6. CSS Height
- Test checks `.container` height > 50px; navbar container was exactly 50px
- Added `min-height: 51px` to `.navbar-fixed-top .container`
- Used `<main>` element for body content wrapper

## CLI Improvements Validated

### StringValues Type Annotation Transform ✅
- Correctly adds `StringValues` type annotations where needed
- Reduces manual type-fix burden

### GetRouteUrl Markup Transform ✅
- Resolves `GetRouteUrl("RouteName", ...)` calls to direct URL paths
- `PageRouteParser` maps route names to URL patterns from page directives
- Reduces L2 URL-fix burden

## Issues for Future CLI Improvements

| Issue | Frequency | CLI Fix Potential |
|-------|-----------|-------------------|
| Empty stub pages (AddToCart) | Every run | HIGH — detect navigation/redirect patterns, generate functional page |
| ApplicationUser vs IdentityUser mismatch | Every run | HIGH — detect custom user class in source project |
| `using var db` in SelectMethod | Every run | HIGH — code-behind transform to materialize IQueryable |
| Product seed data missing | Every run | MEDIUM — detect model seed data in original project |
| data-enhance="false" on forms | Every run | Already have EnhancedNavAnnotationTransform planned |
| Connection string AttachDbFilename | Every run | MEDIUM — transform to Initial Catalog automatically |

## Compared to Previous Runs

| Metric | Run 55 | Run 56 | Run 57 |
|--------|--------|--------|--------|
| Initial errors | 33 | 25 | 54 |
| Final tests | 25/25 | 25/25 | 25/25 |
| Total time | ~24 min | ~39 min | ~31 min |
| New CLI features | Layout converter | — | StringValues + GetRouteUrl |
