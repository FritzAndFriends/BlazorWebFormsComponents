# Decision: Blur + Retry Pattern for BWFC TextBox Playwright Tests

**Author:** Colossus  
**Date:** 2026-03-14  
**Status:** Implemented  

## Context

The `StudentsPage_AddNewStudentFormWorks` Playwright test was failing because BWFC TextBox uses `@onchange` (fires on blur) for `TextChanged`, not `@oninput`. Playwright's `FillAsync` triggers `input` events but the `change` event only fires when focus leaves the field. For the last field filled, no blur occurs until the button click, creating a race with Blazor Server's SignalR event queue.

## Decision

1. **Explicit blur after last field fill** — `await emailBox.BlurAsync()` + 200ms wait ensures the `change` event fires and Blazor processes it before the button click.
2. **Increased post-click wait** — 1000ms (up from 500ms) accounts for the full Blazor Server round-trip (SignalR → DB insert → re-query → re-render → DOM update).
3. **Retry loop with 3-second deadline** — Polls row count and page content every 300ms for up to 3 seconds, making the test resilient to variable CI timing without adding excessive fixed waits.

## Convention

Any future Playwright test that fills BWFC TextBox fields must blur the last field before clicking a submit button. This is a BWFC-specific requirement (Web Forms `onchange` semantics) that does not apply to standard HTML input tests.
