using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorWebFormsComponents;

/// <summary>
/// Extension methods for registering <see cref="HttpHandlerBase"/> subclasses as
/// Minimal API endpoints. Each handler is mapped using <c>endpoints.Map()</c> to
/// handle all HTTP methods, matching Web Forms <c>IHttpHandler</c> behavior.
/// </summary>
public static class HandlerEndpointExtensions
{
	/// <summary>
	/// Maps an <see cref="HttpHandlerBase"/> subclass to the specified route pattern
	/// using Minimal API. The handler is instantiated per-request via DI, supporting
	/// constructor injection. Handles all HTTP methods (GET, POST, PUT, DELETE, etc.)
	/// — matching Web Forms <c>IHttpHandler</c> behavior where <c>ProcessRequest</c>
	/// handles everything.
	/// </summary>
	/// <typeparam name="THandler">
	/// The handler type. Must inherit from <see cref="HttpHandlerBase"/>.
	/// </typeparam>
	/// <param name="endpoints">The endpoint route builder.</param>
	/// <param name="pattern">
	/// The route pattern (e.g., <c>"/Handlers/FileDownload.ashx"</c>).
	/// </param>
	/// <returns>
	/// An <see cref="IEndpointConventionBuilder"/> for further configuration such as
	/// <c>.RequireAuthorization()</c> or <c>.RequireCors()</c>.
	/// </returns>
	public static IEndpointConventionBuilder MapHandler<THandler>(
		this IEndpointRouteBuilder endpoints,
		string pattern)
		where THandler : HttpHandlerBase
	{
		return endpoints.Map(pattern, async (HttpContext httpContext) =>
		{
			// Resolve handler with DI — supports constructor injection
			var handler = ActivatorUtilities.CreateInstance<THandler>(httpContext.RequestServices);

			// Session pre-load for handlers that need it
			if (typeof(THandler).IsDefined(typeof(RequiresSessionStateAttribute), inherit: true))
			{
				await httpContext.Session.LoadAsync(httpContext.RequestAborted);
			}

			// Build the Web Forms-compatible context adapter
			var env = httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
			var handlerContext = new HttpHandlerContext(httpContext, env);

			// Execute the handler — writes directly to Response
			await handler.ProcessRequestAsync(handlerContext);
		});
	}

	/// <summary>
	/// Convention-based overload: derives the route from the handler class name.
	/// Strips the "Handler" suffix and appends ".ashx".
	/// For example, <c>FileDownloadHandler</c> maps to <c>/FileDownload.ashx</c>.
	/// </summary>
	/// <typeparam name="THandler">
	/// The handler type. Must inherit from <see cref="HttpHandlerBase"/>.
	/// </typeparam>
	/// <param name="endpoints">The endpoint route builder.</param>
	/// <returns>
	/// An <see cref="IEndpointConventionBuilder"/> for further configuration.
	/// </returns>
	public static IEndpointConventionBuilder MapHandler<THandler>(
		this IEndpointRouteBuilder endpoints)
		where THandler : HttpHandlerBase
	{
		var name = typeof(THandler).Name;
		if (name.EndsWith("Handler", StringComparison.Ordinal))
		{
			name = name[..^7];
		}
		var pattern = "/" + name + ".ashx";
		return endpoints.MapHandler<THandler>(pattern);
	}

	/// <summary>
	/// Convenience overload: maps a handler to multiple route patterns.
	/// Useful when preserving both the legacy .ashx URL and a clean URL.
	/// </summary>
	/// <typeparam name="THandler">
	/// The handler type. Must inherit from <see cref="HttpHandlerBase"/>.
	/// </typeparam>
	/// <param name="endpoints">The endpoint route builder.</param>
	/// <param name="patterns">
	/// One or more route patterns to map the handler to.
	/// </param>
	public static void MapHandler<THandler>(
		this IEndpointRouteBuilder endpoints,
		params string[] patterns)
		where THandler : HttpHandlerBase
	{
		foreach (var pattern in patterns)
		{
			endpoints.MapHandler<THandler>(pattern);
		}
	}
}
