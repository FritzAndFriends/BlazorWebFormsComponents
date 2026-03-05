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

### Comprehensive Event Handler & SelectMethod Audit (2026-03-06)

**Task:** Deep audit of all BWFC EventCallbacks against Web Forms 4.8 originals, triggered by Run 8 benchmark "15 instances requiring EventCallback conversion".

**Key findings — Event coverage by control:**
- **ListView:** 100% (18/18 events) — best coverage
- **DataGrid:** 100% (10/10)
- **FormView:** 100% (12/12), but 6 CRUD events On-prefix only, `OnItemInserted` has wrong type (`FormViewInsertEventArgs` instead of `FormViewInsertedEventArgs`)
- **DetailsView:** 92% (11/12), missing `ItemCreated`
- **GridView:** 73% (11/15), missing PageIndexChanging, RowUpdated, RowDeleted, RowCreated, RowDataBound; SelectedIndexChanged uses `int` not `EventArgs`
- **Repeater:** **0%** (0/3) — ZERO EventCallbacks, critical gap
- **DataList:** **13%** (1/8) — only `OnItemDataBound`
- **ButtonBaseComponent:** `OnClick` uses `MouseEventArgs` instead of `EventArgs` (migration friction)
- Input controls (TextBox, CheckBox, RadioButton, DropDownList, etc.): All correct with dual-callback pattern
- Menu: 100%, TreeView: 83% (missing TreeNodePopulate)

**On-prefix consistency:** 19 events have On-prefix only (no bare-name alias). Worst offenders: FormView (6), DataGrid (5), TreeView (4).

**Sender property:** Only 15 of 52 EventArgs classes have `Sender`. 37 need it added for `(object sender, e)` migration pattern.

**SelectMethod issues:** Fires once only (firstRender guard), `out` param blocks lambdas, sync only, hard-coded paging params. Proposed fix: move to OnParametersSetAsync, add lambda-friendly overload, add async variant.

**InsertMethod/UpdateMethod/DeleteMethod:** Not implemented anywhere. Proposed design: `Action<T>` and `Func<T, Task>` parameters that auto-invoke after CRUD events and re-invoke SelectMethod to refresh.

**Prioritized 7 P0 items** (migration-blocking): Repeater events, DataList events, GridView RowDataBound/RowCreated, DetailsView ItemCreated, FormView wrong type, SelectMethod one-shot fix. **11 P1 items** (quality): Button MouseEventArgs, Sender on all EventArgs, missing GridView events, bare-name aliases, async SelectMethod. **7 P2 items** (polish): CRUD methods, remaining aliases, script transform.

**Decision document:** `.ai-team/decisions/inbox/forge-event-handler-selectmethod-audit.md`

**Run 7 L2/3:** 6 storefront pages. FormView with single-item list wrapper, ListView/GridView preserved. 26 code-behinds + 12 .razor stubs. Build: 0 errors. L1 residuals in ItemTemplate sections.

**Script gap review (6 gaps):** `src="~/"` not converted, `<script>` tags lost from master `<head>`, no BundleConfig detection, CSS link duplication, `url('~/')` in CSS not converted, no infrastructure file flagging. Decision: forge-script-gap-review.md.

**BWFC-first skill rewrite:** All 4 skills + CHECKLIST.md + METHODOLOGY.md rewritten. 🚫 MANDATORY banners, 110+ component inventory, LoginView/LoginStatus flagged, anti-pattern tables, 9 new BWFC verification items, Layer 2 "MUST NOT" list. Root cause: Runs 6-8 L2 agents replaced BWFC with plain HTML.

### LoginStatus → AuthorizeView Redesign Analysis (2026-03-06)

**Task:** Analyze LoginStatus for AuthorizeView-based redesign (follow-up to LoginView).

**Key findings:** Inherits CompositeControl→WebControl (HAS style properties, unlike LoginView). Renders `<a>` or `<input type="image">` with no wrapper. 4 issues found: manual AuthenticationStateProvider (HIGH), LogoutAction as abstract class vs enum (MEDIUM, separate PR), misleading LoginPageUrl comment (LOW), null-check missing on LoginPageUrl (LOW). Base class, HTML rendering, events, and style application all correct.

**Architecture:** Wrap markup in `<AuthorizeView>`. ZERO breaking changes (unlike LoginView's 4). All 12 tests need mechanical update to bUnit AddTestAuthorization().

**Decision document:** `.ai-team/decisions/inbox/forge-loginstatus-authorizeview-redesign.md`


 Team update (2026-03-06): Event Handler Fidelity Audit merged to decisions.md. All 7 P0 items implemented by Cyclops and tested by Rogue (49 tests, all 1519 pass). P1/P2 items remain in the prioritized backlog.  audit by Forge

