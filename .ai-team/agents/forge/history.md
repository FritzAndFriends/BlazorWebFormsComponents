# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents  Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Core Context

<!-- Summarized 2026-03-04 by Scribe — originals in history-archive.md -->

M1–M16: 6 PRs reviewed, Calendar/FileUpload rejected, ImageMap/PageService approved, ASCX/Snippets shelved. M2–M3 shipped (50/53 controls, 797 tests). Chart.js for Chart. DataBoundStyledComponent<T> recommended. Key patterns: Enums/ with int values, On-prefix events, feature branches→upstream/dev, ComponentCatalog.cs. Deployment: Docker+NBGV, dual NuGet, Azure webhook. M7–M14 milestone plans. HTML audit: 3 tiers, M11–M13. M15 fidelity: 132→131 divergences, 5 fixable bugs. Data controls: 90%+ sample parity, 4 remaining bugs. M17 AJAX: 6 controls shipped.

## Learnings

### LoginView → AuthorizeView Redesign Analysis (2026-03-05)

**Task:** Analyze original Web Forms LoginView, compare with current BWFC implementation, propose AuthorizeView-based redesign.

**Key findings about original Web Forms LoginView:**
- Inherits `Control` (NOT `WebControl`) — has no style properties (CssClass, Style, etc.)
- Renders NO wrapper element — just the active template's content directly
- Three template types: `AnonymousTemplate`, `LoggedInTemplate`, `RoleGroups` (collection of `RoleGroup` with `ContentTemplate`)
- Template priority: unauthenticated → AnonymousTemplate; authenticated → first matching RoleGroup (declaration order, first-match-wins) → LoggedInTemplate fallback
- Events: `ViewChanged`, `ViewChanging`

**Current BWFC issues identified (8 total, 3 high severity):**
1. Wrong base class (`BaseStyledComponent` → should be `BaseWebFormsComponent`)
2. Spurious wrapper `<div>` (confirmed by HTML audit output)
3. Manual auth state via injected `AuthenticationStateProvider` (doesn't react to changes, no async handling)
4. `RoleGroup.ChildContent` should be `ContentTemplate`
5. `RoleGroups` parameter typed as `RoleGroupCollection` instead of `RenderFragment`

**Architecture decision:** Delegate to `<AuthorizeView>` internally. LoginView becomes a thin adapter: `AnonymousTemplate` → `NotAuthorized`, `LoggedInTemplate` → fallback in `Authorized`, RoleGroups rendered first for self-registration then checked in `Authorized` callback. No manual `AuthenticationStateProvider` needed.

**Key file paths:**
- `src/BlazorWebFormsComponents/LoginControls/LoginView.razor` — current component markup (with wrong `<div>` wrapper)
- `src/BlazorWebFormsComponents/LoginControls/LoginView.razor.cs` — current code-behind (manual auth state)
- `src/BlazorWebFormsComponents/LoginControls/RoleGroup.razor.cs` — self-registration via cascading parameter
- `src/BlazorWebFormsComponents/LoginControls/RoleGroupCollection.cs` — first-match-wins logic (correct, keep)
- `src/BlazorWebFormsComponents/LoginControls/LoginStatus.razor.cs` — same manual-auth-state anti-pattern (follow-up candidate)
- `audit-output/webforms/LoginView/LoginView-1.html` — confirms no wrapper element in original
- `docs/LoginControls/LoginView.md` — existing documentation (needs update after redesign)
- Decision: `.ai-team/decisions/inbox/forge-loginview-authorizeview-redesign.md`

**Note:** `LoginStatus` has the identical manual `AuthenticationStateProvider` pattern and should get the same AuthorizeView treatment as a follow-up.


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
<!-- Summarized 2026-03-06 by Scribe -- covers event handler audit through BWFC-first rewrite -->

### 2026-03-05 Architecture, Migration & Skills Summary

**Event handler audit:** ~50 EventCallbacks with bare names needed On-prefix aliases (non-breaking). Repeater has zero EventCallbacks (gap). GridView missing OnRowDataBound/OnRowCreated. Script correctly skips event transforms when components use On-prefix.

**ShoppingCart/BWFC preservation:** GridView supports all ShoppingCart features. L2 agents decomposed to raw HTML (anti-pattern). Added 5 mandatory BWFC preservation rules + `Test-BwfcControlPreservation` to bwfc-migrate.ps1. Propagated to migration-toolkit/skills/.

<!-- Summarized 2026-03-05 by Scribe -- covers event handler audit, LoginStatus redesign, Run 9 review -->

### Event Handler Audit, LoginStatus Redesign & Run 9 Review (2026-03-05 through 2026-03-06)

**Event handler audit:** Full coverage audit of all BWFC EventCallbacks vs Web Forms 4.8. ListView/DataGrid 100%, FormView 100% (wrong type on OnItemInserted), GridView 73%, Repeater 0% (critical gap), DataList 13%. 19 events lack bare-name aliases. 37/52 EventArgs missing Sender. SelectMethod fires only once. 7 P0 items all implemented by Cyclops, tested by Rogue (49 tests, 1519 total passing). 11 P1 + 7 P2 remain.

**Script gap review (6 gaps):** src=~/ not converted, script tags lost from master head, no BundleConfig detection, CSS link duplication, url(~/) in CSS, no infrastructure file flagging.

**BWFC-first skill rewrite:** All 4 skills + CHECKLIST + METHODOLOGY rewritten with mandatory banners, 110+ component inventory, anti-pattern tables, 9 BWFC verification items, Layer 2 MUST NOT list.

**Run 7 L2/3:** 6 storefront pages preserved (FormView single-item wrapper, ListView/GridView preserved). 26 code-behinds + 12 stubs. Build: 0 errors.

**LoginStatus redesign:** AuthorizeView wrap (follow-up to LoginView). 4 issues (manual auth state HIGH, LogoutAction type MEDIUM). ZERO breaking changes. 12 tests need bUnit AddTestAuthorization update.

**Run 9 BWFC review:** APPROVED -- 98.9% preservation (176/178). ShoppingCart GridView and LoginView/LoginStatus/LoginName all FIXED from Runs 6-8. 2 controls lost: ImageButton to img in ShoppingCart (CRITICAL, OnClick lost), HyperLink dropped in Manage (MINOR). ImageButton is a blind spot in Test-BwfcControlPreservation.

### Squad Places Enlistment & First Artifact (2026-03-05)

- Enlisted on Squad Places social network. Squad ID: `5b52c25e-9e05-4c03-a392-16c58a57b144`. API: `https://api.nicebeach-b92b0c14.eastus.azurecontainerapps.io`.
- Published first knowledge artifact — **"Component Emulation: Recreating 110+ Web Forms Controls as Blazor Components for Zero-Rewrite Migration"** (type: pattern, ID: `a8bb7b69-2bb2-4504-b050-69bdac24fa64`). Covers base class hierarchy, GridView HTML fidelity, BoundField/TemplateField, EventCallback dual pattern, and validation at scale. Tags: blazor, webforms, migration, component-emulation, dotnet, aspnet.
