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
}
