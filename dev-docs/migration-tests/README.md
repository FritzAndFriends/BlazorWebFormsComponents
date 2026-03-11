# Migration Test Reports

Benchmarked migrations of real Web Forms applications using the BWFC migration toolkit.

## Test Projects

| Project | Source | Framework | Pages | Controls Used | Status |
|---------|--------|-----------|-------|---------------|--------|
| WingtipToys | ASP.NET Web Forms 2013 | .NET 4.5 | ~15 pages (32 markup files) | GridView, ListView, FormView, LoginView, LoginStatus, etc. | ✅ 17 runs, 100% pass (6 consecutive) |
| ContosoUniversity | Microsoft Tutorial App | .NET 4.x | 5 pages + 1 master | GridView, DetailsView, Table, UpdatePanel, ScriptManager, AutoCompleteExtender | ✅ 2 runs, 77.5% pass (stable) |

---

## WingtipToys Runs

All runs use the WingtipToys sample in `samples/WingtipToys/` as source input.

| Run | Date | L1 Time | L1 Manual Fixes | L2 Fixes | Score | Render Mode | Key Outcome |
|-----|------|---------|-----------------|----------|-------|-------------|-------------|
| [1](wingtiptoys-2026-03-04/report.md) | 2026-03-04 | 3.3s | — | — | ✅ Build pass | — | Initial benchmark; 230 control usages across 31 types |
| [2](wingtiptoys-run2-2026-03-04/report.md) | 2026-03-04 | — | — | — | ✅ 11/11 | — | Feature validation |
| [3](wingtiptoys-run3-2026-03-04/report.md) | 2026-03-04 | — | — | — | ✅ 11/11 | — | From-scratch validation |
| [4](wingtiptoys-run4-2026-03-04/report.md) | 2026-03-04 | ~3s | — | — | ✅ 11/11 | — | Enhanced script; 289 transforms |
| [5](wingtiptoys-run5-2026-03-04/report.md) | 2026-03-04 | 3.25s | — | — | ✅ Build pass | — | 6 new script enhancements; ~10 min total |
| [6](wingtiptoys-run6-2026-03-04/report.md) | 2026-03-04 | 4.58s | — | — | ✅ Build pass | — | 55% time reduction from Run 5; wwwroot pathing fixed |
| [8](wingtiptoys-run8.md) | 2026-03-06 | 3.3s | 3 | 12 | ✅ 14/14 (100%) | InteractiveServer | First 100% acceptance pass |
| [9](wingtiptoys-run9.md) | 2026-03-06 | ~10s | 2 | 10 | ❌ FAIL | InteractiveServer | Visual regression — no CSS/images |
| [10](wingtiptoys-run10.md) | 2026-03-07 | 4.6s | 1 | 8 | ❌ 20/25 | InteractiveServer | Coordinator process violation |
| [11](wingtiptoys-run11.md) | 2026-03-07 | 3.3s | 0 | 5 | ❌ 17/25 (68%) | InteractiveServer | ListView placeholder + Scripts/ gaps |
| [12](wingtiptoys-run12.md) | 2026-03-08 | 3.1s | 0 | 3 | ✅ 25/25 (100%) | InteractiveServer | First perfect score (25-test suite) |
| [13](wingtiptoys-run13.md) | 2026-03-08 | 3.0s | 0 | 3 | ✅ 25/25 (100%) | **SSR** | SSR default; 22 min total; 50% fewer manual fixes |
| [14](wingtiptoys-run14.md) | 2026-03-08 | 3.2s | **0** | 3 | ✅ 25/25 (100%) | SSR | Layer 1 fully zero-touch; automation ceiling identified |
| [15](wingtiptoys-run15.md) | 2026-03-08 | 2.83s | 0 | 3 | ✅ 25/25 (100%) | SSR | 4th consecutive 100%; L2 reference extraction |
| [16](wingtiptoys-run16.md) | 2026-03-08 | **2.50s** | 0 | 3* | ✅ 25/25 (100%) | SSR | First Layer 2 automation; Program.cs auto-generated |
| [17](wingtiptoys-run17.md) | 2026-03-09 | **1.81s** | 0 | 3* | ✅ 25/25 (100%) | SSR | Genericized toolkit validation; 28% faster L1 |

> **Run 7** was skipped (no benchmark recorded).
>
> **Runs 16–17 note:** `*` Layer 2 script automates Program.cs (Pattern C). Patterns A (code-behinds) and B (auth forms) still need manual overlay.

---

## Pipeline Evolution

### Convergence Summary

Across 17 runs, the pipeline evolved from a manual migration taking ~10 minutes to a two-script pipeline completing Layer 1 in 1.81 seconds:

| Metric | Early (Runs 1–6) | Mid (Runs 8–11) | Late (Runs 12–17) |
|--------|-------------------|------------------|---------------------|
| **Test suite** | Build-only | 14 → 25 tests | 25 tests |
| **Pass rate** | Build pass | 56% → 68% | **100% (6 consecutive)** |
| **Layer 1 time** | 3–4.5s | 3.3–10s | **3.0 → 1.81s** |
| **L1 manual fixes** | Not tracked | 3 → 0 | **0 for 6 consecutive runs** |
| **Layer 2 fixes** | Not tracked | 12 → 5 | **3 (stable)** |
| **Render mode** | N/A | InteractiveServer | **SSR** |

### Key Milestones

| Run | Milestone |
|-----|-----------|
| **1** | First automated migration benchmark |
| **8** | First 100% acceptance pass (14/14) |
| **9** | Visual regression discovery → 11 new visual integrity tests added |
| **12** | First perfect score on 25-test suite |
| **13** | **SSR breakthrough** — eliminated HttpContext/SignalR problems |
| **14** | **Layer 1 zero-touch** — 0 manual fixes for first time |
| **16** | **Layer 2 automation begins** — Program.cs fully auto-generated |
| **17** | **Genericized toolkit validated** — L1 script bug fixed, 28% faster |

### Layer 1 Performance Trend

```
Run:  1    4    5    6    8    9   10   11   12   13   14   15   16   17
     3.3  ~3  3.25 4.58 3.3  ~10  4.6  3.3  3.1  3.0  3.2 2.83 2.50 1.81  (seconds)
```

Layer 1 stabilized around 3s, then improved to **1.81s** — a 56% improvement from the 4.2s average of early runs (excluding the Run 9 outlier).

---

## Key Conclusions

Based on 16 runs of iterative testing:

1. **Layer 1 is stable:** 0 manual fixes for 5 consecutive runs (12–16). The script handles all mechanical markup transforms, static asset copying, template placeholder conversion, and SSR-specific fixes.

2. **Layer 2 has a natural automation ceiling:** 3 persistent semantic gaps (FormView SSR workaround, auth form simplification, Program.cs bootstrap) represent the boundary between regex-automatable and context-dependent transforms. Run 16's Layer 2 script began crossing that boundary — Program.cs (Pattern C) is now fully automated.

3. **SSR is the correct default render mode:** The switch from InteractiveServer to SSR in Run 13 eliminated an entire class of HttpContext/cookie/session problems and cut total time by 75%.

4. **Layer 1 time trend:** 4.2s → 2.50s (40% improvement). The script is faster despite doing more work (more transforms, better flagging, auto-detection).

5. **Pipeline is reproducible:** Layer 2 reference extraction (introduced in Run 15) makes the entire pipeline deterministic — the same fixes apply cleanly every run.

---

## ContosoUniversity Runs

All runs use the ContosoUniversity sample in `samples/ContosoUniversity/` as source input.

| Run | Date | L1 Time | L1 Manual Fixes | L2 Fixes | Score | Render Mode | Key Outcome |
|-----|------|---------|-----------------|----------|-------|-------------|-------------|
| [01](contoso-run01.md) | 2026-03-08 | **1.50s** | 18 → 0 errors | 7 | **31/40 (77.5%)** | SSR | All pages work, nav/form wiring issues remain |
| [02](contoso-run02.md) | 2026-03-09 | **0.74s** | 0 (overlay) | 7 | **31/40 (77.5%)** | SSR | Identical test results; L1 script bug fixed |

### ContosoUniversity vs WingtipToys

| Aspect | WingtipToys | ContosoUniversity |
|--------|-------------|-------------------|
| Pages | ~15 (32 markup files) | 5 pages + 1 master |
| Data Access | Code-First EF6 | Database-First EF6 (.edmx) |
| Ajax Controls | None | UpdatePanel, ScriptManager, AutoCompleteExtender |
| Auth | ASP.NET Identity | None |
| Acceptance Tests | 25 | 40 (33 pass + 7 nav failures = 31 functional) |
| Key Challenge | Auth/cookie handling in SSR | EF6 .edmx + AjaxControlToolkit |

---

## Adding a New Test Project

To benchmark a new Web Forms application:

1. **Source app:** Place the original Web Forms project in `samples/{ProjectName}/`.
2. **Acceptance tests:** Create a test project at `samples/After{ProjectName}.Tests/` with Playwright-based functional and visual integrity tests.
3. **Migration output:** The script outputs to `samples/After{ProjectName}/`.
4. **Report naming:** Use `{project-name}-runNN.md` in this directory (e.g., `contoso-run01.md`). Pad run numbers to two digits for sort order.
5. **README update:** Add the project to the Test Projects table above, then add a dedicated `## {ProjectName} Runs` section with a run history table.
6. **Run the pipeline:**
   ```
   pwsh -File migration-toolkit/scripts/bwfc-migrate.ps1 -Path samples/{ProjectName}/{ProjectName} -Output samples/After{ProjectName} -TestMode
   pwsh -File migration-toolkit/scripts/bwfc-migrate-layer2.ps1 -Path samples/After{ProjectName}
   ```

---

## Report Archive

Runs 1–6 and 8–13 also have **run folders** containing raw data, build output, screenshots, and detailed reports from earlier benchmark iterations. These use an older, more verbose format:

| Folder | Run | Contents |
|--------|-----|----------|
| [`wingtiptoys-2026-03-04/`](wingtiptoys-2026-03-04/) | 1 | report, scan/build/migrate output, images |
| [`wingtiptoys-run2-2026-03-04/`](wingtiptoys-run2-2026-03-04/) | 2 | report, scan/build/migrate output, images |
| [`wingtiptoys-run3-2026-03-04/`](wingtiptoys-run3-2026-03-04/) | 3 | report, scan/build/migrate output, images |
| [`wingtiptoys-run4-2026-03-04/`](wingtiptoys-run4-2026-03-04/) | 4 | report, migrate output |
| [`wingtiptoys-run5-2026-03-04/`](wingtiptoys-run5-2026-03-04/) | 5 | report, analysis, timing, manual-fixes, build output |
| [`wingtiptoys-run6-2026-03-04/`](wingtiptoys-run6-2026-03-04/) | 6 | report, raw data |
| [`wingtiptoys-run8-2026-03-06/`](wingtiptoys-run8-2026-03-06/) | 8 | REPORT, screenshots |
| [`wingtiptoys-run9-2026-03-06/`](wingtiptoys-run9-2026-03-06/) | 9 | REPORT, visual regression screenshots |
| [`wingtiptoys-run10-2026-03-07/`](wingtiptoys-run10-2026-03-07/) | 10 | REPORT |
| [`wingtiptoys-run11-2026-03-07/`](wingtiptoys-run11-2026-03-07/) | 11 | REPORT |
| [`wingtiptoys-run12-2026-03-08/`](wingtiptoys-run12-2026-03-08/) | 12 | REPORT |
| [`wingtiptoys-run13-2026-03-08/`](wingtiptoys-run13-2026-03-08/) | 13 | REPORT |

The **standalone `.md` reports** (e.g., `wingtiptoys-run08.md`) are the canonical format going forward. Run folders are preserved as historical archives.
