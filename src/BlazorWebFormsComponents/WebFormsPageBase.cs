using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents;

/// <summary>
/// Base class for converted ASP.NET Web Forms pages. Provides Page.Title,
/// Page.MetaDescription, Page.MetaKeywords, and IsPostBack compatibility
/// so that Web Forms code-behind patterns survive migration with minimal changes.
/// </summary>
public abstract class WebFormsPageBase : ComponentBase
{
	[Inject] private IPageService _pageService { get; set; } = null!;

	/// <summary>
	/// Gets or sets the title of the page. Delegates to IPageService.
	/// Equivalent to Page.Title in Web Forms.
	/// </summary>
	public string Title
	{
		get => _pageService.Title;
		set => _pageService.Title = value;
	}

	/// <summary>
	/// Gets or sets the meta description for the page. Delegates to IPageService.
	/// Equivalent to Page.MetaDescription in Web Forms.
	/// </summary>
	public string MetaDescription
	{
		get => _pageService.MetaDescription;
		set => _pageService.MetaDescription = value;
	}

	/// <summary>
	/// Gets or sets the meta keywords for the page. Delegates to IPageService.
	/// Equivalent to Page.MetaKeywords in Web Forms.
	/// </summary>
	public string MetaKeywords
	{
		get => _pageService.MetaKeywords;
		set => _pageService.MetaKeywords = value;
	}

	/// <summary>
	/// Always returns false. Blazor has no postback model.
	/// Exists so that if (!IsPostBack) { ... } compiles and executes correctly —
	/// the guarded block always runs, which is the correct behavior for
	/// OnInitialized (first-render) context.
	/// </summary>
	public bool IsPostBack => false;

	/// <summary>
	/// Returns this instance, enabling Page.Title, Page.MetaDescription,
	/// and other Page.* patterns to compile unchanged from Web Forms code-behind.
	/// </summary>
	protected WebFormsPageBase Page => this;
}
