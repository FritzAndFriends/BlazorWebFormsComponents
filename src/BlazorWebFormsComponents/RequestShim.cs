using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace BlazorWebFormsComponents;

/// <summary>
/// Compatibility shim for ASP.NET Web Forms <c>Request</c> object.
/// Provides <c>Request.Cookies</c>, <c>Request.Form</c>,
/// <c>Request.QueryString</c>, and <c>Request.Url</c> with graceful
/// degradation when <see cref="HttpContext"/> is unavailable (interactive
/// WebSocket rendering).
/// </summary>
public class RequestShim
{
	private readonly HttpContext? _httpContext;
	private readonly Microsoft.AspNetCore.Components.NavigationManager _nav;
	private readonly ILogger _logger;
	private bool _cookieWarned;
	private bool _formWarned;
	private FormShim? _cachedFormShim;

	internal RequestShim(
		HttpContext? httpContext,
		Microsoft.AspNetCore.Components.NavigationManager nav,
		ILogger logger)
	{
		_httpContext = httpContext;
		_nav = nav;
		_logger = logger;
	}

	/// <summary>
	/// Gets the request cookies. When <see cref="HttpContext"/> is unavailable
	/// (interactive rendering), returns an empty collection and logs a warning
	/// on first access.
	/// </summary>
	public IRequestCookieCollection Cookies
	{
		get
		{
			if (_httpContext != null)
				return _httpContext.Request.Cookies;

			if (!_cookieWarned)
			{
				_logger.LogWarning(
					"Request.Cookies accessed without HttpContext (interactive render mode). " +
					"Returning empty collection. Cookie-dependent logic will not function.");
				_cookieWarned = true;
			}

			return EmptyRequestCookies.Instance;
		}
	}

	/// <summary>
	/// Gets the form POST data. When <see cref="HttpContext"/> is unavailable
	/// (interactive rendering), returns an empty <see cref="FormShim"/> and logs
	/// a warning on first access. Also returns empty when the request body is
	/// not form-encoded (e.g., JSON payloads).
	/// </summary>
	public FormShim Form
	{
		get
		{
			// If SetFormData() was called (WebFormsForm submit), return the cached
			// interop-backed shim even when HttpContext is available (Blazor Server
			// SignalR circuits always have an HttpContext, but its Request.Form is
			// from the original WebSocket upgrade, not the interactive form submit).
			if (_cachedFormShim != null)
				return _cachedFormShim;

			if (_httpContext != null)
			{
				try
				{
					return new FormShim(_httpContext.Request.Form);
				}
				catch (InvalidOperationException)
				{
					// Request body is not form-encoded (e.g., JSON or empty).
					return new FormShim((IFormCollection?)null);
				}
			}

			// Interactive mode without prior SetFormData call
			if (!_formWarned)
			{
				_logger.LogWarning(
					"Request.Form accessed without HttpContext (interactive render mode). " +
					"Use <WebFormsForm> component to enable form data in interactive mode.");
				_formWarned = true;
			}

			_cachedFormShim = new FormShim((IFormCollection?)null);
			return _cachedFormShim;
		}
	}

	/// <summary>
	/// Gets the query string parameters. Falls back to parsing the current
	/// URI from <see cref="Microsoft.AspNetCore.Components.NavigationManager"/>
	/// when <see cref="HttpContext"/> is unavailable.
	/// </summary>
	public IQueryCollection QueryString
	{
		get
		{
			// Always parse from NavigationManager — it has the correct page URL
			// with query parameters. HttpContext.Request.Query in interactive mode
			// returns the SignalR connection's query params, not the page's.
			var uri = new Uri(_nav.Uri);
			var parsed = QueryHelpers.ParseQuery(uri.Query);
			return new QueryCollection(parsed);
		}
	}

	/// <summary>
	/// Gets the request URL. Falls back to
	/// <see cref="Microsoft.AspNetCore.Components.NavigationManager.Uri"/>
	/// when <see cref="HttpContext"/> is unavailable.
	/// </summary>
	public Uri Url
	{
		get
		{
			// Always prefer NavigationManager — it has the correct page URL.
			// HttpContext.Request in Blazor Server interactive mode shows the
			// SignalR connection URL (/_blazor), not the page URL.
			return new Uri(_nav.Uri);
		}
	}

	/// <summary>
	/// Updates the cached <see cref="FormShim"/> with form data captured via
	/// JS interop. Called by <see cref="WebFormsForm"/> during interactive
	/// mode form submissions.
	/// </summary>
	/// <param name="formData">Dictionary of field names to values.</param>
	internal void SetFormData(Dictionary<string, StringValues> formData)
	{
		if (_cachedFormShim == null)
			_cachedFormShim = new FormShim(formData);
		else
			_cachedFormShim.SetFormData(formData);
	}
}
