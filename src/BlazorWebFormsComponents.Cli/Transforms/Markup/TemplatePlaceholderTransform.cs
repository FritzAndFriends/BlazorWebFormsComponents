using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Converts placeholder elements (id containing "Placeholder") inside *Template blocks to @context.
/// </summary>
public class TemplatePlaceholderTransform : IMarkupTransform
{
    public string Name => "TemplatePlaceholder";
    public int Order => 800;

    // Self-closing tags with placeholder ID
    private static readonly Regex SelfClosingRegex = new(
        @"<\w+\s+[^>]*?id\s*=\s*""[^""]*[Pp]laceholder[^""]*""[^>]*/>",
        RegexOptions.Compiled);

    // Open+close tags with placeholder ID (optional whitespace content)
    private static readonly Regex OpenCloseRegex = new(
        @"<(\w+)\s+[^>]*?id\s*=\s*""[^""]*[Pp]laceholder[^""]*""[^>]*>\s*</\1>",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        content = SelfClosingRegex.Replace(content, "@context");
        content = OpenCloseRegex.Replace(content, "@context");
        return content;
    }
}
