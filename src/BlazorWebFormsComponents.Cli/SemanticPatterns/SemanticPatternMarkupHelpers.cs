using System.Text;
using System.Text.RegularExpressions;

namespace BlazorWebFormsComponents.Cli.SemanticPatterns;

internal static partial class SemanticPatternMarkupHelpers
{
    private static readonly Regex WrapperOpenRegex = new(
        @"(?m)^(?<indent>\s*)<(?<name>[A-Z][A-Za-z0-9_]*)>\s*$",
        RegexOptions.Compiled);

    private static readonly Regex ContentBlockRegex = new(
        @"<Content\b[^>]*>[\s\S]*?</Content>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex ChildContentBlockRegex = new(
        @"<ChildContent>(?<inner>[\s\S]*?)</ChildContent>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static bool TryExtractWrapper(string markup, out WrapperMatch wrapper)
    {
        foreach (Match openMatch in WrapperOpenRegex.Matches(markup))
        {
            var name = openMatch.Groups["name"].Value;
            var closeTag = $"</{name}>";
            var closeIndex = markup.LastIndexOf(closeTag, StringComparison.Ordinal);
            if (closeIndex <= openMatch.Index)
            {
                continue;
            }

            var suffix = markup[(closeIndex + closeTag.Length)..];
            if (!string.IsNullOrWhiteSpace(suffix))
            {
                continue;
            }

            wrapper = new WrapperMatch(
                name,
                markup[..openMatch.Index],
                markup.Substring(openMatch.Index, openMatch.Length),
                markup.Substring(openMatch.Index + openMatch.Length, closeIndex - (openMatch.Index + openMatch.Length)),
                closeTag,
                suffix);

            return true;
        }

        wrapper = default;
        return false;
    }

    public static string WrapInNamedRegions(string innerContent)
    {
        var contentMatches = ContentBlockRegex.Matches(innerContent);
        if (contentMatches.Count == 0)
        {
            return innerContent;
        }

        var namedSections = string.Join("\n", contentMatches.Select(m => m.Value.Trim()));
        var remainder = ContentBlockRegex.Replace(innerContent, string.Empty).Trim();
        var builder = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(remainder))
        {
            builder.AppendLine("<ChildContent>");
            builder.AppendLine(Indent(remainder.Trim(), "    "));
            builder.AppendLine("</ChildContent>");
        }

        builder.AppendLine("<ChildComponents>");
        builder.AppendLine(Indent(namedSections, "    "));
        builder.AppendLine("</ChildComponents>");

        return builder.ToString().TrimEnd();
    }

    public static bool HasNamedContentBlocks(string markup) =>
        ContentBlockRegex.IsMatch(markup);

    public static string EnsureChildComponentsRenderSlot(string markup)
    {
        if (markup.Contains("@ChildComponents", StringComparison.Ordinal))
        {
            return markup;
        }

        var match = ChildContentBlockRegex.Match(markup);
        if (!match.Success)
        {
            return markup;
        }

        var inner = match.Groups["inner"].Value.Trim('\r', '\n');
        var rewrittenInner = $"    @ChildComponents\n{inner}";
        return markup[..match.Index]
               + $"<ChildContent>\n{rewrittenInner}\n</ChildContent>"
               + markup[(match.Index + match.Length)..];
    }

    public static string EnsureChildComponentsParameter(string markup)
    {
        if (markup.Contains("public RenderFragment? ChildComponents { get; set; }", StringComparison.Ordinal))
        {
            return markup;
        }

        var parameterBlock = """
    [Parameter]
    public RenderFragment? ChildComponents { get; set; }
""";

        var codeIndex = markup.LastIndexOf("@code", StringComparison.Ordinal);
        if (codeIndex < 0)
        {
            return $"{markup.TrimEnd()}\n\n@code {{\n{parameterBlock}\n}}";
        }

        var openBraceIndex = markup.IndexOf('{', codeIndex);
        var closeBraceIndex = markup.LastIndexOf('}');
        if (openBraceIndex < 0 || closeBraceIndex <= openBraceIndex)
        {
            return $"{markup.TrimEnd()}\n\n@code {{\n{parameterBlock}\n}}";
        }

        var body = markup.Substring(openBraceIndex + 1, closeBraceIndex - openBraceIndex - 1).Trim('\r', '\n');
        var rewrittenBody = string.IsNullOrWhiteSpace(body)
            ? parameterBlock
            : $"{body}\n\n{parameterBlock}";

        return markup[..codeIndex]
               + $"@code {{\n{rewrittenBody}\n}}"
               + markup[(closeBraceIndex + 1)..];
    }

    public static string RebuildWrapper(WrapperMatch wrapper, string rewrittenInner) =>
        $"{wrapper.Prefix}{wrapper.OpenTag}\n{Indent(rewrittenInner.Trim(), "    ")}\n{wrapper.CloseTag}{wrapper.Suffix}";

    public static string Indent(string content, string indent)
    {
        var normalized = content.Replace("\r\n", "\n");
        var lines = normalized.Split('\n');
        return string.Join("\n", lines.Select(line => string.IsNullOrWhiteSpace(line) ? string.Empty : $"{indent}{line.TrimEnd()}"));
    }

    public readonly record struct WrapperMatch(
        string Name,
        string Prefix,
        string OpenTag,
        string InnerContent,
        string CloseTag,
        string Suffix);
}
