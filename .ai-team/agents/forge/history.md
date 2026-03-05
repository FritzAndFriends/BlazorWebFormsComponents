# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents  Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Core Context

<!-- Summarized 2026-03-04 by Scribe — originals in history-archive.md -->

M1–M16: 6 PRs reviewed, Calendar/FileUpload rejected, ImageMap/PageService approved, ASCX/Snippets shelved. M2–M3 shipped (50/53 controls, 797 tests). Chart.js for Chart. DataBoundStyledComponent<T> recommended. Key patterns: Enums/ with int values, On-prefix events, feature branches→upstream/dev, ComponentCatalog.cs. Deployment: Docker+NBGV, dual NuGet, Azure webhook. M7–M14 milestone plans. HTML audit: 3 tiers, M11–M13. M15 fidelity: 132→131 divergences, 5 fixable bugs. Data controls: 90%+ sample parity, 4 remaining bugs. M17 AJAX: 6 controls shipped.

## Learnings


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
### Event Handler Migration Audit (2026-03-05)

**Jeff's question:** "Investigate event handler migration — OnClick, OnSelectedIndexChanged — we should have event handlers on all BWFC components."

**Key findings:**

1. **Naming convention split is the #1 migration blocker.** ~50 EventCallbacks across GridView, DetailsView, FormView, ListView, DataGrid, Menu, TreeView use bare names (e.g., `Sorting`, `SelectedIndexChanged`, `ModeChanging`) instead of `On`-prefixed names matching Web Forms attributes. When `bwfc-migrate.ps1` strips `asp:` and leaves `OnSorting="Handler"`, these won't compile against the BWFC components.

2. **Button controls, input controls, list controls, login controls — all good.** These consistently use `On`-prefix naming and match Web Forms attributes exactly. ButtonBaseComponent (OnClick, OnCommand), TextBox (OnTextChanged), CheckBox (OnCheckedChanged), DropDownList (OnSelectedIndexChanged), all login controls — zero friction.

3. **Repeater has zero EventCallbacks.** DataList is missing 7 of 8 Web Forms events. Both are migration blockers for apps that use OnItemCommand or OnItemDataBound on these controls.

4. **GridView still missing OnRowDataBound, OnRowCreated** (flagged in Run 5, still not implemented). Also missing OnPageIndexChanging, OnRowUpdated, OnRowDeleted.

5. **FormView has inconsistent naming** — OnItemDeleting/OnItemDeleted/OnItemInserting/etc. use `On`-prefix, but ModeChanging/ModeChanged/ItemCommand/ItemCreated/PageIndexChanging/PageIndexChanged do NOT. This is confusing and will cause partial migration failures.

6. **Migration script does zero event handler transformation** — it passes attributes through unchanged after stripping `asp:`. This is correct behavior IF components use `On`-prefix naming. The fix belongs in the components, not the script.

7. **CustomValidator missing OnServerValidate** — the one validation control that had a meaningful server event doesn't have it in BWFC.

**Recommendation:** Add `On`-prefix aliases to all 50 mismatched EventCallbacks (non-breaking). Implement missing events on Repeater, DataList, GridView. Decision doc: `.ai-team/decisions/inbox/forge-event-handler-audit.md`.

 Team update (2026-03-05): Event handler audit complete — 50 naming mismatches, 18 missing events, Repeater/DataList critically underserved. Option A (On-prefix aliases) recommended.  decided by Forge


 Team update (2026-03-05): 50 On-prefix EventCallback aliases added to data components + migration script AutoPostBack fix  by Cyclops, Rogue

### ShoppingCart GridView Feature-Gap Analysis (2026-03-05)

**Jeff's question:** "I lost my editable cart with row stripes. What was lost in the migration? Does BWFC GridView have gaps?"

**Key findings:**

1. **AfterWingtipToys/ShoppingCart.razor is a plain HTML table** — it does NOT use the BWFC GridView. This is exactly the anti-pattern documented in migration-standards: "Replacing BWFC Data Controls with Raw HTML."

2. **Lost features in AfterWingtipToys:** Editable TextBox for quantity (now read-only), CheckBox for item removal (gone), Update button (gone), Checkout ImageButton (gone), CssClass stripes+borders (degraded to just `table`), ShowFooter, GridLines, CellPadding, BoundField columns, ProductID column, Label components for totals. The cart is **read-only** — users cannot edit or check out.

3. **BWFC GridView supports ALL needed features:** CssClass ✅ (via BaseStyledComponent), AutoGenerateColumns="False" with Columns ✅, BoundField (with dotted DataField, DataFormatString) ✅, TemplateField with ItemTemplate (RenderFragment<T> with `context`) ✅, TextBox inside TemplateField ✅, CheckBox inside TemplateField ✅, ShowFooter ✅, GridLines ✅, CellPadding ✅. Zero component gaps for this page.

4. **FreshWingtipToys proves it works** — correctly uses GridView with all original features preserved. This is the reference implementation.

5. **Root cause:** The migration pipeline (Layer 1 script or Layer 2 Copilot) decomposed the GridView into raw HTML instead of preserving it as a BWFC component. Script fix needed: preserve `<asp:GridView>` structure, strip `asp:` prefix only, convert ItemType→TItem, SelectMethod→Items.

**Decision document:** `.ai-team/decisions/inbox/forge-gridview-gap.md`

 Team update (2026-03-05): ShoppingCart GridView gap analysis complete — AfterWingtipToys is a broken plain-HTML table, BWFC GridView has zero gaps for this page, migration scripts need GridView preservation rules.  decided by Forge

 Team update (2026-03-05): AfterWingtipToys must only be produced by migration toolkit output, never hand-edited  decided by Jeffrey T. Fritz

### BWFC Control Preservation Standards (2026-03-05)

**Jeff's directive:** "We need to ALWAYS preserve the default asp: controls by using the BWFC components."

**Actions taken:**

1. **Migration standards SKILL.md updated** — Added mandatory "BWFC Control Preservation" section at top of patterns with:
   - 5 explicit rules: preserve all asp: controls, never flatten data controls, never flatten editor controls, never flatten navigation/structural controls, post-transform verification catches loss
   - ShoppingCart GridView documented as concrete anti-pattern (flattened to read-only HTML table)
   - BAD vs GOOD code examples showing flattened GridView vs preserved BWFC GridView with BoundField/TemplateField
   - Explanation of why this matters (CSS preservation, feature parity, migration velocity, fidelity guarantee)

2. **`Test-BwfcControlPreservation` function added to bwfc-migrate.ps1** — Post-transform verification that:
   - Counts asp: tags in source, counts BWFC component tags in output
   - Warns if output has fewer BWFC components than input had asp: tags (control deficit)
   - Identifies specific controls that may be missing by name
   - Runs after all transforms in Convert-WebFormsFile, before file write
   - Non-blocking: emits warnings in ManualItems report, does not prevent output

3. **Confidence confirmed at "high"** — already set; this pattern is battle-tested across 6 WingtipToys runs with 4 confirmed gotchas (ListView templates, service registration, OnParametersSetAsync, GridView preservation).

**Key learning:** The migration script pipeline (ConvertFrom-AspPrefix) correctly preserves controls mechanically. The flattening problem occurs when humans or AI do Layer 2 work and rewrite controls as raw HTML instead of keeping the BWFC components. The verification function catches this in the migration report.

### Distributable Skills — Control Preservation Propagation (2026-03-05)

**Task:** Propagate BWFC control preservation rules from internal `.ai-team/skills/migration-standards/SKILL.md` to the distributable `migration-toolkit/skills/` directory.

**Actions taken:**

1. **`migration-toolkit/skills/migration-standards/SKILL.md`** — Added full "⚠️ BWFC Control Preservation — MANDATORY" section (all 5 rules, ShoppingCart concrete example, BAD/GOOD code blocks, Why This Matters) after Context and before Patterns. Bumped confidence from "medium" to "high" — battle-tested across 6 WingtipToys runs.

2. **`migration-toolkit/skills/bwfc-migration/SKILL.md`** — Added concise "⚠️ Control Preservation — Critical Rule" warning section after "What Is BWFC?" and before Installation, with cross-reference to migration-standards SKILL.md. Added "Never Flatten Controls" gotcha entry to Common Gotchas with BAD/RIGHT one-liner examples.

3. **No `.ai-team/` files modified** — internal versions already had the content.

**Key learning:** Distributable skills in `migration-toolkit/skills/` must stay in sync with internal `.ai-team/skills/` — they serve different audiences (external users vs internal team) but must carry the same rules. The migration-standards skill is the authoritative source; bwfc-migration keeps a concise pointer to avoid duplication.

 Team update (2026-03-05): BWFC control preservation is mandatory  all asp: controls must be preserved as BWFC components in migration output, never flattened to raw HTML. Test-BwfcControlPreservation verifies automatically.  decided by Jeffrey T. Fritz, implemented by Forge

