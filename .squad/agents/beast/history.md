# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- ⚠ Summarized 2026-02-27 by Scribe — covers M1–M16 -->

### Core Context (2026-02-10 through 2026-02-27)

**Doc structure:** title → intro (MS docs link) → Features Supported → NOT Supported → Web Forms syntax → Blazor syntax → HTML Output → Migration Notes (Before/After) → Examples → See Also. Admonitions for gotchas. mkdocs.yml nav alphabetical within categories. Migration section: "Getting started" and "Migration Strategies" at top.

**Key patterns:** Style migration: TableItemStyle → CSS class string parameters. DeferredControls.md has dual role (fully deferred + partially implemented). Chart screenshots at `docs/images/{component}/chart-{type}.png`. Shared sub-component docs linked from parents. PagerSettings is first shared sub-component with own doc page. Structural components (no HTML output) lead with "renders no HTML" callout. Audit reports at `planning-docs/AUDIT-REPORT-M{N}.md` with historical snapshot headers. Branch naming: `copilot/create-*`.

**Doc work completed:** M1–M3 docs (PasswordRecovery 3-step wizard, DetailsView generic component). Chart doc (JS interop "HTML Output Exception" pattern, Chart Type Gallery, child component doc pattern). M8 release-readiness polish (Substitution/Xml deferred in status.md, Chart Phase 1 hedging removed, README link fixes). M9 Doc Gap Audit (FormView, DetailsView, DataGrid, ChangePassword, PagerSettings.md created). ToolTip universality in Migration/readme.md. ThemesAndSkins.md updated for M10 PoC. NamingContainer.md created with IDRendering.md cross-refs. M9 Consolidated Audit Report (29 findings → M10 issues).

**Pending doc needs:** ClientIDMode property documentation (M16). Menu dual rendering modes. ListView CRUD events. Menu styles (IMenuStyleContainer). Post-M15 verification badges if new exact matches achieved. Login+Identity deferred — do not schedule docs.

<!-- ⚠ Summarized 2026-03-04 by Scribe — covers M17 docs through migration toolkit -->

### Doc Work Summary (2026-02-27 through 2026-03-03)

**M17 AJAX docs (6 pages):** Timer, ScriptManager, ScriptManagerProxy, UpdatePanel, UpdateProgress, Substitution. New "AJAX Controls" nav section in mkdocs.yml. Migration stub doc pattern established (warning admonition + ignored props + include→remove lifecycle). Substitution moved from deferred to implemented.

### 2026-04-27: MasterPages.md Documentation Update

**Task:** Document MasterPageContext architecture and Master/Content/ContentPlaceHolder migration patterns in docs/Migration/MasterPages.md.

**Changes delivered:**
- Created comprehensive MasterPages.md in docs/Migration/ folder
- Added MasterPageContext architecture section explaining cascading pattern
- Included before/after migration examples showing Web Forms vs Blazor syntax
- Documented Content/ContentPlaceHolder nesting patterns and best practices
- Added troubleshooting section for context lookup failures
- Cross-referenced ComponentList.razor for discoverability
- Structured with: Feature Overview → Architecture → Code Examples → Nesting Patterns → Common Issues → Migration Checklist
- Aligned with existing migration guide style (SkinsAndThemes.md pattern)

**Pattern consistency:** Matches Beast's established separation: SkinsAndThemes.md (strategy) vs SkinsAndThemesGuide.md (practical). MasterPages.md is the practical implementation guide.

**Issue #359 doc updates (5 pages):** ChangePassword and PagerSettings verified complete. FormView got CRUD events + NOT Supported section. DetailsView got full style sub-component elements. DataGrid paging section enhanced. Pattern: DataGrid is the only pageable control without PagerSettings.

**M10 Skins & Themes Guide:** Created `docs/Migration/SkinsAndThemes.md` — practical guide coexisting with `ThemesAndSkins.md` (strategy). Convention: separate "Guide" vs "Strategy" docs with clear nav labels.

**Executive Report:** `planning-docs/WINGTIPTOYS-MIGRATION-EXECUTIVE-REPORT.md` — 96.6% coverage, 55-70% time savings, 18-26 hour estimate.

**Migration Toolkit (6 docs):** README, QUICKSTART, CONTROL-COVERAGE (58 components, 6 categories), METHODOLOGY, CHECKLIST, copilot-instructions-template. Key: no content duplication, copilot-instructions-template is self-contained for external projects.

**Distributable BWFC Migration Skill:** Single self-contained SKILL.md (~750 lines) with 10 architecture decision templates, three-layer methodology, per-page checklist. NuGet-first, no internal repo references.

**Toolkit fixes:** Component count 52→58, internal references→distributed paths, AzimoLabs→FritzAndFriends. Key learning: toolkit coverage tables must be updated when new components are added.

**Migration test report structure:** `docs/migration-tests/` standard location. Per-run subfolder `{app}-{YYYY-MM-DD}` with `report.md` + `images/`. README.md index. Added "Migration Tests" nav section to mkdocs.yml.

**Pending doc needs:** ClientIDMode property. Menu dual rendering modes. ListView CRUD events. Menu styles (IMenuStyleContainer). Post-M15 verification badges.

### Key Team Updates (2026-02-27 through 2026-03-03)

- Branching: feature PRs from personal fork to upstream dev (Jeff)
- Issues closed via PR references only (Jeff)
- CascadedTheme (not Theme) is cascading parameter name (Cyclops)
- Theming sample page uses 6-section progressive layout (Jubilee)
- Unified release.yml — single workflow, version.json 3-segment SemVer (PR #408)
- Skins & Themes roadmap: 3 waves, 15 work items (Forge)
- Project reframed as migration acceleration system (Jeff)
- Themes (#369) implementation last — ListView CRUD first, WingtipToys second (Jeff)
- ListView EventArgs now include IOrderedDictionary properties (Cyclops)
- Migration toolkit restructured into self-contained migration-toolkit/ package (Jeff, Forge)


<!-- Summarized 2026-03-05 by Scribe -- covers Run 4-6 reports, render mode, WebFormsPageBase, Page System docs -->

### Migration Reports & Page System Docs Summary (2026-03-04 through 2026-03-05)

**Report conventions:** 3-level deep paths (`../../../planning-docs/`). Executive summary pattern (metrics table, 10-sec grasp). Works/Doesn't-Work sections (Run 5+). Script Bugs table (Run 6+). Enhancement impact table per-enhancement. Transform count decreases = quality improvements.

**Benchmark reports written:** Run 4 (`wingtiptoys-run4-2026-03-04`), Run 5 (`wingtiptoys-run5-2026-03-04`, 309 transforms), Run 6 (`wingtiptoys-run6-2026-03-04`, 269 transforms, ~4.5 min, 55% reduction). Run 5 added difficulty categorization. Run 6 added bug vs enhancement distinction.

**@rendermode correction:** Directive attribute on instances, not standalone. `_Imports.razor` gets `@using static`, `App.razor` gets `@rendermode="InteractiveServer"` on Routes/HeadOutlet. Updated migration-standards, bwfc-migration, METHODOLOGY.

**WebFormsPageBase docs:** Documented across bwfc-migration SKILL.md (`@inherits`, lifecycle table), migration-standards (target architecture, page base class), METHODOLOGY (scaffold). IPageService still valid for non-page components. Page.Request/Response/Session deliberately omitted.

**Page System doc rewrite:** PageService.md renamed to "Page System". Three-piece architecture (WebFormsPageBase primary, IPageService secondary). 3-column Key Differences table. mkdocs.yml + README updated.

Team updates (2026-03-04-05): PRs upstream, reports in docs/migration-tests/, benchmark baseline, Run 2/5/6 validated, GetRouteUrl overloads, standards formalized, @rendermode fix (PR #419), WebFormsPageBase/Page consolidation, 50 On-prefix aliases, AutoPostBack fix.

<!-- Summarized 2026-03-05 by Scribe -- covers Run 7 reports, control preservation docs, Run 9 benchmark -->

### Run 7-9 Reports & Control Preservation Docs (2026-03-05 through 2026-03-06)

**Control preservation docs:** METHODOLOGY.md, CHECKLIST.md, QUICKSTART.md updated with \Test-BwfcControlPreservation\. Rule: ALL asp: controls must be BWFC components, never raw HTML. Migration skill got 3 runtime gotchas (ListView @context, OnParametersSetAsync, AddHttpContextAccessor).

**Run 7 report:** \samples/Run7WingtipToys/MIGRATION-REPORT.md\  32 files, 331 transforms, 1.2s, 97% accuracy. Report structure: exec summary, metrics tables, run-over-run comparison, recommendations. Co-located with output per Jeff.

**Run 7 skill updates:** 5 files updated for runtime failures (UseStaticFiles 404s, AuthorizeView crashes, asset paths). Key pattern: runtime failures more dangerous than compile errors.

**Run 9 report:** \docs/Migration/Run9-WingtipToys-Benchmark.md\ + \samples/Run9WingtipToys/BENCHMARK-REPORT.md\. 667 transforms, 173 BWFC instances (23 types), 8 pages, 0 errors. LoginView preservation (native BWFC, not AuthorizeView rewrite) is the standard. \ItemType\TItem\ bug persists. Convention: BENCHMARK-DATA.md (Bishop)  BENCHMARK-REPORT.md (Beast).

 Team update (2026-03-05): Forge APPROVED Run 9 migration (98.9% control preservation). 2 findings: ImageButtonimg (P0), HyperLink dropped (P2). Beast's benchmark report structure (dual-location, 3-run comparison) established as standard.  decided by Forge

### Squad Places Knowledge Artifact (2026-03-05)

- Published lesson artifact to Squad Places (social network for AI agent teams): **"What 110+ Web Forms Controls Taught Us About Migration-First Component Design"** (type: lesson, ID: `bf853f10-27f0-4ee8-bee8-e43695bf150e`). Covers 6 hard-won lessons: HTML fidelity, exact attribute naming, real enums, data binding translation, EventCallback dual pattern, base class hierarchy. Tags: blazor, webforms, migration, lessons-learned, component-design, dotnet.
- Squad Places is our team's social network presence. Squad ID: `5b52c25e-9e05-4c03-a392-16c58a57b144`. API: `https://api.nicebeach-b92b0c14.eastus.azurecontainerapps.io`.

### Run 10 Benchmark Report (2025-07-25)

- **Run 10 report written:** `docs/Migration/Run10-WingtipToys-Benchmark.md` + `samples/Run10WingtipToys/BENCHMARK-REPORT.md`. Cycle 1 of 3-cycle improvement loop. 673 transforms, 172 BWFC instances (26 types), 8 functional pages, 3 build attempts (down from 7). All 5 P0/P1 fixes validated. mkdocs.yml nav updated.
- **Improvement loop report pattern:** When reporting on fix-validation runs, lead Executive Summary with fix impact table, add "Measured Impact" subsection under improvements, and include "Recommendations for Cycle N+1" section targeting specific build-attempt reduction goals.
- **On-disk vs benchmark discrepancy:** BENCHMARK-DATA.md reported 44 .cs files but on-disk count was 38. Used benchmark numbers for consistency — discrepancy likely due to intermediate files consolidated during Layer 2 optimization.
- **Run 10 key metrics:** Build attempts 7→3 (57% fewer), Layer 2 time 45→25 min (44% faster), 3 new BWFC types counted (LinkButton, ModelErrorMessage, FileUpload). Remaining 3 build attempts target enum string→type conversions (P2 candidates for Cycle 2).
- **4-run comparison table established:** Run-over-Run table now covers Runs 7–10. Added ItemType bug and validator params rows to track fix propagation across runs.

 Team update (2026-03-06): Run 10 BWFC preservation review: 92.7% (164/177)  NEEDS WORK. Below 95% threshold. P0-2 smart stubs fix recovered 113 controls across 12 pages. 3 gaps remain for Cycle 2.  decided by Forge

### Run 11 Benchmark Report (2025-07-25)

- **Run 11 report written:** Commit 49b556be. Cycle 2 of 3-cycle improvement loop. 178 BWFC instances (26 unique types), 4 build attempts, 0 errors. All 3 P0 gaps from Run 10 closed.
- **Key metrics:** Preservation rate 92.7% to 98.9% (Cycle 2 target of >=98% met). Build attempts 3 to 4 (slight increase due to stub model iteration). Layer 2 time ~20min.
- **5-run comparison table:** Runs 7-11 now tracked. ItemType stripping and stub model pattern are new capabilities validated.
- **Convention:** BENCHMARK-DATA.md (Bishop) to BENCHMARK-REPORT.md (Beast) pipeline continues.

 Team update (2025-07-25): Forge reviewed Run 11 preservation: 98.9% (176/178 adjusted) -- APPROVED. All 3 P0 gaps CLOSED. Cycle 3 priorities: functional Login/Register code-behinds per Jeff's directive. -- decided by Forge

📌 Team update (2026-03-06): migration-toolkit is end-user distributable; migration skills belong in migration-toolkit/skills/ not .squad/skills/ — decided by Jeffrey T. Fritz

### Run 7 Benchmark Report (2026-03-06)

- **Run 7 report written:** `dev-docs/migration-tests/wingtiptoys-run7-2026-03-06/report.md` + `raw-data.md`. First runtime-validated benchmark with acceptance tests.
- **Key metrics:** 3.33s script time (27% faster than Run 6), 366 transforms (+97), 80 static files, 2 build rounds (down from 4), 14/14 acceptance tests PASS.
- **New report section:** §5 Acceptance Test Results — test project location, full test breakdown table, fix iteration documentation with Problem/Fix/Impact pattern.
- **Three runtime patterns documented:** Bootstrap CSS links in App.razor, scoped auth providers + cookie auth, anchor-based form submit (`<a role="button">` + minimal API POST endpoints).
- **Layer 2 scope:** 33 files rewritten, 14 modified, 8 created. EF Core models + DbContext + SQLite + seed data, ShoppingCartService, CartStateService, cookie auth wiring.
- **Convention established:** Runtime correctness (acceptance tests) supersedes compilation as the primary quality gate for migration benchmarks going forward.
- **README.md updated:** Added Runs 5–7 to migration test index table.

 Team update (2026-03-06): Layer 2 conventions established  Button OnClick uses EventArgs (not MouseEventArgs), code-behind class names must match .razor filenames exactly, use EF Core wildcard versions for .NET 10, CartStateService replaces Session, GridView needs explicit TItem  decided by Cyclops


 Team update (2026-03-06): WebFormsPageBase is the canonical base class for all migrated pages (not ComponentBase). All agents must use WebFormsPageBase  decided by Jeffrey T. Fritz
 Team update (2026-03-06): LoginView is a native BWFC component  do NOT convert to AuthorizeView. Strip asp: prefix only  decided by Jeffrey T. Fritz
