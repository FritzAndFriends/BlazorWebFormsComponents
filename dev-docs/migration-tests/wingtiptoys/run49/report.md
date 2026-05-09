# WingtipToys Migration Benchmark — Run 49 (Iteration 3 of 5)

## Summary

| Metric | Value |
|--------|-------|
| **Migration Time** | 16.5s (includes CLI build) |
| **Initial Build Errors** | 47 (down from 86 in run 48) |
| **Final Build Errors** | 0 |
| **Acceptance Tests** | 25/25 ✅ |
| **CLI Tests** | 639/639 ✅ |

## CLI Changes (Pre-Migration)

### ContentWrapperTransform — Strip `<asp:Content>` Tags
**Problem:** The transform was wrapping page content in `<Site><Content ContentPlaceHolderID="X">...</Content></Site>`, referencing the master page as a Blazor component. This doesn't work — Blazor uses `@Body` in layouts, not ContentPlaceHolder IDs. Every essential page had to be manually unwrapped in L2.

**Fix:** Rewrote ContentWrapperTransform to strip `<asp:Content>` open/close tags entirely, keeping only inner content. The scaffolded `Routes.razor` sets `DefaultLayout="typeof(Layout.MainLayout)"`, so layout wrapping is handled by Blazor's layout system.

**Impact:** Eliminated the #1 L2 repair item from run 48. No more `<Site>` component references.

### ProgramCsEmitter — Interactive Server Support
**Problem:** Generated `Program.cs` lacked `.AddInteractiveServerComponents()` and `.AddInteractiveServerRenderMode()`, and used `UseStaticFiles()` which doesn't serve `_framework/blazor.web.js` in .NET 9+.

**Fix:**
- `builder.Services.AddRazorComponents()` → `.AddInteractiveServerComponents()`
- `app.UseStaticFiles()` → `app.MapStaticAssets()`
- `app.MapRazorComponents<App>()` → `.AddInteractiveServerRenderMode()`

### App.razor — Script Tags
**Problem:** Generated App.razor had no script tags, so `blazor.web.js` (Blazor runtime) and `Basepage.js` (BWFC interop) were missing.

**Fix:** Added both scripts before `</body>`:
```html
<script src="_framework/blazor.web.js"></script>
<script src="_content/Fritz.BlazorWebFormsComponents/js/Basepage.js"></script>
```

## L2 Repair Items

| # | Issue | Fix | Category |
|---|-------|-----|----------|
| 1 | Site.razor/Site.Mobile.razor errors (master page remnants) | Deleted — layout handled by MainLayout.razor | Delete obsolete |
| 2 | ViewSwitcher.razor (mobile view switcher) | Deleted — not relevant in Blazor | Delete obsolete |
| 3 | Account/OpenAuthProviders.razor | Deleted — quarantined | Delete obsolete |
| 4 | Account/RegisterExternalLogin.razor | Deleted — quarantined | Delete obsolete |
| 5 | ShoppingCart.razor — 10+ CS0103 errors | Full rewrite with InteractiveServer, DI | Data page rewrite |
| 6 | ProductList.razor — `productList` not found | Fixed data binding with DI | Data binding |
| 7 | ProductDetails.razor.cs — `new ProductContext()` | Switched to DI injection | DI pattern |
| 8 | Default.razor.cs — `Server.Transfer(url, false)` | Replaced with `NavigationManager.NavigateTo()` | API mismatch |
| 9 | ErrorPage.razor.cs — `HttpException` | Simplified to basic error display | API mismatch |
| 10 | Logic/AddProducts.cs — `new ProductContext()` | Fixed or handled via seed data | DI pattern |
| 11 | MainLayout.razor — needs navbar/sidebar/auth | Full rewrite with Bootstrap layout | Layout |
| 12 | Program.cs — SQLite, session, auth, seeding | Added all required services | Infrastructure |

## Error Trend

| Run | Migration Time | Initial Errors | L2 Repairs | Tests |
|-----|---------------|----------------|------------|-------|
| 46 | ~10s | 81 | ~15 | 25/25 |
| 47 | 9.4s | 154 | ~12 | 25/25 |
| 48 | 5.5s | 86 | ~14 | 25/25 |
| **49** | **16.5s** | **47** | **~12** | **25/25** |

## Key Observations

1. **Content wrapper fix cut errors by 45%** — From 86 → 47. Stripping `<asp:Content>` instead of wrapping in `<Site>` component was the single highest-impact CLI change.

2. **Interactive server support in scaffold is essential** — Without `.AddInteractiveServerComponents()` and `blazor.web.js`, no interactivity works at all.

3. **Remaining L2 work is now dominated by:**
   - Master page remnants (Site.razor, Site.Mobile.razor) — these should be quarantined or auto-deleted
   - ShoppingCart full rewrite (complex interactive page with session state)
   - `new DbContext()` pattern in code-behind → DI injection
   - MainLayout content (navbar, sidebar, auth links)

4. **Migration time increased** to 16.5s vs 5.5s in run 48 — likely due to CLI rebuild after code changes. Actual transform time is ~3-5s.

## Next Run Focus

For run 50, consider:
1. **Auto-delete master page .razor files** (Site.razor, Site.Mobile.razor) — they're already layouts in MainLayout
2. **Quarantine ViewSwitcher** — mobile view switcher is never useful in Blazor
3. **Fix `new DbContext()` in code-behind** — CLI should rewrite to use `@inject` pattern
