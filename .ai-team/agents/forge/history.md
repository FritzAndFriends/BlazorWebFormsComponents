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

### Summary: M17 AJAX Controls Gate Review (2026-02-28)

**By:** Forge
**What:** Full Web Forms fidelity review of all 6 M17 controls (Timer, ScriptManager, ScriptManagerProxy, UpdatePanel, UpdateProgress, Substitution) against .NET Framework 4.8 API documentation.

**Verdict: APPROVE WITH NOTES** — All 6 controls pass for PR. Four minor fidelity notes for follow-up:

1. **ScriptManager `EnablePartialRendering` default** — Web Forms defaults to `true`, Blazor stub defaults to `false` (bare bool). No functional impact (no-op stub), but fidelity gap if property is read.
2. **ScriptManager missing `Scripts` property** — Web Forms ScriptManager has a `Scripts` collection (ScriptReferenceCollection). Only ScriptManagerProxy has it in the Blazor implementation. Low-priority since ScriptManager is a no-op stub.
3. **UpdateProgress `CssClass` not rendered** — Inherits BaseStyledComponent so accepts CssClass parameter, but the .razor template doesn't apply it to the `<div>` output. Migrating `<asp:UpdateProgress CssClass="loading">` would silently drop the class.
4. **UpdateProgress non-dynamic layout style** — Renders `visibility:hidden` but Web Forms renders `display:block;visibility:hidden`. Functionally identical (div is block by default) but not byte-identical.

**What passed clean:**
- All 6 component names match Web Forms originals exactly
- Timer: Interval (60000 default), Enabled (inherited), OnTick event — all correct
- ScriptManager/Proxy: correct as no-op stubs with BaseWebFormsComponent
- UpdatePanel: RenderMode (Block=div, Inline=span), UpdateMode, ChildrenAsTriggers — all correct. ChildContent instead of ContentTemplate is intentional per Blazor conventions.
- UpdateProgress: AssociatedUpdatePanelID, DisplayAfter (500), DynamicLayout (true), ProgressTemplate — all correct
- Substitution: MethodName preserved for migration, SubstitutionCallback for Blazor, no wrapper element — all correct
- All 3 enums (ScriptMode, UpdatePanelRenderMode, UpdatePanelUpdateMode) match Web Forms values exactly with correct int assignments
- ComponentCatalog properly categorizes AJAX and Migration Helper controls
- Base class choices appropriate: stubs use BaseWebFormsComponent, UpdateProgress uses BaseStyledComponent (has CssClass in WF)

**Key pattern learned:** AJAX controls split cleanly into two categories: (1) functional components that change Blazor behavior (Timer, UpdatePanel, UpdateProgress) and (2) pure migration stubs that render nothing (ScriptManager, ScriptManagerProxy, Substitution). The stub pattern is sound — accept-but-ignore properties to prevent compilation errors during migration.

### Summary: M17 Formal Fidelity Audit (2026-02-28)

**By:** Forge
**What:** Full Web Forms fidelity audit of all 6 M17 controls against Microsoft Learn .NET Framework 4.8.1 API documentation. Report saved to `planning-docs/M17-CONTROL-AUDIT.md`.

**Property coverage by control:**
- Timer: 2/2 (100%) — Interval, Enabled. OnTick event 1/1.
- ScriptManager: 7/17 (41%) — Appropriate for no-op stub. Only declarative-markup properties needed.
- ScriptManagerProxy: 2/4 core (50%) — Scripts and Services collections present. Service manager properties omitted (no Blazor equivalent).
- UpdatePanel: 4/5 (80%) — Only `Triggers` missing (unnecessary in Blazor's rendering model). ContentTemplate→ChildContent is intentional adaptation.
- UpdateProgress: 4/4 (100%) — All control-specific properties present with correct defaults.
- Substitution: 1/1 (100%) — MethodName preserved, SubstitutionCallback added as Blazor adaptation.

**5 follow-up issues identified:**
1. ScriptManager `EnablePartialRendering` default should be `true` (currently `false`)
2. ScriptManager missing `Scripts` collection (only on Proxy)
3. UpdateProgress `CssClass` not rendered on output `<div>` — medium severity
4. UpdateProgress non-dynamic mode missing `display:block;` prefix
5. ScriptReference only has 3 of ~8 WF properties

**All 3 enums** (ScriptMode, UpdatePanelRenderMode, UpdatePanelUpdateMode) verified as exact int-value matches with Web Forms originals.

 Team update (2026-02-27): M17 audit fixes resolved  5 fidelity issues fixed (EnablePartialRendering default, Scripts collection, CssClass rendering, display:block style, ScriptReference properties). 9 new tests, 1367 total. PR #402  decided by Forge, Cyclops

� Team update (2026-02-27): M17 audit fix test patterns  ScriptReference tested via C# instantiation, UpdateProgress CssClass tested with/without value, 9 new tests  decided by Rogue
