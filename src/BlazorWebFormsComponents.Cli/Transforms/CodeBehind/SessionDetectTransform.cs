using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Detects Session["key"] and Cache["key"] patterns, injects [Inject] properties for
/// SessionShim / CacheShim, and generates migration guidance.
/// </summary>
public class SessionDetectTransform : ICodeBehindTransform
{
    public string Name => "SessionDetect";
    public int Order => 400;

    // Literal string key: Session["CartId"]
    private static readonly Regex SessionKeyRegex = new(
        @"Session\[""([^""]*)""\]",
        RegexOptions.Compiled);

    // Broad session access: Session[ (catches both literal and variable keys)
    private static readonly Regex SessionAccessRegex = new(
        @"\bSession\s*\[",
        RegexOptions.Compiled);

    // HttpContext.Current.Session[ — static accessor doesn't work in Blazor
    private static readonly Regex HttpContextCurrentSessionRegex = new(
        @"HttpContext\.Current\.Session\s*\[",
        RegexOptions.Compiled);

    // Literal string key: Cache["ProductList"]
    private static readonly Regex CacheKeyRegex = new(
        @"\bCache\[""([^""]*)""\]",
        RegexOptions.Compiled);

    // Broad cache access: Cache[ (word-boundary prevents matching MemoryCache[)
    private static readonly Regex CacheAccessRegex = new(
        @"\bCache\s*\[",
        RegexOptions.Compiled);

    private static readonly Regex ClassOpenRegex = new(
        @"((?:public|internal|private)\s+(?:partial\s+)?class\s+\w+[^{]*\{)",
        RegexOptions.Compiled);

    private const string TodoEndMarker = "// =============================================================================";
    private const string SessionGuidanceMarker = "// --- Session State Migration ---";
    private const string CacheGuidanceMarker = "// --- Cache Migration ---";

    public string Apply(string content, FileMetadata metadata)
    {
        // Replace HttpContext.Current.Session[ with Session[ BEFORE detection
        if (HttpContextCurrentSessionRegex.IsMatch(content))
        {
            content = HttpContextCurrentSessionRegex.Replace(content, "Session[");
            // Add guidance about static accessor removal
            if (!content.Contains("TODO(bwfc-session-state): HttpContext.Current.Session"))
            {
                var classMatch = ClassOpenRegex.Match(content);
                if (classMatch.Success)
                {
                    var httpContextNote = "\n    // TODO(bwfc-session-state): HttpContext.Current.Session was replaced with Session[].\n"
                        + "    // For non-page classes, inject SessionShim via constructor DI instead of using HttpContext.Current.\n";
                    content = ClassOpenRegex.Replace(content, "$1" + httpContextNote, 1);
                }
            }
        }

        // Detect patterns before any modifications
        var sessionLiteralMatches = SessionKeyRegex.Matches(content);
        var hasSession = SessionAccessRegex.IsMatch(content);

        var cacheLiteralMatches = CacheKeyRegex.Matches(content);
        var hasCache = CacheAccessRegex.IsMatch(content);

        if (!hasSession && !hasCache) return content;

        // Insert session guidance block (idempotent)
        if (hasSession && !content.Contains(SessionGuidanceMarker))
        {
            var sessionKeys = new List<string>();
            foreach (Match m in sessionLiteralMatches)
            {
                var key = m.Groups[1].Value;
                if (!sessionKeys.Contains(key)) sessionKeys.Add(key);
            }

            var sessionBlock = SessionGuidanceMarker + "\n"
                + "// TODO(bwfc-session-state): Session[\"key\"] calls work automatically via SessionShim on WebFormsPageBase.\n"
                + (sessionKeys.Count > 0 ? $"// Session keys found: {string.Join(", ", sessionKeys)}\n" : "")
                + "// Options for long-term replacement:\n"
                + "//   (1) ProtectedSessionStorage (Blazor Server) — persists across circuits\n"
                + "//   (2) Scoped service via DI — lifetime matches user circuit\n"
                + "//   (3) Cascading parameter from a root-level state provider\n"
                + "// See: https://learn.microsoft.com/aspnet/core/blazor/state-management\n\n";

            var lastTodoIdx = content.LastIndexOf(TodoEndMarker);
            if (lastTodoIdx >= 0)
            {
                var insertPos = lastTodoIdx + TodoEndMarker.Length;
                while (insertPos < content.Length && (content[insertPos] == '\r' || content[insertPos] == '\n'))
                    insertPos++;
                content = content[..insertPos] + "\n" + sessionBlock + content[insertPos..];
            }
        }

        // Insert cache guidance block (idempotent)
        if (hasCache && !content.Contains(CacheGuidanceMarker))
        {
            var cacheKeys = new List<string>();
            foreach (Match m in cacheLiteralMatches)
            {
                var key = m.Groups[1].Value;
                if (!cacheKeys.Contains(key)) cacheKeys.Add(key);
            }

            var cacheBlock = CacheGuidanceMarker + "\n"
                + "// TODO(bwfc-session-state): Cache[\"key\"] calls work automatically via CacheShim on WebFormsPageBase.\n"
                + (cacheKeys.Count > 0 ? $"// Cache keys found: {string.Join(", ", cacheKeys)}\n" : "")
                + "// CacheShim wraps IMemoryCache — items are per-server, not distributed.\n"
                + "// For distributed caching, consider IDistributedCache.\n\n";

            // Insert after session block if present, otherwise after TODO header
            var sessionSeeAlso = content.IndexOf("// See: https://learn.microsoft.com/aspnet/core/blazor/state-management");
            if (sessionSeeAlso >= 0)
            {
                var insertPos = content.IndexOf('\n', sessionSeeAlso);
                if (insertPos >= 0)
                {
                    insertPos++;
                    while (insertPos < content.Length && (content[insertPos] == '\r' || content[insertPos] == '\n'))
                        insertPos++;
                    content = content[..insertPos] + cacheBlock + content[insertPos..];
                }
            }
            else
            {
                var lastTodoIdx = content.LastIndexOf(TodoEndMarker);
                if (lastTodoIdx >= 0)
                {
                    var insertPos = lastTodoIdx + TodoEndMarker.Length;
                    while (insertPos < content.Length && (content[insertPos] == '\r' || content[insertPos] == '\n'))
                        insertPos++;
                    content = content[..insertPos] + "\n" + cacheBlock + content[insertPos..];
                }
            }
        }

        // Session and Cache are provided by WebFormsPageBase — no [Inject] needed.

        return content;
    }
}
