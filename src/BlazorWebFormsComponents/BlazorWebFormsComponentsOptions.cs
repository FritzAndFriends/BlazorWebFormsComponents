using System;
using System.Collections.Generic;

namespace BlazorWebFormsComponents;

/// <summary>
/// Configuration options for BlazorWebFormsComponents middleware and services.
/// </summary>
public class BlazorWebFormsComponentsOptions
{
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
