using System;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents;

/// <summary>
/// Compatibility shim for ASP.NET Web Forms <c>Response.Redirect()</c>.
/// Wraps <see cref="NavigationManager"/> so that migrated code-behind using
/// <c>Response.Redirect("~/Products.aspx")</c> compiles and navigates correctly.
/// </summary>
public class ResponseShim
{
	private readonly NavigationManager _nav;

	internal ResponseShim(NavigationManager nav) => _nav = nav;

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
		if (url.StartsWith("~/")) url = url[1..]; // ~/path → /path
		if (url.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
			url = url[..^5]; // /path.aspx → /path
		_nav.NavigateTo(url, forceLoad: false);
	}
}
