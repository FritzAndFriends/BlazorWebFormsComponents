# Decision: ResponseShim.Redirect null URL bug

**Author:** Rogue (QA)
**Date:** 2026-03-11
**Status:** BUG REPORT

## Issue

`ResponseShim.Redirect(string url)` throws `NullReferenceException` when `url` is null.

**Location:** `src/BlazorWebFormsComponents/ResponseShim.cs`, line 28

```csharp
if (url.StartsWith("~/")) url = url[1..]; // NullReferenceException if url is null
```

## Impact

In Web Forms, `Response.Redirect(null)` throws `ArgumentNullException` with a meaningful message. The BWFC shim throws a raw `NullReferenceException` instead, which is confusing during migration debugging.

## Recommendation

Add a null guard at the start of `Redirect()`:

```csharp
public void Redirect(string url, bool endResponse = true)
{
    ArgumentNullException.ThrowIfNull(url);
    // ... existing logic
}
```

## Test Coverage

Test `Redirect_NullUrl_ThrowsNullReferenceException` in `ResponseShimTests.razor` documents the current behavior. If fixed to throw `ArgumentNullException`, update the test accordingly.
