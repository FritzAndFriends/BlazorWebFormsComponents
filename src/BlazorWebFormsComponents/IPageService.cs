using System;

namespace BlazorWebFormsComponents;

/// <summary>
/// Provides page-level services that mimic ASP.NET Web Forms Page object functionality.
/// This service enables programmatic access to page properties like Title, MetaDescription,
/// and MetaKeywords, similar to how the Page object worked in Web Forms.
/// </summary>
public interface IPageService
{
    /// <summary>
    /// Gets or sets the title of the page, which appears in the browser's title bar or tab.
    /// This is equivalent to Page.Title in Web Forms.
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// Gets or sets the meta description for the page.
    /// This is equivalent to Page.MetaDescription in Web Forms (.NET 4.0+).
    /// The description is used by search engines and appears in search results.
    /// </summary>
    string MetaDescription { get; set; }

    /// <summary>
    /// Gets or sets the meta keywords for the page.
    /// This is equivalent to Page.MetaKeywords in Web Forms (.NET 4.0+).
    /// Keywords help categorize the page content for search engines.
    /// </summary>
    string MetaKeywords { get; set; }

    /// <summary>
    /// Event raised when the Title property changes.
    /// </summary>
    event EventHandler<string>? TitleChanged;

    /// <summary>
    /// Event raised when the MetaDescription property changes.
    /// </summary>
    event EventHandler<string>? MetaDescriptionChanged;

    /// <summary>
    /// Event raised when the MetaKeywords property changes.
    /// </summary>
    event EventHandler<string>? MetaKeywordsChanged;
}
