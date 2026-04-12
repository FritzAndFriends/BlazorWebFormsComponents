# Decision: Comprehensive Shim Documentation in Migration Skills

**Proposed by:** Psylocke (Skills Engineer)
**Date:** 2025-07-30
**Domain:** migration, documentation

## Context

All migration skills (bwfc-migration, migration-standards, bwfc-data-migration, copilot-instructions) referenced outdated or incomplete information about the shim system. Several shims existed in code but were not documented in any skill. The CONTROL-COVERAGE.md file contained a contradiction about SelectMethod handling.

## Decision

Updated all migration-related skills to accurately document the full shim API surface:

1. **10 shims documented**: FormShim, ClientScriptShim, ScriptManagerShim, RequestShim, ResponseShim, SessionShim, CacheShim, ServerShim, ViewStateDictionary, WebFormsPageBase (as the hub exposing all shims).
2. **SelectMethod contradiction resolved**: Changed `SelectMethod → Items` to `SelectMethod → delegate or Items` with preference for delegate conversion.
3. **IsPostBack corrected**: Changed from "always false" to accurate SSR/Interactive dual-mode behavior.
4. **PostBack support documented**: Despite earlier team decision against __doPostBack emulation, the implementation exists and is now documented.
5. **AddSessionShim() removed**: All references to non-existent registration methods replaced with correct `AddBlazorWebFormsComponents()`.

## Impact

- Migration agents will now recommend correct shim usage instead of manual rewrites
- Fewer false positives in migration reviews (e.g., flagging working `Response.Redirect` calls)
- SelectMethod preservation will be correctly guided (delegate preferred over Items binding)

## Status

**Implemented** — changes applied to 8 files across migration-toolkit/ and .github/.
