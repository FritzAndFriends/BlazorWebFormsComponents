# WingtipToys Run 9 — Summary

| Metric | Value |
|--------|-------|
| **Date** | 2026-03-06 |
| **Branch** | `squad/run8-improvements` |
| **Score** | ❌ **FAILED — Visual regression (no CSS, broken images)** |
| **Render Mode** | InteractiveServer |
| **Total Time** | ~47 minutes |
| **Layer 1 Transforms** | 297 across 32 files in ~10 seconds |
| **Static Files Copied** | 79 |

## Key Changes from Run 8

- **59% faster** overall (47 min vs 1h 55m)
- Layer 2 completed in a **single Cyclops agent run** (~19 min) vs manual wiring (26 min)
- 15 P0+P1 script fixes from Forge analysis applied
- LoginView.razor fix: restored Web Forms template names (`AnonymousTemplate`/`LoggedInTemplate`)
- Updated migration skills for BWFC components and EF Core patterns

## What Went Wrong

While all 14 functional acceptance tests passed, the migrated app was **visually broken**:

1. **No CSS** — `<webopt:bundlereference>` CSS bundle references were dropped without generating replacement `<link>` tags. The navbar rendered as an unstyled bullet list.
2. **Broken images** — Layer 2 rewrote image `src` paths from `/Catalog/Images/` to `/Images/Products/` without moving the actual files in `wwwroot/`.

## Key Lesson

**Functional tests passing ≠ migration success.** Visual regression is ship-blocking. This run led to adding 11 visual integrity tests for subsequent runs (CSS loading, image HTTP 200 checks, screenshot layout verification).

## Improvements Delivered

Despite the visual failure, Run 9 validated major pipeline improvements:

| Metric | Run 9 | Run 8 | Change |
|--------|-------|-------|--------|
| Total time | ~47 min | 1h 55m | **-59%** |
| Manual review items | 34 | 46 | **-26%** |
| Layer 2 build iterations | 1 | 2 | **-50%** |

## Full Report

See the complete report at [`dev-docs/migration-tests/wingtiptoys-run9-2026-03-06/REPORT.md`](../../dev-docs/migration-tests/wingtiptoys-run9-2026-03-06/REPORT.md).
