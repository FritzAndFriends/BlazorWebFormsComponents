# Decision: FormShim wraps IFormCollection with null-safe degradation

**Author:** Cyclops  
**Date:** 2026-07-31  
**Status:** Implemented  

## Context

Migrated Web Forms code-behind frequently uses `Request.Form["fieldName"]` to read POST data. The existing `RequestShim` had `Cookies`, `QueryString`, and `Url` but no `Form`.

## Decision

Created `FormShim` as a public class (not an interface implementation) that wraps `IFormCollection?` and provides `NameValueCollection`-like API. The `RequestShim.Form` property follows the same graceful degradation pattern as `Cookies`:

1. **SSR mode (HttpContext available):** wraps `HttpContext.Request.Form` via `FormShim`
2. **Non-form-encoded requests:** catches `InvalidOperationException` → returns empty `FormShim(null)`
3. **Interactive mode (no HttpContext):** logs warning once → returns empty `FormShim(null)`

## Why not implement IFormCollection?

`FormShim` targets `NameValueCollection` semantics (Web Forms API), not `IFormCollection` (ASP.NET Core API). The indexer, `GetValues()`, `AllKeys` surface matches what migrated code actually calls. Implementing `IFormCollection` would add unnecessary API surface and file upload concerns.

## Impact

- Unblocks any migrated code-behind that reads `Request.Form["key"]`
- No breaking changes — new property on existing class
