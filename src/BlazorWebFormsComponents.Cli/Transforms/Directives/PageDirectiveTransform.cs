using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Directives;

/// <summary>
/// Converts &lt;%@ Page ... %&gt; directive to one or more @page routes and &lt;PageTitle&gt;.
/// </summary>
public class PageDirectiveTransform : IMarkupTransform
{
    public string Name => "PageDirective";
    public int Order => 100;

    private static readonly Regex PageDirectiveRegex = new(@"<%@\s*Page[^%]*%>\s*\r?\n?", RegexOptions.Compiled);
    private static readonly Regex TitleRegex = new(@"<%@\s*Page[^%]*Title\s*=\s*""([^""]*)""", RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        if (!PageDirectiveRegex.IsMatch(content))
        {
            return content;
        }

        string? pageTitle = null;
        var titleMatch = TitleRegex.Match(content);
        if (titleMatch.Success)
        {
            pageTitle = titleMatch.Groups[1].Value;
        }

        content = PageDirectiveRegex.Replace(content, "");

        var routes = BuildRoutes(metadata);
        var header = string.Join(string.Empty, routes.Select(static route => $"@page \"{route}\"\n"));
        if (pageTitle is not null)
        {
            header += $"<PageTitle>{pageTitle}</PageTitle>\n";
        }

        return header + content;
    }

    private static IReadOnlyList<string> BuildRoutes(FileMetadata metadata)
    {
        var fileName = Path.GetFileNameWithoutExtension(metadata.SourceFilePath);
        var fileNameRoute = BuildFileNameRoute(fileName);
        var routes = new List<string>();

        var sourceRelativeRoute = TryBuildSourceRelativeRoute(metadata);
        if (!string.IsNullOrWhiteSpace(sourceRelativeRoute)
            && !string.Equals(sourceRelativeRoute, fileNameRoute, StringComparison.OrdinalIgnoreCase))
        {
            routes.Add(sourceRelativeRoute);
        }

        routes.Add(fileNameRoute);
        return routes;
    }

    private static string? TryBuildSourceRelativeRoute(FileMetadata metadata)
    {
        if (string.IsNullOrWhiteSpace(metadata.SourceRootPath))
        {
            return null;
        }

        var relativePath = Path.GetRelativePath(metadata.SourceRootPath, metadata.SourceFilePath)
            .Replace(Path.DirectorySeparatorChar, '/')
            .Replace(Path.AltDirectorySeparatorChar, '/');

        if (relativePath.StartsWith("./", StringComparison.Ordinal))
        {
            relativePath = relativePath[2..];
        }

        if (!relativePath.Contains('/', StringComparison.Ordinal))
        {
            return null;
        }

        if (relativePath.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
        {
            relativePath = relativePath[..^".aspx".Length];
        }

        return "/" + relativePath.TrimStart('/');
    }

    private static string BuildFileNameRoute(string fileName)
    {
        var route = "/" + fileName;
        var isHomePage = route is "/Default" or "/default" or "/Index" or "/index";
        return isHomePage ? "/" : route;
    }
}
