using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Detects Response.Redirect() calls and strips Page./this. prefixes so they compile
/// against ResponseShim on WebFormsPageBase. ResponseShim handles ~/ prefix stripping
/// and .aspx extension removal automatically — no NavigationManager injection needed.
/// </summary>
public class ResponseRedirectTransform : ICodeBehindTransform
{
    public string Name => "ResponseRedirect";
    public int Order => 300;

    // Pattern 1: Response.Redirect("url", bool)
    private static readonly Regex RedirectLitBoolRegex = new(
        @"Response\.Redirect\(\s*""([^""]*)""\s*,\s*(?:true|false)\s*\)",
        RegexOptions.Compiled);

    // Pattern 2: Response.Redirect("url")
    private static readonly Regex RedirectLitRegex = new(
        @"Response\.Redirect\(\s*""([^""]*)""\s*\)",
        RegexOptions.Compiled);

    // Pattern 3: Response.Redirect(expr, bool)
    private static readonly Regex RedirectExprBoolRegex = new(
        @"Response\.Redirect\(\s*([^,)]+)\s*,\s*(?:true|false)\s*\)",
        RegexOptions.Compiled);

    // Pattern 4: Response.Redirect(expr)
    private static readonly Regex RedirectExprRegex = new(
        @"Response\.Redirect\(\s*([^)]+)\s*\)",
        RegexOptions.Compiled);

    // Strips "Page." or "this." prefix before Response.Redirect
    private static readonly Regex PageOrThisPrefixRegex = new(
        @"(?:Page\.|this\.)(?=Response\.Redirect\s*\()",
        RegexOptions.Compiled);

    private static readonly Regex ClassOpenRegex = new(
        @"((?:public|internal|private)\s+(?:partial\s+)?class\s+\w+[^{]*\{)",
        RegexOptions.Compiled);

    // catch (ThreadAbortException) — dead code after migration (Blazor doesn't throw)
    private static readonly Regex ThreadAbortCatchRegex = new(
        @"catch\s*\(\s*ThreadAbortException\b",
        RegexOptions.Compiled);

    // Response.Redirect(url, true) — endResponse=true silently ignored by ResponseShim
    private static readonly Regex RedirectEndResponseTrueRegex = new(
        @"Response\.Redirect\s*\([^,)]+,\s*true\s*\)",
        RegexOptions.Compiled);

    private const string GuidanceMarker = "// --- Response.Redirect Migration ---";
    private const string ThreadAbortMarker = "// TODO(bwfc-navigation): DEAD CODE";
    private const string EndResponseMarker = "// TODO(bwfc-navigation): endResponse=true";

    public string Apply(string content, FileMetadata metadata)
    {
        // Count redirect calls for guidance (using existing detection regexes)
        var hasRedirect = RedirectLitBoolRegex.IsMatch(content)
            || RedirectLitRegex.IsMatch(content)
            || RedirectExprBoolRegex.IsMatch(content)
            || RedirectExprRegex.IsMatch(content);

        if (!hasRedirect) return content;

        // Pre-detect endResponse=true BEFORE any other processing
        var hasEndResponseTrue = RedirectEndResponseTrueRegex.IsMatch(content);

        // Strip Page.Response.Redirect → Response.Redirect and this.Response.Redirect → Response.Redirect
        if (PageOrThisPrefixRegex.IsMatch(content))
        {
            content = PageOrThisPrefixRegex.Replace(content, "");
        }

        // Emit guidance (idempotent)
        if (!content.Contains(GuidanceMarker) && ClassOpenRegex.IsMatch(content))
        {
            var guidanceBlock = "\n    " + GuidanceMarker + "\n"
                + "    // TODO(bwfc-navigation): Response.Redirect() works via ResponseShim on WebFormsPageBase. Handles ~/ and .aspx automatically.\n"
                + "    // For non-page classes, inject ResponseShim via DI.\n";

            content = ClassOpenRegex.Replace(content, "$1" + guidanceBlock, 1);
        }

        // Detect ThreadAbortException catch blocks — dead code after migration
        if (ThreadAbortCatchRegex.IsMatch(content) && !content.Contains(ThreadAbortMarker))
        {
            content = ThreadAbortCatchRegex.Replace(content, m =>
                m.Value + $"\n            {ThreadAbortMarker} — Blazor does not throw ThreadAbortException on redirect. " +
                "This catch block is dead code after migration. Review and remove if safe.");
        }

        // Warn about Response.Redirect(url, true) endResponse behavior
        if (hasEndResponseTrue && !content.Contains(EndResponseMarker))
        {
            if (ClassOpenRegex.IsMatch(content))
            {
                var warning = $"\n    {EndResponseMarker} is silently ignored by ResponseShim. " +
                    "Code after redirect calls WILL execute (unlike Web Forms where it threw ThreadAbortException).\n";
                content = ClassOpenRegex.Replace(content, "$1" + warning, 1);
            }
        }

        return content;
    }
}
