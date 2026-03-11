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

### Run 10 Failure Report (2026-03-07)

- **Report location:** `dev-docs/migration-tests/wingtiptoys-run10-2026-03-07/REPORT.md`
- **Status:** ❌ FAILED — Coordinator Process Violation
- **Branch:** `squad/run8-improvements` (same as Run 9)
- **What happened:** Phases 1 and 2 completed successfully (Layer 1 in 4.6s, Layer 2 in ~15 min with 0 build errors). The Coordinator then violated Squad protocol by hand-editing Razor files, installing `npm playwright`, creating throwaway Node.js scripts, using wrong .NET SDK (10.0.200-preview vs 10.0.100), and not setting `ASPNETCORE_ENVIRONMENT=Development`. ~30 minutes wasted on ad-hoc debugging.
- **Test results:** 20/25 passed (14 functional + 11 visual integrity). 5 failed: 3 cart timeouts, 1 auth E2E, 1 blazor.web.js 500.
- **Root causes:** RC-1 Coordinator did domain work instead of routing to agents; RC-2 Layer 2 output had null collections and missing ItemType params; RC-3 Environment not configured for Development mode.
- **Key learning:** The Squad system works when agents do domain work and the Coordinator coordinates. When the Coordinator performs domain work directly, quality controls are bypassed and time is wasted. Process discipline is the #1 priority for Run 11.
- **Pattern across runs:** Run 8 ✅, Run 9 ❌ (visual regression), Run 10 ❌ (process violation). Automated pipeline (Layers 1-2) is improving each run; Phase 3 execution remains the weak link.
- **Decision filed:** `.ai-team/decisions/inbox/beast-run10-failure.md` — Coordinator must never hand-edit application source files.

 Team update (2026-03-07): Run 10 declared FAILED  coordinator violated protocol. Phase 1 skill updates applied correctly. Key issues for next run: missing ItemType param, null Products list, ASPNETCORE_ENVIRONMENT=Development required.  decided by Jeffrey T. Fritz

### Run 11 Skill Fixes — migration-standards SKILL.md (2026-03-07)

- **Scope:** 3 new sections added to `.ai-team/skills/migration-standards/SKILL.md` based on Run 11 WingtipToys benchmark failures.
- **Fix 3 — Static Asset Migration Checklist:** Added comprehensive table of ALL common folders (Content/, Scripts/, Images/, Catalog/, fonts/, favicon.ico) that must be copied to `wwwroot/`. Includes verification checklist for CSS, JS, images, logos, fonts. Documents the common miss: `Scripts/` folder is easy to forget because CSS breakage is more visually obvious.
- **Fix 4 — ListView Template Placeholder Conversion:** Added full conversion guide for LayoutTemplate/GroupTemplate placeholder elements → `@context`. This was the #1 failure cause in Run 11 (5 of 8 test failures). Documents the Web Forms placeholder pattern (`<tr id="groupPlaceholder">`), the Blazor `RenderFragment<RenderFragment>` equivalent, migration rule table, with/without GroupItemCount examples, and diagnostic tip.
- **Fix 5 — Preserving Action Links in Detail Pages:** Added section on verifying action links (Add to Cart, Edit, Delete) survive Layer 1 conversion. Documents the `@context.PropertyName` pattern for data-bound link values.
- **Why needed:** Run 11 exposed gaps where the migration script converts template structure but doesn't handle placeholder-to-@context conversion, and static asset copying was incomplete (Scripts/ folder missed).


 Team update (2026-03-07): Coordinator must not perform domain work  all code changes must route through specialist agents  decided by Jeffrey T. Fritz, Beast
 Team update (2026-03-07): Run 11 script fixes: Invoke-ScriptAutoDetection (JS files) and Convert-TemplatePlaceholders (placeholder@context) added to bwfc-migrate.ps1  decided by Cyclops
 Team update (2026-03-07): Run 11 migration decisions: root-level _Imports.razor required, partial class base class conflict pattern, auth endpoint pattern  decided by Cyclops

 Team update (2026-03-08): Default to SSR (Static Server Rendering) with per-component InteractiveServer opt-in; eliminates HttpContext/cookie/session problems  decided by Forge

 Team update (2026-03-08): @using BlazorWebFormsComponents.LoginControls must be in every generated _Imports.razor  decided by Cyclops

 Team update (2026-03-08): Run 12 migration patterns: auth via plain HTML forms with data-enhance=false, dual DbContext, LoginView _userName from cascading auth state  decided by Cyclops

### Migration-tests folder reorganization (2026-03-11)

- **Scope:** Reorganized `dev-docs/migration-tests/` from flat directory (100+ files with inconsistent naming) into hierarchical `project/runNN/` structure.
- **WingtipToys:** 16 run folders (run01–run17, run07 skipped) under `wingtiptoys/`. Merged standalone summary `.md` files into run folders as `summary.md` alongside detailed `REPORT.md`. Renamed lowercase `report.md` → `REPORT.md` in early runs (1–6).
- **ContosoUniversity:** 18 run folders (run01–run18) under `contosouniversity/`. Resolved two numbering collisions: `contosouniversity-run11` (Mar 9) vs `contoso-run11` (Mar 10) and same for run12. Decision: March 9 batch keeps runs 07–12; March 10–11 runs renumbered to 13–18.
- **Lost files:** `contoso-run16/` (new run18) was never committed to git. Files were untracked and accidentally deleted during reorganization. Created placeholder REPORT.md with reconstructed summary data.
- **Duplicate screenshots:** 5 root-level `contoso-*.png` files were byte-identical to `contoso-run11-2026-03-10/` screenshots (now run13). Removed duplicates via `git rm`.
- **README.md:** Completely rewritten with all 18 Contoso runs documented (was only 2), updated all links to new paths, added renumbering details table, and updated Report Archive section.
- **Cross-project docs:** `component-coverage.md` and `css-architecture-analysis.md` remain at migration-tests root.

### Executive Summary Document (2026-03-11)

- **Scope:** Created `dev-docs/migration-tests/EXECUTIVE-SUMMARY.md` — a polished, data-driven executive summary showcasing migration toolkit progress and capabilities.
- **Key data points used:** 35 benchmark runs (17 WT + 18 CU), 65 acceptance tests (25 + 40), 6 consecutive 100% WT runs, 8 perfect CU runs, L1 times 1.81s (WT) and 0.59s (CU), 45% and 61% performance improvements.
- **Screenshot paths verified and referenced:** `wingtiptoys/run01/images/comparison-*.png` (3 side-by-side comparisons), `wingtiptoys/run15/*.png` (5 latest screenshots), `contosouniversity/run15/*.png` (5 latest screenshots).
- **Sections:** Executive Overview, Drop-In Replacement Strategy, Results at a Glance, Performance Progression (with ASCII charts), Visual Fidelity screenshots, Key Milestones timeline, Two-Layer Pipeline Architecture, Test Project Coverage, What's Next.
- **Document location:** `dev-docs/migration-tests/EXECUTIVE-SUMMARY.md`

📌 Team update (2026-03-11): `AddBlazorWebFormsComponents()` now auto-registers HttpContextAccessor, adds options pattern + `UseBlazorWebFormsComponents()` middleware with .aspx URL rewriting. Consumers no longer need manual `AddHttpContextAccessor()`. All sample Program.cs files updated. — decided by Cyclops

### Chart Image Generation for Executive Summary (2026-03-11)

- **Scope:** Replaced ASCII art performance charts in `EXECUTIVE-SUMMARY.md` with real PNG chart images generated via Python + matplotlib.
- **Chart generation approach:** Created `generate-charts.py` script using matplotlib with a professional white-background style (no default gray), dashed grid lines, trend lines via linear regression, best-time star markers, and area fills. All three charts are 800×400px at 150 DPI.
- **Data sources:** Layer 1 execution times extracted from REPORT.md files across all run folders:
  - WingtipToys: 12 data points from runs 1, 4, 5, 6, 8, 11–17 (range: 1.81s–4.58s)
  - ContosoUniversity: 10 data points from runs 1, 2, 4, 6, 8, 9, 12, 15–17 (range: 0.59s–2.30s)
- **Files generated:**
  - `dev-docs/migration-tests/images/wingtiptoys-layer1-perf.png` — line chart with trend
  - `dev-docs/migration-tests/images/contosouniversity-layer1-perf.png` — line chart with trend
  - `dev-docs/migration-tests/images/combined-improvement.png` — grouped bar chart (45% WT, 61% CU improvement)
  - `dev-docs/migration-tests/images/generate-charts.py` — regeneration script
- **EXECUTIVE-SUMMARY.md updated:** Two ASCII code-block charts replaced with three `![…](images/…)` image references.
- **Key learning:** The `generate-charts.py` script is designed for easy data updates — just append new run numbers/times to the arrays at the top of the file and re-run.


📌 Team update (2026-03-11): Run 18 improvement recommendations prioritized by Forge — see decisions.md



📌 Team update (2026-03-11): User directives from Jeff  eliminate Test-UnconvertiblePage, standardize on ItemType, P0-2 approved  see decisions.md

