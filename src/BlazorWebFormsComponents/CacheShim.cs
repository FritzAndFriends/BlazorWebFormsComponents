using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BlazorWebFormsComponents;

/// <summary>
/// Compatibility shim for Web Forms <c>System.Web.Caching.Cache</c>
/// (<c>HttpRuntime.Cache</c> / <c>Page.Cache</c>).
/// Wraps ASP.NET Core <see cref="IMemoryCache"/> to provide dictionary-style
/// caching with <c>Cache["key"]</c> access patterns.
/// </summary>
public class CacheShim
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheShim> _logger;

    public CacheShim(IMemoryCache cache, ILogger<CacheShim> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Gets or sets a cache item by key.
    /// Equivalent to <c>Cache["key"]</c> in Web Forms.
    /// </summary>
    public object? this[string key]
    {
        get => Get(key);
        set
        {
            if (value == null)
                Remove(key);
            else
                Insert(key, value);
        }
    }

    /// <summary>
    /// Gets an item from the cache, or null if not found.
    /// </summary>
    public object? Get(string key)
    {
        _cache.TryGetValue(key, out var value);
        return value;
    }

    /// <summary>
    /// Gets a typed item from the cache, or default if not found.
    /// </summary>
    public T? Get<T>(string key)
    {
        if (_cache.TryGetValue(key, out var value) && value is T typed)
            return typed;
        return default;
    }

    /// <summary>
    /// Inserts an item into the cache with no expiration.
    /// Equivalent to <c>Cache.Insert(key, value)</c> in Web Forms.
    /// </summary>
    public void Insert(string key, object value)
    {
        _cache.Set(key, value);
    }

    /// <summary>
    /// Inserts an item with an absolute expiration.
    /// Equivalent to <c>Cache.Insert(key, value, null, absoluteExpiration, Cache.NoSlidingExpiration)</c>.
    /// </summary>
    public void Insert(string key, object value, DateTimeOffset absoluteExpiration)
    {
        _cache.Set(key, value, absoluteExpiration);
    }

    /// <summary>
    /// Inserts an item with a sliding expiration.
    /// Equivalent to <c>Cache.Insert(key, value, null, Cache.NoAbsoluteExpiration, slidingExpiration)</c>.
    /// </summary>
    public void Insert(string key, object value, TimeSpan slidingExpiration)
    {
        var options = new MemoryCacheEntryOptions { SlidingExpiration = slidingExpiration };
        _cache.Set(key, value, options);
    }

    /// <summary>
    /// Removes an item from the cache and returns it.
    /// Equivalent to <c>Cache.Remove(key)</c> in Web Forms (which returns the removed value).
    /// </summary>
    public object? Remove(string key)
    {
        if (_cache.TryGetValue(key, out var value))
        {
            _cache.Remove(key);
            return value;
        }
        return null;
    }
}
