# Migration Test Reports

Benchmarked migrations of real Web Forms applications using the BWFC migration toolkit.

## Directory Structure

```
dev-docs/migration-tests/
├── README.md                    ← this file
├── component-coverage.md        ← cross-project analysis
├── css-architecture-analysis.md ← cross-project analysis
├── wingtiptoys/
│   ├── run01/ through run17/    ← 16 runs (run07 skipped)
│   └── (each contains REPORT.md + optional summary.md, images, screenshots)
└── contosouniversity/
    ├── run01/ through run18/    ← 18 runs (renumbered; see note below)
    └── (each contains REPORT.md + optional screenshots)
```

> **Contoso renumbering note:** Runs 01–12 retain their original numbers. Runs originally labeled 11–16 from March 10–11 were renumbered to 13–18 to resolve numbering collisions with the March 9 batch (runs 07–12). See [Renumbering Details](#contoso-renumbering-details) below.

## Test Projects

| Project | Source | Framework | Pages | Controls Used | Status |
|---------|--------|-----------|-------|---------------|--------|
| WingtipToys | ASP.NET Web Forms 2013 | .NET 4.5 | ~15 pages (32 markup files) | GridView, ListView, FormView, LoginView, LoginStatus, etc. | ✅ 17 runs, 100% pass (6 consecutive) |
| ContosoUniversity | Microsoft Tutorial App | .NET 4.x | 5 pages + 1 master | GridView, DetailsView, Table, UpdatePanel, ScriptManager, AutoCompleteExtender | ✅ 18 runs, peak 100% (8 perfect runs) |

---

## WingtipToys Runs

All runs use the WingtipToys sample in `samples/WingtipToys/` as source input.

| Run | Date | L1 Time | L1 Manual Fixes | L2 Fixes | Score | Render Mode | Key Outcome |
|-----|------|---------|-----------------|----------|-------|-------------|-------------|
| [1](wingtiptoys/run01/REPORT.md) | 2026-03-04 | 3.3s | — | — | ✅ Build pass | — | Initial benchmark; 230 control usages across 31 types |
| [2](wingtiptoys/run02/REPORT.md) | 2026-03-04 | — | — | — | ✅ 11/11 | — | Feature validation |
| [3](wingtiptoys/run03/REPORT.md) | 2026-03-04 | — | — | — | ✅ 11/11 | — | From-scratch validation |
| [4](wingtiptoys/run04/REPORT.md) | 2026-03-04 | ~3s | — | — | ✅ 11/11 | — | Enhanced script; 289 transforms |
| [5](wingtiptoys/run05/REPORT.md) | 2026-03-04 | 3.25s | — | — | ✅ Build pass | — | 6 new script enhancements; ~10 min total |
| [6](wingtiptoys/run06/REPORT.md) | 2026-03-04 | 4.58s | — | — | ✅ Build pass | — | 55% time reduction from Run 5; wwwroot pathing fixed |
| [8](wingtiptoys/run08/REPORT.md) | 2026-03-06 | 3.3s | 3 | 12 | ✅ 14/14 (100%) | InteractiveServer | First 100% acceptance pass |
| [9](wingtiptoys/run09/REPORT.md) | 2026-03-06 | ~10s | 2 | 10 | ❌ FAIL | InteractiveServer | Visual regression — no CSS/images |
| [10](wingtiptoys/run10/REPORT.md) | 2026-03-07 | 4.6s | 1 | 8 | ❌ 20/25 | InteractiveServer | Coordinator process violation |
| [11](wingtiptoys/run11/REPORT.md) | 2026-03-07 | 3.3s | 0 | 5 | ❌ 17/25 (68%) | InteractiveServer | ListView placeholder + Scripts/ gaps |
| [12](wingtiptoys/run12/REPORT.md) | 2026-03-08 | 3.1s | 0 | 3 | ✅ 25/25 (100%) | InteractiveServer | First perfect score (25-test suite) |
| [13](wingtiptoys/run13/REPORT.md) | 2026-03-08 | 3.0s | 0 | 3 | ✅ 25/25 (100%) | **SSR** | SSR default; 22 min total; 50% fewer manual fixes |
| [14](wingtiptoys/run14/REPORT.md) | 2026-03-08 | 3.2s | **0** | 3 | ✅ 25/25 (100%) | SSR | Layer 1 fully zero-touch; automation ceiling identified |
| [15](wingtiptoys/run15/REPORT.md) | 2026-03-08 | 2.83s | 0 | 3 | ✅ 25/25 (100%) | SSR | 4th consecutive 100%; L2 reference extraction |
| [16](wingtiptoys/run16/REPORT.md) | 2026-03-08 | **2.50s** | 0 | 3* | ✅ 25/25 (100%) | SSR | First Layer 2 automation; Program.cs auto-generated |
| [17](wingtiptoys/run17/REPORT.md) | 2026-03-09 | **1.81s** | 0 | 3* | ✅ 25/25 (100%) | SSR | Genericized toolkit validation; 28% faster L1 |

> **Run 7** was skipped (no benchmark recorded).
>
> **Runs 16–17 note:** `*` Layer 2 script automates Program.cs (Pattern C). Patterns A (code-behinds) and B (auth forms) still need manual overlay.
>
> **Runs 8–15:** Each run folder contains both a detailed `REPORT.md` and a shorter `summary.md` (the original standalone report).

---

## Pipeline Evolution (WingtipToys)

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

Based on 17 WingtipToys runs and 18 ContosoUniversity runs of iterative testing:

1. **Layer 1 is stable:** 0 manual fixes for 5 consecutive WingtipToys runs (12–16). The script handles all mechanical markup transforms, static asset copying, template placeholder conversion, and SSR-specific fixes.

2. **Layer 2 has a natural automation ceiling:** 3 persistent semantic gaps (FormView SSR workaround, auth form simplification, Program.cs bootstrap) represent the boundary between regex-automatable and context-dependent transforms. Run 16's Layer 2 script began crossing that boundary — Program.cs (Pattern C) is now fully automated.

3. **SSR is the correct default render mode:** The switch from InteractiveServer to SSR in Run 13 eliminated an entire class of HttpContext/cookie/session problems and cut total time by 75%.

4. **Layer 1 time trend:** 4.2s → 2.50s (40% improvement, WingtipToys). The script is faster despite doing more work (more transforms, better flagging, auto-detection).

5. **Pipeline is reproducible:** Layer 2 reference extraction (introduced in Run 15) makes the entire pipeline deterministic — the same fixes apply cleanly every run.

6. **Cross-project validation:** ContosoUniversity (18 runs) confirmed the pipeline works across app architectures — database-first EF6, .edmx models, UpdatePanel, and AjaxControlToolkit patterns all handled. Eight ContosoUniversity runs achieved 100% (40/40).

---

## ContosoUniversity Runs

All runs use the ContosoUniversity sample in `samples/ContosoUniversity/` as source input.

| Run | Date | L1 Time | Score | Render Mode | Key Outcome |
|-----|------|---------|-------|-------------|-------------|
| [01](contosouniversity/run01/REPORT.md) | 2026-03-08 | 1.50s | 31/40 (77.5%) | SSR | All pages work; nav/form wiring issues remain |
| [02](contosouniversity/run02/REPORT.md) | 2026-03-09 | 0.74s | 31/40 (77.5%) | SSR | Identical results; L1 script bug fixed |
| [03](contosouniversity/run03/REPORT.md) | 2026-03-08 | — | 0/40* | N/A | Scaffolding phase; build passes, tests need page migrations |
| [04](contosouniversity/run04/REPORT.md) | 2026-03-09 | ~1s | 16/40 (40%) | SSR | First overlay-free build |
| [05](contosouniversity/run05/REPORT.md) | 2026-03-09 | ~1s | **40/40 (100%)** ✅ | SSR + InteractiveServer | Perfect score; SQL Server LocalDB working |
| [06](contosouniversity/run06/REPORT.md) | 2026-03-09 | 1.43s | 37/40 (92.5%) | SSR | Timed migration; L1+L2 in 1.95s total |
| [07](contosouniversity/run07/REPORT.md) | 2026-03-09 | ~1s | **40/40 (100%)** ✅ | InteractiveServer | DetailsView BoundField fix in BWFC library |
| [08](contosouniversity/run08/REPORT.md) | 2026-03-09 | ~1.1s | **40/40 (100%)** ✅ | InteractiveServer | DetailsView Items binding; URL rewriting |
| [09](contosouniversity/run09/REPORT.md) | 2026-03-09 | ~2.3s | **40/40 (100%)** ✅ | InteractiveServer | 5 migration boundary rules enforced |
| [10](contosouniversity/run10/REPORT.md) | 2026-03-09 | 1.17s | ❌ Build failed | N/A | Script gaps for inline styles and EF6 patterns |
| [11](contosouniversity/run11/REPORT.md) | 2026-03-09 | ~0.80s | Build OK | SSR | Color attributes converted; 4 benign warnings |
| [12](contosouniversity/run12/REPORT.md) | 2026-03-09 | ~1.1s | 32/40 (80%) | SSR + InteractiveServer | StreamRendering timing issue on About page |
| [13](contosouniversity/run13/REPORT.md) | 2026-03-10 | 1.01s | 39/40 (97.5%) | InteractiveServer | Live demo; 1 test design issue (not migration) |
| [14](contosouniversity/run14/REPORT.md) | 2026-03-10 | 1.1s | 38/40 (95%) | InteractiveServer | False negative test failures; Add/Delete work |
| [15](contosouniversity/run15/REPORT.md) | 2026-03-10 | 0.72s | **40/40 (100%)** ✅ | InteractiveServer | PageStyleSheet CSS loading working |
| [16](contosouniversity/run16/REPORT.md) | 2026-03-10 | 0.94s | **40/40 (100%)** ✅ | InteractiveServer | Auto EF scaffolding attempted; ~7 min total |
| [17](contosouniversity/run17/REPORT.md) | 2026-03-10 | 0.59s | **40/40 (100%)** ✅ | InteractiveServer | LocalDB wait/retry feature; git restore for code-behind |
| [18](contosouniversity/run18/REPORT.md) | 2026-03-11 | ~1.1s | ❌ Failed | N/A | Context degradation; session restart needed |

> **Run 03 note:** `*` Tests require running app + completed page migrations; this run focused on EF Core scaffolding only.

### Contoso Renumbering Details

During folder reorganization, two numbering collisions were discovered:

| New # | Original Name | Date | Why Renumbered |
|-------|---------------|------|----------------|
| 13 | `contoso-run11-2026-03-10/` | 2026-03-10 | Collided with `contosouniversity-run11` (2026-03-09); different run, live demo |
| 14 | `contoso-run12-REPORT.md` + `contoso-run12-screenshots/` | 2026-03-10 | Collided with `contosouniversity-run12` (2026-03-09); different run |
| 15 | `contoso-run13-2026-03-10/` | 2026-03-10 | Shifted +2 due to run11/12 insertions |
| 16 | `ContosoUniversity-Run14-REPORT.md` | 2026-03-10 | Shifted +2 |
| 17 | `ContosoUniversity-Run15-REPORT.md` | 2026-03-10 | Shifted +2 |
| 18 | `contoso-run16/` | 2026-03-11 | Shifted +2 (original report was untracked; placeholder only) |

### ContosoUniversity vs WingtipToys

| Aspect | WingtipToys | ContosoUniversity |
|--------|-------------|-------------------|
| Pages | ~15 (32 markup files) | 5 pages + 1 master |
| Data Access | Code-First EF6 | Database-First EF6 (.edmx) |
| Ajax Controls | None | UpdatePanel, ScriptManager, AutoCompleteExtender |
| Auth | ASP.NET Identity | None |
| Acceptance Tests | 25 | 40 |
| Key Challenge | Auth/cookie handling in SSR | EF6 .edmx + AjaxControlToolkit |

---

## Adding a New Test Project

To benchmark a new Web Forms application:

1. **Source app:** Place the original Web Forms project in `samples/{ProjectName}/`.
2. **Acceptance tests:** Create a test project at `samples/After{ProjectName}.Tests/` with Playwright-based functional and visual integrity tests.
3. **Migration output:** The script outputs to `samples/After{ProjectName}/`.
4. **Report structure:** Create a `{projectname}/runNN/` directory under this folder with a `REPORT.md` inside. Zero-pad run numbers to two digits.
5. **README update:** Add the project to the Test Projects table above, then add a dedicated `## {ProjectName} Runs` section with a run history table.
6. **Run the pipeline:**
   ```
   pwsh -File migration-toolkit/scripts/bwfc-migrate.ps1 -Path samples/{ProjectName}/{ProjectName} -Output samples/After{ProjectName} -TestMode
   pwsh -File migration-toolkit/scripts/bwfc-migrate-layer2.ps1 -Path samples/After{ProjectName}
   ```

---

## Report Archive

All run data is organized in project subdirectories. Each `runNN/` folder contains a `REPORT.md` and may include additional artifacts:

### WingtipToys

| Folder | Contents |
|--------|----------|
| [`wingtiptoys/run01/`](wingtiptoys/run01/) | REPORT, scan/build/migrate output, images |
| [`wingtiptoys/run02/`](wingtiptoys/run02/) | REPORT, scan/build/migrate output, images |
| [`wingtiptoys/run03/`](wingtiptoys/run03/) | REPORT, scan/build/migrate output, images |
| [`wingtiptoys/run04/`](wingtiptoys/run04/) | REPORT, migrate output, images |
| [`wingtiptoys/run05/`](wingtiptoys/run05/) | REPORT, analysis, timing, manual-fixes, build output |
| [`wingtiptoys/run06/`](wingtiptoys/run06/) | REPORT, raw data |
| [`wingtiptoys/run08/`](wingtiptoys/run08/) | REPORT, summary, screenshots |
| [`wingtiptoys/run09/`](wingtiptoys/run09/) | REPORT, summary, visual regression screenshots |
| [`wingtiptoys/run10/`](wingtiptoys/run10/) | REPORT, summary |
| [`wingtiptoys/run11/`](wingtiptoys/run11/) | REPORT, summary |
| [`wingtiptoys/run12/`](wingtiptoys/run12/) | REPORT, summary |
| [`wingtiptoys/run13/`](wingtiptoys/run13/) | REPORT, summary |
| [`wingtiptoys/run14/`](wingtiptoys/run14/) | REPORT, summary |
| [`wingtiptoys/run15/`](wingtiptoys/run15/) | REPORT, summary, screenshots |
| [`wingtiptoys/run16/`](wingtiptoys/run16/) | REPORT |
| [`wingtiptoys/run17/`](wingtiptoys/run17/) | REPORT |

### ContosoUniversity

| Folder | Contents |
|--------|----------|
| [`contosouniversity/run01/`](contosouniversity/run01/) | REPORT, findings |
| [`contosouniversity/run02/`](contosouniversity/run02/) | REPORT |
| [`contosouniversity/run03/`](contosouniversity/run03/) | REPORT |
| [`contosouniversity/run04/`](contosouniversity/run04/) | REPORT |
| [`contosouniversity/run05/`](contosouniversity/run05/) | REPORT, screenshots |
| [`contosouniversity/run06/`](contosouniversity/run06/) | REPORT |
| [`contosouniversity/run07/`](contosouniversity/run07/) | REPORT |
| [`contosouniversity/run08/`](contosouniversity/run08/) | REPORT |
| [`contosouniversity/run09/`](contosouniversity/run09/) | REPORT |
| [`contosouniversity/run10/`](contosouniversity/run10/) | REPORT |
| [`contosouniversity/run11/`](contosouniversity/run11/) | REPORT |
| [`contosouniversity/run12/`](contosouniversity/run12/) | REPORT, screenshots |
| [`contosouniversity/run13/`](contosouniversity/run13/) | REPORT, screenshots (was contoso-run11, Mar 10 live demo) |
| [`contosouniversity/run14/`](contosouniversity/run14/) | REPORT, screenshots (was contoso-run12, Mar 10) |
| [`contosouniversity/run15/`](contosouniversity/run15/) | REPORT, screenshots (was contoso-run13, Mar 10) |
| [`contosouniversity/run16/`](contosouniversity/run16/) | REPORT (was ContosoUniversity-Run14) |
| [`contosouniversity/run17/`](contosouniversity/run17/) | REPORT (was ContosoUniversity-Run15) |
| [`contosouniversity/run18/`](contosouniversity/run18/) | REPORT placeholder (was contoso-run16; original not committed) |

For WingtipToys runs 8–15, each folder contains both a detailed `REPORT.md` (from the run folder) and a shorter `summary.md` (from the original standalone report).
