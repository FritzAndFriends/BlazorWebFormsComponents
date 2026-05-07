using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Propagates a typed GridView's ItemType to child column components so TemplateField and
/// other generic BWFC columns compile against the row item type instead of falling back to object.
/// Object-typed grids are left unchanged.
/// </summary>
public sealed class GridViewColumnItemTypeTransform : IMarkupTransform
{
    public string Name => "GridViewColumnItemType";
    public int Order => 705;

    private static readonly Regex GridViewRegex = new(
        @"(?<open><GridView\b[^>]*\bItemType=""(?<itemType>[^""]+)""[^>]*>)(?<inner>.*?)(?<close></GridView>)",
        RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex ColumnTagRegex = new(
        @"<(?<tag>BoundField|TemplateField|HyperLinkField|ButtonField)\b(?<attrs>[^>]*?)(?<close>\s*/?>)",
        RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex ItemTypeRegex = new(
        @"\bItemType=""(?<itemType>[^""]+)""",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        var previous = string.Empty;
        do
        {
            previous = content;
            content = GridViewRegex.Replace(content, RewriteGridView);
        } while (!string.Equals(previous, content, StringComparison.Ordinal));

        return content;
    }

    private static string RewriteGridView(Match match)
    {
        var gridItemType = match.Groups["itemType"].Value;
        if (string.Equals(gridItemType, "object", StringComparison.Ordinal))
        {
            return match.Value;
        }

        var rewrittenInner = ColumnTagRegex.Replace(match.Groups["inner"].Value, column => RewriteColumn(column, gridItemType));
        return match.Groups["open"].Value + rewrittenInner + match.Groups["close"].Value;
    }

    private static string RewriteColumn(Match match, string gridItemType)
    {
        var attrs = match.Groups["attrs"].Value;
        var close = match.Groups["close"].Value;
        var itemTypeMatch = ItemTypeRegex.Match(attrs);

        if (!itemTypeMatch.Success)
        {
            return $"<{match.Groups["tag"].Value} ItemType=\"{gridItemType}\"{attrs}{close}";
        }

        if (!string.Equals(itemTypeMatch.Groups["itemType"].Value, "object", StringComparison.Ordinal))
        {
            return match.Value;
        }

        var rewrittenAttrs = ItemTypeRegex.Replace(attrs, $"ItemType=\"{gridItemType}\"", 1);
        return $"<{match.Groups["tag"].Value}{rewrittenAttrs}{close}";
    }
}
