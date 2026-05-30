using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Normalizes Web Forms template blocks for ASCX/data-control scenarios by:
/// 1) unwrapping <ContentTemplate> wrappers
/// 2) converting common <%# ... %> template bindings to Razor syntax
/// 3) adding Item template context when missing
/// </summary>
public sealed class ContentTemplateUnwrapperTransform : IMarkupTransform
{
    public string Name => "ContentTemplateUnwrapper";
    public int Order => 505; // After expression transforms, before server code blocks

    private static readonly Regex TemplateBlockRegex = new(
        @"<(?<tag>ContentTemplate|HeaderTemplate|ItemTemplate|AlternatingItemTemplate|EditItemTemplate|InsertItemTemplate|SelectedItemTemplate|FooterTemplate|SeparatorTemplate)(?<attrs>[^>]*)>(?<inner>.*?)</\k<tag>>",
        RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

    private static readonly Regex ResidualContentTemplateTagRegex = new(
        @"</?ContentTemplate\b[^>]*>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex EvalFormatRegex = new(
        """<%#\s*Eval\(\s*(?<q>['"])(?<prop>[\w.]+)\k<q>\s*,\s*(?<fq>['"])\{0:(?<fmt>[^}]+)\}\k<fq>\s*\)\s*%>""",
        RegexOptions.Compiled);

    private static readonly Regex EvalRegex = new(
        """<%#\s*Eval\(\s*(?<q>['"])(?<prop>[\w.]+)\k<q>\s*\)\s*%>""",
        RegexOptions.Compiled);

    private static readonly Regex ContainerDataItemMethodRegex = new(
        @"<%#\s*(?<method>[A-Za-z_]\w*(?:\.[A-Za-z_]\w*)*)\s*\(\s*Container\.DataItem\s*\)\s*%>",
        RegexOptions.Compiled);

    private static readonly Regex ItemPropertyRegex = new(
        @"<%#\s*Item\.(?<prop>[A-Za-z_]\w*)\s*%>",
        RegexOptions.Compiled);

    private static readonly Regex ContainerDataItemRegex = new(
        @"<%#\s*Container\.DataItem\s*%>",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        string previous;
        do
        {
            previous = content;
            content = TemplateBlockRegex.Replace(content, match =>
            {
                var tag = match.Groups["tag"].Value;
                var attrs = match.Groups["attrs"].Value;
                var inner = RewriteTemplateBindings(match.Groups["inner"].Value);

                if (string.Equals(tag, "ContentTemplate", StringComparison.OrdinalIgnoreCase))
                {
                    return inner;
                }

                return $"<{tag}{attrs}>{inner}</{tag}>";
            });
        } while (!string.Equals(previous, content, StringComparison.Ordinal));

        // Best-effort cleanup for any remaining wrappers the block matcher did not catch.
        return ResidualContentTemplateTagRegex.Replace(content, string.Empty);
    }

    private static string RewriteTemplateBindings(string inner)
    {
        inner = EvalFormatRegex.Replace(inner, match =>
            $"@Item.{match.Groups["prop"].Value}.ToString(\"{match.Groups["fmt"].Value}\")");
        inner = EvalRegex.Replace(inner, match =>
            $"@Item.{match.Groups["prop"].Value}");
        inner = ContainerDataItemMethodRegex.Replace(inner, "@$1(Item)");
        inner = ItemPropertyRegex.Replace(inner, "@Item.$1");
        inner = ContainerDataItemRegex.Replace(inner, "@Item");
        return inner;
    }
}
