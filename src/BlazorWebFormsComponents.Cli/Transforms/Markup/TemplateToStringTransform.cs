using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Normalizes simple BWFC template Text bindings like <c>Text="@Item.Quantity"</c>
/// to string expressions so TextBox.Text receives a string value.
/// </summary>
public sealed class TemplateToStringTransform : IMarkupTransform
{
    private static readonly Regex ItemTemplateBlockRegex = new(
        @"<(ItemTemplate|AlternatingItemTemplate|EditItemTemplate|InsertItemTemplate|SelectedItemTemplate)([^>]*)>(?<inner>.*?)</\1>",
        RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex SimpleTextBindingRegex = new(
        """\bText\s*=\s*(?<quote>['"])@(?<context>Item|context)\.(?<property>[A-Za-z_]\w*)(?!\s*\.ToString\s*\()\k<quote>""",
        RegexOptions.Compiled);

    public string Name => "TemplateToString";
    public int Order => 806; // Right after TemplateContext (805)

    public string Apply(string content, FileMetadata metadata)
        => ItemTemplateBlockRegex.Replace(content, match =>
        {
            var rewrittenInner = SimpleTextBindingRegex.Replace(match.Groups["inner"].Value, attributeMatch =>
            {
                var quote = attributeMatch.Groups["quote"].Value;
                var contextName = attributeMatch.Groups["context"].Value;
                var propertyName = attributeMatch.Groups["property"].Value;
                return $"Text={quote}@{contextName}.{propertyName}.ToString(){quote}";
            });

            return $"<{match.Groups[1].Value}{match.Groups[2].Value}>{rewrittenInner}</{match.Groups[1].Value}>";
        });
}
