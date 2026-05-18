# WingtipToys Migration Test - Run NN

**Date:** YYYY-MM-DD HH:MM:SS zzz  
**Branch:** `branch-name`  
**Operator:** Copilot / user / agent name  
**Requested by:** requestor name

---

## Summary

| Metric | Value |
|--------|-------|
| Source project | `samples/WingtipToys/WingtipToys` |
| Output project | `samples/AfterWingtipToys` |
| Toolkit entry point | `migration-toolkit/scripts/bwfc-migrate.ps1` |
| Report folder | `dev-docs/migration-tests/wingtiptoys/runNN` |
| Total wall-clock time | `TBD` |
| Build result | `TBD` |
| Acceptance tests | `TBD` |
| Final status | `SUCCESS / FAILED` |

## Executive Summary

One short paragraph covering the overall result, total runtime, and whether the migrated app met the acceptance bar.

## Timing

> Populated from the `run_timing` SQL table. Durations are wall-clock minutes.

| Phase | Started | Finished | Duration | Notes |
|-------|---------|----------|----------|-------|
| Preparation | | | | Run numbering, folder cleanup, report folder creation |
| L1 toolkit migration | | | | `bwfc-migrate.ps1` invocation |
| Build repair | | | | Layer 2 compile-error fixes |
| Startup triage | | | | DI/config/DB root-cause fixes |
| Acceptance tests | | | | Playwright runs + targeted repairs |
| Screenshots | | | | Evidence capture |
| Report | | | | Write-up |
| **Total** | | | | **Start of Phase 0 → end of Phase 6** |

## Commands

```powershell
# Clear output
Get-ChildItem samples\AfterWingtipToys -Force | Remove-Item -Recurse -Force

# Run migration toolkit
pwsh -File migration-toolkit\scripts\bwfc-migrate.ps1 -Path samples\WingtipToys -Output samples\AfterWingtipToys -Verbose

# Build
dotnet build samples\AfterWingtipToys\WingtipToys.csproj

# Run app
dotnet run --project samples\AfterWingtipToys\WingtipToys.csproj

# Acceptance tests
$env:WINGTIPTOYS_BASE_URL = "https://localhost:5001"
dotnet test src\WingtipToys.AcceptanceTests\WingtipToys.AcceptanceTests.csproj --verbosity normal
```

## What Worked Well

1. `TBD`
2. `TBD`
3. `TBD`

## What Didn't Work Well

1. `TBD`
2. `TBD`
3. `TBD`

## Build Result

Summarize the final build state, warning/error counts, and the major error classes encountered during recovery.

## Acceptance Test Result

| Metric | Value |
|--------|-------|
| Total | `TBD` |
| Passed | `TBD` |
| Failed | `TBD` |
| Skipped | `TBD` |

Describe any targeted fixes needed before the final pass.

## Toolkit Gaps Exposed by This Run

Document the concrete gaps revealed by the run so future CLI/toolkit work has actionable follow-up items.

1. `TBD`
2. `TBD`
3. `TBD`

## Screenshot Gallery

| Page | Screenshot |
|------|------------|
| Home | ![Home](images/01-home.png) |
| Products | ![Products](images/02-products.png) |
| Product Details | ![Product Details](images/03-product-details.png) |
| Shopping Cart | ![Shopping Cart](images/04-shopping-cart.png) |
| Login | ![Login](images/05-login.png) |
| About | ![About](images/06-about.png) |

## Notes

Add anything important that did not fit the sections above: environment quirks, known nondeterminism, follow-up recommendations, or references to raw artifacts.
