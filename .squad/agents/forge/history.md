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

<!-- ⚠ Summarized 2026-03-12 by Scribe — Run 18 analysis and L2 automation analysis archived -->

- Run 18 Analysis & Improvement Recommendations (2026-03-11)
- L2 Automation Analysis (2025-07-25)

### Summary (2026-03-11 through 2025-07-25)

Run 18 analysis: Test-UnconvertiblePage architecturally flawed (P0), [Parameter] RouteData annotation line-swallowing bug (P0), BWFC generic type param naming inconsistent (P2), no Layer 2 automation script exists, Session pattern checks markup not code-behind. L2 automation: 6 OPPs identified from Runs 17–21 (enum/bool/unit string normalization is #1 time sink). OPP-1 EnumParameter<T> (P0/M), OPP-2 Unit implicit string (P0/S), OPP-3 Response.Redirect shim (P1/S), OPP-4 Session (deferred), OPP-5 ViewState (P2/S), OPP-6 GetRouteUrl (P2/S). Unit.cs broken explicit operator fixed. BaseWebFormsComponent.ViewState exists but WebFormsPageBase didn’t expose it.

### Render Mode Guard Analysis (2026-03-12)

- **Blazor render mode detection APIs (available on net10.0 / .NET 9+):** `ComponentBase.RendererInfo` has `.Name` (`"Static"`, `"Server"`, `"WebAssembly"`, `"WebView"`) and `.IsInteractive` (bool). `ComponentBase.AssignedRenderMode` returns null for SSR or the specific `IComponentRenderMode`. These are available on WebFormsPageBase because it inherits ComponentBase.
- **Critical nuance:** `RendererInfo.IsInteractive` is NOT the right guard for HttpContext availability. During InteractiveServer prerender, `IsInteractive == false` but HttpContext IS present. After circuit activation, `IsInteractive == true` but HttpContext is null. The ground truth is `_httpContextAccessor.HttpContext != null`. RendererInfo is for diagnostic error messages only.
- **Recommended pattern:** `RequireHttpContext([CallerMemberName])` helper on WebFormsPageBase that throws `InvalidOperationException` with render mode details. `IsHttpContextAvailable` bool property as escape hatch. Each shim also guards its own HttpContext-dependent members.
- **Throw, not degrade:** GetRouteUrl, Response.Cookies, Request.Cookies, Session must throw. Response.Redirect (NavigationManager) and Request.QueryString/Url (NavigationManager fallback) can degrade gracefully. ViewState is unaffected.
- **Don't cache HttpContext:** Always access live via `_httpContextAccessor.HttpContext`. HttpContext exists during prerender, then disappears when the WebSocket circuit starts.
- **Key files:** `src/BlazorWebFormsComponents/WebFormsPageBase.cs`, `src/BlazorWebFormsComponents/ResponseShim.cs`, `src/BlazorWebFormsComponents/Extensions/GetRouteUrlHelper.cs` (also vulnerable, P2 follow-up)
- **Full decision written to:** `.ai-team/decisions/inbox/forge-render-mode-guards.md`


 Team update (2026-03-12): L2 automation consolidated  EnumParameter<T> (OPP-1) + WebFormsPageBase shims (OPP-2,3,5,6) all implemented. Rogue: 4 test files need .Value.ShouldBe() fix. Beast: L2 scripts can emit bare enum strings.  decided by Forge (analysis), Cyclops (implementation)

 Team update (2026-03-12): Jeff approved Pattern B+ (Honest No-Op with Runtime Diagnostics) for cookie graceful degradation. Cookies must NOT throw  overrides render-mode-guards 4 for cookie members.  decided by Jeffrey T. Fritz
 Team update (2026-03-12): PageTitle dedup analysis delivered and accepted. Page.Title is single source of truth. Implementation assigned to Cyclops (P0: Default.razor fix, P1: pipeline fix).  decided by Forge, approved by Jeffrey T. Fritz


### EDMXEF Core Architecture Analysis (2026-03-13)

**Question:** Should EDMXEF Core conversion be an L1 script enhancement or a BWFC library adapter/code generator?

**Context:** Run 21 ContosoUniversity migration (35/40 tests, 87.5%) revealed EDMX metadata is completely lost during migration:
- Primary keys: `Cours` entity with `CourseID` property doesn't match EF Core convention  caused 500 errors until manual `[Key]` fix
- 4 cascade delete rules missing from generated DbContext
- FK relationships with multiplicity missing (no `HasOne`/`HasMany` config)
- Column constraints (MaxLength, Required) missing
- Identity columns missing `ValueGeneratedOnAdd`
- Table name mappings missing (entity `Cours`  table `Courses`)

The current migration-toolkit is 100% EDMX-blind. Zero references to EDMX in scripts or skills.

**Recommendation:** **Option 1  L1 Script Enhancement (PowerShell XML Parser)**

**Why:**
1. **Single point of execution:** Migration is a one-time operation. Customers run the script once  get complete DbContext  done. Option 2 (separate tool) adds friction: discover tool  install  run  fix conflicts.
2. **Migration-time concern:** EDMX doesn't exist in EF Core. It's a legacy artifact that must be converted to C# code at migration time, not build-time or runtime.
3. **L1 already transforms models:** Lines 1832-1895 of bwfc-migrate.ps1 already copy Models/*.cs, detect *Context.cs, transform EF6EF Core usings, inject EF Core constructor. Extending it to parse EDMX is natural expansion, not new paradigm. Option 2 creates two competing pipelines.
4. **PowerShell XML parsing is trivial:** EDMX is well-defined XML (SSDL, CSDL, C-S Mapping). PowerShell has native XML support. ~200-300 lines of PowerShell vs full .NET tool with Roslyn/MSBuild/NuGet/versioning. Complexity ratio is 10:1 in favor of L1.
5. **EDMX is rare and static:** Target audience is niche within niche (migrating from Web Forms AND using EF6 EDMX). Option 2 adds permanent maintenance burden for temporary legacy problem. Option 1 is one-time implementation (EDMX frozen at EF6).
6. **Precedent:** L1 script already parses Web.config XML, generates .csproj with conditional packages, generates Program.cs with Identity/Session boilerplate, scans wwwroot for CSS, parses Site.Master for CDN refs. It's already a sophisticated migration engine.

**Technical approach:**
- New function `Convert-EdmxToEfCore` in bwfc-migrate.ps1 (after line 1831)
- Parse SSDL (storage schema), CSDL (conceptual model), C-S Mapping (entitytable)
- Generate entity classes with `[Key]`, `[MaxLength]`, `[Required]`, `[DatabaseGenerated]` from EDMX metadata
- Generate DbContext with EF Core constructor + `OnModelCreating()` with FK relationships, cascade deletes, table mappings
- Skip copying .edmx, .edmx.diagram, .tt, .Designer.cs files (useless in EF Core)

**Example:** Entity `Cours` with `CourseID` property  generate `[Key]` attribute (EF Core expects `CoursId`). Cascade delete `<OnDelete Action="Cascade" />`  `OnDelete(DeleteBehavior.Cascade)`.

**Success criteria:**
- Run 22 (ContosoUniversity): 40/40 tests pass (up from 35/40)
- Zero manual fixes for keys, FK relationships, cascade deletes, column constraints
- L1 script time: <2 seconds (current 0.93s; XML parsing adds <0.5s)

**Customer impact:**
- Option 1: Run script  complete DbContext  build  done
- Option 2: Run script  skeleton  install tool  run tool  fix conflicts  build  done

**Decision written to:** `.squad/decisions/inbox/forge-edmx-architecture.md`

---
