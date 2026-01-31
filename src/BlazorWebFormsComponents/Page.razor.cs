using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents;

/// <summary>
/// Component that provides Web Forms-style Page object functionality.
/// Use this component to set the page title programmatically, similar to Page.Title in Web Forms.
/// </summary>
public partial class Page : ComponentBase, IDisposable
{
    [Inject]
    private IPageService PageService { get; set; } = null!;

    private string? _currentTitle;

    protected override void OnInitialized()
    {
        PageService.TitleChanged += OnTitleChanged;
        _currentTitle = PageService.Title;
    }

    private async void OnTitleChanged(object? sender, string newTitle)
    {
        try
        {
            _currentTitle = newTitle;
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception)
        {
            // Silently handle exceptions from title updates to prevent
            // breaking the component lifecycle. Title updates are non-critical.
            // In production, consider logging this exception.
        }
    }

    public void Dispose()
    {
        PageService.TitleChanged -= OnTitleChanged;
    }
}
