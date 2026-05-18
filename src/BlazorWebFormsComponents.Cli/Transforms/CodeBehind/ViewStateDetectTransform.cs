using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Detects ViewState["key"] patterns and generates migration guidance with suggested field declarations.
/// </summary>
public class ViewStateDetectTransform : ICodeBehindTransform
{
    public string Name => "ViewStateDetect";
    public int Order => 410;

    private static readonly Regex ViewStateKeyRegex = new(
        @"ViewState\[""([^""]*)""\]",
        RegexOptions.Compiled);

    private const string TodoEndMarker = "// =============================================================================";

    public string Apply(string content, FileMetadata metadata)
    {
        var matches = ViewStateKeyRegex.Matches(content);
        if (matches.Count == 0) return content;

        // Collect unique keys in order of appearance
        var vsKeys = new List<string>();
        foreach (Match m in matches)
        {
            var key = m.Groups[1].Value;
            if (!vsKeys.Contains(key)) vsKeys.Add(key);
        }

        // Build guidance block with suggested field declarations
        var vsBlock = "// --- ViewState Migration ---\n"
            + "// ViewState is in-memory only in Blazor (does not survive navigation).\n"
            + "// Convert to private fields or [Parameter] properties:\n";

        foreach (var key in vsKeys)
        {
            var fieldName = "_" + char.ToLower(key[0]) + key[1..];
            vsBlock += $"//   private object {fieldName}; // was ViewState[\"{key}\"]\n";
        }

        vsBlock += "// Note: BaseWebFormsComponent.ViewState exists as an [Obsolete] compatibility shim.\n\n";

        // Insert after the TODO header end marker
        var lastTodoIdx = content.LastIndexOf(TodoEndMarker);
        if (lastTodoIdx >= 0)
        {
            var insertPos = lastTodoIdx + TodoEndMarker.Length;
            while (insertPos < content.Length && (content[insertPos] == '\r' || content[insertPos] == '\n'))
                insertPos++;
            content = content[..insertPos] + "\n" + vsBlock + content[insertPos..];
        }

        return content;
    }
}
