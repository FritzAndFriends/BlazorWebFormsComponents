# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents  Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Core Context

<!-- Summarized 2026-03-04 by Scribe — originals in history-archive.md -->

M1–M16: 6 PRs reviewed, Calendar/FileUpload rejected, ImageMap/PageService approved, ASCX/Snippets shelved. M2–M3 shipped (50/53 controls, 797 tests). Chart.js for Chart. DataBoundStyledComponent<T> recommended. Key patterns: Enums/ with int values, On-prefix events, feature branches→upstream/dev, ComponentCatalog.cs. Deployment: Docker+NBGV, dual NuGet, Azure webhook. M7–M14 milestone plans. HTML audit: 3 tiers, M11–M13. M15 fidelity: 132→131 divergences, 5 fixable bugs. Data controls: 90%+ sample parity, 4 remaining bugs. M17 AJAX: 6 controls shipped.

## Learnings


<!-- Summarized 2026-03-05 by Scribe -- covers M17 gate review through Page consolidation -->

### M17 through Page Consolidation Summary (2026-02-28 through 2026-03-05)

**M17 gate review:** 6 AJAX controls approved. Timer/UpdateProgress/Substitution 100%, UpdatePanel 80%, ScriptManager 41% (appropriate). 5 fidelity fixes in PR #402. D-11 to D-14 divergences catalogued. Skins roadmap: 3 waves, 15 WIs, .skin parser as source gen.

**Build/Release:** version.json 0.17.0 (3-segment SemVer). Unified release.yml (PR #408). Old workflows to dispatch-only. Docker ghcr.io, dual NuGet. NBGV 3.9.50.

**M22 & WingtipToys:** 12 WIs, 4 waves. 57 controls (51 functional, 6 stubs). WingtipToys: 15+ pages, 22 controls, 100% BWFC coverage. FormView RenderOuterTable resolved. Three-layer pipeline: L1 ~40% mechanical, L2 ~45% structural, L3 ~15% semantic. 85+ syntax patterns from 33 files. ModelErrorMessage: BaseStyledComponent, CascadingParameter EditContext, 29/29 coverage. Pipeline validated at ~70% markup. 18-26 hours total.

**CSS & Schedule:** 7 WingtipToys visual fixes (Cerulean, grid, category, Site.css, gradients). 26 WIs, 7 phases. Migration toolkit: 9 docs, copilot-instructions-template.md highest value. Restructured to migration-toolkit/.

**Run 4-6:** Run 4: ConvertFrom-MasterPage highest-impact, 289 transforms, 0 errors. Run 5: 95+ EventCallbacks, 3 of 4 top rewrites unnecessary. 8 enhancements identified. Run 6: 32 files, clean build ~4.5 min (55% reduction). 269 transforms, 79 static files, 6 auto-stubs. Bugs: @rendermode _Imports.razor, stub misses code-behind.

**Page base class:** Option C recommended (WebFormsPageBase:ComponentBase + Page=>this). Title/MetaDescription/MetaKeywords via IPageService. IsPostBack=>false. Eliminates 27 @inject lines, 12+ IsPostBack fixes. Does NOT inherit BaseWebFormsComponent.

**Page consolidation:** Option B (merge Page.razor into WebFormsPage). PageTitle/HeadContent work from anywhere in render tree. RenderPageHead parameter. Minimum setup: @inherits + wrapper component. Cannot get below 2 setup points. Decision: forge-page-consolidation.md.

Team updates (2026-03-04-05): PRs upstream, reports in docs/migration-tests/, benchmarks L1/L2+3, @rendermode fix (PR #419), EF Core 10.0.3, WebFormsPageBase shipped, WebFormsPage consolidation, 50 On-prefix aliases, AutoPostBack fix.
<!-- Summarized 2026-03-06 by Scribe -- covers event handler audit through Run 7 transforms -->

### 2026-03-05 Architecture & Migration Summary

**Event handler audit:** ~50 EventCallbacks across data components used bare names instead of On-prefixed names matching Web Forms attributes. Button/input/list/login controls all correct. Repeater has zero EventCallbacks (critical gap). GridView missing OnRowDataBound, OnRowCreated. FormView naming inconsistent. Script does zero event handler transformation (correct if components use On-prefix). Recommended On-prefix aliases (non-breaking).

**ShoppingCart gap analysis:** BWFC GridView supports ALL needed features (CssClass, AutoGenerateColumns, BoundField, TemplateField, TextBox/CheckBox in templates, ShowFooter, GridLines, CellPadding). AfterWingtipToys was plain HTML table (anti-pattern). FreshWingtipToys proves correct migration. Root cause: Layer 2 decomposed GridView to raw HTML.

**BWFC control preservation standards:** Updated migration-standards SKILL.md with 5 mandatory rules, ShoppingCart anti-pattern, BAD/GOOD examples. Added `Test-BwfcControlPreservation` to bwfc-migrate.ps1 (post-transform verification). Non-blocking warnings in ManualItems report. Propagated to distributable `migration-toolkit/skills/` (migration-standards + bwfc-migration).

**Run 7 Layer 2/3:** 6 core storefront pages transformed. FormView preserved with `Items=@(new List<Product>{SampleProduct})` single-item wrapper. Category ListView preserved. ShoppingCart GridView preserved with @rendermode InteractiveServer. ProductContext (no Identity). 26 code-behinds + 12 .razor files stubbed to ComponentBase. Build: 0 errors. Key: FormView has no DataItem parameter, use Items with single-item list. Layer 1 residuals cluster in ItemTemplate sections.

Team updates: Event handler audit, On-prefix aliases (50), ShoppingCart regression test, BWFC preservation mandatory, AfterWingtipToys output-only, Run 7 report structure, Run 7 runtime learnings.
### Migration Script Gap Review — Run 7 Learnings (2026-03-05)

**Task:** Review bwfc-migrate.ps1 for remaining gaps after Run 7 fixes were applied.

**Key findings — 6 gaps identified:**

1. **`src="~/"`** not in `ConvertFrom-UrlReferences` — images and scripts with `src="~/path"` keep broken `~/` prefix. Only `href`, `NavigateUrl`, `ImageUrl` are handled.
2. **`<script>` tags lost from master page `<head>`** — CSS links are extracted to App.razor but script tags are not. The entire `<head>` is removed, silently dropping head scripts.
3. **No BundleConfig.cs / `Styles.Render` / `Scripts.Render` detection** — bundling calls become invalid `@(Styles.Render(...))` in output with no warning.
4. **CSS link duplication** — Same stylesheet links appear in both App.razor `<head>` (via `New-AppRazorScaffold`) and layout's `<HeadContent>` (via `ConvertFrom-MasterPage`).
5. **`url('~/')` in CSS files not converted** — CSS files are copied verbatim; `~/` references inside them break.
6. **No flagging of infrastructure files** — Global.asax, RouteConfig.cs, BundleConfig.cs, web.config are not detected or flagged for manual review.

**Verified working (9 items):** UseStaticFiles, CSS extraction, SourceRoot parameter, LoginView→AuthorizeView, GetRouteUrl passthrough, static file directory structure, BWFC control preservation, AutoPostBack stripping, event handler scanning.

**Decision document:** `.ai-team/decisions/inbox/forge-script-gap-review.md`



 Team update (2026-03-05): BWFC control preservation is mandatory  all migration output must use BWFC components, never flatten to raw HTML. Cyclops's decision merged into consolidated block.  decided by Jeffrey T. Fritz, Forge, Cyclops

### BWFC-First Migration Skill Rewrite (2026-03-05)

**Task:** Rewrote all 4 migration skill files + CHECKLIST.md + METHODOLOGY.md to make BWFC library usage the #1 priority per Jeff's directive.

**Root cause:** Runs 6-8 Layer 2 agents consistently replaced BWFC components with plain HTML. Skills didn't make BWFC prominent enough.

**Changes across 6 files:**
- Every skill opens with 🚫 MANDATORY banner about BWFC-first
- Section 1 of every skill is BWFC inventory/utility features
- Complete 110+ component inventory (was 58) in bwfc-migration and migration-standards
- LoginView/LoginStatus explicitly called out as commonly missed (in all 4 skills)
- Anti-pattern tables with ❌/✅ comparison pairs throughout
- Standard Blazor patterns for static files/CSS/JS in infrastructure tables
- BWFC utility features (AddBlazorWebFormsComponents, WebFormsPageBase, Page) marked MANDATORY
- CHECKLIST.md: 9 new 🚫 BWFC VERIFICATION items across all 3 layers
- METHODOLOGY.md: Layer 2 gets explicit "MUST NOT" list of 5 forbidden patterns
- bwfc-identity-migration: LoginView/LoginStatus front-loaded as Section 1, AuthorizeView demoted to "optional upgrade"
- bwfc-data-migration: BWFC data controls front-loaded with "BWFC Front-End + Service Back-End" pattern
