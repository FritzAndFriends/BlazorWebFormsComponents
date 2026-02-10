using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents;

/// <summary>
/// Component that provides Web Forms-style Page object functionality.
/// Use this component to set the page title and meta tags programmatically,
/// similar to Page.Title, Page.MetaDescription, and Page.MetaKeywords in Web Forms.
/// </summary>
public partial class Page : ComponentBase, IDisposable
{
    [Inject]
    private IPageService PageService { get; set; } = null!;

    private string? _currentTitle;
    private string? _currentMetaDescription;
    private string? _currentMetaKeywords;

    protected override void OnInitialized()
    {
        PageService.TitleChanged += OnTitleChanged;
        PageService.MetaDescriptionChanged += OnMetaDescriptionChanged;
        PageService.MetaKeywordsChanged += OnMetaKeywordsChanged;

        _currentTitle = PageService.Title;
        _currentMetaDescription = PageService.MetaDescription;
        _currentMetaKeywords = PageService.MetaKeywords;
    }

    private async void OnTitleChanged(object? sender, string newTitle)
    {
        try
        {
            _currentTitle = newTitle;
            await InvokeAsync(StateHasChanged);
        }
        catch (ObjectDisposedException)
        {
            // Component was disposed before the state update completed.
            // This is expected when navigating away while an event is in flight.
        }
    }

    private async void OnMetaDescriptionChanged(object? sender, string newMetaDescription)
    {
        try
        {
            _currentMetaDescription = newMetaDescription;
            await InvokeAsync(StateHasChanged);
        }
        catch (ObjectDisposedException)
        {
            // Component was disposed before the state update completed.
        }
    }

    private async void OnMetaKeywordsChanged(object? sender, string newMetaKeywords)
    {
        try
        {
            _currentMetaKeywords = newMetaKeywords;
            await InvokeAsync(StateHasChanged);
        }
        catch (ObjectDisposedException)
        {
            // Component was disposed before the state update completed.
        }
    }

    public void Dispose()
    {
        PageService.TitleChanged -= OnTitleChanged;
        PageService.MetaDescriptionChanged -= OnMetaDescriptionChanged;
        PageService.MetaKeywordsChanged -= OnMetaKeywordsChanged;
    }
}
