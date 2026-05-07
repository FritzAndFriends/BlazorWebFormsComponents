# Decision: Run 41 benchmark follow-up items

**Date:** 2026-05-07T15:15:19-04:00  
**Author:** Bishop (Migration Tooling Dev)  
**Requested by:** Jeffrey T. Fritz  
**Status:** Proposed

## Context

WingtipToys benchmark Run 41 finished green (25/25 acceptance tests), but only after manual repair of three fresh-output regressions that the cumulative fixes were supposed to reduce: benchmark-path pages were still quarantined, static assets served as zero-length responses under the generated runtime, and SSR cart postbacks still needed manual antiforgery/form-name wiring.

## Decision

Treat Run 41 as a successful benchmark with three prioritized follow-up items for the CLI/runtime scaffold:

1. **Do not quarantine benchmark-critical commerce pages by default.**
   - Preserve or explicitly allowlist pages like `ProductList`, `ProductDetails`, `AddToCart`, and `ShoppingCart` on Wingtip-style fixtures.
   - Keep quarantine focused on non-benchmark account/admin/checkout/mobile/payment surfaces.

2. **Prefer classic static-file middleware for migrated sample scaffolds that rely on `wwwroot` asset trees.**
   - The Run 41 scaffold returned 200 with `Content-Length: 0` for `/Images/logo.jpg` and `/Catalog/Images/...` until `app.UseStaticFiles()` replaced `app.MapStaticAssets()`.
   - Fresh benchmark runtimes should default to whichever path reliably serves legacy copied assets without additional repair.

3. **Emit a complete SSR form-post contract for pages that use `Request.Form`.**
   - Middleware: `app.UseAntiforgery()`
   - Markup: `<AntiforgeryToken />`
   - Form identity: explicit `@formname`
   - Without all three, the shopping-cart update path either 400s or fails Playwright postback flows.

## Evidence from Run 41

- Final build: `samples\AfterWingtipToys\WingtipToys.csproj` succeeded with 31 warnings / 0 errors.
- Final acceptance: `src\WingtipToys.AcceptanceTests` passed 25/25 against `https://localhost:5001`.
- Quarantine evidence: `samples\AfterWingtipToys\migration-artifacts\quarantine-manifest.json`
- Report: `dev-docs\migration-tests\wingtiptoys\run41\report.md`

## Rationale

Run 41 proved the benchmark can still end green while preserving BWFC controls, but the repair loop is paying for avoidable scaffolding mistakes instead of true migration complexity. Tightening quarantine scope, static asset serving, and SSR form-post emission should reduce future Wingtip repair time materially without weakening compile safety.
