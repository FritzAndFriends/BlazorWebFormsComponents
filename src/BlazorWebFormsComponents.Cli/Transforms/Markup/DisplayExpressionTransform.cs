using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Normalizes Web Forms display expressions before broader expression handling.
/// Converts HTML-encoded display blocks and broken generated Razor output into
/// idiomatic Razor expressions.
/// </summary>
public class DisplayExpressionTransform : IMarkupTransform
{
    private static readonly Regex DisplayExpressionRegex = new(
        @"<%#:(?!\s*(?:Bind|Eval)\()\s*(.*?)\s*%>",
        RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex ColonEqualsExpressionRegex = new(
        @"<%=:\s*(.*?)\s*%>",
        RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex BrokenRazorDisplayRegex = new(
        @"@\(\s*:\s*(.*?)\s*\)",
        RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex SimpleExpressionRegex = new(
        @"^[A-Za-z_][A-Za-z0-9_]*(\.[A-Za-z_][A-Za-z0-9_]*)*$",
        RegexOptions.Compiled);

    public string Name => "DisplayExpression";
    public int Order => 490;

    public string Apply(string content, FileMetadata metadata)
    {
        content = DisplayExpressionRegex.Replace(content, match => FormatExpression(match.Groups[1].Value));
        content = ColonEqualsExpressionRegex.Replace(content, match => FormatExpression(match.Groups[1].Value));
        content = BrokenRazorDisplayRegex.Replace(content, match => FormatExpression(match.Groups[1].Value));
        return content;
    }

    private static string FormatExpression(string expression)
    {
        var expr = expression.Trim();
        return IsSimpleExpression(expr) ? "@" + expr : "@(" + expr + ")";
    }

    private static bool IsSimpleExpression(string expr)
    {
        return SimpleExpressionRegex.IsMatch(expr);
    }
}
