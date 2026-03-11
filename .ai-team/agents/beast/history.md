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

### Migration Pipeline Enforcement in Skill Docs (2026-03-11)

- **Scope:** Updated `migration-toolkit/skills/bwfc-migration/SKILL.md` and `migration-toolkit/skills/migration-standards/SKILL.md` per Jeff Fritz directive.
- **bwfc-migration SKILL.md changes:**
  - Added prominent "Migration Pipeline — MANDATORY" section near the top (after Installation, before existing Migration Workflow) with critical warning admonition, pipeline step table, Layer 1 invocation command, Layer 2 Copilot transform checklist, and pipeline rules.
  - Fixed all `TItem` references → `ItemType` throughout the file (lines 262, 274, 294, 334, 355, 479, 490, 624). BWFC standardized on `ItemType` to match Web Forms `DataBoundControl.ItemType`.
  - Updated Layer 2 checklist: "ItemType → TItem" → "ItemType preserved (strip namespace prefix only)".
  - Updated GridView Key changes note: removed incorrect "ItemType → TItem" guidance.
- **migration-standards SKILL.md changes:**
  - Renamed "Layer 1 (Script) vs Layer 2 (Manual) Boundary" → "Layer 1 (Script) vs Layer 2 (Copilot-Assisted) Boundary".
  - Added critical warning admonition about no manual fixes between layers.
  - Added `bwfc-migrate.ps1` invocation command.
  - Expanded Layer 2 description: "Always manual" → "Copilot-Assisted" with additional transform items (data loading, template context, navigation).
- **Key convention:** `ItemType` is the canonical attribute name for BWFC data controls. Never use `TItem`, `TItemType`, or other variants.
- **Key convention:** Layer 1 = automated script, Layer 2 = Copilot-assisted. No manual fixes between layers. This is a measurement integrity requirement.


 Team update (2026-03-11): Mandatory L1L2 migration pipeline  no code fixes between layers. Both layers must run in sequence.  decided by Jeffrey T. Fritz

 Team update (2026-03-11): All generic type params standardized to ItemType (not TItem/TItemType) across all BWFC data-bound components.  decided by Jeffrey T. Fritz

 Team update (2026-03-11): P0 script fixes  Test-UnconvertiblePage eliminated (always convert), [Parameter] annotation bug fixed.  decided by Jeffrey T. Fritz

### SelectMethod Skills Fix + Run 20 Report Corrections (2025-07-24)

- **Scope:** Fixed incorrect SelectMethod guidance across all three migration skill files and corrected factual errors in Run 20 report.
- **FIX 1 — SelectMethod guidance (all 3 skill files):**
  - `bwfc-migration/SKILL.md`: Updated 8 locations — Layer 2 bullet, GridView/ListView/FormView examples, Data Binding Migration tables, and per-page checklist. All "SelectMethod → Items" guidance changed to "SelectMethod PRESERVED — convert string to SelectHandler<ItemType> delegate."
  - `migration-standards/SKILL.md`: Updated 3 locations — main SelectMethod guidance paragraph, Layer 2 description, and ListView before/after example (now shows both SelectMethod delegate and Items options).
  - `bwfc-data-migration/SKILL.md`: Updated 4 locations — When to Use description, EF6 context, SelectMethod mapping table (now shows delegate conversion + alternative Items approach), and Files table. Also fixed stray `TItem` → `ItemType`.
- **FIX 2 — Run 20 Report validator claim:** Removed false bullet stating RequiredFieldValidator, CompareValidator, RegularExpressionValidator, and ModelErrorMessage are "not yet implemented." All exist in `src/BlazorWebFormsComponents/Validations/`.
- **FIX 3 — Run 20 Report SelectMethod:** Updated L2 data binding description and L1 review items appendix to reflect SelectMethod delegate conversion instead of IDbContextFactory replacement.
- **FIX 4 — GetRouteUrlHelper and ContentPlaceHolder:** Added GetRouteUrlHelper documentation to bwfc-migration route URL section. ContentPlaceHolder/Content/MasterPage already well-documented at lines 593, 709-711.
- **Key learning:** `DataBoundComponent<ItemType>.SelectMethod` is a `SelectHandler<ItemType>` delegate parameter, not a string. BWFC's `OnAfterRenderAsync` auto-populates `Items` when `SelectMethod` is set. This is the native BWFC data-binding path that mirrors Web Forms behavior.


 Team update (2026-03-11): SelectMethod must be preserved in L1 script and skills  BWFC supports it natively via SelectHandler<ItemType> delegate. All validators exist in BWFC.
