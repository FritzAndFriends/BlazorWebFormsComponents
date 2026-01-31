using System;

namespace BlazorWebFormsComponents;

/// <summary>
/// Default implementation of IPageService that provides page-level services
/// mimicking ASP.NET Web Forms Page object functionality.
/// </summary>
public class PageService : IPageService
{
    private string _title = string.Empty;

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
    /// Event raised when the Title property changes.
    /// </summary>
    public event EventHandler<string>? TitleChanged;
}
