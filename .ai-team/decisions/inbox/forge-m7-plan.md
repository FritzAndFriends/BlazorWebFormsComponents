# Decision: Milestone 7 Plan Ratified

**Date:** 2026-02-23
**Author:** Forge (Lead / Web Forms Reviewer)
**Status:** Proposed
**Scope:** Milestone 7 planning — "Control Depth & Navigation Overhaul"

## Context

Milestone 6 closed ~345 gaps across 54 work items, primarily through sweeping base class fixes (AccessKey, ToolTip, DataBoundComponent style inheritance, Validator Display/SetFocusOnError, Image/Label base class upgrades) and targeted control improvements (GridView paging/sorting/editing, Calendar styles+enums, FormView header/footer/empty, HyperLink rename, ValidationSummary, ListControl improvements, Menu Orientation, Label AssociatedControlID, Login base class upgrade).

The remaining gaps are in the "long tail" — style sub-components, complex event pipelines, and navigation control completeness. The audit docs in `planning-docs/` are stale (reflect pre-M6 state).

## Decision

Milestone 7 targets ~138 gap closures across 51 work items, organized as:

### P0 — Re-Audit + GridView Completion (10 WIs, ~23 gaps)
- **Re-audit all 53 controls** (mandatory, opens milestone)
- **GridView selection**: SelectedIndex, SelectedRow, SelectedRowStyle, AutoGenerateSelectButton, events
- **GridView style sub-components**: AlternatingRowStyle, RowStyle, HeaderStyle, FooterStyle, etc.
- **GridView display properties**: ShowHeader/ShowFooter, Caption, EmptyDataTemplate, GridLines

### P1 — Navigation + Data Control Depth (30 WIs, ~67 gaps)
- **TreeView**: Node-level styles (TreeNodeStyle), selection, ExpandAll/CollapseAll, ExpandDepth
- **Menu**: Base class → BaseStyledComponent, selection+events, core missing props
- **DetailsView**: Style sub-components (10 styles), PagerSettings, Caption
- **FormView**: Remaining events (ModeChanged, ItemCommand, paging), style sub-components, PagerSettings
- **Validators**: ControlToValidate string ID support (migration-critical)
- **Integration tests** for all updated controls

### P2 — Nice-to-Have (11 WIs, ~48 gaps)
- **ListView CRUD events** (large effort, ~22 gaps)
- **DataGrid style sub-components + events** (~18 gaps)
- **Menu level styles**, Panel BackImageUrl, Login/ChangePassword Orientation

## Rationale

1. GridView at ~55% is still the most-used data control — completing selection and styles is highest-impact.
2. Menu (42%) and TreeView (60%) are the weakest non-deferred controls.
3. Style sub-components are the biggest systematic remaining gap across data controls.
4. Validator ControlToValidate string ID is a migration-blocking API mismatch.
5. PagerSettings should be a shared type across GridView/FormView/DetailsView.
6. ListView CRUD is P2 due to size (L) and lower usage frequency vs. GridView.

## Scoping Rules (unchanged)
- Substitution, Xml: intentionally deferred
- Chart advanced properties: intentionally deferred
- DataSourceID/model binding: N/A in Blazor

## Impact
- Overall health: ~82% → ~87-90%
- GridView: ~55% → ~75%
- TreeView: ~60% → ~75%
- Menu: ~42% → ~60-65%
- FormView: ~50% → ~65%
- DetailsView: ~70% → ~80%

Full plan in `planning-docs/MILESTONE7-PLAN.md`.
