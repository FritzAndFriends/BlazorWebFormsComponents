using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BlazorWebFormsComponents;

/// <summary>
/// Adapter that presents an ASP.NET Core <see cref="HttpContext"/> with a Web Forms-like
/// API surface. Passed to <see cref="HttpHandlerBase.ProcessRequestAsync"/> so that
/// migrated handler code can use familiar property names like <c>context.Request.QueryString</c>,
/// <c>context.Response.Write</c>, and <c>context.Server.MapPath</c>.
/// </summary>
public class HttpHandlerContext
{
	private readonly HttpContext _httpContext;

	/// <summary>
	/// Creates a new <see cref="HttpHandlerContext"/> wrapping the specified ASP.NET Core context.
	/// </summary>
	/// <param name="httpContext">The ASP.NET Core HTTP context for the current request.</param>
	/// <param name="environment">The web host environment, used by <see cref="Server"/> for path mapping.</param>
	public HttpHandlerContext(HttpContext httpContext, IWebHostEnvironment environment)
	{
		_httpContext = httpContext;
		Request = new HttpHandlerRequest(httpContext.Request);
		Response = new HttpHandlerResponse(httpContext.Response);
		Server = new HttpHandlerServer(environment);
	}

	/// <summary>
	/// Gets the request adapter providing Web Forms-compatible access to query string,
	/// form data, headers, cookies, and uploaded files.
	/// </summary>
	public HttpHandlerRequest Request { get; }

	/// <summary>
	/// Gets the response adapter providing Web Forms-compatible methods for writing
	/// content, setting headers, and controlling the response.
	/// </summary>
	public HttpHandlerResponse Response { get; }

	/// <summary>
	/// Gets the server utilities adapter providing <c>MapPath</c>, <c>HtmlEncode</c>,
	/// <c>UrlEncode</c>, and related helper methods.
	/// </summary>
	public HttpHandlerServer Server { get; }

	/// <summary>
	/// Gets the session state for the current request. Requires ASP.NET Core session
	/// middleware (<c>builder.Services.AddSession()</c> and <c>app.UseSession()</c>).
	/// For handlers marked with <see cref="RequiresSessionStateAttribute"/>,
	/// <c>LoadAsync</c> is called automatically before <c>ProcessRequestAsync</c>.
	/// </summary>
	/// <exception cref="System.InvalidOperationException">
	/// Thrown if session middleware is not configured.
	/// </exception>
	public ISession Session => _httpContext.Session;

	/// <summary>
	/// Gets the authenticated user for the current request.
	/// Equivalent to <c>HttpContext.User</c> in Web Forms.
	/// </summary>
	public ClaimsPrincipal User => _httpContext.User;

	/// <summary>
	/// Gets the per-request items dictionary.
	/// Equivalent to <c>HttpContext.Items</c> in Web Forms.
	/// </summary>
	public IDictionary<object, object> Items => _httpContext.Items;
}
