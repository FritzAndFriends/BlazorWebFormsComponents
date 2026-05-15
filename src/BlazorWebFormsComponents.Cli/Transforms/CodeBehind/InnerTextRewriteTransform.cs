using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Rewrites <c>identifier.InnerText = value</c> and <c>identifier.InnerHtml = value</c>
/// to plain assignment <c>identifier = value</c>.  HTML server controls with <c>runat="server"</c>
/// become simple string-backed fields in Blazor; their <c>InnerText</c>/<c>InnerHtml</c>
/// properties no longer exist.
/// </summary>
public class InnerTextRewriteTransform : ICodeBehindTransform
{
    public string Name => "InnerTextRewrite";
    public int Order => 750; // After EventHandlerSignature (700), before CompileSurfaceStub (850)

    private static readonly Regex InnerTextAccessRegex = new(
        @"(?<prefix>\b[A-Za-z_]\w*)\.(?:InnerText|InnerHtml)\b",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        if (!content.Contains("InnerText", StringComparison.Ordinal) &&
            !content.Contains("InnerHtml", StringComparison.Ordinal))
        {
            return content;
        }

        return InnerTextAccessRegex.Replace(content, "${prefix}");
    }
}
