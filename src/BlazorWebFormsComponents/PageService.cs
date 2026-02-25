using System;

namespace BlazorWebFormsComponents;

/// <summary>
/// Default implementation of IPageService that provides page-level services
/// mimicking ASP.NET Web Forms Page object functionality.
/// </summary>
public class PageService : IPageService
{
    private string _title = string.Empty;
    private string _metaDescription = string.Empty;
    private string _metaKeywords = string.Empty;

    /// <summary>
    /// Gets or sets the title of the page, which appears in the browser's title bar or tab.
    /// This is equivalent to Page.Title in Web Forms.
    /// </summary>
    public string Title
    {
        get => _title;
        set
        {
            if (_title != value)
            {
                _title = value;
                TitleChanged?.Invoke(this, _title);
            }
        }
    }

    /// <summary>
    /// Gets or sets the meta description for the page.
    /// This is equivalent to Page.MetaDescription in Web Forms (.NET 4.0+).
    /// The description is used by search engines and appears in search results.
    /// </summary>
    public string MetaDescription
    {
        get => _metaDescription;
        set
        {
            if (_metaDescription != value)
            {
                _metaDescription = value;
                MetaDescriptionChanged?.Invoke(this, _metaDescription);
            }
        }
    }

    /// <summary>
    /// Gets or sets the meta keywords for the page.
    /// This is equivalent to Page.MetaKeywords in Web Forms (.NET 4.0+).
    /// Keywords help categorize the page content for search engines.
    /// </summary>
    public string MetaKeywords
    {
        get => _metaKeywords;
        set
        {
            if (_metaKeywords != value)
            {
                _metaKeywords = value;
                MetaKeywordsChanged?.Invoke(this, _metaKeywords);
            }
        }
    }

    /// <summary>
    /// Event raised when the Title property changes.
    /// </summary>
    public event EventHandler<string>? TitleChanged;

    /// <summary>
    /// Event raised when the MetaDescription property changes.
    /// </summary>
    public event EventHandler<string>? MetaDescriptionChanged;

    /// <summary>
    /// Event raised when the MetaKeywords property changes.
    /// </summary>
    public event EventHandler<string>? MetaKeywordsChanged;
}
