using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BlazorWebFormsComponents;

/// <summary>
/// Middleware that intercepts requests ending in .aspx and issues a 301 permanent
/// redirect to the clean Blazor-equivalent URL (without the .aspx extension).
/// Handles Default.aspx specially by redirecting to the parent directory path.
/// </summary>
internal class AspxRewriteMiddleware
{
    private readonly RequestDelegate _next;

    public AspxRewriteMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.Value;

        if (path != null && path.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
        {
            // Strip the .aspx extension
            var newPath = path[..^5];

            // Handle Default.aspx → parent directory (e.g., /Default.aspx → /, /Admin/Default.aspx → /Admin)
            if (newPath.Equals("/Default", StringComparison.OrdinalIgnoreCase))
            {
                newPath = "/";
            }
            else if (newPath.EndsWith("/Default", StringComparison.OrdinalIgnoreCase))
            {
                newPath = newPath[..^"/Default".Length];
                if (string.IsNullOrEmpty(newPath)) newPath = "/";
            }
            else if (string.IsNullOrEmpty(newPath))
            {
                newPath = "/";
            }

            // Preserve query string
            var queryString = context.Request.QueryString.Value;
            context.Response.StatusCode = StatusCodes.Status301MovedPermanently;
            context.Response.Headers.Location = newPath + queryString;
            return Task.CompletedTask;
        }

        return _next(context);
    }
}
