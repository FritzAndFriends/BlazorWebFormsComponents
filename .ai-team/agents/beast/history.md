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

