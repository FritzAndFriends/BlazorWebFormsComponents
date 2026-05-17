using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Flags legacy static WebMethod endpoints for API migration and normalizes
/// Page_PreRenderComplete so PageLifecycleTransform can convert it.
/// </summary>
public class WebMethodAnnotationTransform : ICodeBehindTransform
{
    public string Name => "WebMethodAnnotation";
    public int Order => 550;

    private static readonly Regex WebMethodRegex = new(
        @"(?m)^(?<indent>\s*)(\[(?:System\.Web\.Services\.)?WebMethod\])",
        RegexOptions.Compiled);

    private static readonly Regex PreRenderCompleteRegex = new(
        @"\bPage_PreRenderComplete\b",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        content = WebMethodRegex.Replace(content, m =>
            $"{m.Groups["indent"].Value}// TODO(bwfc-webmethod): Migrate legacy static WebMethod endpoint to a Razor component callback or Minimal API.\n{m.Value}");

        content = PreRenderCompleteRegex.Replace(content, "Page_PreRender");
        return content;
    }
}
