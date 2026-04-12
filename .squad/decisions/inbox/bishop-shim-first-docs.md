# Decision: Shim-First Migration Documentation Update

**Date:** 2026-04-13
**Author:** Bishop (Migration Tooling Dev)
**Status:** Implemented

## Context

The BWFC shim infrastructure (WebFormsPageBase, ResponseShim, SessionShim, RequestShim, CacheShim, ServerShim, ClientScriptShim, ViewStateDictionary, PostBack, FormShim, ConfigurationManager, BundleConfig, RouteConfig) has matured to the point where many Web Forms API calls compile and run AS-IS in Blazor. The migration-toolkit documentation still described a "convert everything manually" approach for these items.

## Decision

Updated all 5 migration-toolkit docs to reflect a "shim-first" migration paradigm:

1. **Pipeline percentages shifted**: L1 ~60% (was ~40%), L2 ~30% (was ~45%), L3 ~10% (was ~15%)
2. **Session["key"] moved out of Layer 3**: SessionShim handles basic usage; persistent/distributed session remains an architecture decision
3. **Response.Redirect, IsPostBack, Page.Title, Request.QueryString, Cache removed from Layer 2 manual work**: All handled AS-IS by shims
4. **New "Optional: Refactor to Native Blazor" section**: Developers can choose to keep shims long-term or refactor incrementally

## Impact

- Psylocke: copilot-instructions-template.md already aligned (updated separately)
- Cyclops/Rogue: Migration skills should reference shim-first approach when guiding developers
- All agents: When migrating code-behind, check if the API is shim-handled before writing conversion transforms
