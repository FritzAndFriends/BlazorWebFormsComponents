using System;
using BlazorWebFormsComponents;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Test;

/// <summary>
/// Unit tests for <see cref="CacheShim"/>.
/// </summary>
public class CacheShimTests
{
    private CacheShim CreateShim()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        return new CacheShim(cache, NullLogger<CacheShim>.Instance);
    }

    #region Indexer

    [Fact]
    public void Indexer_SetAndGet_ReturnsValue()
    {
        var shim = CreateShim();

        shim["greeting"] = "hello";

        shim["greeting"].ShouldBe("hello");
    }

    [Fact]
    public void Indexer_GetMissing_ReturnsNull()
    {
        var shim = CreateShim();

        shim["nonexistent"].ShouldBeNull();
    }

    [Fact]
    public void Indexer_SetNull_RemovesEntry()
    {
        var shim = CreateShim();
        shim["key"] = "value";

        shim["key"] = null;

        shim["key"].ShouldBeNull();
    }

    [Fact]
    public void Indexer_OverwriteValue_ReturnsNewValue()
    {
        var shim = CreateShim();

        shim["key"] = "first";
        shim["key"] = "second";

        shim["key"].ShouldBe("second");
    }

    #endregion

    #region Get

    [Fact]
    public void Get_ReturnsNull_ForMissingKey()
    {
        var shim = CreateShim();

        shim.Get("missing").ShouldBeNull();
    }

    [Fact]
    public void Get_ReturnsStoredValue()
    {
        var shim = CreateShim();
        shim.Insert("data", 42);

        shim.Get("data").ShouldBe(42);
    }

    #endregion

    #region Get<T>

    [Fact]
    public void GenericGet_ReturnsTypedValue()
    {
        var shim = CreateShim();
        shim.Insert("count", 42);

        var result = shim.Get<int>("count");

        result.ShouldBe(42);
    }

    [Fact]
    public void GenericGet_MissingKey_ReturnsDefault()
    {
        var shim = CreateShim();

        var result = shim.Get<int>("missing");

        result.ShouldBe(0); // default(int)
    }

    [Fact]
    public void GenericGet_WrongType_ReturnsDefault()
    {
        var shim = CreateShim();
        shim.Insert("str", "hello");

        var result = shim.Get<int>("str");

        result.ShouldBe(0); // default(int)
    }

    #endregion

    #region Insert

    [Fact]
    public void Insert_NoExpiration_StoresValue()
    {
        var shim = CreateShim();

        shim.Insert("key", "value");

        shim.Get("key").ShouldBe("value");
    }

    [Fact]
    public void Insert_WithSlidingExpiration_StoresValue()
    {
        var shim = CreateShim();

        shim.Insert("key", "value", TimeSpan.FromMinutes(10));

        shim.Get("key").ShouldBe("value");
    }

    [Fact]
    public void Insert_WithAbsoluteExpiration_StoresValue()
    {
        var shim = CreateShim();

        shim.Insert("key", "value", DateTimeOffset.UtcNow.AddHours(1));

        shim.Get("key").ShouldBe("value");
    }

    #endregion

    #region Remove

    [Fact]
    public void Remove_ExistingKey_ReturnsRemovedValue()
    {
        var shim = CreateShim();
        shim.Insert("target", "payload");

        var removed = shim.Remove("target");

        removed.ShouldBe("payload");
        shim.Get("target").ShouldBeNull();
    }

    [Fact]
    public void Remove_MissingKey_ReturnsNull()
    {
        var shim = CreateShim();

        var removed = shim.Remove("ghost");

        removed.ShouldBeNull();
    }

    #endregion

    #region Complex Types

    [Fact]
    public void ComplexObject_StoredAndRetrieved()
    {
        var shim = CreateShim();
        var obj = new TestPayload { Name = "Contoso", Id = 99 };

        shim.Insert("obj", obj);

        var retrieved = shim.Get<TestPayload>("obj");
        retrieved.ShouldNotBeNull();
        retrieved.Name.ShouldBe("Contoso");
        retrieved.Id.ShouldBe(99);
    }

    private class TestPayload
    {
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; }
    }

    #endregion
}
