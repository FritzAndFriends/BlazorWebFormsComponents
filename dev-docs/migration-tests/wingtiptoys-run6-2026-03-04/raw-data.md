# WingtipToys Run 6 — Raw Migration Data

**Date:** 2026-03-04
**Executor:** Forge (Lead / Web Forms Reviewer)
**Source:** `samples/WingtipToys/WingtipToys/` (32 Web Forms files)
**Output:** `samples/AfterWingtipToys/`
**Toolkit:** `migration-toolkit/scripts/bwfc-migrate.ps1` (latest)

## Timing

| Phase | Duration | Notes |
|-------|----------|-------|
| Layer 1 (script) | 4.58s | 269 transforms, 32 files processed, 79 static files, 28 manual items |
| Build 1 (initial) | 7.99s | 4 errors (NuGet auth), 12 warnings |
| Fix: csproj/scaffold | ~30s | ProjectReference, EF Core packages, _Imports namespaces, App.razor CSS links |
| Fix: models/data | ~45s | 5 model files (Category, Product, CartItem, Order, OrderDetail), ProductContext |
| Fix: services | ~15s | CartStateService created |
| Fix: pages | ~60s | Default, ProductList, ProductDetails, AddToCart, ShoppingCart — wired up Items/OnInitializedAsync |
| Fix: layout/stubs | ~45s | MainLayout rewritten, 15 Account stubs, 1 Admin stub, ErrorPage simplified |
| Fix: static assets | 0s | **Already in wwwroot/ — no manual fix needed!** |
| Build 2 (post-fixes) | 15.12s | 8 errors — `@rendermode` in _Imports.razor invalid, render mode conflict |
| Fix: _Imports rendermode | ~5s | Removed `@rendermode InteractiveServer` from _Imports.razor |
| Build 3 (clean) | 4.15s | 0 errors, 4 warnings (NuGet preview version resolution) |
| Fix: EF Core version | ~5s | Changed to `10.0.0-preview.*` wildcard |
| Build 4 (final) | 4.40s | **0 errors, 0 warnings** |
| **Total Layer 2** | **~3 min 25s** | |
| **Total (all phases)** | **~4 min 30s** | Script + all fixes + all builds |

## Script Output Summary

- Files processed: 32
- Transforms applied: 269
- Static files copied: 79
- Items needing review: 28
- Scaffold files: 6 (WingtipToys.csproj, _Imports.razor, Program.cs, launchSettings.json, App.razor, Routes.razor)
- Unconvertible stubs auto-generated: 6 (ShoppingCart, 5× Checkout)

### Manual Review Items Breakdown

| Category | Count | Details |
|----------|-------|---------|
| CodeBlock | 7 | Unconverted `<%#:` expressions in GetRouteUrl calls and Account pages |
| ContentPlaceHolder | 1 | Site.Mobile.Master FeaturedContent |
| GetRouteUrl | 2 | Need @inject GetRouteUrlHelper |
| RegisterDirective | 4 | uc: tag prefix verification |
| SelectMethod | 8 | Need Items="@_data" + OnInitializedAsync |
| UnconvertibleStub | 6 | Identity/Auth/Payment pages auto-stubbed |

## Build Progression

| Round | Errors | Warnings | Fix Applied |
|-------|--------|----------|-------------|
| 1 | 4 | 12 | Initial build — NuGet auth failure (Fritz.BlazorWebFormsComponents package) |
| 2 | 8 | 134 | After csproj/models/pages fixes — `@rendermode` in _Imports invalid |
| 3 | 0 | 4 | After removing `@rendermode` from _Imports — only NuGet version warnings |
| 4 | 0 | 0 | After EF Core version wildcard — **CLEAN BUILD** |

## Files Created/Modified in Layer 2

| File | Action | Category |
|------|--------|----------|
| WingtipToys.csproj | Modified | csproj — ProjectReference + EF Core packages |
| _Imports.razor | Modified | scaffold — added namespaces, removed @rendermode |
| Components/App.razor | Modified | scaffold — added CSS links for bootstrap & Site.css |
| Program.cs | Modified | scaffold — EF Core + SQLite + CartStateService + seed |
| Models/Category.cs | Created | models — EF Core model with nullable refs |
| Models/Product.cs | Created | models — EF Core model with nullable refs |
| Models/CartItem.cs | Created | models — EF Core model |
| Models/Order.cs | Created | models — EF Core model |
| Models/OrderDetail.cs | Created | models — EF Core model |
| Data/ProductContext.cs | Created | data — EF Core DbContext with seed |
| Services/CartStateService.cs | Created | services — replaces Session-based ShoppingCartActions |
| Default.razor | Modified | pages — removed Title ref, added PageTitle |
| Default.razor.cs | Modified | pages — ComponentBase stub |
| ProductList.razor | Modified | pages — Items="@_products", fixed links, removed <%#: blocks |
| ProductList.razor.cs | Modified | pages — EF Core query with category filter |
| ProductDetails.razor | Modified | pages — Items="@_products", conditional render |
| ProductDetails.razor.cs | Modified | pages — EF Core query by productID |
| ShoppingCart.razor | Modified | pages — functional cart display with CartStateService |
| AddToCart.razor | Modified | pages — functional add-to-cart with EF Core + CartStateService |
| AddToCart.razor.cs | Deleted | pages — code inlined into .razor |
| About.razor | Modified | pages — removed Title ref, added PageTitle |
| Contact.razor | Modified | pages — removed Title ref, added PageTitle |
| ErrorPage.razor | Modified | pages — simplified to compilable stub |
| Components/Layout/MainLayout.razor | Modified | layout — full rewrite: categories via EF Core, removed LoginView/GetRouteUrl |
| Components/Layout/MainLayout.razor.cs | Deleted | layout — Web Forms code-behind |
| Components/Layout/Site.MobileLayout.razor | Deleted | layout — unused mobile layout |
| Components/Layout/Site.MobileLayout.razor.cs | Deleted | layout — unused |
| Account/*.razor (15 files) | Modified | stubs — all replaced with compilable stubs |
| Account/*.razor.cs (15 files) | Deleted | stubs — Web Forms code-behinds removed |
| Admin/AdminPage.razor | Modified | stubs — compilable stub |
| Admin/AdminPage.razor.cs | Deleted | stubs — Web Forms code-behind removed |
| About.razor.cs | Deleted | cleanup — Web Forms code-behind |
| Contact.razor.cs | Deleted | cleanup — Web Forms code-behind |
| ErrorPage.razor.cs | Deleted | cleanup — Web Forms code-behind |
| ViewSwitcher.razor | Deleted | cleanup — user control not needed |
| ViewSwitcher.razor.cs | Deleted | cleanup — code-behind |

## Run 5 → Run 6 Enhancement Validation

### 1. TFM Fix: net10.0 ✅ FIRED

**Run 5:** Script scaffolded `net8.0` → required manual csproj edit + missing InteractiveServer import fix (2 build fix rounds)
**Run 6:** Script scaffolds `net10.0` (line 139 of bwfc-migrate.ps1) → TFM was correct out of the box. No TFM-related build errors.

**Impact:** Eliminated 1 manual fix step and 1 build fix round. Saved ~15s.

### 2. SelectMethod TODO: BWFC-aware ✅ FIRED

**Run 5:** Generic "inject a service" guidance → developers replaced ListView/FormView with raw `@foreach` + HTML
**Run 6:** TODO now says: `Replace SelectMethod="X" with Items="@_data" parameter on this BWFC data control. Load _data in OnInitializedAsync: _data = await yourDbContext.YourEntities.ToListAsync();`

8 SelectMethod instances converted with BWFC-aware guidance. All data controls preserved as BWFC components (ListView, FormView) with Items parameter.

**Impact:** Preserved BWFC data controls instead of replacing with raw HTML. Saved ~60-120s of unnecessary manual rewrite. This is the single highest-impact enhancement.

### 3. wwwroot Path: Static files copy correctly ✅ FIRED

**Run 5:** Static files copied to project root → required manual relocation to wwwroot/ (~15s)
**Run 6:** Script copies 79 static files to `wwwroot/` subdirectory automatically (lines 1166-1181). CSS in `wwwroot/Content/`, images in `wwwroot/Catalog/Images/` and `wwwroot/Images/`, fonts in `wwwroot/fonts/`.

**Impact:** Eliminated entire "Fix: static assets" phase. Saved ~15s.

### 4. Compilable Stubs: Auto-generated ✅ FIRED

**Run 5:** 28+ unconvertible pages required manual stubbing (~60s)
**Run 6:** Script detected 6 unconvertible pages (ShoppingCart + 5 Checkout pages) via pattern matching for `Session[`, `SignInManager`, `PayPal`, `Checkout` and auto-generated compilable stubs with `@page` directive and `@code {}` block.

**Impact:** 6 pages auto-stubbed. Note: Account pages (15) were NOT auto-stubbed by the script — they don't trigger the unconvertible patterns (no Session[], SignInManager in the .aspx markup, only in .aspx.cs). Manual stubbing was still needed for Account + Admin (16 pages). Saved ~15s vs Run 5's full 28-page stub effort.

**Partial success:** The stub detection works on .aspx content, but Account pages reference Identity in code-behind only. Consider scanning .aspx.cs files too for stub detection.

## Comparison vs Run 5

| Metric | Run 5 | Run 6 | Delta |
|--------|-------|-------|-------|
| Script time | 3.25s | 4.58s | +1.33s (more transforms) |
| Transforms | 309 | 269 | -40 (stubs replace full transforms for 6 pages) |
| Manual review items | 30 | 28 | -2 |
| Auto-generated stubs | 0 | 6 | +6 |
| Static files in wwwroot | ❌ (project root) | ✅ (wwwroot/) | Fixed |
| TFM | net8.0 | net10.0 | Fixed |
| SelectMethod TODO | Generic | BWFC-aware (Items=@_data) | Fixed |
| Build fix rounds | 2 | 2 | Same (but different causes) |
| Layer 2 manual time | ~440s | ~205s | **-235s (53% reduction)** |
| Total time | ~10 min | ~4.5 min | **-5.5 min (55% reduction)** |
| Final build | 0 errors, 0 warnings | 0 errors, 0 warnings | Same |

## Key Findings

1. **55% total time reduction** from Run 5 → Run 6, matching the projected ~4.5 min estimate from the improvement analysis.
2. **SelectMethod BWFC-aware TODO** was the highest-impact single enhancement — it preserved all data controls as BWFC components.
3. **wwwroot/ path fix** eliminated an entire manual fix category.
4. **net10.0 TFM** eliminated a build-fix round.
5. **Stub generation partial** — covers Session/Identity in .aspx but misses Identity-only-in-code-behind pages (Account/*.aspx). Recommend scanning .aspx.cs files.
6. **`@rendermode InteractiveServer` in _Imports.razor** is invalid in .NET 10 — the scaffold generates it but it must be removed. This is a script bug to fix.
7. **NuGet PackageReference vs ProjectReference** — script scaffolds `Fritz.BlazorWebFormsComponents` NuGet ref, but local dev needs ProjectReference. Not a bug per se (NuGet is correct for external users), but benchmark requires manual change.

## Remaining Manual Work Breakdown

| Category | Time | What |
|----------|------|------|
| csproj edits | ~30s | ProjectReference, EF Core packages |
| EF Core models | ~45s | 5 model files from Web Forms originals |
| DbContext + seed | ~15s | ProductContext with Seed() method |
| CartStateService | ~15s | Session → scoped DI service |
| Page data binding | ~60s | 5 pages: wire Items, OnInitializedAsync, EF Core queries |
| Layout rewrite | ~20s | MainLayout: categories, removed LoginView/GetRouteUrl |
| Account/Admin stubs | ~15s | 16 pages manually stubbed |
| Build fix: _Imports | ~5s | Remove invalid @rendermode directive |

**Inherently manual:** EF Core models, DbContext, business logic services, data binding wiring. These cannot be automated without understanding the data model.
