# Run 8 — WingtipToys Migration Benchmark Report

**Date:** 2026-03-05
**Source:** WingtipToys (ASP.NET Web Forms 4.5, circa 2013)
**Target:** Blazor Server (.NET 10, BWFC components)
**Branch:** `squad/event-handler-investigation`

---

## Executive Summary

Run 8 migrated the WingtipToys Web Forms demo app to Blazor Server using the BWFC migration toolkit in two layers. **Layer 1** (automated script) completed in **2.19 seconds**, transforming 32 Web Forms files with 333 regex-based transforms and copying 79 static assets. **Layer 2** (agent-assisted implementation) took the skeleton output and built a functional shopping application with EF Core data layer, cart services, and 7 working pages.

The core shopping flow works end-to-end: **Homepage → Product Listing → Product Details → Add to Cart → Shopping Cart**. CSS styling, product images, and Bootstrap layout are all functional.

**What Run 8 fixed vs Run 7:** Run 7 required 6 separate manual patches over multiple days (CSS, images, UseStaticFiles, auth services, AddToCart implementation). Run 8 incorporated all those learnings into the toolkit and completed Layer 2 properly in a single pass.

---

## Timing

| Phase | Duration | Files Processed |
|-------|----------|----------------|
| **Layer 1** (bwfc-migrate.ps1) | **2.19 seconds** | 32 pages → 333 transforms, 79 static files copied |
| **Layer 2** (agent implementation) | ~3 minutes | 22 files created/modified (models, services, data, pages) |
| **Build verification** | 2.10 seconds | 0 errors, 0 warnings |
| **Total** | ~5 minutes | 279 output files |

---

## Source Inventory (WingtipToys Web Forms)

| Category | Count |
|----------|-------|
| .aspx/.ascx/.master pages | 32 |
| Code-behind files (.cs) | 32 |
| Static assets (css, js, images, fonts) | 79 |
| **Total source files** | **572** (including bin/obj/packages) |

---

## Output Inventory (Run8WingtipToys Blazor)

| Category | Count |
|----------|-------|
| .razor files | 35 |
| .cs files (code-behind + models + services + data) | 43 |
| Static assets in wwwroot/ | 79 |
| Infrastructure files (csproj, Program.cs) | 2 |
| **Total output files** | **279** |

### Page Status Breakdown

| Status | Count | Pages |
|--------|-------|-------|
| **Functional** (working with data) | 7 | ProductList, ProductDetails, AddToCart, ShoppingCart, Contact, ErrorPage, MainLayout |
| **Minimal** (static content, correct routing) | 2 | Default (homepage), About |
| **Stub** (out-of-scope, placeholder) | 21 | Account/* (13), Checkout/* (6), Admin (1), ViewSwitcher (1) |
| **Structural** (framework plumbing) | 5 | App.razor, Routes.razor, _Imports.razor, MainLayout, MobileLayout |

---

## What Worked (Layer 1 — Automated)

| Transform | Result |
|-----------|--------|
| `.aspx` → `.razor` rename | ✅ All 32 files converted |
| `asp:` prefix removal | ✅ BWFC components preserved (ListView, FormView, GridView, etc.) |
| `runat="server"` removal | ✅ Clean removal across all files |
| Web Forms expressions → Razor | ✅ Most `<%: %>` and `<%# %>` converted to `@(...)` |
| Master page → Layout component | ✅ Site.Master → MainLayout.razor with `@Body` |
| Static file copying | ✅ 79 files preserved in wwwroot/ with directory structure |
| CSS link extraction | ⚠️ Not extracted into App.razor (script bug — CSS links still needed manual addition) |
| `~/ ` URL rewriting | ✅ Converted to `/` paths |
| Code-behind copy with TODO annotations | ✅ All 32 code-behind files copied with annotations |
| `SelectMethod` → TODO annotation | ✅ 9 instances flagged with clear migration instructions |
| `LoginView` → `AuthorizeView` | ✅ Converted with auth service requirement flagged |
| Control preservation check | ✅ Detected 1 missing `<PlaceHolder>` in Site.Master |

### Layer 1 Gaps (Required Layer 2 Intervention)

| Gap | Count | Impact |
|-----|-------|--------|
| Unconverted code blocks (`<%#: GetRouteUrl(...)%>`) | 14 | Required manual URL replacement |
| `SelectMethod` removal needs service injection | 9 | Required EF Core + code-behind rewrite |
| Event handler signatures (sender, EventArgs) | 15 | Required Blazor EventCallback conversion |
| Missing data layer (models, DbContext, services) | — | Required full creation from scratch |
| `AddToCart.aspx` had inline HTML, not server-side code | — | Produced empty shell, needed full implementation |

---

## What Layer 2 Built

| Component | Files | Description |
|-----------|-------|-------------|
| **Models** | 5 | Product, Category, CartItem, Order, OrderDetail |
| **Data** | 1 | ProductContext with EF Core + SQLite + seed data |
| **Services** | 1 | CartStateService (in-memory cart with Add/Remove/Total) |
| **Program.cs** | 1 | EF Core, BWFC, Auth, UseStaticFiles, DB seeding |
| **App.razor** | 1 | CSS links for bootstrap.css and Site.css |
| **MainLayout** | 1 | Category navigation, navbar, logo, AuthorizeView |
| **ProductList** | 2 | ListView with data binding, image thumbnails, category filtering |
| **ProductDetails** | 2 | FormView with product image, description, price |
| **AddToCart** | 1 | Query parameter → DB lookup → cart service → confirmation |
| **ShoppingCart** | 1 | Cart display table with product, price, quantity, subtotal, total |

---

## What Didn't Work / Remaining Gaps

| Area | Status | Notes |
|------|--------|-------|
| **Account/Identity pages** (13 pages) | ❌ Stub | Full ASP.NET Identity migration out of scope — requires Identity scaffold |
| **Checkout flow** (6 pages) | ❌ Stub | PayPal integration, order processing — would need payment service |
| **Admin page** | ❌ Stub | CRUD for products/categories — needs GridView editing implementation |
| **CSS extraction in script** | ⚠️ Bug | `New-AppRazorScaffold -SourceRoot` didn't extract `<link>` tags from master page into App.razor |
| **GetRouteUrl expressions** | ⚠️ Not auto-converted | 14 instances of `<%#: GetRouteUrl(...)%>` survived Layer 1 — needed manual URL replacement |
| **LoginStatus component** | ⚠️ Limited | Converted to `AuthorizeView` but `LoginStatus` logout functionality not fully wired |
| **Cart persistence** | ⚠️ In-memory | CartStateService uses in-memory list (scoped per circuit) — not DB-backed like original |
| **Mobile layout** | ❌ Not functional | Site.MobileLayout.razor was converted but ViewSwitcher not implemented |

---

## Key Learnings (Carried Forward from Run 7)

1. **UseStaticFiles() is mandatory** — `MapStaticAssets()` alone doesn't serve subdirectory files reliably
2. **AuthorizeView crashes without auth services** — `AddCascadingAuthenticationState()` + `AddAuthorization()` required in Program.cs
3. **Image paths must match wwwroot structure** — DB stores filename only; templates must use correct wwwroot subdirectory path
4. **CSS extraction from master pages needs work** — Script regex exists but didn't fire for Run 8; manual App.razor CSS addition still needed
5. **Code-behind is the hard part** — Layer 1 copies files with TODOs but the actual EF Core/service wiring is 90% of the effort

---

## Comparison: Run 7 vs Run 8

| Metric | Run 7 | Run 8 |
|--------|-------|-------|
| Layer 1 time | ~2s | 2.19s |
| Manual patches needed after Layer 1 | 6 patches over multiple days | 0 — Layer 2 done in one pass |
| Shopping flow working | After 6 fixes | Immediately after Layer 2 |
| CSS/styling | Broken until patch 4 | Working from Layer 2 |
| Product images | Broken until patch 5 | Working from Layer 2 |
| AddToCart | Empty stub until patch 6 | Functional from Layer 2 |
| Total effort | ~4 hours across multiple sessions | ~5 minutes |
| Build status | 0 errors (after patches) | 0 errors (first build) |

---

## Recommendations

1. **Fix CSS extraction bug in bwfc-migrate.ps1** — The `-SourceRoot` parameter is passed but `<link>` tags aren't making it into App.razor
2. **Add Layer 2 skill for data layer scaffolding** — Model/DbContext/Service creation is the biggest manual effort; could be partially automated
3. **GetRouteUrl → href conversion** — Script should convert `GetRouteUrl("RouteName", new {...})` to simple href patterns where route table is known
4. **Identity migration deserves its own run** — 13 Account pages need ASP.NET Identity → Blazor Identity scaffold, separate benchmark
5. **Consider DB-backed cart** — For production migrations, the in-memory cart pattern doesn't match Web Forms session-based cart behavior
