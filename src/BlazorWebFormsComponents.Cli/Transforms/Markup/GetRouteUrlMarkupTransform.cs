using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Converts <c>GetRouteUrl("RouteName", new { param = value })</c> calls in markup
/// to direct Razor URL interpolation based on route definitions found in Global.asax.cs.
///
/// Web Forms <c>MapPageRoute</c> defines named routes like:
///   <c>routes.MapPageRoute("ProductByNameRoute", "Product/{productName}", "~/ProductDetails.aspx")</c>
///
/// At runtime, <c>GetRouteUrl("ProductByNameRoute", new { productName = x })</c> resolves to
/// <c>/Product/{x}</c>. In migrated Blazor, the GetRouteUrl shim on WebFormsPageBase uses
/// LinkGenerator, which requires named endpoint routes. Since Blazor page routing doesn't
/// register named routes, this transform resolves them statically at migration time.
///
/// Example:
///   <c>@(GetRouteUrl("ProductByNameRoute", new { productName = Item.ProductName }))</c>
/// becomes:
///   <c>@($"/Product/{Item.ProductName}")</c>
/// </summary>
public sealed class GetRouteUrlMarkupTransform : IMarkupTransform
{
	public string Name => "GetRouteUrlMarkup";
	public int Order => 820;

	// Matches GetRouteUrl("RouteName", new { param = expr }) — single parameter
	private static readonly Regex GetRouteUrlSingleParamRegex = new(
		@"GetRouteUrl\(\s*""(?<route>[^""]+)""\s*,\s*new\s*\{\s*(?<param>\w+)\s*=\s*(?<expr>[^}]+?)\s*\}\s*\)",
		RegexOptions.Compiled);

	public string Apply(string content, FileMetadata metadata)
	{
		if (!content.Contains("GetRouteUrl", StringComparison.Ordinal))
			return content;

		var routeMap = PageRouteParser.GetRouteMap(metadata.SourceRootPath);
		if (routeMap.Count == 0)
			return content;

		return GetRouteUrlSingleParamRegex.Replace(content, match =>
		{
			var routeName = match.Groups["route"].Value;
			var paramName = match.Groups["param"].Value.Trim();
			var paramExpr = match.Groups["expr"].Value.Trim();

			if (!routeMap.TryGetValue(routeName, out var route))
				return match.Value;

			var resolvedUrl = ResolveUrl(route.Template, paramName, paramExpr);
			return resolvedUrl ?? match.Value;
		});
	}

	private static string? ResolveUrl(string template, string paramName, string paramExpr)
	{
		// Template like "Product/{productName}" with param "productName" = "Item.ProductName"
		// → $"/Product/{Item.ProductName}"
		var placeholder = $"{{{paramName}}}";
		if (!template.Contains(placeholder, StringComparison.OrdinalIgnoreCase))
			return null;

		var resolvedTemplate = template.Replace(placeholder, $"{{{paramExpr}}}", StringComparison.OrdinalIgnoreCase);
		return $"$\"/{resolvedTemplate.TrimStart('/')}\"";
	}
}
