# Run 5 Timing Data

**Date:** 2026-03-04
**Start:** 14:50:43
**End:** 15:00:43

| Phase | Duration |
|-------|----------|
| Layer 1 (script) | 3.25s |
| Layer 2 — Data models | ~45s |
| Layer 2 — DbContext + seed | ~30s |
| Layer 2 — Services | ~30s |
| Layer 2 — Layout/Auth | ~60s |
| Layer 2 — Program.cs | ~20s |
| Layer 2 — Page fixes | ~180s |
| Layer 2 — Stubs | ~60s |
| Layer 2 — Static assets | ~15s |
| Layer 2 Total | ~7m 20s |
| Build (final, clean) | 4.93s |
| Build fixes | ~30s (2 fix rounds) |
| **Total** | **~10m 0s** |

## Notes

- Layer 1 script: 3.25s, 309 transforms, 32 files, 79 static files, 30 manual items
- Layer 2 total: ~7m 20s wall-clock for all manual code
- Build fix round 1: 33 errors (Account/Checkout pages had un-stubbed Web Forms markup) — 17.4s build
- Build fix round 2: 2 errors (missing `RenderMode` static using) — 4.25s build
- Final clean build: 4.93s, 0 errors, 0 warnings
- Total wall-clock: ~10 minutes from start to clean build
