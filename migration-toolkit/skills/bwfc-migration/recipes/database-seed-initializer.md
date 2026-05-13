# Recipe: Database Seed Data / Initializer Migration

## Error Signature

```
CS0246: The type or namespace name 'IDatabaseInitializer' could not be found
CS1061: 'Database' does not contain a definition for 'SetInitializer'
CS0246: The type or namespace name 'DropCreateDatabaseIfModelChanges' could not be found
```

## Detection

```powershell
Select-String -Path **/*.cs -Pattern "IDatabaseInitializer|SetInitializer|DropCreateDatabase|CreateDatabaseIfNotExists"
```

## Root Cause

EF6 used `Database.SetInitializer<T>(new CreateDatabaseIfNotExists<T>())` or custom `IDatabaseInitializer<T>` classes to seed data. EF Core doesn't have this API — initialization uses `EnsureCreated()` or migrations.

The CLI typically quarantines initializer classes as artifacts under `migration-artifacts/compile-surface/`.

## Fix Pattern

Convert the initializer's `Seed()` method to a startup block in `Program.cs`:

```csharp
// Program.cs — after var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductContext>();
    db.Database.EnsureCreated();
    if (!db.Products.Any())
    {
        // Port seed data from the original initializer
        db.Products.AddRange(
            new Product { ProductName = "Widget A", UnitPrice = 9.99m, CategoryID = 1 },
            new Product { ProductName = "Widget B", UnitPrice = 19.99m, CategoryID = 1 }
            // ... etc.
        );
        db.SaveChanges();
    }
}
```

## Finding the Original Seed Data

1. Check `migration-artifacts/compile-surface/` for the quarantined initializer class
2. Look for a `Seed(XxxContext context)` method — this has the original data
3. Port the `AddRange()` or individual `Add()` calls to the pattern above

## For SQLite Development Databases

When using SQLite for local development (common in migrated apps):
```csharp
builder.Services.AddDbContextFactory<ProductContext>(options =>
    options.UseSqlite("Data Source=wingtiptoys.db"));
```

The `EnsureCreated()` call creates the SQLite file automatically with the schema from your model classes.

## Verification

After porting the seed logic, run the app and verify:
1. Database is created on first run
2. Seed data appears in the application
3. Subsequent runs don't duplicate seed data (the `if (!db.Products.Any())` guard)
