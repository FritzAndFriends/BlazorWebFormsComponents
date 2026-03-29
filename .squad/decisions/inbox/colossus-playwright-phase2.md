# Decision: Phase 2 Playwright Test Strategy

**Author:** Colossus  
**Date:** 2026-07-24  
**Status:** Implemented  

## Context

Phase 2 added SessionShim (GAP-04), Page Lifecycle Transforms (GAP-05), and Event Handler Signatures (GAP-07). GAP-05 and GAP-07 are script transforms with no dedicated UI — they are covered by L1 unit tests. GAP-04 has a live sample page at `/migration/session`.

## Decision

- **Test GAP-04 (SessionShim) with 5 Playwright tests** covering set/get, count, clear, typed counter, and cross-navigation persistence.
- **Add 1 regression test for the Phase 1 ConfigurationManager page** to prevent regressions.
- **Skip browser tests for GAP-05 and GAP-07** since they are script-level transforms with no direct UI surface; L1 tests provide sufficient coverage.
- **Use `data-audit-control` attribute selectors** (already present on the sample page) for robust element targeting that won't break with CSS changes.
- **Use `DOMContentLoaded` wait strategy** (not `NetworkIdle`) for interactive Blazor Server pages per established patterns.

## Files Created

- `samples/AfterBlazorServerSide.Tests/Migration/SessionDemoTests.cs` (5 tests)
- `samples/AfterBlazorServerSide.Tests/Migration/ConfigurationManagerTests.cs` (1 test)
