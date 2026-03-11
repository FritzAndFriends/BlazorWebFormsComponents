# WingtipToys Run 8 — Summary

| Metric | Value |
|--------|-------|
| **Date** | 2026-03-06 |
| **Branch** | `squad/run8-improvements` |
| **Score** | ✅ **14/14 acceptance tests passed (100%)** |
| **Render Mode** | InteractiveServer |
| **Total Time** | ~1 hour 55 minutes |
| **Layer 1 Transforms** | 366 across 152 files in 3.3 seconds |
| **Static Files Copied** | 80 |

## Key Achievements

Run 8 was the **first WingtipToys migration to achieve 100% acceptance test pass rate**. The migration used the improved `bwfc-migrate.ps1` Layer 1 script and updated migration skills.

- Layer 1 completed 366 transforms in 3.3 seconds
- Layer 2 required 2 manual build iterations (~26 minutes)
- Phase 3 (test & fix) took ~1h 20m over 3 fix iterations
- All 14 acceptance tests passed

## Key Discovery

The breakthrough was recognizing that **Blazor Interactive Server mode and HTTP sessions are fundamentally incompatible** during WebSocket circuits. All session-dependent operations (auth, cart) required minimal API endpoints with HTML form POSTs instead of Blazor interactive event handlers.

## Pipeline Timing

| Phase | Duration | Notes |
|-------|----------|-------|
| Phase 1 — Automated Script | 2 min 39s | 366 transforms, 80 static files, 46 review items |
| Phase 2 — Manual Fixes + Build | ~26 min | 33 files rewritten, 14 modified, 8 created |
| Phase 3 — Test & Fix | ~1h 20 min | 3 fix iterations to reach 14/14 |
| Phase 4 — Report | ~6 min | Documentation and screenshots |

## Remaining Manual Work

At 14/14 functional tests passing, all core features worked. However, the run revealed that visual integrity (CSS, images) was not yet being tested — this gap was exposed in Run 9.

## Full Report

See the complete report at [`dev-docs/migration-tests/wingtiptoys-run8-2026-03-06/REPORT.md`](../../dev-docs/migration-tests/wingtiptoys-run8-2026-03-06/REPORT.md).
