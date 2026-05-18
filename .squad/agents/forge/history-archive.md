# Forge ΓÇö History Archive

> Original entries moved here by Scribe during history summarization. Never delete ΓÇö this preserves the full record.

## Archived 2026-03-04 (entries from 2026-02-10 through 2026-02-27)

### Core Context (2026-02-10 through 2026-02-27)

Reviewed 6 PRs in M1 (Calendar, FileUpload, ImageMap, PageService, ASCX CLI, VS Snippets). Calendar/FileUpload rejected, ImageMap/PageService approved, ASCX/Snippets shelved. M2 shipped (4148/53). M3: DetailsView + PasswordRecovery approved (50/53, 797 tests). Chart.js selected for Chart. Feature audit: DataBoundComponent<T> chain lacks style properties  recommended DataBoundStyledComponent<T>. SkinID bug (boolstring). Themes/Skins: CascadingValue ThemeProvider recommended.

**Milestone planning:** M7 "Control Depth & Navigation Overhaul" (51 WIs, ~138 gaps). M9 "Migration Fidelity & Hardening" (12 WIs, ~30 gaps). M12M14 "Migration Analysis Tool PoC" (13 WIs  `bwfc-migrate` CLI, regex parsing, Green/Yellow/Red scoring). M11M13 HTML audit milestones.

**Deployment pipeline:** Docker version via nbgv before build, injected via build-arg. NBGV must be stripped inside Docker. Secret-gated steps use env var indirection. Dual NuGet publishing (GitHub Packages + nuget.org). Azure webhook via curl with fallback.

**Key patterns:** Enum files in `Enums/` with explicit int values. Login Controls  BaseStyledComponent. Data-bound  DataBoundComponent<T>. Events use `On` prefix. Docs + samples ship with components. Feature branches  PR to upstream/dev. ComponentCatalog.cs links all sample pages. Theme core: nullable properties, case-insensitive keys, ApplySkin in OnParametersSet. Audit reports: `planning-docs/AUDIT-REPORT-M{N}.md`.

### Summary: HTML Audit Strategy and Milestones (2026-02-25 through 2026-02-26)

Evaluated Playwright-based HTML audit. Three tiers: Tier 1 (clean HTML, 6 controls), Tier 2 (complex data, 4 controls), Tier 3 (JS-heavy Menu/TreeView). Only ~25% sample coverage. M11M13 plan: M11 (infrastructure + Tier 1), M12 (Tier 2 data), M13 (Tier 3 + master report). Agent distribution: Forge strategy/review, Cyclops infra scripts, Jubilee samples, Colossus capture/comparison, Beast docs, Rogue tests.

### Summary: M15 HTML Fidelity Strategy (2026-02-26)

Post-PR #377: 132131 divergences, 1 exact match (Literal-3). Most divergences are sample data, not bugs. 5 remaining fixable bugs (BulletedList, LinkButton, Image, FileUpload, CheckBox). 12 work items, target 15 exact matches. ~1315 controls can achieve exact normalized match. New divergence candidates D-11 through D-14.

### Summary: Data Control Divergence Analysis (2026-02-26)

Line-by-line classification: DataList (110 lines), GridView (33 lines), ListView (182 lines), Repeater (64 lines). 90%+ sample parity issues. 5 genuine bugs (3 fixed in PR #377). 4 remaining: GridView UseAccessibleHeader default, GridView &nbsp; encoding, GridView thead vs tbody, DataList missing itemtype. Sample alignment alone would give ListView/Repeater exact matches. Calendar closest complex control at 73%.

≡ƒôî Team updates (2026-02-27): PRs from forkΓåÆupstream dev, close issues via PR refs only (Jeff). M17 AJAX controls shipped: 6 controls (Timer, UpdatePanel, UpdateProgress, ScriptManager stub, ScriptManagerProxy stub, Substitution), sample pages created, AJAX nav category + migration stub doc pattern established.

<!-- Archived 2026-03-06 by Scribe -->

### M17-M18 Audit & Themes Roadmap Summary (2026-02-28 through 2026-03-01)

**M17 gate review:** All 6 AJAX controls approved with notes. Property coverage: Timer 100%, UpdateProgress 100%, UpdatePanel 80%, ScriptManager 41% (appropriate for no-op stub), Substitution 100%. 5 follow-up fidelity fixes all resolved in PR #402 (EnablePartialRendering default, Scripts collection, CssClass rendering, display:block, ScriptReference properties). Key pattern: AJAX controls split into functional (Timer, UpdatePanel, UpdateProgress) and migration stubs (ScriptManager, ScriptManagerProxy, Substitution). Audit report: planning-docs/M17-CONTROL-AUDIT.md.

**Divergence registry D-11 to D-14:** D-11 GUID IDs (fix recommended), D-12 boolean attrs (intentional, normalize), D-13 Calendar day padding (fix recommended), D-14 Calendar style pass-through (fix progressively). File: planning-docs/DIVERGENCE-REGISTRY.md.

**Skins & Themes roadmap (#369):** 3 waves, 15 work items. Sub-component styles via Dictionary<string, TableItemStyle> on ControlSkin. ThemeMode enum (StyleSheetTheme vs Theme). 6 style container interfaces with ~41 sub-style slots. EnableTheming propagation via cascading bool. .skin parser as build-time source generator.

Team updates: M17 audit fixes resolved (PR #402), Skins dual docs (SkinsAndThemes.md + ThemesAndSkins.md), Normalizer pipeline codified (Issue #387).



<!-- Summarized 2026-03-02 by Scribe -- covers Build/Release audit through WingtipToys pipeline validation -->

### Build/Release & M22 Migration Summary (2026-03-02)

**Build/Version/Release audit:** version.json on main said 0.15 vs latest tag v0.16. Unified release.yml implemented (PR #408)  single workflow on release:published coordinates NuGet + Docker + docs + demos. NBGV 3.9.50, PackageId Fritz.BlazorWebFormsComponents, Docker ghcr.io/fritzandfriends/blazorwebformscomponents/serversidesamples. version.json now 0.17.0 (3-segment SemVer). Old nuget.yml/deploy-server-side.yml refactored to workflow_dispatch-only.

**M22 Copilot-Led Migration Showcase:** 12 work items, 4 waves. 57 controls (51 functional, 6 stubs). All 16 core demo controls ready. Use existing BeforeWebForms sample (6-8 pages). Separate .github/copilot-migration-instructions.md. Skins & AJAX Toolkit OUT. ListView EditItemTemplate bug (#406) IN.

**WingtipToys initial analysis:** 15+ pages, 22 controls, 100% BWFC coverage (all 4 "missing" controls already existed). Only blocking gap was FormView RenderOuterTable (resolved). Architecture: LayoutComponentBase, @page directives, EF Core + IDbContextFactory, scoped DI services, scaffolded Identity UI, HttpClient for PayPal, InteractiveServer. 36 work items, 5 phases.

**ASPX/ASCX tooling strategy:** 85+ syntax patterns from 33 WingtipToys files. Three-layer pipeline: Layer 1 (bwfc-migrate.ps1, ~40%, mechanical regex), Layer 2 (Copilot skill, ~45%, structural), Layer 3 (Copilot agent, ~15%, semantic). NOT building: standalone CLI, VS Code extension, Roslyn analyzer. SelectMethod->Items = #1 structural transform. Session->scoped DI = hardest semantic transform.

**ModelErrorMessage spec:** BaseStyledComponent (not BaseValidator). CascadingParameter EditContext. ModelStateKey->Field(key). Renders <span>, nothing when no error. Strips \x1F metadata. Validations/ folder. 29/29 WingtipToys coverage.

**WingtipToys pipeline validation (post-script):** Layer 1 ~70% markup: 147+ tag removals, 165+ runat removals, 35+ expression conversions. 18 data-binding expressions unconverted (<%#: inside GetRouteUrl/String.Format/Eval). 3 user-control prefixes survive. FormView RenderOuterTable working. ModelErrorMessage in 2 files. Actionable: (1) add <%#: regex, (2) strip uc:/friendlyUrls: prefixes, (3) SelectMethod->Items as #1 Layer 2 example, (4) Identity scaffold guidance. **18-26 hours total, 4-6 hours for demo subset.**

Team updates (2026-03-02): Unified release (PR #408), project reframed as migration acceleration system (Jeff), ModelErrorMessage docs (52 components, Beast), WingtipToys pipeline validated (4 ready, 21 skill, 8 architecture).

<!-- Summarized 2026-03-03 by Scribe -- covers CSS fidelity through WingtipToys schedule -->

### CSS Fidelity & WingtipToys Schedule Summary (2026-03-02 through 2026-03-03)

**WingtipToys CSS fidelity audit:** 7 visual differences ΓÇö wrong Bootstrap theme (Cerulean), single-column grid, missing Trucks category, Site.css not loaded, BoundField DataFormatString bug, bootstrap-theme gradients.

**M22 planning:** 12 work items, 4 waves. 57 controls ready. Skins & AJAX Toolkit OUT. ListView #406 IN.

**WingtipToys migration:** 15+ pages, 22 controls, 100% BWFC coverage. Architecture: LayoutComponentBase, EF Core, scoped DI, scaffolded Identity, InteractiveServer. 26 work items, 7 phases, critical path 1ΓåÆ2ΓåÆ3ΓåÆ4ΓåÆ7.

**ASPX/ASCX tooling:** Three-layer pipeline validated at ~70% markup. SelectMethodΓåÆItems = #1 structural transform.

**ModelErrorMessage:** BaseStyledComponent, CascadingParameter EditContext, 29/29 WingtipToys coverage.
<!-- Summarized 2026-03-04 by Scribe  covers migration toolkit design through restructure -->

### Migration Toolkit Design & Restructure Summary (2026-03-03)

**Toolkit design:** 9-document package at /migration-toolkit/. References existing scripts/skills by relative path  no duplication. Highest-value: copilot-instructions-template.md (drop-in for external projects). CHECKLIST.md fully net-new. Design doc: planning-docs/MIGRATION-TOOLKIT-DESIGN.md.

**Toolkit restructure:** Per Jeff's directive, moved 3 distributable skills from .github/skills/ to migration-toolkit/skills/. Copied bwfc-scan.ps1 and bwfc-migrate.ps1 into migration-toolkit/scripts/. 5 internal skills remain in .github/skills/. Key: distributable assets in migration-toolkit/, internal skills in .github/skills/.

**Key team updates (2026-03-02-03):** Unified release (PR #408), project reframed as migration system (Jeff), ModelErrorMessage docs (Beast), themes last directive (Jeff Fritz), migration toolkit pivoted to single SKILL.md then restructured.

 Team update (2026-03-04): PRs must target upstream FritzAndFriends/BlazorWebFormsComponents, not the fork  decided by Jeffrey T. Fritz
 Team update (2026-03-04): Migration test reports go in docs/migration-tests/{subfolder}/  decided by Jeffrey T. Fritz
 Team update (2026-03-04): Layer 1 benchmark baseline established  scan 0.9s, migrate 2.4s, 338 build errors (code-behind only)  decided by Cyclops
 Team update (2026-03-04): Layer 2+3 benchmark complete  ~9.4 min with Copilot, clean build, migration skills validated  decided by Cyclops

<!-- Summarized 2026-03-05 by Scribe -- covers Run 4 review through Run 5 analysis -->

### Run 4-5 Review & BWFC Capabilities Analysis (2026-03-04 through 2026-03-05)

**Run 4 review:** ConvertFrom-MasterPage = highest-impact enhancement (auto-generates MainLayout.razor). 289 transforms (+12 from new regexes/master page). 0 errors, 0 warnings, 11/11 features. CascadingAuthenticationState needed in Routes.razor for AuthorizeView.

**Run 5 BWFC analysis:** 95+ EventCallbacks across 30+ components matching Web Forms names. 3 of 4 top manual rewrites unnecessary -- BWFC already had ListView, FormView, GridView. 40% estimated reduction if scripts preserve BWFC data controls. Gaps: Repeater has zero EventCallbacks, GridView missing OnRowDataBound/OnRowCreated. SelectMethod TODOs need `Items=@data` guidance. Deliverables: analysis-and-recommendations.md, migration-standards SKILL.md, forge-run5-standards decision.

<!-- Summarized 2026-03-04 by Scribe ΓÇö covers Run 6 analysis and benchmark execution -->


## Archived 2026-03-12 (entries from 2026-03-11 through 2025-07-25)

### Run 18 Analysis & Improvement Recommendations (2026-03-11)

**Key findings from Run 18 report analysis:**

1. **`Test-UnconvertiblePage` is architecturally flawed** ΓÇö it matches patterns against markup only, causing false positives on UI references (PayPal image URLs, Checkout button IDs). Needs two-pass architecture: check code-behind for auth/session/payment patterns, markup only for structural features. This is the #1 script reliability issue. (P0)

2. **`[Parameter]` RouteData annotation bug is a line-swallowing regex issue** ΓÇö line 1209 of bwfc-migrate.ps1 replaces `[RouteData]` with a `[Parameter] // TODO...` string that consumes the rest of the line (parameter type + name). Causes 6 build errors in every project with route parameters. Fix: use line-aware regex that preserves same-line content and puts TODO on next line. (P0)

3. **BWFC generic type parameter naming is inconsistent** ΓÇö GridView/DataGrid/ListView use `ItemType`, BulletedList/DropDownList use `TItem`, DataBoundComponent uses `TItemType`. The migration script at line 1132 converts `ItemType` ΓåÆ `TItem` which is WRONG for GridView (whose generic param IS named `ItemType`). Major version standardization needed. (P2 ΓÇö works today, just confusing)

4. **Layer 2 doesn't exist as automation** ΓÇö No `bwfc-migrate-layer2.ps1` file. All Layer 2 work (boolean normalization, enum conversion, DI patterns, auth rewiring) is manual. ShoppingCart.razor required 6 manual fixes that are generalizable patterns.

5. **`Session\[` pattern checks markup, not code-behind** ΓÇö Web Forms `Session["key"]` appears in `.aspx.cs` files, not in markup. The current check against markup content misses actual session usage and could false-positive on inline code blocks.

**Decisions made:**
- Recommended two-pass `Test-UnconvertiblePage` with code-behind analysis + severity scoring (P0-1)
- Recommended immediate fix for `[Parameter]` line-swallowing bug (P0-2)
- Boolean normalization should be Layer 1 (P1-1), not library-level ΓÇö Blazor's `bool.Parse` is already case-insensitive
- Enum attribute conversion map needed for GridLines, RepeatDirection, etc. (P1-3)
- Full recommendations written to `.squad/decisions/inbox/forge-run18-improvements.md`


≡ƒôî Team update (2026-03-11): Run 18 improvement recommendations prioritized by Forge ΓÇö see decisions.md


 Team update (2026-03-11): Mandatory L1L2 migration pipeline  no code fixes between layers. Both layers must run in sequence.  decided by Jeffrey T. Fritz

 Team update (2026-03-11): All generic type params standardized to ItemType (not TItem/TItemType) across all BWFC data-bound components.  decided by Jeffrey T. Fritz


 Team update (2026-03-11): SelectMethod must be preserved in L1 script and skills  BWFC supports it natively via SelectHandler<ItemType> delegate. All validators exist in BWFC.


 Team update (2026-03-11): SelectMethod is now natively supported by BWFC. Old WingtipToys analysis sections 2.2 and 6.2 (SelectMethod as 'deliberate design decision') are SUPERSEDED. SelectMethod must be preserved as delegates.  decided by Jeffrey T. Fritz, Beast, Cyclops

### L2 Automation Analysis (2025-07-25)

**Key findings from L2 pattern analysis across Runs 17ΓÇô21 (WT + CU):**

1. **Top L2 time sink is enum/bool/unit string normalization** ΓÇö Blazor's Razor compiler rejects `GridLines="None"` and `Width="125px"` that Web Forms accepted. These cause build errors on every run. The BWFC library can absorb this gap with implicit string conversions, same as `Unit(int)` and `WebColor(string)` already do.

2. **6 automation opportunities identified** ΓÇö OPP-1 (EnumParameter<T> wrapper struct with implicit string conversion, P0/M), OPP-2 (Unit implicit string operator, P0/S), OPP-3 (Response.Redirect shim on WebFormsPageBase, P1/S), OPP-4 (Session state scoped dictionary, P1/M), OPP-5 (ViewState on WebFormsPageBase ΓÇö already exists on BaseWebFormsComponent, just needs page base, P2/S), OPP-6 (GetRouteUrl on WebFormsPageBase ΓÇö helper exists in Extensions/ but not accessible from pages, P2/S).

3. **Unit.cs has a broken explicit string operator** ΓÇö Line 443 only handles integer strings, throws on "125px". Should be replaced with implicit conversion delegating to `Unit.Parse()` which already handles all CSS unit strings correctly.

4. **BaseWebFormsComponent.ViewState exists (line 145) but WebFormsPageBase doesn't expose it** ΓÇö L2 unnecessarily converts ViewState to fields. Adding ViewState to WebFormsPageBase eliminates this entire fix category.

5. **L2 still needed for semantic transforms** ΓÇö Page_LoadΓåÆOnInitializedAsync, EF6ΓåÆEF Core, Identity migration, payment integration. These require application-level understanding and should stay as Copilot-assisted work.

6. **Full analysis written to `.squad/decisions/inbox/forge-l2-automation-analysis.md`** ΓÇö 6 OPPs prioritized with code sketches and risk assessment. Awaiting Jeff's decision on EnumParameter<T> public API change.


 Team update (2026-03-11): WebFormsPageBase now has Response.Redirect shim, ViewState dict, GetRouteUrl, and Unit implicit string conversion. L2 skills should note these patterns compile unchanged on @inherits WebFormsPageBase pages.  decided by Cyclops

### 2026-04-27: MasterPageContext Pattern Approval

**Task:** Review MasterPageContext implementation batch (Cyclops, Bishop, Beast, Jubilee, Rogue, Colossus) for MasterPage/Content/ContentPlaceHolder component bridge.

**Architecture approved:**
- CascadingValue<MasterPageContext> pattern enables discoverable parent-child hierarchy
- MasterPageContext wraps component tree, propagates MasterPage instance
- Content/ContentPlaceHolder locate parent via cascading parameter injection (no direct references)
- Supports multi-level nesting without tight coupling
- Mirrors Blazor AuthenticationState pattern for consistency

**Implementation verified:** 
- All three components (MasterPage, Content, ContentPlaceHolder) cooperate through MasterPageContext service
- Thread-safe context operations
- Dynamic registration/unregistration supported
- Unit tests cover context discovery, registration, parent resolution, nested hierarchies
- Transform coverage: Layer 1 migration toolkit recognizes MasterPage nesting, preserves structure
- Sample page demonstrates multi-level Content nesting
- Playwright integration tests passing (timing fixed for NetworkIdle wait)
- Build: 0 errors, 0 warnings

**Batch status:** ✅ APPROVED
- Cyclops implementation complete + unit tested
- Bishop CLI/PowerShell updates synced to migration-toolkit
- Beast documentation updated in docs/Migration/MasterPages.md
- Jubilee sample page + catalog entry complete
- Rogue transform coverage + validation passing
- Colossus Playwright tests passing with timing fix

**Decision:** No new team decision inbox items. All work aligned with existing architectural consensus (CascadingValue precedent from M10 AuthenticationState, MasterPageContext pattern aligns with Blazor conventions).

### Semantic Pattern Guardrails Review (2026-04-28)

- Reviewed the isolated semantic pattern subsystem entry points and the current L1/L2 assumptions in `src/BlazorWebFormsComponents.Cli\SemanticPatterns\` plus the existing master/content and auth transforms.
- `pattern-master-content-contracts` should normalize **after** mechanical transforms. The clean contract already exists in BWFC tests: master shell markup belongs in `ChildContent`, migrated page `<Content ...>` registrations belong in `ChildComponents`, and placeholder IDs must remain exact (`src/BlazorWebFormsComponents.Test\MasterPage\ContentRelationshipTests.razor`).
- Reject any implementation that collapses named master sections to `@Body` or a single `HeadContent` bucket. `MasterPageTransform`/`ContentWrapperTransform` preserve `ContentPlaceHolderID` mechanically, but the semantic pass should split shell vs section registrars instead of relying on inline `@ChildContent` in the shell (`src/BlazorWebFormsComponents.Cli\Transforms\Markup\MasterPageTransform.cs`, `...ContentWrapperTransform.cs`).
- `pattern-account-pages` must treat real auth flows as HTTP-bound/manual work, not pretend Blazor event handlers can replace OWIN/Identity response behavior. Non-negotiable manual boundaries include cookie sign-in/out, external auth challenge/callback, 2FA, password reset, login removal, and any `Response.End` / `Authentication.Challenge` path (`samples\WingtipToys\WingtipToys\Account\Login.aspx.cs`, `...Manage.aspx.cs`, `...ManageLogins.aspx.cs`, `...OpenAuthProviders.ascx.cs`, `...TwoFactorAuthenticationSignIn.aspx.cs`).
- `pattern-query-details` is only safe for read-only query/route driven detail pages where the `SelectMethod` is pure filtering with no side effects. Preserve query parameter names and route/query precedence; do not silently rewrite multi-entry-point pages to a single `id` contract (`samples\WingtipToys\WingtipToys\ProductDetails.aspx.cs`, `...ProductList.aspx.cs`).
- `pattern-action-pages` is only safe for redirect-only action pages with no visible UI and deterministic query-driven side effects. Reject implementations that turn pages like `AddToCart.aspx` into inert informational Razor pages; action semantics must still run on navigation or be left as an explicit TODO/manual handler (`samples\WingtipToys\WingtipToys\AddToCart.aspx.cs`, `samples\AfterWingtipToys\AddToCart.razor`).

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
- Decision: `.squad/decisions/inbox/forge-loginview-authorizeview-redesign.md`

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

**Decision:** `.squad/decisions/inbox/forge-cycle1-analysis.md` — 3 P0, 4 P1, 4 P2, 3 P3 items. Cycle 1 targets: P0-1 (ItemType fix), P0-2 (smart stubs), P0-3 (base class stripping), P1-1 (validator params), P1-4 (ImageButton warning). All P0/P1 assigned to Bishop except ImageButton verification (Cyclops).

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

**Decision:** `.squad/decisions/inbox/forge-run10-preservation.md` — NEEDS WORK verdict. P0: Fix CheckoutReview DetailsView + ShoppingCart ImageButton. P1: Fix ManageLogins stub. P2: Add ImageButton to Test-BwfcControlPreservation.

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

**Decision:** `.squad/decisions/inbox/forge-cycle2-analysis.md` — 3 P0 (CheckoutReview DetailsView +9, ImageButton +1, ManageLogins +3), 4 P1 (TextMode enum, ValidatorDisplay enum, boolean normalization, ControlToValidate verify), 5 P2 (AdminPage promotion, About Title, ImageButton detection escalation, expression cleanup, GridLines enum), 4 P3 (Identity, Checkout flow, cart persistence, Visible attributes). Cycle 2 target: preservation ≥98%, build attempts = 1.

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
- **Decision:** `.squad/decisions/inbox/forge-cycle3-analysis.md` -- 3-sprint plan (auth foundation, script improvements, account pages polish).

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
  - Decision inbox: `.squad/decisions/inbox/forge-docs-reorganization.md`
  - Detailed proposal: `planning-docs/proposals/DOCS-REORGANIZATION.md`

📌 Team update (2026-03-06): migration-toolkit is end-user distributable; migration skills belong in migration-toolkit/skills/ not .squad/skills/ — decided by Jeffrey T. Fritz

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

**Decision:** `.squad/decisions/inbox/forge-bwfc-improvements.md`


 Team update (2026-03-06): WebFormsPageBase is the canonical base class for all migrated pages (not ComponentBase). All agents must use WebFormsPageBase  decided by Jeffrey T. Fritz
 Team update (2026-03-06): LoginView is a native BWFC component  do NOT convert to AuthorizeView. Strip asp: prefix only  decided by Jeffrey T. Fritz

### CLI Gap Analysis — webforms-to-blazor Tool (2026-07-25)

**Task:** Comprehensive gap analysis of `src/BlazorWebFormsComponents.Cli` migration tool.

**Current state (14 markup + 12 code-behind transforms):**
- Markup: AspPrefix, AjaxToolkit, Attributes, Content, DataSource, Events, Expressions, Form, LoginView, MasterPage, SelectMethod, Template, URL transforms
- Code-behind: BaseClass, DataBind, EventHandler, GetRouteUrl, IsPostBack, PageLifecycle, Response, Session, Todo, UrlCleanup, Using, ViewState transforms
- Scaffolding: .csproj, Program.cs, _Imports.razor, App.razor, Routes.razor, GlobalUsings.cs, WebFormsShims.cs

**Critical gaps identified (6 — will break real migrations):**
1. **FindControl()** — no transform → compile error (every Web Forms app uses this)
2. **ClientScript / RegisterStartupScript** — no transform → runtime failure
3. **FormsAuthentication** — mentioned in shims but no actual transform
4. **Membership/Roles** — no transform → compile error
5. **VB.NET code-behind** — detected but C# regex patterns fail silently
6. **User controls (.ascx)** — prefix stripped but no cross-file correlation, no tests

**High-value additions (8 — cover most real-world apps):**
1. ScriptManager code-behind patterns (GetCurrent, RegisterAsyncPostBackControl)
2. Validation group handling (Page.Validate, Page.IsValid)
3. GridView/ListView code-behind patterns (DataKeys, EditIndex)
4. Request object patterns (QueryString, Form, Cookies)
5. Server.MapPath conversion
6. Global.asax pattern extraction
7. Enum attribute conversions (TextMode, Display, GridLines → strong-typed)
8. Static file url(~/) transforms

**Quality issues in existing transforms:**
1. **ItemType bug** — blindly converts all to TItem, but GridView/ListView/FormView use ItemType
2. **LoginViewTransform conflicts with BWFC component** — should be REMOVED per Jeff's directive
3. IsPostBack edge cases (nested, else-if) produce unparseable output
4. DataBindTransform.InjectItemsAttributes() may not be called from pipeline

**Scaffold gaps:**
- Missing MainLayout.razor (Routes.razor references it)
- Missing `@using BlazorWebFormsComponents.Enums` in _Imports.razor
- Missing `AddHttpContextAccessor()` in Program.cs

**Test coverage gaps (32 current → 45+ needed):**
- TC33: .ascx user control (P0)
- TC35: FindControl patterns (P0)
- TC36-38: ClientScript, Membership, FormsAuth (P1)
- TC47: VB.NET code-behind (P1)
- TC48: Enum string attributes (P1)

**Priority fixes:**
- P0: Remove LoginViewTransform, fix ItemType/TItem logic, add .ascx test
- P1: EnumAttributeTransform, FindControlTransform, RequestAccessTransform, FormsAuthTransform, ScriptManagerCodeBehindTransform, scaffold fixes

**Estimated coverage improvement:** 70% → 88% L1 mechanical coverage after P0+P1+P2

**Decision:** `.squad/decisions/inbox/forge-cli-gap-analysis.md`


