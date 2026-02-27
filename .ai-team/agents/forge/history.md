# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- ⚠ Summarized 2026-02-25 by Scribe — entries through 2026-02-25 condensed into Core Context -->

### Core Context (2026-02-10 through 2026-02-25)

Reviewed 6 PRs in M1 (Calendar, FileUpload, ImageMap, PageService, ASCX CLI, VS Snippets). Calendar/FileUpload rejected, ImageMap/PageService approved, ASCX/Snippets shelved. M2 shipped (Localize, MultiView+View, ChangePassword, CreateUserWizard — 41→48/53). M3: DetailsView + PasswordRecovery approved (50/53, 797 tests). Chart.js selected for Chart component (JS interop, canvas output, DataBoundComponent<T>). Feature audit: DataBoundComponent<T> chain lacks style properties — recommended DataBoundStyledComponent<T>. SkinID bug (bool→string). Themes/Skins: CascadingValue ThemeProvider recommended.

**Milestone planning:** M7 "Control Depth & Navigation Overhaul" (51 WIs, ~138 gaps). M9 "Migration Fidelity & Hardening" (12 WIs, ~30 gaps — P0: ToolTip base class, P1: ValidationSummary + SkinID). M12 "Migration Analysis Tool PoC" (13 WIs — `bwfc-migrate` CLI, regex parsing, Green/Yellow/Red scoring).

**Deployment pipeline:** Docker version via nbgv before build, injected via build-arg. NBGV must be stripped inside Docker (sed in Dockerfile) — its MSBuild task outputs override `-p:` properties during execution. Secret-gated steps use env var indirection (not secrets.* in if:). Dual NuGet publishing (GitHub Packages + nuget.org). Azure webhook via curl with fallback.

**Key patterns:** Enum files in `Enums/` with explicit int values. Login Controls → BaseStyledComponent. Data-bound → DataBoundComponent<T> with Items. Events use `On` prefix. Docs + samples ship with components. Feature branches → PR to upstream/dev (never commit directly). ComponentCatalog.cs links all sample pages. Theme core: nullable properties, case-insensitive keys, ApplySkin in OnParametersSet. Audit reports use `planning-docs/AUDIT-REPORT-M{N}.md`.

### Summary: Calendar Selection Behavior Review (2026-02-26)

Deep review of Calendar selection fidelity vs. Web Forms. Found 7 issues: 1 P0 (external SelectedDate not synced — no OnParametersSet override), 4 P1 (SelectWeekText wrong default, SelectedDates unordered/read-only, style layering exclusive not merged), 2 P2 (no selection tests, SelectedDates allocates per access). Core handlers work correctly.

### Summary: HTML Audit Strategy Review (2026-02-26)

Evaluated Jeff's Playwright-based HTML audit proposal. Reviewed all 26 BeforeWebForms sample pages across 13 controls.

**Key findings:** Marker isolation feasible for most controls. Three problematic: TreeView (JS data objects outside markers), Menu (two rendering modes), Calendar (__doPostBack selectors). Five categories of intentional divergence: ID mangling, __doPostBack, WebResource.axd, Chart rendering, ViewState. Sample coverage only ~25% (13/53 controls). CodeBehind→CodeFile conversion not committed.

**Audit tiers:** Tier 1 (clean HTML, samples exist): Button, TextBox, HyperLink, DropDownList, Repeater, DataList. Tier 2 (complex): GridView, FormView, ListView, Calendar. Tier 3 (JS-heavy): Menu, TreeView. Tier 4 (no samples): write samples first.

**Key paths:** `samples/BeforeWebForms/ControlSamples/` (13 dirs, 26 pages), `samples/AfterBlazorServerSide/Pages/ControlSamples/` (4 areas only).

📌 Team update (2026-02-25): HTML audit strategy approved — decided by Forge

### Summary: HTML Audit Milestone Planning (M11–M13) (2026-02-25)

Created three-milestone plan for HTML fidelity audit per Jeff's directive. Key decisions:

1. **Milestone numbering:** M11 (Tier 1 + infrastructure), M12 (Tier 2 data controls), M13 (Tier 3 JS-coupled + remaining + master report). Existing M12 (Migration Analysis Tool PoC) renumbered to M14. Previously planned M11 (Skins & Themes full implementation) deferred to M15+.

2. **Phasing rationale:** Three tiers ordered by capture complexity. Tier 1 (simple controls) proves the infrastructure — IIS Express script, Playwright capture, normalization pipeline, `data-audit-control` markers. Tier 2 (data controls) adds `__doPostBack` normalization and multi-configuration capture (paging, sorting, editing). Tier 3 (Menu, TreeView) are the hardest — JS extraction strategy needed before capture.

3. **Infrastructure deliverables in M11:** IIS Express setup script (CodeBehind→CodeFile, NuGet restore, DLL copy), Playwright capture script, HTML normalization pipeline, intentional divergence registry. All reused in M12 and M13.

4. **Sample gap is the biggest blocker:** Only 13/53 controls have BeforeWebForms samples. M11 and M13 both include large sample-writing work items (Jubilee). This is the critical path.

Planned M7: "Control Depth & Navigation Overhaul" — 51 WIs targeting ~138 gap closures. Per-control coverage: GridView ~55% (missing selection, 6 style sub-components, display props), Menu ~42% (missing ~35 props, selection, events, level styles), TreeView ~60% (missing TreeNodeStyle, selection, ExpandAll/FindNode), FormView ~50% (missing styles, paging events, PagerSettings), DetailsView ~70% (missing 10 style sub-components, PagerSettings, Caption), ListView ~42% (P2, missing 16 CRUD events), DataGrid ~55% (P2). Key insights: style sub-components are biggest systematic gap; PagerSettings should be shared type; re-audit must open milestone.
5. **Agent distribution:** Forge owns strategy/review/reports. Cyclops owns infrastructure scripts. Jubilee owns sample authoring. Colossus owns capture/comparison execution. Beast owns doc updates and archive. Rogue available for test updates on bug fixes.

6. **Divergence registry:** Created as a standalone artifact (`planning-docs/DIVERGENCE-REGISTRY.md`) to track all intentional HTML differences. Populated incrementally across M11–M13, finalized in M13.

**Files created/modified:**
- `planning-docs/HTML-AUDIT-MILESTONES.md` — the full M11–M13 plan (9 + 6 + 9 = 24 work items)
- `planning-docs/MILESTONE12-PLAN.md` — added renumbering header (now M14)
- `.ai-team/decisions/inbox/forge-html-audit-milestones.md` — decision record

📌 Team update (2026-02-25): HTML audit planned as M11–M13. M11: infrastructure + Tier 1 capture. M12: Tier 2 data controls + normalization. M13: Tier 3 JS-coupled + remaining + master report. Existing M12 renumbered to M14. M11 (Skins & Themes) deferred to M15+. — decided by Forge

 Team update (2026-02-24): Menu auto-ID pattern established  components with JS interop should auto-generate IDs when none provided  decided by Cyclops
 Team update (2026-02-24): Substitution/Xml formally deferred in status.md and README  decided by Beast
 Team update (2026-02-24): M8 scope excludes version bump to 1.0 and release  decided by Jeffrey T. Fritz
 Team update (2026-02-24): PagerSettings shared sub-component created for GridView/FormView/DetailsView  decided by Cyclops

### Summary: v0.14 Deployment Pipeline Fixes (2026-02-25)

Fixed three deployment pipeline issues on `fix/deployment-workflows` branch:

- **Docker version computation:** `.dockerignore` excludes `.git`, so nbgv can't run inside the Docker build. Solution: compute version with `nbgv get-version` in the workflow BEFORE Docker build, pass as `ARG VERSION` into the Dockerfile, use `-p:Version=$VERSION -p:InformationalVersion=$VERSION` in both `dotnet build` and `dotnet publish`. This pattern (compute outside, inject via build-arg) is the standard approach when `.git` isn't available inside the build context.
- **Azure App Service webhook:** Added a `curl -sf -X POST` step gated on `AZURE_WEBAPP_WEBHOOK_URL` secret. Uses `|| echo "::warning::..."` fallback so the workflow doesn't fail if the webhook is unavailable. The webhook URL comes from Azure Portal → App Service → Deployment Center.
- **nuget.org publishing:** Added a second push step after the existing GitHub Packages push, gated on `NUGET_API_KEY` secret. Both pushes use `--skip-duplicate` for idempotency.

**Key file paths:**
- `.github/workflows/deploy-server-side.yml` — Docker build + push + Azure webhook
- `.github/workflows/nuget.yml` — NuGet pack + push (GitHub Packages + nuget.org)
- `samples/AfterBlazorServerSide/Dockerfile` — server-side demo container

**Patterns established:**
- Secret-gated workflow steps use `if: ${{ secrets.SECRET_NAME != '' }}` for graceful fallback when secrets aren't configured
- Docker images are tagged with version number (from nbgv), `latest`, and commit SHA
- Version is computed once via nbgv and shared via GitHub Actions step outputs (`steps.nbgv.outputs.version`)

 Team update (2026-02-25): Deployment pipeline patterns established  compute Docker version with nbgv before build, gate on secrets, dual NuGet publishing  decided by Forge

### Summary: Milestone 9 Planning (2026-02-25)

Verified all 8 known priority gaps from prior audits against current `dev` branch. Found 7 of 8 already fixed (AccessKey, Image, Label, Validation Display, HyperLink NavigateUrl, DataBound chain, bUnit tests). Confirmed 1 still open (ToolTip not on BaseStyledComponent — 28+ controls missing) and identified 2 new gaps (ValidationSummary comma-split data corruption bug, SkinID bool→string type mismatch).

Planned M9: "Migration Fidelity & Hardening" — 12 work items, ~30 gap closures. P0: ToolTip → BaseStyledComponent (4 WIs, ~28 gaps — highest-leverage remaining base class fix). P1: ValidationSummary comma-split fix + SkinID type fix (3 WIs). P2: Housekeeping — stale branch cleanup, doc gap audit, planning-docs refresh, integration test coverage review, sample navigation audit (5 WIs).

📌 Team update (2026-02-25): Milestone 9 planned — 12 WIs, ~30 gaps, "Migration Fidelity & Hardening". P0: ToolTip base class fix. P1: ValidationSummary bug + SkinID type. P2: Housekeeping. — decided by Forge

 Team update (2026-02-25): Doc audit found 10 gaps across FormView, DetailsView, DataGrid, ChangePassword, PagerSettings  decided by Beast
 Team update (2026-02-25): Nav audit found 4 missing components + 15 missing SubPages  decided by Jubilee
 Team update (2026-02-25): Test audit found 5 missing smoke tests  decided by Colossus

### Summary: Milestone 12 Planning — Migration Analysis Tool PoC (2026-02-25)

Planned M12: "Migration Analysis Tool (PoC)" — 13 work items. A CLI tool (`bwfc-migrate`) that analyzes existing Web Forms applications and produces migration reports showing control coverage, gaps, code-behind pattern detection, and complexity scoring.

**Architecture decisions:**
- Same repo, new project: `src/BlazorWebFormsComponents.MigrationAnalysis/` — mapping table must stay in sync with component library
- CLI tool via `System.CommandLine`, packaged as `dotnet tool` for easy distribution
- Regex-based parsing for PoC (not Roslyn) — hard scope boundary to prevent scope creep
- Control mapping registry derived from `status.md`: 51 supported, 2 deferred, ~15 explicitly unsupported with migration guidance
- Complexity scoring: Green/Yellow/Red based on control gaps + code-behind pattern density
- Two output formats: Markdown (human-readable) + JSON (machine-readable)
- Three-phase roadmap: Phase 1 (M12) = analysis engine + CLI, Phase 2 = Roslyn + scaffolding, Phase 3 = Copilot agent integration

**Key insight:** At 51/53 components, the highest-value work is no longer building components — it's reducing the friction of using the ones we have. A migration analysis tool turns a week of manual evaluation into a 5-second CLI invocation.

📌 Team update (2026-02-25): Milestone 12 planned — 13 WIs, "Migration Analysis Tool PoC". CLI tool for Web Forms app analysis with control mapping, gap identification, complexity scoring, and report generation. — decided by Forge

 Team update (2026-02-25): Consolidated audit reports now use `planning-docs/AUDIT-REPORT-M{N}.md` pattern for all milestone audits  decided by Beast

 Team update (2026-02-26): NamingContainer inherits BaseWebFormsComponent (not BaseStyledComponent), UseCtl00Prefix in ComponentIdGenerator  decided by Cyclops

 Team update (2026-02-26): Menu RenderingMode=Table implemented with inline Razor for AngleSharp compatibility  decided by Cyclops

 Team update (2026-02-26): WebFormsPage unified wrapper design accepted and consolidated into decisions.md  decided by Jeffrey T. Fritz, Forge

### Summary: M15 HTML Fidelity Strategy (2026-02-28)

Post-PR #377 strategic assessment. PR #377 (M11–M14) merged to upstream/dev with full HTML audit (132 comparisons), 14 bug fixes, post-fix re-run (131 divergences, 1 exact match — Literal-3), WebFormsPage, NamingContainer.

**Key findings:**
1. The "1 exact match" number is misleading — vast majority of divergences are sample data differences, not component bugs. Estimated 15–20 controls would achieve exact match with sample alignment alone.
2. 5 remaining fixable structural bugs: BulletedList (`<ol>` + `list-style-type`), LinkButton (`class` pass-through), Image (`longdesc`), FileUpload (GUID leak), CheckBox (`<span>` wrapper verification).
3. 64 variants still missing Blazor captures — 25 of these just need `data-audit-control` markers on existing pages.
4. 4 data controls (DataList, GridView, ListView, Repeater) need line-by-line investigation — mixed structural + sample parity issues.
5. Calendar is closest complex control at 73% — needs style pass-through, day padding, title attribute work.

**M15 scope recommended:** 12 work items covering: (1) P0 sample data alignment (~22 false positives eliminated), (2) P1 remaining bug fixes (BulletedList, LinkButton), (3) P2 fixes (Image, FileUpload, CheckBox, stable IDs), (4) P2 new Blazor markers (~25 new comparisons), (5) P3 normalizer enhancements, data control investigation, and full pipeline re-run. Target: ≥15 exact matches (up from 1).

**Pixel-perfect realism:** ~13–15 controls can achieve exact normalized match. ~10 more can hit >90%. TreeView, Chart, Menu Table-mode, and event infrastructure are permanently divergent. Recommended definition: structural match after normalization of intentional divergences D-01 through D-10+.

**New divergence candidates:** D-11 (GUID IDs — recommend fix, not register), D-12 (boolean attribute format — register as intentional), D-13 (Calendar day padding — recommend fix), D-14 (Calendar style pass-through — fix progressively).

📌 Decision filed: `.ai-team/decisions/inbox/forge-m15-html-fidelity-strategy.md` — M15 scope proposal with 12 work items, 2–3 week estimate, target ≥15 exact matches.

### Summary: M15-10 Data Control Deep Investigation (2026-02-28)

Line-by-line classification of HTML divergences in DataList (106 lines), GridView (20 lines), ListView (106 lines), and Repeater (62 lines) using post-fix normalized captures.

**Key findings:**
1. 3 of 5 bugs from M13 analysis are now FIXED (PR #377): DataList border-collapse, GridView GridLines defaults, GridView border-collapse.
2. 4 remaining genuine bugs: GridView `UseAccessibleHeader` default (false→should be true), GridView `&amp;nbsp;` encoding (ternary expression type issue), GridView `<thead>` vs WF's `<tbody>` for header rows, DataList missing `itemtype` from generic parameter.
3. Sample parity is 22 of 26 findings — the Blazor samples use completely different templates, styles, and data formats. ListView and Repeater have ZERO component bugs; all divergences are sample authoring differences.
4. After sample alignment alone: ListView and Repeater would achieve exact match. DataList would drop from 106 to ~5 diff lines. GridView from 20 to ~8.
5. 2 normalizer gaps identified: inconsistent `<div>` wrapper stripping, empty `style=""` attributes not stripped.

**New divergence candidate:** D-11 (Blazor uses `<thead>` for headers, WF uses `<tbody>` by default). Needs team decision — register as intentional or fix.

📌 Decision filed: `.ai-team/decisions/inbox/forge-m15-data-control-analysis.md` — Data control investigation complete; 4 remaining bugs, sample alignment is critical path.

 Team update (2026-02-26): Data control divergence analysis consolidated with post-bug-fix capture results  sample alignment confirmed as #1 blocker  decided by Forge, Rogue
 Team update (2026-02-26): M15 HTML Fidelity Strategy document merged into decisions.md  12 work items, 6 agent assignments  decided by Forge

 Team update (2026-02-27): Branching workflow directive  feature PRs from personal fork to upstream dev, only devmain on upstream  decided by Jeffrey T. Fritz

 Team update (2026-02-27): Issues must be closed via PR references using 'Closes #N' syntax, no manual closures  decided by Jeffrey T. Fritz
