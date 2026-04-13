---
name: bwfc-data-migration
description: "Migrate Web Forms data access and application architecture to Blazor Server. Covers EF6 to EF Core, Session state to scoped services, Global.asax to Program.cs, Web.config to appsettings.json, and HTTP handlers to middleware. WHEN: \"migrate EF6\", \"session state to services\", \"Global.asax to Program.cs\", \"Web.config to appsettings\", \"data access migration\"."
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

## Session State Migration

### Use SessionShim (Default — Works Everywhere)

Pages inheriting `WebFormsPageBase` get a `Session` property backed by `SessionShim`.
SessionShim works in BOTH SSR and interactive modes:
- **SSR:** Reads/writes to ASP.NET Core `ISession` (cookie-backed)
- **Interactive:** Uses in-memory `ConcurrentDictionary` scoped per circuit

**Original Web Forms:**
```csharp
Session["CartId"] = Guid.NewGuid().ToString();
var cartId = Session["CartId"].ToString();
Session["payment_amt"] = 99.99m;
```

**Migrated Blazor (IDENTICAL):**
```csharp
Session["CartId"] = Guid.NewGuid().ToString();
var cartId = Session["CartId"].ToString();
Session["payment_amt"] = 99.99m;
```

No `IHttpContextAccessor`. No Minimal API. No cookies. Just `Session["key"]`.

**For non-page components**, inject SessionShim directly:
```razor
@inject SessionShim Session

@code {
    protected override void OnInitialized()
    {
        var userId = Session["UserId"]?.ToString() ?? "guest";
    }
}
```

### When to Upgrade Beyond SessionShim

Only consider alternatives when you need cross-tab or cross-server persistence:

**ProtectedBrowserStorage** — For data that must survive page refreshes:
```csharp
@inject ProtectedSessionStorage SessionStorage

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        var result = await SessionStorage.GetAsync<ShoppingCart>("cart");
        cart = result.Success ? result.Value! : new ShoppingCart();
    }
}
```

**Database-backed** — For shopping carts that persist across sessions:
```csharp
public class CartService(IDbContextFactory<AppDbContext> factory)
{
    public async Task<Cart> GetCartAsync(string userId)
    {
        using var db = factory.CreateDbContext();
        return await db.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId) ?? new Cart();
    }
}
```

**Scoped services** — When the pattern doesn't fit key-value storage:
```csharp
public class WizardStateService
{
    public int CurrentStep { get; set; }
    public FormData Data { get; set; } = new();
    public bool IsComplete => CurrentStep == 5 && Data.IsValid();
}

// Program.cs
builder.Services.AddScoped<WizardStateService>();
```

**Progression model:**
1. Start with SessionShim (zero migration cost)
2. Move to scoped services if you need typed, structured state
3. Move to database if you need persistence across circuits/sessions

---

## 1. Entity Framework 6 → EF Core

**Web Forms:** EF6 with `DbContext` instantiated directly in code-behind or via `SelectMethod` string binding.
**Blazor:** EF Core **10.0.3** (latest .NET 10) with `IDbContextFactory` registered in DI.

> **Step 1: Detect the provider.** The L1 script's `Find-DatabaseProvider` function reads `Web.config` `<connectionStrings>` and scaffolds the correct EF Core package. Check the L1 output's `[DatabaseProvider]` review item for the detected provider and connection string. Use these values in your `Program.cs` configuration — do not guess or substitute.
>
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
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddBlazorWebFormsComponents(); // ⚠️ REQUIRED — registers BWFC services
builder.Services.AddDbContextFactory<ProductContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    // ↑ Match the original provider: UseNpgsql(), UseMySql(), UseSqlite(), etc.

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// ... after builder.Build() ...
app.UseBlazorWebFormsComponents(); // ⚠️ REQUIRED — .aspx URL rewriting middleware. BEFORE MapRazorComponents.
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
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

## 3. Session, ViewState, and Application State Migration

**Web Forms:** `Session["key"]`, `ViewState["key"]`, `Application["key"]` dictionaries.
**Blazor:** SessionShim (auto-registered by `AddBlazorWebFormsComponents()`), component fields, and singleton services.

### Session["key"] → SessionShim (Zero-Change Migration)

No code changes needed. `WebFormsPageBase.Session` delegates to `SessionShim` automatically:

```csharp
// Web Forms — works IDENTICALLY in Blazor via SessionShim
Session["ShoppingCart"] = cart;
var cart = (ShoppingCart)Session["ShoppingCart"];

// SessionShim also supports typed access:
var cart = Session.Get<ShoppingCart>("ShoppingCart");
Session.Set("ShoppingCart", cart);
```

**How SessionShim works:**
- **SSR mode:** Backed by ASP.NET Core `ISession` (cookie-persisted)
- **Interactive mode:** In-memory `ConcurrentDictionary` scoped per circuit
- **Seamless:** Automatically switches based on render mode

**For non-page components:**
```razor
@inject SessionShim Session

@code {
    private string GetUserId() => Session["UserId"]?.ToString() ?? "guest";
}
```

### ViewState["key"] → Component Fields

ViewState is component-instance state. Use normal C# fields/properties:

```csharp
// Web Forms
ViewState["CurrentPage"] = pageIndex;
var page = (int)ViewState["CurrentPage"];

// Blazor
private int currentPage;
```

### Application["key"] → Singleton Services

Application-wide state becomes singleton services:

```csharp
// AppStateService.cs
public class AppStateService
{
    private readonly ConcurrentDictionary<string, object> _state = new();
    public void Set(string key, object value) => _state[key] = value;
    public T? Get<T>(string key) => _state.TryGetValue(key, out var val) ? (T)val : default;
}

// Program.cs
builder.Services.AddSingleton<AppStateService>();
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

## Reference Documents

Architecture migration patterns (Global.asax, Web.config, routes, handlers, enhanced navigation) are in the child document:

- **[ARCHITECTURE-TRANSFORMS.md](ARCHITECTURE-TRANSFORMS.md)**  Global.asax → Program.cs, Web.config → appsettings.json, route table → @page directives, HTTP handlers/modules → middleware, third-party integrations → HttpClient, files to create during migration, and Blazor enhanced navigation workarounds.

---

## Common Data Migration Gotchas

### DbContext Lifetime — CRITICAL
Blazor Server circuits are long-lived. Always use `IDbContextFactory` and create short-lived `DbContext` instances per operation.

**WRONG — IQueryable returned from disposed context:**
```csharp
private IQueryable<Product> GetProducts(int categoryId)
{
    using var db = DbFactory.CreateDbContext();
    return db.Products.Where(p => p.CategoryId == categoryId); // Context disposed before query executes!
}
```

**RIGHT — materialize inside using block:**
```csharp
private IQueryable<Product> GetProducts(int categoryId)
{
    using var db = DbFactory.CreateDbContext();
    var results = db.Products
        .Where(p => p.CategoryId == categoryId)
        .ToList(); // Execute query NOW while context is alive
    return results.AsQueryable(); // Return materialized data as IQueryable
}
```

**For SelectHandler delegates**, the delegate is invoked by BWFC infrastructure AFTER your method returns. You MUST materialize:
```csharp
// BWFC SelectHandler delegate — MUST materialize
private IQueryable<Product> SelectProducts(int maxRows, int startRowIndex, 
    string sortByExpression, out int totalRowCount)
{
    using var db = DbFactory.CreateDbContext();
    totalRowCount = db.Products.Count();
    
    var results = db.Products
        .OrderBy(p => p.Name)
        .Skip(startRowIndex)
        .Take(maxRows)
        .ToList(); // CRITICAL — materialize NOW
        
    return results.AsQueryable();
}
```

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

### ConfigurationManager Shim Available
`ConfigurationManager.AppSettings["key"]` works via BWFC's `ConfigurationManager` shim. Call `app.UseConfigurationManagerShim()` in `Program.cs` to bind it to `IConfiguration`. For new code, prefer injecting `IConfiguration` or using the Options pattern.

### Static Helpers with HttpContext
Web Forms often has static helper classes that access `HttpContext.Current`. These must be refactored to accept dependencies via constructor injection.

### ThreadAbortException Dead Code Warning
Web Forms throws `ThreadAbortException` when `Response.Redirect(url, true)` is called with `endResponse=true`. Blazor does **not** throw this exception — `ResponseShim.Redirect()` silently ignores the `endResponse` parameter. Any `catch (ThreadAbortException)` blocks become dead code after migration. Review and remove them. Code that runs AFTER `Response.Redirect(url, true)` **will execute** in Blazor (unlike Web Forms where execution stopped).

---

## ❌ Common Anti-Patterns to Avoid

### DO NOT Create Minimal API Endpoints for Page Actions

Minimal APIs are for **real HTTP endpoints** (REST APIs, webhooks), NOT for migrating Web Forms page actions.

**WRONG:**
```csharp
// Program.cs — creating API endpoint for a page action
app.MapPost("/api/cart/add", async (CartItem item, CartService cart) =>
{
    cart.Add(item);
    return Results.Ok();
});

// Cart.razor — calling the API
await Http.PostAsJsonAsync("/api/cart/add", item);
```

**RIGHT:**
```csharp
// Cart.razor — just call the service directly
@inject CartService CartService

<button @onclick="() => CartService.Add(item)">Add to Cart</button>
```

**When Minimal APIs ARE appropriate:**
- External REST API consumed by mobile apps, SPAs, or third parties
- Webhooks from payment processors, GitHub, etc.
- Form POST endpoints for authentication (login/logout/register) — these need HTTP context for cookies

**When they are NOT appropriate:**
- Replacing button click handlers in migrated Web Forms pages
- Working around Session["key"] access — use SessionShim instead
- "Because HttpContext is null" — you don't need HttpContext for most operations

### DO NOT Use IHttpContextAccessor to Access Session

You already have `Session` via `WebFormsPageBase` or `@inject SessionShim`.

**WRONG:**
```csharp
@inject IHttpContextAccessor HttpContextAccessor

var session = HttpContextAccessor.HttpContext?.Session;
var cartId = session?.GetString("CartId");
```

**RIGHT:**
```csharp
@inherits WebFormsPageBase

var cartId = Session["CartId"]?.ToString();
```

### DO NOT Replace Session with Cookies

If the original Web Forms code used `Session["key"]`, use `SessionShim`. Don't invent cookie-based workarounds.

**WRONG:**
```csharp
// Creating cookie-based cart ID because "Session doesn't work in Blazor"
Response.Cookies.Append("CartId", Guid.NewGuid().ToString());
var cartId = Request.Cookies["CartId"];
```

**RIGHT:**
```csharp
// SessionShim handles the storage — just use Session
Session["CartId"] = Guid.NewGuid().ToString();
var cartId = Session["CartId"]?.ToString();
```

### DO NOT Use HttpContext.Current.Session

There is no `HttpContext.Current` in ASP.NET Core. Use the `Session` property.

**WRONG:**
```csharp
HttpContext.Current.Session["UserId"] = userId;
```

**RIGHT:**
```csharp
Session["UserId"] = userId; // From WebFormsPageBase or injected SessionShim
```

---


