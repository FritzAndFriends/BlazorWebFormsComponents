using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Cleans .aspx URL literals in code-behind:
///   "~/SomePage.aspx" → "/SomePage"
///   "~/SomePage.aspx?param=val" → "/SomePage?param=val"
///   Also handles relative .aspx references in NavigateTo calls.
/// </summary>
public class UrlCleanupTransform : ICodeBehindTransform
{
    public string Name => "UrlCleanup";
    public int Order => 900;

    // Pattern 1: "~/SomePage.aspx?query" → "/SomePage?query"
    private static readonly Regex AspxTildeQsRegex = new(
        @"""~/([^""]*?)\.aspx\?([^""]*)""",
        RegexOptions.Compiled);

    // Pattern 2: "~/SomePage.aspx" → "/SomePage"
    private static readonly Regex AspxTildeRegex = new(
        @"""~/([^""]*?)\.aspx""",
        RegexOptions.Compiled);

    // Pattern 3: NavigationManager.NavigateTo("SomePage.aspx?q") → relative + query
    private static readonly Regex AspxRelQsRegex = new(
        @"(NavigationManager\.NavigateTo\(\s*"")([^""~/][^""]*?)\.aspx\?([^""]*)""",
        RegexOptions.Compiled);

    // Pattern 4: NavigationManager.NavigateTo("SomePage.aspx") → relative only
    private static readonly Regex AspxRelRegex = new(
        @"(NavigationManager\.NavigateTo\(\s*"")([^""~/][^""]*?)\.aspx""",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        content = AspxTildeQsRegex.Replace(content, @"""/$1?$2""");
        content = AspxTildeRegex.Replace(content, @"""/$1""");
        content = AspxRelQsRegex.Replace(content, @"$1/$2?$3""");
        content = AspxRelRegex.Replace(content, @"$1/$2""");
        return content;
    }
}
