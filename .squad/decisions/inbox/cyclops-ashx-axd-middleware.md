# Decision: ASHX/AXD Middleware Design (#423)

**Author:** Cyclops  
**Date:** 2026-03-18  
**Status:** Implemented on `feature/docs-middleware-reports`

## Context

Issue #423 requires handling legacy `.ashx` and `.axd` URLs in the existing `UseBlazorWebFormsComponents()` middleware pipeline.

## Decision

- **Separate middleware classes** for each URL type (`.aspx`, `.ashx`, `.axd`) rather than one monolith. Each is focused and independently toggleable.
- **AshxHandlerMiddleware** takes `BlazorWebFormsComponentsOptions` via constructor injection to access `AshxRedirectMappings`. Returns 410 Gone by default; 301 redirect when a mapping exists.
- **AxdHandlerMiddleware** is stateless (no options needed). Uses path suffix matching: `ChartImg.axd` → 410 Gone; everything else → 404.
- **Registration order** in `UseBlazorWebFormsComponents()`: aspx → ashx → axd. Order doesn't matter since each targets a distinct file extension.
- **`AshxRedirectMappings`** uses `StringComparer.OrdinalIgnoreCase` for case-insensitive path matching, consistent with how ASP.NET handles URLs.

## Team Impact

- No breaking changes. All three features default to `true`.
- Sample projects continue to work with no config changes.
- Future work: if we need more granular .axd control, we can add an `AxdResponseMappings` dictionary similar to `AshxRedirectMappings`.
