using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Rewrites direct FindControl("ControlId") lookups to component @ref fields when available.
/// Only rewrites direct calls on the current component (FindControl / this.FindControl),
/// and only when the markup pipeline already created a matching component reference.
/// </summary>
public class FindControlComponentRefTransform : ICodeBehindTransform
{
    public string Name => "FindControlComponentRef";
    public int Order => 230;

    private static readonly Regex FindControlRegex = new(
        @"\b(?:this\.)?FindControl\s*\(\s*""(?<id>\w+)""\s*\)",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        if (metadata.ComponentRefs.Count == 0)
            return content;

        if (metadata.FileType == FileType.Control
            && metadata.AscxDescriptor is { ReferencedControlIds.Count: 0 })
        {
            return content;
        }

        return FindControlRegex.Replace(content, match =>
        {
            var controlId = match.Groups["id"].Value;
            return metadata.ComponentRefs.ContainsKey(controlId)
                ? controlId
                : match.Value;
        });
    }
}
