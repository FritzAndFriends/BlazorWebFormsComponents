# Decision: CLI Transform Preserves All ClientScript/ScriptManager Patterns (Phase 2)

**Date:** 2026-07-31
**Author:** Bishop (Migration Tooling Dev)
**Status:** Implemented

## Context

ClientScriptTransform Phase 1 emitted TODO markers for `GetPostBackEventReference` and `ScriptManager.GetCurrent()` because no runtime shim existed. Phase 2 shims (ClientScriptShim + ScriptManagerShim) now handle these patterns.

## Decision

The CLI transform now preserves ALL six ClientScript/ScriptManager patterns instead of commenting out two of them. Zero TODO markers remain — shims handle everything at runtime.

- `GetPostBackEventReference` → prefix-stripped, preserved for ClientScriptShim
- `ScriptManager.GetCurrent(Page)` → converted to `ScriptManager.GetCurrent(this)` for ScriptManagerShim
- Shim dependency comment conditionally mentions ScriptManagerShim when ScriptManager patterns detected

## Impact

- All agents: migrated code no longer has `// TODO: Replace __doPostBack` or `// TODO: ScriptManager.GetCurrent() has no Blazor equivalent` — these calls just work via shims
- Layer 2 agents: less manual fixup needed for postback and ScriptManager patterns
- Test count: 349 → 353
