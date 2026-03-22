# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents  Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Core Context

<!-- Summarized 2026-03-04 by Scribe — originals in history-archive.md -->

M1–M16: 6 PRs reviewed, Calendar/FileUpload rejected, ImageMap/PageService approved, ASCX/Snippets shelved. M2–M3 shipped (50/53 controls, 797 tests). Chart.js for Chart. DataBoundStyledComponent<T> recommended. Key patterns: Enums/ with int values, On-prefix events, feature branches→upstream/dev, ComponentCatalog.cs. Deployment: Docker+NBGV, dual NuGet, Azure webhook. M7–M14 milestone plans. HTML audit: 3 tiers, M11–M13. M15 fidelity: 132→131 divergences, 5 fixable bugs. Data controls: 90%+ sample parity, 4 remaining bugs. M17 AJAX: 6 controls shipped.

## Learnings

### NuGet Static Asset Migration Strategy (2026-03-08)

**Strategic Analysis Complete — Hybrid Option C Recommended**

Analyzed how Web Forms apps reference static assets via NuGet packages (`packages.config` → `packages/` folder auto-extraction → `BundleConfig.cs` bundling). Discovered:

1. **DepartmentPortal Pattern (Minimal Case):** Only custom `Content/Site.css`, no external NuGet libraries. Build tool (`Microsoft.CodeDom.Providers`) has no static assets. Validates extraction logic: custom CSS copied to `wwwroot/css/`, no external packages to map.

2. **Four Migration Strategies Evaluated:**
   - **Option A (CDN):** Simple but breaks for custom packages, internet-dependent. ❌ Insufficient for enterprise.
   - **Option B (LibMan):** Good for VS integration, limited to public libs. ⚠️ Acceptable alternative.
   - **Option C (Extraction Tool):** PowerShell script reads `packages.config`, extracts `Content/` and `Scripts/` to `wwwroot/lib/`, suggests CDN for known OSS packages. ✅ **Recommended**.
   - **Option D (npm):** Modern but requires Node.js toolchain. ⚠️ Good for teams with JS expertise.

3. **Recommendation:** Implement **Option C (Hybrid)** — Extract custom/private packages, suggest CDN for known OSS (jQuery, Bootstrap, DataTables, Modernizr, SignalR, etc.), generate asset manifest + Blazor-compatible reference HTML.

4. **Automation:** New `bwfc migrate-assets` command (PowerShell script `Migrate-NugetStaticAssets.ps1`):
   - Input: `packages.config` + `/packages` folder
   - Output: `wwwroot/lib/{PackageName}/`, `asset-manifest.json`, `AssetReferences.html`
   - Strategy options: `extract` (all), `cdn` (known packages only), `hybrid` (default)

5. **BundleConfig Translation:** Don't recreate bundling. Instead, teams can:
   - Use manual `<link>` / `<script>` tags (works with HTTP/2)
   - Integrate WebOptimizer or esbuild post-migration (optional)
   - Avoid BundleConfig entirely (Blazor has no equivalent)

6. **Deliverables Created:**
   - Strategy document: `dev-docs/proposals/nuget-static-asset-migration.md` (29KB, 4 options analyzed, DepartmentPortal case study)
   - Decision document: `.squad/decisions/inbox/forge-nuget-asset-strategy.md` (awaiting team approval)
   - GitHub issue draft: Specifications complete, ready for Jeffrey T. Fritz to create upstream (personal token lacks write permissions on upstream repo, per M8 discovery)

7. **Key Insight:** DepartmentPortal validates the minimal extraction case (custom CSS only). Enterprise Web Forms apps will likely have 10–50 NuGet packages — the hybrid approach scales to that complexity while remaining simple for small apps.

8. **Timeline:** 6 weeks (extraction + toolkit integration + docs + hardening + GA)

### DepartmentPortal Migration Analysis & Upstream Issue Creation (2026-03-08)

**Key Discovery:** DepartmentPortal migration exposed 5 critical BWFC gaps that block custom control migrations:

1. **DataBoundWebControl<T> gap** — `CustomControls.DataBoundControl` exists but doesn't integrate HtmlTextWriter rendering. Controls inheriting from DataBoundControl and overriding RenderContents(HtmlTextWriter) cannot migrate. EmployeeDataGrid is canonical example. Requires new base class bridging data binding + HtmlTextWriter rendering.

2. **TagKey + AddAttributesToRender gap** — Web Forms WebControl auto-renders outer tag via TagKey property. BWFC WebControl lacks both, forcing manual tag management. StarRating (renders `<span>`) and NotificationBell (renders `<div>` with data-* attributes) cannot migrate cleanly.

3. **HTML5 enum coverage gap** — HtmlTextWriterTag/Attribute/Style enums are .NET Framework 2.0 era. Missing: semantic tags (nav, section, article, header, footer, main, figure, details, summary), ARIA attributes (role, aria-*), HTML5 form attrs (placeholder, required, autofocus, pattern, min, max, step), modern CSS (flex, grid, gap, transform, transition, animation, opacity, box-shadow, border-radius). DepartmentPortal controls use all these extensively.

4. **CompositeControl child rendering gap** — CompositeControl throws NotSupportedException for non-WebControl children. Web Forms allows LiteralControl, HtmlGenericControl, Panel, PlaceHolder, raw text. EmployeeCard is a CompositeControl with mixed children. Must support IComponent broadly.

5. **ITemplate → RenderFragment bridging gap** — Controls with template properties use ITemplate. Critical discovery: Controls inheriting from Control (not WebControl) require [ParseChildren(true)] attribute to treat inner content as property assignments, not child controls. SectionPanel has HeaderTemplate and ContentTemplate properties that map to RenderFragment in Blazor. Need documented pattern + TemplatedControl base class.

**Additional Discoveries:**
- **FindControl architectural incompatibility:** Web Forms FindControl traverses naming container boundaries (INamingContainer). BWFC's flat search fails for Master.FindControl(), SectionPanel.FindControl() with templates, Page.FindControl() with ContentPlaceHolder. Migration guidance should favor @ref, CascadingParameter, EventCallback, DI instead of FindControl entirely.
- **User-Controls documentation missing:** docs/Migration/User-Controls.md is empty. 12+ ASCX controls in DepartmentPortal need migration guide covering Register directive → _Imports.razor, code-behind → @code, data binding, FindControl → @ref, lifecycle mapping.

**Upstream Issue Creation Status:** Attempted to create 7 GitHub issues on FritzAndFriends/BlazorWebFormsComponents (P1–P5 priorities + FindControl + User-Controls docs). **BLOCKED:** Personal auth token lacks write permissions on upstream repo. Issue specifications documented in `.squad/decisions/inbox/forge-upstream-issues.md` — requires manual creation by Jeffrey T. Fritz or org maintainer.

**Learnings:**
- DepartmentPortal is a "canary" migration that exposes real gaps in BWFC. Small app (12 ASCX, 4 custom controls, 3 data-bound grids) reveals architectural patterns missing from the base library.
- P1–P2 are critical blockers (DataBoundWebControl<T>, TagKey) that should ship before next milestone to unblock data-heavy migrations.
- Authentication barrier: Copilot's token doesn't have upstream write permissions. Document issue specs locally, require human (Jeffrey) to create on upstream.
- Migration pattern discovery: ITemplate → RenderFragment + [ParseChildren(true)] is non-obvious but critical for templated controls.

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

### Component Health Audit — March 2026

**Health Snapshot Analysis (Generated 2026-03-16):**

- **Overall Status:** 52 of 54 tracked components at 100% health (96.3% complete)
- **Components Below 100%:** 
  - BulletedList (90% - missing 2/3 events: OnClick implemented, missing events from baseline)
  - FileUpload (88% - 3/5 properties: AllowMultiple, Accept, MaxFileSize implemented)
  - RadioButton (90% - property/event parity at 100%, health penalized for missing features)
  - Substitution (90% - MethodName, SubstitutionCallback implemented, deferred for cache patterns)
  - View (75% - OnActivate/OnDeactivate implemented, missing docs/sample)
  - Content (75% - ContentPlaceHolderID implemented, missing docs)
  - ContentPlaceHolder (75% - functional but missing docs)
  - SiteMapPath (85% - 8 properties implemented, missing events)
  - TreeView (88.3% - 11/18 properties, 12 events vs 5 expected)
  - CustomValidator (85% - 13 properties, missing events)
  - ScriptManager (70% - stub implementation only)
  - Xml (15% - deferred indefinitely, XSLT rarely used)

**Test Coverage:** Comprehensive across all categories. 797+ tests across 70+ test folders. Strong coverage on data controls (DataList: 53 tests, GridView: 24 tests, TreeView: 24 tests), login controls (36 tests), validations (43 tests). All major components have tests except Xml (deferred).

**Documentation Status:** 136 markdown files across 6 categories. All completed components documented except infrastructure components (Content, ContentPlaceHolder) which are functional but lack migration guides. ACT extenders have full docs (28 files in AjaxToolkit/). Migration guides comprehensive.

**ACT Extender Status:** 12 of ~40 ACT extenders implemented (30% coverage). Core set complete: AutoComplete, ConfirmButton, FilteredTextBox, Modal, Slider, MaskedEdit, NumericUpDown, Calendar, Collapsible, HoverMenu, Popup, Toggle. High-priority missing: TextBoxWatermark, CascadingDropDown, ResizableControl, CreditCard, DragPanel, ListSearch, ComboBox.

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

### UpdatePanel ContentTemplate Review (2026-03-14)

**Task:** Review Cyclops' UpdatePanel ContentTemplate enhancement for Web Forms compatibility and correctness.

**Verdict:** ✅ **APPROVE** — Production-ready. All 8 checklist items pass.

**Review Summary:**
- ✅ Web Forms fidelity — `ContentTemplate` matches `System.Web.UI.UpdatePanel` property
- ✅ HTML output — Renders as `<div>` (Block) or `<span>` (Inline), exactly matching Web Forms
- ✅ Base class change — `BaseStyledComponent` is justified enhancement; improves DX without breaking compatibility
- ✅ Backward compatibility — Existing `<ChildContent>` patterns work perfectly
- ✅ Migration story — L1 output compiles without RZ10012 warnings
- ✅ Render mode decision — Correct; library components don't force render modes
- ✅ Tests — 12 comprehensive tests, 100% pass rate
- ✅ Sample page — Reference-quality with migration guide

**Team Recognition:**
- Cyclops: Clean implementation, correct architectural decisions
- Rogue: Comprehensive test coverage (12 tests hit every scenario)
- Jubilee: Outstanding sample page — gold standard for component samples
- All agents: Perfect adherence to lockout protocol

**Recommendation:** Merge immediately. Reference-quality work.

**Decision written to:** decisions.md (consolidated with Beast's documentation update)

📌 Team update (2026-03-14): UpdatePanel ContentTemplate enhancement approved by Forge — production-ready, all 8 checklist items pass. Web Forms fidelity ✅, HTML output ✅, base class ✅ (justified enhancement), backward compatibility ✅, migration story ✅, render mode ✅, tests ✅, sample page ✅ excellent. Cyclops/Rogue/Jubilee recognized for reference-quality work. Decisions merged.

### Divergence & Feature Gap Issue Plan (2026-03-14)

**Task:** Create comprehensive GitHub issue plan from DIVERGENCE-REGISTRY + known feature gaps.

**Deliverable:** `.squad/decisions/inbox/forge-divergence-issue-plan.md` — 34 actionable issues organized by 4 milestones (M20–M23).

**Key learnings from registry analysis:**

1. **Base class inheritance fixes close 180+ gaps at once:** Adding `AccessKey` + `ToolTip` to `BaseWebFormsComponent`, changing `DataBoundComponent<T>` to inherit `BaseStyledComponent`, and fixing `Image`/`Label` base classes closes ~180 property gaps across 10+ controls. This is the highest-ROI work upfront.

2. **GridView is the critical blocker (20.7% health):** Missing paging (AllowPaging/PageSize/PageIndex), sorting (AllowSorting/SortExpression), and row editing (RowEditing/RowUpdating/RowDeleting). GridView is ~80% of data grid usage in Web Forms — fixing these 3 features moves GridView from 20.7% to 70%+ and unblocks the largest migration class.

3. **Three actionable divergence bugs (D-11, D-13, D-14):** GUID-based ID generation is a bug (should use developer-provided ID + suffixes); Calendar missing previous-month padding in 42-cell grid (visible structural gap); Calendar style properties not fully applied to table cells. These are migration fidelity issues, not intentional divergences.

4. **Validator `Display` property is layout-critical:** Missing `Display` (None/Static/Dynamic) on all 6 validators causes layout differences in migrated pages. `SetFocusOnError` is also missing but lower priority. These should be fixed together.

5. **L1 script automation at 40%, target 60%:** 6 OPPs identified: enum/bool/unit string normalization, Response.Redirect shimming, GetRouteUrl shimming, Session detection, ViewState visibility, DataSourceID warnings. Pushing to 60% unblocks Run 22 milestone.

6. **No L1 script test harness exists:** Need metrics for automation coverage %, build success, test pass rate, HTML divergence vs Web Forms. Baseline measurement enables data-driven improvements.

7. **DetailsView remains unmerged:** On `sprint3/detailsview-passwordrecovery` with 63.2% health (27 matching props, 16 matching events). Merging restores actual shipped count from 49/53 (92%) to 50/53 (94%). This is a DevOps correction, not new work.

8. **Data controls category at 53.2% — fixable to 75%+:** With base class fixes + GridView features + ListView CRUD + Menu orientation, data controls climb from 53.2% to 75%+. This is the most impactful category for migration success.

9. **Issue consolidation strategy:** Don't create 1 issue per property. Consolidate: GridView paging + sorting + editing as 3 large issues; Calendar style fixes as 2 issues (D-13 grid padding, D-14 style properties); validators as 2 issues (Display+SetFocusOnError, HeaderText+ValidationGroup). Keeps issue count manageable (~34 vs 100+).

10. **Prioritization by fidelity impact:** High priority = directly impacts migration fidelity or blocks common patterns (base class fixes, GridView, D-11/D-13/D-14). Medium priority = improves completeness but has workarounds (L1 script, ListView CRUD). Low priority = edge cases or rarely-used features (BackImageUrl, node-level TreeView styles, DataGrid deprecation notice).

**Outcome:** 34 issues planned: 13 High, 10 Medium, 11 Low. M20 (Component Parity) is foundational, fixes 180+ gaps. M21–M23 build on M20 with advanced features, CRUD, and fine-tuning. Expected project health post-all-34 issues: 68.5% → 78-80%.


📌 Team update (2026-03-14): M20 Batch 6 orchestration spawn — Forge designing component health dashboard, Cyclops advancing L1 script fixes, Rogue building L1 test harness — decided by Scribe

### Component Health Dashboard Scoring Spec (2026-03-15)

**Task:** Design scoring methodology for Component Health Dashboard (#48) — a public MkDocs page showing migration maturity per component.

**Deliverable:** `.squad/decisions/inbox/forge-health-dashboard-design.md`

**Key design decisions:**

1. **7 scoring dimensions with weights:** Property Parity (30%), HTML Fidelity (15%), bUnit Tests (15%), Integration Tests (5%), Style Support (15%), Sample Page (10%), Event Support (10%). Weights reflect migration impact — property parity is #1 because missing properties directly block markup migration.

2. **Divergence penalty model:** Infrastructure divergences (D-01 through D-04, D-07 through D-10) score 0 penalty — they're intentional platform differences. Only structural (D-13: -25), style (D-14: -15), and ID generation (D-11: -10) bugs penalize. Calendar is the most impacted component at 60% fidelity.

3. **Component inventory:** 60+ components organized into 9 categories (Data, Editor, Display, Button, Navigation, Login, Validation, Infrastructure, Charting). Infrastructure and Charting displayed separately — they're intentionally minimal.

4. **Reference property data:** Documented top 15-20 expected Web Forms properties for every primary component. This is the denominator for parity scoring. Inherited base-class properties (CssClass, Enabled, etc.) tracked via Style Support, not per-component.

5. **Test thresholds by complexity:** Complex data controls need 10 test files for 100%, medium interactive controls need 5, simple display controls need 3. This prevents a Label with 3 tests scoring the same as a GridView with 3 tests.

6. **No Playwright tests exist** — Integration test dimension is 5% placeholder. When Playwright infrastructure arrives, weight increases to 10% (stealing from bUnit).

7. **Login controls have 0 bUnit tests** — biggest gap discovered during scan. 7 components, 0 test files.

8. **Visual indicators:** 🟢 >80% (production-ready), 🟡 50-80% (usable with gaps), 🔴 <50% (migration risk).

**Learnings:**
- BaseStyledComponent inheritance chain gives most components 90%+ on style support automatically — good design payoff from early base-class investment.
- GridView estimated at ~72% health (property parity is the drag — AllowPaging/AllowSorting still missing).
- Label estimated at ~92% health — simple controls score highest because they have less to implement.
- Sample coverage is excellent (48/60+ components have samples), but 3 Login controls lack individual samples.

📌 Team update (2026-03-15): Component Health Dashboard scoring spec delivered to decisions inbox. 7 dimensions, weighted formula, reference data for 60+ components. Rogue implements as `scripts/Invoke-ComponentHealthScan.ps1` → `docs/component-health.md`. — decided by Forge

### ASCX Sample Application Milestone (2026-03-12)

**Task:** Design comprehensive milestone for a .NET Framework 4.8 sample app showcasing ASCX user controls and custom base classes (requested by Jeffrey T. Fritz).

**Deliverable:** `planning-docs/ASCX-SAMPLE-MILESTONE.md` (28KB specification) + decision to inbox

**Key design decisions:**

1. **Application concept: DepartmentPortal** — Internal HR/IT portal for employees, announcements, training, resources. Real-world enterprise domain that naturally requires reusable UI components and shared page behaviors.

2. **12 ASCX user controls spanning all patterns:**
   - Simple display: Breadcrumb, PageHeader (Session access), Footer, QuickStats (web.config tagPrefix)
   - Data-bound: AnnouncementCard (ViewState), EmployeeList (GridView + ViewState), TrainingCatalog (custom event)
   - Input with events: SearchBox (custom SearchEventArgs), DepartmentFilter, Pager
   - Complex: DashboardWidget (ITemplate pattern), ResourceBrowser (nested ASCX composition)

3. **3 custom base classes:**
   - `BasePage : System.Web.UI.Page` — auth check (Session redirect), audit logging (database), theme management, helper methods
   - `BaseMasterPage : System.Web.UI.MasterPage` — menu population (database), UserDisplayName property, script injection
   - `BaseUserControl : System.Web.UI.UserControl` — LogActivity(), cache helpers (CacheGet/CacheSet), common properties

4. **14 pages:** 2 public (Default, Login), 9 authenticated (inherit BasePage), 3 admin (BasePage + admin check), 1 master (Site.Master inherits BaseMasterPage). Pages demonstrate template controls, Repeater with ASCX ItemTemplate, event handling, Session write/read, nested ASCX.

5. **EF6 Database First with EDMX** — 6 entities (Employee, Department, Announcement, TrainingCourse, Resource, Enrollment). Tests toolkit's existing EDMX conversion.

6. **Work breakdown:** Phase 1 (Foundation: project + data model + base classes), Phase 2 (ASCX controls), Phase 3 (Pages), Phase 4 (Smoke test + migration coverage analysis + documentation). Phases 1-3 = Jubilee, Phase 4.1 = Jubilee, 4.2 = Bishop (toolkit gaps), 4.3 = Beast (docs). Acceptance tests (Colossus) and unit tests (Rogue) DEFERRED until Blazor "After" version exists.

7. **Timeline:** 7-11 days (1.5-2 weeks) for Phases 1-4.3. Deferred work TBD based on toolkit roadmap.

**Gap analysis — current samples vs DepartmentPortal:**

| Sample | .aspx Pages | .ascx Controls | Custom Base Classes | Patterns Exercised |
|--------|-------------|----------------|---------------------|-------------------|
| BeforeWebForms | 62 | 1 (trivial ViewSwitcher) | 0 | Control samples only |
| WingtipToys | 28 | 2 (trivial) | 0 | E-commerce, basic ASCX |
| ContosoUniversity | 5 | 0 | 0 | Education, minimal |
| **DepartmentPortal** | **14** | **12 (diverse)** | **3 (Page/Master/UserControl)** | **ITemplate, nested ASCX, web.config tagPrefix, custom events, ViewState/Session in controls** |

**High risks identified:**

- **R1:** Migration toolkit may not support ASCX → Blazor (Mitigation: Document manual patterns, Owner: Bishop)
- **R2:** Custom base classes have no Blazor equivalent (Mitigation: Design Blazor base component patterns, Owner: Forge + Cyclops)
- **R3:** ITemplate pattern not supported in Blazor (Mitigation: Document RenderFragment approach, Owner: Beast)

**Success criteria:**

✅ MVP: .NET 4.8 app builds, all ASCX render, all base classes function, events fire, ViewState/Session work, nested controls render, web.config tagPrefix works, ITemplate works  
✅ Migration coverage: Toolkit executed, gaps documented, backlog items created  
✅ Documentation: DEPARTMENTPORTAL.md with ASCX migration notes and base class patterns  
⏳ Full migration: Deferred until toolkit supports ASCX/base class conversion  

**Learnings:**

- **Enterprise Web Forms apps are ASCX-heavy.** Current samples (BeforeWebForms, WingtipToys, ContosoUniversity) have minimal ASCX usage, leaving the #1 reusability pattern untested by the migration toolkit.
- **Custom base classes (BasePage, BaseMasterPage, BaseUserControl) are standard enterprise patterns** for shared auth, logging, theme, menu population, caching. Zero samples demonstrate these patterns, so toolkit coverage is unknown.
- **ASCX patterns missing from toolkit scope:** Nested ASCX (controls containing other controls), ITemplate controls, web.config tagPrefix registration, custom event args, ViewState/Session in UserControl base class.
- **DepartmentPortal targets 8-12 controls** because that's the sweet spot for testing diverse patterns without over-complexity. Each control exercises a different migration challenge.
- **EF6 EDMX validation is bonus coverage.** ContosoUniversity already uses EDMX, but DepartmentPortal adds 6 entities vs 5, tests more FK relationships and cascade deletes.
- **Phased approach critical:** Foundation → Controls → Pages → Testing. Can't test migration until the Web Forms app exists. Can't write Blazor tests until ASCX → Blazor conversion works.
- **Deferred work (acceptance tests, unit tests) is CORRECT sequencing.** Playwright and bUnit tests require the Blazor "After" version. Creating those tests now would be speculative.

**File paths:**
- Milestone plan: `planning-docs/ASCX-SAMPLE-MILESTONE.md`
- Decision: `.squad/decisions/inbox/forge-ascx-sample-milestone.md`

### Ajax Control Toolkit Extender Pattern Design (2026-03-15)

**Task:** Design Blazor-native architecture for Ajax Control Toolkit extenders (#442, M24). This is the gating architecture decision for all 13+ ACT component implementations.

**Deliverable:** `.squad/decisions/inbox/forge-ajax-extender-pattern.md`

**Key design decisions:**

1. **Modified Option 2 — string-based TargetControlID with JS-side resolution:** Extenders accept `TargetControlID` as a string (matching Web Forms), resolve it via BWFC's `FindControl()` → `ClientID` chain, then pass the ClientID to JavaScript's `document.getElementById()`. No `@ref` required on target controls. Migration markup is zero-change: `<CalendarExtender TargetControlID="txtDate" />`.

2. **Two base classes:** `BaseExtenderComponent` (inherits `BaseWebFormsComponent`) for controls that render no HTML and attach JS behavior to a target. `BaseStandaloneToolkitComponent` (inherits `BaseStyledComponent`) for controls like Accordion/TabContainer/Rating that render their own HTML plus JS. Both follow the same JS module lifecycle pattern.

3. **One ES module per component:** Following the Chart.js interop precedent (`ChartJsInterop` + `chart-interop.js`). Lazy-loaded via `import()`. Shared utilities in `_shared/behavior-base.js`. Tree-shaking by usage — only JS for used controls loads.

4. **Separate NuGet package:** `Fritz.BlazorAjaxToolkitComponents` in `src/BlazorAjaxToolkitComponents/`. References core BWFC transitively. Keeps ACT overhead out of core package. Different release cadence.

5. **Component classification:** 10 extenders (CalendarExtender, ModalPopupExtender, CollapsiblePanelExtender, ConfirmButtonExtender, AutoCompleteExtender, MaskedEditExtender, NumericUpDownExtender, FilteredTextBoxExtender, TextBoxWatermarkExtender, ValidatorCalloutExtender) + 3 standalone (Accordion, TabContainer/TabPanel, Rating).

6. **Implementation order:** Phase 1 (foundation + CollapsiblePanel + ConfirmButton), Phase 2 (Calendar + ModalPopup + Accordion), Phase 3 (TextBox extenders), Phase 4 (TabContainer + Rating + AutoComplete + ValidatorCallout). ~8 sprints total.

7. **SSR compatibility:** Extenders silently degrade (try/catch on JS init). Standalone controls render HTML structure visible; JS adds show/hide behavior on hydration. Matches existing SSR-first strategy.

8. **TargetControlID fallback:** If `FindControl()` fails (target is plain HTML or non-BWFC component), extender uses `TargetControlID` as literal DOM ID. JavaScript `getElementById()` handles both cases.

**Learnings:**

- ACT `ExtenderControlBase` renders zero HTML — all behavior is JS. This means extender `.razor` files are one-liners (`@inherits BaseExtenderComponent`). The abstraction lives entirely in the C# code-behind and JS module.
- BWFC already has 90% of the infrastructure needed: `Parent` cascading, `FindControl()`, `ComponentIdGenerator.GetClientID()`, `IJSRuntime` injection. The extender base just orchestrates the JS lifecycle on top.
- `TextBoxWatermarkExtender` is functionally identical to HTML5 `placeholder` attribute. Recommend implementing as thin shim that sets `placeholder` — zero JS needed. Mark `[Obsolete]` with migration guidance.
- `ValidatorCalloutExtender` depends on Validator Display property improvements (separate M20 issue). Should be last in implementation order.
- The Chart.js interop pattern (`Lazy<Task<IJSObjectReference>>` + ES module import) is the proven template. Every ACT extender JS module exports 3 functions: create, update, dispose.

📌 Team update (2026-03-15): Ajax Toolkit extender pattern designed for #442 (M24). Two base classes (BaseExtenderComponent, BaseStandaloneToolkitComponent), string-based TargetControlID→FindControl→ClientID resolution, one ES module per component, separate NuGet package. 10 extenders + 3 standalone classified. 4-phase implementation plan (~8 sprints). SSR-safe. Decision in inbox for Jeff review. — decided by Forge


### Component Health Dashboard PRD Review (2026-07-25)

**Task:** Self-review of PRD `dev-docs/prd-component-health-dashboard.md` (issue #439) at Jeff's request.

**Key findings:**

1. **Data model bug — ToolTip misplaced:** Appendix A lists ToolTip under BaseStyledComponent (claiming 10 params) but ToolTip is actually declared on BaseWebFormsComponent (line 146). Correct counts: BaseWebFormsComponent = 21 params, BaseStyledComponent = 9 params. PRD total says 36 base class params — needs recalculation.

2. **Scoring model evolved:** Original spec (2026-03-15) had 7 dimensions including HTML Fidelity (15%), Style Support (15%), and Integration Tests (5%). Current PRD has 6 dimensions — dropped HTML Fidelity, Style Support, Integration Tests; added Documentation (15%) and Implementation Status (10%). The simplification is an improvement — HTML fidelity requires Playwright infrastructure that doesn't exist, and binary test detection is more honest than complexity-weighted thresholds.

3. **tools/WebFormsPropertyCounter/ doesn't exist** and no tools/ directory exists. The .NET Fx 4.8 console app approach requires infrastructure the team doesn't have. MSDN manual-count fallback is realistic.

4. **No overlap with bwfc-scan.ps1:** That script scans *customer* Web Forms projects for migration readiness. The dashboard scans *BWFC library* components for implementation completeness. Completely different tools, no redundancy.

5. **scripts/Invoke-ComponentHealthScan.ps1 was planned (2026-03-15 spec) but never created.** The PRD supersedes that plan with a C# runtime reflection approach instead of PowerShell — correct decision, reflection from the live assembly is more reliable.

6. **ComponentCatalog.cs has 183 entries** but PRD tracks 54 components. The catalog includes AJAX controls, sub-components, and utilities that the dashboard correctly excludes. No conflict.

**Architecture learnings:** Phase ordering (baselines → service → UI → export) is correct. Phase 1 (baselines) is the bottleneck — no .NET Fx 4.8 toolchain available, so manual curation from MSDN is the realistic path.

### Reskill Audit  Charter Optimization (2026-03-15)

Completed full reskill audit of all 7 agent charters per .squad/skills/reskill/SKILL.md.Total current: 21,875 bytes across 7 charters. Identified 18 procedural blocks for extraction into 3 new skills: gent-workflow (shared collaboration boilerplate from all 7 agents), playwright-testing (Colossus's 5 test procedure blocks), scribe-procedures (Scribe's 6 operational procedure blocks). Projected savings: ~13,875 bytes (64%). All charters projected under 1,500 bytes. Biggest wins: Scribe (5,045800, -84%) and Colossus (4,8041,400, -71%). Also flagged .ai-team/  .squad/ path correction needed across all charters. Full analysis with complete slim charter text written to .squad/decisions/inbox/forge-reskill-audit.md for Scribe to merge.

### PRD Bug Fixes — Component Health Dashboard (2026-07-25)

Fixed 3 bugs in `dev-docs/prd-component-health-dashboard.md` identified during review:

1. **ToolTip misplaced in Appendix A:** ToolTip was listed under BaseStyledComponent (10 params) but source code (BaseWebFormsComponent.cs:146) proves it is declared on BaseWebFormsComponent. Fixed counts: BaseWebFormsComponent=21, BaseStyledComponent=9, total still 36. Also corrected the Pitfall 2 example counts and the hierarchy annotation.
2. **Baseline methodology priority flipped (3.2):** MSDN manual curation is now Preferred (immediately actionable), .NET Fx 4.8 reflection tool is Acceptable fallback (requires SDK + nonexistent tools/WebFormsPropertyCounter/).
3. **Acceptance criterion #9 (10) was dishonest:** Changed from 'All 52 completed components show tests=check' to explicitly exclude the 7 Login controls (ChangePassword, CreateUserWizard, Login, LoginName, LoginStatus, LoginView, PasswordRecovery) which have zero bUnit coverage.

📌 Team update (2026-03-16): MSBuild toolchain verified for .NET 4.8 WebForms compilation — reflection-based property discovery tool confirmed viable as primary methodology. — verified by Coordinator

### PRD Decomposition — Component Health Dashboard #439 (2026-07-25)

**Task:** Decompose PRD `dev-docs/prd-component-health-dashboard.md` into implementable work items with dependency ordering.

**Deliverable:** `.squad/decisions/inbox/forge-prd-decomposition.md` — 12 work items (11 actionable + 1 blocked).

**Key decomposition decisions:**

1. **Reflection tool is primary, not fallback:** MSBuild 18.5 + Roslyn confirmed viable this session. The `webforms-reflection-tool` work item (Cyclops, L) builds `tools/WebFormsPropertyCounter/` as a .NET Fx 4.8 console app — the denominator for all health scoring.

2. **Invoke-ComponentHealthScan.ps1 abandoned:** Was planned 2026-03-15 but never created. PRD supersedes with C# runtime reflection — strictly superior for type hierarchy awareness and the 10 pitfalls from §8. No overlap to manage.

3. **HTML Fidelity dimension BLOCKED:** Playwright not installed. Work item `html-fidelity-dimension` (Colossus, L) parked until Playwright infrastructure exists. Current scoring model uses 6 dimensions (100% weight) — when HTML Fidelity is added, weights redistribute.

4. **Critical path:** `webforms-reflection-tool` → `reference-baselines-curate` → `scoring-engine` → `health-service-assembly` → `dashboard-razor-page`. ~XL overall. 3 parallel lanes possible after `tracked-components-config` and after `health-service-assembly`.

5. **5 agents engaged:** Cyclops (4 items: tool + counter + detection + service + scoring), Forge (2 items: config + baselines curation), Rogue (1: unit tests), Jubilee (2: UI + catalog), Beast (1: MkDocs export). Colossus blocked on Playwright.

6. **Open questions for Jeff:** Where should ComponentHealthService live (sample app vs library)? Should reflection tool auto-generate tracked-components-config? MkDocs export: CI or manual?

### Reference Baselines & Tracked Components — Deliverables Complete (2026-07-25)

**What was done:**
1. **PRD §3.2 updated:** Removed reflection-tool fallback. MSDN manual curation is now the SOLE method for obtaining reference baselines. Source field updated in JSON example. §7.3 updated to remove `tools/WebFormsPropertyCounter/` reference.
2. **`dev-docs/tracked-components.json` created:** 61 components mapped to Web Forms types and categories (Editor:28, Data:9, Validation:8, Navigation:3, Login:7, Infrastructure:6). Includes all deferred (Substitution, Xml) and abstract (BaseValidator, BaseCompareValidator).
3. **`dev-docs/reference-baselines.json` created:** Full property/event baselines for all 61 components sourced from MSDN .NET Framework 4.8 API documentation. Includes propertyList and eventList arrays for verifiability. 24 complex controls flagged with `confidence: needs-verification`.

**Key architecture decisions:**
- **MSDN-only sourcing:** Jeff's directive — no reflection tools, no .NET Fx console apps. MSDN docs are the sole authoritative source.
- **Symmetric counting rules:** Style sub-object properties (DayStyle, HeaderStyle, etc.) excluded from Web Forms baselines to match BWFC's RenderFragment exclusion. Template properties (ITemplate) likewise excluded.
- **Stop-points for Web Forms counting:** WebControl, Control, BaseDataBoundControl, DataBoundControl — symmetric with BWFC's BaseWebFormsComponent/BaseStyledComponent/BaseDataBoundComponent/DataBoundComponent<T>.
- **Validator family inclusion:** Concrete validators include full property chain from their family (e.g., CompareValidator includes BaseCompareValidator + BaseValidator + Label declared properties).

**Key file paths:**
- `dev-docs/prd-component-health-dashboard.md` — updated §3.2, §7.3
- `dev-docs/tracked-components.json` — component → Web Forms type mapping
- `dev-docs/reference-baselines.json` — expected property/event counts per component
- Decision: `.squad/decisions/inbox/forge-baselines-methodology.md`

### HttpHandlerBase Architecture Proposal (2026-07-25)

**Task:** Design base class for migrating `.ashx` handler code-behind to ASP.NET Core middleware with minimal rewrites.

**Deliverable:** `.squad/decisions/inbox/forge-ashx-handler-base-class.md` — comprehensive architecture proposal covering IHttpHandler API surface analysis, ASP.NET Core mapping (member-by-member), proposed class hierarchy, before/after code examples, risks, and 6-issue implementation plan.

**Key design decisions:**

1. **`HttpHandlerBase` abstract base class** (not interface) — follows `WebFormsPageBase` pattern. Exposes `ProcessRequestAsync(HttpHandlerContext)` as the migration-compatible override point. `IsReusable` always true (middleware is inherently reusable).

2. **`HttpHandlerContext` adapter class** — wraps ASP.NET Core `HttpContext` with Web Forms-like API surface. Sub-objects: `HttpHandlerRequest` (QueryString, Form, Files), `HttpHandlerResponse` (Write, BinaryWrite, AddHeader, ContentType), `HttpHandlerServer` (MapPath, HtmlEncode, UrlEncode).

3. **Endpoint routing, not raw middleware** — handlers are URL-specific, so `MapHandler<T>()` and `MapBlazorWebFormsHandlers()` use endpoint routing. Convention-based assembly scanning via `[HandlerRoute]` attribute.

4. **Sync-over-async shims for Write/BinaryWrite** — deliberate migration compatibility decision. No SynchronizationContext in middleware context (unlike Blazor), so sync-over-async is safe. Async alternatives provided for perf-sensitive handlers.

5. **`Response.End()` marked `[Obsolete]`** — cannot replicate ThreadAbortException. Sets `IsEnded` flag instead. Developer adds `return`. Matches `ViewState` obsolescence pattern.

6. **Session support via `[RequiresSessionState]` attribute** — middleware auto-calls `LoadAsync()` before `ProcessRequestAsync()`. `GetObject<T>()`/`SetObject<T>()` extensions for typed session values.

7. **Main BWFC package** — not separate NuGet. ~500 lines of production code, same migration infrastructure category as WebFormsPageBase/ResponseShim/RequestShim.

8. **Coordinates with existing AshxHandlerMiddleware** — modified to skip paths with registered handler endpoints (no 410 for migrated handlers).

**What doesn't map:**
- `Server.Transfer()` / `Server.Execute()` — no Core equivalent, unsupported
- `Application["key"]` — global mutable state, migrate to DI/IMemoryCache
- `Response.End()` ThreadAbortException — behavioral change, shim only sets flag
- Complex `HttpPostedFile.SaveAs()` — different API shape from `IFormFile`

**Scope:** Medium. ~1,200 lines total (840 prod + 380 test). 6 issues: core adapters (M), routing (S), middleware coordination (S), session (S), docs (S), L1 script (S).

📌 Team update (2026-07-25): HttpHandlerBase architecture proposal delivered — abstract base class + context adapters for .ashx code migration. Endpoint routing, sync-over-async shims, session via attribute, main BWFC package. 6-issue implementation plan. Decision in inbox for Jeff review. — decided by Forge

### Minimal API Pivot for HttpHandlerBase (2026-07-25)

Jeff directed a design pivot: replace custom `[HandlerRoute]` attribute + `MapBlazorWebFormsHandlers()` assembly scanning with **Minimal API registration**. Analyzed and produced revised design (§R1–R9) appended to the original proposal.

**Key design decisions in the revision:**

1. **`app.MapHandler<THandler>("/path")` as primary API** — generic extension on `IEndpointRouteBuilder`, follows `MapGet`/`MapPost` naming convention. Returns `RouteHandlerBuilder` for auth/CORS/rate-limiting chaining.

2. **`ActivatorUtilities.CreateInstance<THandler>` per request** — DI-powered handler instantiation without explicit container registration. Constructor injection just works. Transient lifetime (matches Web Forms `IsReusable = false` common case).

3. **`endpoints.Map()` for all HTTP methods** — not `MapGet`/`MapPost`. Web Forms `ProcessRequest` handles all methods; `Map()` (available .NET 7+, we target .NET 10) preserves this behavior exactly.

4. **No `IResult` return** — delegate returns `Task` (void). Handler writes directly to Response via adapter. Minimal API sees no return value and doesn't interfere. Matches Web Forms behavior.

5. **`[HandlerRoute]` attribute eliminated** — routes declared in `Program.cs` only. Explicit, visible, matches Minimal API philosophy. One fewer file, one fewer concept.

6. **Assembly scanning eliminated** — no `MapBlazorWebFormsHandlers()`. Each handler gets an explicit `MapHandler<T>()` call. L1 script generates these lines.

**Impact:** ~100 fewer lines of production code (400 vs 500). One fewer file (`HandlerRouteAttribute.cs` gone). Issue 2 (routing) shrinks from Size S to XS. Estimated timeline: 2-3 weeks (was 3-4). Adapter layer (`HttpHandlerContext`, `HttpHandlerResponse`, `HttpHandlerServer`) completely unchanged.

**Learning:** Original §7.3 rejected Minimal API because "they require completely different code structure." Wrong framing — Jeff's insight is to use Minimal API as *registration and dispatch infrastructure* while keeping the `HttpHandlerBase` class structure intact. The handler code developers write is nearly identical. Only the plumbing underneath changes.


 **Team update (2026-03-20):** Component audit recommendations merged (March 2026 prioritization guide). 52/54 components at 100% health (96.3%). Tier 1 quick wins identified: FileUpload properties, infrastructure docs, View docs, ScriptManager decision.  decided by Forge


 **Team update (2026-03-21):** DepartmentPortal ASCX sample milestone plan created (28KB specification). 12 ASCX controls, 3 custom base classes, 14 pages designed to test migration toolkit coverage for enterprise Web Forms patterns. 4-phase work breakdown with 7-11 day timeline.  decided by Forge

### Custom Server Controls Addition to DepartmentPortal Milestone (2026-03-21)

DepartmentPortal scope expanded to include **6 custom server controls** covering Web Forms custom control patterns absent from ASCX-only test coverage:

1. **StarRating : WebControl** — Simple property-driven rendering, `RenderContents()`, star HTML generation
2. **EmployeeCard : CompositeControl** — Programmatic child control creation, `CreateChildControls()`, data binding to composite children
3. **SectionPanel : Templated Control** — `ITemplate` properties, template instantiation, INamingContainer pattern
4. **PollQuestion : IPostBackEventHandler** — ViewState for selected option, postback event routing, vote submission handling
5. **NotificationBell : Custom Events** — Custom EventArgs classes, event delegates, UI state events
6. **EmployeeDataGrid : DataBoundControl** — `PerformDataBinding()`, filtering/sorting, paging state in ViewState, child GridView binding

**Rationale:** ASCX controls exercise 50% of Web Forms component patterns. Custom controls hit the other 50%: programmatic control creation (CompositeControl), template-driven rendering (ITemplate), postback handling (IPostBackEventHandler/IPostBackDataHandler), ViewState management at control level, custom events, and DataBoundControl patterns. These patterns appear frequently in enterprise Web Forms and require distinct migration logic.

**Key migration challenge differences:**
- ASCX: markup-defined child controls, declarative data binding, server-side include pattern
- Custom controls: code-defined control tree (CreateChildControls), imperative binding (PerformDataBinding), postback events as method implementations

**Architecture note:** Custom controls differ from ASCX in control tree construction (code vs markup) and event routing (method implementation vs page event handlers). The migration strategy must handle both patterns: ASCX→Blazor component markup, custom controls→Blazor component C# class with programmatic child binding.

**File additions to planning doc:**
- Section 3: Custom Server Controls (6 controls, 7 subsections, ~600 lines)
- Updated Executive Summary (Purpose, Gap Analysis, Target reflect custom controls scope)
- Renumbered Sections 4-14 (Custom Base Classes now §4, Page Inventory §5, etc.)
- **Bare System.Web.UI.Control base class coverage added (3.7 DepartmentBreadcrumb):** Jeff requested explicit testing of the primitive Control base class (no WebControl wrapping, no built-in HTML element, no style properties). DepartmentBreadcrumb demonstrates pure `Render()` override, `IPostBackEventHandler` direct event handling, and zero ViewState usage—patterns distinct from WebControl/CompositeControl/DataBoundControl. Rounds out custom control pattern coverage for migration testing.

**Decision:** Planning doc finalized. Custom controls scope is *designed, not implemented*. Implementation will verify these patterns execute on .NET Framework 4.8, render correct HTML, and exercise the code-based control creation patterns Blazor components must simulate. Approved for L1 scripting phase.

### DepartmentPortal Migration Analysis (2026-07-25)

Completed comprehensive migration analysis of DepartmentPortal's 12 ASCX + 7 custom controls against BWFC capabilities. Key findings:

- **BWFC covers ~70% of migration patterns today.** ASCX controls (7 Easy, 4 Medium, 1 Hard) map well to existing BWFC components. Simple asp:Literal/TextBox/Button/Label/DropDownList/GridView conversions are well-supported.
- **CustomControls/ namespace is the critical enabler** for custom C# server controls. `WebControl` + `HtmlTextWriter` bridge allows near-direct port of `RenderContents` code. `CompositeControl` supports `CreateChildControls` pattern.
- **4 key gaps identified:**
  1. No `DataBoundWebControl<T>` — controls inheriting `DataBoundControl` with custom `RenderContents` can't use a single base class (P1, HIGH)
  2. No `TagKey` / `AddAttributesToRender` on `CustomControls.WebControl` — Web Forms auto-wraps outer tag, BWFC doesn't (P2, HIGH)
  3. `HtmlTextWriter` enum coverage incomplete — missing Colspan, For, Checked, Nav, Footer, etc. (P3, MEDIUM)
  4. `CompositeControl.RenderChildren` throws for non-WebControl children — can't add BWFC Label/Image/HyperLink as children (P4, MEDIUM)
- **IPostBackEventHandler has no BWFC equivalent** and requires complete rewrite to EventCallback — this is by design (Blazor has no postback model).
- **ITemplate → RenderFragment is conceptually clean** but has no documentation or helper in BWFC.
- **BaseUserControl (logging + caching)** maps to standard .NET DI: `ILogger<T>` + `IMemoryCache`.

Analysis written to `.squad/decisions/inbox/forge-departmentportal-migration-plan.md`.

 Team update (2026-03-22): DepartmentPortal migration analysis completed  12 ASCX + 7 custom controls assessed, BWFC gaps identified, improvement recommendations provided. Analysis logged to decisions.md for team review.  decided by Forge


 Team update (2026-03-22): Upstream issue creation completed  7 GitHub issues created on FritzAndFriends/BlazorWebFormsComponents (#490 P1 DataBoundWebControl, #491 P4 CompositeControl, #492 P2 TagKey, #493 P3 HtmlTextWriter, #494 P5 ITemplate, #495 User Controls docs, #496 FindControl)  decided by Forge

### P1–P5 Drop-in Replacement Implementation Plan (2026-03-22)

**Created comprehensive implementation plan** for addressing all 7 upstream issues (#490–#496) with drop-in replacement shimming. Key decisions:

1. **Implementation order:** P2 (TagKey) → P3 (enums) → P1 (DataBoundWebControl) → P4 (CompositeControl) → P5 (TemplatedWebControl) → FindControl → Docs. P2 is foundation because Web Forms Render() pipeline depends on TagKey/AddAttributesToRender.

2. **Namespace: `BlazorWebFormsComponents.CustomControls`** — NOT `System.Web.UI`. Avoids conflicts with any remaining Web Forms references during incremental migration.

3. **WebControl.Render() breaking change accepted:** Default Render() changes from no-op to auto-rendering outer tag via TagKey. Matches Web Forms behavior. Controls overriding Render() unaffected.

4. **Postback controls (DepartmentBreadcrumb, PollQuestion) cannot be shimmed** — IPostBackEventHandler/GetPostBackEventReference require manual rewrite to EventCallback/@onclick. Documented as migration pattern, not shim target.

5. **ITemplate → RenderFragment is documentation + TemplatedWebControl base class** — no fake ITemplate interface. The paradigms are fundamentally different (imperative vs declarative).

6. **FindControl: guidance-first with FindControlRecursive bridge** — existing FindControl compiles but doesn't traverse deep. Add opt-in recursive version, but document @ref/CascadingParameter/EventCallback as the real target.

7. **4 new files, 4 modified files, ~9 days estimated effort.** Net-new: DataBoundWebControl.cs, LiteralControl.cs, ShimControls.cs, TemplatedWebControl.cs.

**Learnings:**
- Web Forms WebControl.Render() is NOT a blank canvas — it's a structured pipeline (AddAttributesToRender → RenderBeginTag → RenderContents → RenderEndTag). BWFC's current Render() as a virtual no-op is incorrect.
- 5 of 7 DepartmentPortal custom controls can be drop-in shimmed; 2 (postback-based) require manual rewrite.
- The DataBoundWebControl gap is the single biggest blocker — data-heavy migrations (grids, lists) all hit this.

Plan written to session workspace plan.md. Architecture decisions written to `.squad/decisions/inbox/forge-p1p5-plan.md`.

📡 Team update (2026-03-22): P1–P5 implementation plan completed — 6 phases, 4 new files, 4 modified files, ~9 days. Execution order: P2→P3→P1→P4→P5→FindControl. Key: WebControl gets proper Render pipeline, 4 new shim types, namespace stays in BWFC.CustomControls. ✅ decided by Forge
