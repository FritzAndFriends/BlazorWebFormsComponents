# Migrating .ashx Handlers

## Overview

ASP.NET Web Forms used `.ashx` files (HTTP Handler files) to handle HTTP requests directly, without the overhead of page lifecycle machinery. Handlers were ideal for serving file downloads, generating images, providing JSON APIs, and other request-response patterns that didn't need full page functionality.

In Blazor, there's no direct equivalent to `.ashx` handlers — but ASP.NET Core provides middleware, which is the modern way to intercept and handle HTTP requests. The BlazorWebFormsComponents library provides `HttpHandlerBase`, a base class that lets you port your `.ashx` logic to ASP.NET Core **without rewriting your handler code**, making migration straightforward and mechanical.

### Why Migrate?

- `.ashx` files don't exist in .NET Core — your handlers will break when you move your application
- ASP.NET Core's middleware is the modern, recommended approach
- `HttpHandlerBase` provides a shim that preserves the familiar Web Forms `HttpContext` API, so you can migrate with **~6 mechanical changes** per handler
- Session state, file I/O, encoding — all supported

---

## Quick Start: 6-Step Migration Checklist

Here are the mechanical changes required to migrate any `.ashx` handler to Blazor:

| Step | Web Forms | Blazor |
|------|-----------|--------|
| 1. | `using System.Web;` | `using BlazorWebFormsComponents;` |
| 2. | `: IHttpHandler` | `: HttpHandlerBase` |
| 3. | `ProcessRequest(HttpContext context)` | `ProcessRequestAsync(HttpHandlerContext context)` (async) |
| 4. | `context.Response.End();` | `return;` (End is now [Obsolete]) |
| 5. | Add `[HandlerRoute("/path.ashx")]` | Attribute declares handler path |
| 6. | Delete `.ashx` markup file | No longer needed; registration is in Program.cs |

**Before registering handlers in `Program.cs`, verify:**
- Session state handlers → mark with `[RequiresSessionState]`
- Complex paths → use explicit route attribute

---

## Registration in Program.cs

`HttpHandlerBase` handlers are registered in your Blazor `Program.cs` using `MapHandler<T>()`, which integrates with the standard ASP.NET Core routing system.

### Explicit Path Registration

Register a handler at a specific path using the `[HandlerRoute]` attribute:

```csharp
// MyApp/FileDownloadHandler.cs
[HandlerRoute("/Handlers/FileDownload.ashx")]
public class FileDownloadHandler : HttpHandlerBase
{
    public override async Task ProcessRequestAsync(HttpHandlerContext context)
    {
        // ... handler logic ...
    }
}

// Program.cs
var builder = WebApplication.CreateBuilder(args);
// ... other registrations ...

var app = builder.Build();
app.MapBlazorWebFormsHandlers();  // Auto-discovers [HandlerRoute] attributes
app.Run();
```

### Convention-Based Routing

If you omit the attribute, the handler is registered at a derived path:

```csharp
// MyApp/ProductApiHandler.cs (no [HandlerRoute] attribute)
public class ProductApiHandler : HttpHandlerBase
{
    public override async Task ProcessRequestAsync(HttpHandlerContext context)
    {
        // ... handler logic ...
    }
}

// Convention: /ProductApi.ashx (class name minus "Handler" suffix + .ashx)
// Accessible at: http://yourapp.com/ProductApi.ashx
```

!!!note
    Convention-based routing derives the path from the class name. `FileDownloadHandler` → `/FileDownload.ashx`. Use `[HandlerRoute]` to override this behavior.

### Multiple Paths for a Single Handler

Register one handler at multiple paths using `MapHandler` with varargs:

```csharp
// Program.cs
var app = builder.Build();

app.MapHandler<FileDownloadHandler>("/Handlers/FileDownload.ashx", "/download.ashx", "/files/get");

app.Run();
```

### Chaining Authorization and CORS

Handlers support middleware chain configuration:

```csharp
// Program.cs
app.MapHandler<ApiHandler>("/api/Data.ashx")
   .RequireAuthorization()
   .WithOpenApi();

app.MapHandler<FileHandler>("/Secure/Download.ashx")
   .RequireAuthorization("AdminOnly")
   .RequireCors("AllowFrontend");
```

---

## Before/After Examples

### Example 1: JSON API Handler (GET Request)

**Web Forms (.ashx):**

```csharp
using System.Web;
using System.Collections.Generic;
using System.Linq;

namespace MyApp
{
    public class ProductApiHandler : IHttpHandler
    {
        public bool IsReusable => true;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            var action = context.Request.QueryString["action"];
            
            if (action == "list")
            {
                var products = GetProducts();
                var json = JsonConvert.SerializeObject(products);
                context.Response.Write(json);
            }
            else if (action == "count")
            {
                context.Response.Write("{\"count\":" + GetProducts().Count + "}");
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Write("{\"error\":\"Unknown action\"}");
            }
        }

        private List<Product> GetProducts()
        {
            return new List<Product>
            {
                new Product { Id = 1, Name = "Widget", Price = 9.99m },
                new Product { Id = 2, Name = "Gadget", Price = 19.99m }
            };
        }
    }
}
```

**Blazor (Migrated):**

```csharp
using BlazorWebFormsComponents;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MyApp
{
    [HandlerRoute("/api/Products.ashx")]
    public class ProductApiHandler : HttpHandlerBase
    {
        public override async Task ProcessRequestAsync(HttpHandlerContext context)
        {
            context.Response.ContentType = "application/json";

            var action = context.Request.QueryString["action"];
            
            if (action == "list")
            {
                var products = GetProducts();
                var json = JsonSerializer.Serialize(products);
                context.Response.Write(json);
            }
            else if (action == "count")
            {
                context.Response.Write("{\"count\":" + GetProducts().Count + "}");
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Write("{\"error\":\"Unknown action\"}");
            }
        }

        private List<Product> GetProducts()
        {
            return new List<Product>
            {
                new Product { Id = 1, Name = "Widget", Price = 9.99m },
                new Product { Id = 2, Name = "Gadget", Price = 19.99m }
            };
        }
    }
}
```

**Changes Made:**
1. `using System.Web;` → `using BlazorWebFormsComponents;`
2. `: IHttpHandler` → `: HttpHandlerBase`
3. `ProcessRequest(HttpContext context)` → `async Task ProcessRequestAsync(HttpHandlerContext context)`
4. Added `[HandlerRoute("/api/Products.ashx")]`
5. `JsonConvert` → `System.Text.Json.JsonSerializer` (separate migration)

---

### Example 2: File Download Handler (Binary Response)

**Web Forms (.ashx):**

```csharp
using System;
using System.IO;
using System.Web;

namespace MyApp
{
    public class FileDownloadHandler : IHttpHandler
    {
        public bool IsReusable => true;

        public void ProcessRequest(HttpContext context)
        {
            var fileId = context.Request.QueryString["id"];
            
            if (string.IsNullOrEmpty(fileId) || !int.TryParse(fileId, out var id))
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "text/plain";
                context.Response.Write("Invalid file ID");
                return;
            }

            // Look up file in database or file system
            var filePath = context.Server.MapPath("~/App_Data/Files/" + fileId + ".pdf");
            
            if (!File.Exists(filePath))
            {
                context.Response.StatusCode = 404;
                context.Response.Write("File not found");
                return;
            }

            var fileBytes = File.ReadAllBytes(filePath);
            var fileName = Path.GetFileName(filePath);

            context.Response.Clear();
            context.Response.ContentType = "application/pdf";
            context.Response.AddHeader("Content-Disposition", 
                $"attachment; filename=\"{fileName}\"");
            context.Response.BinaryWrite(fileBytes);
            context.Response.End();
        }
    }
}
```

**Blazor (Migrated):**

```csharp
using BlazorWebFormsComponents;
using System;
using System.IO;

namespace MyApp
{
    [HandlerRoute("/Handlers/FileDownload.ashx")]
    public class FileDownloadHandler : HttpHandlerBase
    {
        public override async Task ProcessRequestAsync(HttpHandlerContext context)
        {
            var fileId = context.Request.QueryString["id"];
            
            if (string.IsNullOrEmpty(fileId) || !int.TryParse(fileId, out var id))
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "text/plain";
                context.Response.Write("Invalid file ID");
                return;
            }

            // Look up file in database or file system
            var filePath = context.Server.MapPath("~/App_Data/Files/" + fileId + ".pdf");
            
            if (!File.Exists(filePath))
            {
                context.Response.StatusCode = 404;
                context.Response.Write("File not found");
                return;
            }

            var fileBytes = File.ReadAllBytes(filePath);
            var fileName = Path.GetFileName(filePath);

            context.Response.Clear();
            context.Response.ContentType = "application/pdf";
            context.Response.AddHeader("Content-Disposition", 
                $"attachment; filename=\"{fileName}\"");
            context.Response.BinaryWrite(fileBytes);
            // return; — Response.End() no longer needed
        }
    }
}
```

**Changes Made:**
1. `using System.Web;` → `using BlazorWebFormsComponents;`
2. `: IHttpHandler` → `: HttpHandlerBase`
3. `ProcessRequest(HttpContext context)` → `async Task ProcessRequestAsync(HttpHandlerContext context)`
4. Removed `context.Response.End();` — `return` statement is sufficient
5. Added `[HandlerRoute("/Handlers/FileDownload.ashx")]`

---

### Example 3: Image Generation Handler (Thumbnail/Chart)

**Web Forms (.ashx):**

```csharp
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

namespace MyApp
{
    public class ThumbnailHandler : IHttpHandler
    {
        public bool IsReusable => true;

        public void ProcessRequest(HttpContext context)
        {
            var imagePath = context.Request.QueryString["path"];
            var width = int.Parse(context.Request.QueryString["w"] ?? "150");
            var height = int.Parse(context.Request.QueryString["h"] ?? "150");

            if (string.IsNullOrEmpty(imagePath))
            {
                context.Response.StatusCode = 400;
                context.Response.Write("Missing image path");
                return;
            }

            var fullPath = context.Server.MapPath("~/Images/" + imagePath);
            if (!File.Exists(fullPath))
            {
                context.Response.StatusCode = 404;
                return;
            }

            try
            {
                using (var image = new Bitmap(fullPath))
                {
                    using (var thumbnail = image.GetThumbnailImage(width, height, 
                        () => false, IntPtr.Zero))
                    {
                        context.Response.ContentType = "image/jpeg";
                        using (var ms = new MemoryStream())
                        {
                            thumbnail.Save(ms, ImageFormat.Jpeg);
                            context.Response.BinaryWrite(ms.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/plain";
                context.Response.Write("Error generating thumbnail: " + ex.Message);
            }
        }
    }
}
```

**Blazor (Migrated):**

```csharp
using BlazorWebFormsComponents;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MyApp
{
    [HandlerRoute("/Handlers/Thumbnail.ashx")]
    public class ThumbnailHandler : HttpHandlerBase
    {
        public override async Task ProcessRequestAsync(HttpHandlerContext context)
        {
            var imagePath = context.Request.QueryString["path"];
            var width = int.Parse(context.Request.QueryString["w"] ?? "150");
            var height = int.Parse(context.Request.QueryString["h"] ?? "150");

            if (string.IsNullOrEmpty(imagePath))
            {
                context.Response.StatusCode = 400;
                context.Response.Write("Missing image path");
                return;
            }

            var fullPath = context.Server.MapPath("~/Images/" + imagePath);
            if (!File.Exists(fullPath))
            {
                context.Response.StatusCode = 404;
                return;
            }

            try
            {
                using (var image = new Bitmap(fullPath))
                {
                    using (var thumbnail = image.GetThumbnailImage(width, height, 
                        () => false, IntPtr.Zero))
                    {
                        context.Response.ContentType = "image/jpeg";
                        using (var ms = new MemoryStream())
                        {
                            thumbnail.Save(ms, ImageFormat.Jpeg);
                            context.Response.BinaryWrite(ms.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/plain";
                context.Response.Write("Error generating thumbnail: " + ex.Message);
            }
        }
    }
}
```

**Changes Made:**
1. `using System.Web;` → `using BlazorWebFormsComponents;`
2. `: IHttpHandler` → `: HttpHandlerBase`
3. `ProcessRequest(HttpContext context)` → `async Task ProcessRequestAsync(HttpHandlerContext context)`
4. Added `[HandlerRoute("/Handlers/Thumbnail.ashx")]`

!!!note
    Image generation logic transfers directly. No rewrite needed for `System.Drawing` code — the API is identical between Web Forms and Core.

---

## API Reference

### HttpHandlerContext

The context object passed to `ProcessRequestAsync`. It wraps the ASP.NET Core `HttpContext` and provides familiar Web Forms properties.

```csharp
public class HttpHandlerContext
{
    public HttpHandlerRequest Request { get; }      // Request data
    public HttpHandlerResponse Response { get; }    // Response output
    public HttpHandlerServer Server { get; }        // Server utilities
    public ISession? Session { get; }               // Session state (if [RequiresSessionState])
    public ClaimsPrincipal User { get; }            // Current user principal
    public IDictionary<object, object> Items { get; }  // Request-scoped items dictionary
}
```

---

### HttpHandlerRequest

Encapsulates HTTP request data: query strings, form data, headers, files.

```csharp
public class HttpHandlerRequest
{
    // Query strings: context.Request.QueryString["id"]
    public string? this[string key] { get; }
    public NameValueCollection QueryString { get; }

    // Form data (POST): context.Request.Form["field"]
    public NameValueCollection Form { get; }

    // File uploads: context.Request.Files["upload"]
    public IFormFileCollection Files { get; }

    // HTTP method: "GET", "POST", etc.
    public string HttpMethod { get; }

    // Request headers
    public IHeaderDictionary Headers { get; }

    // Request content type
    public string? ContentType { get; }

    // Raw request stream
    public Stream InputStream { get; }

    // Request URL information
    public string Path { get; }
    public string RawUrl { get; }
    public string Url { get; }

    // Is the request authenticated?
    public bool IsAuthenticated { get; }

    // Client IP address
    public string RemoteAddr { get; }
}
```

**Common Usage:**

```csharp
var userId = context.Request.QueryString["id"];    // GET parameter
var userName = context.Request.Form["username"];   // POST field
var file = context.Request.Files["upload"];        // Uploaded file
if (context.Request.IsAuthenticated)
{
    // User is logged in
}
```

---

### HttpHandlerResponse

Controls HTTP response output: content type, status code, headers, body content.

```csharp
public class HttpHandlerResponse
{
    // Content type (e.g., "application/json", "image/png")
    public string ContentType { get; set; }

    // HTTP status code (200, 404, 500, etc.)
    public int StatusCode { get; set; }

    // Response stream for binary writes
    public Stream OutputStream { get; }

    // Write text to response (sync)
    public void Write(string text);

    // Write text to response (async) — preferred
    public Task WriteAsync(string text);

    // Write binary data to response (sync)
    public void BinaryWrite(byte[] data);

    // Write binary data to response (async) — preferred
    public Task BinaryWriteAsync(byte[] data);

    // Add a response header
    public void AddHeader(string name, string value);

    // Clear response content and headers
    public void Clear();

    // [Obsolete] In Web Forms, End() threw ThreadAbortException.
    // In Core, it sets a flag. Use 'return' instead.
    [Obsolete("Use 'return' to exit handler.")]
    public void End();

    // Check if Response.End() was called
    public bool IsEnded { get; }
}
```

**Common Usage:**

```csharp
// Set response type and status
context.Response.ContentType = "application/json";
context.Response.StatusCode = 200;

// Write content
context.Response.Write("{\"message\":\"success\"}");

// Add header for download
context.Response.AddHeader("Content-Disposition", 
    "attachment; filename=\"report.pdf\"");

// For file downloads, clear and write binary
context.Response.Clear();
context.Response.ContentType = "application/pdf";
context.Response.BinaryWrite(fileBytes);
```

---

### HttpHandlerServer

Server-side utilities: path mapping, encoding/decoding.

```csharp
public class HttpHandlerServer
{
    // Map virtual path to physical file path
    // ~/path → webroot, /path → content root
    public string MapPath(string virtualPath);

    // HTML-encode a string (prevent XSS)
    public string HtmlEncode(string text);

    // HTML-decode a string
    public string HtmlDecode(string text);

    // URL-encode a string
    public string UrlEncode(string text);

    // URL-decode a string
    public string UrlDecode(string text);
}
```

**Common Usage:**

```csharp
// Get physical file path from virtual path
var file = context.Server.MapPath("~/App_Data/users.xml");

// Safe HTML output (prevent XSS)
var safe = context.Server.HtmlEncode("<script>alert(1)</script>");
// Result: "&lt;script&gt;alert(1)&lt;/script&gt;"

// URL encoding for query strings
var encoded = context.Server.UrlEncode("hello world");
// Result: "hello+world"
```

---

## Session State

If your handler uses session state (e.g., storing user preferences, shopping cart), mark it with the `[RequiresSessionState]` attribute. The framework will automatically load session before invoking your handler.

### Enabling Session in Program.cs

First, configure session in your `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Enable session middleware
app.UseSession();

// Register handlers (after UseSession)
app.MapBlazorWebFormsHandlers();

app.Run();
```

### Using Session in a Handler

Mark the handler with `[RequiresSessionState]`:

```csharp
[HandlerRoute("/Handlers/ShoppingCart.ashx")]
[RequiresSessionState]
public class ShoppingCartHandler : HttpHandlerBase
{
    public override async Task ProcessRequestAsync(HttpHandlerContext context)
    {
        var action = context.Request.QueryString["action"];

        if (action == "add")
        {
            var productId = context.Request.Form["productId"];
            var quantity = int.Parse(context.Request.Form["quantity"] ?? "1");

            // Get cart from session (returns null if not set)
            var cart = context.Session?.GetObject<List<CartItem>>("Cart") 
                ?? new List<CartItem>();

            cart.Add(new CartItem { ProductId = productId, Quantity = quantity });

            // Save cart back to session
            context.Session?.SetObject("Cart", cart);

            context.Response.ContentType = "application/json";
            context.Response.Write("{\"status\":\"added\"}");
        }
        else if (action == "view")
        {
            var cart = context.Session?.GetObject<List<CartItem>>("Cart") 
                ?? new List<CartItem>();

            context.Response.ContentType = "application/json";
            var json = JsonSerializer.Serialize(new { items = cart, count = cart.Count });
            context.Response.Write(json);
        }
    }
}

public class CartItem
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
}
```

!!!warning
    Session is cookie-based in ASP.NET Core, not in-process. If you relied on Web Forms session state server or SQL session store, you must reconfigure that separately. For now, session is volatile and lost on app restart.

!!!note
    `GetObject<T>()` and `SetObject<T>()` are extension methods on `ISession` provided by BWFC. They use `System.Text.Json` to serialize/deserialize objects.

---

## What's Not Supported

Some Web Forms patterns cannot be shimmed in ASP.NET Core. Here's a clear list and recommended workarounds:

### `Response.End()` — ThreadAbort Pattern

**Web Forms:**
```csharp
context.Response.Write("Data");
context.Response.End();  // Threw ThreadAbortException, halted execution
```

**Blazor:**
```csharp
context.Response.Write("Data");
return;  // Use return instead
```

!!!warning
    In Web Forms, `Response.End()` threw an exception to stop the handler mid-execution. ASP.NET Core has no equivalent. The `HttpHandlerResponse.End()` method is marked `[Obsolete]` and only sets a flag. **You must change `Response.End()` to `return`.**

---

### `Server.Transfer()` — Server-Side Redirect

**Not Supported.** `Server.Transfer()` re-executes another handler in the same request without a round-trip. ASP.NET Core has no equivalent.

**Workaround:** Use `Response.Redirect()` or restructure as a service method:

```csharp
// Web Forms (not supported)
// context.Server.Transfer("/Handlers/Other.ashx?id=123");

// Blazor alternative 1: Redirect (browser round-trip)
context.Response.Redirect("/Handlers/Other.ashx?id=123");

// Blazor alternative 2: Refactor as shared service
var handler = new OtherHandler();
await handler.ProcessRequestAsync(context);
```

---

### `Application["key"]` — Global State

**Not Supported.** `Application` was a global dictionary shared across requests. ASP.NET Core discourages this pattern.

**Workaround:** Use dependency injection or `IMemoryCache`:

```csharp
// Web Forms (not supported)
// var count = (int)context.Application["RequestCount"];

// Blazor: Use DI + singleton service
[HandlerRoute("/api/Stats.ashx")]
public class StatsHandler : HttpHandlerBase
{
    private readonly AppStatisticsService _stats;

    public StatsHandler(AppStatisticsService stats)
    {
        _stats = stats;
    }

    public override async Task ProcessRequestAsync(HttpHandlerContext context)
    {
        var count = _stats.GetRequestCount();
        context.Response.Write(count.ToString());
    }
}

// Register in Program.cs
builder.Services.AddSingleton<AppStatisticsService>();
```

---

### `context.Cache` — Web Forms Caching

**Not Supported.** Web Forms `System.Web.Caching.Cache` doesn't exist in Core.

**Workaround:** Use `IMemoryCache`:

```csharp
// Web Forms (not supported)
// var cached = context.Cache["key"];

// Blazor: Inject IMemoryCache
[HandlerRoute("/api/Data.ashx")]
public class CachedDataHandler : HttpHandlerBase
{
    private readonly IMemoryCache _cache;

    public CachedDataHandler(IMemoryCache cache)
    {
        _cache = cache;
    }

    public override async Task ProcessRequestAsync(HttpHandlerContext context)
    {
        if (!_cache.TryGetValue("data", out List<Data> data))
        {
            data = await LoadDataFromDatabase();
            _cache.Set("data", data, TimeSpan.FromMinutes(5));
        }

        context.Response.ContentType = "application/json";
        context.Response.Write(JsonSerializer.Serialize(data));
    }
}

// Register in Program.cs
builder.Services.AddMemoryCache();
```

---

### `Server.Execute()` — Execute Without Transfer

**Not Supported.** Similar to `Server.Transfer()` — there's no way to execute another handler and return to the caller.

**Workaround:** Refactor as a service or async method.

---

### Complex `Request.Files` Scenarios

**Partially Supported.** The `IFormFile` API in Core differs from Web Forms `HttpPostedFile`.

**Web Forms:**
```csharp
var file = context.Request.Files["upload"];
file.SaveAs(context.Server.MapPath("~/uploads/" + file.FileName));
```

**Blazor:**
```csharp
var file = context.Request.Files["upload"];
var path = context.Server.MapPath("~/uploads/" + file.FileName);
using (var stream = System.IO.File.Create(path))
{
    await file.CopyToAsync(stream);
}
```

The logic is the same; the API is slightly different.

---

## Interaction with AshxHandlerMiddleware

If your application uses the `AshxHandlerMiddleware` (which returns 410 Gone for old `.ashx` files), migrated handlers will **bypass the middleware** and be handled by the routing system instead.

**Setup:**
```csharp
// Program.cs
var app = builder.Build();

// Ashx middleware (returns 410 for unmigrated handlers)
app.UseMiddleware<AshxHandlerMiddleware>();

// Handler routing (takes precedence)
app.MapBlazorWebFormsHandlers();

// Other middleware...
app.Run();
```

**How it works:**
1. Request arrives for `/api/Products.ashx`
2. `AshxHandlerMiddleware` runs first, checks: "Is there a handler registered for this path?"
3. Yes → middleware passes through (short-circuits)
4. ASP.NET Core routing finds the registered handler and executes it
5. If no handler found, middleware returns 410 Gone (old handlers are gone)

This setup lets you migrate handlers incrementally: old ones return 410; new ones work normally.

---

## Dependency Injection in Handlers

Handlers support constructor injection. Register your services in `Program.cs` and they'll be available:

```csharp
// Program.cs
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ProductRepository>();

// Handler.cs
[HandlerRoute("/api/users.ashx")]
public class UserApiHandler : HttpHandlerBase
{
    private readonly UserRepository _repo;

    public UserApiHandler(UserRepository repo)
    {
        _repo = repo;
    }

    public override async Task ProcessRequestAsync(HttpHandlerContext context)
    {
        var users = await _repo.GetAllAsync();
        context.Response.ContentType = "application/json";
        context.Response.Write(JsonSerializer.Serialize(users));
    }
}
```

---

## Testing Handlers

Use `TestServer` to test handlers in isolation:

```csharp
// xUnit test
[Fact]
public async Task GetProducts_ReturnsJson()
{
    var builder = new WebHostBuilder()
        .ConfigureServices(services =>
        {
            services.AddBlazorWebFormsComponents();
        })
        .Configure(app =>
        {
            app.MapHandler<ProductApiHandler>("/api/Products.ashx");
        });

    using (var server = new TestServer(builder))
    using (var client = server.CreateClient())
    {
        var response = await client.GetAsync("/api/Products.ashx?action=list");

        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("application/json", response.Content.Headers.ContentType.ToString());

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Widget", content);
    }
}
```

---

## Common Patterns and Examples

### Example: JSON POST Handler with Form Data

```csharp
[HandlerRoute("/api/Submit.ashx")]
public class FormSubmitHandler : HttpHandlerBase
{
    private readonly FormRepository _repo;

    public FormSubmitHandler(FormRepository repo)
    {
        _repo = repo;
    }

    public override async Task ProcessRequestAsync(HttpHandlerContext context)
    {
        if (context.Request.HttpMethod != "POST")
        {
            context.Response.StatusCode = 405;
            context.Response.Write("{\"error\":\"Method not allowed\"}");
            return;
        }

        var name = context.Request.Form["name"];
        var email = context.Request.Form["email"];

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email))
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            context.Response.Write("{\"error\":\"Missing fields\"}");
            return;
        }

        await _repo.SaveFormAsync(new { name, email });

        context.Response.ContentType = "application/json";
        context.Response.Write("{\"status\":\"submitted\"}");
    }
}
```

### Example: Authenticated Handler (GET only)

```csharp
[HandlerRoute("/secure/export.ashx")]
public class ExportHandler : HttpHandlerBase
{
    public override async Task ProcessRequestAsync(HttpHandlerContext context)
    {
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            context.Response.StatusCode = 403;
            context.Response.Write("Unauthorized");
            return;
        }

        var data = await GetExportDataAsync();
        var csv = ConvertToCSV(data);

        context.Response.ContentType = "text/csv";
        context.Response.AddHeader("Content-Disposition", 
            "attachment; filename=\"export.csv\"");
        context.Response.Write(csv);
    }
}

// Program.cs
app.MapHandler<ExportHandler>("/secure/export.ashx")
   .RequireAuthorization();
```

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| **Handler returns 404** | Verify the route in `[HandlerRoute]` matches the request URL. Check `MapBlazorWebFormsHandlers()` is called. |
| **Session state is null** | Add `[RequiresSessionState]` attribute. Ensure `app.UseSession()` is called in `Program.cs`. |
| **"Response.End() was called" warning** | Remove `Response.End()` and use `return` instead. |
| **Dependency injection fails** | Register services in `Program.cs` before `builder.Build()`. |
| **File upload not working** | Check content type is `multipart/form-data`. Use `context.Request.Files["fieldName"]`. |
| **CORS blocked** | Use `.RequireCors()` on the handler route. Ensure CORS policy is registered. |
| **Old handler returns 410** | This is correct — old `.ashx` files should return 410 until migrated. Use `AshxHandlerMiddleware` for this. |

---

## Summary

Migrating `.ashx` handlers to Blazor with `HttpHandlerBase` is straightforward:

- **6 mechanical changes** per handler (shown in Quick Start)
- **Familiar API surface** — `context.Request`, `context.Response`, `context.Server` work as in Web Forms
- **Modern routing** — Use `[HandlerRoute]` and `MapHandler<T>()` in `Program.cs`
- **DI support** — Inject services via constructor
- **Session state** — Mark with `[RequiresSessionState]`, configure in `Program.cs`
- **What's unsupported** — `Response.End()`, `Server.Transfer()`, global state — listed with workarounds

Your handler logic stays nearly identical. You're just updating the registration and removing obsolete patterns.

For questions or edge cases, refer to the API Reference section or consult the BlazorWebFormsComponents repository.
