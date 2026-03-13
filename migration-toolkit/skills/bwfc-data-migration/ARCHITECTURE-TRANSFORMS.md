# Architecture Transforms

Code and configuration migration patterns for converting Web Forms infrastructure to Blazor Server equivalents.

---

## Global.asax → Program.cs

```csharp
// Web Forms — Global.asax
protected void Application_Start(object sender, EventArgs e)
{
    RouteConfig.RegisterRoutes(RouteTable.Routes);
    BundleConfig.RegisterBundles(BundleTable.Bundles);
}

protected void Application_Error(object sender, EventArgs e)
{
    var ex = Server.GetLastError();
    Logger.LogError(ex);
}

protected void Session_Start(object sender, EventArgs e)
{
    Session["Cart"] = new ShoppingCart();
}
```

```csharp
// Blazor — Program.cs
var builder = WebApplication.CreateBuilder(args);

// Services (replaces Application_Start registrations)
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddBlazorWebFormsComponents();
builder.Services.AddDbContextFactory<ProductContext>(options =>
    /* Use provider matching original: UseSqlServer, UseNpgsql, UseMySql, etc. */ ...);
builder.Services.AddScoped<CartService>(); // replaces Session_Start

var app = builder.Build();

// Middleware pipeline
app.UseExceptionHandler("/Error"); // replaces Application_Error
app.UseStaticFiles();
app.UseRouting();
app.UseBlazorWebFormsComponents(); // ⚠️ REQUIRED — .aspx URL rewriting (301 redirects). BEFORE MapRazorComponents.
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
```

### Global.asax Event → Blazor Equivalent

| Global.asax Event | Blazor Equivalent |
|-------------------|-------------------|
| `Application_Start` | `Program.cs` — service registration and app configuration |
| `Application_Error` | `app.UseExceptionHandler(...)` middleware |
| `Session_Start` | Scoped service constructor (lazy init) |
| `Session_End` | `IDisposable` on scoped services or circuit handler |
| `Application_BeginRequest` | Custom middleware |
| `Application_EndRequest` | Custom middleware |

---

## Web.config → appsettings.json

```xml
<!-- Web Forms — Web.config -->
<appSettings>
  <add key="PayPal:Mode" value="sandbox" />
  <add key="MaxItemsPerPage" value="20" />
</appSettings>
```

```json
// Blazor — appsettings.json
{
  "PayPal": {
    "Mode": "sandbox"
  },
  "MaxItemsPerPage": 20
}
```

```csharp
// Web Forms access
var mode = ConfigurationManager.AppSettings["PayPal:Mode"];

// Blazor access — IConfiguration
@inject IConfiguration Config
var mode = Config["PayPal:Mode"];

// Blazor access — Options pattern (recommended)
builder.Services.Configure<PayPalOptions>(builder.Configuration.GetSection("PayPal"));
@inject IOptions<PayPalOptions> PayPalOptions
var mode = PayPalOptions.Value.Mode;
```

---

## Route Table → @page Directives

```csharp
// Web Forms — RouteConfig.cs
routes.MapPageRoute("ProductRoute", "Product/{productId}", "~/ProductDetail.aspx");
routes.MapPageRoute("CategoryRoute", "Category/{categoryId}", "~/ProductList.aspx");
```

```razor
@* Blazor — ProductDetail.razor *@
@page "/Product/{ProductId:int}"
@code {
    [Parameter] public int ProductId { get; set; }
}

@* Blazor — ProductList.razor *@
@page "/Category/{CategoryId:int}"
@code {
    [Parameter] public int CategoryId { get; set; }
}
```

### URL Pattern Conversion

| Web Forms Route Pattern | Blazor @page Pattern |
|------------------------|---------------------|
| `{id}` | `{Id:int}` (add type constraint) |
| `{name}` | `{Name}` (string, no constraint needed) |
| `{category}/{subcategory}` | `{Category}/{Subcategory}` |
| Optional: `{id?}` | `{Id:int?}` |
| Default: `{action=Index}` | Multiple `@page` directives |

### Friendly URLs

```csharp
// Web Forms — FriendlyUrls
routes.EnableFriendlyUrls();
// Maps Products.aspx → /Products, Products/Details/5 → Products.aspx?id=5

// Blazor — direct @page mapping
@page "/Products"
@page "/Products/Details/{Id:int}"
```

---

## HTTP Handlers/Modules → Middleware

```csharp
// Web Forms — IHttpHandler
public class ImageHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var id = context.Request.QueryString["id"];
        // serve image
    }
    public bool IsReusable => true;
}
```

```csharp
// Blazor — Minimal API endpoint
app.MapGet("/api/images/{id}", async (int id, ImageService svc) =>
{
    var image = await svc.GetImageAsync(id);
    return Results.File(image.Data, image.ContentType);
});
```

```csharp
// Web Forms — IHttpModule
public class LoggingModule : IHttpModule
{
    public void Init(HttpApplication context)
    {
        context.BeginRequest += (s, e) => Log("Begin: " + context.Request.Url);
    }
}
```

```csharp
// Blazor — Middleware
app.Use(async (context, next) =>
{
    Log($"Begin: {context.Request.Path}");
    await next(context);
});
```

---

## Third-Party Integrations → HttpClient

```csharp
// Web Forms — WebRequest/WebClient
var request = WebRequest.Create("https://api.paypal.com/v1/payments");
request.Method = "POST";
// ... manual serialization and error handling
```

```csharp
// Blazor — Program.cs
builder.Services.AddHttpClient("PayPal", client =>
{
    client.BaseAddress = new Uri("https://api.paypal.com/v1/");
});

// Blazor — Service
public class PayPalService(IHttpClientFactory factory)
{
    public async Task<PaymentResult> CreatePaymentAsync(Order order)
    {
        var client = factory.CreateClient("PayPal");
        var response = await client.PostAsJsonAsync("payments", order);
        return await response.Content.ReadFromJsonAsync<PaymentResult>()!;
    }
}
```

---

## Files to Create During Migration

| File | Purpose | Replaces |
|------|---------|----------|
| `Program.cs` | Service registration, middleware | `Global.asax`, `Startup.cs`, `RouteConfig.cs` |
| `appsettings.json` | Configuration | `Web.config` `<appSettings>` and `<connectionStrings>` |
| `App.razor` | Root component with Router | `Default.aspx` (entry point) |
| `_Imports.razor` | Global usings | `Web.config` `<namespaces>` |
| `Components/Layout/MainLayout.razor` | Application layout | `Site.Master` |
| `Components/Pages/*.razor` | Pages | `*.aspx` files |
| `Services/*.cs` | Data access services | `SelectMethod` delegates, DataSource controls, code-behind queries |
| `Models/*.cs` | Domain models | Copy from Web Forms project |

---

## Blazor Enhanced Navigation

Blazor's **enhanced navigation** intercepts `<a href>` clicks and handles them as client-side SPA navigation. This is seamless for navigating between Blazor pages, but it **breaks links to minimal API endpoints** because the request never actually hits the server as an HTTP request.

### The Problem

```razor
@* ❌ BROKEN — Blazor intercepts the click, attempts client-side navigation *@
<a href="/AddToCart?productID=@product.ProductID">Add to Cart</a>

@* The user sees a blank page or "not found" because Blazor tries to render
   "/AddToCart" as a Razor component, but it's a minimal API endpoint *@
```

### Workaround Options

**Option 1: Use `<form method="post">` (Recommended)**

```razor
@* ✅ CORRECT — form POST is a full HTTP request, not intercepted by Blazor *@
<form method="post" action="/Cart/Add">
    <input type="hidden" name="productId" value="@product.ProductID" />
    <button type="submit" class="btn btn-primary">Add to Cart</button>
</form>
```

> **Important:** The endpoint MUST call `.DisableAntiforgery()` because Blazor's HTML rendering does not include antiforgery tokens. Example: `app.MapPost("/endpoint", handler).DisableAntiforgery();`

**Option 2: Add `data-enhance-nav="false"` to the link**

```razor
@* ✅ CORRECT — disables enhanced navigation for this specific link *@
<a href="/AddToCart?productID=@product.ProductID" data-enhance-nav="false">Add to Cart</a>
```

This tells Blazor to let the browser handle the navigation normally (full HTTP request).

**Option 3: JavaScript workaround**

```razor
@* ✅ Works — forces a full page navigation via JavaScript *@
<a href="/AddToCart?productID=@product.ProductID"
   onclick="window.location.href=this.href; return false;">Add to Cart</a>
```

### Which Workaround to Use

| Scenario | Recommended Approach |
|----------|---------------------|
| Auth operations (login/register/logout) | `<form method="post">` — always |
| Cart operations (add/remove/update) | `<form method="post">` — most reliable |
| Simple GET redirects to API endpoints | `data-enhance-nav="false"` on the `<a>` tag |
| Download links to file endpoints | `data-enhance-nav="false"` on the `<a>` tag |

> **Rule of thumb:** Any link that targets a minimal API endpoint (not a Blazor page) needs either `<form method="post">` or `data-enhance-nav="false"` to work correctly.
