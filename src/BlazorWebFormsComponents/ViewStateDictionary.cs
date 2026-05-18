using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;

namespace BlazorWebFormsComponents;

/// <summary>
/// Dictionary-based state storage emulating ASP.NET Web Forms ViewState.
/// In ServerInteractive mode, persists for the component's lifetime (in-memory).
/// In SSR mode, round-trips via a protected hidden form field.
///
/// <para><b>Migration shim — not a destination.</b> This exists so Web Forms
/// <c>ViewState["key"]</c> patterns compile and run correctly in Blazor during
/// migration. Once running, refactor to <c>[Parameter]</c> properties, component
/// fields, or cascading values. Unlike Web Forms ViewState, this implementation
/// is opt-in, per-component, dirty-tracked, and encrypted by default.</para>
/// </summary>
public class ViewStateDictionary : IDictionary<string, object?>
{
	private readonly Dictionary<string, object?> _store = new();

	/// <summary>
	/// Gets or sets the value associated with the specified key.
	/// Returns <c>null</c> for missing keys (matches Web Forms ViewState behavior).
	/// </summary>
	public object? this[string key]
	{
		get => _store.TryGetValue(key, out var value) ? value : null;
		set
		{
			_store[key] = value;
			IsDirty = true;
		}
	}

	/// <summary>
	/// Type-safe convenience method to retrieve a value from ViewState.
	/// Handles JSON deserialization type coercion automatically.
	/// </summary>
	/// <typeparam name="T">The expected value type.</typeparam>
	/// <param name="key">The key to look up.</param>
	/// <param name="defaultValue">Value to return if the key is missing or null.</param>
	/// <returns>The stored value converted to <typeparamref name="T"/>, or <paramref name="defaultValue"/>.</returns>
	public T GetValueOrDefault<T>(string key, T defaultValue = default!)
	{
		if (!_store.TryGetValue(key, out var value) || value is null)
			return defaultValue;

		return CoerceValue<T>(value);
	}

	/// <summary>
	/// Type-safe convenience method to store a value in ViewState.
	/// </summary>
	/// <typeparam name="T">The value type.</typeparam>
	/// <param name="key">The key to store under.</param>
	/// <param name="value">The value to store.</param>
	public void Set<T>(string key, T value)
	{
		_store[key] = value;
		IsDirty = true;
	}

	/// <summary>
	/// Indicates whether the dictionary has been modified since the last
	/// <see cref="MarkClean"/> call. Used to skip serialization when no changes occurred.
	/// </summary>
	internal bool IsDirty { get; private set; }

	/// <summary>
	/// Resets the dirty flag after serialization.
	/// </summary>
	internal void MarkClean() => IsDirty = false;

	/// <summary>
	/// Serializes the dictionary to a protected (encrypted + signed) string
	/// suitable for embedding in a hidden form field.
	/// </summary>
	/// <param name="protector">The data protector to encrypt and sign the payload.</param>
	/// <returns>A protected string containing the serialized ViewState.</returns>
	internal string Serialize(IDataProtector protector)
	{
		var json = JsonSerializer.Serialize(_store);
		return protector.Protect(json);
	}

	/// <summary>
	/// Serializes the dictionary to a protected string, optionally logging a warning
	/// when the payload exceeds a size threshold (approximate UTF-16 byte size in a hidden field).
	/// </summary>
	/// <param name="protector">The data protector to encrypt and sign the payload.</param>
	/// <param name="logger">Optional logger for size warnings.</param>
	/// <param name="warningThresholdBytes">Byte threshold above which a warning is logged. Default 4096.</param>
	/// <returns>A protected string containing the serialized ViewState.</returns>
	internal string Serialize(IDataProtector protector, ILogger? logger, int warningThresholdBytes = 4096)
	{
		var json = JsonSerializer.Serialize(_store);
		var payload = protector.Protect(json);

		var estimatedBytes = payload.Length * 2;
		if (logger is not null && estimatedBytes > warningThresholdBytes)
		{
			logger.LogWarning(
				"ViewState payload is {PayloadLength} bytes (threshold: {WarningThresholdBytes}). Consider reducing state or using server-side storage for large datasets.",
				estimatedBytes,
				warningThresholdBytes);
		}

		return payload;
	}

	/// <summary>
	/// Deserializes a protected ViewState payload back into a <see cref="ViewStateDictionary"/>.
	/// Values are stored as <see cref="JsonElement"/> for lazy type coercion.
	/// </summary>
	/// <param name="protectedPayload">The protected string from the hidden form field.</param>
	/// <param name="protector">The data protector to decrypt and verify the payload.</param>
	/// <returns>A new <see cref="ViewStateDictionary"/> populated with the deserialized state.</returns>
	internal static ViewStateDictionary Deserialize(string protectedPayload, IDataProtector protector)
	{
		var json = protector.Unprotect(protectedPayload);
		var dict = new ViewStateDictionary();
		var values = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
		if (values is not null)
		{
			foreach (var kvp in values)
			{
				dict._store[kvp.Key] = kvp.Value;
			}
		}
		return dict;
	}

	/// <summary>
	/// Merges state from another <see cref="ViewStateDictionary"/> (typically from deserialization)
	/// into this instance. Existing keys are overwritten.
	/// </summary>
	/// <param name="other">The source dictionary to merge from.</param>
	internal void LoadFrom(ViewStateDictionary other)
	{
		foreach (var kvp in other._store)
		{
			_store[kvp.Key] = kvp.Value;
		}
	}

	/// <summary>
	/// Coerces a stored value (which may be a <see cref="JsonElement"/> after deserialization)
	/// to the requested type <typeparamref name="T"/>.
	/// </summary>
	private static T CoerceValue<T>(object value)
	{
		if (value is T typed)
			return typed;

		if (value is JsonElement element)
			return element.Deserialize<T>()!;

		return (T)Convert.ChangeType(value, typeof(T));
	}

	#region IDictionary<string, object?> implementation

	/// <inheritdoc />
	public ICollection<string> Keys => _store.Keys;

	/// <inheritdoc />
	public ICollection<object?> Values => _store.Values;

	/// <inheritdoc />
	public int Count => _store.Count;

	/// <inheritdoc />
	public bool IsReadOnly => false;

	/// <inheritdoc />
	public void Add(string key, object? value)
	{
		_store.Add(key, value);
		IsDirty = true;
	}

	/// <inheritdoc />
	public bool ContainsKey(string key) => _store.ContainsKey(key);

	/// <inheritdoc />
	public bool Remove(string key)
	{
		var removed = _store.Remove(key);
		if (removed) IsDirty = true;
		return removed;
	}

	/// <inheritdoc />
	public bool TryGetValue(string key, out object? value) => _store.TryGetValue(key, out value);

	/// <inheritdoc />
	public void Add(KeyValuePair<string, object?> item)
	{
		((ICollection<KeyValuePair<string, object?>>)_store).Add(item);
		IsDirty = true;
	}

	/// <inheritdoc />
	public void Clear()
	{
		_store.Clear();
		IsDirty = true;
	}

	/// <inheritdoc />
	public bool Contains(KeyValuePair<string, object?> item)
		=> ((ICollection<KeyValuePair<string, object?>>)_store).Contains(item);

	/// <inheritdoc />
	public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
		=> ((ICollection<KeyValuePair<string, object?>>)_store).CopyTo(array, arrayIndex);

	/// <inheritdoc />
	public bool Remove(KeyValuePair<string, object?> item)
	{
		var removed = ((ICollection<KeyValuePair<string, object?>>)_store).Remove(item);
		if (removed) IsDirty = true;
		return removed;
	}

	/// <inheritdoc />
	public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => _store.GetEnumerator();

	/// <inheritdoc />
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	#endregion
}
