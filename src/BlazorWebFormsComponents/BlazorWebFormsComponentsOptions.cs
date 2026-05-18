using System;
using System.Collections.Generic;
using BlazorWebFormsComponents.Theming;

namespace BlazorWebFormsComponents;

/// <summary>
/// Configuration options for BlazorWebFormsComponents middleware and services.
/// </summary>
public class BlazorWebFormsComponentsOptions
{
    /// <summary>
    /// Path to the themes folder relative to wwwroot. When null (default),
    /// auto-discovery looks for "App_Themes" under wwwroot. Set to a custom
    /// path to override, or set to empty string to disable auto-discovery.
    /// </summary>
    public string? ThemesPath { get; set; }

    /// <summary>
    /// Controls how auto-discovered theme skins interact with explicit property values.
    /// Default is StyleSheetTheme (theme sets defaults, explicit values win).
    /// </summary>
    public ThemeMode ThemeMode { get; set; } = ThemeMode.StyleSheetTheme;

    /// <summary>
    /// When true, registers middleware to rewrite .aspx URLs to their Blazor equivalents.
    /// Default: true
    /// </summary>
    public bool EnableAspxUrlRewriting { get; set; } = true;

    /// <summary>
    /// When true, registers middleware to intercept .ashx (HTTP handler) requests.
    /// By default, .ashx requests return 410 Gone. Custom redirect mappings can be
    /// registered via <see cref="AshxRedirectMappings"/>.
    /// Default: true
    /// </summary>
    public bool EnableAshxHandling { get; set; } = true;

    /// <summary>
    /// When true, registers middleware to intercept .axd (Web Resource) requests.
    /// WebResource.axd, ScriptResource.axd, and Trace.axd return 404.
    /// ChartImg.axd returns 410 Gone.
    /// Default: true
    /// </summary>
    public bool EnableAxdHandling { get; set; } = true;

    /// <summary>
    /// Optional redirect mappings for .ashx handlers. Keys are request paths
    /// (e.g., "/MyHandler.ashx") and values are target URLs to 301-redirect to.
    /// Paths not in this dictionary return 410 Gone.
    /// Comparisons are case-insensitive.
    /// </summary>
    public Dictionary<string, string> AshxRedirectMappings { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
