using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Replaces Label @ref field accesses with backing string field assignments.
/// Companion to LabelFieldBindTransform (markup side).
///
/// For each label in LabelFieldBindings:
/// - Adds a private string backing field declaration
/// - Replaces fieldName.Text = expr → _fieldName_Text = expr
/// - Replaces fieldName?.Text = expr → _fieldName_Text = expr
/// - Removes the old @ref field declaration if present
/// </summary>
public class LabelFieldBindCodeBehindTransform : ICodeBehindTransform
{
    public string Name => "LabelFieldBindCodeBehind";
    public int Order => 606; // Right after ComponentRefNullSafetyTransform (605)

    // Match "partial class ClassName" followed by optional whitespace and "{"
    private static readonly Regex ClassOpenRegex = new(
        @"partial\s+class\s+\w+\s*\{",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        if (metadata.LabelFieldBindings.Count == 0)
            return content;

        var modified = content;

        foreach (var (controlId, backingField) in metadata.LabelFieldBindings)
        {
            // Replace fieldName.Text = expr → backingField = expr
            var textSetRegex = new Regex(
                $@"{Regex.Escape(controlId)}\??\s*\.\s*Text\s*=\s*",
                RegexOptions.Compiled);
            modified = textSetRegex.Replace(modified, $"{backingField} = ");

            // Replace fieldName.Text (read access, not assignment) → backingField
            var textReadRegex = new Regex(
                $@"{Regex.Escape(controlId)}\??\s*\.\s*Text\b(?!\s*=)",
                RegexOptions.Compiled);
            modified = textReadRegex.Replace(modified, backingField);

            // Replace fieldName.Visible = expr → ignore (span is always visible, controlled by content)
            // Just remove the line entirely to avoid compilation errors
            var visibleSetRegex = new Regex(
                $@"^\s*{Regex.Escape(controlId)}\??\s*\.\s*Visible\s*=\s*[^;]+;\s*$",
                RegexOptions.Multiline | RegexOptions.Compiled);
            modified = visibleSetRegex.Replace(modified, "");

            // Remove old @ref field declarations: private Label fieldName = default!;
            var fieldDeclRegex = new Regex(
                $@"^\s*private\s+Label\s+{Regex.Escape(controlId)}\s*=\s*default!;\s*$",
                RegexOptions.Multiline | RegexOptions.Compiled);
            modified = fieldDeclRegex.Replace(modified, "");
        }

        // Add backing field declarations at class opening
        var classMatch = ClassOpenRegex.Match(modified);
        if (classMatch.Success)
        {
            var fields = metadata.LabelFieldBindings.Values
                .Select(f => $"    private string {f} = \"\";")
                .ToList();

            // Check which fields don't already exist
            fields = fields.Where(f =>
            {
                var fieldName = f.Split(' ').Last().TrimEnd(';').Trim();
                return !modified.Contains(fieldName, StringComparison.Ordinal);
            }).ToList();

            if (fields.Count > 0)
            {
                var fieldBlock = "\n" + string.Join("\n", fields);
                var insertPos = classMatch.Index + classMatch.Length;
                modified = modified.Insert(insertPos, fieldBlock);
            }
        }

        return modified;
    }
}
