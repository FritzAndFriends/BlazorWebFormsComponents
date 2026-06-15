using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Detects and comments out code blocks that use Web Forms control-tree APIs
/// that don't exist in Blazor (Table.Rows, TableRow.Cells, Controls[] collection).
/// These patterns represent programmatic UI manipulation that has no direct Blazor equivalent.
/// </summary>
public class DeadControlTreeCodeTransform : ICodeBehindTransform
{
    public string Name => "DeadControlTreeCode";
    public int Order => 710; // After EventHandlerSignature (700)

    // Matches lines that access .Rows, .Cells, or .Controls[] on Web Forms table types
    private static readonly Regex ControlTreeAccessRegex = new(
        @"\.(Rows|Cells|Controls)\s*[\[\.]",
        RegexOptions.Compiled);

    // Matches foreach with TableRow
    private static readonly Regex TableRowForeachRegex = new(
        @"foreach\s*\(\s*TableRow\s+",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        if (!ControlTreeAccessRegex.IsMatch(content) && !TableRowForeachRegex.IsMatch(content))
            return content;

        var lines = content.Split('\n');
        var result = new List<string>();
        var inDeadBlock = false;
        var braceDepth = 0;
        var blockStartBraceDepth = 0;
        var commentedOut = false;

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var trimmed = line.TrimStart();

            // Detect start of a dead code block: foreach (TableRow ...) or line with .Rows[
            if (!inDeadBlock && (TableRowForeachRegex.IsMatch(line) ||
                (trimmed.StartsWith("foreach") && i + 1 < lines.Length && ControlTreeAccessRegex.IsMatch(lines[i + 1]))))
            {
                inDeadBlock = true;
                blockStartBraceDepth = braceDepth;
                result.Add(line.Replace(trimmed, $"// TODO(bwfc-control-tree): Blazor does not support programmatic control-tree access. Rewrite using bound properties."));
                result.Add(line.Substring(0, line.Length - trimmed.Length) + "// " + trimmed);
                commentedOut = true;

                // Track braces on this line
                braceDepth += line.Count(c => c == '{') - line.Count(c => c == '}');
                continue;
            }

            if (inDeadBlock)
            {
                braceDepth += line.Count(c => c == '{') - line.Count(c => c == '}');
                result.Add(line.Substring(0, line.Length - trimmed.Length) + "// " + trimmed);

                // End of block: when we return to the same brace depth we started
                if (braceDepth <= blockStartBraceDepth)
                {
                    inDeadBlock = false;
                }
                continue;
            }

            braceDepth += line.Count(c => c == '{') - line.Count(c => c == '}');
            result.Add(line);
        }

        if (!commentedOut)
            return content;

        return string.Join('\n', result);
    }
}
