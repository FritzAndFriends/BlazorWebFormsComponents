using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BlazorWebFormsComponents;

/// <summary>
/// Base class for converted ASP.NET Web Forms pages. Provides Page.Title,
/// Page.MetaDescription, Page.MetaKeywords, IsPostBack, Response.Redirect,
/// ViewState, and GetRouteUrl compatibility so that Web Forms code-behind
/// patterns survive migration with minimal changes.
/// </summary>
public abstract class WebFormsPageBase : ComponentBase
{
	[Inject] private IPageService _pageService { get; set; } = null!;
	[Inject] private NavigationManager _navigationManager { get; set; } = null!;
	[Inject] private LinkGenerator _linkGenerator { get; set; } = null!;
	[Inject] private IHttpContextAccessor _httpContextAccessor { get; set; } = null!;

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

	/// <summary>
	/// Compatibility shim for Web Forms <c>Response.Redirect()</c>.
	/// Delegates to <see cref="NavigationManager.NavigateTo(string, bool)"/>.
	/// </summary>
	protected ResponseShim Response => new(_navigationManager);

	/// <summary>
	/// In-memory dictionary emulating Web Forms ViewState.
	/// Values do NOT survive navigation — they live only for the lifetime of
	/// the component instance (equivalent to private fields).
	/// </summary>
	[Obsolete("ViewState is in-memory only in Blazor. Values do not survive navigation.")]
	public Dictionary<string, object> ViewState { get; } = new();

	/// <summary>
	/// Generates a URL for the named route with the specified parameters.
	/// Equivalent to <c>Page.GetRouteUrl("RouteName", new { id = 1 })</c> in Web Forms.
	/// Strips <c>.aspx</c> extension from route names for compatibility.
	/// </summary>
	protected string GetRouteUrl(string routeName, object routeParameters = null)
	{
		if (routeName != null && routeName.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
			routeName = routeName[..^5];

		return _linkGenerator.GetPathByRouteValues(
			_httpContextAccessor.HttpContext, routeName, routeParameters);
	}
}
