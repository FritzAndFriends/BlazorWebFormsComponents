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
