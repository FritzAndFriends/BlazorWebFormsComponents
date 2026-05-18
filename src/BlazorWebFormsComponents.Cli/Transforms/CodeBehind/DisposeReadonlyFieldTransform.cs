using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Removes null-assignment lines inside Dispose() methods for readonly fields.
/// When DbContextInstantiationTransform converts a field to readonly (for constructor injection),
/// any existing Dispose() method that sets the field to null causes CS0191.
/// </summary>
public class DisposeReadonlyFieldTransform : ICodeBehindTransform
{
    public string Name => "DisposeReadonlyField";
    public int Order => 910; // Late pass — after all DI transforms

    // Matches readonly field declarations: private readonly Type _name;
    private static readonly Regex ReadonlyFieldRegex = new(
        @"private\s+readonly\s+\w[\w<>,\s]*\s+(?<name>_?\w+)\s*[;=]",
        RegexOptions.Compiled);

    // Matches Dispose method body
    private static readonly Regex DisposeMethodRegex = new(
        @"(?:void\s+Dispose\s*\([^)]*\)\s*\{)",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        if (!content.Contains("Dispose", StringComparison.Ordinal))
            return content;

        if (!content.Contains("readonly", StringComparison.Ordinal))
            return content;

        // Collect readonly field names
        var readonlyFields = new HashSet<string>(StringComparer.Ordinal);
        foreach (Match m in ReadonlyFieldRegex.Matches(content))
        {
            readonlyFields.Add(m.Groups["name"].Value);
        }

        if (readonlyFields.Count == 0)
            return content;

        // Find Dispose method and remove null assignments for readonly fields
        var disposeMatch = DisposeMethodRegex.Match(content);
        if (!disposeMatch.Success)
            return content;

        // Find the body of the Dispose method
        var braceStart = content.IndexOf('{', disposeMatch.Index);
        if (braceStart < 0)
            return content;

        var braceDepth = 1;
        var braceEnd = braceStart + 1;
        while (braceEnd < content.Length && braceDepth > 0)
        {
            if (content[braceEnd] == '{') braceDepth++;
            else if (content[braceEnd] == '}') braceDepth--;
            braceEnd++;
        }

        var disposeBody = content[(braceStart + 1)..(braceEnd - 1)];
        var modifiedBody = disposeBody;

        foreach (var fieldName in readonlyFields)
        {
            // Remove lines like: _field = null;  or  _field = null!;
            var nullAssignRegex = new Regex(
                $@"^\s*{Regex.Escape(fieldName)}\s*=\s*null!?\s*;\s*$",
                RegexOptions.Multiline);
            modifiedBody = nullAssignRegex.Replace(modifiedBody, "");
        }

        if (modifiedBody == disposeBody)
            return content;

        // Check if the Dispose method body is now empty (only whitespace)
        if (string.IsNullOrWhiteSpace(modifiedBody))
        {
            // Remove the entire Dispose method
            var methodStart = disposeMatch.Index;
            // Back up to find any preceding whitespace/newline
            while (methodStart > 0 && content[methodStart - 1] is ' ' or '\t')
                methodStart--;
            if (methodStart > 0 && content[methodStart - 1] == '\n')
                methodStart--;
            if (methodStart > 0 && content[methodStart - 1] == '\r')
                methodStart--;

            return content[..methodStart] + content[braceEnd..];
        }

        return content[..(braceStart + 1)] + modifiedBody + content[(braceEnd - 1)..];
    }
}
