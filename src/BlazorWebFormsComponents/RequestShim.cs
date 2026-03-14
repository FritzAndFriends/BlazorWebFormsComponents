using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace BlazorWebFormsComponents;

/// <summary>
/// Compatibility shim for ASP.NET Web Forms <c>Request</c> object.
/// Provides <c>Request.Cookies</c>, <c>Request.QueryString</c>, and
/// <c>Request.Url</c> with graceful degradation when <see cref="HttpContext"/>
/// is unavailable (interactive WebSocket rendering).
/// </summary>
public class RequestShim
{
	private readonly HttpContext? _httpContext;
	private readonly Microsoft.AspNetCore.Components.NavigationManager _nav;
	private readonly ILogger _logger;
	private bool _cookieWarned;

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
	/// Gets the query string parameters. Falls back to parsing the current
	/// URI from <see cref="Microsoft.AspNetCore.Components.NavigationManager"/>
	/// when <see cref="HttpContext"/> is unavailable.
	/// </summary>
	public IQueryCollection QueryString
	{
		get
		{
			if (_httpContext != null)
				return _httpContext.Request.Query;

			// Fallback: parse query string from NavigationManager.Uri
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
			if (_httpContext != null)
			{
				var req = _httpContext.Request;
				return new Uri($"{req.Scheme}://{req.Host}{req.PathBase}{req.Path}{req.QueryString}");
			}

			return new Uri(_nav.Uri);
		}
	}
}
