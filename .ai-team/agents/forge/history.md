# Project Context

- **Owner:** Jeffrey T. Fritz (csharpfritz@users.noreply.github.com)
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

5. **Agent distribution:** Forge owns strategy/review/reports. Cyclops owns infrastructure scripts. Jubilee owns sample authoring. Colossus owns capture/comparison execution. Beast owns doc updates and archive. Rogue available for test updates on bug fixes.

6. **Divergence registry:** Created as a standalone artifact (`planning-docs/DIVERGENCE-REGISTRY.md`) to track all intentional HTML differences. Populated incrementally across M11–M13, finalized in M13.

**Files created/modified:**
- `planning-docs/HTML-AUDIT-MILESTONES.md` — the full M11–M13 plan (9 + 6 + 9 = 24 work items)
- `planning-docs/MILESTONE12-PLAN.md` — added renumbering header (now M14)
- `.ai-team/decisions/inbox/forge-html-audit-milestones.md` — decision record

📌 Team update (2026-02-25): HTML audit planned as M11–M13. M11: infrastructure + Tier 1 capture. M12: Tier 2 data controls + normalization. M13: Tier 3 JS-coupled + remaining + master report. Existing M12 renumbered to M14. M11 (Skins & Themes) deferred to M15+. — decided by Forge
