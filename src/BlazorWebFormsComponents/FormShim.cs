using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace BlazorWebFormsComponents;

/// <summary>
/// Compatibility shim for ASP.NET Web Forms <c>Request.Form</c>
/// (<see cref="System.Collections.Specialized.NameValueCollection"/>).
/// Wraps <see cref="IFormCollection"/> and provides the subset of
/// <c>NameValueCollection</c> members commonly used in Web Forms code-behind.
/// When <see cref="IFormCollection"/> is unavailable (interactive rendering or
/// non-form-encoded requests), all members return empty/null/0 — no exceptions.
/// </summary>
public class FormShim
{
	private readonly IFormCollection? _form;

	/// <summary>
	/// Initializes a new <see cref="FormShim"/> wrapping the specified form collection.
	/// </summary>
	/// <param name="form">
	/// The underlying form collection, or <c>null</c> when <see cref="HttpContext"/>
	/// is unavailable or the request body is not form-encoded.
	/// </param>
	internal FormShim(IFormCollection? form)
	{
		_form = form;
	}

	/// <summary>
	/// Gets the first value associated with the specified form field name,
	/// or <c>null</c> if the field does not exist or the form is unavailable.
	/// </summary>
	/// <param name="key">The form field name.</param>
	/// <returns>The first value for <paramref name="key"/>, or <c>null</c>.</returns>
	public string? this[string key]
	{
		get
		{
			if (_form == null)
				return null;

			return _form.TryGetValue(key, out var values) ? values.FirstOrDefault() : null;
		}
	}

	/// <summary>
	/// Gets all values associated with the specified form field name.
	/// Returns <c>null</c> if the field does not exist or the form is unavailable.
	/// Useful for multi-value fields such as checkboxes or multi-select lists.
	/// </summary>
	/// <param name="key">The form field name.</param>
	/// <returns>An array of values, or <c>null</c> if the key is not present.</returns>
	public string[]? GetValues(string key)
	{
		if (_form == null)
			return null;

		if (!_form.TryGetValue(key, out var values))
			return null;

		var result = values.ToArray();
		return result.Length > 0 ? result : null;
	}

	/// <summary>
	/// Gets the names of all form fields submitted in the request.
	/// Returns an empty array when the form is unavailable.
	/// </summary>
	public string[] AllKeys
		=> _form?.Keys.ToArray() ?? Array.Empty<string>();

	/// <summary>
	/// Gets the number of form fields submitted in the request.
	/// Returns <c>0</c> when the form is unavailable.
	/// </summary>
	public int Count
		=> _form?.Count ?? 0;

	/// <summary>
	/// Determines whether the form contains a field with the specified name.
	/// Returns <c>false</c> when the form is unavailable.
	/// </summary>
	/// <param name="key">The form field name to check.</param>
	/// <returns><c>true</c> if the field exists; otherwise, <c>false</c>.</returns>
	public bool ContainsKey(string key)
		=> _form?.ContainsKey(key) ?? false;
}
