<!-- Archived 2026-03-06 by Scribe -->

### Core Context (2026-02-10 through 2026-02-27)

**Doc structure:** title ΓåÆ intro (MS docs link) ΓåÆ Features Supported ΓåÆ NOT Supported ΓåÆ Web Forms syntax ΓåÆ Blazor syntax ΓåÆ HTML Output ΓåÆ Migration Notes (Before/After) ΓåÆ Examples ΓåÆ See Also. Admonitions for gotchas. mkdocs.yml nav alphabetical within categories. Migration section: "Getting started" and "Migration Strategies" at top.

**Key patterns:** Style migration: TableItemStyle ΓåÆ CSS class string parameters. DeferredControls.md has dual role (fully deferred + partially implemented). Chart screenshots at `docs/images/{component}/chart-{type}.png`. Shared sub-component docs linked from parents. PagerSettings is first shared sub-component with own doc page. Structural components (no HTML output) lead with "renders no HTML" callout. Audit reports at `planning-docs/AUDIT-REPORT-M{N}.md` with historical snapshot headers. Branch naming: `copilot/create-*`.

**Doc work completed:** M1ΓÇôM3 docs (PasswordRecovery 3-step wizard, DetailsView generic component). Chart doc (JS interop "HTML Output Exception" pattern, Chart Type Gallery, child component doc pattern). M8 release-readiness polish (Substitution/Xml deferred in status.md, Chart Phase 1 hedging removed, README link fixes). M9 Doc Gap Audit (FormView, DetailsView, DataGrid, ChangePassword, PagerSettings.md created). ToolTip universality in Migration/readme.md. ThemesAndSkins.md updated for M10 PoC. NamingContainer.md created with IDRendering.md cross-refs. M9 Consolidated Audit Report (29 findings ΓåÆ M10 issues).

**Pending doc needs:** ClientIDMode property documentation (M16). Menu dual rendering modes. ListView CRUD events. Menu styles (IMenuStyleContainer). Post-M15 verification badges if new exact matches achieved. Login+Identity deferred ΓÇö do not schedule docs.

<!-- ΓÜá Summarized 2026-03-04 by Scribe ΓÇö covers M17 docs through migration toolkit -->

### Doc Work Summary (2026-02-27 through 2026-03-03)

**M17 AJAX docs (6 pages):** Timer, ScriptManager, ScriptManagerProxy, UpdatePanel, UpdateProgress, Substitution. New "AJAX Controls" nav section in mkdocs.yml. Migration stub doc pattern established (warning admonition + ignored props + includeΓåÆremove lifecycle). Substitution moved from deferred to implemented.

**Issue #359 doc updates (5 pages):** ChangePassword and PagerSettings verified complete. FormView got CRUD events + NOT Supported section. DetailsView got full style sub-component elements. DataGrid paging section enhanced. Pattern: DataGrid is the only pageable control without PagerSettings.

**M10 Skins & Themes Guide:** Created `docs/Migration/SkinsAndThemes.md` ΓÇö practical guide coexisting with `ThemesAndSkins.md` (strategy). Convention: separate "Guide" vs "Strategy" docs with clear nav labels.

**Executive Report:** `planning-docs/WINGTIPTOYS-MIGRATION-EXECUTIVE-REPORT.md` ΓÇö 96.6% coverage, 55-70% time savings, 18-26 hour estimate.

**Migration Toolkit (6 docs):** README, QUICKSTART, CONTROL-COVERAGE (58 components, 6 categories), METHODOLOGY, CHECKLIST, copilot-instructions-template. Key: no content duplication, copilot-instructions-template is self-contained for external projects.

**Distributable BWFC Migration Skill:** Single self-contained SKILL.md (~750 lines) with 10 architecture decision templates, three-layer methodology, per-page checklist. NuGet-first, no internal repo references.

**Toolkit fixes:** Component count 52ΓåÆ58, internal referencesΓåÆdistributed paths, AzimoLabsΓåÆFritzAndFriends. Key learning: toolkit coverage tables must be updated when new components are added.

**Migration test report structure:** `docs/migration-tests/` standard location. Per-run subfolder `{app}-{YYYY-MM-DD}` with `report.md` + `images/`. README.md index. Added "Migration Tests" nav section to mkdocs.yml.

**Pending doc needs:** ClientIDMode property. Menu dual rendering modes. ListView CRUD events. Menu styles (IMenuStyleContainer). Post-M15 verification badges.

### Key Team Updates (2026-02-27 through 2026-03-03)

- Branching: feature PRs from personal fork to upstream dev (Jeff)
- Issues closed via PR references only (Jeff)
- CascadedTheme (not Theme) is cascading parameter name (Cyclops)
- Theming sample page uses 6-section progressive layout (Jubilee)
- Unified release.yml ΓÇö single workflow, version.json 3-segment SemVer (PR #408)
- Skins & Themes roadmap: 3 waves, 15 work items (Forge)
- Project reframed as migration acceleration system (Jeff)
- Themes (#369) implementation last ΓÇö ListView CRUD first, WingtipToys second (Jeff)
- ListView EventArgs now include IOrderedDictionary properties (Cyclops)
- Migration toolkit restructured into self-contained migration-toolkit/ package (Jeff, Forge)


 Team update (2026-03-04): PRs must target upstream FritzAndFriends/BlazorWebFormsComponents, not the fork  decided by Jeffrey T. Fritz
 Team update (2026-03-04): Migration test reports go in docs/migration-tests/{subfolder}/  decided by Jeffrey T. Fritz
 Team update (2026-03-04): Layer 1 benchmark baseline established  data at docs/migration-tests/wingtiptoys-2026-03-04/  decided by Cyclops
 Team update (2026-03-04): Migration Run 2  11/11 features pass, toolkit ready for customer-facing documentation  decided by Forge

### Migration Report Conventions (2026-03-04)

- **Image path depth**: Reports in `docs/migration-tests/{app}-{run}/report.md` are 3 levels deep from repo root. Relative paths to `planning-docs/` must use `../../../planning-docs/`, not `../../planning-docs/`. This is a common off-by-one error to watch for.
- **Executive summary pattern**: Migration reports should open with a concise paragraph summarizing enhancements tested, pass/fail, and key deltas from prior runs, followed by a quick-reference metrics table (8ΓÇô10 rows). Executives should grasp the full picture in Γëñ10 seconds.
- **Run 4 report location**: `docs/migration-tests/wingtiptoys-run4-2026-03-04/report.md` with local `images/` subfolder for Blazor screenshots and cross-references to `planning-docs/screenshots/` for original Web Forms screenshots.

 Team update (2026-03-05): GetRouteUrl RouteValueDictionary overloads now functional  all 4 overloads match Web Forms API  decided by Cyclops

### Run 5 Benchmark Report (2026-03-05)

- **Report written:** `docs/migration-tests/wingtiptoys-run5-2026-03-04/report.md` ΓÇö comprehensive 9-section report with executive summary, metrics comparison table, what-works/what-doesn't breakdown, enhancement impact analysis, Layer 2 fixes summary, build results, gap analysis, and recommendations.
- **Key convention reinforced:** When manual review item counts increase between runs, explain *why* in the report (granular flagging vs regression). Jeff needs to see that higher counts can mean better output quality.
- **Report structure evolution:** Run 5 report adds "What Works" and "What Doesn't Work" sections (Jeff's request) plus categorization of manual items by difficulty ("mechanical but tedious" vs "requires architectural decisions"). This pattern should carry forward to future runs.
- **Enhancement impact table pattern:** Per-enhancement rows with Fired/Count/Run4-impact/Run5-status/Net-impact columns. Effective for showing ROI of individual script improvements.

 Team update (2026-03-04): Run 5 migration complete  309 transforms, clean build (0 errors, 0 warnings, 4.56s). Benchmark report pending. Key artifacts at docs/migration-tests/wingtiptoys-run5-2026-03-04/  decided by Cyclops

 Team update (2026-03-04): Migration standards formalized  EF Core, .NET 10, ASP.NET Core Identity, BWFC event handler preservation. Documentation priorities: document single-item FormView usage, document ListView Items parameter in migration context. migration-toolkit/ is canonical home.  decided by Jeffrey T. Fritz, Forge

≡ƒôï Team update (2026-03-04): Run 6 improvement analysis  decided by Forge

### Run 6 Benchmark Report (2026-03-04)

- **Report written:** `docs/migration-tests/wingtiptoys-run6-2026-03-04/report.md` ΓÇö comprehensive 9-section report matching Run 5 format with executive summary, Run 5 vs Run 6 metrics comparison, what-works/what-doesn't breakdown, enhancement impact analysis, Layer 2 fixes summary, build results (4 rounds), gap analysis (2 script bugs), and recommendations.
- **Key data:** 55% total time reduction (Run 5 ~10 min ΓåÆ Run 6 ~4.5 min). Layer 2 manual time dropped 53% (440s ΓåÆ 205s). 4 enhancements all fired. 269 transforms, 79 static files to wwwroot/, 6 auto-stubs.
- **Format evolution:** Run 6 report adds explicit "Script Bugs Found" table in Gaps section (separate from "Patterns That Could Be Enhanced"). This distinguishes regressions/bugs from enhancement opportunities ΓÇö important for prioritizing Run 7 fixes.
- **Transform count can decrease:** Run 6 had fewer transforms than Run 5 (269 vs 309) because auto-stubbing replaces full transforms for unconvertible pages. Reports should explain count decreases as quality improvements, not regressions.
- **Build rounds can increase without regression:** Run 6 had 4 build rounds vs Run 5's 2, but for entirely different root causes (NuGet auth, @rendermode bug). Reports should contextualize build round counts with root cause analysis.
- **Highest-impact enhancement pattern:** SelectMethod BWFC-aware guidance changed the migration *approach* (preserve components vs replace with HTML), not just the speed. Enhancement impact sections should capture qualitative shifts, not just time savings.

 Team update (2026-03-04): Run 6 benchmark validates all migration-standards SKILL.md patterns. 32 Web Forms files  clean Blazor build in ~4.5 min (55% reduction from Run 5). 2 script bugs identified: @rendermode in _Imports.razor, stub detection misses code-behind.  decided by Forge

### Render Mode Placement Correction (2026-03-05)

- **Key learning:** `@rendermode` is a directive *attribute* (goes on component instances like `<Routes @rendermode="InteractiveServer" />`), NOT a standalone directive. Placing `@rendermode InteractiveServer` in `_Imports.razor` causes build errors (RZ10003, CS0103, RZ10024).
- **Correct pattern:** `_Imports.razor` gets `@using static Microsoft.AspNetCore.Components.Web.RenderMode` (enables shorthand). `App.razor` gets `@rendermode="InteractiveServer"` on both `<Routes>` and `<HeadOutlet>`.
- **Files updated:** `migration-toolkit/skills/migration-standards/SKILL.md` (new "Render Mode Placement" subsection under Target Architecture), `migration-toolkit/skills/bwfc-migration/SKILL.md` (Step 2 expanded with `@using static` + new Step 2b for App.razor), `migration-toolkit/METHODOLOGY.md` (scaffold table includes App.razor).
- **Reference:** https://learn.microsoft.com/aspnet/core/blazor/components/render-modes
- **Source:** Jeff confirmed correct pattern; Microsoft Learn docs verified.


 Team update (2026-03-04): @rendermode InteractiveServer belongs in App.razor, not _Imports.razor  consolidated from Forge, Cyclops, Jeffrey T. Fritz (PR #419)


 Team update (2026-03-04): EF Core must use 10.0.3 (latest .NET 10)  directed by Jeff


## Archived 2026-03-12 (entries from 2026-03-11 through 2026-03-12)

### Migration Pipeline Enforcement in Skill Docs (2026-03-11)

- **Scope:** Updated `migration-toolkit/skills/bwfc-migration/SKILL.md` and `migration-toolkit/skills/migration-standards/SKILL.md` per Jeff Fritz directive.
- **bwfc-migration SKILL.md changes:**
  - Added prominent "Migration Pipeline ΓÇö MANDATORY" section near the top (after Installation, before existing Migration Workflow) with critical warning admonition, pipeline step table, Layer 1 invocation command, Layer 2 Copilot transform checklist, and pipeline rules.
  - Fixed all `TItem` references ΓåÆ `ItemType` throughout the file (lines 262, 274, 294, 334, 355, 479, 490, 624). BWFC standardized on `ItemType` to match Web Forms `DataBoundControl.ItemType`.
  - Updated Layer 2 checklist: "ItemType ΓåÆ TItem" ΓåÆ "ItemType preserved (strip namespace prefix only)".
  - Updated GridView Key changes note: removed incorrect "ItemType ΓåÆ TItem" guidance.
- **migration-standards SKILL.md changes:**
  - Renamed "Layer 1 (Script) vs Layer 2 (Manual) Boundary" ΓåÆ "Layer 1 (Script) vs Layer 2 (Copilot-Assisted) Boundary".
  - Added critical warning admonition about no manual fixes between layers.
  - Added `bwfc-migrate.ps1` invocation command.
  - Expanded Layer 2 description: "Always manual" ΓåÆ "Copilot-Assisted" with additional transform items (data loading, template context, navigation).
- **Key convention:** `ItemType` is the canonical attribute name for BWFC data controls. Never use `TItem`, `TItemType`, or other variants.
- **Key convention:** Layer 1 = automated script, Layer 2 = Copilot-assisted. No manual fixes between layers. This is a measurement integrity requirement.


 Team update (2026-03-11): Mandatory L1L2 migration pipeline  no code fixes between layers. Both layers must run in sequence.  decided by Jeffrey T. Fritz

 Team update (2026-03-11): All generic type params standardized to ItemType (not TItem/TItemType) across all BWFC data-bound components.  decided by Jeffrey T. Fritz

 Team update (2026-03-11): P0 script fixes  Test-UnconvertiblePage eliminated (always convert), [Parameter] annotation bug fixed.  decided by Jeffrey T. Fritz

### SelectMethod Skills Fix + Run 20 Report Corrections (2025-07-24)

- **Scope:** Fixed incorrect SelectMethod guidance across all three migration skill files and corrected factual errors in Run 20 report.
- **FIX 1 ΓÇö SelectMethod guidance (all 3 skill files):**
  - `bwfc-migration/SKILL.md`: Updated 8 locations ΓÇö Layer 2 bullet, GridView/ListView/FormView examples, Data Binding Migration tables, and per-page checklist. All "SelectMethod ΓåÆ Items" guidance changed to "SelectMethod PRESERVED ΓÇö convert string to SelectHandler<ItemType> delegate."
  - `migration-standards/SKILL.md`: Updated 3 locations ΓÇö main SelectMethod guidance paragraph, Layer 2 description, and ListView before/after example (now shows both SelectMethod delegate and Items options).
  - `bwfc-data-migration/SKILL.md`: Updated 4 locations ΓÇö When to Use description, EF6 context, SelectMethod mapping table (now shows delegate conversion + alternative Items approach), and Files table. Also fixed stray `TItem` ΓåÆ `ItemType`.
- **FIX 2 ΓÇö Run 20 Report validator claim:** Removed false bullet stating RequiredFieldValidator, CompareValidator, RegularExpressionValidator, and ModelErrorMessage are "not yet implemented." All exist in `src/BlazorWebFormsComponents/Validations/`.
- **FIX 3 ΓÇö Run 20 Report SelectMethod:** Updated L2 data binding description and L1 review items appendix to reflect SelectMethod delegate conversion instead of IDbContextFactory replacement.
- **FIX 4 ΓÇö GetRouteUrlHelper and ContentPlaceHolder:** Added GetRouteUrlHelper documentation to bwfc-migration route URL section. ContentPlaceHolder/Content/MasterPage already well-documented at lines 593, 709-711.
- **Key learning:** `DataBoundComponent<ItemType>.SelectMethod` is a `SelectHandler<ItemType>` delegate parameter, not a string. BWFC's `OnAfterRenderAsync` auto-populates `Items` when `SelectMethod` is set. This is the native BWFC data-binding path that mirrors Web Forms behavior.


 Team update (2026-03-11): SelectMethod must be preserved in L1 script and skills  BWFC supports it natively via SelectHandler<ItemType> delegate. All validators exist in BWFC.

### Database Provider Detection Framing (2025-07-24)

- **Scope:** Reframed database guidance in all three migration skill files from reactive ("don't use SQLite") to proactive ("detect and match the original provider").
- **migration-standards/SKILL.md:** Replaced "NEVER substitute database providers" bullet with "Detect and match the original database provider" ΓÇö leads with the `Web.config` `<connectionStrings>` detection workflow and references L1's `[DatabaseProvider]` review item. NEVER-substitute guardrail retained at end.
- **bwfc-data-migration/SKILL.md:** Prepended "Step 1: Detect the provider" blockquote above existing CRITICAL/NEVER warnings. References L1's `Find-DatabaseProvider` function and `[DatabaseProvider]` review item. Existing warnings preserved unchanged.
- **bwfc-migration/SKILL.md:** Added "Database provider" bullet to L2 checklist directing agents to verify L1-detected provider from `[DatabaseProvider]` review item.
- **Key learning:** Skill file tone matters for agent behavior. "Detect and match X" (positive/proactive) is more effective than "NEVER use Y" (negative/reactive) because agents prioritize affirmative instructions over prohibitions.

### SelectMethod & SQLite Enforcement in Skill Files (2025-07-24)

- **Scope:** Hardened all three migration skill files to prevent two recurring agent mistakes.
- **Fix 1 ΓÇö SQLite contamination:** Removed "Prefer SQLite for local dev / demos" from `migration-standards/SKILL.md` (line 93). This single sentence was the root cause of agents defaulting to SQLite instead of preserving the original SQL Server LocalDB provider. Added "NEVER default to SQLite" warnings in both `migration-standards` and `bwfc-data-migration` SKILL files.
- **Fix 2 ΓÇö SelectMethodΓåÆItems regression:** Removed the "Alternatively, bypass SelectMethod and set Items directly" sentence from `migration-standards/SKILL.md`. Replaced with a WARNING admonition making SelectMethod preservation mandatory. In `bwfc-data-migration/SKILL.md`, restricted Option B (Items= binding) to DataSource-originating patterns only. In `bwfc-migration/SKILL.md`, added MANDATORY warning before the SelectMethod bullet and clarified that Items should not be set when SelectMethod is active.
- **Patterns that caused regression:**
  1. Agents read "Prefer SQLite for local dev" and interpreted it as a default, ignoring the original app's SQL Server connection strings.
  2. Agents read "Alternatively... set Items directly" and chose the simpler path, converting SelectMethod to Items= and losing the native BWFC data-binding pattern.
- **Key learning:** Skill file wording must be prescriptive with explicit NEVER/MUST admonitions. "Alternatively" or "prefer X" phrasing gives agents permission to deviate. Warnings must be impossible to miss.


 Team update (2026-03-11): NEVER default to SQLite; SelectMethod MUST be preserved as SelectHandler<ItemType> delegates. All skill files hardened, L1 script now scaffolds SQL Server.  decided by Jeffrey T. Fritz, Beast, Cyclops


 Team update (2026-03-11): Database provider guidance reframed  lead with 'detect and match original provider' instead of 'NEVER substitute'. L1 auto-detection connects to L2 verification.  decided by Beast (requested by Jeffrey T. Fritz)


 Team update (2026-03-12): Database provider auto-detection consolidated  Jeff directive + Beast skill reframe + Cyclops Find-DatabaseProvider implementation merged into single decision. Skill file guidance leads with 'detect and match'.  decided by Jeffrey T. Fritz, Beast, Cyclops

### Executive Summary Update ΓÇö Runs 19-21 (2026-03-12)

- **Scope:** Updated `dev-docs/migration-tests/EXECUTIVE-SUMMARY.md` with data from WT Run 20, WT Run 21, and CU Run 19.
- **Run count:** 38 ΓåÆ 40 benchmark runs (WT=21, CU=19).
- **Results at a Glance table:** Updated benchmark runs, control usages (420+), added L1 Provider Detection row, updated CU SelectMethod status to "Items= binding", updated key takeaway.
- **Performance table:** Added "Latest Run" column showing WT Run 21 (1.79s) and CU Run 19 (0.62s). Explained marginal L1 time increase due to richer transforms.
- **Milestones table:** Added CU Run 19 (SQL Server preservation), WT Run 20 (zero-error pipeline), WT Run 21 (SelectMethod delegates).
- **Two-Layer Pipeline section:** Added `Find-DatabaseProvider` feature description with `[DatabaseProvider]` review item.
- **L1 Performance line:** Added Run 20 (1.70s) and CU Run 19 (0.62s) data points.
- **CU screenshots section:** Clarified Run 15 as visual reference, added Run 19 context (SQL Server preservation, 0 errors, 229 output files).
- **Test Project Coverage table:** Updated benchmark runs (WT=21, CU=19), CU best result mentions SQL Server preservation.
- **What's Next section:** Marked SelectMethod core as Γ£à done, added CU SelectMethod re-run and acceptance test validation items.
- **Charts:** Added WT Run 20 (1.70s), WT Run 21 (1.79s), CU Run 19 (0.62s) to `generate-charts.py` and regenerated all 3 PNGs.
- **Key data points:** WT Run 21: 1.79s L1, 348 transforms, 0 errors, SelectHandler<ItemType> delegates on 3 core pages. CU Run 19: 0.62s L1, 72 transforms, 0 errors, SQL Server LocalDB preserved, 229 output files, 5 BLL classes.