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
        _currentTitle = newTitle;
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        PageService.TitleChanged -= OnTitleChanged;
    }
}
