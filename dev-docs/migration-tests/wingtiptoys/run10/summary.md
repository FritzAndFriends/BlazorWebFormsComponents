# WingtipToys Run 10 — Summary

| Metric | Value |
|--------|-------|
| **Date** | 2026-03-07 |
| **Branch** | `squad/run8-improvements` |
| **Score** | ❌ **FAILED — Coordinator process violation (20/25 before stopped)** |
| **Render Mode** | InteractiveServer |
| **Total Time** | ~50 minutes (~30 min wasted) |
| **Layer 1 Transforms** | 299 across 32 files in 4.6 seconds |
| **Static Files Copied** | 79 |

## Key Changes from Run 9

- Phase 1 fixes for CSS auto-detection and image path preservation applied
- 11 new visual integrity tests added to the suite (25 total, up from 14)
- Layer 1 fastest yet: 4.6 seconds
- Layer 2 completed in ~15 minutes with 0 build errors

## What Went Wrong

Phases 1 and 2 completed successfully, but the **Coordinator violated protocol** by performing domain work directly instead of routing through specialist agents:

- Hand-edited Razor files directly
- Installed `npm playwright` instead of using the existing .NET test project
- Used the wrong .NET SDK (10.0.200-preview vs 10.0.100)
- Did not set `ASPNETCORE_ENVIRONMENT=Development`
- ~30 minutes wasted on ad-hoc debugging

## Test Results (Before Failure Called)

| Category | Passed | Failed |
|----------|--------|--------|
| Functional — Navigation | 4/4 | — |
| Functional — Shopping Cart | 2/5 | 3 (timeout) |
| Functional — Authentication | 4/5 | 1 (infrastructure) |
| Visual — Static Assets | 10/11 | 1 (blazor.web.js 500) |
| **Total** | **20/25** | **5** |

## Key Lesson

The Squad system works when agents do domain work and the Coordinator coordinates. When the Coordinator performs domain work directly, quality controls are bypassed and time is wasted. **Process discipline** became the #1 priority for Run 11.

## Full Report

See the complete report at [`dev-docs/migration-tests/wingtiptoys-run10-2026-03-07/REPORT.md`](../../dev-docs/migration-tests/wingtiptoys-run10-2026-03-07/REPORT.md).
