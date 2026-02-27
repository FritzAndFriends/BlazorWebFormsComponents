# Decision: Issue #358 — 5 Missing Smoke Tests Gap Closed

**Decided by:** Colossus  
**Date:** 2026-02-27  
**Status:** Complete

## Context

M9 audit identified 5 sample pages without smoke test coverage. The smoke test `[InlineData]` entries were added in a prior session. This session adds 5 interaction tests for the pages with testable interactive/rendering behavior.

## What Was Done

- Verified all 5 smoke test entries already present in `ControlSampleTests.cs`
- Added 5 `[Fact]` interaction tests in `InteractiveComponentTests.cs`:
  - **ListView CrudOperations** (P0): 2 tests — Edit mode activation, Delete row removal
  - **Label**: 1 test — `AssociatedControlID` renders `<label for>` vs `<span>`
  - **DataGrid Styles**: 1 test — caption, header, data rows, GridLines variations
  - **LoginControls Orientation**: 1 test — all 4 layout variants render

## What Was Skipped

- **Panel/BackImageUrl**: No interaction test — page is static display only, smoke test provides sufficient coverage.

## Impact

- All 5 gap pages from M9 audit now have both smoke AND interaction test coverage (except Panel/BackImageUrl which only warrants a smoke test).
- ListView CrudOperations (P0) has the strongest coverage with 2 dedicated interaction tests.
