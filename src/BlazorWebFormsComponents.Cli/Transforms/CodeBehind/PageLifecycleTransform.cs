using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Converts Web Forms page lifecycle methods to Blazor equivalents:
///   Page_Load → OnInitializedAsync
///   Page_Init → OnInitialized
///   Page_PreRender → OnAfterRenderAsync (with firstRender guard)
/// </summary>
public class PageLifecycleTransform : ICodeBehindTransform
{
    public string Name => "PageLifecycle";
    public int Order => 600;

    // Match any access modifier combination + void + Page_Load (case-insensitive method name)
    private static readonly Regex PageLoadRegex = new(
        @"(?m)([ \t]*)(?:(?:protected|private|public|internal)\s+)?(?:(?:virtual|override|new|static|sealed|abstract)\s+)*void\s+(?i:Page_Load)\s*\(\s*object\s+\w+\s*,\s*EventArgs\s+\w+\s*\)",
        RegexOptions.Compiled);

    private static readonly Regex PageInitRegex = new(
        @"(?m)([ \t]*)(?:(?:protected|private|public|internal)\s+)?(?:(?:virtual|override|new|static|sealed|abstract)\s+)*void\s+(?i:Page_Init)\s*\(\s*object\s+\w+\s*,\s*EventArgs\s+\w+\s*\)",
        RegexOptions.Compiled);

    private static readonly Regex PreRenderRegex = new(
        @"(?m)([ \t]*)(?:(?:protected|private|public|internal)\s+)?(?:(?:virtual|override|new|static|sealed|abstract)\s+)*void\s+(?i:Page_PreRender)\s*\(\s*object\s+\w+\s*,\s*EventArgs\s+\w+\s*\)",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        content = ConvertPageLoad(content);
        content = ConvertPageInit(content);
        content = ConvertPreRender(content);
        return content;
    }

    private static string ConvertPageLoad(string content)
    {
        var match = PageLoadRegex.Match(content);
        if (!match.Success) return content;

        var indent = match.Groups[1].Value;
        var matchStart = match.Index;
        var matchEnd = matchStart + match.Length;

        var newSig = $"{indent}protected override async Task OnInitializedAsync()";
        content = content[..matchStart] + newSig + content[matchEnd..];

        // Find opening brace after signature and inject base call
        var sigEnd = matchStart + newSig.Length;
        var bracePos = sigEnd;
        while (bracePos < content.Length && char.IsWhiteSpace(content[bracePos])) bracePos++;

        if (bracePos < content.Length && content[bracePos] == '{')
        {
            var injection = $"\n{indent}    // TODO(bwfc-lifecycle): Review lifecycle conversion — verify async behavior\n{indent}    await base.OnInitializedAsync();\n";
            content = content[..(bracePos + 1)] + injection + content[(bracePos + 1)..];
        }

        return content;
    }

    private static string ConvertPageInit(string content)
    {
        var match = PageInitRegex.Match(content);
        if (!match.Success) return content;

        var indent = match.Groups[1].Value;
        var matchStart = match.Index;
        var matchEnd = matchStart + match.Length;

        var newSig = $"{indent}protected override void OnInitialized()";
        content = content[..matchStart] + newSig + content[matchEnd..];

        // Find opening brace and inject TODO
        var sigEnd = matchStart + newSig.Length;
        var bracePos = sigEnd;
        while (bracePos < content.Length && char.IsWhiteSpace(content[bracePos])) bracePos++;

        if (bracePos < content.Length && content[bracePos] == '{')
        {
            var injection = $"\n{indent}    // TODO(bwfc-lifecycle): Review lifecycle conversion — verify async behavior\n";
            content = content[..(bracePos + 1)] + injection + content[(bracePos + 1)..];
        }

        return content;
    }

    private static string ConvertPreRender(string content)
    {
        var match = PreRenderRegex.Match(content);
        if (!match.Success) return content;

        var indent = match.Groups[1].Value;
        var matchStart = match.Index;
        var matchEnd = matchStart + match.Length;

        var newSig = $"{indent}protected override async Task OnAfterRenderAsync(bool firstRender)";
        content = content[..matchStart] + newSig + content[matchEnd..];

        // Find opening brace
        var sigEnd = matchStart + newSig.Length;
        var braceStart = sigEnd;
        while (braceStart < content.Length && char.IsWhiteSpace(content[braceStart])) braceStart++;

        if (braceStart < content.Length && content[braceStart] == '{')
        {
            // Brace-count to find matching close brace
            var depth = 1;
            var pos = braceStart + 1;
            while (pos < content.Length && depth > 0)
            {
                if (content[pos] == '{') depth++;
                else if (content[pos] == '}') depth--;
                pos++;
            }

            if (depth == 0)
            {
                var braceEnd = pos - 1;
                var body = content.Substring(braceStart + 1, braceEnd - braceStart - 1);
                var bodyIndent = indent + "    ";

                // Build wrapped body with firstRender guard
                var newBody = $"\n{bodyIndent}// TODO(bwfc-lifecycle): Review lifecycle conversion — verify async behavior";
                newBody += $"\n{bodyIndent}if (firstRender)";
                newBody += $"\n{bodyIndent}{{";

                // Re-indent original body lines by one level
                var bodyLines = body.Split('\n');
                foreach (var line in bodyLines)
                {
                    var trimmed = line.TrimEnd();
                    if (trimmed.Length > 0)
                        newBody += $"\n    {trimmed}";
                }

                newBody += $"\n{bodyIndent}}}";
                newBody += $"\n{indent}";

                content = content[..(braceStart + 1)] + newBody + content[braceEnd..];
            }
        }

        return content;
    }
}
