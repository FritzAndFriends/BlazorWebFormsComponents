using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Injects private field declarations for @ref component references.
/// Reads the ComponentRefs dictionary populated by ComponentRefMarkupTransform
/// and generates corresponding field declarations in the code-behind class.
/// </summary>
public class ComponentRefCodeBehindTransform : ICodeBehindTransform
{
    public string Name => "ComponentRefField";
    public int Order => 220; // After ClassNameAlignTransform (210)

    // Match "partial class ClassName" followed by optional whitespace and "{"
    private static readonly Regex ClassOpenRegex = new(
        @"partial\s+class\s+\w+\s*\{",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        if (metadata.ComponentRefs.Count == 0)
            return content;

        // Find the class opening brace
        var match = ClassOpenRegex.Match(content);
        if (!match.Success)
            return content;

        // Generate field declarations
        var fields = new List<string>();
        foreach (var (controlId, fieldType) in metadata.ComponentRefs.OrderBy(kv => kv.Key))
        {
            // Skip if a field with this name already exists in the code-behind
            if (Regex.IsMatch(content, $@"\b{Regex.Escape(controlId)}\s*[;=]"))
            {
                // Check if it's actually a field declaration (not just usage)
                if (Regex.IsMatch(content, $@"(?:private|protected|public|internal)\s+\w+.*\b{Regex.Escape(controlId)}\s*[;=]"))
                    continue;
            }

            fields.Add($"    private {fieldType} {controlId} = default!;");
        }

        if (fields.Count == 0)
            return content;

        var fieldBlock = "\n" + string.Join("\n", fields);

        // Insert after the class opening brace
        var insertPos = match.Index + match.Length;
        return content.Insert(insertPos, fieldBlock);
    }
}
