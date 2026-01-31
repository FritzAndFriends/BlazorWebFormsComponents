using System;

namespace BlazorWebFormsComponents;

/// <summary>
/// Provides page-level services that mimic ASP.NET Web Forms Page object functionality.
/// This service enables programmatic access to page properties like Title, similar to
/// how Page.Title worked in Web Forms.
/// </summary>
public interface IPageService
{
    /// <summary>
    /// Gets or sets the title of the page, which appears in the browser's title bar or tab.
    /// This is equivalent to Page.Title in Web Forms.
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// Event raised when the Title property changes.
    /// </summary>
    event EventHandler<string>? TitleChanged;
}
