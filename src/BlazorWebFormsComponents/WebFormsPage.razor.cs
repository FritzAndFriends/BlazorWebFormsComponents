using BlazorWebFormsComponents.Theming;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BlazorWebFormsComponents;

/// <summary>
/// Unified legacy Web Forms support wrapper. Combines NamingContainer (ID mangling),
/// ThemeProvider (skin/theme cascading), and Page head rendering (title + meta tags)
/// into a single component that mirrors System.Web.UI.Page — the root of every
/// Web Forms page.
///
/// Place in MainLayout.razor wrapping @Body to give all pages naming scope, theming,
/// and automatic page title/meta rendering, or use per-page for area-specific configuration.
/// </summary>
public partial class WebFormsPage : NamingContainer, IDisposable
{
	/// <summary>
	/// Optional theme configuration to cascade to all child components.
	/// When null, theming is effectively disabled (child components skip theme application).
	/// </summary>
	[Parameter]
	public ThemeConfiguration Theme { get; set; }

	/// <summary>
	/// When true (the default), renders &lt;PageTitle&gt; and &lt;HeadContent&gt; based on
	/// IPageService values. Set to false if head rendering is handled separately
	/// (e.g., via a standalone &lt;Page /&gt; component).
	/// </summary>
	[Parameter]
	public bool RenderPageHead { get; set; } = true;

	[Inject]
	private IServiceProvider ServiceProvider { get; set; } = null!;

	private IPageService? _pageService;
	private bool _pageServiceAvailable;
	private string? _currentTitle;
	private string? _currentMetaDescription;
	private string? _currentMetaKeywords;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		_pageService = ServiceProvider.GetService<IPageService>();
		_pageServiceAvailable = _pageService is not null;

		if (_pageService is not null)
		{
			_pageService.TitleChanged += OnTitleChanged;
			_pageService.MetaDescriptionChanged += OnMetaDescriptionChanged;
			_pageService.MetaKeywordsChanged += OnMetaKeywordsChanged;

			_currentTitle = _pageService.Title;
			_currentMetaDescription = _pageService.MetaDescription;
			_currentMetaKeywords = _pageService.MetaKeywords;
		}
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
		if (_pageService is not null)
		{
			_pageService.TitleChanged -= OnTitleChanged;
			_pageService.MetaDescriptionChanged -= OnMetaDescriptionChanged;
			_pageService.MetaKeywordsChanged -= OnMetaKeywordsChanged;
		}
	}
}
