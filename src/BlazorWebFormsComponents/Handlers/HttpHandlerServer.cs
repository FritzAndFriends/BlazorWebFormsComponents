using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Hosting;

namespace BlazorWebFormsComponents;

/// <summary>
/// Adapter that provides Web Forms <c>HttpServerUtility</c>-compatible methods
/// using ASP.NET Core's <see cref="IWebHostEnvironment"/> and BCL utilities.
/// Supports <see cref="MapPath"/>, HTML/URL encoding, and provides migration
/// guidance for unsupported operations like <c>Server.Transfer</c>.
/// </summary>
public class HttpHandlerServer
{
	private readonly IWebHostEnvironment _env;

	internal HttpHandlerServer(IWebHostEnvironment environment)
	{
		_env = environment;
	}

	/// <summary>
	/// Maps a virtual path to a physical file path on the server.
	/// Paths starting with <c>~/</c> are resolved relative to <c>WebRootPath</c> (wwwroot).
	/// All other paths are resolved relative to <c>ContentRootPath</c>.
	/// Equivalent to <c>HttpServerUtility.MapPath</c> in Web Forms.
	/// </summary>
	/// <param name="virtualPath">
	/// The virtual path to map. Use <c>~/</c> prefix for web root-relative paths.
	/// </param>
	/// <returns>The physical file path.</returns>
	public string MapPath(string virtualPath)
	{
		if (virtualPath.StartsWith("~/"))
		{
			return Path.Combine(
				_env.WebRootPath,
				virtualPath[2..].Replace('/', Path.DirectorySeparatorChar));
		}

		return Path.Combine(
			_env.ContentRootPath,
			virtualPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
	}

	/// <summary>
	/// HTML-encodes a string. Equivalent to <c>HttpServerUtility.HtmlEncode</c> in Web Forms.
	/// </summary>
	/// <param name="text">The string to encode.</param>
	/// <returns>The HTML-encoded string.</returns>
	public string HtmlEncode(string text) => WebUtility.HtmlEncode(text);

	/// <summary>
	/// HTML-decodes a string. Equivalent to <c>HttpServerUtility.HtmlDecode</c> in Web Forms.
	/// </summary>
	/// <param name="text">The string to decode.</param>
	/// <returns>The decoded string.</returns>
	public string HtmlDecode(string text) => WebUtility.HtmlDecode(text);

	/// <summary>
	/// URL-encodes a string. Equivalent to <c>HttpServerUtility.UrlEncode</c> in Web Forms.
	/// </summary>
	/// <param name="text">The string to encode.</param>
	/// <returns>The URL-encoded string.</returns>
	public string UrlEncode(string text) => WebUtility.UrlEncode(text);

	/// <summary>
	/// URL-decodes a string. Equivalent to <c>HttpServerUtility.UrlDecode</c> in Web Forms.
	/// </summary>
	/// <param name="text">The string to decode.</param>
	/// <returns>The decoded string.</returns>
	public string UrlDecode(string text) => WebUtility.UrlDecode(text);

	/// <summary>
	/// Not supported in ASP.NET Core. Throws <see cref="NotSupportedException"/> with
	/// migration guidance. In Web Forms, <c>Server.Transfer</c> executes another handler
	/// within the same request — this has no equivalent in ASP.NET Core.
	/// </summary>
	/// <param name="path">The target path (unused).</param>
	/// <exception cref="NotSupportedException">Always thrown with migration guidance.</exception>
	public string Transfer(string path)
	{
		throw new NotSupportedException(
			$"Server.Transfer(\"{path}\") is not supported in ASP.NET Core. " +
			"Server.Transfer executes another handler within the same request, which has " +
			"no equivalent in the ASP.NET Core pipeline. Migrate to one of these alternatives: " +
			"(1) Response.Redirect for client-side redirect, " +
			"(2) Extract shared logic into a service class and call it directly, or " +
			"(3) Use HttpClient to call the target endpoint internally.");
	}
}
