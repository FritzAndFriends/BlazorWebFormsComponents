using System;
using BlazorWebFormsComponents;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Test;

/// <summary>
/// Unit tests for SessionShim in-memory fallback mode.
/// SessionShim falls back to ConcurrentDictionary when ISession is null.
/// These tests exercise the fallback path by omitting IHttpContextAccessor.
/// </summary>
public class SessionShimTests
{
	private SessionShim CreateShim()
	{
		// NullLogger satisfies the required logger param;
		// omitting httpContextAccessor triggers in-memory fallback
		return new SessionShim(NullLogger<SessionShim>.Instance);
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
	public void Indexer_SetNull_StoresNull()
	{
		var shim = CreateShim();

		// Should not throw
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

	#region Remove

	[Fact]
	public void Remove_ExistingKey_RemovesIt()
	{
		var shim = CreateShim();
		shim["target"] = "value";

		shim.Remove("target");

		shim.ContainsKey("target").ShouldBeFalse();
	}

	[Fact]
	public void Remove_MissingKey_NoException()
	{
		var shim = CreateShim();

		// Should not throw
		Should.NotThrow(() => shim.Remove("ghost"));
	}

	#endregion

	#region Clear

	[Fact]
	public void Clear_RemovesAll()
	{
		var shim = CreateShim();
		shim["a"] = 1;
		shim["b"] = 2;
		shim["c"] = 3;

		shim.Clear();

		shim.Count.ShouldBe(0);
	}

	#endregion

	#region Count

	[Fact]
	public void Count_ReturnsCorrectCount()
	{
		var shim = CreateShim();

		shim["one"] = 1;
		shim["two"] = 2;
		shim["three"] = 3;

		shim.Count.ShouldBe(3);
	}

	#endregion

	#region ContainsKey

	[Fact]
	public void ContainsKey_ReturnsTrueForExisting()
	{
		var shim = CreateShim();
		shim["present"] = "yes";

		shim.ContainsKey("present").ShouldBeTrue();
	}

	[Fact]
	public void ContainsKey_ReturnsFalseForMissing()
	{
		var shim = CreateShim();

		shim.ContainsKey("absent").ShouldBeFalse();
	}

	#endregion

	#region Generic Get<T>

	[Fact]
	public void GenericGet_ReturnsTypedValue()
	{
		var shim = CreateShim();
		shim["count"] = 42;

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

	#endregion

	#region Multiple Types

	[Fact]
	public void MultipleTypes_StoredCorrectly()
	{
		var shim = CreateShim();
		var complexObj = new TestPayload { Name = "Contoso", Id = 99 };

		shim["str"] = "hello";
		shim["num"] = 42;
		shim["flag"] = true;
		shim["obj"] = complexObj;

		shim["str"].ShouldBe("hello");
		shim["num"].ShouldBe(42);
		shim["flag"].ShouldBe(true);

		var retrieved = shim.Get<TestPayload>("obj");
		retrieved.ShouldNotBeNull();
		retrieved.Name.ShouldBe("Contoso");
		retrieved.Id.ShouldBe(99);
	}

	/// <summary>Test DTO for complex object storage verification.</summary>
	private class TestPayload
	{
		public string Name { get; set; } = string.Empty;
		public int Id { get; set; }
	}

	#endregion
}
