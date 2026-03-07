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

📬 Team update (2026-03-06): All 15 P0+P1 items from Run 8 post-mortem implemented — Cyclops did 9 script fixes (RF-03/04/06/07/08/10/11/12/13), Beast did 6 skill fixes (RF-01/02/05/09/13/14). Committed to squad/run8-improvements branch. — decided by Forge (analysis), implemented by Cyclops + Beast

### Run 9 CSS/Image Failure — Root Cause Analysis (2026-03-06)

**Investigation:** Deep-dive into why Run 9 screenshots show completely unstyled HTML (navbar as bullet list, all product images 404) despite 14/14 acceptance tests passing and Run 8 having proper Bootstrap styling.

**Three independent root causes identified:**

1. **RC-1 (P0 Script):** `ConvertFrom-MasterPage` doesn't handle `<webopt:bundlereference>` — the source `Site.Master` uses ASP.NET Web Optimization bundle syntax (`<webopt:bundlereference path="~/Content/css" />`) instead of explicit `<link>` tags. The script's regex only matches `<link>`, `<meta>`, `<title>`. The `New-AppRazorScaffold` also generates App.razor with zero CSS links. Result: Layer 1 output has NO CSS references anywhere.

2. **RC-2 (P0 Layer 2):** Run 9's Cyclops agent changed image `src` paths from `/Catalog/Images/` (where files actually are in `wwwroot/Catalog/Images/`) to `/Images/Products/` (FreshWingtipToys convention) without moving the files. Run 8's Layer 2 preserved the original paths and worked. Verified at runtime: `/Images/Products/carconvert.png` → 404; `/Catalog/Images/carconvert.png` → 200.

3. **RC-3 (P1 Tests):** All 14 acceptance tests check only functional behavior (navigation, auth, forms). None verify CSS loads, Bootstrap classes are styled, or images render. This creates a false-positive pass rate.

**Key evidence:** The committed App.razor DOES have CSS links (added by Layer 2 after screenshots), and CSS loads correctly at runtime. The wwwroot diff between Run 8 and Run 9 is empty — identical files. Only the references differ. The Run 9 REPORT.md mischaracterizes broken screenshots as working.

**5 fixes proposed** (prioritized): immediate image path repair, script CSS auto-detection, script bundle reference handling, Layer 2 skill guidance, visual smoke test.

**Decision document:** `.ai-team/decisions/inbox/forge-run9-css-failure-rca.md`

### Fix 1a + Fix 1b Implementation — Run 9 RCA Remediation (2026-03-06)

**Scope:** Two surgical fixes to `migration-toolkit/scripts/bwfc-migrate.ps1` addressing RC-1 from the Run 9 CSS/Image Failure RCA.

**Fix 1a: `<webopt:bundlereference>` extraction in ConvertFrom-MasterPage**
- Added regex extraction for `<webopt:bundlereference>` tags in the head section extraction loop (after `<link>` extraction)
- For each match: extracts `path` attribute, writes a `Write-ManualItem` with category `CSSBundle`, and injects a `@* TODO *@` comment into the `<HeadContent>` block
- Also added CDN link/script preservation — any `<link>` or `<script>` tags referencing CDN domains (bootstrapcdn, googleapis, jsdelivr, unpkg, cdnjs, cloudflare) are now captured from `<head>` and preserved

**Fix 1b: CSS auto-detection via `Invoke-CssAutoDetection` function**
- New function added to the Project Scaffolding region
- After static files are copied, scans `wwwroot/Content/` for `.css` files (with `wwwroot/css/` fallback), plus root-level `wwwroot/*.css`
- Scans source `Site.Master` for CDN `<link>` and `<script>` tags
- Injects all found CSS `<link>` tags and CDN references into `App.razor` `<head>` before `<HeadOutlet>`
- Guarded by `-not $WhatIfPreference -and -not $SkipProjectScaffold`

**Verification:** Tested with mock Web Forms project containing `<webopt:bundlereference>`, CDN Bootstrap/jQuery links, and two CSS files in Content/. All three verification criteria passed:
1. Bundle reference flagged as `[CSSBundle]` manual review item ✅
2. CSS files auto-detected and `<link>` tags injected into App.razor ✅
3. CDN references (Bootstrap CSS, jQuery JS) preserved in App.razor `<head>` ✅

