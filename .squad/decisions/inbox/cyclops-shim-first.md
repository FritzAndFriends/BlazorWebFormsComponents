# Decision: CLI Transforms Adopt Shim-First Strategy

**Date:** 2026-07-30
**Author:** Cyclops
**Status:** Implemented

## Context

WebFormsPageBase already provides protected shim properties (Request, Response, Server, Session, Cache, ClientScript) that migrated pages inherit. CLI transforms were redundantly injecting `[Inject]` properties or rewriting calls that already compile against these shims.

## Decision

Refactored 5 CLI transforms to be shim-first:

1. **ResponseRedirectTransform** — No longer rewrites `Response.Redirect()` → `NavigationManager.NavigateTo()`. Calls compile against ResponseShim which handles ~/prefix and .aspx stripping. Only strips `Page.`/`this.` prefix. No `[Inject] NavigationManager`.
2. **SessionDetectTransform** — Removed `[Inject] SessionShim Session` and `[Inject] CacheShim Cache` injection. WebFormsPageBase already provides these.
3. **ClientScriptTransform** — Guidance now says "works via WebFormsPageBase — no injection needed" instead of suggesting `@inject ClientScriptShim`.
4. **RequestFormTransform** — Guidance leads with "Request.Form calls work automatically via RequestShim on WebFormsPageBase."
5. **ServerShimTransform** — Guidance leads with "Server.* calls work automatically via ServerShim on WebFormsPageBase."

## Rationale

- Eliminates redundant `[Inject]` properties that shadow base class members
- `Response.Redirect()` calls now preserved as-is — ResponseShim handles URL normalization
- Guidance comments accurately reflect the runtime architecture
- Non-page classes still directed to inject shims via DI

## Build Verification

0 errors across net8.0/net9.0/net10.0.
