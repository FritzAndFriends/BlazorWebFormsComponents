using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace BlazorWebFormsComponents;

/// <summary>
/// Base class for converted ASP.NET Web Forms pages. Provides Page.Title,
/// Page.MetaDescription, Page.MetaKeywords, IsPostBack, Response.Redirect,
/// Response.Cookies, Request.Cookies, Request.QueryString, Request.Url,
/// ViewState, and GetRouteUrl compatibility so that Web Forms code-behind
/// patterns survive migration with minimal changes.
/// </summary>
public abstract class WebFormsPageBase : ComponentBase
{
[Inject] private IPageService _pageService { get; set; } = null!;
[Inject] private NavigationManager _navigationManager { get; set; } = null!;
[Inject] private LinkGenerator _linkGenerator { get; set; } = null!;
[Inject] private IHttpContextAccessor _httpContextAccessor { get; set; } = null!;
[Inject] private ILogger<WebFormsPageBase> _logger { get; set; } = null!;

/// <summary>
/// Returns <c>true</c> when an <see cref="HttpContext"/> is available
/// (SSR or pre-render), <c>false</c> during interactive WebSocket rendering.
/// Use this to guard code that depends on HTTP-level features such as
/// cookies, headers, or <see cref="GetRouteUrl"/>.
/// </summary>
protected bool IsHttpContextAvailable
=> _httpContextAccessor.HttpContext is not null;

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
/// Page.GetRouteUrl, and other Page.* patterns to compile unchanged
/// from Web Forms code-behind.
/// </summary>
protected WebFormsPageBase Page => this;

/// <summary>
/// Compatibility shim for Web Forms <c>Response</c> object.
/// Supports <c>Response.Redirect()</c> and <c>Response.Cookies</c>.
/// Cookies degrade gracefully to no-op when HttpContext is unavailable.
/// </summary>
protected ResponseShim Response
=> new(_navigationManager, _httpContextAccessor.HttpContext, _logger);

/// <summary>
/// Compatibility shim for Web Forms <c>Request</c> object.
/// Supports <c>Request.Cookies</c>, <c>Request.QueryString</c>,
/// and <c>Request.Url</c>. Degrades gracefully when HttpContext
/// is unavailable: cookies return empty, QueryString and Url fall
/// back to <see cref="NavigationManager"/>.
/// </summary>
protected RequestShim Request
=> new(_httpContextAccessor.HttpContext, _navigationManager, _logger);

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
/// Requires <see cref="HttpContext"/> — throws during interactive WebSocket rendering.
/// </summary>
/// <exception cref="InvalidOperationException">
/// Thrown when called during interactive rendering where HttpContext is unavailable.
/// </exception>
protected string GetRouteUrl(string routeName, object routeParameters = null)
{
RequireHttpContext(nameof(GetRouteUrl));

if (routeName != null && routeName.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
routeName = routeName[..^5];

return _linkGenerator.GetPathByRouteValues(
_httpContextAccessor.HttpContext, routeName, routeParameters);
}

/// <summary>
/// Generates a URL for the specified route parameters.
/// Equivalent to Page.GetRouteUrl(routeParameters) in Web Forms.
/// </summary>
public string GetRouteUrl(object routeParameters)
=> _linkGenerator.GetPathByRouteValues(_httpContextAccessor.HttpContext, null, routeParameters);

/// <summary>
/// Generates a URL for the specified route parameters dictionary.
/// Equivalent to Page.GetRouteUrl(routeParameters) in Web Forms.
/// </summary>
public string GetRouteUrl(RouteValueDictionary routeParameters)
=> _linkGenerator.GetPathByRouteValues(_httpContextAccessor.HttpContext, null, routeParameters);

/// <summary>
/// Generates a URL for the specified named route with a parameters dictionary.
/// Equivalent to Page.GetRouteUrl(routeName, routeParameters) in Web Forms.
/// </summary>
public string GetRouteUrl(string routeName, RouteValueDictionary routeParameters)
=> _linkGenerator.GetPathByRouteValues(_httpContextAccessor.HttpContext, routeName, routeParameters);

/// <summary>
/// Guards a member that requires <see cref="HttpContext"/>.
/// Throws <see cref="InvalidOperationException"/> when HttpContext is unavailable.
/// </summary>
/// <param name="memberName">The name of the calling member, for diagnostics.</param>
private void RequireHttpContext(string memberName)
{
if (_httpContextAccessor.HttpContext is null)
throw new InvalidOperationException(
$"{memberName} requires HttpContext, which is unavailable during interactive " +
$"rendering (WebSocket mode). Use {nameof(IsHttpContextAvailable)} to guard " +
$"calls to this member, or ensure the page runs in SSR mode.");
}
}