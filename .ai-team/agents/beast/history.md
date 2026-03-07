# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- ⚠ Summarized 2026-02-27 by Scribe — covers M1–M16 -->

<!-- ⚠ Summarized 2026-03-06 by Scribe — older entries archived -->

### Archived Sessions

- Core Context (2026-02-10 through 2026-02-27)
- Doc Work Summary (2026-02-27 through 2026-03-03)
- Key Team Updates (2026-02-27 through 2026-03-03)
- Migration Report Conventions (2026-03-04)
- Run 5 Benchmark Report (2026-03-05)
- Run 6 Benchmark Report (2026-03-04)
- Render Mode Placement Correction (2026-03-05)

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

 Team update (2026-03-06): LoginView must be preserved as BWFC component, not converted to AuthorizeView  decided by Jeff (directive)

### Run 9 Skill Documentation Fixes (2026-03-07)

- **Scope:** 6 fixes (RF-01, RF-02, RF-05, RF-09, RF-13, RF-14) across 4 skill files, identified by Forge in Run 8 post-mortem.
- **bwfc-identity-migration/SKILL.md updated:**
  - RF-01 (P0): Added "⚠️ Cookie Auth Under Interactive Server Mode" section after Overview — documents that HttpContext is NULL during WebSocket circuits, cookie auth MUST use `<form method="post">` → minimal API endpoints, not Blazor event handlers.
  - RF-02 (P0): Added "Ready-to-Use Endpoint Templates" section with complete copy-paste C# code for login, register, and logout minimal API endpoints (based on actual Run 8 Program.cs).
  - RF-05 (P1): Added bold warning callout "NEVER replace LoginView with AuthorizeView" after the LoginView conditional content section.
  - RF-14 (P1): Added "DisableAntiforgery Requirement" section explaining that `.DisableAntiforgery()` is required on all minimal API endpoints receiving Blazor form POSTs.
- **bwfc-data-migration/SKILL.md updated:**
  - RF-01 (P0): Added "⚠️ Session State Under Interactive Server Mode" section — documents three options (minimal API, scoped services, database-backed) for handling session-dependent state when HttpContext.Session is null.
  - RF-09 (P1): Added "Blazor Enhanced Navigation" section with the problem, three workaround options (form POST, data-enhance-nav, JS), and a decision table for which to use.
  - RF-14 (P1): Added DisableAntiforgery notes inline wherever `<form method="post">` patterns appear.
- **migration-standards/SKILL.md updated:**
  - RF-05 (P1): Reinforced LoginView → "preserve as BWFC LoginView — do NOT rewrite as AuthorizeView" with expanded explanation.
  - RF-09 (P1): Added "Blazor Enhanced Navigation" pattern under standards — documents the `<form method="post">` / `data-enhance-nav="false"` requirement for minimal API links.
- **bwfc-migration/SKILL.md updated:**
  - RF-13 (P1): Added "ListView with GroupItemCount" example showing Web Forms 4-column grid → BWFC equivalent with LayoutTemplate, GroupTemplate, and ItemTemplate.
- **Key patterns documented:**
  - The `<form method="post">` → minimal API → redirect pattern is the canonical way to do auth + session operations under Interactive Server mode.
  - `.DisableAntiforgery()` is required on all minimal API endpoints receiving form POSTs from Blazor pages.
  - Enhanced navigation breaks `<a href>` links to API endpoints; use `<form>` or `data-enhance-nav="false"`.
  - LoginView is a first-class Blazor component, never convert to AuthorizeView.

📬 Team update (2026-03-06): All 15 P0+P1 Run 9 prep items implemented — Cyclops completed 9 script fixes (RF-03/04/06/07/08/10/11/12/13) in bwfc-migrate.ps1. Combined with Beast's 6 skill fixes, all P0+P1 items done on squad/run8-improvements. — decided by Forge (analysis), implemented by Cyclops + Beast


 Team update (2026-03-06): Run 9 CSS/image failure RCA  script drops bundle refs, Layer 2 changed image paths without moving files. 5 fixes proposed.  decided by Forge

### Run 9 RCA Documentation Updates (2026-03-07)

- **migration-standards/SKILL.md updated:** Added two new sections after "Static Asset Relocation":
  - **Static Asset Path Preservation** — CRITICAL RULE: Layer 2 must preserve source image/asset path structure. `wwwroot/` files are the source of truth. Never rewrite `src` attributes to paths where files don't exist. Includes concrete bad/good example from Run 9 (Catalog/Images → Images/Products rewrite that caused 404s).
  - **CSS Reference Verification** — After Layer 2, verify App.razor `<head>` has `<link>` tags for CSS files. Bootstrap CSS is required for navbar/layout. Missing CSS is P0.
  - Updated the existing image path guidance to clarify it applies only to tilde-prefixed (`~/`) paths.
- **Run 9 REPORT.md updated to FAILED status:** Changed result from ✅ 14/14 to ❌ FAILED. Added failure banner explaining CSS + image regression causes. Rewrote executive summary bottom line to acknowledge visual regression despite functional test pass. Updated conclusion verdict.
- **Learning:** Functional acceptance tests passing does NOT mean a migration is successful. Visual regression (no CSS, broken images) is a ship-blocking failure even when all Playwright tests are green. Future runs need visual regression checks in addition to functional tests.

 Team update (2026-03-07): Layer 1 now auto-detects CSS via Invoke-CssAutoDetection  skills no longer need Layer 2 CSS wiring guidance.  decided by Forge
 Team update (2026-03-07): 11 static asset smoke tests added to acceptance suite. Migration scripts must preserve static asset paths or tests fail.  decided by Rogue
