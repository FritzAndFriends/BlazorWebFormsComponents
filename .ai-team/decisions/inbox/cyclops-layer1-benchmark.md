# Decision: Layer 1 Benchmark Baseline Established

**By:** Cyclops
**Date:** 2026-03-04
**Status:** Informational

## What

Ran bwfc-scan.ps1 and bwfc-migrate.ps1 against WingtipToys to establish Layer 1 benchmark baselines. Results saved to `docs/migration-tests/wingtiptoys-2026-03-04/`.

## Key Numbers

- **Scan:** 0.9s, 32 files, 230 controls, 100% BWFC coverage
- **Migrate:** 2.4s, 276 transforms, 33 .razor files generated, 18 manual items flagged
- **Build:** 338 errors (expected — code-behind not yet transformed)

## Observations for the Team

1. **bwfc-migrate.ps1 scaffold targets net8.0** — should be updated to detect repo TFM or default to net10.0. Also generates NuGet PackageReference instead of ProjectReference for local dev.
2. **14 unconverted code blocks** are complex data binding expressions (`<%#: String.Format(...)%>`, `<%#: GetRouteUrl(...)%>`). These should be targeted by Layer 2 Copilot skill transforms.
3. **Register directives** are stripped but the component tag prefixes (`uc:`, `friendlyUrls:`) remain in markup as bare tags. Layer 2 needs to resolve these to Blazor component references.
4. **All 338 build errors are in code-behind** — markup transforms are clean. This validates the Layer 1 / Layer 2 boundary.

## Impact

- Beast: benchmark data is ready at `docs/migration-tests/wingtiptoys-2026-03-04/layer1-results.md`
- Forge: the 14 unconverted expressions + 4 Register directives define Layer 2 scope for markup
- All: `samples/FreshWingtipToys/` is the new fresh migration target — do NOT touch `samples/AfterWingtipToys/`
