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

    // Match "partial class ClassName" followed by optional base class/interface list and "{"
    // [^{]* handles ": WebFormsPageBase", ": Base, IInterface", newlines, etc.
    private static readonly Regex ClassOpenRegex = new(
        @"partial\s+class\s+\w+[^{]*\{",
        RegexOptions.Compiled);

    // Known AJAX Toolkit component types that live in BlazorAjaxToolkitComponents namespace
    private static readonly HashSet<string> AjaxToolkitTypes = new(StringComparer.Ordinal)
    {
        "AutoCompleteExtender", "CalendarExtender", "CollapsiblePanelExtender",
        "ModalPopupExtender", "TabContainer", "TabPanel", "Accordion", "AccordionPane",
    };

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
        var needsAjaxToolkitUsing = false;
        foreach (var (controlId, fieldType) in metadata.ComponentRefs.OrderBy(kv => kv.Key))
        {
            // Skip if a field with this name already exists in the code-behind
            if (Regex.IsMatch(content, $@"\b{Regex.Escape(controlId)}\s*[;=]"))
            {
                // Check if it's actually a field declaration (not just usage)
                if (Regex.IsMatch(content, $@"(?:private|protected|public|internal)\s+\w+.*\b{Regex.Escape(controlId)}\s*[;=]"))
                    continue;
            }

            if (AjaxToolkitTypes.Contains(fieldType))
                needsAjaxToolkitUsing = true;

            fields.Add($"    private {fieldType} {controlId} = default!;");
        }

        if (fields.Count == 0)
            return content;

        // Add using directive for AJAX Toolkit types if needed
        if (needsAjaxToolkitUsing && !content.Contains("using BlazorAjaxToolkitComponents"))
        {
            // Find the last top-level using directive (ends with semicolon on same line, before namespace/class)
            var lines = content.Split('\n');
            var lastUsingLineIndex = -1;
            for (var i = 0; i < lines.Length; i++)
            {
                var trimmed = lines[i].TrimStart();
                if (trimmed.StartsWith("using ") && trimmed.TrimEnd().EndsWith(";") && !trimmed.Contains("("))
                    lastUsingLineIndex = i;
                if (trimmed.StartsWith("namespace ") || trimmed.StartsWith("public ") || trimmed.StartsWith("internal "))
                    break;
            }
            if (lastUsingLineIndex >= 0)
            {
                var linesList = new List<string>(lines);
                linesList.Insert(lastUsingLineIndex + 1, "using BlazorAjaxToolkitComponents;");
                content = string.Join("\n", linesList);
            }
            // Re-match since content changed
            match = ClassOpenRegex.Match(content);
            if (!match.Success)
                return content;
        }

        var fieldBlock = "\n" + string.Join("\n", fields);

        // Insert after the class opening brace
        var insertPos = match.Index + match.Length;
        return content.Insert(insertPos, fieldBlock);
    }
}
