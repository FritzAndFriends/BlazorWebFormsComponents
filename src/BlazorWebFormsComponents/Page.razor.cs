namespace BlazorWebFormsComponents;

/// <summary>
/// Head-rendering companion for <see cref="WebFormsPageBase"/>.
/// Renders the current title and meta tags from the same page shim surface used by migrated page code-behind.
/// </summary>
public partial class Page : WebFormsPageBase
{
    protected override bool EnablePostBackInterop => false;
}
