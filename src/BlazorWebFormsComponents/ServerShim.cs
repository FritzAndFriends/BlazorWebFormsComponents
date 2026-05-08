using System;
using System.IO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;

namespace BlazorWebFormsComponents;

/// <summary>
/// Compatibility shim for Web Forms <c>Server</c> (HttpServerUtility).
/// Provides <c>Server.MapPath()</c>, encoding helpers, and light-weight
/// compatibility methods for <c>Transfer()</c>, <c>GetLastError()</c>, and
/// <c>ClearError()</c>.
/// </summary>
public class ServerShim
{
    private readonly IWebHostEnvironment _env;
    private readonly NavigationManager? _navigationManager;

    public ServerShim(IWebHostEnvironment env, NavigationManager? navigationManager = null)
    {
        _env = env;
        _navigationManager = navigationManager;
    }

    /// <summary>
    /// Maps a virtual path to a physical path on the server.
    /// <c>~/</c> prefix maps to WebRootPath (wwwroot).
    /// Other paths map relative to ContentRootPath.
    /// </summary>
    public string MapPath(string virtualPath)
    {
        if (string.IsNullOrEmpty(virtualPath))
            return _env.ContentRootPath;

        if (virtualPath.StartsWith("~/", StringComparison.Ordinal))
            return Path.Combine(_env.WebRootPath ?? _env.ContentRootPath,
                virtualPath[2..].Replace('/', Path.DirectorySeparatorChar));

        return Path.Combine(_env.ContentRootPath,
            virtualPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
    }

    /// <summary>HTML-encodes a string.</summary>
    public string HtmlEncode(string text) => System.Net.WebUtility.HtmlEncode(text);

    /// <summary>HTML-decodes a string.</summary>
    public string HtmlDecode(string text) => System.Net.WebUtility.HtmlDecode(text);

    /// <summary>URL-encodes a string.</summary>
    public string UrlEncode(string text) => System.Net.WebUtility.UrlEncode(text);

    /// <summary>URL-decodes a string.</summary>
    public string UrlDecode(string text) => System.Net.WebUtility.UrlDecode(text);

    /// <summary>
    /// Compatibility stub for <c>Server.GetLastError()</c>.
    /// Returns <see langword="null"/> in Blazor because exception state is owned by middleware.
    /// </summary>
    public Exception? GetLastError() => null;

    /// <summary>
    /// Compatibility stub for <c>Server.ClearError()</c>.
    /// Error clearing is handled by ASP.NET Core middleware, so this is a no-op.
    /// </summary>
    public void ClearError()
    {
    }

    /// <summary>
    /// Compatibility implementation for <c>Server.Transfer(path)</c>.
    /// Delegates to <see cref="NavigationManager.NavigateTo(string, bool, bool)"/>.
    /// </summary>
    public void Transfer(string path)
    {
        if (_navigationManager is null)
            throw new InvalidOperationException("NavigationManager is required for Server.Transfer().");

        _navigationManager.NavigateTo(path);
    }
}
