# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents  Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Core Context

<!-- Summarized 2026-03-04 by Scribe — originals in history-archive.md -->

M1–M16: 6 PRs reviewed, Calendar/FileUpload rejected, ImageMap/PageService approved, ASCX/Snippets shelved. M2–M3 shipped (50/53 controls, 797 tests). Chart.js for Chart. DataBoundStyledComponent<T> recommended. Key patterns: Enums/ with int values, On-prefix events, feature branches→upstream/dev, ComponentCatalog.cs. Deployment: Docker+NBGV, dual NuGet, Azure webhook. M7–M14 milestone plans. HTML audit: 3 tiers, M11–M13. M15 fidelity: 132→131 divergences, 5 fixable bugs. Data controls: 90%+ sample parity, 4 remaining bugs. M17 AJAX: 6 controls shipped.

## Learnings

<!-- Summarized 2026-03-02 by Scribe -- covers M17 gate review through Themes roadmap -->

### M17-M18 Audit & Themes Roadmap Summary (2026-02-28 through 2026-03-01)

**M17 gate review:** All 6 AJAX controls approved with notes. Property coverage: Timer 100%, UpdateProgress 100%, UpdatePanel 80%, ScriptManager 41% (appropriate for no-op stub), Substitution 100%. 5 follow-up fidelity fixes all resolved in PR #402 (EnablePartialRendering default, Scripts collection, CssClass rendering, display:block, ScriptReference properties). Key pattern: AJAX controls split into functional (Timer, UpdatePanel, UpdateProgress) and migration stubs (ScriptManager, ScriptManagerProxy, Substitution). Audit report: planning-docs/M17-CONTROL-AUDIT.md.

**Divergence registry D-11 to D-14:** D-11 GUID IDs (fix recommended), D-12 boolean attrs (intentional, normalize), D-13 Calendar day padding (fix recommended), D-14 Calendar style pass-through (fix progressively). File: planning-docs/DIVERGENCE-REGISTRY.md.

**Skins & Themes roadmap (#369):** 3 waves, 15 work items. Sub-component styles via Dictionary<string, TableItemStyle> on ControlSkin. ThemeMode enum (StyleSheetTheme vs Theme). 6 style container interfaces with ~41 sub-style slots. EnableTheming propagation via cascading bool. .skin parser as build-time source generator.

Team updates: M17 audit fixes resolved (PR #402), Skins dual docs (SkinsAndThemes.md + ThemesAndSkins.md), Normalizer pipeline codified (Issue #387).



<!-- Summarized 2026-03-02 by Scribe -- covers Build/Release audit through WingtipToys pipeline validation -->

### Build/Release & M22 Migration Summary (2026-03-02)

**Build/Version/Release audit:** version.json on main said 0.15 vs latest tag v0.16. Unified release.yml implemented (PR #408)  single workflow on release:published coordinates NuGet + Docker + docs + demos. NBGV 3.9.50, PackageId Fritz.BlazorWebFormsComponents, Docker ghcr.io/fritzandfriends/blazorwebformscomponents/serversidesamples. version.json now 0.17.0 (3-segment SemVer). Old nuget.yml/deploy-server-side.yml refactored to workflow_dispatch-only.

**M22 Copilot-Led Migration Showcase:** 12 work items, 4 waves. 57 controls (51 functional, 6 stubs). All 16 core demo controls ready. Use existing BeforeWebForms sample (6-8 pages). Separate .github/copilot-migration-instructions.md. Skins & AJAX Toolkit OUT. ListView EditItemTemplate bug (#406) IN.

**WingtipToys initial analysis:** 15+ pages, 22 controls, 100% BWFC coverage (all 4 "missing" controls already existed). Only blocking gap was FormView RenderOuterTable (resolved). Architecture: LayoutComponentBase, @page directives, EF Core + IDbContextFactory, scoped DI services, scaffolded Identity UI, HttpClient for PayPal, InteractiveServer. 36 work items, 5 phases.

**ASPX/ASCX tooling strategy:** 85+ syntax patterns from 33 WingtipToys files. Three-layer pipeline: Layer 1 (bwfc-migrate.ps1, ~40%, mechanical regex), Layer 2 (Copilot skill, ~45%, structural), Layer 3 (Copilot agent, ~15%, semantic). NOT building: standalone CLI, VS Code extension, Roslyn analyzer. SelectMethod->Items = #1 structural transform. Session->scoped DI = hardest semantic transform.

**ModelErrorMessage spec:** BaseStyledComponent (not BaseValidator). CascadingParameter EditContext. ModelStateKey->Field(key). Renders <span>, nothing when no error. Strips \x1F metadata. Validations/ folder. 29/29 WingtipToys coverage.

**WingtipToys pipeline validation (post-script):** Layer 1 ~70% markup: 147+ tag removals, 165+ runat removals, 35+ expression conversions. 18 data-binding expressions unconverted (<%#: inside GetRouteUrl/String.Format/Eval). 3 user-control prefixes survive. FormView RenderOuterTable working. ModelErrorMessage in 2 files. Actionable: (1) add <%#: regex, (2) strip uc:/friendlyUrls: prefixes, (3) SelectMethod->Items as #1 Layer 2 example, (4) Identity scaffold guidance. **18-26 hours total, 4-6 hours for demo subset.**

Team updates (2026-03-02): Unified release (PR #408), project reframed as migration acceleration system (Jeff), ModelErrorMessage docs (52 components, Beast), WingtipToys pipeline validated (4 ready, 21 skill, 8 architecture).

<!-- Summarized 2026-03-03 by Scribe -- covers CSS fidelity through WingtipToys schedule -->

### CSS Fidelity & WingtipToys Schedule Summary (2026-03-02 through 2026-03-03)

**WingtipToys CSS fidelity audit:** 7 visual differences — wrong Bootstrap theme (Cerulean), single-column grid, missing Trucks category, Site.css not loaded, BoundField DataFormatString bug, bootstrap-theme gradients.

**M22 planning:** 12 work items, 4 waves. 57 controls ready. Skins & AJAX Toolkit OUT. ListView #406 IN.

**WingtipToys migration:** 15+ pages, 22 controls, 100% BWFC coverage. Architecture: LayoutComponentBase, EF Core, scoped DI, scaffolded Identity, InteractiveServer. 26 work items, 7 phases, critical path 1→2→3→4→7.

**ASPX/ASCX tooling:** Three-layer pipeline validated at ~70% markup. SelectMethod→Items = #1 structural transform.

**ModelErrorMessage:** BaseStyledComponent, CascadingParameter EditContext, 29/29 WingtipToys coverage.
<!-- Summarized 2026-03-04 by Scribe  covers migration toolkit design through restructure -->

### Migration Toolkit Design & Restructure Summary (2026-03-03)

**Toolkit design:** 9-document package at /migration-toolkit/. References existing scripts/skills by relative path  no duplication. Highest-value: copilot-instructions-template.md (drop-in for external projects). CHECKLIST.md fully net-new. Design doc: planning-docs/MIGRATION-TOOLKIT-DESIGN.md.

**Toolkit restructure:** Per Jeff's directive, moved 3 distributable skills from .github/skills/ to migration-toolkit/skills/. Copied bwfc-scan.ps1 and bwfc-migrate.ps1 into migration-toolkit/scripts/. 5 internal skills remain in .github/skills/. Key: distributable assets in migration-toolkit/, internal skills in .github/skills/.

**Key team updates (2026-03-02-03):** Unified release (PR #408), project reframed as migration system (Jeff), ModelErrorMessage docs (Beast), themes last directive (Jeff Fritz), migration toolkit pivoted to single SKILL.md then restructured.

 Team update (2026-03-04): PRs must target upstream FritzAndFriends/BlazorWebFormsComponents, not the fork  decided by Jeffrey T. Fritz
 Team update (2026-03-04): Migration test reports go in docs/migration-tests/{subfolder}/  decided by Jeffrey T. Fritz
 Team update (2026-03-04): Layer 1 benchmark baseline established  scan 0.9s, migrate 2.4s, 338 build errors (code-behind only)  decided by Cyclops
 Team update (2026-03-04): Layer 2+3 benchmark complete  ~9.4 min with Copilot, clean build, migration skills validated  decided by Cyclops

<!-- Summarized 2026-03-05 by Scribe -- covers Run 4 review through Run 5 analysis -->

### Run 4-5 Review & BWFC Capabilities Analysis (2026-03-04 through 2026-03-05)

**Run 4 review:** ConvertFrom-MasterPage = highest-impact enhancement (auto-generates MainLayout.razor). 289 transforms (+12 from new regexes/master page). 0 errors, 0 warnings, 11/11 features. CascadingAuthenticationState needed in Routes.razor for AuthorizeView.

**Run 5 BWFC analysis:** 95+ EventCallbacks across 30+ components matching Web Forms names. 3 of 4 top manual rewrites unnecessary -- BWFC already had ListView, FormView, GridView. 40% estimated reduction if scripts preserve BWFC data controls. Gaps: Repeater has zero EventCallbacks, GridView missing OnRowDataBound/OnRowCreated. SelectMethod TODOs need `Items=@data` guidance. Deliverables: analysis-and-recommendations.md, migration-standards SKILL.md, forge-run5-standards decision.

<!-- Summarized 2026-03-04 by Scribe — covers Run 6 analysis and benchmark execution -->

### Run 5→6 Analysis & Run 6 Benchmark (2026-03-04 through 2026-03-05)

**Run 5→6 analysis:** 8 enhancements identified from Run 5 manual-fixes.md. Top 4 implemented: TFM net10.0, SelectMethod BWFC-aware TODO (-120s), wwwroot static file copy, compilable stubs. Remaining 4 deferred: Page.Title, base class swap, BundleConfig, event handler annotations. Repeater has zero EventCallbacks; GridView missing OnRowDataBound/OnRowCreated.

**Run 6 benchmark:** 32 Web Forms files → clean Blazor build in ~4.5 min (55% reduction from Run 5). Layer 1: 4.58s, 269 transforms, 79 static files, 6 auto-stubs. Layer 2: ~3m 25s manual (models, DbContext, services, layout). 4 builds to clean. All 4 enhancements validated. Bugs: @rendermode InteractiveServer invalid in _Imports.razor (line 164), Test-UnconvertiblePage misses code-behind (.aspx.cs). Report: docs/migration-tests/wingtiptoys-run6-2026-03-04/raw-data.md.

Team updates: GetRouteUrl overloads (Cyclops), migration standards formalized (Jeff/Forge), migration report 3-level traversal (Beast).

 Team update (2026-03-04): Run 6 benchmark decisions merged  @rendermode removal, code-behind scanning, pattern validation. All decisions propagated to Cyclops and Beast.  decided by Forge


 Team update (2026-03-04): @rendermode InteractiveServer belongs in App.razor, not _Imports.razor  consolidated from Forge, Cyclops, Jeffrey T. Fritz (PR #419)


 Team update (2026-03-04): EF Core must use 10.0.3 (latest .NET 10)  directed by Jeff

### Page Base Class Architecture Analysis (2026-03-05)

**Jeff's question:** "What if converted ASPX pages inherited from a BWFC base class? Can we dramatically improve migration?"

**Analysis:** Reviewed System.Web.UI.Page surface area (Title, IsPostBack, MetaDescription, MetaKeywords, Request, Response, Session, IsValid, etc.) against current BWFC architecture (PageService as scoped DI, Page.razor render component, BaseWebFormsComponent hierarchy, WebFormsPage wrapper). Evaluated 3 options: (A) add to BaseWebFormsComponent (rejected — pollutes controls), (B) new WebFormsPageBase : ComponentBase (clean but no Page.Title syntax), (C) Option B + `Page => this` self-reference (enables literal `Page.Title = "X"` syntax).

**Recommendation:** Option C — `WebFormsPageBase : ComponentBase` with `Title`, `MetaDescription`, `MetaKeywords` delegating to IPageService, `IsPostBack => false`, and `protected WebFormsPageBase Page => this;`. Converted pages use `@inherits WebFormsPageBase` (one line in _Imports.razor). Eliminates per-page `@inject IPageService Page`, makes `Page.Title = "X"` and `if (!IsPostBack)` compile unchanged. Deliberately omits Request/Response/Session to force proper Blazor migration.

**Impact:** For WingtipToys (27 pages): eliminates 27 @inject lines, 12+ IsPostBack manual fixes, ~15-25 minutes of manual work. The two most common Web Forms code-behind patterns survive migration with zero changes. Verdict: dramatic improvement.

**Key risks:** (1) `Page` property shadows `Page.razor` in @code — acceptable, already the pattern in samples. (2) `if (IsPostBack)` without `!` becomes dead code — scripts must flag it. (3) Doesn't inherit BaseWebFormsComponent — pages don't need CascadingValue wrapping, FindControl, or ViewState.

**Decision document:** .ai-team/decisions/inbox/forge-page-base-class.md — pending Jeff's approval.

 Team update (2026-03-04): WebFormsPageBase implemented  decided by Forge, approved by Jeff

### Page Consolidation Analysis (2026-03-05)

**Jeff's question:** "Can we consolidate NamingContainer, ThemeProvider, and WebFormsPageBase so we have 1 entry point that delivers all features?"

**Analysis:** Evaluated 4 options for reducing the 5-piece setup (WebFormsPageBase, Page.razor, WebFormsPage, NamingContainer, ThemeProvider). Option A (base class renders head) rejected — base classes can't emit `<PageTitle>`/`<HeadContent>`, breaks SSR. Option C (JSInterop) rejected — doesn't work during SSR, breaks crawlers. Option D (accept status quo) is fallback.

**Recommendation:** Option B — Merge `Page.razor` functionality into `WebFormsPage`. `<PageTitle>` and `<HeadContent>` work from anywhere in the render tree, so `WebFormsPage` can render them alongside its existing `<CascadingValue>` wrapper. Add `bool RenderPageHead` parameter (default true) for opt-out. Keep `Page.razor`, `NamingContainer`, and `ThemeProvider` as standalone components — they have independent consumers and tests.

**Key findings:** (1) Cannot get below 2 setup points — `@inherits` is compile-time, `<WebFormsPage>` is render-time, fundamentally different mechanisms. (2) WebFormsPageBase must NOT inherit NamingContainer — it would inject 5+ unnecessary services, add reflection overhead, create double naming scopes, and break existing tests. (3) Merging into WebFormsPage is non-breaking: existing `Page.razor` consumers unaffected, existing `WebFormsPage` consumers gain head rendering automatically.

**Resulting DX:** `@inherits WebFormsPageBase` in _Imports.razor + `<WebFormsPage>@Body</WebFormsPage>` in layout. Two one-liners, total. Decision doc: `.ai-team/decisions/inbox/forge-page-consolidation.md`.

**Learnings:**
- `<PageTitle>` and `<HeadContent>` are Blazor built-ins that render into `<head>` regardless of where they appear in the component tree — they don't need DOM proximity to `<head>`.
- Base classes (`ComponentBase` subclasses) cannot inject render components into the inheriting page's markup — they only participate in lifecycle, not BuildRenderTree.
- The theoretical minimum setup for a library that provides both compile-time API (base class properties) and render-time services (cascading values, head injection) is two entry points: an `@inherits` directive and a wrapper component.
- When merging render-time concerns into a single component, optional service resolution (`ServiceProvider.GetService`) is the right pattern to avoid hard dependencies on registrations the consumer may not need.

� Team update (2026-03-05): WebFormsPage now includes IPageService head rendering (title + meta tags), merging Page.razor capability per Option B consolidation. Layout simplified to single <WebFormsPage> component. Page.razor remains standalone.  decided by Forge, implemented by Cyclops

### Event Handler Migration Audit (2026-03-05)

**Jeff's question:** "Investigate event handler migration — OnClick, OnSelectedIndexChanged — we should have event handlers on all BWFC components."

**Key findings:**

1. **Naming convention split is the #1 migration blocker.** ~50 EventCallbacks across GridView, DetailsView, FormView, ListView, DataGrid, Menu, TreeView use bare names (e.g., `Sorting`, `SelectedIndexChanged`, `ModeChanging`) instead of `On`-prefixed names matching Web Forms attributes. When `bwfc-migrate.ps1` strips `asp:` and leaves `OnSorting="Handler"`, these won't compile against the BWFC components.

2. **Button controls, input controls, list controls, login controls — all good.** These consistently use `On`-prefix naming and match Web Forms attributes exactly. ButtonBaseComponent (OnClick, OnCommand), TextBox (OnTextChanged), CheckBox (OnCheckedChanged), DropDownList (OnSelectedIndexChanged), all login controls — zero friction.

3. **Repeater has zero EventCallbacks.** DataList is missing 7 of 8 Web Forms events. Both are migration blockers for apps that use OnItemCommand or OnItemDataBound on these controls.

4. **GridView still missing OnRowDataBound, OnRowCreated** (flagged in Run 5, still not implemented). Also missing OnPageIndexChanging, OnRowUpdated, OnRowDeleted.

5. **FormView has inconsistent naming** — OnItemDeleting/OnItemDeleted/OnItemInserting/etc. use `On`-prefix, but ModeChanging/ModeChanged/ItemCommand/ItemCreated/PageIndexChanging/PageIndexChanged do NOT. This is confusing and will cause partial migration failures.

6. **Migration script does zero event handler transformation** — it passes attributes through unchanged after stripping `asp:`. This is correct behavior IF components use `On`-prefix naming. The fix belongs in the components, not the script.

7. **CustomValidator missing OnServerValidate** — the one validation control that had a meaningful server event doesn't have it in BWFC.

**Recommendation:** Add `On`-prefix aliases to all 50 mismatched EventCallbacks (non-breaking). Implement missing events on Repeater, DataList, GridView. Decision doc: `.ai-team/decisions/inbox/forge-event-handler-audit.md`.

 Team update (2026-03-05): Event handler audit complete — 50 naming mismatches, 18 missing events, Repeater/DataList critically underserved. Option A (On-prefix aliases) recommended.  decided by Forge


 Team update (2026-03-05): 50 On-prefix EventCallback aliases added to data components + migration script AutoPostBack fix  by Cyclops, Rogue

### ShoppingCart GridView Feature-Gap Analysis (2026-03-05)

**Jeff's question:** "I lost my editable cart with row stripes. What was lost in the migration? Does BWFC GridView have gaps?"

**Key findings:**

1. **AfterWingtipToys/ShoppingCart.razor is a plain HTML table** — it does NOT use the BWFC GridView. This is exactly the anti-pattern documented in migration-standards: "Replacing BWFC Data Controls with Raw HTML."

2. **Lost features in AfterWingtipToys:** Editable TextBox for quantity (now read-only), CheckBox for item removal (gone), Update button (gone), Checkout ImageButton (gone), CssClass stripes+borders (degraded to just `table`), ShowFooter, GridLines, CellPadding, BoundField columns, ProductID column, Label components for totals. The cart is **read-only** — users cannot edit or check out.

3. **BWFC GridView supports ALL needed features:** CssClass ✅ (via BaseStyledComponent), AutoGenerateColumns="False" with Columns ✅, BoundField (with dotted DataField, DataFormatString) ✅, TemplateField with ItemTemplate (RenderFragment<T> with `context`) ✅, TextBox inside TemplateField ✅, CheckBox inside TemplateField ✅, ShowFooter ✅, GridLines ✅, CellPadding ✅. Zero component gaps for this page.

4. **FreshWingtipToys proves it works** — correctly uses GridView with all original features preserved. This is the reference implementation.

5. **Root cause:** The migration pipeline (Layer 1 script or Layer 2 Copilot) decomposed the GridView into raw HTML instead of preserving it as a BWFC component. Script fix needed: preserve `<asp:GridView>` structure, strip `asp:` prefix only, convert ItemType→TItem, SelectMethod→Items.

**Decision document:** `.ai-team/decisions/inbox/forge-gridview-gap.md`

 Team update (2026-03-05): ShoppingCart GridView gap analysis complete — AfterWingtipToys is a broken plain-HTML table, BWFC GridView has zero gaps for this page, migration scripts need GridView preservation rules.  decided by Forge

 Team update (2026-03-05): AfterWingtipToys must only be produced by migration toolkit output, never hand-edited  decided by Jeffrey T. Fritz
