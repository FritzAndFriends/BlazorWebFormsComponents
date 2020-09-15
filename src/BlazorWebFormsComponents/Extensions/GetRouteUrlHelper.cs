using Microsoft.AspNetCore.Routing;

namespace BlazorWebFormsComponents.Extensions
{
	public static class GetRouteUrlHelper
	{
		public static string GetRouteUrl(this BaseWebFormsComponent component, object routeParameters)
			=> component.LinkGenerator.GetPathByRouteValues(component.HttpContextAccessor.HttpContext, null, routeParameters);

		public static string GetRouteUrl(this BaseWebFormsComponent component, string routeName, object routeParameters)
			=> component.LinkGenerator.GetPathByRouteValues(component.HttpContextAccessor.HttpContext, routeName, routeParameters);

		public static string GetRouteUrl(this BaseWebFormsComponent component, RouteValueDictionary routeParameters)
			=> null;

		public static string GetRouteUrl(this BaseWebFormsComponent component, string routeName, RouteValueDictionary routeParameters)
			=> null;
	}
}
