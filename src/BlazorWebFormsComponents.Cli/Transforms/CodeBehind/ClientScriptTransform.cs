using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Detects Page.ClientScript and ScriptManager code-behind patterns and preserves them
/// for use with the ClientScriptShim. Strips Page./this. prefixes so calls are compatible
/// with the shim API, and emits TODO markers for unsupported patterns.
///
/// Shim-compatible (prefix stripping, calls preserved):
///   - RegisterStartupScript() → strip Page./this. prefix
///   - RegisterClientScriptInclude() → strip Page./this. prefix
///   - RegisterClientScriptBlock() → strip Page./this. prefix
///   - GetPostBackEventReference() → strip Page./this. prefix
///   - ScriptManager.RegisterStartupScript() → convert to ClientScript.RegisterStartupScript()
///   - ScriptManager.GetCurrent(Page) → ScriptManager.GetCurrent(this)
/// </summary>
public class ClientScriptTransform : ICodeBehindTransform
{
    public string Name => "ClientScript";
    public int Order => 850;

    // --- Shim-compatible patterns (strip prefix, preserve calls) ---

    // Strips "Page." or "this." prefix before ClientScript method calls that the shim supports
    private static readonly Regex PageOrThisPrefixRegex = new(
        @"(?:Page\.|this\.)(?=ClientScript\.(?:RegisterStartupScript|RegisterClientScriptInclude|RegisterClientScriptBlock|GetPostBackEventReference)\s*\()",
        RegexOptions.Compiled);

    // Pattern 1b: ScriptManager.RegisterStartupScript(control, type, key, script, bool)
    //   → ClientScript.RegisterStartupScript(type, key, script, bool) — drops the first param
    private static readonly Regex ScriptManagerStartupScriptRegex = new(
        @"ScriptManager\.RegisterStartupScript\s*\(\s*[^,]*,\s*",
        RegexOptions.Compiled);

    // Pattern 5: ScriptManager.GetCurrent(Page) or ScriptManager.GetCurrent(this.Page) → ScriptManager.GetCurrent(this)
    private static readonly Regex ScriptManagerGetCurrentPageRegex = new(
        @"ScriptManager\.GetCurrent\s*\(\s*(?:this\.)?Page\s*\)",
        RegexOptions.Compiled);

    // For injecting ClientScriptShim dependency comment
    private static readonly Regex ClassOpenRegex = new(
        @"((?:public|internal|private)\s+(?:partial\s+)?class\s+\w+[^{]*\{)",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        var hasShimCall = false;
        var hasScriptManagerCall = false;

        // Patterns 1, 2, 3, 4: Strip Page./this. prefix — calls become shim-compatible
        if (PageOrThisPrefixRegex.IsMatch(content))
        {
            content = PageOrThisPrefixRegex.Replace(content, "");
            hasShimCall = true;
        }

        // Pattern 1b: ScriptManager.RegisterStartupScript → ClientScript.RegisterStartupScript (drop first param)
        if (ScriptManagerStartupScriptRegex.IsMatch(content))
        {
            content = ScriptManagerStartupScriptRegex.Replace(content, "ClientScript.RegisterStartupScript(");
            hasShimCall = true;
        }

        // Detect calls that were already "ClientScript.XXX(...)" without prefix — still shim-compatible
        if (!hasShimCall && (content.Contains("ClientScript.RegisterStartupScript") ||
            content.Contains("ClientScript.RegisterClientScriptInclude") ||
            content.Contains("ClientScript.RegisterClientScriptBlock") ||
            content.Contains("ClientScript.GetPostBackEventReference")))
        {
            hasShimCall = true;
        }

        // Pattern 5: ScriptManager.GetCurrent(Page) → ScriptManager.GetCurrent(this)
        if (ScriptManagerGetCurrentPageRegex.IsMatch(content))
        {
            content = ScriptManagerGetCurrentPageRegex.Replace(content, "ScriptManager.GetCurrent(this)");
            hasShimCall = true;
            hasScriptManagerCall = true;
        }

        // Detect ScriptManager.GetCurrent(this) already in correct form
        if (!hasScriptManagerCall && content.Contains("ScriptManager.GetCurrent(this)"))
        {
            hasShimCall = true;
            hasScriptManagerCall = true;
        }

        // Add shim dependency comment when shim-preserving transforms were made
        if (hasShimCall && ClassOpenRegex.IsMatch(content) &&
            !content.Contains("// TODO(bwfc-general): ClientScript calls preserved"))
        {
            var shimComment = hasScriptManagerCall
                ? "\n    // TODO(bwfc-general): ClientScript calls preserved — works via WebFormsPageBase (no injection needed). ScriptManagerShim may need @inject ScriptManagerShim ScriptManager for non-page classes.\n"
                : "\n    // TODO(bwfc-general): ClientScript calls preserved — works via WebFormsPageBase (no injection needed).\n";
            content = ClassOpenRegex.Replace(content, "$1" + shimComment, 1);
        }

        return content;
    }
}
