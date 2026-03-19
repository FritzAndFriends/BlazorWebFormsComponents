using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorWebFormsComponents.AspxMiddleware;

/// <summary>
/// Extension methods for configuring the ASPX rendering middleware.
/// </summary>
public static class AspxMiddlewareExtensions
{
    /// <summary>
    /// Register services required by the ASPX rendering middleware.
    /// </summary>
    public static IServiceCollection AddAspxMiddleware(this IServiceCollection services)
    {
        services.AddScoped<HtmlRenderer>();
        services.AddLogging();
        return services;
    }

    /// <summary>
    /// Register the ASPX rendering middleware to intercept .aspx requests
    /// and render them using BWFC Blazor components via SSR.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="aspxRootPath">
    /// Root directory containing .aspx files. Can be absolute or relative to content root.
    /// </param>
    public static IApplicationBuilder UseAspxPages(this IApplicationBuilder app, string aspxRootPath)
    {
        // Resolve to absolute path if relative
        var resolvedPath = Path.IsPathRooted(aspxRootPath)
            ? aspxRootPath
            : Path.Combine(Directory.GetCurrentDirectory(), aspxRootPath);

        app.UseMiddleware<AspxRenderingMiddleware>(resolvedPath);
        return app;
    }
}
