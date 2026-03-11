---
name: bwfc-data-migration
description: "Migrate Web Forms data access and application architecture to Blazor Server. Covers Entity Framework 6 to EF Core, DataSource controls to service injection, Session state to scoped services, Global.asax to Program.cs, Web.config to appsettings.json, routing, HTTP handlers to middleware, and third-party integrations. Use for Layer 3 architecture decisions during Web Forms migration."
---

# Web Forms Data Access & Architecture Migration

This skill covers migrating Web Forms data access patterns and application architecture to Blazor Server. These are the **Layer 3 architecture decisions** that require project-specific judgment.

**Related skills:**
- `/bwfc-migration` — Core markup migration (controls, expressions, layouts)
- `/bwfc-identity-migration` — Authentication and authorization migration

---

## When to Use This Skill

Use this skill when you need to:
- Convert `SelectMethod` string to `SelectHandler` delegate, replace `DataSource` controls with service injection
- Migrate Entity Framework 6 to EF Core
- Convert `Session`/`ViewState`/`Application` state to Blazor patterns
- Migrate `Global.asax` to `Program.cs`
- Convert `Web.config` to `appsettings.json`
- Replace HTTP Handlers/Modules with middleware
- Wire up third-party integrations

---

## ⚠️ Session State Under Interactive Server Mode

> **CRITICAL:** When using `<Routes @rendermode="InteractiveServer" />` (global interactive server mode), `HttpContext.Session` is **NULL** during WebSocket rendering. Any code that accesses `HttpContext.Session` inside a Blazor component event handler or lifecycle method will throw a `NullReferenceException` or silently fail.

**Why this happens:** After the initial HTTP request establishes the SignalR circuit, Blazor communicates over WebSocket. There is no HTTP request/response — and therefore no session middleware processing — during component interactions.

**Options for session-dependent operations (shopping cart, user preferences, etc.):**

### Option A: Minimal API Endpoints (most reliable for HTTP-dependent state)

Use the same `<form method="post">` → minimal API pattern used for auth. The endpoint has a real `HttpContext` with session access.

```csharp
// Program.cs — cart add operation via minimal API
app.MapPost("/Cart/Add", async (HttpContext context, ShoppingCartService cart) =>
{
    var form = await context.Request.ReadFormAsync();
    if (int.TryParse(form["productId"], out var productId))
        cart.AddToCart(productId);
    return Results.Redirect("/ShoppingCart");
}).DisableAntiforgery();
```

```razor
@* Blazor page — form submits via HTTP POST, not a Blazor event *@
<form method="post" action="/Cart/Add">
    <input type="hidden" name="productId" value="@product.ProductID" />
    <button type="submit">Add to Cart</button>
</form>
```

> **Important:** The endpoint MUST call `.DisableAntiforgery()` because Blazor's HTML rendering does not include antiforgery tokens. Example: `app.MapPost("/endpoint", handler).DisableAntiforgery();`

### Option B: Scoped Services (in-memory, per-circuit)

Replace `Session["key"]` with a scoped DI service. State lives in server memory for the duration of the SignalR circuit.

```csharp
// CartService.cs — registered as AddScoped<CartService>()
public class CartService
{
    public ShoppingCart Cart { get; set; } = new();
    public void AddItem(int productId) { /* ... */ }
}
```

**Trade-off:** State is lost if the user refreshes the page or the circuit disconnects. Good for transient UI state, not for durable cart data.

### Option C: Database-Backed State (most durable)

Store state in the database, keyed by user ID or a cookie-based session token. Survives circuit disconnects, page refreshes, and server restarts.

```csharp
public class CartService(IDbContextFactory<ProductContext> factory)
{
    public async Task AddItemAsync(string userId, int productId)
    {
        using var db = factory.CreateDbContext();
        db.CartItems.Add(new CartItem { UserId = userId, ProductId = productId });
        await db.SaveChangesAsync();
    }
}
```

**Recommendation:** For shopping carts and other business-critical state, prefer Option A (minimal API) or Option C (database). Use Option B only for transient UI state that can be safely lost.

---

## 1. Entity Framework 6 → EF Core

**Web Forms:** EF6 with `DbContext` instantiated directly in code-behind or via `SelectMethod` string binding.
**Blazor:** EF Core **10.0.3** (latest .NET 10) with `IDbContextFactory` registered in DI.

> **CRITICAL: Preserve the original database provider.** Examine the Web Forms project's `Web.config` connection strings and EF configuration to identify the database provider (SQL Server, PostgreSQL, MySQL, SQLite, Oracle, etc.). The migrated Blazor application MUST use the **same database provider** — do NOT switch providers unless explicitly requested by the user.
>
> **⚠️ NEVER default to SQLite.** The most common Web Forms database is SQL Server (often LocalDB for dev). If you see `System.Data.SqlClient` or `(LocalDB)` in connection strings, use `Microsoft.EntityFrameworkCore.SqlServer` — NOT `Microsoft.EntityFrameworkCore.Sqlite`. SQLite is ONLY appropriate if the original application specifically used `System.Data.SQLite`.

### Database Provider Detection & Migration

**Step 1: Identify the original provider** from the Web Forms project:

| Web.config Indicator | Original Provider | EF Core Package |
|---------------------|------------------|-----------------|
| `providerName="System.Data.SqlClient"` | SQL Server | `Microsoft.EntityFrameworkCore.SqlServer` |
| `providerName="System.Data.SQLite"` | SQLite | `Microsoft.EntityFrameworkCore.Sqlite` |
| `providerName="Npgsql"` or `Server=...;Port=5432` | PostgreSQL | `Npgsql.EntityFrameworkCore.PostgreSQL` |
| `providerName="MySql.Data.MySqlClient"` | MySQL | `Pomelo.EntityFrameworkCore.MySql` or `MySql.EntityFrameworkCore` |
| `providerName="Oracle.ManagedDataAccess.Client"` | Oracle | `Oracle.EntityFrameworkCore` |

**Step 2: Install the matching EF Core provider package** in the Blazor project:

```bash
# Example for SQL Server
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 10.0.3

# Example for PostgreSQL
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 10.0.3

# Example for MySQL (Pomelo)
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 10.0.3
```

**Step 3: Configure the matching provider** in `Program.cs`:

```csharp
// SQL Server — matches System.Data.SqlClient
options.UseSqlServer(connectionString)

// PostgreSQL — matches Npgsql
options.UseNpgsql(connectionString)

// MySQL — matches MySql.Data.MySqlClient
options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))

// SQLite — matches System.Data.SQLite
options.UseSqlite(connectionString)
```

> Install matching EF Core packages for .NET 10: `Microsoft.EntityFrameworkCore`, the provider-specific package (see table above), `.Tools`, and `.Design`.

```csharp
// Web Forms — direct DbContext in code-behind
public IQueryable<Product> GetProducts()
{
    var db = new ProductContext();
    return db.Products;
}
```

```csharp
// Blazor — Program.cs (use the provider that matches the original Web Forms database)
builder.Services.AddDbContextFactory<ProductContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    // ↑ Replace with UseNpgsql(), UseMySql(), UseSqlite(), etc. to match original provider
```

```csharp
// Blazor — Service layer
public class ProductService(IDbContextFactory<ProductContext> factory)
{
    public async Task<List<Product>> GetProductsAsync()
    {
        using var db = factory.CreateDbContext();
        return await db.Products.ToListAsync();
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        using var db = factory.CreateDbContext();
        return await db.Products.FindAsync(id);
    }
}
```

> **Critical:** Use `IDbContextFactory`, NOT `AddDbContext`, for Blazor Server. Blazor circuits are long-lived — a single `DbContext` accumulates stale data and tracking issues.

### EF6 → EF Core API Changes

| EF6 | EF Core | Notes |
|-----|---------|-------|
| `using System.Data.Entity;` | `using Microsoft.EntityFrameworkCore;` | Namespace change |
| `DbModelBuilder` in `OnModelCreating` | `ModelBuilder` | Same concepts, different API |
| `HasRequired()` / `HasOptional()` | Navigation properties + `IsRequired()` | Simpler relationship config |
| `Database.SetInitializer(...)` | `Database.EnsureCreated()` or Migrations | Different init strategy |
| `db.Products.Include("Category")` | `db.Products.Include(p => p.Category)` | Prefer lambda includes |
| `WillCascadeOnDelete(false)` | `.OnDelete(DeleteBehavior.Restrict)` | Cascade config |
| `.HasDatabaseGeneratedOption(...)` | `.ValueGeneratedOnAdd()` | Key generation |

### Connection String Migration

```xml
<!-- Web Forms — Web.config -->
<connectionStrings>
  <add name="DefaultConnection"
       connectionString="Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=MyApp;Integrated Security=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

```json
// Blazor — appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=MyApp;Integrated Security=True"
  }
}
```

---

## 2. DataSource Controls → Service Injection

Web Forms `DataSource` controls have **no BWFC equivalent**. Replace with injected services.

```xml
<!-- Web Forms — declarative data binding -->
<asp:SqlDataSource ID="ProductsDS" runat="server"
    ConnectionString="<%$ ConnectionStrings:DefaultConnection %>"
    SelectCommand="SELECT * FROM Products" />
<asp:GridView DataSourceID="ProductsDS" runat="server" />
```

```razor
@* Blazor — service injection *@
@inject IProductService ProductService

<GridView Items="products" ItemType="Product" AutoGenerateColumns="true" />

@code {
    private List<Product> products = new();

    protected override async Task OnInitializedAsync()
    {
        products = await ProductService.GetProductsAsync();
    }
}
```

### Service Registration Pattern

```csharp
// Program.cs — use the provider that matches the original Web Forms database
builder.Services.AddDbContextFactory<ProductContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    // ↑ Match the original provider: UseNpgsql(), UseMySql(), UseSqlite(), etc.

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
```

### SelectMethod String → SelectHandler Delegate Conversion

BWFC's `DataBoundComponent<ItemType>` has a native `SelectMethod` parameter of type `SelectHandler<ItemType>` — a delegate with signature `(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount) → IQueryable<ItemType>`. When set, `OnAfterRenderAsync` automatically calls it to populate `Items`. This is the **native BWFC data-binding pattern** that mirrors how Web Forms did it.

**Option A — Preserve SelectMethod as delegate (recommended):**

| Web Forms SelectMethod | BWFC SelectMethod Delegate |
|----------------------|---------------------|
| `SelectMethod="GetProducts"` | `SelectMethod="@productService.GetProducts"` (if signature matches `SelectHandler<T>`) |
| `SelectMethod="GetProduct"` | `SelectMethod="@productService.GetProduct"` (or use `DataItem` for single-record controls) |

**Option B — Items binding (ONLY when original used DataSource, NOT SelectMethod):**

> ⚠️ Use Option B ONLY when the original Web Forms markup used `DataSource`/`DataBind()`, NOT when it used `SelectMethod`. If the original had `SelectMethod="GetProducts"`, you MUST use Option A above.

| Web Forms SelectMethod | Blazor Service Call |
|----------------------|---------------------|
| `SelectMethod="GetProducts"` | `products = await ProductService.GetProductsAsync();` then `Items="@products"` |
| `SelectMethod="GetProduct"` | `product = await ProductService.GetProductAsync(id);` then `DataItem="@product"` |

**CRUD methods** (no BWFC parameter equivalent — wire to service calls in event handlers):

| Web Forms Method | Blazor Service Call |
|----------------------|---------------------|
| `InsertMethod="InsertProduct"` | `await ProductService.InsertAsync(product);` |
| `UpdateMethod="UpdateProduct"` | `await ProductService.UpdateAsync(product);` |
| `DeleteMethod="DeleteProduct"` | `await ProductService.DeleteAsync(id);` |

---

## 3. Session State → Scoped Services

**Web Forms:** `Session["key"]` dictionary accessed anywhere.
**Blazor:** Scoped services via DI. For browser persistence, use `ProtectedSessionStorage`.

```csharp
// Web Forms
Session["ShoppingCart"] = cart;
var cart = (ShoppingCart)Session["ShoppingCart"];
```

```csharp
// Blazor — Scoped service (in-memory, per-circuit)
public class CartService
{
    public ShoppingCart Cart { get; set; } = new();
    public void AddItem(Product product, int quantity = 1) { ... }
    public decimal GetTotal() => Cart.Items.Sum(i => i.Price * i.Quantity);
}

// Program.cs
builder.Services.AddScoped<CartService>();

// Component
@inject CartService CartService
```

### State Storage Options

| Web Forms | Blazor Equivalent | Scope |
|-----------|------------------|-------|
| `Session["key"]` | Scoped service | Per-circuit (lost on disconnect) |
| `Session["key"]` (persistent) | `ProtectedSessionStorage` | Browser session tab |
| `Application["key"]` | Singleton service | App-wide |
| `Cache["key"]` | `IMemoryCache` or `IDistributedCache` | Configurable |
| `ViewState["key"]` | Component fields/properties | Per-component |
| `TempData["key"]` | `ProtectedSessionStorage` | One read |
| `Cookies` | `ProtectedLocalStorage` or HTTP endpoints | Browser |

### ProtectedSessionStorage Example

```razor
@inject ProtectedSessionStorage SessionStorage

@code {
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var result = await SessionStorage.GetAsync<ShoppingCart>("cart");
            cart = result.Success ? result.Value! : new ShoppingCart();
        }
    }

    private async Task SaveCart()
    {
        await SessionStorage.SetAsync("cart", cart);
    }
}
```

> **Note:** `ProtectedSessionStorage` only works after the first render (it requires JS interop). Always check in `OnAfterRenderAsync`, not `OnInitializedAsync`.

---

## 4. Global.asax → Program.cs

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

## 5. Web.config → appsettings.json

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

## 6. Route Table → @page Directives

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

## 7. HTTP Handlers/Modules → Middleware

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

## 8. Third-Party Integrations → HttpClient

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

## Common Data Migration Gotchas

### DbContext Lifetime
Blazor Server circuits are long-lived. Always use `IDbContextFactory` and create short-lived `DbContext` instances per operation.

### No Page-Level Transaction Scope
Web Forms `SelectMethod` runs inside a page lifecycle. Blazor doesn't have this. Use explicit transaction scopes in services if needed:
```csharp
using var db = factory.CreateDbContext();
using var transaction = await db.Database.BeginTransactionAsync();
// ... operations
await transaction.CommitAsync();
```

### Async All the Way
Web Forms `SelectMethod` returns `IQueryable` synchronously. Blazor services should be async:
```csharp
// WRONG: return db.Products.ToList();
// RIGHT: return await db.Products.ToListAsync();
```

### No ConfigurationManager
`ConfigurationManager.AppSettings["key"]` doesn't exist. Inject `IConfiguration` or use the Options pattern.

### Static Helpers with HttpContext
Web Forms often has static helper classes that access `HttpContext.Current`. These must be refactored to accept dependencies via constructor injection.

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
