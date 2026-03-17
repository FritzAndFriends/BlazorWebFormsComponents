using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BlazorWebFormsComponents;

/// <summary>
/// Middleware that intercepts requests ending in .ashx (legacy HTTP handlers).
/// If a custom redirect mapping exists, issues a 301 redirect; otherwise returns 410 Gone.
/// </summary>
internal class AshxHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly BlazorWebFormsComponentsOptions _options;

    public AshxHandlerMiddleware(RequestDelegate next, BlazorWebFormsComponentsOptions options)
    {
        _next = next;
        _options = options;
    }

    public Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.Value;

        if (path != null && path.EndsWith(".ashx", StringComparison.OrdinalIgnoreCase))
        {
            if (_options.AshxRedirectMappings.TryGetValue(path, out var target))
            {
                var queryString = context.Request.QueryString.Value;
                context.Response.StatusCode = StatusCodes.Status301MovedPermanently;
                context.Response.Headers.Location = target + queryString;
                return Task.CompletedTask;
            }

            context.Response.StatusCode = StatusCodes.Status410Gone;
            context.Response.ContentType = "text/plain";
            return context.Response.WriteAsync("This HTTP handler (.ashx) is no longer available. The application has been migrated to Blazor.");
        }

        return _next(context);
    }
}
