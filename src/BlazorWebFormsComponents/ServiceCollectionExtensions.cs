using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorWebFormsComponents;

/// <summary>
/// Extension methods for registering and configuring BlazorWebFormsComponents.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds BlazorWebFormsComponents services to the service collection with default options.
    /// This automatically handles JavaScript loading - no manual script tags required.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddBlazorWebFormsComponents(this IServiceCollection services)
        => services.AddBlazorWebFormsComponents(configure: null);

    /// <summary>
    /// Adds BlazorWebFormsComponents services to the service collection with configurable options.
    /// This automatically registers HttpContextAccessor (required by BaseWebFormsComponent),
    /// JS interop services, and the page service.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Optional action to configure <see cref="BlazorWebFormsComponentsOptions"/></param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddBlazorWebFormsComponents(this IServiceCollection services, Action<BlazorWebFormsComponentsOptions>? configure)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<BlazorWebFormsJsInterop>();
        services.AddScoped<IPageService, PageService>();

        var options = new BlazorWebFormsComponentsOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);

        return services;
    }

    /// <summary>
    /// Adds BlazorWebFormsComponents middleware to the application pipeline.
    /// When <see cref="BlazorWebFormsComponentsOptions.EnableAspxUrlRewriting"/> is true (default),
    /// requests ending in .aspx are permanently redirected (301) to clean Blazor URLs.
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    public static IApplicationBuilder UseBlazorWebFormsComponents(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetService<BlazorWebFormsComponentsOptions>()
                     ?? new BlazorWebFormsComponentsOptions();

        if (options.EnableAspxUrlRewriting)
        {
            app.UseMiddleware<AspxRewriteMiddleware>();
        }

        return app;
    }
}
