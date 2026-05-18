# Data & Architecture Migration Skill

The **Data & Architecture Migration Skill** handles Layer 3 architecture decisions and data access pattern migrations from Web Forms to Blazor. This skill covers the decisions that require understanding your application's business logic and technical requirements.

---

## When to Use This Skill

Use this skill when migrating:

- Entity Framework 6 to EF Core
- DataSource controls (`SqlDataSource`, `ObjectDataSource`) to service injection
- Session state patterns (when you need persistence beyond SessionShim)
- `Global.asax` to `Program.cs` middleware
- `Web.config` to `appsettings.json`
- HTTP handlers (`.ashx`) and modules to middleware
- Third-party integrations and APIs

---

## 1. Entity Framework 6  EF Core

### Database Provider Detection

** CRITICAL:** Preserve the original database provider. Examine the Web Forms project's `Web.config` to identify the provider:

| Web.config Indicator | Provider | EF Core Package |
|---------------------|----------|-----------------|
| `System.Data.SqlClient` or `(LocalDB)` | SQL Server | `Microsoft.EntityFrameworkCore.SqlServer` |
| `System.Data.SQLite` | SQLite | `Microsoft.EntityFrameworkCore.Sqlite` |
| `Npgsql` or `Port=5432` | PostgreSQL | `Npgsql.EntityFrameworkCore.PostgreSQL` |
| `MySql.Data.MySqlClient` | MySQL | `Pomelo.EntityFrameworkCore.MySql` |
| `Oracle.ManagedDataAccess.Client` | Oracle | `Oracle.EntityFrameworkCore` |

** NEVER default to SQLite.** Most Web Forms apps use SQL Server (often LocalDB for dev).

### Migration Steps

1. **Install the correct EF Core provider:**
   ```bash
   # For SQL Server (most common)
   dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 10.0.3
   ```

2. **Migrate DbContext:**
   ```csharp
   // Web Forms (EF6)
   public class ApplicationDbContext : DbContext
   {
       public ApplicationDbContext() : base("DefaultConnection")
       {
       }
       
       public DbSet<Product> Products { get; set; }
   }
   ```

   ```csharp
   // Blazor (EF Core)
   public class ApplicationDbContext : DbContext
   {
       public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
       {
       }
       
       public DbSet<Product> Products { get; set; }
   }
   ```

3. **Register with DI using IDbContextFactory:**
   ```csharp
   // Program.cs
   builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
   ```

4. **Use in components:**
   ```razor
   @inject IDbContextFactory<ApplicationDbContext> DbFactory
   
   @code {
       protected override async Task OnInitializedAsync()
       {
           using var db = DbFactory.CreateDbContext();
           products = await db.Products.AsNoTracking().ToListAsync();
       }
   }
   ```

### Key EF Core Differences

| EF6 Pattern | EF Core Replacement |
|-------------|-------------------|
| `db.Products.ToList()` | `await db.Products.ToListAsync()` |
| `db.SaveChanges()` | `await db.SaveChangesAsync()` |
| `Include("Category")` | `Include(p => p.Category)` (strongly-typed) |
| Direct instantiation | Use `IDbContextFactory<T>` in Blazor |
| `db.Database.Connection.ConnectionString` | `db.Database.GetConnectionString()` |

---

## 2. DataSource Controls  Service Injection

Web Forms DataSource controls (`SqlDataSource`, `ObjectDataSource`, `EntityDataSource`) have **no BWFC equivalent**. Replace them with injected services.

### Migration Pattern

**Before (Web Forms):**
```html
<asp:SqlDataSource ID="ProductsDataSource" runat="server"
    ConnectionString="<%$ ConnectionStrings:DefaultConnection %>"
    SelectCommand="SELECT * FROM Products WHERE CategoryId = @CategoryId">
    <SelectParameters>
        <asp:QueryStringParameter Name="CategoryId" QueryStringField="id" />
    </SelectParameters>
</asp:SqlDataSource>

<asp:GridView DataSourceID="ProductsDataSource" runat="server" />
```

**After (Blazor with service):**

1. **Create a service:**
   ```csharp
   public interface IProductService
   {
       Task<List<Product>> GetProductsByCategoryAsync(int categoryId);
   }
   
   public class ProductService : IProductService
   {
       private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
       
       public ProductService(IDbContextFactory<ApplicationDbContext> dbFactory)
       {
           _dbFactory = dbFactory;
       }
       
       public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
       {
           using var db = _dbFactory.CreateDbContext();
           return await db.Products
               .Where(p => p.CategoryId == categoryId)
               .AsNoTracking()
               .ToListAsync();
       }
   }
   ```

2. **Register service:**
   ```csharp
   // Program.cs
   builder.Services.AddScoped<IProductService, ProductService>();
   ```

3. **Use in component:**
   ```razor
   @page "/Products"
   @inject IProductService ProductService
   
   <GridView TItem="Product" SelectMethod="@LoadProducts">
       <Columns>
           <BoundField DataField="Name" HeaderText="Name" />
           <BoundField DataField="Price" HeaderText="Price" />
       </Columns>
   </GridView>
   
   @code {
       [SupplyParameterFromQuery]
       public int Id { get; set; }
       
       private async Task<IEnumerable<Product>> LoadProducts()
       {
           return await ProductService.GetProductsByCategoryAsync(Id);
       }
   }
   ```

---

## 3. Session State Migration

### Use SessionShim (Default)

For most migration scenarios, **keep using `Session["key"]`**  the SessionShim makes it work AS-IS:

```csharp
// Web Forms
Session["CartId"] = Guid.NewGuid().ToString();
var cartId = Session["CartId"]?.ToString();

// Blazor  IDENTICAL CODE
Session["CartId"] = Guid.NewGuid().ToString();
var cartId = Session["CartId"]?.ToString();
```

SessionShim works in **both SSR and interactive modes**:
- **SSR:** Backed by ASP.NET Core `ISession` (cookie-based)
- **Interactive:** In-memory `ConcurrentDictionary` per circuit

### When to Upgrade Beyond SessionShim

Only consider alternatives when you need:

**Cross-tab persistence**  ProtectedBrowserStorage:
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

**Cross-server persistence**  Database-backed state:
```csharp
public class CartService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    
    public async Task<Cart> GetCartAsync(string userId)
    {
        using var db = _dbFactory.CreateDbContext();
        return await db.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId) ?? new Cart();
    }
}
```

**Typed state management**  Scoped services:
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

---

## 4. Global.asax  Program.cs

### Application_Start

**Before (Web Forms):**
```csharp
// Global.asax.cs
protected void Application_Start()
{
    RegisterRoutes(RouteTable.Routes);
    BundleConfig.RegisterBundles(BundleTable.Bundles);
}
```

**After (Blazor):**
```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Equivalent configuration
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddBlazorWebFormsComponents();

var app = builder.Build();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

### Application_Error

**Before (Web Forms):**
```csharp
protected void Application_Error()
{
    var exception = Server.GetLastError();
    Logger.LogError(exception, "Unhandled exception");
}
```

**After (Blazor):**
```csharp
// Program.cs
app.UseExceptionHandler("/Error");

// Create Error.razor page
@page "/Error"
@inject ILogger<Error> Logger

<h1>Error</h1>
<p>An error occurred.</p>

@code {
    [CascadingParameter]
    public HttpContext? HttpContext { get; set; }
    
    protected override void OnInitialized()
    {
        var feature = HttpContext?.Features.Get<IExceptionHandlerFeature>();
        if (feature?.Error is { } error)
        {
            Logger.LogError(error, "Unhandled exception");
        }
    }
}
```

---

## 5. Web.config  appsettings.json

### Connection Strings

**Before (Web Forms):**
```xml
<!-- Web.config -->
<connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Server=(localdb)\mssqllocaldb;Database=MyDb;Trusted_Connection=True;" 
         providerName="System.Data.SqlClient" />
</connectionStrings>
```

**After (Blazor):**
```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyDb;Trusted_Connection=True;"
  }
}
```

```csharp
// Program.cs
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### App Settings

**Before (Web Forms):**
```xml
<!-- Web.config -->
<appSettings>
    <add key="AdminEmail" value="admin@example.com" />
    <add key="MaxUploadSize" value="10485760" />
</appSettings>
```

```csharp
// Web Forms code
var email = ConfigurationManager.AppSettings["AdminEmail"];
```

**After (Blazor):**
```json
// appsettings.json
{
  "AppSettings": {
    "AdminEmail": "admin@example.com",
    "MaxUploadSize": 10485760
  }
}
```

```csharp
// Blazor  use IConfiguration
@inject IConfiguration Configuration

@code {
    protected override void OnInitialized()
    {
        var email = Configuration["AppSettings:AdminEmail"];
    }
}
```

---

## 6. HTTP Handlers (.ashx)  Middleware

### Generic Handler Migration

**Before (Web Forms):**
```csharp
// DownloadFile.ashx
public class DownloadFile : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var fileId = context.Request.QueryString["id"];
        var file = GetFile(fileId);
        context.Response.ContentType = file.ContentType;
        context.Response.BinaryWrite(file.Data);
    }
    
    public bool IsReusable => true;
}
```

**After (Blazor):**
```csharp
// Program.cs  minimal API endpoint
app.MapGet("/DownloadFile", async (HttpContext context, IFileService fileService, string id) =>
{
    var file = await fileService.GetFileAsync(id);
    return Results.File(file.Data, file.ContentType, file.FileName);
});
```

---

## How to Use This Skill

### In Copilot Chat

```
@workspace Use the data migration skill to help me convert this 
SqlDataSource to a service. The original connects to SQL Server LocalDB.
```

### Pattern-Specific Questions

```
Using BWFC data migration patterns, how should I migrate this EF6 DbContext 
to EF Core? The original uses SQL Server with lazy loading.
```

---

## Common Migration Scenarios

### Scenario: Shopping Cart State

**Before (Web Forms):**
```csharp
// Using Session
Session["CartId"] = Guid.NewGuid().ToString();
var items = (List<CartItem>)Session["CartItems"];
```

**After (Blazor)  Option 1: Keep SessionShim:**
```csharp
// Works AS-IS via SessionShim
Session["CartId"] = Guid.NewGuid().ToString();
var items = Session.Get<List<CartItem>>("CartItems");
```

**After (Blazor)  Option 2: Scoped Service:**
```csharp
// CartService.cs
public class CartService
{
    private List<CartItem> _items = new();
    
    public void AddItem(CartItem item) => _items.Add(item);
    public List<CartItem> GetItems() => _items;
}

// Program.cs
builder.Services.AddScoped<CartService>();
```

---

## Related Skills

- **[Core Migration](CoreMigration.md)**  For markup and code-behind
- **[Identity & Authentication](IdentityMigration.md)**  For auth migration

---

## Skill File Location

```
migration-toolkit/skills/bwfc-data-migration/SKILL.md
```

---

## Related Documentation

- [Quick Start Guide](../QuickStart.md)
- [Three-Layer Methodology](../Methodology.md)
- [Session State Shim](../../UtilityFeatures/ViewStateAndPostBack.md)
