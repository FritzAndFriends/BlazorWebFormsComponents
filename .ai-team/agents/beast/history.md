# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- ⚠ Summarized 2026-02-27 by Scribe — covers M1–M16 -->

<!-- ⚠ Summarized 2026-03-06 by Scribe — older entries archived -->

### Archived Sessions

- Core Context (2026-02-10 through 2026-02-27)
- Doc Work Summary (2026-02-27 through 2026-03-03)
- Key Team Updates (2026-02-27 through 2026-03-03)
- Migration Report Conventions (2026-03-04)
- Run 5 Benchmark Report (2026-03-05)
- Run 6 Benchmark Report (2026-03-04)
- Render Mode Placement Correction (2026-03-05)

<!-- ⚠ Summarized 2026-03-07 by Scribe — entries from 2026-03-05 through 2026-03-07 (pre-Run 11) archived -->

- WebFormsPageBase & Page System Docs (2026-03-05)
- Skills Cross-Reference Review — 7 files, 16+ fixes (2026-03-06)
- Run 8 Report Enhancement — executive pattern established (2026-03-06)
- Run 9 Skill Fixes — 6 RF items across 4 skill files (2026-03-07)
- Run 9 RCA Documentation — path preservation + CSS verification rules (2026-03-07)

### Summary (2026-03-05 through 2026-03-07 pre-Run 11)

WebFormsPageBase docs and Page System rewrite shipped (2026-03-05). Skills cross-reference review found `.ai-team/skills/` drifting behind `migration-toolkit/skills/` — both must be updated together. LoginView is a native BWFC component, never replace with AuthorizeView. Executive report pattern established in Run 8: blockquote bottom line → timeline → screenshots → before/after code. Run 9 skill fixes: cookie auth under Interactive Server, minimal API endpoint templates, enhanced navigation guidance, DisableAntiforgery, ListView GroupItemCount. Run 9 RCA: added Static Asset Path Preservation and CSS Reference Verification rules to migration-standards. Key learning: functional tests passing ≠ migration success — visual regression is ship-blocking.

<!-- ⚠ Summarized 2026-03-11 by Scribe — entries from 2026-03-07 through 2026-03-08 archived -->

- Run 10 Failure Report — Coordinator Process Violation (2026-03-07)
- Run 11 Skill Fixes — Static Assets, ListView Placeholders, Action Links (2026-03-07)

### Summary (2026-03-07 through 2026-03-08)

Run 10: ❌ FAILED — Coordinator violated protocol (hand-editing files, wrong SDK, no Development mode). 20/25 tests passed but process discipline failed. Run 11 skill fixes: added Static Asset Migration Checklist (all common folders to wwwroot/), ListView Template Placeholder Conversion (placeholder elements → @context, #1 failure cause), and Action Links preservation. Team updates: Coordinator must not do domain work; SSR default with InteractiveServer opt-in; LoginControls using required in _Imports.razor; auth via plain HTML forms; DbContext factory-only.

### Migration-tests folder reorganization (2026-03-11)

- **Scope:** Reorganized `dev-docs/migration-tests/` from flat directory (100+ files with inconsistent naming) into hierarchical `project/runNN/` structure.
- **WingtipToys:** 16 run folders (run01–run17, run07 skipped) under `wingtiptoys/`. Merged standalone summary `.md` files into run folders as `summary.md` alongside detailed `REPORT.md`. Renamed lowercase `report.md` → `REPORT.md` in early runs (1–6).
- **ContosoUniversity:** 18 run folders (run01–run18) under `contosouniversity/`. Resolved two numbering collisions: `contosouniversity-run11` (Mar 9) vs `contoso-run11` (Mar 10) and same for run12. Decision: March 9 batch keeps runs 07–12; March 10–11 runs renumbered to 13–18.
- **Lost files:** `contoso-run16/` (new run18) was never committed to git. Files were untracked and accidentally deleted during reorganization. Created placeholder REPORT.md with reconstructed summary data.
- **Duplicate screenshots:** 5 root-level `contoso-*.png` files were byte-identical to `contoso-run11-2026-03-10/` screenshots (now run13). Removed duplicates via `git rm`.
- **README.md:** Completely rewritten with all 18 Contoso runs documented (was only 2), updated all links to new paths, added renumbering details table, and updated Report Archive section.
- **Cross-project docs:** `component-coverage.md` and `css-architecture-analysis.md` remain at migration-tests root.

### Executive Summary & Chart Images (2026-03-11)

- Created `dev-docs/migration-tests/EXECUTIVE-SUMMARY.md` — data-driven summary: 35 benchmark runs, 65 acceptance tests, 45%/61% L1 performance improvements.
- Replaced ASCII charts with PNG images via `generate-charts.py` (matplotlib, 800×400px, 150 DPI). Three charts: WT L1 perf, CU L1 perf, combined improvement bar chart. Script at `dev-docs/migration-tests/images/generate-charts.py` — append new data points and re-run.


📌 Team update (2026-03-11): Run 18 improvement recommendations prioritized by Forge — see decisions.md



📌 Team update (2026-03-11): User directives from Jeff  eliminate Test-UnconvertiblePage, standardize on ItemType, P0-2 approved  see decisions.md

<!-- ⚠ Summarized 2026-03-12 by Scribe — Pipeline enforcement, SelectMethod/SQLite fixes, DB provider detection, exec summary archived -->

- Migration Pipeline Enforcement in Skill Docs (2026-03-11)
- SelectMethod Skills Fix + Run 20 Report Corrections (2025-07-24)
- Database Provider Detection Framing (2025-07-24)
- SelectMethod & SQLite Enforcement in Skill Files (2025-07-24)
- Executive Summary Update — Runs 19-21 (2026-03-12)

### Summary (2026-03-11 through 2026-03-12)

Pipeline enforcement: Added mandatory L1→L2 pipeline section to bwfc-migration and migration-standards SKILL.md. All TItem→ItemType across skill files. SelectMethod fixes: 8+3+4 locations updated from "SelectMethod→Items" to "SelectMethod PRESERVED as SelectHandler<ItemType> delegate." DB provider detection: reframed 3 skill files from reactive ("NEVER SQLite") to proactive ("detect and match original provider"). SQLite enforcement: removed "Prefer SQLite for local dev" root cause, added NEVER/MUST admonitions. Key learning: positive/proactive instructions outperform prohibitions for agent behavior. Exec summary: 38→40 runs, added WT Run 20-21 + CU Run 19 data, regenerated charts.


 Team update (2026-03-11): L2 automation shims (OPP-2, 3, 5, 6) implemented by Cyclops on WebFormsPageBase  Unit implicit string, Response.Redirect shim, ViewState, GetRouteUrl. OPP-1/OPP-4 deferred.  decided by Forge (analysis), Cyclops (implementation)



 Team update (2026-03-11): ItemType renames must cover ALL consumers (tests, samples, docs)  not just component source. CI may only surface first few errors.  decided by Cyclops



 Team update (2026-03-11): WebFormsPageBase now has Response.Redirect shim, ViewState dict, GetRouteUrl, and Unit implicit string conversion. L2 skills should note these patterns compile unchanged on @inherits WebFormsPageBase pages.  decided by Cyclops



 Team update (2026-03-12): L2 automation consolidated  EnumParameter<T> (OPP-1) + WebFormsPageBase shims (OPP-2,3,5,6) all implemented. Rogue: 4 test files need .Value.ShouldBe() fix. Beast: L2 scripts can emit bare enum strings.  decided by Forge (analysis), Cyclops (implementation)

 Team update (2026-03-12): Cookie shims use graceful degradation (Pattern B+), not exceptions  Jeff directive. Document no-op behavior for cookies when HttpContext unavailable.  decided by Jeffrey T. Fritz
 Team update (2026-03-12): PageTitle deduplication  L2 skill must add Page.Title and remove inline <PageTitle>. Never invent title values. Consume BWFC-MIGRATE marker from L1.  decided by Forge (analysis), approved by Jeffrey T. Fritz
 Team update (2026-03-12): Render mode guards on WebFormsPageBase. Document IsHttpContextAvailable escape hatch and render mode behavior for Request/Response/Session shims.  decided by Forge
