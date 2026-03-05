# Colossus — History

<!-- ⚠ Summarized 2026-02-28 by Scribe — entries before 2026-02-26 archived below as summaries. Full early history in history-archive.md -->

## Core Context

Integration test engineer. Built test coverage from M1 through M19. 130+ integration tests (smoke + interaction) covering all milestone sample pages. Key patterns established: `WaitUntilState.DOMContentLoaded` for async-bound components, `Filter(HasTextString)` for specific element targeting, ISO timestamp filtering for console errors, `PressSequentiallyAsync` + Tab for Blazor Server inputs. LoginControls pages require `@using BlazorWebFormsComponents.LoginControls`. Never use external image URLs. Full early history in `history-archive.md`.

## Key Learnings (Consolidated)

- FormView/ListView bind data in `OnAfterRenderAsync`/`OnAfterRender` — use `DOMContentLoaded` + `WaitForSelectorAsync`.
- Menu interaction tests: skip console error checks (JS interop produces expected errors in headless Playwright).
- Playwright `text=` locator matches innermost element — use `Filter(HasTextString)` on parent container instead.
- For strict-mode violations with duplicate text, target specific element (e.g., `page.Locator("td").Filter(...).First`).
- Use specific selectors like `button:has-text('Edit')` instead of generic selectors to avoid premature wait resolution.
- When sample data models change, interaction test assertions must be updated in lockstep (smoke tests won't catch text mismatches).
- Panel/BackImageUrl has external URLs — smoke test sufficient, no interaction test needed.
- Timer interaction test needs 3-second wait for 2000ms interval tick.
- AJAX controls form a natural test category group.

## Summary: M1–M9 (archived)

Covered milestones 1–9: initial smoke tests, Calendar/Chart/FileUpload/ImageMap integration tests, Sprint 2–3 components, M7 data controls (GridView, TreeView, Menu, DetailsView, FormView — 9 smoke + 9 interaction). M9 audit found 105 routes, 100 covered, 5 gaps identified.

## Summary: Issue #358 — Smoke + Interaction Tests (2026-02-25 to 2026-02-27)

Added 5 smoke test InlineData entries (M9 audit gaps: ListView/CrudOperations, Label, Panel/BackImageUrl, LoginControls/Orientation, DataGrid/Styles). Later added 5 interaction tests: ListView CRUD (2 tests), Label AssociatedControlID, DataGrid Styles, LoginControls Orientation. Panel/BackImageUrl skipped (static). All gaps closed.

## Summary: PR #377 DetailsView Integration Test Fix (2026-02-26)

Fixed 5 stale Customer→Product assertions in InteractiveComponentTests.cs after DetailsView sample pages migrated to Product model. All 7 DetailsView integration tests passing.

## Summary: M17 AJAX Control Integration Tests (2026-02-27)

Added 5 smoke tests (Timer, UpdatePanel, UpdateProgress, ScriptManager, Substitution) as `AjaxControl_Loads_WithoutErrors` Theory group. Added 1 interaction test for Timer auto-increment. Build green.

## Team Updates (Current)

📌 Team update (2026-02-26): WebFormsPage unified wrapper — inherits NamingContainer, adds Theme cascading — decided by Jeffrey T. Fritz, Forge
📌 Team update (2026-02-26): SharedSampleObjects is the single source for sample data parity — decided by Jeffrey T. Fritz
📌 Team update (2026-02-26): M15 HTML fidelity strategy — full audit pipeline re-run assigned to Colossus — decided by Forge
📌 Team update (2026-02-27): Branching workflow directive — feature PRs from personal fork to upstream dev — decided by Jeffrey T. Fritz
📌 Team update (2026-02-27): Issues must be closed via PR references using 'Closes #N' syntax — decided by Jeffrey T. Fritz
📌 Team update (2026-02-27): M17 AJAX controls implemented — decided by Cyclops
📌 Team update (2026-02-27): M17 audit fixes resolved — 5 fidelity issues, 9 new tests, PR #402 — decided by Forge, Cyclops
📌 Team update (2026-02-27): Timer duplicate [Parameter] bug fixed; 47 M17 tests — decided by Rogue
📌 Team update (2026-02-28): Cyclops fixed CheckBox bare input missing id attribute — integration tests targeting CheckBox by id may now work in no-text scenarios. All 5 M9 audit gap pages now have interaction test coverage.

 Team update (2026-03-01): Normalizer pipeline order is fixed  regex rules  style norm  empty style strip  boolean attrs  GUID IDs  attr sort  artifact cleanup  whitespace. Case-insensitive file pairing enabled  decided by Cyclops
 Team update (2026-03-01): D-11 through D-14 formally registered. D-12 boolean attrs and GUID IDs now handled by normalizer  decided by Forge
📌 Team update (2026-03-02): FontInfo.Name/Names now auto-synced bidirectionally. Theme font-family renders correctly. Integration tests targeting font-family should now work — decided by Cyclops, Rogue
📌 Team update (2026-03-02): CascadedTheme (not Theme) is the cascading parameter name on BaseWebFormsComponent — decided by Cyclops

 Team update (2026-03-02): Unified release process implemented  single release.yml triggered by GitHub Release publication coordinates all artifacts (NuGet, Docker, docs, demos). version.json now uses 3-segment SemVer (0.17.0). Existing nuget.yml and deploy-server-side.yml are workflow_dispatch-only escape hatches. PR #408  decided by Forge (audit), Cyclops (implementation)


 Team update (2026-03-02): M22 Copilot-Led Migration Showcase planned  decided by Forge

 Team update (2026-03-02): WingtipToys migration analysis complete  36 work items across 5 phases, FormView RenderOuterTable is only blocking gap  decided by Forge

 Team update (2026-03-02): Project reframed  final product is a migration acceleration system (tool/skill/agent), not just a component library. WingtipToys is proof-of-concept.  decided by Jeffrey T. Fritz

## Learnings

### ModelErrorMessage Integration Tests (added by task request)
- Added 1 smoke test `[InlineData]` for `/ControlSamples/ModelErrorMessage` in the `ValidationControl_Loads_WithoutErrors` Theory group.
- Added 3 interactive tests in `InteractiveComponentTests.cs`:
  - `ModelErrorMessage_Submit_ShowsErrors` — submits empty form, asserts `span.text-danger` appears with error text.
  - `ModelErrorMessage_ValidSubmit_NoErrors` — fills matching valid passwords, asserts no error spans and success message appears.
  - `ModelErrorMessage_ClearButton_RemovesErrors` — triggers errors then clicks Clear, asserts errors are removed from DOM.
- The ModelErrorMessage component renders nothing when no errors exist (conditional `@if`), so error-gone assertions use `CountAsync() == 0` rather than visibility checks.
- For the Clear button test, used `WaitForSelectorAsync` with `State.Hidden` to reliably wait for Blazor re-render after clearing the EditContext.
- `PressSequentiallyAsync` + `Tab` pattern used for Blazor Server InputText fields, consistent with established team conventions.

 Team update (2026-03-03): ListView CRUD events  ItemCreated now fires per-item, ItemCommand fires for ALL commands before specific handlers  decided by Cyclops

 Team update (2026-03-03): Themes (#369) implementation last  ListView CRUD first, WingtipToys features second, themes last  directed by Jeff Fritz


 Team update (2026-03-03): WingtipToys 7-phase feature schedule established  26 work items, critical path through Data Foundation  Product Browsing  Shopping Cart  Checkout  Polish  decided by Forge


 Team update (2026-03-03): ListView CRUD test conventions established  43 tests, event ordering via List<string>, cancellation assertions, bUnit double-render handling  decided by Rogue


 Team update (2026-03-04): PRs must target upstream FritzAndFriends/BlazorWebFormsComponents, not the fork  decided by Jeffrey T. Fritz

 Team update (2026-03-05): Migration report image paths must use ../../../ (3-level traversal) for repo-root assets  decided by Beast

 Team update (2026-03-05): BWFC control preservation is mandatory  all asp: controls must be preserved as BWFC components in migration output, never flattened to raw HTML. Test-BwfcControlPreservation verifies automatically.  decided by Jeffrey T. Fritz, implemented by Forge

