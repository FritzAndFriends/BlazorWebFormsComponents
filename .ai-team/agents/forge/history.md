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
