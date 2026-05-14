using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Late-pass markup cleanup that fixes common HTML issues in generated .razor files:
/// - Self-closing non-void elements: &lt;td/&gt; → &lt;td&gt;&lt;/td&gt;
/// - Unclosed inline formatting tags: &lt;b&gt;text&lt;b&gt; → &lt;b&gt;text&lt;/b&gt;
/// - Orphan closing tags without matching openers
/// These patterns cause Razor compilation errors or broken rendering.
/// </summary>
public class MarkupCleanupTransform : IMarkupTransform
{
    public string Name => "MarkupCleanup";
    public int Order => 950; // Very late — after all other markup transforms

    // HTML void elements that are legitimately self-closing
    private static readonly HashSet<string> VoidElements = new(StringComparer.OrdinalIgnoreCase)
    {
        "area", "base", "br", "col", "embed", "hr", "img", "input",
        "link", "meta", "param", "source", "track", "wbr"
    };

    // Self-closing non-void HTML elements: <td/>, <div/>, <span/>, <p/>, etc.
    private static readonly Regex SelfClosingNonVoidRegex = new(
        @"<(?<tag>[a-zA-Z][\w-]*)(?<attrs>[^>]*?)\s*/>",
        RegexOptions.Compiled);

    // Unclosed inline formatting: <b>text<b> (should be <b>text</b>)
    private static readonly Regex UnclosedInlineRegex = new(
        @"<(?<tag>b|i|u|s|em|strong|small|mark|sub|sup)(?<attrs>[^>]*)>(?<content>[^<]*)<\k<tag>(?=[>\s])",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Apply(string content, FileMetadata metadata)
    {
        // Fix self-closing non-void elements
        content = SelfClosingNonVoidRegex.Replace(content, match =>
        {
            var tag = match.Groups["tag"].Value;
            var attrs = match.Groups["attrs"].Value;

            // Skip void elements — they should be self-closing
            if (VoidElements.Contains(tag))
                return match.Value;

            // Skip Blazor/Razor components (PascalCase)
            if (char.IsUpper(tag[0]))
                return match.Value;

            // Convert to open+close pair
            return $"<{tag}{attrs}></{tag}>";
        });

        // Fix unclosed inline formatting tags: <b>text<b> → <b>text</b>
        content = UnclosedInlineRegex.Replace(content, match =>
        {
            var tag = match.Groups["tag"].Value;
            var attrs = match.Groups["attrs"].Value;
            var innerContent = match.Groups["content"].Value;
            return $"<{tag}{attrs}>{innerContent}</{tag}";
        });

        return content;
    }
}
