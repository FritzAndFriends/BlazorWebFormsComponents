# Decision: Pattern B+ — Graceful Cookie Degradation

**Date:** 2026-03-12
**Author:** Forge (approved by Jeff)
**Status:** APPROVED
**Scope:** WebFormsPageBase Request/Response shims

## Context

When HttpContext is null (InteractiveServer post-prerender), Response.Cookies and Request.Cookies
cannot delegate to the real ASP.NET Core implementations. Jeff explicitly rejected throwing
exceptions for cookie access, requesting graceful degradation instead.

## Decision

Adopt **Pattern B+** for cookie shims:

### Response.Cookies (when HttpContext is null)
- Return `NullResponseCookies` implementing `IResponseCookies`
- Append/Delete silently no-op
- Log `ILogger.LogWarning()` on first access per-request (not every call)

### Request.Cookies (when HttpContext is null)
- Return `EmptyRequestCookies` implementing `IRequestCookieCollection`
- Returns empty/null for all lookups, Count=0, empty enumerator
- Log `ILogger.LogWarning()` on first access per-request (not every call)

### What STILL throws
- `GetRouteUrl()` → `InvalidOperationException` (requires HttpContext for LinkGenerator)
- `Session` → `InvalidOperationException` (silent failure would cause data loss)

### What needs NO guard
- `Response.Redirect` → works via NavigationManager (no HttpContext needed)
- `ViewState` → in-memory dictionary (no HttpContext needed)

### What degrades gracefully (non-cookie)
- `Request.QueryString` → falls back to NavigationManager URI parsing
- `Request.Url` → falls back to NavigationManager URI

## Implementation Notes
- Both null implementations use a `bool _warned` flag to log only once per instance
- `ResponseShim` constructor gains `HttpContext?` and `ILogger` parameters
- New `RequestShim` class follows same pattern
- Guard method: `_httpContextAccessor.HttpContext is not null` (NOT RendererInfo.IsInteractive)

## Rationale
- Cookie operations in interactive mode are inherently impossible (no HTTP request/response)
- Throwing would break pages that conditionally use cookies but work fine without them
- Logging ensures developers can diagnose missing cookie behavior via standard logging
- GetRouteUrl/Session still throw because silent failure would produce incorrect URLs or data loss
