using System;
using System.IO;
using BlazorWebFormsComponents.Diagnostics;
using BlazorWebFormsComponents.Theming;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
        services.AddDistributedMemoryCache();
        services.AddSession();
        services.AddScoped<BlazorWebFormsJsInterop>();
        services.AddScoped<IPageService, PageService>();
        services.AddScoped<SessionShim>();

        var options = new BlazorWebFormsComponentsOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);

        // Auto-discover Web Forms themes from wwwroot/App_Themes (or custom path).
        // Uses IWebHostEnvironment at resolution time so WebRootPath is available.
        services.AddSingleton<ThemeConfiguration>(sp =>
        {
            var env = sp.GetService<IWebHostEnvironment>();
            var themesRelPath = options.ThemesPath ?? "App_Themes";

            // Empty string means auto-discovery is explicitly disabled
            if (string.IsNullOrEmpty(themesRelPath) && options.ThemesPath is not null)
                return new ThemeConfiguration();

            var webRoot = env?.WebRootPath ?? "";
            if (string.IsNullOrEmpty(webRoot))
                return new ThemeConfiguration();

            var fullPath = Path.Combine(webRoot, themesRelPath);
            if (!Directory.Exists(fullPath))
                return new ThemeConfiguration();

            var config = SkinFileParser.ParseThemeFolder(fullPath);
            config.Mode = options.ThemeMode;

            // Auto-discover CSS files in the theme folder
            foreach (var css in Directory.GetFiles(fullPath, "*.css", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(webRoot, css).Replace('\\', '/');
                config.WithCssFile(relativePath);
            }

            return config;
        });

        return services;
    }

    /// <summary>
    /// Adds the Component Health Dashboard diagnostic service as a singleton.
    /// Loads reference baselines from dev-docs/reference-baselines.json and enables
    /// runtime reflection-based health scoring of all tracked components.
    /// Falls back to a pre-generated health-snapshot.json when the repository
    /// filesystem is not available (e.g., in Docker containers).
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="solutionRoot">Path to the repository root (for file scanning of tests, docs, samples).</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddComponentHealthDashboard(this IServiceCollection services, string solutionRoot)
    {
        var baselinesPath = System.IO.Path.Combine(solutionRoot, "dev-docs", "reference-baselines.json");
        var trackedPath = System.IO.Path.Combine(solutionRoot, "dev-docs", "tracked-components.json");

        // If the repo filesystem exists, use live reflection/file scanning
        if (System.IO.File.Exists(baselinesPath) || System.IO.File.Exists(trackedPath))
        {
            var baselines = ReferenceBaselines.LoadFromFile(baselinesPath);
            var healthService = new ComponentHealthService(baselines, solutionRoot);
            services.AddSingleton(healthService);
            return services;
        }

        // Fallback: look for a pre-generated snapshot alongside the running assembly
        var assemblyDir = System.IO.Path.GetDirectoryName(typeof(ComponentHealthService).Assembly.Location) ?? "";
        var snapshotFileName = HealthSnapshotGenerator.SnapshotFileName;
        var snapshotPath = System.IO.Path.Combine(assemblyDir,
            snapshotFileName.TrimStart(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar));
        var snapshotReports = HealthSnapshotGenerator.LoadSnapshot(snapshotPath);

        // Also check the app's base directory (common for published apps)
        if (snapshotReports == null)
        {
            snapshotPath = System.IO.Path.Combine(System.AppContext.BaseDirectory,
                snapshotFileName.TrimStart(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar));
            snapshotReports = HealthSnapshotGenerator.LoadSnapshot(snapshotPath);
        }

        if (snapshotReports != null)
        {
            var healthService = new ComponentHealthService(snapshotReports);
            services.AddSingleton(healthService);
        }
        else
        {
            // Last resort: use empty baselines (components will show partial health)
            var baselines = ReferenceBaselines.LoadFromFile(baselinesPath);
            var healthService = new ComponentHealthService(baselines, solutionRoot);
            services.AddSingleton(healthService);
        }

        return services;
    }

    /// <summary>
    /// Initializes the <see cref="ConfigurationManager"/> shim from the application's
    /// <see cref="IConfiguration"/>. Call this in Program.cs after building the app:
    /// <code>
    /// var app = builder.Build();
    /// app.UseConfigurationManagerShim();
    /// </code>
    /// This enables migrated Web Forms code to use
    /// <c>ConfigurationManager.AppSettings["key"]</c> and
    /// <c>ConfigurationManager.ConnectionStrings["name"]</c>.
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application for chaining</returns>
    public static WebApplication UseConfigurationManagerShim(this WebApplication app)
    {
        ConfigurationManager.Initialize(app.Configuration);
        return app;
    }

    /// <summary>
    /// Adds BlazorWebFormsComponents middleware to the application pipeline.
    /// Configurable via <see cref="BlazorWebFormsComponentsOptions"/>:
    /// <list type="bullet">
    ///   <item><description>.aspx URL rewriting (301 redirect to clean Blazor URLs)</description></item>
    ///   <item><description>.ashx handler interception (410 Gone or custom redirect)</description></item>
    ///   <item><description>.axd resource interception (404 Not Found; 410 Gone for ChartImg.axd)</description></item>
    /// </list>
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

        if (options.EnableAshxHandling)
        {
            app.UseMiddleware<AshxHandlerMiddleware>();
        }

        if (options.EnableAxdHandling)
        {
            app.UseMiddleware<AxdHandlerMiddleware>();
        }

        return app;
    }
}
