using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.SemanticPatterns;

public sealed class MasterContentContractsSemanticPattern : ISemanticPattern
{
    private static readonly Regex ContentPlaceHolderRegex = new(
        @"<ContentPlaceHolder\b",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Id => "pattern-master-content-contracts";
    public int Order => 100;

    public SemanticPatternMatch Match(SemanticPatternContext context)
    {
        if (context.Metadata.FileType == FileType.Master
            && ContentPlaceHolderRegex.IsMatch(context.Markup)
            && !context.Markup.Contains("public RenderFragment? ChildComponents { get; set; }", StringComparison.Ordinal))
        {
            return SemanticPatternMatch.Match("Normalized master shell contract for named content sections.");
        }

        if (context.Metadata.FileType == FileType.Page
            && SemanticPatternMarkupHelpers.TryExtractWrapper(context.Markup, out var wrapper)
            && SemanticPatternMarkupHelpers.HasNamedContentBlocks(wrapper.InnerContent)
            && !wrapper.InnerContent.Contains("<ChildComponents>", StringComparison.Ordinal))
        {
            return SemanticPatternMatch.Match("Grouped page content sections under ChildComponents.");
        }

        return SemanticPatternMatch.NoMatch();
    }

    public SemanticPatternResult Apply(SemanticPatternContext context)
    {
        if (context.Metadata.FileType == FileType.Master)
        {
            var rewritten = SemanticPatternMarkupHelpers.EnsureChildComponentsRenderSlot(context.Markup);
            rewritten = SemanticPatternMarkupHelpers.EnsureChildComponentsParameter(rewritten);
            return new SemanticPatternResult(
                rewritten,
                context.CodeBehind,
                "Added ChildComponents wiring to the generated master shell.");
        }

        if (context.Metadata.FileType == FileType.Page
            && SemanticPatternMarkupHelpers.TryExtractWrapper(context.Markup, out var wrapper))
        {
            var rewrittenInner = SemanticPatternMarkupHelpers.WrapInNamedRegions(wrapper.InnerContent);
            var rewritten = SemanticPatternMarkupHelpers.RebuildWrapper(wrapper, rewrittenInner);
            return new SemanticPatternResult(
                rewritten,
                context.CodeBehind,
                "Wrapped named Content regions under ChildComponents for the generated page shell.");
        }

        return SemanticPatternResult.FromContext(context);
    }
}
