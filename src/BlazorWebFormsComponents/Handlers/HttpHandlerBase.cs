using System.Threading.Tasks;

namespace BlazorWebFormsComponents;

/// <summary>
/// Abstract base class for migrating ASP.NET Web Forms .ashx HTTP handlers to ASP.NET Core.
/// Developers inherit from this class instead of implementing IHttpHandler, and override
/// <see cref="ProcessRequestAsync"/> with their handler logic. Registration is done via
/// <c>app.MapHandler&lt;THandler&gt;("/path.ashx")</c> in Program.cs.
/// </summary>
public abstract class HttpHandlerBase
{
	/// <summary>
	/// Override this method to handle the HTTP request.
	/// This is the async equivalent of <c>IHttpHandler.ProcessRequest</c>.
	/// The handler writes directly to <see cref="HttpHandlerContext.Response"/>.
	/// </summary>
	/// <param name="context">
	/// A Web Forms-compatible context adapter wrapping the ASP.NET Core <c>HttpContext</c>.
	/// Provides <c>Request</c>, <c>Response</c>, <c>Server</c>, and <c>Session</c> with
	/// familiar API shapes.
	/// </param>
	public abstract Task ProcessRequestAsync(HttpHandlerContext context);

	/// <summary>
	/// Indicates whether the handler instance can be reused for multiple requests.
	/// Defaults to <c>false</c>, matching the most common Web Forms pattern.
	/// In ASP.NET Core, handlers are instantiated per-request via
	/// <c>ActivatorUtilities.CreateInstance</c>, so this property has no behavioral
	/// effect — it exists for API surface compatibility with <c>IHttpHandler.IsReusable</c>.
	/// </summary>
	public virtual bool IsReusable => false;
}
