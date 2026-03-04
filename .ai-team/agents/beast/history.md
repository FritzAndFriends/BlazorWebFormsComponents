# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- ⚠ Summarized 2026-02-27 by Scribe — covers M1–M16 -->

### Core Context (2026-02-10 through 2026-02-27)

**Doc structure:** title → intro (MS docs link) → Features Supported → NOT Supported → Web Forms syntax → Blazor syntax → HTML Output → Migration Notes (Before/After) → Examples → See Also. Admonitions for gotchas. mkdocs.yml nav alphabetical within categories. Migration section: "Getting started" and "Migration Strategies" at top.

**Key patterns:** Style migration: TableItemStyle → CSS class string parameters. DeferredControls.md has dual role (fully deferred + partially implemented). Chart screenshots at `docs/images/{component}/chart-{type}.png`. Shared sub-component docs linked from parents. PagerSettings is first shared sub-component with own doc page. Structural components (no HTML output) lead with "renders no HTML" callout. Audit reports at `planning-docs/AUDIT-REPORT-M{N}.md` with historical snapshot headers. Branch naming: `copilot/create-*`.

**Doc work completed:** M1–M3 docs (PasswordRecovery 3-step wizard, DetailsView generic component). Chart doc (JS interop "HTML Output Exception" pattern, Chart Type Gallery, child component doc pattern). M8 release-readiness polish (Substitution/Xml deferred in status.md, Chart Phase 1 hedging removed, README link fixes). M9 Doc Gap Audit (FormView, DetailsView, DataGrid, ChangePassword, PagerSettings.md created). ToolTip universality in Migration/readme.md. ThemesAndSkins.md updated for M10 PoC. NamingContainer.md created with IDRendering.md cross-refs. M9 Consolidated Audit Report (29 findings → M10 issues).

**Pending doc needs:** ClientIDMode property documentation (M16). Menu dual rendering modes. ListView CRUD events. Menu styles (IMenuStyleContainer). Post-M15 verification badges if new exact matches achieved. Login+Identity deferred — do not schedule docs.

- **M17 AJAX Controls documentation (6 pages):** Created documentation for 6 AJAX-era Web Forms controls added in M17:
  1. **Timer.md** (`docs/EditorControls/Timer.md`) — Interval-based tick events using System.Threading.Timer internally. No ScriptManager dependency. Full before/after migration with auto-refresh and countdown examples.
  2. **ScriptManager.md** (`docs/EditorControls/ScriptManager.md`) — Migration stub that renders nothing. Documented all accepted-but-ignored properties. Emphasized "scaffolding" approach: include during migration, remove when stable.
  3. **ScriptManagerProxy.md** (`docs/EditorControls/ScriptManagerProxy.md`) — Migration stub for content pages. Documented IJSRuntime replacement for script registration.
  4. **UpdatePanel.md** (`docs/EditorControls/UpdatePanel.md`) — Structural wrapper rendering `<div>` or `<span>`. Key message: Blazor already does partial rendering, UpdatePanel is for HTML structure preservation. Documented RenderMode Block/Inline.
  5. **UpdateProgress.md** (`docs/EditorControls/UpdateProgress.md`) — Loading indicator with ProgressTemplate. Key migration pattern: replace automatic UpdatePanel association with explicit `bool IsLoading` state management.
  6. **Substitution.md** (`docs/EditorControls/Substitution.md`) — Renders callback output directly. Migrated from "deferred" to "implemented" in DeferredControls.md summary table.
  - Added "AJAX Controls" section to mkdocs.yml nav (alphabetical within section, between Login Controls and Utility Features).
  - Added AJAX Controls category to README.md component listing with links to all 6 doc pages.
  - Updated `docs/Migration/DeferredControls.md` — changed Substitution from ❌ Deferred to ✅ Complete with implementation note.
- **Migration stub documentation pattern:** ScriptManager and ScriptManagerProxy establish a new "migration stub" doc pattern: lead with a `!!! warning "Migration Stub Only"` admonition, document all accepted-but-ignored properties, and include explicit "include → remove" lifecycle guidance. Reuse this pattern for any future no-op migration compatibility components.
- **AJAX Controls nav category:** Created a new "AJAX Controls" nav section in mkdocs.yml separate from "Editor Controls" to group the AJAX-era controls (Timer, ScriptManager, ScriptManagerProxy, UpdatePanel, UpdateProgress, Substitution). This keeps them discoverable as a cohesive migration topic.

 Team update (2026-02-27): Branching workflow directive  feature PRs from personal fork to upstream dev, only devmain on upstream  decided by Jeffrey T. Fritz

 Team update (2026-02-27): Issues must be closed via PR references using 'Closes #N' syntax, no manual closures  decided by Jeffrey T. Fritz


 Team update (2026-02-27): M17 AJAX controls implemented  ScriptManager/Proxy are no-op stubs, Timer shadows Enabled, UpdatePanel uses ChildContent, UpdateProgress renders hidden, Substitution uses Func callback, new AJAX/Migration Helper categories  decided by Cyclops


 Team update (2026-02-27): M17 sample pages created for Timer, UpdatePanel, UpdateProgress, ScriptManager, Substitution. Default.razor filenames. ComponentCatalog already populated  decided by Jubilee

 Team update (2026-02-27): M17 audit fixes resolved  5 fidelity issues fixed (EnablePartialRendering default, Scripts collection, CssClass rendering, display:block style, ScriptReference properties). 9 new tests, 1367 total. PR #402  decided by Forge, Cyclops

 Team update (2026-02-27): No-op stub property coverage intentionally limited (41-50% acceptable)  deep AJAX infrastructure properties omitted  decided by Forge

 Team update (2026-02-27): UpdatePanel Triggers collection deliberately omitted  Blazor rendering model makes it unnecessary  decided by Forge

- **Issue #359 — M6-M8 doc page updates (5 pages):**
  1. **ChangePassword** (`docs/LoginControls/ChangePassword.md`) — Verified already complete. Orientation and TextLayout sections with enum tables, migration examples, and `@using BlazorWebFormsComponents.Enums` tip were already present from a prior session.
  2. **PagerSettings** (`docs/DataControls/PagerSettings.md`) — Verified already complete. Properties reference, enum tables, parent control usage examples, and migration notes all match `PagerSettings.cs` source.
  3. **FormView** (`docs/DataControls/FormView.md`) — Added individual CRUD event names (OnItemDeleting/Deleted, OnItemInserting/Inserted, OnItemUpdating/Updated) to Features section. Added "Web Forms Features NOT Supported" section (DataSourceID, ViewState, Theming, RenderTable). Added CRUD event handling example with FormViewUpdateEventArgs/FormViewDeleteEventArgs.
  4. **DetailsView** (`docs/DataControls/DetailsView.md`) — Added Caption/CaptionAlign attributes to Web Forms declarative syntax. Added all 10 style sub-component elements and PagerSettings child element to the Web Forms syntax block, bringing it to parity with actual Web Forms control markup.
  5. **DataGrid** (`docs/DataControls/DataGrid.md`) — Removed stale "not every syntax element supported" caveat (features were implemented in M6-M8). Enhanced paging section with property reference table, PagerStyle example, and admonition explaining DataGrid's built-in numeric pager vs. GridView/FormView/DetailsView PagerSettings. Added PagerSettings cross-reference in See Also.
  - All 5 pages verified present in `mkdocs.yml` nav. No nav changes needed.
  - **Pattern discovered:** DataGrid is the only pageable data control without PagerSettings sub-component support — it always uses a numeric pager. Worth noting for future migration guidance.

 Team update (2026-02-28): Cyclops fixed MenuItemStyle Font- attributes (SetFontsFromAttributes) and CheckBox bare input id  may need doc updates. Issue #379 (LinkButton CssClass) verified as already fixed in M15, can be closed.

- **M10 Skins & Themes Developer Guide (`docs/Migration/SkinsAndThemes.md`):**
  - Created comprehensive developer guide following the Utility Feature Documentation Template.
  - Structure: Background → Web Forms Usage → Blazor Implementation → Migration Path (6 steps) → Code Examples (4 scenarios) → Limitations (PoC) → Moving On.
  - Uses tabbed before/after comparison (pymdownx.tabbed) for the complete migration example.
  - Coexists with existing `ThemesAndSkins.md` (strategy/architecture comparison). New doc is the practical "how to use it" guide; existing doc is the "why this approach" analysis.
  - Added to `mkdocs.yml` nav as "Skins and Themes Guide" (alphabetical before "Themes and Skins Strategy").
  - Updated `README.md` to replace "skins or themes" deferred statement with active ThemeProvider link.
  - **Key convention:** When a feature has both a strategy/comparison doc and a practical guide, use separate files with clear nav labels distinguishing them ("Guide" vs "Strategy").

 Team update (2026-03-01): SkinBuilder uses expression trees for nested property access  if API changes, update SkinsAndThemes.md examples first (primary dev-facing doc)  decided by Cyclops
📌 Team update (2026-03-02): FontInfo.Name/Names now auto-synced bidirectionally. Theme font-family renders correctly. Update SkinsAndThemes.md if font examples need revision — decided by Cyclops, Rogue
📌 Team update (2026-03-02): CascadedTheme (not Theme) is the cascading parameter name on BaseWebFormsComponent — decided by Cyclops
📌 Team update (2026-03-02): Theming sample page uses 6-section progressive layout (Jubilee). Docs should reference this pattern for theming examples — decided by Jubilee


 Team update (2026-03-02): Unified release process implemented  single release.yml triggered by GitHub Release publication coordinates all artifacts (NuGet, Docker, docs, demos). version.json now uses 3-segment SemVer (0.17.0). Existing nuget.yml and deploy-server-side.yml are workflow_dispatch-only escape hatches. PR #408  decided by Forge (audit), Cyclops (implementation)

 Team update (2026-03-02): Full Skins & Themes roadmap defined  3 waves, 15 work items. Wave 1: Theme mode, sub-component styles (41 slots across 6 controls), EnableTheming propagation, runtime switching. See decisions.md for full roadmap and agent assignments  decided by Forge


 Team update (2026-03-02): M22 Copilot-Led Migration Showcase planned  decided by Forge

 Team update (2026-03-02): WingtipToys migration analysis complete  36 work items across 5 phases, FormView RenderOuterTable is only blocking gap  decided by Forge

 Team update (2026-03-02): Project reframed  final product is a migration acceleration system (tool/skill/agent), not just a component library. WingtipToys is proof-of-concept.  decided by Jeffrey T. Fritz
 Team update (2026-03-02): ASPX/ASCX migration tooling strategy produced  85+ patterns, 3-layer pipeline (mechanical/structural/semantic), 11 deliverables.  decided by Forge

 Team update (2026-03-02): ModelErrorMessage component spec consolidated  29/29 WingtipToys coverage, BaseStyledComponent, EditContext pattern  decided by Forge

- **Executive Migration Report (`planning-docs/WINGTIPTOYS-MIGRATION-EXECUTIVE-REPORT.md`):**
  - Created executive migration report for manager audience
  - Key metrics: 96.6% control coverage, 55-70% time savings, 18-26 hour migration estimate
  - Structured for non-technical readers: bottom-line callout, business value lead, tables for data, minimal jargon
  - Sections: What is BWFC, Migration Scope, Component Coverage, Three-Layer Pipeline, Time & Cost Impact, Layer 1 Results, Page Readiness, Risk Reduction, What's Next

- **Migration Toolkit (6 priority documents in `/migration-toolkit/`):**
  1. **README.md** — Entry point. Prerequisites, three-layer pipeline overview, quick overview (scan→transform→guide→verify), file map, links to existing artifacts, honest "what BWFC doesn't cover" section. 100% net-new.
  2. **QUICKSTART.md** — Linear 9-step walkthrough from "I have a Web Forms app" to running Blazor. ~30% extracted from migration skill, ~70% net-new.
  3. **CONTROL-COVERAGE.md** — Full 52-component table with complexity ratings (Trivial/Easy/Medium/Complex), key changes, gotchas. Includes unsupported controls (DataSource, Wizard, Web Parts, AJAX Toolkit). ~70% extracted from migration skill, ~30% net-new.
  4. **METHODOLOGY.md** — Three-layer pipeline deep-dive with ASCII diagram, layer boundaries, readiness categories, time estimates. ~60% from executive report, ~40% net-new.
  5. **CHECKLIST.md** — Copy-paste per-page migration checklist organized by layer. Usage tips for GitHub issues and tracking. 100% net-new.
  6. **copilot-instructions-template.md** — Drop-in `.github/copilot-instructions.md` template with condensed rules, expression tables, placeholder sections. ~60% from migration skill, ~40% net-new.
  - **Key decisions:** No content duplication — all docs reference scripts/skill/agent by relative path. CONTROL-COVERAGE.md is single source for coverage table in toolkit. copilot-instructions-template.md is self-contained (copied out of repo). Practitioner tone throughout.
  - **Sources:** MIGRATION-TOOLKIT-DESIGN.md (blueprint), SKILL.md (rules/tables), WINGTIPTOYS-MIGRATION-EXECUTIVE-REPORT.md (metrics), migration.agent.md (decision frameworks), bwfc-scan.ps1/bwfc-migrate.ps1 (parameter docs).


 Team update (2026-03-03): ListView CRUD events  ItemCreated now fires per-item, ItemCommand fires for ALL commands before specific handlers  decided by Cyclops

 Team update (2026-03-03): Migration toolkit pivoted from 9-doc folder to single SKILL.md in Copilot skill format  decided by Jeffrey T. Fritz

- **Distributable BWFC Migration Skill (`.github/skills/bwfc-migration/SKILL.md`):**
  - Created single distributable Copilot skill file consolidating all migration toolkit content into one self-contained document.
  - **Key content decisions vs. internal webforms-migration skill:**
    1. **Self-contained and NuGet-first:** No references to internal repo scripts (`bwfc-scan.ps1`, `bwfc-migrate.ps1`), agents, or `.ai-team/` paths. BWFC comes from NuGet, not repo clone. Designed to be dropped into any project's `.github/skills/` folder.
    2. **Added three-layer methodology section:** Extracted from executive report — Layer 1 (mechanical, ~40%), Layer 2 (structural, ~45%), Layer 3 (architecture, ~15%) with expected page-readiness breakdown.
    3. **Added 10 architecture decision templates (NEW content):** Master Page→Layout, Session→Scoped Services, Identity→Blazor Identity, EF6→EF Core, Global.asax→Program.cs, Web.config→appsettings.json, DataSource→Service Injection, RouteTable→@page, Handlers/Modules→Middleware, Third-Party→HttpClient. Each has before/after code.
    4. **Added per-page migration checklist:** Layer-organized checkbox template from Forge's CHECKLIST design.
    5. **Expanded component coverage summary:** Added category counts, component list per category, and "What BWFC Does NOT Cover" table (DataSource controls, Wizard, Web Parts, AJAX Toolkit extenders).
    6. **Expanded common gotchas:** Added event handler signatures, TextMode casing, ScriptManager no-op guidance.
    7. **Removed WingtipToys-specific section:** Not applicable for a distributable skill — that content is project-specific.
  - Total: ~750 lines. Preserves ~90% of existing internal skill content, adds ~30% new content from design doc and executive report.


 Team update (2026-03-03): Themes (#369) implementation last  ListView CRUD first, WingtipToys features second, themes last  directed by Jeff Fritz


 Team update (2026-03-03): ListView EventArgs now include IOrderedDictionary properties (Keys, Values, OldValues, NewValues) for full Web Forms parity  decided by Cyclops

- **Migration toolkit doc fixes (component count + internal references):**
  - **Component count corrected:** Updated from "52 components across 7 categories" to "58 components across 6 categories" across all toolkit files (CONTROL-COVERAGE.md, SKILL.md, README.md). The tables already showed 55 items across 6 categories; added 3 missing components (Chart, ImageMap, Substitution) to the Editor Controls table to match the actual library inventory.
  - **Internal references removed:** Replaced all `../.github/skills/webforms-migration/SKILL.md` and `../.github/agents/migration.agent.md` references with distributed toolkit paths (`skills/bwfc-migration/SKILL.md`, `skills/bwfc-data-migration/SKILL.md`). Toolkit is designed for external projects — cannot reference internal repo paths.
  - **Wrong org name fixed:** Changed `AzimoLabs` → `FritzAndFriends` in copilot-instructions-template.md URLs and bwfc-scan.ps1.
  - **Files changed:** CONTROL-COVERAGE.md, METHODOLOGY.md, QUICKSTART.md, README.md, copilot-instructions-template.md, bwfc-scan.ps1, skills/bwfc-migration/SKILL.md.
  - **Key learning:** When new components are added to the library (like M17 AJAX controls), the migration toolkit coverage tables AND summary counts must both be updated. The "Migration Helper" 7th category was a phantom — no table section existed for it.

- **Migration test report structure (`docs/migration-tests/`):**
  - Created `docs/migration-tests/` as the standard location for migration benchmark reports.
  - **README.md** — Index page with table linking to individual test runs (Run, Source App, Date, Pages, Result columns).
  - **wingtiptoys-2026-03-04/report.md** — Template for the first benchmark run with sections: Summary metrics table, Methodology (three-layer pipeline), Phase Timing table (7 phases + total), Layer 1a–3 detail sections, Verification (build + screenshots), Before/After comparison, Conclusions.
  - **wingtiptoys-2026-03-04/images/.gitkeep** — Placeholder for screenshots.
  - Added "Migration Tests" nav section to `mkdocs.yml` after the Migration section.
  - **Convention established:** Each migration test run gets its own subfolder named `{app}-{YYYY-MM-DD}` containing `report.md` and an `images/` directory. The README.md index links to all runs.


 Team update (2026-03-04): Migration test reports go in docs/migration-tests/  directed by Jeffrey T. Fritz
 Team update (2026-03-04): PRs must target upstream FritzAndFriends repo, not origin fork  directed by Jeffrey T. Fritz
 Team update (2026-03-04): Layer 1 benchmark baseline established by Cyclops  data at docs/migration-tests/wingtiptoys-2026-03-04/  decided by Cyclops
