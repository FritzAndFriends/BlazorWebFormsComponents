using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Adds explicit template contexts for BWFC data-control fragments so generated markup stays
/// structurally valid and readable for item, group, and layout templates.
/// </summary>
public sealed class TemplateContextTransform : IMarkupTransform
{
    public string Name => "TemplateContext";
    public int Order => 805;

    private static readonly Regex ItemTemplateBlockRegex = new(
        @"<(ItemTemplate|AlternatingItemTemplate|EditItemTemplate|InsertItemTemplate|SelectedItemTemplate)(?![^>]*\bContext=)([^>]*)>(.*?)</\1>",
        RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex ListViewPlaceholderTemplateBlockRegex = new(
        @"<(GroupTemplate|LayoutTemplate)(?![^>]*\bContext=)([^>]*)>(.*?)</\1>",
        RegexOptions.Compiled | RegexOptions.Singleline);

    public string Apply(string content, FileMetadata metadata)
    {
        content = ItemTemplateBlockRegex.Replace(content, static match =>
        {
            var rewrittenInner = match.Groups[3].Value
                .Replace("@context", "@Item", StringComparison.Ordinal)
                .Replace("context.", "Item.", StringComparison.Ordinal);

            return $"<{match.Groups[1].Value} Context=\"Item\"{match.Groups[2].Value}>{rewrittenInner}</{match.Groups[1].Value}>";
        });

        return ListViewPlaceholderTemplateBlockRegex.Replace(content, static match =>
        {
            var contextName = string.Equals(match.Groups[1].Value, "GroupTemplate", StringComparison.Ordinal)
                ? "items"
                : "groups";
            var rewrittenInner = match.Groups[3].Value
                .Replace("@context", $"@{contextName}", StringComparison.Ordinal);

            return $"<{match.Groups[1].Value} Context=\"{contextName}\"{match.Groups[2].Value}>{rewrittenInner}</{match.Groups[1].Value}>";
        });
    }
}
