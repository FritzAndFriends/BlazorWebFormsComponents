using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Adds null-conditional access (?.) to @ref component field accesses inside
/// OnInitializedAsync and OnInitialized bodies.  Component @ref fields are null
/// during initialization because Blazor hasn't rendered the component tree yet.
/// </summary>
public class ComponentRefNullSafetyTransform : ICodeBehindTransform
{
    public string Name => "ComponentRefNullSafety";
    public int Order => 605; // Right after PageLifecycleTransform (600)

    // Matches lifecycle methods where @ref fields may be null
    private static readonly Regex LifecycleMethodRegex = new(
        @"(?:override\s+(?:async\s+)?(?:Task|void)\s+(?:OnInitializedAsync|OnInitialized)\s*\()",
        RegexOptions.Compiled);

    // Matches field access like: fieldName.Property (with assignment or method call)
    // Captures: fieldName.Something — but NOT fieldName?.Something (already safe)
    private static readonly string FieldAccessPattern =
        @"(?<!\?)\b{0}\.(?<member>[A-Z]\w*)";

    public string Apply(string content, FileMetadata metadata)
    {
        // Only process if we have known @ref fields
        if (metadata.ComponentRefs.Count == 0)
            return content;

        // Only process if there's an OnInitializedAsync or OnInitialized method
        if (!LifecycleMethodRegex.IsMatch(content))
            return content;

        // Find all lifecycle method bodies
        var lifecycleMatches = LifecycleMethodRegex.Matches(content);
        foreach (Match lifecycleMatch in lifecycleMatches)
        {
            // Find the opening brace of this method
            var bracePos = content.IndexOf('{', lifecycleMatch.Index + lifecycleMatch.Length);
            if (bracePos < 0) continue;

            var closingBrace = FindMatchingBrace(content, bracePos);
            if (closingBrace < 0) continue;

            // Extract the method body
            var bodyStart = bracePos + 1;
            var bodyEnd = closingBrace;
            var body = content[bodyStart..bodyEnd];

            // For each @ref field, add null-conditional access inside this method body
            var modifiedBody = body;
            foreach (var (controlId, _) in metadata.ComponentRefs)
            {
                var pattern = string.Format(FieldAccessPattern, Regex.Escape(controlId));
                var regex = new Regex(pattern, RegexOptions.Compiled);

                // Replace field.Member with field?.Member, but only simple property access
                modifiedBody = regex.Replace(modifiedBody, m =>
                {
                    return $"{controlId}?.{m.Groups["member"].Value}";
                });
            }

            if (!ReferenceEquals(body, modifiedBody) && body != modifiedBody)
            {
                content = content[..bodyStart] + modifiedBody + content[bodyEnd..];
            }
        }

        return content;
    }

    private static int FindMatchingBrace(string content, int openBracePos)
    {
        var depth = 1;
        var inString = false;
        var inVerbatimString = false;
        var inSingleLineComment = false;
        var inMultiLineComment = false;

        for (var i = openBracePos + 1; i < content.Length; i++)
        {
            var c = content[i];
            var next = i + 1 < content.Length ? content[i + 1] : '\0';

            if (inSingleLineComment) { if (c == '\n') inSingleLineComment = false; continue; }
            if (inMultiLineComment) { if (c == '*' && next == '/') { inMultiLineComment = false; i++; } continue; }
            if (inVerbatimString) { if (c == '"' && next == '"') { i++; continue; } if (c == '"') inVerbatimString = false; continue; }
            if (inString) { if (c == '\\') { i++; continue; } if (c == '"') inString = false; continue; }
            if (c == '/' && next == '/') { inSingleLineComment = true; i++; continue; }
            if (c == '/' && next == '*') { inMultiLineComment = true; i++; continue; }
            if (c == '@' && next == '"') { inVerbatimString = true; i++; continue; }
            if (c == '"') { inString = true; continue; }

            if (c == '{') depth++;
            if (c == '}') { depth--; if (depth == 0) return i; }
        }
        return -1;
    }
}
