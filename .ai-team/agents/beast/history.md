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


 Team update (2026-03-04): PRs must target upstream FritzAndFriends/BlazorWebFormsComponents, not the fork  decided by Jeffrey T. Fritz
 Team update (2026-03-04): Migration test reports go in docs/migration-tests/{subfolder}/  decided by Jeffrey T. Fritz
 Team update (2026-03-04): Layer 1 benchmark baseline established  data at docs/migration-tests/wingtiptoys-2026-03-04/  decided by Cyclops
 Team update (2026-03-04): Migration Run 2  11/11 features pass, toolkit ready for customer-facing documentation  decided by Forge
