# Cache Shim

The `CacheShim` class provides compatibility with the ASP.NET Web Forms `Cache` object (`System.Web.Caching.Cache`). It wraps ASP.NET Core's `IMemoryCache` so that migrated code-behind using `Cache["key"]` dictionary-style access compiles and functions correctly without rewriting.

Original Microsoft implementation: https://docs.microsoft.com/en-us/dotnet/api/system.web.caching.cache?view=netframework-4.8

## Background

In ASP.NET Web Forms, the `Cache` object was available on every `Page` and through `HttpRuntime.Cache`:

```csharp
// Web Forms code-behind
Cache["products"] = GetProductList();
var products = (List<Product>)Cache["products"];
Cache.Insert("config", configData, null,
    DateTime.Now.AddHours(1), Cache.NoSlidingExpiration);
Cache.Remove("products");
```

The `Cache` provided application-wide in-memory storage with optional expiration policies.

## Blazor Implementation

In Blazor, application caching is handled by `IMemoryCache` from `Microsoft.Extensions.Caching.Memory`. The `CacheShim` bridges the gap by:

1. **Dictionary-style access** — `Cache["key"]` get/set works like Web Forms
2. **`Insert()` overloads** — Supports no-expiry, absolute expiry, and sliding expiry
3. **`Get<T>(key)`** — Type-safe retrieval (bonus over Web Forms)
4. **`Remove(key)`** — Removes and returns the cached value, matching Web Forms behavior

### Availability

`CacheShim` is automatically registered when you call `AddBlazorWebFormsComponents()` in `Program.cs`. It requires `IMemoryCache`:

```csharp
// Program.cs — both are handled by AddBlazorWebFormsComponents()
builder.Services.AddMemoryCache();
builder.Services.AddScoped<CacheShim>();
```

Access it through `WebFormsPageBase`:

```razor
@inherits WebFormsPageBase

@code {
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Cache["greeting"] = "Hello, World!";
        var greeting = (string)Cache["greeting"];
    }
}
```

## Web Forms Usage

```csharp
// Simple get/set
Cache["products"] = GetProductList();
var products = (List<Product>)Cache["products"];

// Insert with absolute expiration
Cache.Insert("config", configData, null,
    DateTime.Now.AddHours(1), Cache.NoSlidingExpiration);

// Insert with sliding expiration
Cache.Insert("session-data", sessionData, null,
    Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(20));

// Remove
object removed = Cache.Remove("products");
```

## Blazor Usage

```razor
@inherits WebFormsPageBase

@code {
    private List<Product> _products = new();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        // Dictionary-style set
        Cache["products"] = GetProductList();

        // Dictionary-style get (returns object?)
        _products = (List<Product>)Cache["products"];

        // Type-safe get (returns T? — no cast needed)
        _products = Cache.Get<List<Product>>("products");

        // Insert with absolute expiration
        Cache.Insert("config", LoadConfig(),
            DateTimeOffset.Now.AddHours(1));

        // Insert with sliding expiration
        Cache.Insert("session-data", GetSessionData(),
            TimeSpan.FromMinutes(20));

        // Remove and get the removed value
        object? removed = Cache.Remove("products");
    }

    private void RefreshProducts()
    {
        // Setting null removes the item
        Cache["products"] = null;

        // Re-populate
        Cache["products"] = GetProductList();
    }
}
```

## API Reference

| Method | Signature | Description |
|---|---|---|
| Indexer get | `Cache["key"]` → `object?` | Gets item or `null` |
| Indexer set | `Cache["key"] = value` | Sets item; `null` removes it |
| `Get<T>` | `Cache.Get<T>(key)` → `T?` | Type-safe retrieval |
| `Insert` | `Cache.Insert(key, value)` | No expiration |
| `Insert` | `Cache.Insert(key, value, DateTimeOffset)` | Absolute expiration |
| `Insert` | `Cache.Insert(key, value, TimeSpan)` | Sliding expiration |
| `Remove` | `Cache.Remove(key)` → `object?` | Removes and returns item |

## Migration Path

| Web Forms | BWFC Shim | Native Blazor |
|---|---|---|
| `HttpRuntime.Cache["key"]` | `Cache["key"]` | `IMemoryCache.TryGetValue()` |
| `Cache["key"] = value` | `Cache["key"] = value` | `cache.Set(key, value)` |
| `Cache.Insert(key, val, null, expiry, noSliding)` | `Cache.Insert(key, val, expiry)` | `cache.Set(key, val, opts)` |
| `Cache.Remove(key)` | `Cache.Remove(key)` | `cache.Remove(key)` |

## Moving On

`CacheShim` is a migration bridge. As you refactor:

1. **Replace with `IMemoryCache`** — Inject `IMemoryCache` directly for full control over cache entry options
2. **Consider `IDistributedCache`** — For multi-server deployments, switch to Redis or SQL Server distributed caching
3. **Use typed wrappers** — Create service classes that encapsulate caching logic instead of sprinkling `Cache["key"]` throughout pages

```razor
@* Before (migration shim) *@
@inherits WebFormsPageBase
@code {
    var products = (List<Product>)Cache["products"];
    Cache.Insert("products", newList, DateTimeOffset.Now.AddHours(1));
}

@* After (native Blazor) *@
@inject IMemoryCache MemoryCache
@code {
    MemoryCache.TryGetValue("products", out List<Product> products);
    MemoryCache.Set("products", newList, DateTimeOffset.Now.AddHours(1));
}
```

## See Also

- [WebFormsPage](WebFormsPage.md) — Page-level base class providing the `Cache` property
- [Service Registration](ServiceRegistration.md) — How `AddBlazorWebFormsComponents()` registers services
- [L2 Automation Shims](L2AutomationShims.md) — Overview of all migration automation features
