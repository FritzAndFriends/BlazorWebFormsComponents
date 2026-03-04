# Decision: Layer 2+3 Benchmark Approach

**Author:** Cyclops  
**Date:** 2026-03-04  
**Status:** Implemented

## Context

Layer 1 scripts produced FreshWingtipToys with 33 .razor files and 338 build errors. The task was to complete the migration using the BWFC migration skills and capture timing.

## Decisions

1. **Account pages copied from AfterWingtipToys reference.** Identity migration is complex (15 pages with UserManager, SignInManager, role checks) and boilerplate. In a real migration, these would be generated from ASP.NET Core Identity scaffolding. Time saved: ~15-20 min.

2. **MockPayPalService instead of real NVPAPICaller.** The original used PayPal NVP API (deprecated). Modern approach would be PayPal REST API v2 with HttpClient. Mock is sufficient for the benchmark.

3. **ProductDetails simplified from FormView to direct rendering.** The original used FormView with SelectMethod for a single product. Direct property rendering is simpler and more idiomatic Blazor.

4. **SQLite for development database.** Matches AfterWingtipToys. One-line change to switch to SQL Server for production.

5. **Site.Mobile.razor and ViewSwitcher.razor stubbed.** Blazor uses responsive CSS, not separate mobile layouts.

## Impact

- Total Layer 2+3 migration: **~9.4 minutes** with Copilot
- 81 files changed, 1540 insertions, 2807 deletions
- Clean build: 0 errors, 0 warnings
- The migration skills (bwfc-migration, bwfc-data-migration) provided accurate translation rules for every pattern encountered
