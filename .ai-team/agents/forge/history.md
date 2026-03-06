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

<!-- ⚠ Summarized 2026-03-06 by Scribe — older entries archived -->

### Archived Sessions

- M17-M18 Audit & Themes Roadmap Summary (2026-02-28 through 2026-03-01)
- Build/Release & M22 Migration Summary (2026-03-02)
- CSS Fidelity & WingtipToys Schedule Summary (2026-03-02 through 2026-03-03)
- Migration Toolkit Design & Restructure Summary (2026-03-03)
- Run 4-5 Review & BWFC Capabilities Analysis (2026-03-04 through 2026-03-05)

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


 Team update (2026-03-06): CRITICAL  Git workflow: feature branches from dev, PRs target dev. NEVER push to or merge into upstream main (production releases only).  directed by Jeff Fritz

### Full Library Audit (2026-03-06)

**Scope:** Comprehensive audit of all files in `src/BlazorWebFormsComponents/`.

**Key Findings:**
- Library ships **153 Razor components** and **197 standalone C# classes** (enums, event args, base classes, interfaces, utilities)
- CONTROL-COVERAGE.md documented 58 primary controls — count was accurate for top-level Web Forms equivalents, but omitted 95 supporting Razor components
- **ContentPlaceHolder** was listed as "Not Supported" in CONTROL-COVERAGE.md — WRONG, a working component exists alongside Content.razor and MasterPage.razor
- **4 field column components** (BoundField, ButtonField, HyperLinkField, TemplateField) shipped but undocumented as separate entries
- **7 infrastructure components** (Content, ContentPlaceHolder, MasterPage, WebFormsPage, Page, NamingContainer, EmptyLayout) shipped but undocumented
- **63 style sub-components** + 3 PagerSettings shipped but undocumented
- **3 helper components** (BlazorWebFormsHead, BlazorWebFormsScripts, Eval) shipped but undocumented
- Theming system (ThemeProvider + ThemeConfiguration + ControlSkin + SkinBuilder) undocumented
- 54 enums, 22 interfaces, 3 extensions, 4 data binding classes, 3 custom control shims all present
- All 58 listed primary controls verified present in code — no phantom entries

**Changes Made:**
1. Updated `migration-toolkit/CONTROL-COVERAGE.md`:
   - Fixed component count from "58" to "58 primary + 95 supporting (153 total)"
   - Removed ContentPlaceHolder from "Not Supported" — added to new Infrastructure section
   - Added Infrastructure Controls section (7 components)
   - Added Field Column Components section (4 components)
   - Added Style Sub-Components section (66 components)
   - Added Utilities & Infrastructure section (base classes, services, shims, enums, helpers)
   - Updated visual summary
2. Created `dev-docs/bwfc-audit-2026-03-06.md` — full audit report with complete catalog
3. Created `.ai-team/decisions/inbox/forge-bwfc-audit.md` — decision document for team

� Team update (2026-03-06): LoginView is a native BWFC component  do NOT replace with AuthorizeView in migration guidance. Both migration-standards SKILL.md files (in .ai-team/skills/ and migration-toolkit/skills/) must be kept in sync. WebFormsPageBase patterns corrected in all supporting docs.  decided by Beast

 Team update (2026-03-06): Migration reports should lead with executive content (timing, screenshots, before/after code), technical details below the fold. Pattern established in Run 8 report.  decided by Beast

 Team update (2026-03-06): Only document top-level components and utility features for promotion. Do not promote/document style sub-components, internal infrastructure, or implementation-detail classes.  decided by Jeffrey T. Fritz

 Team update (2026-03-06): LoginView must be preserved as BWFC component, not converted to AuthorizeView  decided by Jeff (directive)

### Run 8 Post-Mortem & Run 9 Preparation (2026-03-06)

**Analysis:** Comprehensive post-mortem of Run 8 (14/14 acceptance tests, 1h 55m total). Compared all original WingtipToys Web Forms files against migrated Blazor output. Read full report, migration script, all 4 skills, and every key source/output file pair.

**Key patterns found:**
- HTTP Session + Interactive Server is the #1 architectural blocker — HttpContext is null during WebSocket circuits, so cookie auth and session-based operations must use minimal API endpoints with HTML form POSTs
- Blazor enhanced navigation intercepts `<a>` clicks — links to server endpoints need `onclick` workaround or `data-enhance-nav="false"`
- Models directory contains reusable entities that can be auto-copied with minimal transforms (EF6→EF Core namespace swap)
- Redirect-only pages (like AddToCart.aspx) should generate minimal API endpoint TODOs, not dead stubs
- Page Title extracted from `<%@ Page %>` directive is lost — should be preserved
- `[QueryString]`/`[RouteData]` attributes need `[SupplyParameterFromQuery]` annotation hints

**Priority fixes identified (22 total):**
- P0 (3): Session/auth warning in skills, minimal API endpoint templates, auto-copy Models
- P1 (11): DbContext transform, LoginView preservation, .csproj packages, Program.cs boilerplate, redirect page detection, enhanced nav warning, Page.Title extraction, GetRouteUrl hints, QueryString/RouteData annotations, ListView GroupItemCount guidance, DisableAntiforgery docs
- P2 (8): Skip .designer.cs, WebFormsPageBase in _Imports, CSS bundle handling, coverage doc gotchas, session service pattern, Logic/ detection, webopt removal, component count update

**Estimated impact:** P0+P1 implementation could reduce Run 9 from ~1h 55m to ~50-60 min.

**Decision document:** `.ai-team/decisions/inbox/forge-run9-analysis.md`

