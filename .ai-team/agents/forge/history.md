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

### Cycle 1 Analysis — Run 9 Deep Dive (2026-03-06)

**Task:** Analyzed Run 9 benchmark report + actual migrated output to produce prioritized Cycle 1 fix list.

**Key findings:**
1. **ItemType→TItem bug confirmed at script line 862-867.** Only DropDownList/ListBox/CheckBoxList/RadioButtonList/BulletedList use `TItem`; GridView/ListView/FormView/DetailsView all use `ItemType`. Script blindly converts all. This is the #1 recurring build failure.
2. **Stub mechanism is overly aggressive.** `Test-UnconvertiblePage` path-based checks (`Account/`, `Checkout/`) stub entire pages including their BWFC markup. But Run 9 output proves Layer 2 recreated Login, Register, Confirm, Manage, CheckoutReview with full BWFC controls from scratch — wasting ~30 min of Layer 2 time on work Layer 1 could have done mechanically.
3. **Code-behind copy doesn't strip Web Forms base classes.** `: Page`, `: System.Web.UI.Page` in copied code-behinds conflicts with `@inherits WebFormsPageBase` from _Imports.razor → CS0263 every run.
4. **Validator type params not auto-injected.** 26 validators in WingtipToys needed manual `Type="string"` / `InputType="string"` addition. Deterministic transform.
5. **GridView editing support EXISTS** — EditIndex, OnRowEditing, OnRowUpdating confirmed in GridView.razor.cs. AdminPage gap is code-behind implementation, not component capability.
6. **Manage.razor HyperLinks are present** — the "HyperLink dropped" warning from Run 9 review may be stale.
7. **OpenAuthProviders** is a WingtipToys user control (.ascx), not a missing BWFC component. Correctly stubbed.

**Decision:** `.ai-team/decisions/inbox/forge-cycle1-analysis.md` — 3 P0, 4 P1, 4 P2, 3 P3 items. Cycle 1 targets: P0-1 (ItemType fix), P0-2 (smart stubs), P0-3 (base class stripping), P1-1 (validator params), P1-4 (ImageButton warning). All P0/P1 assigned to Bishop except ImageButton verification (Cyclops).

### Run 10 BWFC Preservation Review (2026-03-06)

**Task:** Full BWFC preservation audit of Run 10 migration output against original WingtipToys .aspx source.

**Key findings:**
1. **Overall: 92.7% (164/177) — NEEDS WORK.** Below the 95% approval threshold. Two pages account for all 13 lost controls.
2. **P0-2 fix largely successful:** 12 of 14 Account/Checkout pages now contain full BWFC markup (were empty stubs in Run 9). 113 controls preserved across these pages. Login, Register, Confirm, Manage, ManagePassword, Forgot, AddPhoneNumber, ResetPassword, TwoFactorAuth, VerifyPhoneNumber, RegisterExternalLogin, ResetPasswordConfirmation, CheckoutComplete all at 100%.
3. **ManageLogins still a stub (0/3, 0%):** Source has PlaceHolder + ListView + Button. Output is a dismissive text-only message. P0-2 missed this page entirely.
4. **CheckoutReview partial (6/15, 40%):** GridView + 4 BoundFields + Button preserved, but the entire DetailsView shipping address section (DetailsView + TemplateField + 7 Labels) is missing. Critical checkout UX gap.
5. **ImageButton→img persists in ShoppingCart (12/13, 92%):** PayPal checkout button still flattened to `<img>`, losing OnClick handler. Carried from Run 9. Test-BwfcControlPreservation still does not detect this pattern.
6. **AdminPage perfect (21/21):** Most complex non-data page fully preserved — all Labels, DropDownLists, TextBoxes, validators, FileUpload, Buttons.
7. **ManagePassword perfect (24/24):** Most control-dense Account page fully preserved including ModelErrorMessage, dual PlaceHolder sections, 5 validators.
8. **Run 9→10 rate regression is misleading:** Run 9 reported 98.9% because stub pages were N/A. Run 10 counts their source controls in the denominator because P0-2 attempted them. Fixing CheckoutReview DetailsView section alone brings rate to 97.7%.

**Decision:** `.ai-team/decisions/inbox/forge-run10-preservation.md` — NEEDS WORK verdict. P0: Fix CheckoutReview DetailsView + ShoppingCart ImageButton. P1: Fix ManageLogins stub. P2: Add ImageButton to Test-BwfcControlPreservation.

### Cycle 2 Analysis — Run 10 Deep Dive (2026-03-06)

**Task:** Analyzed Run 10 benchmark report, preservation review, and actual migrated output to produce prioritized Cycle 2 fix list.

**Key findings:**
1. **CheckoutReview DetailsView omission is a Layer 2 problem, not a component gap.** The BWFC DetailsView component exists at `src/BlazorWebFormsComponents/DetailsView.razor` with full support for AutoGenerateRows, GridLines, CellPadding, Fields, TemplateField. Layer 2 simply didn't include the section (lines 14-36 of source) in the output.
2. **ImageButton→img is also a Layer 2 problem.** The BWFC ImageButton component exists at `src/BlazorWebFormsComponents/ImageButton.razor.cs` with ImageUrl, AlternateText, OnClick. Layer 2 replaces it with `<img>` — likely because the checkout code-behind is complex. The preservation test already detects this (lines 1052-1060) but only warns, doesn't fail.
3. **ManageLogins was stubbed due to a build error** on `@context.LoginProvider` (the ItemType `Microsoft.AspNet.Identity.UserLoginInfo` doesn't exist in .NET 10). Layer 2 gave up and replaced with text instead of creating a stub model class.
4. **Enum conversions are the #1 remaining build cause.** 16 TextMode, 15 Display, and 3+ GridLines instances all need deterministic string→type conversion. A single `Convert-EnumAttributes` function with a mapping table would eliminate all of them.
5. **ControlToValidate is already stripped** in Run 10 output (0 instances found), but unclear if Layer 1 or Layer 2 does this — needs verification.
6. **About page drops `<%: Title %>` dynamic binding** — hard-codes `<h2>About</h2>` instead of using `@Title` from WebFormsPageBase.
7. **AdminPage (21/21 controls) is ready for functional promotion** — just needs code-behind CRUD implementation.

**Decision:** `.ai-team/decisions/inbox/forge-cycle2-analysis.md` — 3 P0 (CheckoutReview DetailsView +9, ImageButton +1, ManageLogins +3), 4 P1 (TextMode enum, ValidatorDisplay enum, boolean normalization, ControlToValidate verify), 5 P2 (AdminPage promotion, About Title, ImageButton detection escalation, expression cleanup, GridLines enum), 4 P3 (Identity, Checkout flow, cart persistence, Visible attributes). Cycle 2 target: preservation ≥98%, build attempts = 1.

 Team update (2026-03-06): Scribe consolidated Layer 1 bugs decisions (old ItemType/validator/base-class entries merged into single block). Run 10 preservation review, Cycle 1 fix list, smart stubs, and enum conversion P2 proposal all merged to decisions.md. Inbox cleared.  decided by Scribe


### Run 11 BWFC Preservation Review (2025-07-25)

- **Task:** Full BWFC preservation audit of Run 11 migration output against original WingtipToys source.
- **Result:** 98.9% (176/178 adjusted) -- APPROVED. All 3 P0 gaps from Run 10 CLOSED:
  - CheckoutReview DetailsView -- fully preserved with OrderShipInfo stub model (+9 controls)
  - ShoppingCart ImageButton -- preserved as `<ImageButton>` with OnClick navigation (+1 control)
  - ManageLogins -- full ListView + Button + PlaceHolder preserved with UserLoginInfo stub model (+3 controls)
- **Remaining:** 1 HyperLink dropped in Manage.razor (conditional Visible pattern). 2 ModelErrorMessage (non-standard, no BWFC equivalent expected).
- **Build:** 0 errors, 70 warnings (all BL0007 from BWFC library, not Run 11).

### Cycle 3 Priority Analysis (2025-07-25)

- **Task:** Produced Cycle 3 prioritized fix list based on Run 11 review and Jeff's Login/Register directive.
- **Key shift:** Markup fidelity now at ~99%. Focus moves to functional completeness -- code-behinds are stubs.
- **P0:** Login.razor + Register.razor functional code-behinds, MockAuthService, MockAuthenticationStateProvider.
- **P1:** Manage.razor code-behind, ManageLogins code-behind, Visible to @if conversion, enum gaps (LogoutAction, BorderStyle, WebColor).
- **P2:** bwfc-scan.ps1 parse error, hex color escaping, remaining account page stubs, ModelErrorMessage equivalent.
- **Decision:** `.ai-team/decisions/inbox/forge-cycle3-analysis.md` -- 3-sprint plan (auth foundation, script improvements, account pages polish).

### Documentation Reorganization Proposal (2026-03-06)

- **Task:** Audited docs/, planning-docs/, and samples/ to propose consolidation of end-user vs developer documentation.
- **Key findings:**
  - `docs/` mixes user guides with internal benchmark reports (Run9-Run11) and 6 migration-tests/ folders
  - `samples/` has 6 `Run*WingtipToys/` test run folders mixed with demo samples
  - `mkdocs.yml` nav publishes internal content to end users (Migration Tests section, benchmark links)
- **Proposed structure:**
  - `docs/` — End-user only (55+ component docs, migration guides, utility features)
  - `dev-docs/` — New folder for contributor docs (benchmarks/, migration-tests/, migration-runs/, screenshots/)
  - `planning-docs/` — Unchanged (analysis/, components/, milestones/, reports/, proposals/)
  - `samples/` — Clean demos only (AfterBlazor*, BeforeWebForms, WingtipToys targets)
- **Key decisions in proposal:**
  1. Create `dev-docs/` as new top-level internal documentation folder
  2. Move `Run*WingtipToys/` from samples/ to `dev-docs/migration-runs/`
  3. Remove benchmark reports and migration-tests from mkdocs.yml nav
  4. Consolidate screenshots from planning-docs/ to dev-docs/
- **Deliverables:**
  - Decision inbox: `.ai-team/decisions/inbox/forge-docs-reorganization.md`
  - Detailed proposal: `planning-docs/proposals/DOCS-REORGANIZATION.md`

📌 Team update (2026-03-06): migration-toolkit is end-user distributable; migration skills belong in migration-toolkit/skills/ not .ai-team/skills/ — decided by Jeffrey T. Fritz

 Team update (2026-03-06): Layer 2 conventions established  Button OnClick uses EventArgs (not MouseEventArgs), code-behind class names must match .razor filenames exactly, use EF Core wildcard versions for .NET 10, CartStateService replaces Session, GridView needs explicit TItem  decided by Cyclops


 Team update (2026-03-06): bwfc-migrate.ps1 uses -Path and -Output params (not -SourcePath/-DestinationPath). ProjectName is auto-detected  decided by Bishop

### Run 7 BWFC Gap Analysis & Improvement Recommendations (2026-03-06)

**Task:** Analyzed Run 7 benchmark report + AfterWingtipToys migration output + current BWFC library to produce actionable BWFC improvement recommendations.

**Key BWFC gaps identified:**
1. **WebFormsPageBase completely unused in migration output.** Every AfterWingtipToys code-behind inherits `ComponentBase` explicitly because: (a) `_Imports.razor` scaffold doesn't include `@inherits WebFormsPageBase`, (b) code-behind stripping removes `: Page` but doesn't replace with `: WebFormsPageBase`, (c) Program.cs scaffold missing `AddHttpContextAccessor()`.
2. **No Response.Redirect shim.** NavigationManager.NavigateTo injected in 5+ code-behinds — could be provided via `WebFormsPageBase.Response.Redirect()`.
3. **No Request.QueryString shim.** Manual `[SupplyParameterFromQuery]` on every page with query params.
4. **`@using BlazorWebFormsComponents.Enums` missing from scaffold** — but required by the script's own enum conversions.
5. **LoginView→AuthorizeView conversion still in script** despite native BWFC LoginView component existing.
6. **No Session state service** — CartStateService manually created every migration.

**Key recommendations made (14 items, ranked by impact):**
- **Immediate (S1-S4):** Script-only changes — `@inherits WebFormsPageBase` in _Imports, code-behind base class replacement, `AddHttpContextAccessor()` in Program.cs, Enums using directive. Collectively eliminate ~35 files of manual Layer 2 work.
- **Next (L1, S5, S6):** Response.Redirect shim on WebFormsPageBase, Page_Load→OnInitializedAsync rename, cookie auth default in scaffold.
- **Backlog (L2-L6, S7-S8):** Request.QueryString shim, DataBind no-op, form-submit.js, Session service, LoginView fix, src=~/ URLs.

**Patterns observed in AfterWingtipToys:**
- Title handling is ad-hoc (`private string Title => "Home"`) because pages don't inherit WebFormsPageBase.
- Auth pages use raw HTML forms for POST submission (correct pattern for cookie auth), not BWFC components.
- 3 service classes manually created; 2 already scaffolded by script, 1 (CartStateService) generalizable.
- Estimated 40% Layer 2 reduction (~10 hours saved on WingtipToys-scale) if all recommendations implemented.

**Decision:** `.ai-team/decisions/inbox/forge-bwfc-improvements.md`


 Team update (2026-03-06): WebFormsPageBase is the canonical base class for all migrated pages (not ComponentBase). All agents must use WebFormsPageBase  decided by Jeffrey T. Fritz
 Team update (2026-03-06): LoginView is a native BWFC component  do NOT convert to AuthorizeView. Strip asp: prefix only  decided by Jeffrey T. Fritz
