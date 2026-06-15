using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Microsoft.JSInterop;

namespace BlazorWebFormsComponents;

/// <summary>
/// Base class for converted ASP.NET Web Forms pages. Provides Page.Title,
/// Page.MetaDescription, Page.MetaKeywords, IsPostBack, Response.Redirect,
/// Response.Cookies, Request.Cookies, Request.QueryString, Request.Url,
/// ViewState, GetRouteUrl, ClientScript, and PostBack compatibility so that
/// Web Forms code-behind patterns survive migration with minimal changes.
///
/// <para>Cascades itself as a <see cref="CascadingValue{TValue}"/> named
/// "WebFormsPage" so that data-bound components (ListView, GridView, etc.)
/// can resolve string-based SelectMethod/InsertMethod/UpdateMethod/DeleteMethod
/// names via reflection, exactly as ASP.NET Web Forms does.</para>
/// </summary>
public abstract class WebFormsPageBase : ComponentBase, IAsyncDisposable
{
internal const string CascadingParameterName = "WebFormsPage";

private static readonly FieldInfo _renderFragmentField =
	typeof(ComponentBase).GetField("_renderFragment", BindingFlags.NonPublic | BindingFlags.Instance);

private readonly RenderFragment _baseRenderFragment;

protected WebFormsPageBase()
{
	// Wrap the default render output in a CascadingValue so child data-bound
	// components can locate this page instance for SelectMethod resolution.
	_baseRenderFragment = (RenderFragment)_renderFragmentField.GetValue(this);
	_renderFragmentField.SetValue(this, (RenderFragment)PageCascadingBuildRenderTree);

	void PageCascadingBuildRenderTree(RenderTreeBuilder builder)
	{
		builder.OpenComponent(0, typeof(CascadingValue<WebFormsPageBase>));
		builder.AddAttribute(1, nameof(CascadingValue<object>.Name), CascadingParameterName);
		builder.AddAttribute(2, nameof(CascadingValue<object>.Value), this);
		builder.AddAttribute(3, nameof(CascadingValue<object>.ChildContent), _baseRenderFragment);
		// Do not mark the page cascade as fixed. During enhanced navigation the
		// renderer can reuse this CascadingValue component across route transitions,
		// and descendants must observe the updated page instance.
		builder.CloseComponent();
	}
}
[Inject] private IServiceProvider ServiceProvider { get; set; } = null!;

private SessionShim? _sessionShim;
private CacheShim? _cacheShim;
private ClientScriptShim? _clientScriptShim;
private IMemoryCache? _fallbackMemoryCache;

private IPageService? _pageService;
private string _currentTitle = string.Empty;
private string _currentMetaDescription = string.Empty;
private string _currentMetaKeywords = string.Empty;
private bool _pageServiceSubscribed;

private NavigationManager NavigationManager => ServiceProvider.GetRequiredService<NavigationManager>();
private IJSRuntime JsRuntime => ServiceProvider.GetRequiredService<IJSRuntime>();
private LinkGenerator LinkGenerator => ServiceProvider.GetRequiredService<LinkGenerator>();
private IHttpContextAccessor HttpContextAccessor
    => ServiceProvider.GetService<IHttpContextAccessor>() ?? new HttpContextAccessor();
private ILogger<WebFormsPageBase> Logger
    => ServiceProvider.GetService<ILogger<WebFormsPageBase>>() ?? NullLogger<WebFormsPageBase>.Instance;
private IWebHostEnvironment WebHostEnvironment => ServiceProvider.GetRequiredService<IWebHostEnvironment>();

/// <summary>
/// Provides access to client script registration methods, emulating
/// <c>Page.ClientScript</c> from ASP.NET Web Forms.
/// </summary>
public ClientScriptShim ClientScript
    => _clientScriptShim ??= ServiceProvider.GetService<ClientScriptShim>()
        ?? new ClientScriptShim(ServiceProvider.GetService<ILogger<ClientScriptShim>>()
            ?? NullLogger<ClientScriptShim>.Instance);

// ─── PostBack Support ─────────────────────────────────────────────

private DotNetObjectReference<WebFormsPageBase>? _postBackRef;
private string? _postBackTargetId;

/// <summary>
/// Raised when a postback is triggered from JavaScript via <c>__doPostBack()</c>.
/// Subscribe to this event in derived pages to handle postback actions.
/// </summary>
public event EventHandler<PostBackEventArgs>? PostBack;

/// <summary>
/// Provides dictionary-style <c>Session["key"]</c> access, emulating
/// ASP.NET Web Forms <c>HttpSessionState</c>. Backed by ASP.NET Core
/// <see cref="ISession"/> in SSR mode; falls back to in-memory storage
/// in interactive Blazor Server mode.
/// </summary>
protected SessionShim Session
    => _sessionShim ??= ServiceProvider.GetService<SessionShim>()
        ?? new SessionShim(ServiceProvider.GetService<ILogger<SessionShim>>()
            ?? NullLogger<SessionShim>.Instance, HttpContextAccessor);

/// <summary>
/// Returns <c>true</c> when an <see cref="HttpContext"/> is available
/// (SSR or pre-render), <c>false</c> during interactive WebSocket rendering.
/// Use this to guard code that depends on HTTP-level features such as
/// cookies, headers, or <see cref="GetRouteUrl"/>.
/// </summary>
protected bool IsHttpContextAvailable
    => HttpContextAccessor.HttpContext is not null;

/// <summary>
/// Gets or sets the title of the page. Delegates to IPageService.
/// Equivalent to Page.Title in Web Forms.
/// </summary>
public string Title
{
get => _pageService?.Title ?? _currentTitle;
set
{
    _currentTitle = value ?? string.Empty;
    if (_pageService is not null)
    {
        _pageService.Title = _currentTitle;
    }
}
}

/// <summary>
/// Gets or sets the meta description for the page. Delegates to IPageService.
/// Equivalent to Page.MetaDescription in Web Forms.
/// </summary>
public string MetaDescription
{
get => _pageService?.MetaDescription ?? _currentMetaDescription;
set
{
    _currentMetaDescription = value ?? string.Empty;
    if (_pageService is not null)
    {
        _pageService.MetaDescription = _currentMetaDescription;
    }
}
}

/// <summary>
/// Gets or sets the meta keywords for the page. Delegates to IPageService.
/// Equivalent to Page.MetaKeywords in Web Forms.
/// </summary>
public string MetaKeywords
{
get => _pageService?.MetaKeywords ?? _currentMetaKeywords;
set
{
    _currentMetaKeywords = value ?? string.Empty;
    if (_pageService is not null)
    {
        _pageService.MetaKeywords = _currentMetaKeywords;
    }
}
}

/// <summary>
/// The current page title resolved from <see cref="IPageService"/> or the local fallback state.
/// Intended for components that render page head content.
/// </summary>
protected string CurrentTitle => _currentTitle;

/// <summary>
/// The current page meta description resolved from <see cref="IPageService"/> or the local fallback state.
/// Intended for components that render page head content.
/// </summary>
protected string CurrentMetaDescription => _currentMetaDescription;

/// <summary>
/// The current page meta keywords resolved from <see cref="IPageService"/> or the local fallback state.
/// Intended for components that render page head content.
/// </summary>
protected string CurrentMetaKeywords => _currentMetaKeywords;

/// <summary>
/// Returns <c>true</c> when the current request is a postback (form POST in SSR mode)
/// or after the first initialization (in ServerInteractive mode).
/// Matches the ASP.NET Web Forms <c>Page.IsPostBack</c> semantics:
/// <list type="bullet">
///   <item><description>SSR GET request → <c>false</c></description></item>
///   <item><description>SSR POST request → <c>true</c></description></item>
///   <item><description>ServerInteractive first render → <c>false</c></description></item>
///   <item><description>ServerInteractive subsequent renders → <c>true</c></description></item>
/// </list>
/// </summary>
public bool IsPostBack
{
	get
	{
		// SSR mode: HttpContext is available — check HTTP method
		if (HttpContextAccessor.HttpContext is { } context)
			return HttpMethods.IsPost(context.Request.Method);

		// ServerInteractive mode: track initialization state
		return _hasInitialized;
	}
}
private bool _hasInitialized;

/// <summary>
/// Returns this instance, enabling Page.Title, Page.MetaDescription,
/// Page.GetRouteUrl, and other Page.* patterns to compile unchanged
/// from Web Forms code-behind.
/// </summary>
protected WebFormsPageBase Page => this;

/// <summary>
/// Compatibility shim for Web Forms <c>Context</c> access.
/// Returns the current <see cref="HttpContext"/> when available.
/// </summary>
protected HttpContext? Context => HttpContextAccessor.HttpContext;

/// <summary>
/// Compatibility property for Web Forms <c>Page.ViewStateUserKey</c>.
/// </summary>
public string? ViewStateUserKey { get; set; }

/// <summary>
/// Compatibility shim for Web Forms <c>Response</c> object.
/// Supports <c>Response.Redirect()</c> and <c>Response.Cookies</c>.
/// Cookies degrade gracefully to no-op when HttpContext is unavailable.
/// </summary>
protected ResponseShim Response
	=> new(NavigationManager, HttpContextAccessor.HttpContext, Logger);

/// <summary>
/// Compatibility shim for Web Forms <c>Request</c> object.
/// Supports <c>Request.Cookies</c>, <c>Request.Form</c>,
/// <c>Request.QueryString</c>, and <c>Request.Url</c>. Degrades
/// gracefully when HttpContext is unavailable: cookies and form
/// return empty, QueryString and Url fall back to
/// <see cref="NavigationManager"/>.
/// </summary>
protected RequestShim Request
	=> _requestShim ??= new(HttpContextAccessor.HttpContext, NavigationManager, Logger);
private RequestShim? _requestShim;

/// <summary>
/// Injects form data captured by <see cref="WebFormsForm"/> into the
/// <see cref="Request"/> shim so that <c>Request.Form["key"]</c>
/// returns the submitted values during interactive mode.
/// </summary>
/// <param name="formData">Form field data from JS interop.</param>
public void SetRequestFormData(Dictionary<string, StringValues> formData)
{
	// Ensure the shim is initialized, then push form data into it
	_ = Request;
	_requestShim!.SetFormData(formData);
}

/// <summary>
/// Convenience overload accepting <see cref="FormSubmitEventArgs"/>
/// directly from a <c>&lt;WebFormsForm OnSubmit="SetRequestFormData"&gt;</c> binding.
/// </summary>
/// <param name="e">The form submit event args.</param>
public void SetRequestFormData(FormSubmitEventArgs e)
{
	SetRequestFormData(e.FormData);
}

/// <summary>
/// Compatibility shim for Web Forms <c>Server</c> object.
/// Supports <c>Server.MapPath()</c>, <c>Server.HtmlEncode()</c>,
/// <c>Server.UrlEncode()</c>, etc.
/// </summary>
protected ServerShim Server => new(WebHostEnvironment, NavigationManager);

/// <summary>
/// Compatibility shim for Web Forms <c>Cache</c> object
/// (<c>Page.Cache</c> / <c>HttpRuntime.Cache</c>).
/// Provides dictionary-style <c>Cache["key"]</c> access backed by
/// ASP.NET Core <see cref="Microsoft.Extensions.Caching.Memory.IMemoryCache"/>.
/// </summary>
protected CacheShim Cache
    => _cacheShim ??= ServiceProvider.GetService<CacheShim>()
        ?? new CacheShim(_fallbackMemoryCache ??= new MemoryCache(new MemoryCacheOptions()),
            ServiceProvider.GetService<ILogger<CacheShim>>() ?? NullLogger<CacheShim>.Instance);

/// <summary>
/// Resolves a relative URL to an application-absolute URL.
/// Equivalent to <c>Page.ResolveUrl("~/images/logo.png")</c> in Web Forms.
/// Strips <c>~/</c> prefix and <c>.aspx</c> extensions.
/// </summary>
protected string ResolveUrl(string relativeUrl)
{
    if (string.IsNullOrEmpty(relativeUrl))
        return relativeUrl;

    if (relativeUrl.StartsWith("~/", StringComparison.Ordinal))
        relativeUrl = relativeUrl[1..]; // ~/foo → /foo

    // Strip .aspx extension for Blazor routing
    if (relativeUrl.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
        relativeUrl = relativeUrl[..^5];

    return relativeUrl;
}

/// <summary>
/// Resolves a URL relative to this page's location.
/// Equivalent to <c>Page.ResolveClientUrl()</c> in Web Forms.
/// For Blazor, this behaves the same as <see cref="ResolveUrl"/>.
/// </summary>
protected string ResolveClientUrl(string relativeUrl) => ResolveUrl(relativeUrl);

/// <summary>
/// Dictionary-based state storage emulating ASP.NET Web Forms ViewState.
/// In ServerInteractive mode, persists for the component's lifetime (in-memory).
/// In SSR mode, round-trips via a protected hidden form field.
///
/// <para><b>Migration note:</b> This enables Web Forms ViewState-backed property
/// patterns to work unchanged. For new Blazor code, prefer [Parameter] properties
/// and component fields.</para>
/// </summary>
public ViewStateDictionary ViewState { get; } = new();

/// <inheritdoc />
protected override void OnInitialized()
{
	base.OnInitialized();
    EnsurePageService();
	_hasInitialized = true;
}

private void EnsurePageService()
{
    if (_pageServiceSubscribed)
    {
        return;
    }

    _pageService = ServiceProvider.GetService<IPageService>();
    if (_pageService is null)
    {
        return;
    }

    _currentTitle = _pageService.Title;
    _currentMetaDescription = _pageService.MetaDescription;
    _currentMetaKeywords = _pageService.MetaKeywords;

    _pageService.TitleChanged += OnTitleChanged;
    _pageService.MetaDescriptionChanged += OnMetaDescriptionChanged;
    _pageService.MetaKeywordsChanged += OnMetaKeywordsChanged;
    _pageServiceSubscribed = true;
}

private async void OnTitleChanged(object? sender, string newTitle)
{
    _currentTitle = newTitle;
    await NotifyPageStateChangedAsync();
}

private async void OnMetaDescriptionChanged(object? sender, string newMetaDescription)
{
    _currentMetaDescription = newMetaDescription;
    await NotifyPageStateChangedAsync();
}

private async void OnMetaKeywordsChanged(object? sender, string newMetaKeywords)
{
    _currentMetaKeywords = newMetaKeywords;
    await NotifyPageStateChangedAsync();
}

protected virtual async Task NotifyPageStateChangedAsync()
{
    try
    {
        await InvokeAsync(StateHasChanged);
    }
    catch (ObjectDisposedException)
    {
    }
}

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

return LinkGenerator.GetPathByRouteValues(
	HttpContextAccessor.HttpContext, routeName, routeParameters);
}

/// <summary>
/// Generates a URL for the specified route parameters.
/// Equivalent to Page.GetRouteUrl(routeParameters) in Web Forms.
/// </summary>
public string GetRouteUrl(object routeParameters)
	=> LinkGenerator.GetPathByRouteValues(HttpContextAccessor.HttpContext, null, routeParameters);

/// <summary>
/// Generates a URL for the specified route parameters dictionary.
/// Equivalent to Page.GetRouteUrl(routeParameters) in Web Forms.
/// </summary>
public string GetRouteUrl(RouteValueDictionary routeParameters)
	=> LinkGenerator.GetPathByRouteValues(HttpContextAccessor.HttpContext, null, routeParameters);

/// <summary>
/// Generates a URL for the specified named route with a parameters dictionary.
/// Equivalent to Page.GetRouteUrl(routeName, routeParameters) in Web Forms.
/// </summary>
public string GetRouteUrl(string routeName, RouteValueDictionary routeParameters)
	=> LinkGenerator.GetPathByRouteValues(HttpContextAccessor.HttpContext, routeName, routeParameters);

/// <summary>
/// Guards a member that requires <see cref="HttpContext"/>.
/// Throws <see cref="InvalidOperationException"/> when HttpContext is unavailable.
/// </summary>
/// <param name="memberName">The name of the calling member, for diagnostics.</param>
private void RequireHttpContext(string memberName)
{
if (HttpContextAccessor.HttpContext is null)
throw new InvalidOperationException(
$"{memberName} requires HttpContext, which is unavailable during interactive " +
$"rendering (WebSocket mode). Use {nameof(IsHttpContextAvailable)} to guard " +
$"calls to this member, or ensure the page runs in SSR mode.");
}

// ─── PostBack JS Interop ──────────────────────────────────────────

// Inline bootstrap ensures __doPostBack and registration functions exist
// before any postback target is registered. The standalone bwfc-postback.js
// file is also available for manual <script> inclusion.
private const string PostBackBootstrapJs = @"
window.__bwfc = window.__bwfc || {};
window.__bwfc.postBackTargets = window.__bwfc.postBackTargets || {};
if (!window.__doPostBack) {
    window.__doPostBack = function(t, a) {
        var h = window.__bwfc.postBackTargets[t];
        if (h) h.invokeMethodAsync('HandlePostBackFromJs', t, a);
        else console.warn('[BWFC] No postback handler for:', t);
    };
}
if (!window.__bwfc.registerPostBackTarget) {
    window.__bwfc.registerPostBackTarget = function(id, dotNetRef) {
        window.__bwfc.postBackTargets[id] = dotNetRef;
    };
}
if (!window.__bwfc.unregisterPostBackTarget) {
    window.__bwfc.unregisterPostBackTarget = function(id) {
        delete window.__bwfc.postBackTargets[id];
    };
}
if (!window.__bwfc_callback) {
    window.__bwfc_callback = function(id, arg, successCb, ctx, errorCb) {
        var h = window.__bwfc.postBackTargets[id];
        if (h) {
            h.invokeMethodAsync('HandleCallbackFromJs', id, arg)
                .then(function(r) { if (successCb) successCb(r, ctx); })
                .catch(function(e) { if (errorCb) errorCb(e.message, ctx); });
        }
    };
}
";

/// <summary>
/// Called from JavaScript when <c>__doPostBack(eventTarget, eventArgument)</c> fires.
/// Raises the <see cref="PostBack"/> event and triggers a re-render.
/// </summary>
[JSInvokable]
public Task HandlePostBackFromJs(string eventTarget, string eventArgument)
{
    PostBack?.Invoke(this, new PostBackEventArgs(eventTarget, eventArgument));
    StateHasChanged();
    return Task.CompletedTask;
}

/// <summary>
/// Called from JavaScript when <c>__bwfc_callback</c> fires.
/// Override in derived pages to return callback data.
/// </summary>
[JSInvokable]
public virtual Task<string> HandleCallbackFromJs(string eventTarget, string eventArgument)
{
    return Task.FromResult(string.Empty);
}

/// <inheritdoc />
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    await base.OnAfterRenderAsync(firstRender);

    if (firstRender && EnablePostBackInterop)
    {
        _postBackTargetId = GetType().Name;
        _postBackRef = DotNetObjectReference.Create(this);

        // Bootstrap __doPostBack and registration functions
        await JsRuntime.InvokeVoidAsync("eval", PostBackBootstrapJs);
        await JsRuntime.InvokeVoidAsync("__bwfc.registerPostBackTarget",
            _postBackTargetId, _postBackRef);
    }

    // Flush any queued ClientScript registrations
        if (_clientScriptShim != null)
        {
            await _clientScriptShim.FlushAsync(JsRuntime);
        }
}

/// <summary>
/// Controls whether the page base should register Web Forms postback JS interop.
/// Head-only helper components can disable this.
/// </summary>
protected virtual bool EnablePostBackInterop => true;

/// <inheritdoc />
public virtual async ValueTask DisposeAsync()
{
    if (_pageServiceSubscribed && _pageService is not null)
    {
        _pageService.TitleChanged -= OnTitleChanged;
        _pageService.MetaDescriptionChanged -= OnMetaDescriptionChanged;
        _pageService.MetaKeywordsChanged -= OnMetaKeywordsChanged;
        _pageServiceSubscribed = false;
    }

    if (_postBackTargetId != null && _postBackRef != null)
    {
        try
        {
            await JsRuntime.InvokeVoidAsync(
                "__bwfc.unregisterPostBackTarget", _postBackTargetId);
        }
        catch (JSDisconnectedException) { }
        catch (ObjectDisposedException) { }
        catch (InvalidOperationException) { }

        _postBackRef.Dispose();
        _postBackRef = null;
    }
}
}
