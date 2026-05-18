using System.IO;
using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

public class ServerShimTransform : ICodeBehindTransform
{
    private static readonly Regex ServerMapPathRegex = new(
        @"\bServer\.MapPath\s*\(",
        RegexOptions.Compiled);

    private static readonly Regex NonPageMapPathCallRegex = new(
        @"\b(?:HttpContext\.Current\.)?Server\.MapPath\s*\((?<arg>[^)]+)\)",
        RegexOptions.Compiled);

    private static readonly Regex LocalStringAssignmentRegex = new(
        "(?:string|var)\\s+(?<name>[A-Za-z_]\\w*)\\s*=\\s*(?<value>@\"(?:\"\"|[^\"])*\"|\"(?:\\\\.|[^\"])*\")\\s*;",
        RegexOptions.Compiled);

    private static readonly Regex ServerEncodeRegex = new(
        @"\bServer\.(HtmlEncode|HtmlDecode|UrlEncode|UrlDecode)\s*\(",
        RegexOptions.Compiled);

    private static readonly Regex HttpServerUtilityRegex = new(
        @"\bHttpServerUtility\b",
        RegexOptions.Compiled);

    private static readonly Regex ServerTransferRegex = new(
        @"\bServer\.Transfer\s*\(",
        RegexOptions.Compiled);

    private static readonly Regex ServerGetLastErrorRegex = new(
        @"\bServer\.GetLastError\s*\(",
        RegexOptions.Compiled);

    private static readonly Regex ServerClearErrorRegex = new(
        @"\bServer\.ClearError\s*\(",
        RegexOptions.Compiled);

    private static readonly Regex ClassOpenRegex = new(
        @"((?:public|internal|private)\s+(?:partial\s+)?class\s+\w+[^{]*\{)",
        RegexOptions.Compiled);

    private static readonly Regex PageBaseRegex = new(
        @":\s*(?:WebFormsPageBase|ComponentBase|LayoutComponentBase|BaseWebFormsComponent)\b",
        RegexOptions.Compiled);

    private static readonly Regex SystemIoUsingRegex = new(
        @"^using\s+System\.IO;\s*$",
        RegexOptions.Compiled | RegexOptions.Multiline);

    private static readonly Regex SystemWebUsingRegex = new(
        @"^using\s+System\.Web;\s*\r?\n",
        RegexOptions.Compiled | RegexOptions.Multiline);

    private static readonly Regex LastUsingRegex = new(
        @"^using\s+[^;]+;\s*$",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.RightToLeft);

    private const string GuidanceMarker = "// --- Server Utility Migration ---";

    public string Name => "ServerShim";
    public int Order => 330;

    public string Apply(string content, FileMetadata metadata)
    {
        if (ShouldRewriteMapPathCalls(content, metadata))
        {
            content = RewriteNonPageMapPathCalls(content);
        }

        var hasMapPath = ServerMapPathRegex.IsMatch(content);
        var hasEncode = ServerEncodeRegex.IsMatch(content);
        var hasUtility = HttpServerUtilityRegex.IsMatch(content);
        var hasTransfer = ServerTransferRegex.IsMatch(content);
        var hasGetLastError = ServerGetLastErrorRegex.IsMatch(content);
        var hasClearError = ServerClearErrorRegex.IsMatch(content);

        if (!hasMapPath && !hasEncode && !hasUtility && !hasTransfer && !hasGetLastError && !hasClearError)
            return content;

        if (!content.Contains(GuidanceMarker, StringComparison.Ordinal) && ClassOpenRegex.IsMatch(content))
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
            if (hasTransfer) methods.Add("Transfer");
            if (hasGetLastError) methods.Add("GetLastError");
            if (hasClearError) methods.Add("ClearError");

            var guidanceBlock = "\n    " + GuidanceMarker + "\n"
                + "    // TODO(bwfc-server): Server.* calls work automatically via ServerShim on WebFormsPageBase.\n"
                + (methods.Count > 0 ? $"    // Methods found: {string.Join(", ", methods)}\n" : "")
                + "    // For non-page classes, inject ServerShim via DI.\n"
                + (hasMapPath ? "    // MapPath(\"~/path\") maps to IWebHostEnvironment.WebRootPath.\n" : "")
                + (hasTransfer ? "    // Transfer(path) delegates to NavigationManager.NavigateTo(path).\n" : "")
                + ((hasGetLastError || hasClearError) ? "    // GetLastError() currently returns null and ClearError() is a no-op compatibility stub.\n" : "");

            content = ClassOpenRegex.Replace(content, "$1" + guidanceBlock, 1);
        }

        return content;
    }

    private static bool ShouldRewriteMapPathCalls(string content, FileMetadata metadata)
    {
        if (metadata.FileType != FileType.CodeFile)
            return false;

        return !PageBaseRegex.IsMatch(content);
    }

    private static string RewriteNonPageMapPathCalls(string content)
    {
        var localStringLiterals = LocalStringAssignmentRegex.Matches(content)
            .Cast<Match>()
            .GroupBy(match => match.Groups["name"].Value, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => UnwrapStringLiteral(group.Last().Groups["value"].Value),
                StringComparer.Ordinal);

        var replaced = NonPageMapPathCallRegex.Replace(content, match =>
        {
            var argument = match.Groups["arg"].Value.Trim();
            if (argument.Contains("AppContext.BaseDirectory", StringComparison.Ordinal))
                return match.Value;

            var relativePath = TryResolveRelativePath(argument, localStringLiterals);
            return relativePath is null ? match.Value : BuildPathCombine(relativePath);
        });

        if (string.Equals(replaced, content, StringComparison.Ordinal))
            return content;

        replaced = EnsureUsing(replaced, "using System.IO;", SystemIoUsingRegex);

        if (!ContainsLegacySystemWebUsage(replaced))
            replaced = SystemWebUsingRegex.Replace(replaced, string.Empty);

        return replaced;
    }

    private static string? TryResolveRelativePath(string argument, IReadOnlyDictionary<string, string> localStringLiterals)
    {
        if (argument.StartsWith("@\"", StringComparison.Ordinal) || argument.StartsWith("\"", StringComparison.Ordinal))
            return UnwrapStringLiteral(argument);

        return localStringLiterals.TryGetValue(argument, out var path) ? path : null;
    }

    private static string UnwrapStringLiteral(string literal)
    {
        if (literal.StartsWith("@\"", StringComparison.Ordinal) && literal.EndsWith("\"", StringComparison.Ordinal))
            return literal[2..^1].Replace("\"\"", "\"");

        if (literal.StartsWith("\"", StringComparison.Ordinal) && literal.EndsWith("\"", StringComparison.Ordinal))
            return Regex.Unescape(literal[1..^1]);

        return literal;
    }

    private static string BuildPathCombine(string path)
    {
        var normalized = path.Trim().TrimStart('~').TrimStart('/', '\\').Replace('\\', '/');
        var segments = normalized
            .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(segment => $"\"{segment}\"")
            .ToList();

        if (segments.Count == 0)
            return "AppContext.BaseDirectory";

        return $"Path.Combine(AppContext.BaseDirectory, {string.Join(", ", segments)})";
    }

    private static bool ContainsLegacySystemWebUsage(string content)
    {
        return content.Contains("HttpContext.Current", StringComparison.Ordinal)
            || content.Contains("HttpServerUtility", StringComparison.Ordinal)
            || content.Contains("Server.MapPath", StringComparison.Ordinal);
    }

    private static string EnsureUsing(string content, string usingStatement, Regex usingRegex)
    {
        if (usingRegex.IsMatch(content))
            return content;

        var lastUsing = LastUsingRegex.Match(content);
        if (lastUsing.Success)
        {
            var insertAt = lastUsing.Index + lastUsing.Length;
            return content[..insertAt] + Environment.NewLine + usingStatement + content[insertAt..];
        }

        return usingStatement + Environment.NewLine + content;
    }
}
