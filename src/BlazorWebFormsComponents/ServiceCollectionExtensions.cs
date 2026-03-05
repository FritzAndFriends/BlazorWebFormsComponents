using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorWebFormsComponents;

/// <summary>
/// Extension methods for registering BlazorWebFormsComponents services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds BlazorWebFormsComponents services to the service collection.
    /// This automatically handles JavaScript loading - no manual script tags required.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddBlazorWebFormsComponents(this IServiceCollection services)
    {
        // Register IHttpContextAccessor via reflection — the consuming ASP.NET Core app
        // always has Microsoft.AspNetCore.Http loaded, but this library can't reference
        // it at compile time (no shared framework reference in the Razor class library).
        var httpExtType = Type.GetType(
            "Microsoft.Extensions.DependencyInjection.HttpServiceCollectionExtensions, Microsoft.AspNetCore.Http");
        httpExtType?.GetMethod("AddHttpContextAccessor", BindingFlags.Public | BindingFlags.Static,
            null, new[] { typeof(IServiceCollection) }, null)?.Invoke(null, new object[] { services });

        services.AddScoped<BlazorWebFormsJsInterop>();
        services.AddScoped<IPageService, PageService>();
        return services;
    }
}
