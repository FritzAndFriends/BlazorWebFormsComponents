using System;
using System.Collections.Generic;
using BlazorWebFormsComponents;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Test;

/// <summary>
/// Unit tests for <see cref="FormShim"/> dual-mode support.
/// Tests both SSR mode (backed by <see cref="IFormCollection"/>) and
/// interactive mode (backed by <c>Dictionary&lt;string, StringValues&gt;</c>).
/// </summary>
public class FormShimTests
{
	#region Helpers

	/// <summary>
	/// Creates a FormShim wrapping an <see cref="IFormCollection"/> (SSR mode).
	/// </summary>
	private static FormShim CreateSsrShim(Dictionary<string, StringValues> data)
	{
		var formCollection = new FormCollection(data);
		return new FormShim(formCollection);
	}

	/// <summary>
	/// Creates a FormShim with no data source (null IFormCollection).
	/// </summary>
	private static FormShim CreateEmptyShim()
	{
		return new FormShim((IFormCollection?)null);
	}

	/// <summary>
	/// Creates a FormShim backed by interop dictionary (interactive mode).
	/// </summary>
	private static FormShim CreateInteropShim(Dictionary<string, StringValues> data)
	{
		return new FormShim(data);
	}

	#endregion

	#region Indexer — No Data

	[Fact]
	public void Indexer_NullForm_ReturnsNull()
	{
		var shim = CreateEmptyShim();

		shim["anything"].ShouldBeNull();
	}

	#endregion

	#region Indexer — IFormCollection (SSR)

	[Fact]
	public void Indexer_SsrMode_ReturnsValue()
	{
		var shim = CreateSsrShim(new Dictionary<string, StringValues>
		{
			["username"] = "jeff"
		});

		shim["username"].ShouldBe("jeff");
	}

	[Fact]
	public void Indexer_SsrMode_MissingKey_ReturnsNull()
	{
		var shim = CreateSsrShim(new Dictionary<string, StringValues>
		{
			["name"] = "val"
		});

		shim["missing"].ShouldBeNull();
	}

	[Fact]
	public void Indexer_SsrMode_MultipleValues_ReturnsFirst()
	{
		var shim = CreateSsrShim(new Dictionary<string, StringValues>
		{
			["color"] = new StringValues(new[] { "red", "blue", "green" })
		});

		shim["color"].ShouldBe("red");
	}

	#endregion

	#region Indexer — Interop Dictionary (Interactive)

	[Fact]
	public void Indexer_InteropMode_ReturnsValue()
	{
		var shim = CreateInteropShim(new Dictionary<string, StringValues>
		{
			["email"] = "test@example.com"
		});

		shim["email"].ShouldBe("test@example.com");
	}

	[Fact]
	public void Indexer_InteropMode_MissingKey_ReturnsNull()
	{
		var shim = CreateInteropShim(new Dictionary<string, StringValues>
		{
			["name"] = "val"
		});

		shim["absent"].ShouldBeNull();
	}

	[Fact]
	public void Indexer_InteropMode_MultipleValues_ReturnsFirst()
	{
		var shim = CreateInteropShim(new Dictionary<string, StringValues>
		{
			["items"] = new StringValues(new[] { "apple", "banana" })
		});

		shim["items"].ShouldBe("apple");
	}

	#endregion

	#region GetValues — No Data

	[Fact]
	public void GetValues_NullForm_ReturnsNull()
	{
		var shim = CreateEmptyShim();

		shim.GetValues("field").ShouldBeNull();
	}

	#endregion

	#region GetValues — IFormCollection (SSR)

	[Fact]
	public void GetValues_SsrMode_ReturnsArray()
	{
		var shim = CreateSsrShim(new Dictionary<string, StringValues>
		{
			["tags"] = new StringValues(new[] { "blazor", "dotnet" })
		});

		var result = shim.GetValues("tags");
		result.ShouldNotBeNull();
		result.Length.ShouldBe(2);
		result.ShouldContain("blazor");
		result.ShouldContain("dotnet");
	}

	[Fact]
	public void GetValues_SsrMode_MissingKey_ReturnsNull()
	{
		var shim = CreateSsrShim(new Dictionary<string, StringValues>
		{
			["present"] = "val"
		});

		shim.GetValues("ghost").ShouldBeNull();
	}

	[Fact]
	public void GetValues_SsrMode_SingleValue_ReturnsSingleElementArray()
	{
		var shim = CreateSsrShim(new Dictionary<string, StringValues>
		{
			["name"] = "solo"
		});

		var result = shim.GetValues("name");
		result.ShouldNotBeNull();
		result.Length.ShouldBe(1);
		result[0].ShouldBe("solo");
	}

	#endregion

	#region GetValues — Interop Dictionary (Interactive)

	[Fact]
	public void GetValues_InteropMode_ReturnsArray()
	{
		var shim = CreateInteropShim(new Dictionary<string, StringValues>
		{
			["sizes"] = new StringValues(new[] { "S", "M", "L" })
		});

		var result = shim.GetValues("sizes");
		result.ShouldNotBeNull();
		result.Length.ShouldBe(3);
		result.ShouldContain("M");
	}

	[Fact]
	public void GetValues_InteropMode_MissingKey_ReturnsNull()
	{
		var shim = CreateInteropShim(new Dictionary<string, StringValues>
		{
			["field"] = "value"
		});

		shim.GetValues("nope").ShouldBeNull();
	}

	#endregion

	#region AllKeys

	[Fact]
	public void AllKeys_NullForm_ReturnsEmpty()
	{
		var shim = CreateEmptyShim();

		shim.AllKeys.ShouldBeEmpty();
	}

	[Fact]
	public void AllKeys_SsrMode_ReturnsFieldNames()
	{
		var shim = CreateSsrShim(new Dictionary<string, StringValues>
		{
			["first"] = "a",
			["second"] = "b"
		});

		var keys = shim.AllKeys;
		keys.Length.ShouldBe(2);
		keys.ShouldContain("first");
		keys.ShouldContain("second");
	}

	[Fact]
	public void AllKeys_InteropMode_ReturnsFieldNames()
	{
		var shim = CreateInteropShim(new Dictionary<string, StringValues>
		{
			["username"] = "jeff",
			["password"] = "secret"
		});

		var keys = shim.AllKeys;
		keys.Length.ShouldBe(2);
		keys.ShouldContain("username");
		keys.ShouldContain("password");
	}

	#endregion

	#region Count

	[Fact]
	public void Count_NullForm_ReturnsZero()
	{
		var shim = CreateEmptyShim();

		shim.Count.ShouldBe(0);
	}

	[Fact]
	public void Count_SsrMode_ReturnsFieldCount()
	{
		var shim = CreateSsrShim(new Dictionary<string, StringValues>
		{
			["a"] = "1",
			["b"] = "2",
			["c"] = "3"
		});

		shim.Count.ShouldBe(3);
	}

	[Fact]
	public void Count_InteropMode_ReturnsFieldCount()
	{
		var shim = CreateInteropShim(new Dictionary<string, StringValues>
		{
			["x"] = "1",
			["y"] = "2"
		});

		shim.Count.ShouldBe(2);
	}

	#endregion

	#region ContainsKey

	[Fact]
	public void ContainsKey_NullForm_ReturnsFalse()
	{
		var shim = CreateEmptyShim();

		shim.ContainsKey("anything").ShouldBeFalse();
	}

	[Fact]
	public void ContainsKey_SsrMode_ExistingKey_ReturnsTrue()
	{
		var shim = CreateSsrShim(new Dictionary<string, StringValues>
		{
			["token"] = "abc123"
		});

		shim.ContainsKey("token").ShouldBeTrue();
	}

	[Fact]
	public void ContainsKey_SsrMode_MissingKey_ReturnsFalse()
	{
		var shim = CreateSsrShim(new Dictionary<string, StringValues>
		{
			["token"] = "abc123"
		});

		shim.ContainsKey("nope").ShouldBeFalse();
	}

	[Fact]
	public void ContainsKey_InteropMode_ExistingKey_ReturnsTrue()
	{
		var shim = CreateInteropShim(new Dictionary<string, StringValues>
		{
			["field"] = "value"
		});

		shim.ContainsKey("field").ShouldBeTrue();
	}

	[Fact]
	public void ContainsKey_InteropMode_MissingKey_ReturnsFalse()
	{
		var shim = CreateInteropShim(new Dictionary<string, StringValues>
		{
			["field"] = "value"
		});

		shim.ContainsKey("other").ShouldBeFalse();
	}

	#endregion

	#region SetFormData — Interactive Mode Mutation

	[Fact]
	public void SetFormData_PopulatesEmptyInteropShim()
	{
		var shim = CreateInteropShim(new Dictionary<string, StringValues>());

		shim.SetFormData(new Dictionary<string, StringValues>
		{
			["name"] = "Fritz",
			["role"] = "Developer"
		});

		shim["name"].ShouldBe("Fritz");
		shim["role"].ShouldBe("Developer");
		shim.Count.ShouldBe(2);
	}

	[Fact]
	public void SetFormData_ReplacesExistingInteropData()
	{
		var shim = CreateInteropShim(new Dictionary<string, StringValues>
		{
			["old"] = "stale"
		});

		shim.SetFormData(new Dictionary<string, StringValues>
		{
			["new"] = "fresh"
		});

		// Old data replaced
		shim["old"].ShouldBeNull();
		shim["new"].ShouldBe("fresh");
		shim.Count.ShouldBe(1);
	}

	[Fact]
	public void SetFormData_MultiValueField_PreservedCorrectly()
	{
		var shim = CreateInteropShim(new Dictionary<string, StringValues>());

		shim.SetFormData(new Dictionary<string, StringValues>
		{
			["checkboxes"] = new StringValues(new[] { "opt1", "opt2", "opt3" })
		});

		var values = shim.GetValues("checkboxes");
		values.ShouldNotBeNull();
		values.Length.ShouldBe(3);
		values.ShouldContain("opt1");
		values.ShouldContain("opt2");
		values.ShouldContain("opt3");
	}

	#endregion
}
