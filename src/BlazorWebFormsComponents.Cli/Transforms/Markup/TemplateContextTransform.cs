using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Adds Context="Item" to typed item templates and rewrites generated @context references
/// inside those templates to @Item so migrated BWFC data controls preserve Web Forms naming.
/// </summary>
public sealed class TemplateContextTransform : IMarkupTransform
{
    public string Name => "TemplateContext";
    public int Order => 805;

    private static readonly Regex ItemTemplateBlockRegex = new(
        @"<(ItemTemplate|AlternatingItemTemplate|EditItemTemplate|InsertItemTemplate|SelectedItemTemplate)(?![^>]*\bContext=)([^>]*)>(.*?)</\1>",
        RegexOptions.Compiled | RegexOptions.Singleline);

    public string Apply(string content, FileMetadata metadata)
    {
        return ItemTemplateBlockRegex.Replace(content, static match =>
        {
            var rewrittenInner = match.Groups[3].Value
                .Replace("@context", "@Item", StringComparison.Ordinal)
                .Replace("context.", "Item.", StringComparison.Ordinal);

            return $"<{match.Groups[1].Value} Context=\"Item\"{match.Groups[2].Value}>{rewrittenInner}</{match.Groups[1].Value}>";
        });
    }
}
