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

    // Block/inline elements where orphan closing tags commonly appear after transforms
    private static readonly HashSet<string> OrphanCheckElements = new(StringComparer.OrdinalIgnoreCase)
    {
        "p", "div", "span", "td", "tr", "th", "table", "tbody", "thead", "tfoot",
        "li", "ul", "ol", "dl", "dt", "dd", "section", "article", "aside", "header", "footer", "nav", "main"
    };

    // Self-closing non-void HTML elements: <td/>, <div/>, <span/>, <p/>, etc.
    private static readonly Regex SelfClosingNonVoidRegex = new(
        @"<(?<tag>[a-zA-Z][\w-]*)(?<attrs>[^>]*?)\s*/>",
        RegexOptions.Compiled);

    // Unclosed inline formatting: <b>text<b> (should be <b>text</b>)
    private static readonly Regex UnclosedInlineRegex = new(
        @"<(?<tag>b|i|u|s|em|strong|small|mark|sub|sup)(?<attrs>[^>]*)>(?<content>[^<]*)<\k<tag>(?=[>\s])",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // Opening tag: <tagname ...> (not self-closing, not closing)
    private static readonly Regex OpenTagRegex = new(
        @"<(?<tag>[a-zA-Z][\w-]*)(?:\s[^>]*)?>",
        RegexOptions.Compiled);

    // Closing tag: </tagname>
    private static readonly Regex CloseTagRegex = new(
        @"</(?<tag>[a-zA-Z][\w-]*)\s*>",
        RegexOptions.Compiled);

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

        // Remove orphan closing tags (closing tags with no matching opener)
        content = RemoveOrphanClosingTags(content);

        return content;
    }

    private static string RemoveOrphanClosingTags(string content)
    {
        // Count opening and closing tags for elements we care about
        var openCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var closeCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (Match m in OpenTagRegex.Matches(content))
        {
            var tag = m.Groups["tag"].Value;
            if (OrphanCheckElements.Contains(tag))
            {
                openCounts.TryGetValue(tag, out var count);
                openCounts[tag] = count + 1;
            }
        }

        foreach (Match m in CloseTagRegex.Matches(content))
        {
            var tag = m.Groups["tag"].Value;
            if (OrphanCheckElements.Contains(tag))
            {
                closeCounts.TryGetValue(tag, out var count);
                closeCounts[tag] = count + 1;
            }
        }

        // Find tags where closing count exceeds opening count
        var excessTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in closeCounts)
        {
            openCounts.TryGetValue(kvp.Key, out var opens);
            if (kvp.Value > opens)
                excessTags.Add(kvp.Key);
        }

        if (excessTags.Count == 0)
            return content;

        // Remove excess closing tags — process line by line to cleanly remove whole lines
        var lines = content.Split('\n');
        var result = new List<string>(lines.Length);
        var remainingExcess = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var tag in excessTags)
        {
            openCounts.TryGetValue(tag, out var opens);
            remainingExcess[tag] = closeCounts[tag] - opens;
        }

        // Walk lines in reverse to remove trailing orphans first
        for (var i = lines.Length - 1; i >= 0; i--)
        {
            var line = lines[i];
            var trimmed = line.TrimEnd('\r').Trim();
            var closeMatch = CloseTagRegex.Match(trimmed);

            // Only remove if the line contains JUST the orphan closing tag (with optional whitespace)
            if (closeMatch.Success && closeMatch.Value == trimmed)
            {
                var tag = closeMatch.Groups["tag"].Value;
                if (remainingExcess.TryGetValue(tag, out var excess) && excess > 0)
                {
                    remainingExcess[tag] = excess - 1;
                    continue; // Skip this line
                }
            }

            result.Add(lines[i]);
        }

        result.Reverse();
        return string.Join('\n', result);
    }
}
