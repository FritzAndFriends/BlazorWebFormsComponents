namespace System.Web.Routing;

/// <summary>
/// No-op shim for ASP.NET Web Forms <c>RouteCollection</c>.
/// Allows <c>App_Start/RouteConfig.cs</c> files to compile without modification.
/// Routing in Blazor is handled via <c>@page</c> directives and endpoint routing.
/// </summary>
public class RouteCollection
{
	public void MapPageRoute(string routeName, string routeUrl, string physicalFile) { }
	public void Ignore(string url) { }
	public void Ignore(string url, object constraints) { }
}

/// <summary>
/// No-op shim for ASP.NET Web Forms <c>RouteTable</c>.
/// <c>RouteTable.Routes</c> returns a no-op <see cref="RouteCollection"/>.
/// </summary>
public static class RouteTable
{
	public static RouteCollection Routes { get; } = new RouteCollection();
}
