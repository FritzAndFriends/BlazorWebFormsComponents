using System;
using System.Collections.Generic;
using BlazorWebFormsComponents;
using Microsoft.AspNetCore.DataProtection;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Test;

/// <summary>
/// Contract tests for the ViewStateDictionary class.
/// Validates dictionary operations, null safety, type coercion,
/// dirty tracking, serialization roundtrip, and IDictionary interface compliance.
/// </summary>
public class ViewStateDictionaryTests
{
	#region Basic Dictionary Operations

	[Fact]
	public void Indexer_SetAndGet_ReturnsStoredValue()
	{
		var dict = new ViewStateDictionary();

		dict["name"] = "Wingtip Toys";

		dict["name"].ShouldBe("Wingtip Toys");
	}

	[Fact]
	public void Indexer_Overwrite_ReturnsLatestValue()
	{
		var dict = new ViewStateDictionary();

		dict["key"] = "first";
		dict["key"] = "second";

		dict["key"].ShouldBe("second");
	}

	[Fact]
	public void ContainsKey_ExistingKey_ReturnsTrue()
	{
		var dict = new ViewStateDictionary();
		dict["exists"] = "yes";

		dict.ContainsKey("exists").ShouldBeTrue();
	}

	[Fact]
	public void ContainsKey_MissingKey_ReturnsFalse()
	{
		var dict = new ViewStateDictionary();

		dict.ContainsKey("missing").ShouldBeFalse();
	}

	[Fact]
	public void Remove_ExistingKey_ReturnsTrueAndRemovesEntry()
	{
		var dict = new ViewStateDictionary();
		dict["key"] = "value";

		var result = dict.Remove("key");

		result.ShouldBeTrue();
		dict.ContainsKey("key").ShouldBeFalse();
	}

	[Fact]
	public void Remove_MissingKey_ReturnsFalse()
	{
		var dict = new ViewStateDictionary();

		dict.Remove("nonexistent").ShouldBeFalse();
	}

	[Fact]
	public void Clear_RemovesAllEntries()
	{
		var dict = new ViewStateDictionary();
		dict["a"] = 1;
		dict["b"] = 2;
		dict["c"] = 3;

		dict.Clear();

		dict.Count.ShouldBe(0);
		dict.ContainsKey("a").ShouldBeFalse();
	}

	[Fact]
	public void Count_ReflectsNumberOfEntries()
	{
		var dict = new ViewStateDictionary();

		dict.Count.ShouldBe(0);

		dict["a"] = 1;
		dict.Count.ShouldBe(1);

		dict["b"] = 2;
		dict.Count.ShouldBe(2);
	}

	#endregion

	#region Null Safety

	[Fact]
	public void Indexer_MissingKey_ReturnsNull()
	{
		var dict = new ViewStateDictionary();

		var result = dict["nonexistent"];

		result.ShouldBeNull();
	}

	[Fact]
	public void Indexer_NullValue_CanBeStoredAndRetrieved()
	{
		var dict = new ViewStateDictionary();

		dict["nullable"] = null;

		dict.ContainsKey("nullable").ShouldBeTrue();
		dict["nullable"].ShouldBeNull();
	}

	#endregion

	#region Type Coercion (In-Memory)

	[Fact]
	public void Indexer_StoreInt_CastWorks()
	{
		var dict = new ViewStateDictionary();

		dict["count"] = 42;

		var result = (int)dict["count"]!;
		result.ShouldBe(42);
	}

	[Fact]
	public void Indexer_StoreBool_CastWorks()
	{
		var dict = new ViewStateDictionary();

		dict["flag"] = true;

		var result = (bool)dict["flag"]!;
		result.ShouldBeTrue();
	}

	[Fact]
	public void Set_And_GetValueOrDefault_TypedRoundtrip()
	{
		var dict = new ViewStateDictionary();

		dict.Set("count", 42);

		var result = dict.GetValueOrDefault<int>("count");
		result.ShouldBe(42);
	}

	[Fact]
	public void Set_String_GetValueOrDefault_ReturnsCorrectValue()
	{
		var dict = new ViewStateDictionary();

		dict.Set("name", "Blazor");

		var result = dict.GetValueOrDefault<string>("name");
		result.ShouldBe("Blazor");
	}

	#endregion

	#region IsDirty Tracking

	[Fact]
	public void IsDirty_FalseOnCreation()
	{
		var dict = new ViewStateDictionary();

		dict.IsDirty.ShouldBeFalse();
	}

	[Fact]
	public void IsDirty_TrueAfterIndexerSet()
	{
		var dict = new ViewStateDictionary();

		dict["key"] = "value";

		dict.IsDirty.ShouldBeTrue();
	}

	[Fact]
	public void IsDirty_TrueAfterTypedSet()
	{
		var dict = new ViewStateDictionary();

		dict.Set("key", 42);

		dict.IsDirty.ShouldBeTrue();
	}

	[Fact]
	public void IsDirty_TrueAfterAdd()
	{
		var dict = new ViewStateDictionary();

		dict.Add("key", "value");

		dict.IsDirty.ShouldBeTrue();
	}

	[Fact]
	public void IsDirty_TrueAfterRemove()
	{
		var dict = new ViewStateDictionary();
		dict["key"] = "value";
		dict.MarkClean();

		dict.Remove("key");

		dict.IsDirty.ShouldBeTrue();
	}

	[Fact]
	public void IsDirty_TrueAfterClear()
	{
		var dict = new ViewStateDictionary();
		dict["key"] = "value";
		dict.MarkClean();

		dict.Clear();

		dict.IsDirty.ShouldBeTrue();
	}

	[Fact]
	public void IsDirty_FalseAfterMarkClean()
	{
		var dict = new ViewStateDictionary();
		dict["key"] = "value";

		dict.MarkClean();

		dict.IsDirty.ShouldBeFalse();
	}

	[Fact]
	public void IsDirty_TrueAgainAfterModificationFollowingMarkClean()
	{
		var dict = new ViewStateDictionary();
		dict["key"] = "value";
		dict.MarkClean();

		dict["another"] = "value2";

		dict.IsDirty.ShouldBeTrue();
	}

	#endregion

	#region Serialization Roundtrip

	private static IDataProtector CreateTestProtector()
	{
		var provider = new EphemeralDataProtectionProvider();
		return provider.CreateProtector("BWFC.ViewState.Test");
	}

	[Fact]
	public void Serialize_Deserialize_ProducesEquivalentDictionary()
	{
		var dict = new ViewStateDictionary();
		dict["name"] = "Wingtip";
		dict["count"] = 42;
		dict["active"] = true;
		var protector = CreateTestProtector();

		var payload = dict.Serialize(protector);
		var restored = ViewStateDictionary.Deserialize(payload, protector);

		restored.Count.ShouldBe(3);
		restored.ContainsKey("name").ShouldBeTrue();
		restored.ContainsKey("count").ShouldBeTrue();
		restored.ContainsKey("active").ShouldBeTrue();
	}

	[Fact]
	public void Serialize_Deserialize_EmptyDictionary()
	{
		var dict = new ViewStateDictionary();
		var protector = CreateTestProtector();

		var payload = dict.Serialize(protector);
		var restored = ViewStateDictionary.Deserialize(payload, protector);

		restored.Count.ShouldBe(0);
	}

	[Fact]
	public void Serialize_ProducesNonEmptyProtectedString()
	{
		var dict = new ViewStateDictionary();
		dict["key"] = "value";
		var protector = CreateTestProtector();

		var payload = dict.Serialize(protector);

		payload.ShouldNotBeNullOrEmpty();
	}

	[Fact]
	public void Deserialize_TamperedPayload_Throws()
	{
		var protector = CreateTestProtector();
		var tamperedPayload = "definitely-not-a-valid-protected-payload";

		Should.Throw<Exception>(() =>
		{
			ViewStateDictionary.Deserialize(tamperedPayload, protector);
		});
	}

	#endregion

	#region JSON Type Coercion After Deserialization

	[Fact]
	public void Deserialize_IntValue_GetValueOrDefaultWorks()
	{
		var dict = new ViewStateDictionary();
		dict["count"] = 42;
		var protector = CreateTestProtector();

		var payload = dict.Serialize(protector);
		var restored = ViewStateDictionary.Deserialize(payload, protector);

		var result = restored.GetValueOrDefault<int>("count");
		result.ShouldBe(42);
	}

	[Fact]
	public void Deserialize_BoolValue_GetValueOrDefaultWorks()
	{
		var dict = new ViewStateDictionary();
		dict["active"] = true;
		var protector = CreateTestProtector();

		var payload = dict.Serialize(protector);
		var restored = ViewStateDictionary.Deserialize(payload, protector);

		var result = restored.GetValueOrDefault<bool>("active");
		result.ShouldBeTrue();
	}

	[Fact]
	public void Deserialize_StringValue_GetValueOrDefaultWorks()
	{
		var dict = new ViewStateDictionary();
		dict["name"] = "Blazor";
		var protector = CreateTestProtector();

		var payload = dict.Serialize(protector);
		var restored = ViewStateDictionary.Deserialize(payload, protector);

		var result = restored.GetValueOrDefault<string>("name");
		result.ShouldBe("Blazor");
	}

	[Fact]
	public void Deserialize_DoubleValue_GetValueOrDefaultWorks()
	{
		var dict = new ViewStateDictionary();
		dict["price"] = 14.99;
		var protector = CreateTestProtector();

		var payload = dict.Serialize(protector);
		var restored = ViewStateDictionary.Deserialize(payload, protector);

		var result = restored.GetValueOrDefault<double>("price");
		result.ShouldBe(14.99);
	}

	[Fact]
	public void Deserialize_DateTimeValue_GetValueOrDefaultWorks()
	{
		var dict = new ViewStateDictionary();
		var date = new DateTime(2026, 3, 24, 12, 0, 0, DateTimeKind.Utc);
		dict["created"] = date;
		var protector = CreateTestProtector();

		var payload = dict.Serialize(protector);
		var restored = ViewStateDictionary.Deserialize(payload, protector);

		var result = restored.GetValueOrDefault<DateTime>("created");
		result.ShouldBe(date);
	}

	[Fact]
	public void Deserialize_MultipleTypedValues_AllCoerceCorrectly()
	{
		var dict = new ViewStateDictionary();
		dict["intVal"] = 100;
		dict["boolVal"] = false;
		dict["strVal"] = "hello";
		dict["dblVal"] = 3.14;
		var protector = CreateTestProtector();

		var payload = dict.Serialize(protector);
		var restored = ViewStateDictionary.Deserialize(payload, protector);

		restored.GetValueOrDefault<int>("intVal").ShouldBe(100);
		restored.GetValueOrDefault<bool>("boolVal").ShouldBeFalse();
		restored.GetValueOrDefault<string>("strVal").ShouldBe("hello");
		restored.GetValueOrDefault<double>("dblVal").ShouldBe(3.14);
	}

	#endregion

	#region Edge Cases

	[Fact]
	public void VeryLongStringValue_StoredAndRetrieved()
	{
		var dict = new ViewStateDictionary();
		var longString = new string('x', 100_000);

		dict["long"] = longString;

		dict["long"].ShouldBe(longString);
	}

	[Fact]
	public void SpecialCharactersInKeys_Work()
	{
		var dict = new ViewStateDictionary();

		dict["key with spaces"] = "a";
		dict["key.with.dots"] = "b";
		dict["key/with/slashes"] = "c";
		dict["key<with>angles"] = "d";
		dict[""] = "empty key";

		dict["key with spaces"].ShouldBe("a");
		dict["key.with.dots"].ShouldBe("b");
		dict["key/with/slashes"].ShouldBe("c");
		dict["key<with>angles"].ShouldBe("d");
		dict[""].ShouldBe("empty key");
	}

	[Fact]
	public void VeryLongStringValue_SurvivesSerializationRoundtrip()
	{
		var dict = new ViewStateDictionary();
		var longString = new string('x', 100_000);
		dict["long"] = longString;
		var protector = CreateTestProtector();

		var payload = dict.Serialize(protector);
		var restored = ViewStateDictionary.Deserialize(payload, protector);

		restored.GetValueOrDefault<string>("long").ShouldBe(longString);
	}

	[Fact]
	public void SpecialCharactersInKeys_SurviveSerializationRoundtrip()
	{
		var dict = new ViewStateDictionary();
		dict["key with spaces"] = "a";
		dict["key.with.dots"] = "b";
		dict["key<with>angles"] = "c";
		var protector = CreateTestProtector();

		var payload = dict.Serialize(protector);
		var restored = ViewStateDictionary.Deserialize(payload, protector);

		restored.GetValueOrDefault<string>("key with spaces").ShouldBe("a");
		restored.GetValueOrDefault<string>("key.with.dots").ShouldBe("b");
		restored.GetValueOrDefault<string>("key<with>angles").ShouldBe("c");
	}

	#endregion

	#region GetValueOrDefault<T>

	[Fact]
	public void GetValueOrDefault_MissingKey_ReturnsDefault()
	{
		var dict = new ViewStateDictionary();

		dict.GetValueOrDefault<int>("missing").ShouldBe(0);
		dict.GetValueOrDefault<string>("missing").ShouldBeNull();
		dict.GetValueOrDefault<bool>("missing").ShouldBeFalse();
	}

	[Fact]
	public void GetValueOrDefault_MissingKey_ReturnsProvidedDefault()
	{
		var dict = new ViewStateDictionary();

		dict.GetValueOrDefault("missing", 99).ShouldBe(99);
		dict.GetValueOrDefault("missing", "fallback").ShouldBe("fallback");
	}

	[Fact]
	public void GetValueOrDefault_ExistingKey_ReturnsCorrectTypedValue()
	{
		var dict = new ViewStateDictionary();
		dict.Set("count", 42);
		dict.Set("name", "Blazor");
		dict.Set("active", true);

		dict.GetValueOrDefault<int>("count").ShouldBe(42);
		dict.GetValueOrDefault<string>("name").ShouldBe("Blazor");
		dict.GetValueOrDefault<bool>("active").ShouldBeTrue();
	}

	[Fact]
	public void GetValueOrDefault_NullValue_ReturnsDefault()
	{
		var dict = new ViewStateDictionary();
		dict["key"] = null;

		dict.GetValueOrDefault<int>("key").ShouldBe(0);
	}

	[Fact]
	public void GetValueOrDefault_NullValue_ReturnsProvidedDefault()
	{
		var dict = new ViewStateDictionary();
		dict["key"] = null;

		dict.GetValueOrDefault("key", 42).ShouldBe(42);
	}

	#endregion

	#region IDictionary<string, object?> Interface

	[Fact]
	public void ImplementsIDictionaryInterface()
	{
		var dict = new ViewStateDictionary();

		(dict is IDictionary<string, object?>).ShouldBeTrue();
	}

	[Fact]
	public void Add_ViaInterface_AddsEntry()
	{
		IDictionary<string, object?> dict = new ViewStateDictionary();

		dict.Add("key", "value");

		dict["key"].ShouldBe("value");
		dict.Count.ShouldBe(1);
	}

	[Fact]
	public void Add_DuplicateKey_Throws()
	{
		var dict = new ViewStateDictionary();
		dict.Add("key", "value");

		Should.Throw<ArgumentException>(() =>
		{
			dict.Add("key", "other");
		});
	}

	[Fact]
	public void TryGetValue_ExistingKey_ReturnsTrueAndValue()
	{
		var dict = new ViewStateDictionary();
		dict["key"] = "value";

		var found = dict.TryGetValue("key", out var result);

		found.ShouldBeTrue();
		result.ShouldBe("value");
	}

	[Fact]
	public void TryGetValue_MissingKey_ReturnsFalse()
	{
		var dict = new ViewStateDictionary();

		var found = dict.TryGetValue("missing", out var result);

		found.ShouldBeFalse();
		result.ShouldBeNull();
	}

	[Fact]
	public void Keys_ReturnsAllKeys()
	{
		var dict = new ViewStateDictionary();
		dict["a"] = 1;
		dict["b"] = 2;
		dict["c"] = 3;

		dict.Keys.ShouldContain("a");
		dict.Keys.ShouldContain("b");
		dict.Keys.ShouldContain("c");
		dict.Keys.Count.ShouldBe(3);
	}

	[Fact]
	public void Values_ReturnsAllValues()
	{
		var dict = new ViewStateDictionary();
		dict["a"] = 1;
		dict["b"] = "two";

		dict.Values.Count.ShouldBe(2);
	}

	[Fact]
	public void IsReadOnly_ReturnsFalse()
	{
		IDictionary<string, object?> dict = new ViewStateDictionary();

		((ICollection<KeyValuePair<string, object?>>)dict).IsReadOnly.ShouldBeFalse();
	}

	[Fact]
	public void Enumerable_IteratesAllEntries()
	{
		var dict = new ViewStateDictionary();
		dict["a"] = 1;
		dict["b"] = 2;

		var count = 0;
		foreach (var kvp in dict)
		{
			count++;
		}

		count.ShouldBe(2);
	}

	#endregion

	#region Web Forms Migration Pattern

	[Fact]
	public void WebForms_ViewStateBackedProperty_Pattern()
	{
		// Classic pattern: get { object val = ViewState["Key"]; return val != null ? (int)val : 0; }
		var dict = new ViewStateDictionary();

		// Initial read — missing key returns null → default
		var val = dict["SelectedDepartmentId"];
		var result = val != null ? (int)val : 0;
		result.ShouldBe(0);

		// Set value
		dict["SelectedDepartmentId"] = 42;

		// Read back — cast works
		val = dict["SelectedDepartmentId"];
		result = val != null ? (int)val : 0;
		result.ShouldBe(42);
	}

	[Fact]
	public void WebForms_ViewStateBackedProperty_AfterSerializationRoundtrip()
	{
		// After serialization roundtrip, GetValueOrDefault<T> handles JSON coercion
		var dict = new ViewStateDictionary();
		dict["SelectedDepartmentId"] = 42;
		var protector = CreateTestProtector();

		var payload = dict.Serialize(protector);
		var restored = ViewStateDictionary.Deserialize(payload, protector);

		var result = restored.GetValueOrDefault<int>("SelectedDepartmentId");
		result.ShouldBe(42);
	}

	#endregion

	#region LoadFrom

	[Fact]
	public void LoadFrom_MergesEntries()
	{
		var source = new ViewStateDictionary();
		source["a"] = 1;
		source["b"] = 2;

		var target = new ViewStateDictionary();
		target["c"] = 3;
		target.LoadFrom(source);

		target.Count.ShouldBe(3);
		target["a"].ShouldBe(1);
		target["b"].ShouldBe(2);
		target["c"].ShouldBe(3);
	}

	[Fact]
	public void LoadFrom_OverwritesExistingKeys()
	{
		var source = new ViewStateDictionary();
		source["key"] = "new";

		var target = new ViewStateDictionary();
		target["key"] = "old";
		target.LoadFrom(source);

		target["key"].ShouldBe("new");
	}

	#endregion
}
