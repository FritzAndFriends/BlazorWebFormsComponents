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
- Replace `SelectMethod`/`DataSource` controls with service injection
- Migrate Entity Framework 6 to EF Core
- Convert `Session`/`ViewState`/`Application` state to Blazor patterns
- Migrate `Global.asax` to `Program.cs`
- Convert `Web.config` to `appsettings.json`
- Replace HTTP Handlers/Modules with middleware
- Wire up third-party integrations

---

## 1. Entity Framework 6 → EF Core

**Web Forms:** EF6 with `DbContext` instantiated directly in code-behind or via `SelectMethod`.
**Blazor:** EF Core with `IDbContextFactory` registered in DI.

```csharp
// Web Forms — direct DbContext in code-behind
public IQueryable<Product> GetProducts()
{
    var db = new ProductContext();
    return db.Products;
}
```

```csharp
// Blazor — Program.cs
builder.Services.AddDbContextFactory<ProductContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
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

<GridView Items="products" TItem="Product" AutoGenerateColumns="true" />

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
// Program.cs
builder.Services.AddDbContextFactory<ProductContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
```

### SelectMethod → Service Method Mapping

| Web Forms SelectMethod | Blazor Service Call |
|----------------------|---------------------|
| `SelectMethod="GetProducts"` | `products = await ProductService.GetProductsAsync();` |
| `SelectMethod="GetProduct"` | `product = await ProductService.GetProductAsync(id);` |
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
builder.Services.AddDbContextFactory<ProductContext>(options => ...);
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
| `Services/*.cs` | Data access services | `SelectMethod`s, DataSource controls, code-behind queries |
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
