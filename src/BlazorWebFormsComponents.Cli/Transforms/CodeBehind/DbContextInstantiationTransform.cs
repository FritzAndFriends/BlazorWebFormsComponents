using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Replaces <c>new XxxContext()</c> instantiations with an injected property,
/// converting inline DbContext creation to the DI pattern required in Blazor.
/// </summary>
public class DbContextInstantiationTransform : ICodeBehindTransform
{
    // Matches field/variable assignments like: XxxContext _db = new XxxContext();
    // or using blocks like: using (XxxContext _db = new XxxContext())
    private static readonly Regex NewContextRegex = new(
        @"(?<type>\w+Context)\s+(?<var>\w+)\s*=\s*new\s+\k<type>\s*\(\s*\)",
        RegexOptions.Compiled);

    // Matches just the `new XxxContext()` expression
    private static readonly Regex NewContextExprRegex = new(
        @"new\s+(?<type>\w+Context)\s*\(\s*\)",
        RegexOptions.Compiled);

    // Check if a class already has an [Inject] property for a given type
    private static readonly string InjectAttr = "[Inject]";

    public string Name => "DbContextInstantiation";
    public int Order => 107; // Right after EfContextConstructor (106)

    public string Apply(string content, FileMetadata metadata)
    {
        // Only apply to page code-behind, not to DbContext class definitions themselves
        if (metadata.FileType != FileType.Page && metadata.FileType != FileType.Control)
            return content;

        var matches = NewContextExprRegex.Matches(content);
        if (matches.Count == 0)
            return content;

        // Collect unique context type names
        var contextTypes = matches
            .Select(m => m.Groups["type"].Value)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        foreach (var contextType in contextTypes)
        {
            var fieldName = "_" + char.ToLowerInvariant(contextType[0]) + contextType[1..];

            // Replace field declarations like: XxxContext _db = new XxxContext();
            var fieldDeclRegex = new Regex(
                $@"(?<indent>[ \t]*)(?:private\s+)?{Regex.Escape(contextType)}\s+\w+\s*=\s*new\s+{Regex.Escape(contextType)}\s*\(\s*\)\s*;",
                RegexOptions.Compiled);
            content = fieldDeclRegex.Replace(content, "");

            // Replace using declarations: using (XxxContext x = new XxxContext())
            var usingDeclRegex = new Regex(
                $@"using\s*\(\s*{Regex.Escape(contextType)}\s+\w+\s*=\s*new\s+{Regex.Escape(contextType)}\s*\(\s*\)\s*\)",
                RegexOptions.Compiled);
            content = usingDeclRegex.Replace(content, m =>
            {
                // Convert to a simple block — the injected context is managed by DI
                return $"// DbContext '{contextType}' is injected via DI — using block removed\n{{";
            });

            // Replace local variable declarations: var db = new XxxContext();
            var localVarRegex = new Regex(
                $@"(?<indent>[ \t]*)(?:var|{Regex.Escape(contextType)})\s+(?<var>\w+)\s*=\s*new\s+{Regex.Escape(contextType)}\s*\(\s*\)\s*;",
                RegexOptions.Compiled);
            content = localVarRegex.Replace(content, m =>
            {
                var varName = m.Groups["var"].Value;
                var indent = m.Groups["indent"].Value;
                return $"{indent}var {varName} = {fieldName}; // Injected via DI";
            });

            // Replace any remaining `new XxxContext()` expressions
            var remainingNewRegex = new Regex(
                $@"new\s+{Regex.Escape(contextType)}\s*\(\s*\)",
                RegexOptions.Compiled);
            content = remainingNewRegex.Replace(content, fieldName);

            // Add [Inject] property if not already present
            if (!content.Contains($"{contextType} {fieldName}", StringComparison.Ordinal))
            {
                var classBodyRegex = new Regex(
                    @"(partial\s+class\s+\w+[^{]*\{)",
                    RegexOptions.Compiled);
                var classMatch = classBodyRegex.Match(content);
                if (classMatch.Success)
                {
                    var insertPos = classMatch.Index + classMatch.Length;
                    var injection = $"\n    {InjectAttr}\n    protected {contextType} {fieldName} {{ get; set; }} = default!;\n";
                    content = content[..insertPos] + injection + content[insertPos..];
                }
            }
        }

        // Ensure Microsoft.AspNetCore.Components using is present for [Inject]
        if (content.Contains(InjectAttr) && !content.Contains("using Microsoft.AspNetCore.Components;", StringComparison.Ordinal))
        {
            var lastUsing = Regex.Match(content, @"^using\s+[^;]+;\s*$", RegexOptions.Multiline | RegexOptions.RightToLeft);
            if (lastUsing.Success)
            {
                var insertAt = lastUsing.Index + lastUsing.Length;
                content = content[..insertAt] + "\nusing Microsoft.AspNetCore.Components;" + content[insertAt..];
            }
        }

        // Clean up empty lines left by removed field declarations
        content = Regex.Replace(content, @"\n{3,}", "\n\n");

        return content;
    }
}
