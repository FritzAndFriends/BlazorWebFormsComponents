# Decision: SSR Opt-Out Pattern via ExcludeFromInteractiveRouting

**Author:** Cyclops  
**Date:** 2025-07-24  
**Status:** Implemented  

## Context

The sample app uses `@rendermode="InteractiveServer"` on `<Routes>` in `App.razor`, meaning all pages run interactively via SignalR. Pages that need real HTTP POST (like the Request.Form demo) can't function because there's no form data in interactive mode.

## Decision

Modified `App.razor` to use `HttpContext.AcceptsInteractiveRouting()` for conditional render mode assignment. Pages that need SSR add `@attribute [ExcludeFromInteractiveRouting]` — the standard Microsoft pattern for .NET 8+.

**Impact:** This is non-breaking. All existing pages continue as InteractiveServer. Only pages explicitly decorated with the attribute will render as SSR.

## Applies To

Any future sample page or migration demo that needs real HTTP request/response semantics (form POSTs, Request.Form, Request.Cookies during POST, etc.) should use this same attribute.
