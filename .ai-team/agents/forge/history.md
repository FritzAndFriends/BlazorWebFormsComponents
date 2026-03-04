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

<!-- Summarized 2026-03-03 by Scribe -- covers CSS fidelity through WingtipToys schedule -->

### CSS Fidelity & WingtipToys Schedule Summary (2026-03-02 through 2026-03-03)

**WingtipToys CSS fidelity audit:** 7 visual differences — wrong Bootstrap theme (Cerulean), single-column grid, missing Trucks category, Site.css not loaded, BoundField DataFormatString bug, bootstrap-theme gradients.

**M22 planning:** 12 work items, 4 waves. 57 controls ready. Skins & AJAX Toolkit OUT. ListView #406 IN.

**WingtipToys migration:** 15+ pages, 22 controls, 100% BWFC coverage. Architecture: LayoutComponentBase, EF Core, scoped DI, scaffolded Identity, InteractiveServer. 26 work items, 7 phases, critical path 1→2→3→4→7.

**ASPX/ASCX tooling:** Three-layer pipeline validated at ~70% markup. SelectMethod→Items = #1 structural transform.

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

### Migration Run 4 — Enhanced Script Validation (2026-03-04)

**By:** Forge (Lead / Web Forms Reviewer)

**What was tested:** Full from-scratch migration of WingtipToys using the enhanced `bwfc-migrate.ps1` script with `ConvertFrom-MasterPage`, `New-AppRazorScaffold`, Eval format-string regex, and String.Format regex.

**Key findings vs Run 3:**
1. **ConvertFrom-MasterPage is the highest-impact enhancement.** Auto-generates MainLayout.razor from Site.Master with correct @inherits, HeadContent extraction, ContentPlaceHolder→@Body, and LoginView/SelectMethod flagging. Eliminates the most time-consuming manual step.
2. **App.razor + Routes.razor scaffolding saves boilerplate time.** Combined with master page conversion, the script now generates 7 scaffold files vs 4 in Run 3.
3. **Eval format-string regex works correctly:** `<%#: Eval("Total", "{0:C}") %>` → `@context.Total.ToString("C")`.
4. **String.Format regex works for simple cases:** `<%#: String.Format("{0:c}", Item.UnitPrice) %>` → `@($"{context.UnitPrice:c}")`. Complex expressions (arithmetic with Convert.ToDouble) correctly left as manual.
5. **Total transforms increased from 277 to 289** (+12 from new regexes and master page processing).
6. **Build: 0 errors, 0 warnings** (improved from Run 3's 63 warnings).
7. **11/11 features pass** — consistent with Run 2 and Run 3.
8. **CascadingAuthenticationState must be added to Routes.razor** when using AuthorizeView — the scaffolded Routes.razor doesn't include this automatically. Consider adding it to `New-AppRazorScaffold`.
