using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Normalizes Web Forms display expressions before broader expression handling.
/// Converts HTML-encoded display blocks and broken generated Razor output into
/// standard Razor <c>@(...)</c> expressions.
/// </summary>
public class DisplayExpressionTransform : IMarkupTransform
{
    private static readonly Regex DisplayExpressionRegex = new(
        @"<%#:(?!\s*(?:Bind|Eval|String\.Format)\()\s*(.*?)\s*%>",
        RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex ColonEqualsExpressionRegex = new(
        @"<%=:\s*(.*?)\s*%>",
        RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex BrokenRazorDisplayRegex = new(
        @"@\(\s*:\s*(.*?)\s*\)",
        RegexOptions.Compiled | RegexOptions.Singleline);

    public string Name => "DisplayExpression";
    public int Order => 490;

    public string Apply(string content, FileMetadata metadata)
    {
        content = DisplayExpressionRegex.Replace(content, match => $"@({match.Groups[1].Value.Trim()})");
        content = ColonEqualsExpressionRegex.Replace(content, match => $"@({match.Groups[1].Value.Trim()})");
        content = BrokenRazorDisplayRegex.Replace(content, match => $"@({match.Groups[1].Value.Trim()})");
        return content;
    }
}
