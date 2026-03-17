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

## ⚠️ Session State Under Interactive Server Mode

> **CRITICAL:** When using `<Routes @rendermode="InteractiveServer" />` (global interactive server mode), `HttpContext.Session` is **NULL** during WebSocket rendering. Any code that accesses `HttpContext.Session` inside a Blazor component event handler or lifecycle method will throw a `NullReferenceException` or silently fail.

**Why this happens:** After the initial HTTP request establishes the SignalR circuit, Blazor communicates over WebSocket. There is no HTTP request/response — and therefore no session middleware processing — during component interactions.

**Options for session-dependent operations (shopping cart, user preferences, etc.):**

### Option A: Minimal API Endpoints (Recommended for form submissions)

Use the same `<form method="post">` → minimal API pattern used for auth. The endpoint has a real `HttpContext` with session access.

```csharp
// Program.cs
app.MapPost("/api/students/add", async (StudentDto dto, SchoolContext db) =>
{
    var student = new Student 
    { 
        FirstName = dto.FirstName, 
        LastName = dto.LastName, 
        EnrollmentDate = dto.EnrollmentDate 
    };
    db.Students.Add(student);
    await db.SaveChangesAsync();
    return Results.Ok(student.StudentID);
}).DisableAntiforgery();
```

```razor
@* In Students.razor *@
@inject HttpClient Http

@code {
    private async Task AddStudent()
    {
        await Http.PostAsJsonAsync("/api/students/add", newStudent);
        await RefreshGrid();
    }
}
```

> **Important:** The endpoint MUST call `.DisableAntiforgery()` because Blazor's HTML rendering does not include antiforgery tokens.

### Option B: Scoped Service (For transient UI state)

Replace `Session["key"]` with a scoped DI service. State lives in server memory for the duration of the SignalR circuit.

```csharp
// CartService.cs
public class CartService
{
    private readonly List<CartItem> _items = new();
    public void Add(CartItem item) => _items.Add(item);
    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();
    public decimal GetTotal() => _items.Sum(i => i.Price * i.Quantity);
}

// Program.cs
builder.Services.AddScoped<CartService>();

// Component usage
@inject CartService CartService

<button @onclick="() => CartService.Add(new CartItem(...))">Add</button>
```

**Trade-off:** State is lost if the user refreshes the page or the circuit disconnects. Good for transient UI state (form drafts, temporary selections), not for durable cart data.

### Option C: Database-Backed (For persistent state)

Store state in the database, keyed by user ID or a cookie-based session token. Survives circuit disconnects, page refreshes, and server restarts.

```csharp
// UserPreferencesService.cs  
public class UserPreferencesService(IDbContextFactory<SchoolContext> factory)
{
    public async Task<string?> GetAsync(string userId, string key)
    {
        using var db = factory.CreateDbContext();
        var pref = await db.UserPreferences.FirstOrDefaultAsync(p => p.UserId == userId && p.Key == key);
        return pref?.Value;
    }
    
    public async Task SetAsync(string userId, string key, string value)
    {
        using var db = factory.CreateDbContext();
        var pref = await db.UserPreferences.FirstOrDefaultAsync(p => p.UserId == userId && p.Key == key);
        if (pref != null)
            pref.Value = value;
        else
            db.UserPreferences.Add(new UserPreference { UserId = userId, Key = key, Value = value });
        await db.SaveChangesAsync();
    }
}

// Program.cs
builder.Services.AddScoped<UserPreferencesService>();
```

**Recommendation:** For shopping carts and other business-critical state, prefer Option A (minimal API endpoints) or Option C (database). Use Option B only for transient UI state that can be safely lost on refresh or disconnect.

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

## Reference Documents

Architecture migration patterns (Global.asax, Web.config, routes, handlers, enhanced navigation) are in the child document:

- **[ARCHITECTURE-TRANSFORMS.md](ARCHITECTURE-TRANSFORMS.md)**  Global.asax → Program.cs, Web.config → appsettings.json, route table → @page directives, HTTP handlers/modules → middleware, third-party integrations → HttpClient, files to create during migration, and Blazor enhanced navigation workarounds.

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


