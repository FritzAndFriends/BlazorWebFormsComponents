# Recipe: `new DbContext()` → Factory Injection

## Error Signature

```
CS7036: There is no argument given that corresponds to the required parameter 'options' of 'XxxContext.XxxContext(DbContextOptions<XxxContext>)'
```

## Detection

```powershell
Select-String -Path **/*.cs -Pattern "new \w+Context\(\)" -SimpleMatch
```

## Root Cause

Web Forms code instantiated `DbContext` subclasses directly with `new MyContext()`. EF Core requires `DbContextOptions` in the constructor — there is no parameterless constructor.

This appears in two locations:
1. **Page code-behind** (`.razor.cs`) — direct DB access in lifecycle methods
2. **Logic/service classes** — business logic classes the page calls

## Fix — Page Code-Behind

```csharp
// BEFORE (Web Forms):
var db = new ProductContext();
var items = db.Products.ToList();

// AFTER (inject factory):
[Inject] public IDbContextFactory<ProductContext> DbFactory { get; set; }

private void LoadData()
{
    using var db = DbFactory.CreateDbContext();
    var items = db.Products.ToList();
}
```

**Search-and-replace pattern:**
1. Add `[Inject] public IDbContextFactory<XxxContext> DbFactory { get; set; }` to the class
2. Replace `new XxxContext()` → `DbFactory.CreateDbContext()`
3. Wrap in `using var db = ...` if not already in a `using` block
4. Add `using Microsoft.EntityFrameworkCore;` if missing

## Fix — Logic/Service Classes (Not Components)

Service classes can't use `[Inject]`. Accept `IDbContextFactory<T>` via constructor injection:

```csharp
// BEFORE:
public class ShoppingCartActions : IDisposable
{
    private ProductContext _db = new ProductContext();
    public void AddToCart(int productId) { /* uses _db */ }
    public void Dispose() => _db.Dispose();
}

// AFTER:
public class ShoppingCartActions
{
    private readonly IDbContextFactory<ProductContext> _dbFactory;
    public ShoppingCartActions(IDbContextFactory<ProductContext> dbFactory)
        => _dbFactory = dbFactory;

    public void AddToCart(int productId)
    {
        using var db = _dbFactory.CreateDbContext();
        // ... use db ...
    }
}
```

Register in `Program.cs`:
```csharp
builder.Services.AddScoped<ShoppingCartActions>();
```

## Verification

After applying the fix, the `new XxxContext()` errors resolve. Run `dotnet build` to confirm no remaining parameterless constructor calls.
