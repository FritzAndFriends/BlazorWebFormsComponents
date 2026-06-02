using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Removes duplicate [Parameter] properties when multiple case-insensitive matches
/// exist for the same route token declared in the paired .razor file.
/// </summary>
public sealed class DuplicateRouteParameterTransform : ICodeBehindTransform
{
    private static readonly Regex PageRouteRegex = new(
        "@page\\s+\"[^\"]*\\{(?<param>\\w+)(?::(?<constraint>\\w+))?\\}[^\"]*\"",
        RegexOptions.Compiled);

    private static readonly Regex ParameterPropertyRegex = new(
        @"(?<block>(?:^[ \t]*\[[^\]]+\][ \t]*(?:\r?\n)?)+[ \t]*(?:public|protected|private|internal)\s+[^\r\n{;=]+\s+(?<name>\w+)\s*\{\s*get;\s*set;\s*\}[ \t]*(?:\r?\n)?)",
        RegexOptions.Compiled | RegexOptions.Multiline);

    public string Name => "DuplicateRouteParameter";
    public int Order => 137; // Right after RouteDataParameterPromotion (136)

    public string Apply(string content, FileMetadata metadata)
    {
        if (metadata.FileType != FileType.Page)
            return content;

        var markup = ReadMarkup(metadata);
        if (string.IsNullOrWhiteSpace(markup))
            return content;

        var routeParams = ExtractRouteParameters(markup);
        if (routeParams.Count == 0)
            return content;

        var parameterMatches = ParameterPropertyRegex.Matches(content)
            .Cast<Match>()
            .ToList();
        if (parameterMatches.Count < 2)
            return content;

        var removals = new List<(int Index, int Length)>();

        foreach (var routeParam in routeParams)
        {
            var duplicates = parameterMatches
                .Where(m => routeParam.Equals(m.Groups["name"].Value, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (duplicates.Count <= 1)
                continue;

            var keep = duplicates.FirstOrDefault(m => string.Equals(m.Groups["name"].Value, routeParam, StringComparison.Ordinal))
                ?? duplicates[0];

            foreach (var duplicate in duplicates)
            {
                if (duplicate.Index == keep.Index && duplicate.Length == keep.Length)
                    continue;

                removals.Add((duplicate.Index, duplicate.Length));
            }
        }

        if (removals.Count == 0)
            return content;

        foreach (var removal in removals
            .Distinct()
            .OrderByDescending(r => r.Index))
        {
            content = content.Remove(removal.Index, removal.Length);
        }

        return content;
    }

    private static string? ReadMarkup(FileMetadata metadata)
    {
        var markupPath = metadata.OutputFilePath.EndsWith(".razor.cs", StringComparison.OrdinalIgnoreCase)
            ? metadata.OutputFilePath[..^3]
            : null;

        if (!string.IsNullOrWhiteSpace(markupPath) && File.Exists(markupPath))
            return File.ReadAllText(markupPath);

        return metadata.MarkupContent;
    }

    private static HashSet<string> ExtractRouteParameters(string markup)
    {
        var routeParams = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (Match match in PageRouteRegex.Matches(markup))
        {
            routeParams.Add(match.Groups["param"].Value);
        }

        return routeParams;
    }
}
