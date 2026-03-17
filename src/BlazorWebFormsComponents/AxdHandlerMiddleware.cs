using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BlazorWebFormsComponents;

/// <summary>
/// Middleware that intercepts requests ending in .axd (legacy Web Resource handlers).
/// WebResource.axd, ScriptResource.axd, and Trace.axd return 404 Not Found.
/// ChartImg.axd returns 410 Gone.
/// All other .axd requests return 404 Not Found.
/// </summary>
internal class AxdHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public AxdHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.Value;

        if (path != null && path.EndsWith(".axd", StringComparison.OrdinalIgnoreCase))
        {
            // ChartImg.axd: the chart image handler is permanently gone
            if (path.EndsWith("/ChartImg.axd", StringComparison.OrdinalIgnoreCase)
                || path.Equals("/ChartImg.axd", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status410Gone;
                context.Response.ContentType = "text/plain";
                return context.Response.WriteAsync("ChartImg.axd is no longer available. The application has been migrated to Blazor.");
            }

            // WebResource.axd, ScriptResource.axd, Trace.axd, and all others: 404
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return Task.CompletedTask;
        }

        return _next(context);
    }
}
