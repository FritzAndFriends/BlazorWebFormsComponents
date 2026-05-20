using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Converts <c>new BllClass()</c> instantiations in page/component code-behind into
/// <c>[Inject]</c> property declarations, promoting BLL/service classes to DI-injected
/// dependencies. The BLL classes themselves will get constructor injection from
/// <see cref="DbContextInstantiationTransform"/> when their files are processed.
/// </summary>
public class ServiceConstructorWiringTransform : ICodeBehindTransform
{
    public string Name => "ServiceConstructorWiring";
    public int Order => 108; // After DbContextInstantiation (107)

    // Match field/variable assignments like: BllClass field = new BllClass();
    // Captures the type name and variable name.
    private static readonly Regex FieldDeclRegex = new(
        @"(?<indent>[ \t]*)(?:private\s+)?(?<type>\w+(?:Logic|_Logic))\s+(?<var>\w+)\s*=\s*new\s+\k<type>\s*\(\s*\)\s*;",
        RegexOptions.Compiled);

    // Match standalone `new BllClass()` expressions (not preceded by type declaration)
    private static readonly Regex NewBllRegex = new(
        @"(?<assign>(?<var>\w+)\s*=\s*)?new\s+(?<type>\w+(?:Logic|_Logic))\s*\(\s*\)",
        RegexOptions.Compiled);

    // Detects page/component classes
    private static readonly Regex PageBaseRegex = new(
        @":\s*(?:WebFormsPageBase|ComponentBase|LayoutComponentBase|BaseWebFormsComponent)\b",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        // Only apply to page/component files
        var isPageOrComponent = metadata.FileType == FileType.Page
            || metadata.FileType == FileType.Control
            || PageBaseRegex.IsMatch(content);

        if (!isPageOrComponent)
            return content;

        // Find all BLL types being instantiated
        var fieldMatches = FieldDeclRegex.Matches(content);
        var otherMatches = NewBllRegex.Matches(content);

        if (fieldMatches.Count == 0 && otherMatches.Count == 0)
            return content;

        // Collect unique BLL types and their variable names
        var bllTypes = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (Match m in fieldMatches)
        {
            var type = m.Groups["type"].Value;
            var varName = m.Groups["var"].Value;
            if (!bllTypes.ContainsKey(type))
                bllTypes[type] = varName;
        }
        foreach (Match m in otherMatches)
        {
            var type = m.Groups["type"].Value;
            if (!bllTypes.ContainsKey(type))
            {
                var varName = m.Groups["var"].Success && !string.IsNullOrEmpty(m.Groups["var"].Value)
                    ? m.Groups["var"].Value
                    : "_" + char.ToLowerInvariant(type[0]) + type[1..];
                bllTypes[type] = varName;
            }
        }

        if (bllTypes.Count == 0)
            return content;

        foreach (var (typeName, fieldName) in bllTypes)
        {
            // Remove field declarations like: BllType _field = new BllType();
            var specificFieldDecl = new Regex(
                $@"[ \t]*(?:private\s+)?{Regex.Escape(typeName)}\s+\w+\s*=\s*new\s+{Regex.Escape(typeName)}\s*\(\s*\)\s*;\s*\n?",
                RegexOptions.Compiled);
            content = specificFieldDecl.Replace(content, "");

            // Remove standalone field declarations: private BllType field;
            var plainFieldDecl = new Regex(
                $@"[ \t]*(?:private\s+)?{Regex.Escape(typeName)}\s+\w+\s*;\s*\n?",
                RegexOptions.Compiled);
            content = plainFieldDecl.Replace(content, "");

            // Remove assignments like: field = new BllType(); (the field is now injected via DI)
            var assignmentRegex = new Regex(
                $@"[ \t]*\w+\s*=\s*new\s+{Regex.Escape(typeName)}\s*\(\s*\)\s*;\s*\n?",
                RegexOptions.Compiled);
            content = assignmentRegex.Replace(content, "");

            // Replace any remaining inline `new BllType()` expressions with the field name
            var remainingNew = new Regex(
                $@"new\s+{Regex.Escape(typeName)}\s*\(\s*\)",
                RegexOptions.Compiled);
            content = remainingNew.Replace(content, fieldName);

            // Add [Inject] property if not already present
            if (!content.Contains($"{typeName} {fieldName}", StringComparison.Ordinal))
            {
                var classBodyRegex = new Regex(
                    @"((?:partial\s+)?class\s+\w+[^{]*\{)",
                    RegexOptions.Compiled);
                var classMatch = classBodyRegex.Match(content);
                if (classMatch.Success)
                {
                    var insertPos = classMatch.Index + classMatch.Length;
                    var injection = $"\n    [Inject]\n    protected {typeName} {fieldName} {{ get; set; }} = default!;\n";
                    content = content[..insertPos] + injection + content[insertPos..];
                }
            }
        }

        // Ensure Microsoft.AspNetCore.Components using is present
        if (content.Contains("[Inject]") && !content.Contains("using Microsoft.AspNetCore.Components;", StringComparison.Ordinal))
        {
            var lastUsing = Regex.Match(content, @"^using\s+[^;(\n]+;\s*$", RegexOptions.Multiline | RegexOptions.RightToLeft);
            if (lastUsing.Success)
            {
                var insertAt = lastUsing.Index + lastUsing.Length;
                content = content[..insertAt] + "\nusing Microsoft.AspNetCore.Components;" + content[insertAt..];
            }
        }

        // Clean up empty lines
        content = Regex.Replace(content, @"\n{3,}", "\n\n");

        return content;
    }
}
