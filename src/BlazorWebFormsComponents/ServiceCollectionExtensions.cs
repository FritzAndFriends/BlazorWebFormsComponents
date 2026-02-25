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
        services.AddScoped<BlazorWebFormsJsInterop>();
        services.AddScoped<IPageService, PageService>();
        return services;
    }
}
