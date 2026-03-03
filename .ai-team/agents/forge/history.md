# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents  Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!--  Summarized 2026-02-27 by Scribe  covers M1M16 -->

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

 Team update (2026-02-27): Branching workflow directive  feature PRs from personal fork to upstream dev, only devmain on upstream  decided by Jeffrey T. Fritz

 Team update (2026-02-27): Issues must be closed via PR references using 'Closes #N' syntax, no manual closures  decided by Jeffrey T. Fritz


 Team update (2026-02-27): AJAX Controls nav category created; migration stub doc pattern for no-op components; Substitution moved from deferred to implemented; UpdateProgress uses explicit state pattern  decided by Beast


 Team update (2026-02-27): M17 AJAX controls implemented  ScriptManager/Proxy are no-op stubs, Timer shadows Enabled, UpdatePanel uses ChildContent, UpdateProgress renders hidden, Substitution uses Func callback, new AJAX/Migration Helper categories  decided by Cyclops


 Team update (2026-02-27): M17 sample pages created for Timer, UpdatePanel, UpdateProgress, ScriptManager, Substitution. Default.razor filenames. ComponentCatalog already populated  decided by Jubilee

<!-- Summarized 2026-03-02 by Scribe -- covers M17 gate review through Themes roadmap -->

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

### WingtipToys CSS Fidelity Audit (2026-03-02)

**7 visual differences found** between original WingtipToys (:5200) and migrated Blazor (:5201):

1. **Navbar color (CRITICAL):** Original uses Bootswatch Cerulean v3.2.0 (`#033c73` dark blue navbar, `#ffffff` white link text). Migrated loads stock Bootstrap 3.4.1 from CDN (`#222222` dark gray navbar, `#999999` gray link text). Root cause: `App.razor` references CDN bootstrap instead of the custom `Content/bootstrap.css`. File: `samples/WingtipToys/WingtipToys/Content/bootstrap.css` line 1 = `bootswatch v3.2.0`, line 4177 = `background-color: #033c73`.

2. **Heading colors:** Bootswatch Cerulean sets `h1-h6 { color: #317eac }` (line 995). Stock Bootstrap uses `#333333`. Visible on "Welcome.", "Shopping Cart", "Products" headings.

3. **Link colors:** Cerulean: `a { color: #2fa4e7 }` (line 906). Stock Bootstrap 3.4.1: `#337ab7`. Affects category menu, product names, "Add To Cart" links.

4. **Product grid layout (CRITICAL):** Original `ProductList.aspx` uses `GroupItemCount="4"` + `GroupTemplate` + `LayoutTemplate` for 4-column grid. Migrated `ProductList.razor` omits all three — products render in single column. BWFC ListView supports GroupItemCount (lines 80-134 of `ListView.razor`), so this is a migration omission.

5. **Missing "Trucks" category:** Original has 5 categories (data-driven via ListView). `MainLayout.razor` hardcodes only 4 (Cars, Planes, Boats, Rockets — missing Trucks).

6. **Site.css not loaded:** `Content/Site.css` exists in migrated project but is not referenced in `App.razor`. Original bundles it via `Bundle.config`. Missing `body { padding-top: 50px }` and `.body-content` padding rules.

7. **BoundField DataFormatString bug:** `BoundField.razor.cs` line 48: `string.Format(DataFormatString, obj?.ToString())` converts obj to string BEFORE formatting, so `{0:c}` currency format is lost. Cart "Price (each)" shows "15.95" instead of "$15.95". Fix: use `obj` not `obj?.ToString()`.

**Key files:** Original CSS: `samples/WingtipToys/WingtipToys/Content/bootstrap.css` (Bootswatch Cerulean). Migrated layout: `samples/AfterWingtipToys/Components/App.razor` (CDN bootstrap ref). Migrated content: `samples/AfterWingtipToys/Components/Layout/MainLayout.razor`. BWFC bug: `src/BlazorWebFormsComponents/BoundField.razor.cs:48`.

 Team update (2026-03-03): Original WingtipToys build/run config documented (LocalDB, NBGV isolation, NuGet restore, IIS Express port 5200)  decided by Cyclops
**Key infrastructure details learned:**
- NBGV 3.9.50 in `Directory.Build.props`, applied to all projects
- NuGet PackageId: `Fritz.BlazorWebFormsComponents`
- Docker image: `ghcr.io/fritzandfriends/blazorwebformscomponents/serversidesamples`
- Dockerfile strips NBGV via `sed`, accepts `VERSION` build-arg, passes `-p:Version=$VERSION`
- `version.json` `publicReleaseRefSpec` includes main, master, v-branches, and v-tags
- `version.json` `firstUnstableTag` is `preview` — dev branch gets `-preview.X` suffix
- MkDocs uses Docker-based build (`docs/Dockerfile`), deploys to gh-pages branch via `crazy-max/ghaction-github-pages@v2.1.1`
- `docs.yml` deploy guard: `github.event_name != 'pull_request' && (endsWith(github.ref, 'main') || steps.prepare.outputs.release == 'true')`

 Team update (2026-03-02): Unified release process implemented  single release.yml triggered by GitHub Release publication coordinates all artifacts (NuGet, Docker, docs, demos). version.json now uses 3-segment SemVer (0.17.0). Existing nuget.yml and deploy-server-side.yml are workflow_dispatch-only escape hatches. PR #408  decided by Forge (audit), Cyclops (implementation)

### Summary: M22 Planning — Copilot-Led Migration Showcase (2026-03-02)

**By:** Forge
**What:** Comprehensive plan for M22 strategic milestone. 12 work items across 4 waves. Targets a live demo where Jeff migrates a Web Forms app to Blazor using Copilot + BWFC in under 30 minutes.

**Component inventory assessment:** 57 total controls (51 functional, 6 stubs/deferred). ~35 are Tier 1 demo-ready with high migration fidelity. All 16 core demo controls (Button, TextBox, GridView, validators, etc.) are ready — no blocking component work needed.

**Key scope decisions:**
- Use existing BeforeWebForms sample (curate 6-8 pages, not build from scratch)
- Create separate `.github/copilot-migration-instructions.md` for migration guidance (distinct from library dev instructions)
- Skins & Themes (#369) OUT — CssClass styling sufficient for demo
- AJAX Toolkit extenders (#297) OUT — not core WF controls
- ListView CRUD (#356) partially in — 4 essential events only if demo needs them
- ListView EditItemTemplate bug (#406) IN — real bug, fix regardless
- New migration walkthrough doc, before/after comparison, demo script, integration test

**Files:** `planning-docs/MILESTONE22-COPILOT-MIGRATION-SHOWCASE.md`, `.ai-team/decisions/inbox/forge-m22-copilot-migration-plan.md`


 Team update (2026-03-02): M22 Copilot-Led Migration Showcase planned  decided by Forge

### Summary: WingtipToys Migration Analysis (2026-03-02)

**By:** Forge
**What:** Comprehensive page-by-page analysis of WingtipToys Web Forms demo for migration to Blazor Server Side. Full analysis saved to `.ai-team/decisions/inbox/forge-wingtiptoys-migration-plan.md`.

**WingtipToys app structure:**
- 15+ pages including Site.Master, product browsing (list/details), shopping cart, checkout flow (5 pages with PayPal NVP integration), admin (add/remove products with file upload and validation), and 14 Account/* identity pages
- 22 distinct Web Forms controls used: ListView, FormView, GridView, DetailsView, BoundField, TemplateField, Label, TextBox, Button, ImageButton, Image, CheckBox, DropDownList, FileUpload, RequiredFieldValidator, RegularExpressionValidator, LoginView, LoginStatus, ContentPlaceHolder/Content, ScriptManager, PlaceHolder, Panel
- Models: Product, Category, CartItem, Order, OrderDetail with EF6 ProductContext
- Logic: ShoppingCartActions (Session-based cart), PayPalFunctions (NVP API), RoleActions, AddProducts
- Custom routes: `Category/{categoryName}` → ProductList, `Product/{productName}` → ProductDetails

**CRITICAL CORRECTION — All controls exist in library:**
Jeff's initial catalog listed RequiredFieldValidator, RegularExpressionValidator, LoginView, and LoginStatus as "missing" — **all four already exist** in our library (Validations/ and LoginControls/ directories). The component library has 100% control coverage for WingtipToys.

**Component library gaps identified:**
1. **BLOCKING: FormView RenderOuterTable** — ProductDetails.aspx sets `RenderOuterTable="false"` but our FormView always renders a wrapping `<table>`. Must add `RenderOuterTable` parameter (property exists on ChangePassword/CreateUserWizard/PasswordRecovery already, but not FormView). LOW-MEDIUM fix effort.
2. **SelectMethod/ItemType pattern** — Web Forms uses `SelectMethod="GetProducts" ItemType="Product"`, BWFC uses `Items="@products" TItem="Product"`. Deliberate design decision, not a bug. Mechanical pattern change.
3. **GridView row value extraction** — ShoppingCart code-behind walks `CartList.Rows` with `FindControl()`. No Blazor equivalent — must use `@bind` pattern instead. Architecture change, not component gap.
4. **GetRouteUrl()** — Used in data-binding templates. Replace with string interpolation in Blazor.

**Architecture decisions for migration:**
- Master Page → Blazor LayoutComponentBase with @Body
- Routing: @page directives + [SupplyParameterFromQuery] for query strings
- Data access: EF6 → EF Core, register ProductContext in DI, use IDbContextFactory for Blazor Server
- Session → Scoped DI services (CartStateService, CheckoutStateService)
- Identity: ASP.NET Core Identity + scaffold Identity UI (Razor Pages) for Account pages
- PayPal: Port NVPAPICaller to use HttpClient, keep NVP protocol
- Render mode: Full InteractiveServer for simplest migration

**Migration phases (36 work items, ~16-26 days):**
- Phase 1: Core infrastructure (8 items) — project, models, EF Core, layout, routing
- Phase 2: Product browsing (8 items) — home, product list/details, category nav, FormView fix
- Phase 3: Shopping cart & checkout (9 items) — most complex phase
- Phase 4: Admin (5 items) — forms, validation, file upload
- Phase 5: Auth (6 items) — Identity, LoginView/LoginStatus, admin authorization

 Team update (2026-03-02): WingtipToys migration analysis complete  36 work items across 5 phases, FormView RenderOuterTable is only blocking gap  decided by Forge

### Summary: ASPX/ASCX Migration Tooling Strategy (2026-03-02)

**By:** Forge
**What:** Exhaustive analysis of 33 ASPX/ASCX/Master files from WingtipToys (1,100+ lines), cataloguing 85+ syntax patterns across 15 categories. Designed three-layer migration pipeline and recommended 11 deliverables. Full analysis: `.ai-team/decisions/inbox/forge-migration-tooling-strategy.md`.

**ASPX syntax patterns catalogued:**
- 15 directive patterns (Page, Master, Control, Register)
- 4 Content/ContentPlaceHolder patterns
- 29 distinct server control types mapped to BWFC (96.6% coverage)
- 10 data-binding expression patterns (Item.X, Eval, String.Format)
- 10 code render expression patterns (<%: %>, <%= %>)
- 3 route expression patterns (GetRouteUrl → interpolation)
- 7 runat="server" HTML element patterns
- 6 event handler patterns (OnClick, SelectMethod, DeleteMethod)
- 9 model binding attribute patterns
- 2 comment syntax patterns
- 2 inline code block patterns (<% if %>)
- 6 visibility patterns (Visible="false" → @if blocks)
- 8 special patterns (ScriptManager, bundling, ModelErrorMessage)
- 15 template patterns (all preserved 1:1 in BWFC)
- 20 code-behind patterns (lifecycle, navigation, session, identity, data access)

**Migration automation strategy (three layers):**
- Layer 1: `bwfc-migrate.ps1` — mechanical script, ~40% of transforms, regex-based, deterministic
- Layer 2: `.copilot/skills/webforms-migration/SKILL.md` — structural transforms via Copilot, ~45% of transforms
- Layer 3: `.github/agents/migration.agent.md` — interactive agent for semantic decisions, ~15% of transforms

**Copilot agent/skill design decisions:**
- Copilot skill file is highest-value deliverable (teaches Copilot all structural rules)
- Mechanical script handles safe deterministic transforms first (no AI needed)
- Agent orchestrates full workflow: scan → scaffold → migrate → architect → verify
- Route table and layout mapping provided via JSON config file
- NOT building: standalone CLI tool, VS Code extension, Roslyn analyzer, full ASP.NET parser

**Key findings:**
- 28/29 controls used in WingtipToys have BWFC equivalents (only ModelErrorMessage missing — use Blazor's ValidationMessage)
- FormView RenderOuterTable remains only blocking component gap
- SelectMethod → Items is the most common structural transform (every data-bound control)
- Session → scoped DI services is the hardest semantic transform (ShoppingCartActions)
- Account pages (14 files) map to ASP.NET Core Identity — recommend scaffold, not component migration

 Team update (2026-03-02): Project reframed  final product is a migration acceleration system (tool/skill/agent), not just a component library. WingtipToys is proof-of-concept.  decided by Jeffrey T. Fritz

### Summary: ModelErrorMessage Component Specification (2026-03-02)

**By:** Forge
**What:** Designed component spec for `<ModelErrorMessage>` — the last control without a BWFC equivalent (28/29 WingtipToys controls had coverage). Spec written to `.ai-team/decisions/inbox/forge-modelerrormessage-spec.md`.

**ModelErrorMessage design decisions:**
- Inherits `BaseStyledComponent`, NOT `BaseValidator<T>`. ModelErrorMessage is a passive error display (like Label), not a validator. Same reasoning as AspNetValidationSummary.
- Uses `[CascadingParameter] EditContext` as error source — maps Web Forms' `ModelState[key]` to Blazor's `EditContext.GetValidationMessages(field)`. Zero-markup migration: just remove `asp:` and `runat="server"`.
- `ModelStateKey` parameter maps to `EditContext.Field(key)` — string-keyed like Web Forms, no lambda expressions required (unlike Blazor's `<ValidationMessage For="...">`).
- Renders `<span class="...">message</span>` (matching Web Forms output). Renders nothing when no error (no space reservation).
- Strips BWFC validator metadata (`\x1F` encoding, `Text,ErrorMessage` format) from messages before display.
- `SetFocusOnError` reuses existing `bwfc.Validation.SetFocus` JS interop. Requires `AssociatedControlID` to know the target.
- `Text` property (Label fallback) and `Display` property (validator-only) omitted — not used in any WingtipToys sample, not part of original ModelErrorMessage API.
- Multiple errors for same key rendered with `<br>` separator inside single `<span>`.
- Lives in `Validations/` folder alongside validators and AspNetValidationSummary for discoverability.

� Team update (2026-03-02): ModelErrorMessage documentation shipped  docs/ValidationControls/ModelErrorMessage.md, status.md updated to 52 components  decided by Beast

### Summary: Migration Toolkit Package Design (2026-03-03)

**By:** Forge
**What:** Designed the structure and content inventory for a portable migration toolkit package at `/migration-toolkit/`. 9 documents total. Analyzed all existing skills, scripts, agents, and the executive report to determine what can be extracted vs. what's net-new.

**Key decisions:**
- Toolkit lives at `/migration-toolkit/` (top-level, not in docs/ or .github/) because migration is the primary product per Jeff's reframing
- Toolkit REFERENCES existing artifacts (scripts, skills, agent) by relative path — no duplication
- 5 documents are primarily extraction/adaptation from existing content; 4 are primarily net-new
- Highest-value deliverable is `copilot-instructions-template.md` — a drop-in template developers copy into their own project to give Copilot migration context
- `CHECKLIST.md` is fully net-new — no existing per-page migration checklist exists

**Existing content reusable:**
- `.github/skills/webforms-migration/SKILL.md` — the core Layer 2 skill, heavily referenced by QUICKSTART, CONTROL-COVERAGE, and copilot-instructions-template
- `.github/agents/migration.agent.md` — Layer 3 decision frameworks, extracted into ARCHITECTURE-GUIDE
- `planning-docs/WINGTIPTOYS-MIGRATION-EXECUTIVE-REPORT.md` — metrics, timelines, screenshots for METHODOLOGY and CASE-STUDY
- `scripts/bwfc-scan.ps1` and `scripts/bwfc-migrate.ps1` — referenced as-is, not modified
- Forge's own history entries on WingtipToys analysis, CSS fidelity audit, and ASPX tooling strategy — lessons learned for CASE-STUDY and FAQ

**Net-new content needed:**
- README.md (entry point), QUICKSTART.md (step-by-step), CHECKLIST.md (per-page template), copilot-instructions-template.md (Copilot handoff with project-specific placeholders)

**Design doc:** `planning-docs/MIGRATION-TOOLKIT-DESIGN.md`


 Team update (2026-03-03): Migration toolkit pivoted from 9-doc folder to single SKILL.md in Copilot skill format  decided by Jeffrey T. Fritz
