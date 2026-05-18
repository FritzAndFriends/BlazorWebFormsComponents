using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BlazorWebFormsComponents;

/// <summary>
/// Compatibility shim for ASP.NET Web Forms <c>Response</c> object.
/// Wraps <see cref="NavigationManager"/> for <c>Response.Redirect()</c> and
/// provides <c>Response.Cookies</c> with graceful degradation when
/// <see cref="HttpContext"/> is unavailable (interactive WebSocket rendering).
/// </summary>
public class ResponseShim
{
	private readonly NavigationManager _nav;
	private readonly HttpContext? _httpContext;
	private readonly ILogger _logger;
	private bool _cookieWarned;

	internal ResponseShim(NavigationManager nav, HttpContext? httpContext, ILogger logger)
	{
		_nav = nav;
		_httpContext = httpContext;
		_logger = logger;
	}

	/// <summary>
	/// Gets the response cookies. When <see cref="HttpContext"/> is unavailable
	/// (interactive rendering), returns a no-op implementation that silently
	/// discards cookie operations and logs a warning on first access.
	/// </summary>
	public IResponseCookies Cookies
	{
		get
		{
			if (_httpContext != null)
				return _httpContext.Response.Cookies;

			if (!_cookieWarned)
			{
				_logger.LogWarning(
					"Response.Cookies accessed without HttpContext (interactive render mode). " +
					"Cookie operations will be silently discarded.");
				_cookieWarned = true;
			}

			return NullResponseCookies.Instance;
		}
	}

	/// <summary>
	/// Navigates to the specified URL, stripping Web Forms virtual path
	/// prefix (<c>~/</c>) and <c>.aspx</c> extensions automatically.
	/// </summary>
	/// <param name="url">The target URL. May contain <c>~/</c> prefix and/or <c>.aspx</c> extension.</param>
	/// <param name="endResponse">
	/// Ignored in Blazor. Exists for API compatibility with
	/// <c>HttpResponse.Redirect(string, bool)</c>.
	/// </param>
	public void Redirect(string url, bool endResponse = true)
	{
		ArgumentNullException.ThrowIfNull(url);
		if (url.StartsWith("~/")) url = url[1..]; // ~/path → /path
		if (url.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
			url = url[..^5]; // /path.aspx → /path
		_nav.NavigateTo(url, forceLoad: true);
	}
}
