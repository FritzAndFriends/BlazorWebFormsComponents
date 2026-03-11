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

<!-- ⚠ Summarized 2026-03-06 by Scribe — Run 5→6 and Page Architecture entries archived -->

### Archived Sessions (cont.)

- Run 5→6 Analysis & Run 6 Benchmark (2026-03-04 through 2026-03-05)
- Page Base Class Architecture Analysis (2026-03-05)
- Page Consolidation Analysis (2026-03-05)

### Run 5→6 + Page Architecture Summary (2026-03-04 through 2026-03-05)

Run 5→6: 8 enhancements identified, top 4 implemented (TFM net10.0, SelectMethod BWFC TODO, wwwroot copy, compilable stubs). Run 6: 32 files → clean build in ~4.5 min (55% reduction). Bugs found: @rendermode in _Imports invalid, Test-UnconvertiblePage misses .aspx.cs. EF Core 10.0.3 mandated. @rendermode belongs in App.razor only.

WebFormsPageBase: Option C chosen — `WebFormsPageBase : ComponentBase` with `Page => this` self-reference, Title/MetaDescription/MetaKeywords delegating to IPageService, `IsPostBack => false`. Eliminates 27 @inject lines, 12+ manual fixes for WingtipToys. Deliberately omits Request/Response/Session.

Page Consolidation: Option B — merged Page.razor head rendering into WebFormsPage. `<PageTitle>`/`<HeadContent>` work anywhere in render tree. Min setup: `@inherits WebFormsPageBase` + `<WebFormsPage>@Body</WebFormsPage>`. WebFormsPageBase must NOT inherit NamingContainer (breaks tests, adds overhead). Page.razor remains standalone.

� Team update (2026-03-05): WebFormsPage now includes IPageService head rendering (title + meta tags), merging Page.razor capability per Option B consolidation. Layout simplified to single <WebFormsPage> component. Page.razor remains standalone.  decided by Forge, implemented by Cyclops


 Team update (2026-03-06): CRITICAL  Git workflow: feature branches from dev, PRs target dev. NEVER push to or merge into upstream main (production releases only).  directed by Jeff Fritz

<!-- ⚠ Summarized 2026-03-07 by Scribe — entries from 2026-03-06 archived -->

- Full Library Audit (2026-03-06)
- Run 8 Post-Mortem & Run 9 Preparation (2026-03-06)
- Run 9 CSS/Image Failure RCA (2026-03-06)
- Fix 1a + Fix 1b Implementation — Run 9 RCA Remediation (2026-03-06)

### Summary (2026-03-06)

Library audit: 153 Razor components + 197 C# classes (CONTROL-COVERAGE.md was listing 58 — corrected). ContentPlaceHolder reclassified from "Not Supported" to Infrastructure. Run 8 post-mortem: 22 fixes identified (3 P0, 11 P1, 8 P2); HTTP Session + Interactive Server is #1 blocker (HttpContext null during WebSocket). Run 9 CSS/image RCA: 3 root causes — (1) script doesn't extract `<webopt:bundlereference>`, (2) Layer 2 rewrote image paths without moving files, (3) tests don't verify visual output. Fix 1a: `<webopt:bundlereference>` extraction + CDN link preservation in ConvertFrom-MasterPage. Fix 1b: new `Invoke-CssAutoDetection` function scans wwwroot/Content/ for .css files and injects `<link>` tags into App.razor.


 Team update (2026-03-07): Coordinator must not perform domain work  all code changes must route through specialist agents  decided by Jeffrey T. Fritz, Beast
 Team update (2026-03-07): Run 11 script fixes: Invoke-ScriptAutoDetection and Convert-TemplatePlaceholders added to bwfc-migrate.ps1  decided by Cyclops
 Team update (2026-03-07): migration-standards SKILL.md updated with 3 new sections for Run 11 gaps  decided by Beast
 Team update (2026-03-07): Migration order directive  fresh Blazor project first, then apply BWFC  decided by Jeffrey T. Fritz

 Team update (2026-03-08): Default to SSR (Static Server Rendering) with per-component InteractiveServer opt-in; eliminates HttpContext/cookie/session problems  decided by Forge

 Team update (2026-03-08): Run 12 migration patterns: auth via plain HTML forms with data-enhance=false, dual DbContext, LoginView _userName from cascading auth state  decided by Cyclops

 Team update (2026-03-08): Enhanced navigation must be bypassed for minimal API endpoints  `data-enhance-nav="false"` required (consolidated decision)  decided by Cyclops
 Team update (2026-03-08): DbContext registration simplified  `AddDbContextFactory` only, no dual registration (supersedes Run 12 dual pattern)  decided by Cyclops
 Team update (2026-03-08): Middleware order: UseAuthentication  UseAuthorization  UseAntiforgery  decided by Cyclops
 Team update (2026-03-08): Logout must use `<a>` link not `<button>` in navbar  decided by Cyclops


 Team update (2026-03-11): `AddBlazorWebFormsComponents()` now auto-registers HttpContextAccessor, adds options pattern + `UseBlazorWebFormsComponents()` middleware with .aspx URL rewriting. All sample Program.cs files updated.  decided by Cyclops
 Team update (2026-03-11): Migration tests reorganized  `dev-docs/migration-tests/` now uses `wingtiptoys/runNN/` and `contosouniversity/runNN/` structure.  decided by Beast
 Team update (2026-03-11): Executive summary created at `dev-docs/migration-tests/EXECUTIVE-SUMMARY.md`  35 runs, 65 tests, performance data.  decided by Beast

### Run 18 Analysis & Improvement Recommendations (2026-03-11)

**Key findings from Run 18 report analysis:**

1. **`Test-UnconvertiblePage` is architecturally flawed** — it matches patterns against markup only, causing false positives on UI references (PayPal image URLs, Checkout button IDs). Needs two-pass architecture: check code-behind for auth/session/payment patterns, markup only for structural features. This is the #1 script reliability issue. (P0)

2. **`[Parameter]` RouteData annotation bug is a line-swallowing regex issue** — line 1209 of bwfc-migrate.ps1 replaces `[RouteData]` with a `[Parameter] // TODO...` string that consumes the rest of the line (parameter type + name). Causes 6 build errors in every project with route parameters. Fix: use line-aware regex that preserves same-line content and puts TODO on next line. (P0)

3. **BWFC generic type parameter naming is inconsistent** — GridView/DataGrid/ListView use `ItemType`, BulletedList/DropDownList use `TItem`, DataBoundComponent uses `TItemType`. The migration script at line 1132 converts `ItemType` → `TItem` which is WRONG for GridView (whose generic param IS named `ItemType`). Major version standardization needed. (P2 — works today, just confusing)

4. **Layer 2 doesn't exist as automation** — No `bwfc-migrate-layer2.ps1` file. All Layer 2 work (boolean normalization, enum conversion, DI patterns, auth rewiring) is manual. ShoppingCart.razor required 6 manual fixes that are generalizable patterns.

5. **`Session\[` pattern checks markup, not code-behind** — Web Forms `Session["key"]` appears in `.aspx.cs` files, not in markup. The current check against markup content misses actual session usage and could false-positive on inline code blocks.

**Decisions made:**
- Recommended two-pass `Test-UnconvertiblePage` with code-behind analysis + severity scoring (P0-1)
- Recommended immediate fix for `[Parameter]` line-swallowing bug (P0-2)
- Boolean normalization should be Layer 1 (P1-1), not library-level — Blazor's `bool.Parse` is already case-insensitive
- Enum attribute conversion map needed for GridLines, RepeatDirection, etc. (P1-3)
- Full recommendations written to `.ai-team/decisions/inbox/forge-run18-improvements.md`


📌 Team update (2026-03-11): Run 18 improvement recommendations prioritized by Forge — see decisions.md

