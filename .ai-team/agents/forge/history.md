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


### Summary: Build/Version/Release Process Audit (2026-03-02)

**By:** Forge
**What:** Full audit of 7 CI/CD workflows, NBGV version management, and release coordination. Audit saved to `.ai-team/decisions/inbox/forge-version-release-audit.md`.

**Critical findings:**
1. `version.json` on main says `0.15` but latest tag is `v0.16` — NBGV computes `0.15.X`, not `0.16.0`. Every artifact built from main has wrong version prefix.
2. NuGet (tag-triggered), Docker (main-push-triggered), docs (main-push or tag), and demos (main-push) all fire independently with no version coordination.
3. `docs.yml` uses deprecated `::set-output` and has a release-detection regex that never matches this project's 2-segment tags (`v0.16`). Docs only deploy on main push, never on tag.
4. No GitHub Release automation — releases are created manually, not all tags have releases.
5. Docker image version comes from `nbgv get-version` on runner which reads stale `version.json`, not the tag.

**Recommendation:**
- Create unified `release.yml` triggered by GitHub Release `published` event. One trigger → NuGet + Docker + docs + demos, all with same version from tag.
- Keep NBGV but use it correctly: version.json drives dev/CI versions, release workflow overrides with `-p:Version=${TAG}` for exact release version.
- Standardize on 3-segment SemVer tags (`v0.17.0`).
- Retire independent deployment triggers (nuget.yml tag trigger, deploy-server-side.yml main-push trigger).
- Automate post-release version bump via PR to dev.
- Fix docs.yml deprecated syntax and broken regex immediately.

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
