using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace BlazorWebFormsComponents.Cli.Transforms;

/// <summary>
/// Parses <c>MapPageRoute</c> definitions from Global.asax.cs and provides
/// a route map keyed by route name. Used by both <see cref="Directives.PageDirectiveTransform"/>
/// (to add extra <c>@page</c> directives) and <see cref="Markup.GetRouteUrlMarkupTransform"/>
/// (to resolve <c>GetRouteUrl</c> calls to direct URLs).
/// </summary>
public static class PageRouteParser
{
	private static readonly ConcurrentDictionary<string, IReadOnlyList<PageRoute>> Cache = new();

	// Matches MapPageRoute("name", "template/{param}", "~/Page.aspx")
	private static readonly Regex MapPageRouteRegex = new(
		@"MapPageRoute\s*\(\s*""(?<name>[^""]+)""\s*,\s*""(?<template>[^""]+)""\s*,\s*""(?<page>[^""]+)""",
		RegexOptions.Compiled);

	/// <summary>
	/// Parses and caches page routes from Global.asax.cs in the given source root.
	/// </summary>
	public static IReadOnlyList<PageRoute> GetRoutes(string? sourceRootPath)
	{
		if (string.IsNullOrWhiteSpace(sourceRootPath) || !Directory.Exists(sourceRootPath))
			return [];

		return Cache.GetOrAdd(sourceRootPath, static path =>
		{
			var routes = new List<PageRoute>();

			var globalAsaxFiles = Directory.EnumerateFiles(path, "Global.asax.cs", SearchOption.AllDirectories)
				.Take(1)
				.ToList();

			foreach (var file in globalAsaxFiles)
			{
				var content = File.ReadAllText(file);
				foreach (Match match in MapPageRouteRegex.Matches(content))
				{
					routes.Add(new PageRoute(
						match.Groups["name"].Value,
						match.Groups["template"].Value,
						match.Groups["page"].Value));
				}
			}

			return routes;
		});
	}

	/// <summary>
	/// Finds routes whose target page matches the given source file.
	/// </summary>
	public static IReadOnlyList<PageRoute> FindRoutesForPage(string? sourceRootPath, string sourceFilePath)
	{
		var allRoutes = GetRoutes(sourceRootPath);
		if (allRoutes.Count == 0 || string.IsNullOrWhiteSpace(sourceRootPath))
			return [];

		var sourceFileName = Path.GetFileNameWithoutExtension(sourceFilePath);
		var results = new List<PageRoute>();

		foreach (var route in allRoutes)
		{
			// Route target is like "~/ProductList.aspx" — extract file name
			var routeTargetName = Path.GetFileNameWithoutExtension(route.TargetPage.TrimStart('~', '/'));
			if (string.Equals(routeTargetName, sourceFileName, StringComparison.OrdinalIgnoreCase))
			{
				results.Add(route);
			}
		}

		return results;
	}

	/// <summary>
	/// Returns a dictionary of route definitions keyed by route name (case-insensitive).
	/// </summary>
	public static IReadOnlyDictionary<string, PageRoute> GetRouteMap(string? sourceRootPath)
	{
		var routes = GetRoutes(sourceRootPath);
		var map = new Dictionary<string, PageRoute>(StringComparer.OrdinalIgnoreCase);
		foreach (var route in routes)
		{
			map[route.Name] = route;
		}
		return map;
	}

	/// <summary>
	/// Clears the cached route data (for testing).
	/// </summary>
	public static void ClearCache() => Cache.Clear();
}

/// <summary>
/// A named page route from <c>routes.MapPageRoute(name, template, targetPage)</c>.
/// </summary>
/// <param name="Name">Route name (e.g., "ProductByNameRoute")</param>
/// <param name="Template">URL template (e.g., "Product/{productName}")</param>
/// <param name="TargetPage">Target page (e.g., "~/ProductDetails.aspx")</param>
public sealed record PageRoute(string Name, string Template, string TargetPage);
