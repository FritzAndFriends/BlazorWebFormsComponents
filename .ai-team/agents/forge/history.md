# Project Context

- **Owner:** Jeffrey T. Fritz (csharpfritz@users.noreply.github.com)
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- ⚠ Summarized 2026-02-25 by Scribe — older entries condensed into Core Context -->

### Core Context (2026-02-10 through 2026-02-25)

Reviewed 6 PRs in M1 (Calendar, FileUpload, ImageMap, PageService, ASCX CLI, VS Snippets). Calendar/FileUpload rejected, ImageMap/PageService approved, ASCX/Snippets shelved. M2 shipped (Localize, MultiView+View, ChangePassword, CreateUserWizard — 41→48/53). M3: DetailsView + PasswordRecovery approved (50/53, 797 tests). Chart.js selected for Chart component (JS interop, canvas output, DataBoundComponent<T>). Chart gate review: conditionally approved pending data binding fix (140 tests). Feature audit: DataBoundComponent<T> chain lacks style properties (systematic gap), recommended DataBoundStyledComponent<T>. SkinID bug (bool→string). Themes/Skins: CascadingValue ThemeProvider recommended.

**Key patterns:** Enum files in `Enums/` with explicit int values. Login Controls → BaseWebFormsComponent (later upgraded to BaseStyledComponent). Data-bound → DataBoundComponent<T> with Items. Events use `On` prefix. Docs + samples ship with components. Secret-gated workflow steps use env var indirection (not secrets.* in if:). Docker version computed by nbgv before build, injected via build-arg.

### Summary: Milestone 7 Planning (2026-02-23)

Planned M7: "Control Depth & Navigation Overhaul" — 51 WIs targeting ~138 gap closures. Per-control coverage: GridView ~55% (missing selection, 6 style sub-components, display props), Menu ~42% (missing ~35 props, selection, events, level styles), TreeView ~60% (missing TreeNodeStyle, selection, ExpandAll/FindNode), FormView ~50% (missing styles, paging events, PagerSettings), DetailsView ~70% (missing 10 style sub-components, PagerSettings, Caption), ListView ~42% (P2, missing 16 CRUD events), DataGrid ~55% (P2). Key insights: style sub-components are biggest systematic gap; PagerSettings should be shared type; re-audit must open milestone.

📌 Team update (2026-02-23): Milestone 7 planned — 51 WIs, ~138 gaps, "Control Depth & Navigation Overhaul". P0: GridView completion + re-audit. P1: TreeView, Menu, DetailsView, FormView, Validators. P2: ListView CRUD, DataGrid, Menu levels. — decided by Forge

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



📌 Team update (2026-02-25): CI secret-gating pattern corrected — secrets.* cannot be used in step-level if: conditions. Use env var indirection: declare secret in env:, check env.VAR_NAME in if:. Applied to nuget.yml and deploy-server-side.yml (PR #372). — decided by Forge

### Summary: NBGV Docker Version Stamping Fix (2026-02-25)

**Root cause:** NBGV (Nerdbank.GitVersioning 3.9.50) overrides externally-passed `-p:InformationalVersion` inside Docker, even when `.git` is excluded by `.dockerignore`. The mechanism:

1. NBGV's targets (`Nerdbank.GitVersioning.targets` line 28) set `GenerateAssemblyInformationalVersionAttribute=false`, suppressing the SDK's attribute generation.
2. NBGV's `GetBuildVersion` task (`Nerdbank.GitVersioning.Inner.targets` line 34) unconditionally overwrites `AssemblyInformationalVersion` via `<Output>` during build execution — this overrides even `-p:` global properties.
3. NBGV's `GenerateAssemblyNBGVVersionInfo` target generates its own assembly version source file using NBGV's computed value (from `version.json` fallback when no `.git`).

Result: The `BlazorWebFormsComponents` assembly gets stamped with NBGV's fallback version (e.g., `0.13.0` from old version.json) instead of the precise version computed by nbgv outside Docker.

**Fix:** Added `sed` command in `samples/AfterBlazorServerSide/Dockerfile` after `COPY . .` to strip the NBGV PackageReference from `Directory.Build.props` inside the Docker build. This completely removes NBGV from the build, allowing the SDK's default attribute generation to use the `-p:InformationalVersion=$VERSION` property passed from the workflow. The `dotnet build` implicit restore handles the changed project metadata automatically.

**Key insight:** You cannot override NBGV's version stamping via `-p:` properties alone. NBGV's MSBuild task outputs overwrite properties during execution (not evaluation), bypassing global property precedence. The only reliable fix is to remove NBGV from the build when git history is unavailable and version is injected externally.

📌 Team update (2026-02-25): Docker NBGV fix — strip NBGV PackageReference inside Docker build via sed. NBGV cannot be overridden by -p: properties; must be removed entirely when .git is unavailable. — decided by Forge



 Team update (2026-02-25): Future milestone work should include a doc review pass to catch stale 'NOT Supported' entries  decided by Beast

 Team update (2026-02-25): Shared sub-components of sufficient complexity get their own doc page (e.g., PagerSettings)  decided by Beast

 Team update (2026-02-25): All login controls (Login, LoginView, ChangePassword, PasswordRecovery, CreateUserWizard) now inherit from BaseStyledComponent  decided by Cyclops

 Team update (2026-02-25): ComponentCatalog.cs now links all sample pages; new samples must be registered there  decided by Jubilee

### Summary: Calendar Selection Behavior Review (2026-02-26)

Deep review of Calendar component selection fidelity vs. Web Forms Calendar control, requested by Jeff. Found 7 issues: 1 P0 (external SelectedDate parameter not synced to _selectedDays — no OnParametersSet override), 4 P1 (SelectWeekText wrong default "&gt;&gt;" should be "&gt;"; SelectedDates not sorted ascending; SelectedDates read-only — Web Forms has Add/Remove/Clear/SelectRange; style layering exclusive not merged), 2 P2 (no tests for week/month selection; SelectedDates allocates new collection per access). Core click handlers work correctly — day/week/month selection, clearing, events all fire properly. Title row colspan math correct. Style precedence order matches Web Forms. Wrote findings to `.ai-team/decisions/inbox/forge-calendar-selection-review.md`.

📌 Team update (2026-02-26): Calendar selection review — 7 issues found (1 P0, 4 P1, 2 P2). P0: external SelectedDate not synced. P1: SelectWeekText default wrong, SelectedDates unordered/read-only, style merging broken. — decided by Forge

 Team update (2026-02-25): All new work MUST use feature branches pushed to origin with PR to upstream/dev. Never commit directly to dev.  decided by Jeffrey T. Fritz


 Team update (2026-02-25): Theme core types (#364) use nullable properties for StyleSheetTheme semantics, case-insensitive keys, empty-string default skin key. ThemeProvider is infrastructure, not a WebForms control. GetSkin returns null for missing entries.  decided by Cyclops


 Team update (2026-02-25): SkinID defaults to empty string, EnableTheming defaults to true. [Obsolete] removed  these are now functional [Parameter] properties.  decided by Cyclops


 Team update (2026-02-25): ThemeConfiguration CascadingParameter wired into BaseStyledComponent (not BaseWebFormsComponent). ApplySkin runs in OnParametersSet with StyleSheetTheme semantics. Font properties checked individually.  decided by Cyclops


 Team update (2026-02-25): ThemesAndSkins.md documentation updated to match PoC implementation  class names, API, roadmap status, PoC decisions table added  decided by Beast
