# Decision: PostBack Shim Runtime Strategy

**Date:** 2026-07-17  
**Author:** Cyclops  
**Status:** Implemented  

## Context

ClientScriptShim Phase 1 left `GetPostBackEventReference()`, `GetPostBackClientHyperlink()`, and `GetCallbackEventReference()` as `NotSupportedException` stubs. Phase 2 needed to make these return working JavaScript strings — zero rewrite, same API.

## Decision

1. **Inline JS bootstrap** — WebFormsPageBase.OnAfterRenderAsync injects `__doPostBack` and registration functions via `eval()` on firstRender. This avoids race conditions with external script loading while keeping `bwfc-postback.js` available as an optional static asset.

2. **ResolveControlId priority** — Checks `BaseWebFormsComponent.ID` first (the developer-assigned HTML ID), then falls back to `GetType().Name`. This matches Web Forms' ClientID behavior.

3. **ScriptManagerShim dual-path** — Supports both DI injection (`services.AddScoped<ScriptManagerShim>`) and static `GetCurrent(page)` factory. Migrated code using `ScriptManager.GetCurrent(Page)` compiles unchanged.

4. **PostBack target ID format** — `TypeName_HashCode` ensures per-instance uniqueness when multiple page components exist.

## Impact

- Tests updated: 3 tests changed from throw-verification to return-value-verification, plus 3 new null-handling tests
- No breaking changes to existing consumers
- WebFormsPageBase now implements IAsyncDisposable (new interface on the class)
