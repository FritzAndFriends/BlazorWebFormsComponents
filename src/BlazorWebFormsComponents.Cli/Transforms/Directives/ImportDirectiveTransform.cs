using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Directives;

/// <summary>
/// Converts &lt;%@ Import Namespace="..." %&gt; to @using statements.
/// </summary>
public class ImportDirectiveTransform : IMarkupTransform
{
    public string Name => "ImportDirective";
    public int Order => 200;

    private static readonly Regex ImportRegex = new(
        @"<%@\s*Import\s+Namespace=""([^""]+)""\s*%>\s*\r?\n?",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        return ImportRegex.Replace(content, m => $"@using {m.Groups[1].Value}\n");
    }
}
