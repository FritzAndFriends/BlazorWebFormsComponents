# Data Access Patterns & Reference

## Session State Migration

### Use SessionShim (Default — Works Everywhere)

Pages inheriting `WebFormsPageBase` get a `Session` property backed by `SessionShim`.
SessionShim works in BOTH SSR and interactive modes:
- **SSR:** Reads/writes to ASP.NET Core `ISession` (cookie-backed)
- **Interactive:** Uses in-memory `ConcurrentDictionary` scoped per circuit

**Original Web Forms AND Migrated Blazor (IDENTICAL):**
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

**Progression model:**
1. Start with SessionShim (zero migration cost)
2. Move to scoped services if you need typed, structured state
3. Move to database if you need persistence across circuits/sessions

---

## Entity Framework 6 → EF Core

**Web Forms:** EF6 with `DbContext` instantiated directly in code-behind or via `SelectMethod` string binding.
**Blazor:** EF Core **10.0.3** (latest .NET 10) with `IDbContextFactory` registered in DI.

> **CRITICAL: Preserve the original database provider.** Examine `Web.config` connection strings. Use the SAME provider — do NOT switch to SQLite unless the original used SQLite.

### Database Provider Detection & Migration

| Web.config Indicator | Original Provider | EF Core Package |
|---------------------|------------------|-----------------|
| `providerName="System.Data.SqlClient"` | SQL Server | `Microsoft.EntityFrameworkCore.SqlServer` |
| `providerName="System.Data.SQLite"` | SQLite | `Microsoft.EntityFrameworkCore.Sqlite` |
| `providerName="Npgsql"` | PostgreSQL | `Npgsql.EntityFrameworkCore.PostgreSQL` |
| `providerName="MySql.Data.MySqlClient"` | MySQL | `Pomelo.EntityFrameworkCore.MySql` |
| `providerName="Oracle.ManagedDataAccess.Client"` | Oracle | `Oracle.EntityFrameworkCore` |

### Provider Configuration

```csharp
// Program.cs — match the original provider
builder.Services.AddDbContextFactory<ProductContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    // ↑ Replace with UseNpgsql(), UseMySql(), UseSqlite(), etc. to match original
```

### Service Layer Pattern

```csharp
public class ProductService(IDbContextFactory<ProductContext> factory)
{
    public async Task<List<Product>> GetProductsAsync()
    {
        using var db = factory.CreateDbContext();
        return await db.Products.ToListAsync();
    }
}
```

> **Critical:** Use `IDbContextFactory`, NOT `AddDbContext`, for Blazor Server. Circuits are long-lived — a single `DbContext` accumulates stale data.

### EF6 → EF Core API Changes

| EF6 | EF Core | Notes |
|-----|---------|-------|
| `using System.Data.Entity;` | `using Microsoft.EntityFrameworkCore;` | Namespace change |
| `DbModelBuilder` in `OnModelCreating` | `ModelBuilder` | Same concepts, different API |
| `HasRequired()` / `HasOptional()` | Navigation properties + `IsRequired()` | Simpler relationship config |
| `Database.SetInitializer(...)` | `Database.EnsureCreated()` or Migrations | Different init strategy |
| `db.Products.Include("Category")` | `db.Products.Include(p => p.Category)` | Prefer lambda includes |
| `WillCascadeOnDelete(false)` | `.OnDelete(DeleteBehavior.Restrict)` | Cascade config |

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

## DataSource Controls → Service Injection

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
// Program.cs
builder.Services.AddBlazorWebFormsComponents(); // ⚠️ REQUIRED
builder.Services.AddDbContextFactory<ProductContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductService, ProductService>();

app.UseBlazorWebFormsComponents(); // ⚠️ REQUIRED — .aspx URL rewriting. BEFORE MapRazorComponents.
```

### SelectMethod String → SelectHandler Delegate

BWFC's `DataBoundComponent<ItemType>` has `SelectMethod` of type `SelectHandler<ItemType>` — signature: `(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount) → IQueryable<ItemType>`.

**Option A — Preserve SelectMethod as delegate (recommended):**
```csharp
SelectMethod="@productService.GetProducts"  // if signature matches SelectHandler<T>
```

**Option B — Items binding (ONLY when original used DataSource, NOT SelectMethod):**
```csharp
products = await ProductService.GetProductsAsync();
// then: Items="@products"
```

---

## State Migration Summary

| Web Forms | Blazor Equivalent | Scope |
|-----------|------------------|-------|
| `Session["key"]` | SessionShim (zero-change) | Per-circuit |
| `ViewState["key"]` | Component fields/properties | Per-component |
| `Application["key"]` | Singleton service | App-wide |
| `Cache["key"]` | `IMemoryCache` / `IDistributedCache` | Configurable |
| `TempData["key"]` | `ProtectedSessionStorage` | One read |

---

## Data Migration Gotchas

### DbContext Lifetime — CRITICAL
Always use `IDbContextFactory` and create short-lived `DbContext` instances per operation.

**WRONG — IQueryable from disposed context:**
```csharp
private IQueryable<Product> GetProducts(int categoryId)
{
    using var db = DbFactory.CreateDbContext();
    return db.Products.Where(p => p.CategoryId == categoryId); // Context disposed!
}
```

**RIGHT — materialize inside using block:**
```csharp
private IQueryable<Product> GetProducts(int categoryId)
{
    using var db = DbFactory.CreateDbContext();
    return db.Products.Where(p => p.CategoryId == categoryId)
        .ToList().AsQueryable(); // Execute NOW
}
```

### SelectHandler Must Materialize
```csharp
private IQueryable<Product> SelectProducts(int maxRows, int startRowIndex,
    string sortByExpression, out int totalRowCount)
{
    using var db = DbFactory.CreateDbContext();
    totalRowCount = db.Products.Count();
    return db.Products.OrderBy(p => p.Name)
        .Skip(startRowIndex).Take(maxRows)
        .ToList().AsQueryable(); // CRITICAL — materialize NOW
}
```

### ConfigurationManager Shim
`ConfigurationManager.AppSettings["key"]` works via BWFC's shim. Call `app.UseConfigurationManagerShim()` in `Program.cs`.

### ThreadAbortException Dead Code
`Response.Redirect(url, true)` does NOT throw in Blazor. Remove `catch (ThreadAbortException)` blocks. Code after `Response.Redirect()` WILL execute (unlike Web Forms).

### Static Helpers with HttpContext
Web Forms static helpers using `HttpContext.Current` must be refactored to accept dependencies via constructor injection.
