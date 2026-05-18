using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Detects Request.Form["key"] patterns in code-behind and emits migration guidance
/// for the BWFC FormShim. On WebFormsPageBase, Request.Form["key"] compiles against
/// the FormShim's indexer — no code change needed. For interactive Blazor Server mode,
/// wrap form markup in &lt;WebFormsForm OnSubmit="SetRequestFormData"&gt;.
/// </summary>
public class RequestFormTransform : ICodeBehindTransform
{
    public string Name => "RequestForm";
    public int Order => 320;

    // Literal key: Request.Form["fieldName"]
    private static readonly Regex FormKeyRegex = new(
        @"Request\.Form\[""([^""]*)""\]",
        RegexOptions.Compiled);

    // Broad access: Request.Form[ (catches variable keys too)
    private static readonly Regex FormAccessRegex = new(
        @"\bRequest\.Form\s*\[",
        RegexOptions.Compiled);

    // Request.Form.AllKeys, Request.Form.Count, etc.
    private static readonly Regex FormMemberRegex = new(
        @"\bRequest\.Form\.(AllKeys|Count|ContainsKey|GetValues)\b",
        RegexOptions.Compiled);

    private static readonly Regex ClassOpenRegex = new(
        @"((?:public|internal|private)\s+(?:partial\s+)?class\s+\w+[^{]*\{)",
        RegexOptions.Compiled);

    private const string GuidanceMarker = "// --- Request.Form Migration ---";

    public string Apply(string content, FileMetadata metadata)
    {
        var hasFormAccess = FormAccessRegex.IsMatch(content);
        var hasFormMember = FormMemberRegex.IsMatch(content);

        if (!hasFormAccess && !hasFormMember) return content;

        // Collect literal keys for guidance
        var literalMatches = FormKeyRegex.Matches(content);
        var keys = new List<string>();
        foreach (Match m in literalMatches)
        {
            var key = m.Groups[1].Value;
            if (!keys.Contains(key)) keys.Add(key);
        }

        // Inject guidance block (idempotent)
        if (!content.Contains(GuidanceMarker) && ClassOpenRegex.IsMatch(content))
        {
            var guidanceBlock = "\n    " + GuidanceMarker + "\n"
                + "    // TODO(bwfc-form): Request.Form calls work automatically via RequestShim on WebFormsPageBase.\n"
                + "    // For interactive mode, wrap your form in <WebFormsForm OnSubmit=\"SetRequestFormData\">.\n"
                + (keys.Count > 0 ? $"    // Form keys found: {string.Join(", ", keys)}\n" : "")
                + "    // For non-page classes, inject RequestShim via DI.\n";

            content = ClassOpenRegex.Replace(content, "$1" + guidanceBlock, 1);
        }

        return content;
    }
}
