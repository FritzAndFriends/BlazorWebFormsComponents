using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace BlazorWebFormsComponents;

/// <summary>
/// Compatibility shim for ASP.NET Web Forms <c>Request.Form</c>
/// (<see cref="System.Collections.Specialized.NameValueCollection"/>).
/// Wraps <see cref="IFormCollection"/> and provides the subset of
/// <c>NameValueCollection</c> members commonly used in Web Forms code-behind.
/// In SSR mode, delegates to <see cref="IFormCollection"/>.
/// In interactive mode, uses form data captured via JS interop.
/// When neither source is available, all members return empty/null/0 — no exceptions.
/// </summary>
public class FormShim
{
	private readonly IFormCollection? _form;
	private Dictionary<string, StringValues>? _interopData;

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
	/// Initializes a new <see cref="FormShim"/> backed by JS interop form data.
	/// Used in interactive Blazor Server mode where <see cref="IFormCollection"/>
	/// is unavailable.
	/// </summary>
	/// <param name="interopData">Form field data captured via JavaScript interop.</param>
	internal FormShim(Dictionary<string, StringValues> interopData)
	{
		_form = null;
		_interopData = interopData;
	}

	/// <summary>
	/// Replaces the interop-sourced form data. Called by <see cref="RequestShim"/>
	/// when a <c>&lt;WebFormsForm&gt;</c> submit is processed.
	/// </summary>
	internal void SetFormData(Dictionary<string, StringValues> data)
	{
		_interopData = data;
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
			if (_form != null)
				return _form.TryGetValue(key, out var values) ? values.FirstOrDefault() : null;

			if (_interopData != null)
				return _interopData.TryGetValue(key, out var values) ? values.FirstOrDefault() : null;

			return null;
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
		string[]? result = null;

		if (_form != null)
		{
			if (!_form.TryGetValue(key, out var values))
				return null;
			result = values.ToArray();
		}
		else if (_interopData != null)
		{
			if (!_interopData.TryGetValue(key, out var values))
				return null;
			result = values.ToArray();
		}

		return result is { Length: > 0 } ? result : null;
	}

	/// <summary>
	/// Gets the names of all form fields submitted in the request.
	/// Returns an empty array when the form is unavailable.
	/// </summary>
	public string[] AllKeys
	{
		get
		{
			if (_form != null)
				return _form.Keys.ToArray();
			if (_interopData != null)
				return _interopData.Keys.ToArray();
			return Array.Empty<string>();
		}
	}

	/// <summary>
	/// Gets the number of form fields submitted in the request.
	/// Returns <c>0</c> when the form is unavailable.
	/// </summary>
	public int Count
	{
		get
		{
			if (_form != null)
				return _form.Count;
			if (_interopData != null)
				return _interopData.Count;
			return 0;
		}
	}

	/// <summary>
	/// Determines whether the form contains a field with the specified name.
	/// Returns <c>false</c> when the form is unavailable.
	/// </summary>
	/// <param name="key">The form field name to check.</param>
	/// <returns><c>true</c> if the field exists; otherwise, <c>false</c>.</returns>
	public bool ContainsKey(string key)
	{
		if (_form != null)
			return _form.ContainsKey(key);
		if (_interopData != null)
			return _interopData.ContainsKey(key);
		return false;
	}
}
