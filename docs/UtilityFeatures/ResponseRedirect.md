# Response.Redirect Shim

The `ResponseShim` class provides compatibility with the ASP.NET Web Forms `Response.Redirect()` pattern. It wraps Blazor's `NavigationManager` so that migrated code-behind using `Response.Redirect("~/Products.aspx")` compiles and navigates correctly without rewriting.

Original Microsoft implementation: https://docs.microsoft.com/en-us/dotnet/api/system.web.httpresponse.redirect?view=netframework-4.8

## Background

In ASP.NET Web Forms, server-side redirects were performed through the `Response` object:

```csharp
// Web Forms code-behind
protected void btnSearch_Click(object sender, EventArgs e)
{
    Response.Redirect("~/Products.aspx");
    Response.Redirect("~/Products/Details.aspx?id=5", false);
    Response.Redirect("~/Admin/Dashboard.aspx", true);
}
```

The `Response` object was available on every `Page` and `UserControl` via the inherited `HttpResponse` property.

## Blazor Implementation

In Blazor, navigation is handled by `NavigationManager.NavigateTo()`. The `ResponseShim` bridges this gap by:

1. **Stripping `~/` prefix** — Converts `~/path` to `/path` (virtual path resolution)
2. **Stripping `.aspx` extension** — Converts `/path.aspx` to `/path` (Blazor uses extensionless routes)
3. **Ignoring `endResponse`** — The second parameter has no meaning in Blazor's component model

### Availability

`ResponseShim` is automatically available when your page inherits from `WebFormsPageBase`:

```razor
@inherits WebFormsPageBase

@code {
    private void HandleClick()
    {
        // Works exactly like Web Forms
        Response.Redirect("~/Products.aspx");
    }
}
```

## Web Forms Usage

```csharp
// Common Web Forms patterns
Response.Redirect("~/Products.aspx");
Response.Redirect("~/Products/Details.aspx?id=" + productId);
Response.Redirect("~/Login.aspx", true);   // endResponse = true
Response.Redirect("~/Dashboard.aspx", false); // endResponse = false
```

## Blazor Usage

```razor
@inherits WebFormsPageBase

<Button Text="Go to Products" OnClick="GoToProducts" />
<Button Text="View Details" OnClick="ViewDetails" />

@code {
    private int _selectedProductId = 5;

    private void GoToProducts()
    {
        // ~/Products.aspx → /Products
        Response.Redirect("~/Products.aspx");
    }

    private void ViewDetails()
    {
        // Query strings are preserved
        Response.Redirect($"~/Products/Details.aspx?id={_selectedProductId}");
    }
}
```

## URL Transformations

| Web Forms URL | Blazor URL | Notes |
|---|---|---|
| `~/Products.aspx` | `/Products` | `~/` stripped, `.aspx` removed |
| `~/Admin/Dashboard.aspx` | `/Admin/Dashboard` | Path structure preserved |
| `~/Products.aspx?id=5` | `/Products?id=5` | Query strings preserved |
| `/Products` | `/Products` | Already-clean URLs pass through |
| `~/` | `/` | Root redirect |

## Migration Path

The `ResponseShim` handles the most common redirect patterns. For each pattern:

| Web Forms Pattern | ResponseShim Support | Notes |
|---|---|---|
| `Response.Redirect(url)` | ✅ Full | Primary use case |
| `Response.Redirect(url, endResponse)` | ✅ Compiles | `endResponse` is ignored |
| `Response.RedirectPermanent(url)` | ❌ Not supported | Use `NavigationManager.NavigateTo()` directly |
| `Response.Write(html)` | ❌ Not supported | Not applicable in Blazor |
| `Response.StatusCode = 404` | ❌ Not supported | Use middleware or `NavigationManager` |

## Moving On

`Response.Redirect()` is a migration bridge. As you refactor:

1. **Replace with `NavigationManager`** — Inject `NavigationManager` and call `NavigateTo()` directly
2. **Remove `.aspx` from route references** — Update string constants to use clean Blazor routes
3. **Remove `~/` prefixes** — Use absolute paths (`/Products`) or relative navigation

```razor
@* Before (migration shim) *@
@inherits WebFormsPageBase
@code {
    void Navigate() => Response.Redirect("~/Products.aspx");
}

@* After (native Blazor) *@
@inject NavigationManager Nav
@code {
    void Navigate() => Nav.NavigateTo("/Products");
}
```

## See Also

- [WebFormsPage](WebFormsPage.md) — Page-level base class providing the `Response` property
- [Page System](PageService.md) — Title, MetaDescription, and other page-level services
- [L2 Automation Shims](L2AutomationShims.md) — Overview of all migration automation features
