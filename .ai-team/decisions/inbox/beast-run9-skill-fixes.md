# Beast — Run 9 Skill Documentation Fixes

**Date:** 2026-03-07
**By:** Beast (Technical Writer)
**Requested by:** Jeffrey T. Fritz (via Forge post-mortem)

## Decisions

### Interactive Server Mode requires minimal API endpoints for auth and session operations

All 4 migration skill files now document that `HttpContext` is NULL during WebSocket circuits when using `<Routes @rendermode="InteractiveServer" />`. Cookie auth (login/register/logout) and session-dependent operations (cart, preferences) MUST use `<form method="post">` submissions to minimal API endpoints — they cannot be done via Blazor event handlers.

**Applies to:** Any migration where the target app uses global Interactive Server render mode (which is our standard).

### DisableAntiforgery() is required on all Blazor → minimal API form POSTs

Blazor's HTML rendering does not include antiforgery tokens in `<form>` elements. All minimal API endpoints receiving form POSTs from Blazor pages must call `.DisableAntiforgery()` or the request fails with 400 Bad Request.

### Enhanced navigation must be bypassed for non-Blazor endpoints

Blazor's enhanced navigation intercepts `<a href>` clicks. Links to minimal API endpoints must use `<form method="post">` or `data-enhance-nav="false"` to ensure the request reaches the server.

### LoginView must NEVER be replaced with AuthorizeView (reinforced)

This was already a team decision but was reinforced with prominent callouts in both the identity and standards skill files after Run 8 showed it was still being attempted.
