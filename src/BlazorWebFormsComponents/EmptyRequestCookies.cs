using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace BlazorWebFormsComponents;

/// <summary>
/// A read-only empty <see cref="IRequestCookieCollection"/> returned when
/// <see cref="HttpContext"/> is unavailable (e.g., during interactive WebSocket
/// rendering). Returns <c>null</c> for all key lookups and logs a warning on
/// first access via the owning <see cref="RequestShim"/>.
/// </summary>
internal sealed class EmptyRequestCookies : IRequestCookieCollection
{
	public static readonly EmptyRequestCookies Instance = new();

	public string? this[string key] => null;
	public int Count => 0;
	public ICollection<string> Keys => Array.Empty<string>();
	public bool ContainsKey(string key) => false;

	public bool TryGetValue(string key, out string? value)
	{
		value = null;
		return false;
	}

	public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		=> Enumerable.Empty<KeyValuePair<string, string>>().GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
