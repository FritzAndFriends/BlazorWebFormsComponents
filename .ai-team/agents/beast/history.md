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

### Migration Report Conventions (2026-03-04)

- **Image path depth**: Reports in `docs/migration-tests/{app}-{run}/report.md` are 3 levels deep from repo root. Relative paths to `planning-docs/` must use `../../../planning-docs/`, not `../../planning-docs/`. This is a common off-by-one error to watch for.
- **Executive summary pattern**: Migration reports should open with a concise paragraph summarizing enhancements tested, pass/fail, and key deltas from prior runs, followed by a quick-reference metrics table (8–10 rows). Executives should grasp the full picture in ≤10 seconds.
- **Run 4 report location**: `docs/migration-tests/wingtiptoys-run4-2026-03-04/report.md` with local `images/` subfolder for Blazor screenshots and cross-references to `planning-docs/screenshots/` for original Web Forms screenshots.

 Team update (2026-03-05): GetRouteUrl RouteValueDictionary overloads now functional  all 4 overloads match Web Forms API  decided by Cyclops

### Run 5 Benchmark Report (2026-03-05)

- **Report written:** `docs/migration-tests/wingtiptoys-run5-2026-03-04/report.md` — comprehensive 9-section report with executive summary, metrics comparison table, what-works/what-doesn't breakdown, enhancement impact analysis, Layer 2 fixes summary, build results, gap analysis, and recommendations.
- **Key convention reinforced:** When manual review item counts increase between runs, explain *why* in the report (granular flagging vs regression). Jeff needs to see that higher counts can mean better output quality.
- **Report structure evolution:** Run 5 report adds "What Works" and "What Doesn't Work" sections (Jeff's request) plus categorization of manual items by difficulty ("mechanical but tedious" vs "requires architectural decisions"). This pattern should carry forward to future runs.
- **Enhancement impact table pattern:** Per-enhancement rows with Fired/Count/Run4-impact/Run5-status/Net-impact columns. Effective for showing ROI of individual script improvements.

 Team update (2026-03-04): Run 5 migration complete  309 transforms, clean build (0 errors, 0 warnings, 4.56s). Benchmark report pending. Key artifacts at docs/migration-tests/wingtiptoys-run5-2026-03-04/  decided by Cyclops

 Team update (2026-03-04): Migration standards formalized  EF Core, .NET 10, ASP.NET Core Identity, BWFC event handler preservation. Documentation priorities: document single-item FormView usage, document ListView Items parameter in migration context. migration-toolkit/ is canonical home.  decided by Jeffrey T. Fritz, Forge

📋 Team update (2026-03-04): Run 6 improvement analysis  decided by Forge

### Run 6 Benchmark Report (2026-03-04)

- **Report written:** `docs/migration-tests/wingtiptoys-run6-2026-03-04/report.md` — comprehensive 9-section report matching Run 5 format with executive summary, Run 5 vs Run 6 metrics comparison, what-works/what-doesn't breakdown, enhancement impact analysis, Layer 2 fixes summary, build results (4 rounds), gap analysis (2 script bugs), and recommendations.
- **Key data:** 55% total time reduction (Run 5 ~10 min → Run 6 ~4.5 min). Layer 2 manual time dropped 53% (440s → 205s). 4 enhancements all fired. 269 transforms, 79 static files to wwwroot/, 6 auto-stubs.
- **Format evolution:** Run 6 report adds explicit "Script Bugs Found" table in Gaps section (separate from "Patterns That Could Be Enhanced"). This distinguishes regressions/bugs from enhancement opportunities — important for prioritizing Run 7 fixes.
- **Transform count can decrease:** Run 6 had fewer transforms than Run 5 (269 vs 309) because auto-stubbing replaces full transforms for unconvertible pages. Reports should explain count decreases as quality improvements, not regressions.
- **Build rounds can increase without regression:** Run 6 had 4 build rounds vs Run 5's 2, but for entirely different root causes (NuGet auth, @rendermode bug). Reports should contextualize build round counts with root cause analysis.
- **Highest-impact enhancement pattern:** SelectMethod BWFC-aware guidance changed the migration *approach* (preserve components vs replace with HTML), not just the speed. Enhancement impact sections should capture qualitative shifts, not just time savings.

 Team update (2026-03-04): Run 6 benchmark validates all migration-standards SKILL.md patterns. 32 Web Forms files  clean Blazor build in ~4.5 min (55% reduction from Run 5). 2 script bugs identified: @rendermode in _Imports.razor, stub detection misses code-behind.  decided by Forge

### Render Mode Placement Correction (2026-03-05)

- **Key learning:** `@rendermode` is a directive *attribute* (goes on component instances like `<Routes @rendermode="InteractiveServer" />`), NOT a standalone directive. Placing `@rendermode InteractiveServer` in `_Imports.razor` causes build errors (RZ10003, CS0103, RZ10024).
- **Correct pattern:** `_Imports.razor` gets `@using static Microsoft.AspNetCore.Components.Web.RenderMode` (enables shorthand). `App.razor` gets `@rendermode="InteractiveServer"` on both `<Routes>` and `<HeadOutlet>`.
- **Files updated:** `migration-toolkit/skills/migration-standards/SKILL.md` (new "Render Mode Placement" subsection under Target Architecture), `migration-toolkit/skills/bwfc-migration/SKILL.md` (Step 2 expanded with `@using static` + new Step 2b for App.razor), `migration-toolkit/METHODOLOGY.md` (scaffold table includes App.razor).
- **Reference:** https://learn.microsoft.com/aspnet/core/blazor/components/render-modes
- **Source:** Jeff confirmed correct pattern; Microsoft Learn docs verified.


 Team update (2026-03-04): @rendermode InteractiveServer belongs in App.razor, not _Imports.razor  consolidated from Forge, Cyclops, Jeffrey T. Fritz (PR #419)


 Team update (2026-03-04): EF Core must use 10.0.3 (latest .NET 10)  directed by Jeff

### WebFormsPageBase Documentation (2026-03-05)

- **Architecture decision documented:** `WebFormsPageBase` inherits from `ComponentBase` and provides `Title`, `MetaDescription`, `MetaKeywords`, `IsPostBack`, and `Page` (self-reference) for converted ASPX pages. Eliminates per-page `@inject IPageService` boilerplate.
- **bwfc-migration SKILL.md updated:** Step 2 (`_Imports.razor`) now includes `@inherits BlazorWebFormsComponents.WebFormsPageBase`. Step 3 expanded to include `<BlazorWebFormsComponents.Page />` in layout. Lifecycle methods table updated for `IsPostBack` (works AS-IS) and `if (IsPostBack)` (dead code — flag for review). "No PostBack" gotcha rewritten. New "Page Base Class" row added to control translation table.
- **migration-standards SKILL.md updated:** Target Architecture table updated (`WebFormsPageBase` for pages, `ComponentBase` for non-page components). New "Page Base Class" section added with one-time setup, properties table, and guidance on when `@inject IPageService` is still needed. Page Lifecycle Mapping updated for `IsPostBack` and `Page.Title`. Anti-pattern section corrected to show `WebFormsPageBase` as the right base class.
- **METHODOLOGY.md updated:** Layer 1 scaffold section now documents that `_Imports.razor` includes `@inherits WebFormsPageBase` and layout includes `<BlazorWebFormsComponents.Page />`.
- **Key nuance preserved:** `@inject IPageService` remains valid and documented for non-page components. `<BlazorWebFormsComponents.Page />` render component is still required in the layout — `WebFormsPageBase` provides the code-behind API, `<Page />` does the rendering. `Page.Request`, `Page.Response`, `Page.Session` are deliberately omitted to force proper Blazor migration.
- **Source:** Architecture decision at `.ai-team/decisions/inbox/forge-page-base-class.md`, requested by Jeffrey T. Fritz.

 Team update (2026-03-04): WebFormsPageBase implemented  decided by Forge, approved by Jeff

### Page System Documentation Rewrite (2026-03-05)

- **PageService.md rewritten as "Page System" doc:** Restructured from single-approach `@inject IPageService` doc to comprehensive three-piece architecture guide. Leads with `WebFormsPageBase` as primary approach for converted pages, keeps `@inject IPageService` as secondary approach for non-page components.
- **New sections added:** Architecture diagram (ASCII art showing WebFormsPageBase → IPageService → Page.razor flow), One-Time Setup, Properties Available/NOT Available tables, `if (IsPostBack)` dead code warning admonition.
- **Key Differences table expanded:** Now has three columns (Web Forms / WebFormsPageBase / inject IPageService) and includes `IsPostBack`, inheritance, and `Page.Request`/`Page.Response` rows showing deliberate omissions.
- **Migration Path updated:** Before/After now shows `WebFormsPageBase` inheritance approach with `if (!IsPostBack)` preserved, instead of the previous `@inject` approach.
- **mkdocs.yml nav renamed:** `PageService` → `Page System` to reflect broader scope.
- **README.md updated:** Utility Features entry now references "Page System" and mentions WebFormsPageBase, IPageService, and Page renderer.
- **Key distinction preserved:** `WebFormsPage` (NamingContainer + ThemeProvider) is a completely different component — added explicit cross-reference in See Also to avoid confusion.
- **Learning:** When a utility feature grows from "one service" to "multi-piece system," the doc title and nav label should reflect the system, not the individual service. This prevents readers from thinking the doc only covers one piece.

� Team update (2026-03-05): WebFormsPage now includes IPageService head rendering (title + meta tags), merging Page.razor capability per Option B consolidation. Layout simplified to single <WebFormsPage> component. Page.razor remains standalone.  decided by Forge, implemented by Cyclops


 Team update (2026-03-06): CRITICAL  Git workflow: feature branches from dev, PRs target dev. NEVER push to or merge into upstream main (production releases only).  directed by Jeff Fritz

### Comprehensive Skills Cross-Reference Review (2026-03-06)

- **Scope:** Reviewed all 7 skill files + 4 supporting migration-toolkit docs against actual BWFC library source code.
- **Most stale file:** `.ai-team/skills/migration-standards/SKILL.md` — had 7 issues including wrong base class (`ComponentBase` instead of `WebFormsPageBase`), stale IsPostBack/Page.Title mappings, and incorrect LoginView→AuthorizeView guidance.
- **Key discrepancy pattern:** Two versions of `migration-standards/SKILL.md` exist (`.ai-team/` and `migration-toolkit/`). The migration-toolkit version was well-maintained but the .ai-team version was severely stale. **Both must be updated whenever migration standards change.**
- **LoginView is NOT a shim:** All 3 docs that mentioned LoginView→AuthorizeView were wrong. `LoginView.razor.cs` injects `AuthenticationStateProvider` natively — it's a first-class Blazor component, not a shim that needs replacement.
- **WebFormsPageBase not propagated everywhere:** `bwfc-migration` SKILL had it, but `copilot-instructions-template.md`, `QUICKSTART.md`, and `CHECKLIST.md` still referenced the old "remove IsPostBack" or "no PostBack" patterns.
- **Missing BWFC features:** The `bwfc-migration` SKILL was missing: WebFormsPage (unified wrapper), MasterPage/Content/ContentPlaceHolder components, DataBinder.Eval shim, NamingContainer, Theming infrastructure, EmptyLayout, CustomControls base classes.
- **Report location:** `dev-docs/skills-review-2026-03-06.md`
- **Files updated:** 7 files, 16+ individual fixes across skills and supporting docs.
- **Pattern observed:** Internal `.ai-team/skills/` files drift behind `migration-toolkit/skills/` files because team updates primarily target the external-facing toolkit.

 Team update (2026-03-06): CONTROL-COVERAGE.md updated  library ships 153 Razor components (was listed as 58). ContentPlaceHolder reclassified from 'Not Supported' to Infrastructure Controls. Reference updated CONTROL-COVERAGE.md for accurate component inventory.  decided by Forge

### Run 8 Migration Report Enhancement (2026-03-06)

- **Report updated:** `dev-docs/migration-tests/wingtiptoys-run8-2026-03-06/REPORT.md` — enhanced with executive-focused content: timing timeline, screenshot gallery, and 4 before/after code comparisons.
- **Executive report pattern:** Lead with a blockquote "bottom line" sentence, then timeline visualization (ASCII art), then screenshot gallery in markdown tables, then before/after code blocks with green-check callouts. Technical sections preserved below the fold.
- **Before/after comparison technique:** Show 3–4 key pages with stacked code blocks (Before/After) and a callout summarizing what changed and what stayed the same. Best pairs: simple pages (Default), layout/master page, data-bound lists (ProductList), and forms (Login). Executives care most about "the markup is recognizable."
- **Screenshot gallery layout:** Use 2-column markdown tables with `![alt](path)` in cells and bold captions below. Group by functional area (Navigation, Catalog, Cart, Auth). Relative paths from report location: `screenshots/blazor-*.png`.
- **Timing data matters for executives:** The ASCII timeline diagram + phase breakdown table + category percentages (automated <1%, manual 23%, testing 70%) tell the story faster than prose. The "under 2 hours" headline is the money number.
- **Key file paths:** Report at `dev-docs/migration-tests/wingtiptoys-run8-2026-03-06/REPORT.md`, screenshots in `screenshots/` subfolder, Web Forms source at `samples/WingtipToys/WingtipToys/`, Blazor output at `samples/AfterWingtipToys/`.

 Team update (2026-03-06): Only document top-level components and utility features for promotion. Do not promote/document style sub-components, internal infrastructure, or implementation-detail classes.  decided by Jeffrey T. Fritz
