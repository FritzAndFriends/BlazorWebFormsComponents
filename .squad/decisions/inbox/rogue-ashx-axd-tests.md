# Decision: Middleware Integration Testing Pattern with TestServer

**By:** Rogue (QA Analyst)
**Date:** 2026-03-17
**Issue:** #423

## What

Established `Microsoft.AspNetCore.TestHost` + `TestServer` as the standard middleware testing pattern for this project. Added `Microsoft.AspNetCore.TestHost 10.0.5` package to the test csproj. Created `src/BlazorWebFormsComponents.Test/Middleware/AspxRewriteMiddlewareTests.cs` with 46 integration tests covering the full `UseBlazorWebFormsComponents` middleware pipeline (.aspx, .ashx, .axd handling).

## Pattern

Tests create a `TestServer` with `UseBlazorWebFormsComponents()` in the pipeline and a terminal `app.Run` returning 200 "PASSTHROUGH". Any request that reaches the terminal means no middleware intercepted it. Tests send HTTP requests via `TestServer.CreateClient()` and assert on status codes, headers, and body content.

For custom options, a `CreateServerAndClient` helper creates a fresh server+client pair with proper disposal.

## Why

- **Integration over unit tests:** Testing through the extension method validates the full registration + middleware chain, not individual classes in isolation. Catches wiring bugs.
- **TestServer is lightweight:** No ports, no networking, sub-millisecond per request. 46 tests run in under 1 second.
- **Aligned with ASP.NET Core conventions:** This is the recommended Microsoft pattern for middleware testing.

## Impact

Future middleware (e.g., .asmx handling, custom URL rewriting rules) should follow this same TestServer pattern in the `Middleware/` test folder.
