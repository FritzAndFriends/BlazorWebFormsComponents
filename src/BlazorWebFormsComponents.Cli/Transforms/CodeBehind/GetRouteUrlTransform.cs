using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Converts Page.GetRouteUrl() calls to GetRouteUrl() which is available on WebFormsPageBase.
/// The Page. prefix causes compile errors in Blazor since there is no Page property.
/// </summary>
public class GetRouteUrlTransform : ICodeBehindTransform
{
    public string Name => "GetRouteUrl";
    public int Order => 310;

    // Matches Page.GetRouteUrl( with optional this. prefix and optional whitespace before paren
    private static readonly Regex PageGetRouteUrlRegex = new(
        @"(?:this\.)?Page\.GetRouteUrl\s*\(",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        if (!PageGetRouteUrlRegex.IsMatch(content))
            return content;

        content = PageGetRouteUrlRegex.Replace(content,
            "GetRouteUrl( /* TODO(bwfc-route-url): converted from Page route lookup – ensure page inherits WebFormsPageBase */ ");

        return content;
    }
}
