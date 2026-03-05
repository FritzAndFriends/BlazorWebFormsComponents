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
### WingtipToys Migration Gotchas (2026-03-05)

- Migration skill updated with three runtime gotchas from WingtipToys: ListView @context templates, OnParametersSetAsync for query params, AddHttpContextAccessor registration
- Confidence bumped to high — patterns validated through real migration debugging

### Control Preservation Docs Update (2026-03-05)

- **METHODOLOGY.md updated:** New "Post-Transform Verification: Control Preservation" subsection in Layer 1 documenting `Test-BwfcControlPreservation`. Includes warning callout: never flatten BWFC components to raw HTML.
- **CHECKLIST.md updated:** Three new checklist items — Layer 1 control preservation verified, Layer 2 no-flattening guard, Verification section all-controls-preserved check.
- **QUICKSTART.md updated:** Step 3 now mentions the automatic control preservation check and how to interpret deficit warnings.
- **CONTROL-COVERAGE.md:** Reviewed, no changes needed — it's a control reference table, not a process doc.
- **Key rule documented:** ALL `asp:` controls must be preserved as BWFC components. Flattening GridView→table or TextBox→input is always wrong. This is Jeff's directive and the script now enforces it automatically.

 Team update (2026-03-05): BWFC control preservation is mandatory  all asp: controls must be preserved as BWFC components in migration output, never flattened to raw HTML. Test-BwfcControlPreservation verifies automatically.  decided by Jeffrey T. Fritz, implemented by Forge

