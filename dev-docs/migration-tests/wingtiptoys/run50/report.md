# WingtipToys Migration Benchmark — Run 50 (Iteration 4 of 5)

## Summary

| Metric | Value |
|--------|-------|
| **Migration Time** | 10.8s |
| **Initial Build Errors** | 36 (down from 47 in run 49) |
| **Final Build Errors** | 0 |
| **Acceptance Tests** | 25/25 ✅ |
| **CLI Tests** | 639/639 ✅ |

## CLI Changes (Pre-Migration)

### Master Page Files Excluded from Compile Surface
**Problem:** Master page files (Site.Master → Site.razor, Site.Mobile.Master → Site.Mobile.razor) were written to the output as `.razor` files, but they are NOT valid Blazor layouts. They reference `ContentPlaceHolder`, `@ChildComponents`, and other patterns that don't compile. The scaffolded `MainLayout.razor` already serves as the layout.

**Fix:** Added early return in `MigrationPipeline.ProcessSourceFileAsync()` for `FileType.Master` files — writes them as artifacts under `migration-artifacts/codebehind/` instead of the compile surface. Added manual item with category `bwfc-master-page` for tracking.

**Impact:** Eliminated 11 build errors (Site.razor had 7, Site.Mobile.razor had 4).

## L2 Repair Items

| # | Issue | Fix | Category |
|---|-------|-----|----------|
| 1 | ViewSwitcher.razor (mobile view switcher) | Deleted | Obsolete |
| 2 | ShoppingCart.razor — 10+ errors | Full rewrite with InteractiveServer | Data page |
| 3 | ProductList.razor — missing `productList` | Fixed data binding with DI | Data binding |
| 4 | ProductDetails.razor.cs — `new ProductContext()` | Switched to DI | DI pattern |
| 5 | Default.razor.cs — `Server.Transfer(url, false)` | Replaced with NavigationManager | API mismatch |
| 6 | ErrorPage.razor.cs — `HttpException`, `ExceptionUtility` | Simplified | API mismatch |
| 7 | Logic/AddProducts.cs — `new ProductContext()` | Handled via seed data | DI pattern |
| 8 | MainLayout.razor — needs full content | Rebuilt with navbar, sidebar, auth | Layout |
| 9 | Program.cs — SQLite, session, auth, seeding | Complete rewrite | Infrastructure |
| 10 | AddToCart — minimal API endpoint | Added to Program.cs | API endpoint |

## Error Trend

| Run | Migration Time | Initial Errors | L2 Repairs | Tests |
|-----|---------------|----------------|------------|-------|
| 46 | ~10s | 81 | ~15 | 25/25 |
| 47 | 9.4s | 154 | ~12 | 25/25 |
| 48 | 5.5s | 86 | ~14 | 25/25 |
| 49 | 16.5s | 47 | ~12 | 25/25 |
| **50** | **10.8s** | **36** | **~10** | **25/25** |

## Key Observations

1. **Error count trend is clearly improving**: 81 → 154 → 86 → 47 → 36. Run 47's spike was due to P5 (additional DbContext registration) which initially added more files. Since then, each fix has consistently reduced errors.

2. **Remaining L2 items are converging on a stable set:**
   - ViewSwitcher deletion (mobile-specific, should be auto-handled)
   - ShoppingCart full rewrite (complex interactive page — hard to automate)
   - `new DbContext()` → DI pattern (could be a CLI transform)
   - MainLayout content (needs app-specific nav/auth — hard to fully automate)
   - Program.cs infrastructure (SQLite, session, seeding — partially automated)

3. **Two highest-impact remaining CLI improvements:**
   - Auto-quarantine ViewSwitcher and similar mobile-only user controls
   - Transform `new DbContext()` to DI injection in code-behind

## Next Run Focus (Run 51 — Final)

For the final run, consider:
1. Auto-quarantine or delete `.Mobile.` user controls (ViewSwitcher, etc.)
2. Code-behind transform to replace `new XxxContext()` with injected property
