# Decision: M7 Integration Tests Added (WI-39 + WI-40)

**Author:** Colossus
**Date:** 2026-02-24
**Status:** Done

## Context

Milestone 7 added 9 new sample pages across GridView, TreeView, Menu, DetailsView, and FormView. Each page needed smoke tests (page loads without errors) and, where applicable, interaction tests (behaviors work).

## What Was Added

### Smoke Tests (ControlSampleTests.cs)

Added `[InlineData]` entries to existing `[Theory]` methods:

- **DataControl_Loads_WithoutErrors:**
  - `/ControlSamples/GridView/Selection`
  - `/ControlSamples/GridView/DisplayProperties`
  - `/ControlSamples/FormView/Events`
  - `/ControlSamples/FormView/Styles`
  - `/ControlSamples/DetailsView/Styles`
  - `/ControlSamples/DetailsView/Caption`

- **NavigationControl_Loads_WithoutErrors:**
  - `/ControlSamples/TreeView/Selection`
  - `/ControlSamples/TreeView/ExpandCollapse`

- **MenuControl_Loads_AndRendersContent:**
  - `/ControlSamples/Menu/Selection`

### Interaction Tests (InteractiveComponentTests.cs)

Added 9 new `[Fact]` tests:

| Test | What It Verifies |
|------|-----------------|
| `GridView_Selection_ClickSelect_HighlightsRow` | Click Select link → selected index updates, count increments |
| `GridView_DisplayProperties_RendersCaption` | Caption element, EmptyDataTemplate, ShowHeader/ShowFooter checkboxes |
| `TreeView_Selection_ClickNode_ShowsSelected` | Click node → selection text and count update |
| `TreeView_ExpandCollapse_ButtonsWork` | Expand All / Collapse All buttons, leaf node visibility, NodeIndent slider |
| `Menu_Selection_ClickItem_ShowsFeedback` | Click menu item → click count increments (no console error checks — JS interop) |
| `DetailsView_Styles_RendersStyledTable` | Table renders, "Customer Details" header visible |
| `DetailsView_Caption_RendersCaptionElement` | `<caption>` elements present, "Customer Record" text |
| `FormView_Events_ClickEdit_LogsEvent` | Click Edit → event log entries appear |
| `FormView_Styles_RendersStyledHeader` | "Widget Catalog" header text visible |

## Patterns Used

- Menu Selection test skips console error checks (JS interop produces expected errors)
- FormView tests use `WaitUntilState.DOMContentLoaded` (items bound in `OnAfterRenderAsync`)
- All other tests use `WaitUntilState.NetworkIdle` with 30s timeout
- Console error filtering: ISO 8601 timestamps + "Failed to load resource"

## Build Verification

`dotnet build samples/AfterBlazorServerSide.Tests/ -c Release` — succeeded with no errors.
