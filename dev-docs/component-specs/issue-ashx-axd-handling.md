# Issue: Add ASHX and AXD URL rewriting to UseBlazorWebFormsComponents middleware

> **Status:** Draft — Issues are disabled on the upstream repo. File this when issues are re-enabled, or convert to a GitHub Discussion.

## Summary

The `UseBlazorWebFormsComponents()` middleware currently handles `.aspx` URL rewriting (301 redirects to clean Blazor routes). It should also handle legacy `.ashx` (HTTP handlers) and `.axd` (Web Resources) URL patterns that migrated applications may still receive requests for.

## Background

Web Forms applications commonly use:
- **`.ashx` files** — Generic HTTP handlers (`IHttpHandler`) for serving dynamic content (images, downloads, AJAX endpoints, etc.)
- **`.axd` endpoints** — Framework handlers like `WebResource.axd`, `ScriptResource.axd`, `Trace.axd`, and `ChartImg.axd`

After migration to Blazor, external links, bookmarks, cached URLs, and third-party integrations may still reference these legacy endpoints. The middleware should intercept these requests and handle them gracefully.

## Proposed Behavior

### `.ashx` Handling
- Configurable via a new `EnableAshxHandling` option on `BlazorWebFormsComponentsOptions` (default: `true`)
- **Default behavior:** Return 410 Gone with a descriptive message, since handler logic must be manually reimplemented as Minimal API endpoints or controllers
- **Optional:** Allow users to register custom redirect mappings (e.g., `/ImageHandler.ashx?id=5` → `/api/images/5`)

### `.axd` Handling
- Configurable via a new `EnableAxdHandling` option on `BlazorWebFormsComponentsOptions` (default: `true`)
- **`WebResource.axd` / `ScriptResource.axd`:** Return 404 — these are no longer needed since Blazor uses standard static file serving
- **`Trace.axd`:** Return 404 — replaced by ASP.NET Core diagnostics middleware
- **`ChartImg.axd`:** Return 410 Gone — charts need reimplementation

### Configuration Example

```csharp
builder.Services.AddBlazorWebFormsComponents(options =>
{
    options.EnableAspxUrlRewriting = true;   // existing (default: true)
    options.EnableAshxHandling = true;       // new (default: true)
    options.EnableAxdHandling = true;        // new (default: true)
});

app.UseBlazorWebFormsComponents();
```

## Acceptance Criteria

- [ ] `.ashx` requests return 410 Gone by default
- [ ] `.axd` requests return 404 Not Found by default
- [ ] Both can be disabled independently via options
- [ ] Custom `.ashx` redirect mappings can be registered
- [ ] Existing `.aspx` rewriting behavior is unchanged
- [ ] Unit tests cover all three URL patterns
