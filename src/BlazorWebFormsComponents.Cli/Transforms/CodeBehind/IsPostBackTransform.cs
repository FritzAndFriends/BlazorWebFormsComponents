using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Unwraps if (!IsPostBack) guards and extracts else-branch bodies into HandlePostBack().
/// Adds TODO for complex else-if patterns.
/// In Blazor, OnInitializedAsync runs only once so the guard is unnecessary.
/// </summary>
public class IsPostBackTransform : ICodeBehindTransform
{
    public string Name => "IsPostBack";
    public int Order => 500;

    // Combined pattern for "if not postback" variants
    private static readonly Regex GuardRegex = new(
        @"(?:if\s*\(\s*!(?:Page\.|this\.)?IsPostBack\s*\)|if\s*\(\s*(?:Page\.|this\.)?IsPostBack\s*==\s*false\s*\)|if\s*\(\s*false\s*==\s*(?:Page\.|this\.)?IsPostBack\s*\))",
        RegexOptions.Compiled);

    private const int MaxIterations = 50;

    public string Apply(string content, FileMetadata metadata)
    {
        var iterations = 0;

        while (GuardRegex.IsMatch(content) && iterations < MaxIterations)
        {
            iterations++;
            var match = GuardRegex.Match(content);
            var matchStart = match.Index;
            var afterMatch = matchStart + match.Length;

            // Skip whitespace to find opening brace
            var braceStart = afterMatch;
            while (braceStart < content.Length && char.IsWhiteSpace(content[braceStart]))
                braceStart++;

            if (braceStart >= content.Length || content[braceStart] != '{')
            {
                // Single-statement guard — add TODO on its own line
                var ls = matchStart;
                while (ls > 0 && content[ls - 1] != '\n') ls--;
                var guardIndent = content[ls..matchStart];
                content = content[..matchStart]
                    + "// TODO(bwfc-ispostback): IsPostBack guard — review for Blazor\n"
                    + guardIndent
                    + content[matchStart..];
                continue;
            }

            // Brace-count to find matching close brace of if body
            var depth = 1;
            var pos = braceStart + 1;
            while (pos < content.Length && depth > 0)
            {
                if (content[pos] == '{') depth++;
                else if (content[pos] == '}') depth--;
                pos++;
            }

            if (depth != 0)
            {
                // Unbalanced braces
                var ls2 = matchStart;
                while (ls2 > 0 && content[ls2 - 1] != '\n') ls2--;
                var parseIndent = content[ls2..matchStart];
                content = content[..matchStart]
                    + "// TODO(bwfc-ispostback): IsPostBack guard — could not parse\n"
                    + parseIndent
                    + content[matchStart..];
                continue;
            }

            var braceEnd = pos - 1; // position of if-body closing brace

            // Check for else clause
            var checkPos = braceEnd + 1;
            while (checkPos < content.Length && char.IsWhiteSpace(content[checkPos]))
                checkPos++;

            var hasElse = (checkPos + 3 < content.Length)
                && content.Substring(checkPos, 4).StartsWith("else", StringComparison.Ordinal)
                && (checkPos + 4 >= content.Length || !char.IsLetterOrDigit(content[checkPos + 4]));

            // Determine indentation of original if statement
            var lineStart = matchStart;
            while (lineStart > 0 && content[lineStart - 1] != '\n') lineStart--;
            var indent = "";
            var leadingText = content[lineStart..matchStart];
            var indentMatch = Regex.Match(leadingText, @"^(\s+)");
            if (indentMatch.Success) indent = indentMatch.Groups[1].Value;

            if (hasElse)
            {
                var afterElse = checkPos + 4;

                // Skip whitespace after "else"
                var elseNext = afterElse;
                while (elseNext < content.Length && char.IsWhiteSpace(content[elseNext]))
                    elseNext++;

                // Detect "else if" — too complex for L1
                var isElseIf = elseNext + 1 < content.Length
                    && content[elseNext] == 'i' && content[elseNext + 1] == 'f'
                    && (elseNext + 2 >= content.Length || !char.IsLetterOrDigit(content[elseNext + 2]));

                if (isElseIf)
                {
                    // Replace guard with non-matching marker to prevent infinite re-matching
                    var todoComment = "// TODO(bwfc-ispostback): IsPostBack guard with else-if clause — too complex for automated extraction.\n"
                        + indent + "// Review: move 'if' body to OnInitializedAsync and 'else if' branches to event handlers or remove.\n"
                        + indent;
                    content = content[..matchStart] + todoComment
                        + "if (true /* BWFC: was !IsPostBack */)" + content[afterMatch..];
                }
                else
                {
                    // else { ... } or single-line else — extract into HandlePostBack()
                    string elseBodyContent;
                    int elseEndPos;

                    if (elseNext < content.Length && content[elseNext] == '{')
                    {
                        // Braced else block
                        var elseBraceStart = elseNext;
                        var elseDepth = 1;
                        var elsePos = elseBraceStart + 1;
                        while (elsePos < content.Length && elseDepth > 0)
                        {
                            if (content[elsePos] == '{') elseDepth++;
                            else if (content[elsePos] == '}') elseDepth--;
                            elsePos++;
                        }

                        if (elseDepth != 0)
                        {
                            // Unbalanced else braces — add TODO
                            content = content[..matchStart]
                                + "// TODO(bwfc-ispostback): IsPostBack guard — could not parse\n"
                                + indent
                                + content[matchStart..];
                            continue;
                        }

                        var elseBraceEnd = elsePos - 1;
                        elseBodyContent = content.Substring(elseBraceStart + 1, elseBraceEnd - elseBraceStart - 1);
                        elseEndPos = elseBraceEnd + 1;
                    }
                    else
                    {
                        // Single-line else — find statement ending at semicolon
                        var stmtEnd = content.IndexOf(';', elseNext);
                        if (stmtEnd < 0)
                        {
                            content = content[..matchStart]
                                + "// TODO(bwfc-ispostback): IsPostBack guard — could not parse\n"
                                + indent
                                + content[matchStart..];
                            continue;
                        }

                        elseBodyContent = content[elseNext..(stmtEnd + 1)];
                        elseEndPos = stmtEnd + 1;
                    }

                    // Extract and trim else body lines for HandlePostBack
                    var elseLines = elseBodyContent.Split('\n')
                        .Select(line => line.Trim())
                        .Where(line => line.Length > 0)
                        .ToArray();

                    // Unwrap the if body (same logic as simple case)
                    var ifBody = content.Substring(braceStart + 1, braceEnd - braceStart - 1);
                    var ifBodyLines = ifBody.Split('\n');
                    var dedentedIfLines = ifBodyLines.Select(line =>
                    {
                        if (line.StartsWith("    ")) return line[4..];
                        if (line.StartsWith("\t")) return line[1..];
                        return line;
                    }).ToArray();
                    var dedentedIfBody = string.Join("\n", dedentedIfLines).Trim();

                    // Build replacement for the if body (unwrapped)
                    var replacement = indent + "// BWFC: IsPostBack guard unwrapped — Blazor re-renders on every state change\n";
                    foreach (var line in dedentedIfBody.Split('\n'))
                    {
                        if (line.Trim().Length > 0)
                            replacement += indent + line + "\n";
                        else
                            replacement += "\n";
                    }

                    // Replace entire if-else construct with unwrapped if body
                    content = content[..matchStart] + replacement.TrimEnd('\n') + content[elseEndPos..];

                    // Compute indentation levels for the new method
                    var memberIndent = indent;
                    if (memberIndent.EndsWith("    "))
                        memberIndent = memberIndent[..^4];
                    else if (memberIndent.EndsWith("\t"))
                        memberIndent = memberIndent[..^1];

                    // Build HandlePostBack method
                    var methodText = "\n\n" + memberIndent + "private void HandlePostBack()\n"
                        + memberIndent + "{\n"
                        + indent + "// TODO(bwfc-ispostback): Wire HandlePostBack() to appropriate Blazor event handlers (e.g., button Click, form Submit)\n";
                    foreach (var line in elseLines)
                    {
                        methodText += indent + line + "\n";
                    }
                    methodText += memberIndent + "}";

                    // Insert HandlePostBack() before the class closing brace
                    var classCloseIndent = memberIndent;
                    if (classCloseIndent.EndsWith("    "))
                        classCloseIndent = classCloseIndent[..^4];
                    else if (classCloseIndent.EndsWith("\t"))
                        classCloseIndent = classCloseIndent[..^1];

                    var insertPos = FindClosingBrace(content, classCloseIndent);
                    if (insertPos >= 0)
                    {
                        content = content[..insertPos] + methodText + "\n" + content[insertPos..];
                    }
                }
            }
            else
            {
                // Simple case — unwrap the guard
                var body = content.Substring(braceStart + 1, braceEnd - braceStart - 1);

                // Dedent: remove one level of leading whitespace (4 spaces or 1 tab) per line
                var bodyLines = body.Split('\n');
                var dedentedLines = bodyLines.Select(line =>
                {
                    if (line.StartsWith("    ")) return line[4..];
                    if (line.StartsWith("\t")) return line[1..];
                    return line;
                }).ToArray();
                var dedentedBody = string.Join("\n", dedentedLines).Trim();

                var replacement = indent + "// BWFC: IsPostBack guard unwrapped — Blazor re-renders on every state change\n";
                foreach (var line in dedentedBody.Split('\n'))
                {
                    if (line.Trim().Length > 0)
                        replacement += indent + line + "\n";
                    else
                        replacement += "\n";
                }

                content = content[..matchStart] + replacement.TrimEnd('\n') + content[(braceEnd + 1)..];
            }
        }

        return content;
    }

    /// <summary>
    /// Finds the last line matching the expected closing brace at a given indentation level.
    /// Returns the string position of the start of that line, or -1 if not found.
    /// </summary>
    private static int FindClosingBrace(string content, string expectedIndent)
    {
        var target = expectedIndent + "}";
        var lines = content.Split('\n');
        var pos = 0;
        var lastMatch = -1;

        for (var i = 0; i < lines.Length; i++)
        {
            if (lines[i].TrimEnd() == target)
            {
                lastMatch = pos;
            }
            pos += lines[i].Length + 1; // +1 for \n separator
        }

        return lastMatch;
    }
}
