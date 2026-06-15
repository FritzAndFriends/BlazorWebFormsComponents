using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Detects StaticItems.Add() calls on @ref DropDownList/ListBox fields inside
/// OnInitializedAsync and refactors to a backing ListItemCollection + markup parameter binding.
///
/// Gate: Only applies when:
///   1. The target is a known @ref field (from metadata.ComponentRefs)
///   2. The call is inside OnInitializedAsync/OnInitialized body
///   3. The call uses null-conditional (?.) pattern on the ref
///
/// Pattern detected:
///   this.dropdown?.StaticItems.Add(new ListItem(...));
///   dropdown?.StaticItems.Add(new ListItem(...));
///   -- OR loop patterns like:
///   foreach (...) { this.dropdown?.StaticItems.Add(...); }
///
/// Refactored to:
///   Code-behind: private ListItemCollection _dropdown_StaticItems = new();  (field)
///                _dropdown_StaticItems.Add(new ListItem(...));              (in OnInit)
///   Markup:      StaticItems="@_dropdown_StaticItems"                      (injected)
/// </summary>
public class StaticItemsParameterBindingTransform : ICodeBehindTransform
{
    public string Name => "StaticItemsParameterBinding";
    public int Order => 812; // After DataSourceParameterBindingTransform (810)

    // Matches: controlId?.StaticItems.Add(...) with optional this. prefix
    private static readonly Regex NullSafeStaticItemsAddRegex = new(
        @"(?:this\.)?(\w+)\?\.StaticItems\.Add\(",
        RegexOptions.Compiled);

    // Matches OnInitializedAsync or OnInitialized method signature
    private static readonly Regex LifecycleMethodRegex = new(
        @"(?:override\s+(?:async\s+)?(?:Task|void)\s+(?:OnInitializedAsync|OnInitialized)\s*\()",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        if (metadata.ComponentRefs.Count == 0)
            return content;

        if (!LifecycleMethodRegex.IsMatch(content))
            return content;

        var markup = metadata.MarkupContent;
        if (string.IsNullOrEmpty(markup))
            return content;

        // Check if there are any null-safe StaticItems.Add calls on @ref fields
        var prelimMatches = NullSafeStaticItemsAddRegex.Matches(content);
        var affectedControls = new HashSet<string>();

        foreach (Match m in prelimMatches)
        {
            var controlId = m.Groups[1].Value;
            if (metadata.ComponentRefs.ContainsKey(controlId))
                affectedControls.Add(controlId);
        }

        if (affectedControls.Count == 0)
            return content;

        // For each affected control, replace controlId?.StaticItems.Add with _controlId_StaticItems.Add
        foreach (var controlId in affectedControls)
        {
            var fieldName = $"_{controlId}_StaticItems";

            // Replace all occurrences of controlId?.StaticItems with fieldName
            var pattern = new Regex(
                $@"(?:this\.)?{Regex.Escape(controlId)}\?\.StaticItems\.Add\(",
                RegexOptions.Compiled);

            content = pattern.Replace(content, $"{fieldName}.Add(");

            // Also handle non-null-safe version (in case it wasn't converted)
            var plainPattern = new Regex(
                $@"(?:this\.)?{Regex.Escape(controlId)}\.StaticItems\.Add\(",
                RegexOptions.Compiled);

            content = plainPattern.Replace(content, $"{fieldName}.Add(");

            // Add backing field declaration after the @ref field
            var refFieldPattern = new Regex(
                $@"([ \t]*private\s+\w+(?:<\w+>)?\s+{Regex.Escape(controlId)}\s*=\s*default!;\s*\r?\n)",
                RegexOptions.Compiled);

            var refMatch = refFieldPattern.Match(content);
            if (refMatch.Success)
            {
                var insertPos = refMatch.Index + refMatch.Length;
                var lineMatch = Regex.Match(refMatch.Value, @"^([ \t]*)");
                var indent = lineMatch.Success ? lineMatch.Groups[1].Value : "    ";

                content = content[..insertPos] +
                    $"{indent}private BlazorWebFormsComponents.ListItemCollection {fieldName} = new();\n" +
                    content[insertPos..];
            }
            else
            {
                // Fallback: insert near top of class
                var classOpenBrace = content.IndexOf('{', content.IndexOf("class "));
                if (classOpenBrace > 0)
                {
                    var insertPos = classOpenBrace + 1;
                    content = content[..insertPos] +
                        $"\n    private BlazorWebFormsComponents.ListItemCollection {fieldName} = new();\n" +
                        content[insertPos..];
                }
            }

            // Inject StaticItems="@fieldName" into the markup component tag
            var markupRefPattern = new Regex(
                $@"(<\w+[^>]*@ref\s*=\s*""{Regex.Escape(controlId)}""[^>]*?)(\/?>)",
                RegexOptions.Compiled);

            if (markupRefPattern.IsMatch(markup))
            {
                // Don't inject if StaticItems is already present
                var hasStaticItems = Regex.IsMatch(markup,
                    $@"<\w+[^>]*@ref\s*=\s*""{Regex.Escape(controlId)}""[^>]*StaticItems\s*=",
                    RegexOptions.Compiled);

                if (!hasStaticItems)
                {
                    markup = markupRefPattern.Replace(markup, m =>
                    {
                        return $"{m.Groups[1].Value} StaticItems=\"@{fieldName}\"{m.Groups[2].Value}";
                    }, 1);
                }
            }
        }

        metadata.MarkupContent = markup;
        return content;
    }
}
