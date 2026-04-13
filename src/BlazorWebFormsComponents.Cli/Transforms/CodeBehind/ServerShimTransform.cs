using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Detects Server.MapPath(), Server.HtmlEncode(), Server.HtmlDecode(),
/// Server.UrlEncode(), and Server.UrlDecode() patterns and emits migration
/// guidance for the BWFC ServerShim. On WebFormsPageBase, these methods
/// compile against the ServerShim — no code change needed.
/// </summary>
public class ServerShimTransform : ICodeBehindTransform
{
    public string Name => "ServerShim";
    public int Order => 330;

    // Server.MapPath("~/path") or Server.MapPath(expr)
    private static readonly Regex ServerMapPathRegex = new(
        @"\bServer\.MapPath\s*\(",
        RegexOptions.Compiled);

    // Server.HtmlEncode, Server.HtmlDecode, Server.UrlEncode, Server.UrlDecode
    private static readonly Regex ServerEncodeRegex = new(
        @"\bServer\.(HtmlEncode|HtmlDecode|UrlEncode|UrlDecode)\s*\(",
        RegexOptions.Compiled);

    // HttpServerUtility references (sometimes stored as a variable)
    private static readonly Regex HttpServerUtilityRegex = new(
        @"\bHttpServerUtility\b",
        RegexOptions.Compiled);

    // Server.Transfer("page.aspx") — no shim, architecturally incompatible with Blazor
    private static readonly Regex ServerTransferRegex = new(
        @"\bServer\.Transfer\s*\(",
        RegexOptions.Compiled);

    // Server.GetLastError() — no shim, use middleware-based error handling
    private static readonly Regex ServerGetLastErrorRegex = new(
        @"\bServer\.GetLastError\s*\(",
        RegexOptions.Compiled);

    // Server.ClearError() — no shim, use middleware-based error handling
    private static readonly Regex ServerClearErrorRegex = new(
        @"\bServer\.ClearError\s*\(",
        RegexOptions.Compiled);

    private static readonly Regex ClassOpenRegex = new(
        @"((?:public|internal|private)\s+(?:partial\s+)?class\s+\w+[^{]*\{)",
        RegexOptions.Compiled);

    private const string GuidanceMarker = "// --- Server Utility Migration ---";

    public string Apply(string content, FileMetadata metadata)
    {
        var hasMapPath = ServerMapPathRegex.IsMatch(content);
        var hasEncode = ServerEncodeRegex.IsMatch(content);
        var hasUtility = HttpServerUtilityRegex.IsMatch(content);
        var hasTransfer = ServerTransferRegex.IsMatch(content);
        var hasGetLastError = ServerGetLastErrorRegex.IsMatch(content);
        var hasClearError = ServerClearErrorRegex.IsMatch(content);

        if (!hasMapPath && !hasEncode && !hasUtility && !hasTransfer && !hasGetLastError && !hasClearError) return content;

        // Build guidance
        if (!content.Contains(GuidanceMarker) && ClassOpenRegex.IsMatch(content))
        {
            var methods = new List<string>();
            if (hasMapPath) methods.Add("MapPath");
            if (hasEncode)
            {
                foreach (Match m in ServerEncodeRegex.Matches(content))
                {
                    var method = m.Groups[1].Value;
                    if (!methods.Contains(method)) methods.Add(method);
                }
            }

            var guidanceBlock = "\n    " + GuidanceMarker + "\n"
                + "    // TODO(bwfc-server): Server.* calls work automatically via ServerShim on WebFormsPageBase.\n"
                + (methods.Count > 0 ? $"    // Methods found: {string.Join(", ", methods)}\n" : "")
                + "    // For non-page classes, inject ServerShim via DI.\n"
                + (hasMapPath ? "    // MapPath(\"~/path\") maps to IWebHostEnvironment.WebRootPath.\n" : "");

            content = ClassOpenRegex.Replace(content, "$1" + guidanceBlock, 1);
        }

        // Emit manual-rewrite TODOs for unsupported Server methods
        if (hasTransfer && !content.Contains("TODO(bwfc-server): Server.Transfer"))
        {
            content = ServerTransferRegex.Replace(content, m =>
                "Server.Transfer( /* TODO(bwfc-server): Server.Transfer has NO SHIM — architecturally incompatible with Blazor. " +
                "Use NavigationManager.NavigateTo() instead. Server.Transfer does server-side URL rewriting which doesn't exist in Blazor. */ ");
        }

        if (hasGetLastError && !content.Contains("TODO(bwfc-server): Server.GetLastError"))
        {
            content = ServerGetLastErrorRegex.Replace(content, m =>
                "Server.GetLastError( /* TODO(bwfc-server): Server.GetLastError has NO SHIM. " +
                "Use ILogger and middleware-based error handling (app.UseExceptionHandler). */ ");
        }

        if (hasClearError && !content.Contains("TODO(bwfc-server): Server.ClearError"))
        {
            content = ServerClearErrorRegex.Replace(content, m =>
                "Server.ClearError( /* TODO(bwfc-server): Server.ClearError has NO SHIM. " +
                "Error clearing is handled by middleware in ASP.NET Core. */ ");
        }

        return content;
    }
}
