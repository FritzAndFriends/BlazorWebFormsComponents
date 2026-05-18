# Server & Path Resolution Shim

The `ServerShim` class provides compatibility with the ASP.NET Web Forms `Server` object (`HttpServerUtility`). It wraps ASP.NET Core's `IWebHostEnvironment`, `NavigationManager`, and `System.Net.WebUtility` so that migrated code-behind using `Server.MapPath()`, encoding helpers, `Server.Transfer()`, `Server.GetLastError()`, and `Server.ClearError()` compiles and works correctly. The `ResolveUrl()` and `ResolveClientUrl()` methods on `WebFormsPageBase` handle virtual path and `.aspx` extension stripping.

Original Microsoft implementation: https://docs.microsoft.com/en-us/dotnet/api/system.web.httpserverutility?view=netframework-4.8

## Background

In ASP.NET Web Forms, the `Server` object was available on every `Page` and `UserControl`:

```csharp
// Web Forms code-behind
string uploadDir = Server.MapPath("~/uploads");
string safe = Server.HtmlEncode(userInput);
string encoded = Server.UrlEncode(searchTerm);
string url = ResolveUrl("~/Products.aspx");
```

These utilities handled virtual-to-physical path mapping, HTML/URL encoding, and URL resolution — all common operations in Web Forms applications.

## Blazor Implementation

In Blazor, these concerns are split across several APIs. The `ServerShim` bridges the gap by:

1. **`MapPath("~/path")`** — Resolves `~/` to `IWebHostEnvironment.WebRootPath` (wwwroot) and other paths to `ContentRootPath`
2. **`HtmlEncode()` / `HtmlDecode()`** — Delegates to `System.Net.WebUtility`
3. **`UrlEncode()` / `UrlDecode()`** — Delegates to `System.Net.WebUtility`
4. **`Transfer(path)`** — Delegates to `NavigationManager.NavigateTo(path)`
5. **`GetLastError()` / `ClearError()`** — Compatibility stubs for Web Forms error-handling patterns

The `ResolveUrl()` and `ResolveClientUrl()` methods live on `WebFormsPageBase` and:

1. **Strip `~/` prefix** — Converts `~/path` to `/path`
2. **Strip `.aspx` extension** — Converts `/path.aspx` to `/path`

### Availability

`ServerShim` is automatically available when your page inherits from `WebFormsPageBase`. The `IWebHostEnvironment` dependency is injected automatically:

```razor
@inherits WebFormsPageBase

@code {
    private void Example()
    {
        // All work exactly like Web Forms
        string path = Server.MapPath("~/uploads");
        string safe = Server.HtmlEncode("<script>alert('xss')</script>");
        string url = ResolveUrl("~/Products.aspx");
    }
}
```

## Web Forms Usage

```csharp
// Path resolution
string uploadDir = Server.MapPath("~/uploads");
string configPath = Server.MapPath("/App_Data/config.xml");

// HTML encoding
string safe = Server.HtmlEncode(userInput);
string decoded = Server.HtmlDecode(encodedHtml);

// URL encoding
string param = Server.UrlEncode(searchTerm);
string original = Server.UrlDecode(encodedParam);

// URL resolution
string productUrl = ResolveUrl("~/Products.aspx");
string imageUrl = ResolveClientUrl("~/images/logo.png");
```

## Blazor Usage

```razor
@inherits WebFormsPageBase

<img src="@ResolveUrl("~/images/logo.png")" alt="Logo" />
<a href="@ResolveUrl("~/Products.aspx")">Products</a>

@code {
    private string _uploadDir = "";
    private string _safeHtml = "";

    protected override void OnInitialized()
    {
        base.OnInitialized();

        // ~/uploads → {WebRootPath}/uploads
        _uploadDir = Server.MapPath("~/uploads");

        // HTML encoding for safe output
        _safeHtml = Server.HtmlEncode("<script>alert('xss')</script>");
    }

    private void BuildSearchUrl()
    {
        string term = Server.UrlEncode("blazor web forms");
        // ~/Products.aspx → /Products
        string url = ResolveUrl($"~/Search.aspx?q={term}");
        Response.Redirect(url);
    }
}
```

## Path Resolution

| Input | `MapPath` Result | Notes |
|---|---|---|
| `"~/uploads"` | `{WebRootPath}/uploads` | `~/` maps to wwwroot |
| `"~/images/logo.png"` | `{WebRootPath}/images/logo.png` | File within wwwroot |
| `"/App_Data/config.xml"` | `{ContentRootPath}/App_Data/config.xml` | Non-tilde paths use content root |
| `""` or `null` | `{ContentRootPath}` | Empty returns content root |

## URL Transformations

| Input | `ResolveUrl` Result | Notes |
|---|---|---|
| `"~/Products.aspx"` | `"/Products"` | `~/` stripped, `.aspx` removed |
| `"~/images/logo.png"` | `"/images/logo.png"` | Non-aspx extensions preserved |
| `"/Products"` | `"/Products"` | Already-clean URLs pass through |

## Migration Path

| Web Forms | BWFC Shim | Native Blazor |
|---|---|---|
| `Server.MapPath("~/img")` | `Server.MapPath("~/img")` | Inject `IWebHostEnvironment` |
| `Server.HtmlEncode(text)` | `Server.HtmlEncode(text)` | `WebUtility.HtmlEncode(text)` |
| `Server.HtmlDecode(text)` | `Server.HtmlDecode(text)` | `WebUtility.HtmlDecode(text)` |
| `Server.UrlEncode(text)` | `Server.UrlEncode(text)` | `WebUtility.UrlEncode(text)` |
| `Server.UrlDecode(text)` | `Server.UrlDecode(text)` | `WebUtility.UrlDecode(text)` |
| `ResolveUrl("~/page.aspx")` | `ResolveUrl("~/page.aspx")` | Use `NavigationManager.ToAbsoluteUri()` |
| `ResolveClientUrl("~/page.aspx")` | `ResolveClientUrl("~/page.aspx")` | Use `NavigationManager.ToAbsoluteUri()` |

## Moving On

`ServerShim` is a migration bridge. As you refactor:

1. **Replace `MapPath`** — Inject `IWebHostEnvironment` directly and use `env.WebRootPath` or `env.ContentRootPath`
2. **Replace encoding helpers** — Use `System.Net.WebUtility.HtmlEncode()` and `UrlEncode()` directly
3. **Replace `ResolveUrl`** — Use `NavigationManager.ToAbsoluteUri()` and remove `.aspx` references from your routes

```razor
@* Before (migration shim) *@
@inherits WebFormsPageBase
@code {
    string path = Server.MapPath("~/uploads");
    string url = ResolveUrl("~/Products.aspx");
}

@* After (native Blazor) *@
@inject IWebHostEnvironment Env
@inject NavigationManager Nav
@code {
    string path = Path.Combine(Env.WebRootPath, "uploads");
    string url = Nav.ToAbsoluteUri("/Products").ToString();
}
```

## See Also

- [WebFormsPage](WebFormsPage.md) — Page-level base class providing the `Server` property
- [Response.Redirect](ResponseRedirect.md) — Companion shim for navigation
- [L2 Automation Shims](L2AutomationShims.md) — Overview of all migration automation features
