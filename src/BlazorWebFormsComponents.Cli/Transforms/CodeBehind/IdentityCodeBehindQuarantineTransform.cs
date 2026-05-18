using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Replaces legacy OWIN-era identity page and control code-behind method bodies with
/// compile-safe stubs once account markup has been rewritten to ASP.NET Core Identity handlers.
/// </summary>
public partial class IdentityCodeBehindQuarantineTransform : ICodeBehindTransform
{
    private static readonly Regex IdentityPathRegex = new(
        @"(?:^|/)Account/(?:Login|Register|OpenAuthProviders|RegisterExternalLogin)(?:\.[^/]*)?$|(?:^|/)Account/Manage(?:[^/]*)?(?:/|(?:\.[^/]*)?$)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex LegacyIdentitySignalRegex = new(
        @"\bGetOwinContext\s*\(|\bMicrosoft\.Owin\b|\bMicrosoft\.AspNet\.Identity(?:\.Owin)?\b|\bIdentityHelper\b|\bApplicationUserManager\b|\bApplicationSignInManager\b|\bSignInStatus\b|\bAuthenticationManager\b|\bGetExternalLoginInfo\b|\bPasswordSignIn\b|\bAuthentication\.Challenge\b",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex LegacyUsingRegex = new(
        @"^\s*using\s+(?:Microsoft\.Owin(?:\.[A-Za-z0-9_.]+)?|Microsoft\.AspNet\.Identity(?:\.Owin)?|Owin)\s*;\s*\r?\n?",
        RegexOptions.Compiled | RegexOptions.Multiline);

    private static readonly Regex MethodRegex = new(
        @"(?m)^(?<indent>[ \t]*)(?<access>public|protected|private|internal)\s+(?<mods>(?:(?:static|virtual|override|async|sealed|new|partial)\s+)*)?(?<returnType>[A-Za-z_][\w<>\[\],?.:\s]*?)\s+(?<name>[A-Za-z_]\w*)\s*\((?<parameters>[^)]*)\)\s*(?<constraints>(?:where\s+[^{\r\n]+\s*)*)\{",
        RegexOptions.Compiled);

    public string Name => "IdentityCodeBehindQuarantine";
    public int Order => 905;

    public string Apply(string content, FileMetadata metadata)
    {
        if (metadata.FileType is not (FileType.Page or FileType.Control))
        {
            return content;
        }

        var isIdentityPath = IsIdentityPath(metadata.SourceFilePath) || IsIdentityPath(metadata.OutputFilePath);
        var signalContent = string.Join("\n", content, metadata.CodeBehindContent ?? string.Empty, metadata.OriginalContent ?? string.Empty);
        var hasLegacySignals = LegacyIdentitySignalRegex.IsMatch(signalContent);

        if (!isIdentityPath && !hasLegacySignals)
        {
            return content;
        }

        if (!hasLegacySignals)
        {
            return content;
        }

        var rewritten = LegacyUsingRegex.Replace(content, string.Empty);
        rewritten = StubMethodBodies(rewritten);
        rewritten = Regex.Replace(rewritten, @"(\r?\n){3,}", Environment.NewLine + Environment.NewLine);

        return rewritten;
    }

    private static bool IsIdentityPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        return IdentityPathRegex.IsMatch(path.Replace('\\', '/'));
    }

    private static string StubMethodBodies(string content)
    {
        var matches = MethodRegex.Matches(content);
        if (matches.Count == 0)
        {
            return content;
        }

        var rewritten = content;
        for (var i = matches.Count - 1; i >= 0; i--)
        {
            var match = matches[i];
            var braceIndex = match.Index + match.Length - 1;
            var closeBraceIndex = FindMatchingBrace(rewritten, braceIndex);
            if (closeBraceIndex < 0)
            {
                continue;
            }

            var indent = match.Groups["indent"].Value;
            var returnType = match.Groups["returnType"].Value.Trim();
            var modifiers = match.Groups["mods"].Value;
            var stubBody = BuildStubBody(indent, returnType, modifiers);
            rewritten = rewritten[..(braceIndex + 1)] + stubBody + rewritten[closeBraceIndex..];
        }

        return rewritten;
    }

    private static string BuildStubBody(string indent, string returnType, string modifiers)
    {
        var normalizedReturnType = Regex.Replace(returnType, @"\s+", " ").Trim();
        var isAsync = modifiers.Contains("async", StringComparison.Ordinal);
        var commentLine = $"{Environment.NewLine}{indent}    // Identity flow handled by generated ASP.NET Core endpoints.{Environment.NewLine}";

        if (string.Equals(normalizedReturnType, "void", StringComparison.Ordinal))
        {
            return commentLine + indent;
        }

        if (isAsync && (string.Equals(normalizedReturnType, "Task", StringComparison.Ordinal)
            || string.Equals(normalizedReturnType, "System.Threading.Tasks.Task", StringComparison.Ordinal)
            || string.Equals(normalizedReturnType, "ValueTask", StringComparison.Ordinal)
            || string.Equals(normalizedReturnType, "System.Threading.Tasks.ValueTask", StringComparison.Ordinal)))
        {
            return $"{Environment.NewLine}{indent}    // Identity flow handled by generated ASP.NET Core endpoints.{Environment.NewLine}{indent}    return;{Environment.NewLine}{indent}";
        }

        return $"{Environment.NewLine}{indent}    // Identity flow handled by generated ASP.NET Core endpoints.{Environment.NewLine}{indent}    return default!;{Environment.NewLine}{indent}";
    }

    private static int FindMatchingBrace(string content, int openBraceIndex)
    {
        var depth = 0;
        for (var i = openBraceIndex; i < content.Length; i++)
        {
            if (content[i] == '{')
            {
                depth++;
            }
            else if (content[i] == '}')
            {
                depth--;
                if (depth == 0)
                {
                    return i;
                }
            }
        }

        return -1;
    }
}
