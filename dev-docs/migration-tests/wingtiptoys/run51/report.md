# WingtipToys Migration Benchmark — Run 51

## Run Metadata

| Field | Value |
|-------|-------|
| Date | 2026-05-09 |
| Branch | `feature/cli-optimizations` |
| Operator | Copilot CLI (automated benchmark loop, iteration 5 of 5) |
| Source | `samples/WingtipToys/WingtipToys/` |
| Output | `samples/AfterWingtipToys/` |
| Toolkit | `migration-toolkit/scripts/bwfc-migrate.ps1` |
| Tests | `src/WingtipToys.AcceptanceTests/` |

## CLI Changes This Run

1. **Mobile control exclusion** — `ViewSwitcher.ascx` and other mobile-related user controls are now excluded from the compile surface and written as artifacts only (same pattern as master page exclusion from run 50).

2. **DbContextInstantiationTransform** (Order 107) — Replaces `new XxxContext()` in page/control code-behind with `[Inject] public XxxContext XxxContext { get; set; }` DI property injection. Handles field declarations, using blocks, local variables, and standalone expressions. Adds `using Microsoft.AspNetCore.Components;` for the `[Inject]` attribute.

## Results Summary

| Metric | Value |
|--------|-------|
| Layer 1 migration time | 12.2 seconds |
| Initial build errors | **33** |
| Final build errors | **0** |
| Acceptance tests | **25/25 passed** |
| Total L2 repair time | ~19 minutes |

## What Worked Well

1. **Lowest initial error count yet** — 33 errors, down from 154 in run 47 (78% reduction across the 5-run loop).
2. **DbContextInstantiationTransform eliminated a class of errors** — Pages that did `new ProductContext()` now get proper DI injection automatically.
3. **Mobile control exclusion** — ViewSwitcher.ascx no longer causes compile errors.
4. **All core scaffolding is solid** — Program.cs, App.razor, _Imports.razor, identity stubs, project file all generate correctly.
5. **All 25 acceptance tests passed** on first test run after L2 repair.

## What Did Not Work Well

### Recurring L2 Items (consistent across runs 48-51)

1. **ShoppingCart.razor** — Always requires full rewrite. Complex interactive page with session state, cart calculations, and AJAX-style updates. The CLI generates markup that compiles but doesn't function correctly.

2. **MainLayout.razor** — Always needs manual content (navbar, sidebar, auth links, category navigation, footer). The CLI generates a minimal layout shell but can't infer the app's navigation structure from master page content.

3. **Program.cs** — Always needs SQLite configuration, data seeding, AddToCart endpoint, authentication endpoints. The scaffolder generates the framework but can't produce app-specific business logic.

4. **ProductList/ProductDetails** — Data binding fixes needed for DI injection of services and correct SelectMethod wiring.

5. **Default.razor.cs** — `Server.Transfer()` API doesn't exist in the shim; needs manual redirect conversion.

6. **ErrorPage.razor.cs** — `HttpException` doesn't exist in .NET Core; needs conversion to modern exception handling.

### Infrastructure Notes

- `blazor.web.js` enhanced navigation still requires `data-enhance-nav="false"` on links to non-Razor endpoints (AddToCart, logout).
- `navbar-fixed-top` must be changed to `navbar-static-top` to avoid Playwright click interception issues.

## Error Trend (Runs 47-51)

| Run | Iteration | Initial Errors | CLI Fix |
|-----|-----------|---------------|---------|
| 47 | 1 | 154 | Register all DbContext types in ProgramCsEmitter |
| 48 | 2 | 86 | Aggressive quarantine + LegacyHelperStub |
| 49 | 3 | 47 | Strip asp:Content, interactive server, blazor.web.js |
| 50 | 4 | 36 | Exclude master pages from compile surface |
| 51 | 5 | 33 | Exclude mobile controls, DbContext DI transform |

## Build Error Categories (33 initial errors)

- Missing service registrations / DI issues — resolved by L2 repair
- ShoppingCart complex logic — full rewrite needed
- Missing layout content — MainLayout rebuild
- Server.Transfer / HttpException — API mismatch fixes
- Minor namespace and using issues — quick fixes

## Toolkit Gaps Exposed

1. **ShoppingCart-class interactive pages** — Pages with heavy session state, AJAX updates, and business logic in event handlers need a specialized transform or template.
2. **Navigation structure inference** — CLI can't extract nav items from master page `<asp:Menu>` or `<asp:TreeView>` to populate MainLayout.
3. **Server.Transfer shim** — Not yet implemented in WebFormsPageBase.
4. **HttpException replacement** — Could be handled by a code-behind transform.
5. **Enhanced navigation conflicts** — CLI should auto-add `data-enhance-nav="false"` to non-page links.

## Acceptance Test Results

```
Test summary: total: 25, failed: 0, succeeded: 25, skipped: 0
```

All 25 tests passing:
- Home page, About, Contact, Login, Register pages render correctly
- Product catalog browsing and category filtering works
- Product details display correctly
- Shopping cart add/view/update/remove operations work
- Authentication flows (login, register, logout) function
- Navigation links work across the site
