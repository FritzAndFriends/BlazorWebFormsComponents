using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Rewrites legacy <c>HttpUtility</c> calls to <c>WebUtility</c> so migrated
/// projects do not depend on the legacy System.Web.HttpUtility package.
/// </summary>
public class HttpUtilityRewriteTransform : ICodeBehindTransform
{
    private static readonly Regex HttpUtilityCallRegex = new(
        @"\b(?:System\.Web\.)?HttpUtility\.(UrlEncode|UrlDecode|HtmlEncode|HtmlDecode)\s*\(",
        RegexOptions.Compiled);

    private static readonly Regex SystemWebUsingRegex = new(
        @"^using\s+System\.Web;\s*\r?\n",
        RegexOptions.Compiled | RegexOptions.Multiline);

    private static readonly string[] KnownSystemWebTypeMarkers =
    [
        "HttpContext",
        "HttpCookie",
        "HttpRequest",
        "HttpResponse",
        "HttpServerUtility",
        "HttpPostedFile",
        "HttpFileCollection",
        "VirtualPathUtility"
    ];

    public string Name => "HttpUtilityRewrite";
    public int Order => 104;

    public string Apply(string content, FileMetadata metadata)
    {
        var rewritten = HttpUtilityCallRegex.Replace(content, match => $"WebUtility.{match.Groups[1].Value}(");
        if (ReferenceEquals(rewritten, content) || rewritten == content)
            return content;

        rewritten = EnsureUsing(rewritten, "System.Net");

        if (CanRemoveSystemWebUsing(rewritten))
            rewritten = SystemWebUsingRegex.Replace(rewritten, string.Empty);

        return rewritten;
    }

    private static bool CanRemoveSystemWebUsing(string content)
    {
        if (!content.Contains("using System.Web;", StringComparison.Ordinal))
            return false;

        if (content.Contains("System.Web.", StringComparison.Ordinal))
            return false;

        if (content.Contains("HttpUtility", StringComparison.Ordinal))
            return false;

        return !KnownSystemWebTypeMarkers.Any(content.Contains);
    }

    private static string EnsureUsing(string content, string @namespace)
    {
        var usingLine = $"using {@namespace};";
        if (content.Contains(usingLine, StringComparison.Ordinal))
            return content;

        var lastUsing = Regex.Match(content, @"^using\s+[^;(\n]+;\s*$", RegexOptions.Multiline | RegexOptions.RightToLeft);
        if (lastUsing.Success)
        {
            var insertAt = lastUsing.Index + lastUsing.Length;
            return content[..insertAt] + Environment.NewLine + usingLine + content[insertAt..];
        }

        return usingLine + Environment.NewLine + content;
    }
}
