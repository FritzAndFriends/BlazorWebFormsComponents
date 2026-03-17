# Decision: L1 Script Bug Fixes + Test Coverage for #472

**Author:** Cyclops  
**Date:** 2026-03-17  
**Issue:** #472 — Improve L1 migration script automation  

## Context

The L1 migration script had 3 bugs causing test failures (7/10 pass rate). All five conversion patterns requested in #472 (bool/enum/unit normalization, Response.Redirect, Session detection, ViewState detection, DataSourceID warnings) were already implemented but lacked test coverage.

## Decisions

1. **Scoped Eval regex in GetRouteUrl** — The `Eval()` → `context.` conversion now only runs on lines containing `GetRouteUrl` to avoid corrupting data-binding expressions elsewhere. This is a safe narrowing since `Eval()` inside `GetRouteUrl` route values is the only legitimate use case for that conversion.

2. **Test harness extended for code-behind verification** — `Run-L1Tests.ps1` now copies `.aspx.cs` files alongside `.aspx` inputs and compares `.razor.cs` output when expected files exist. This enables testing code-behind transforms (Response.Redirect, Session, ViewState) without changing the core test flow.

3. **ContentWrapper regex uses horizontal whitespace only** — Changed `\s*\r?\n?` to `[ \t]*\r?\n?` after Content tag closing `>` to prevent eating indentation on the next line.

## Impact

Test suite: 7/10 (70%) → 15/15 (100%), line accuracy: 94.3% → 100%.
