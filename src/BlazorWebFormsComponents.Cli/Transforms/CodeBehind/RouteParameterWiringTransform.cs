using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Ensures that route parameters declared in @page directives have matching
/// [Parameter] properties in the code-behind class. For example, if the markup
/// has @page "/Product/{productName}", this transform adds:
///   [Parameter] public string? productName { get; set; }
/// </summary>
public class RouteParameterWiringTransform : ICodeBehindTransform
{
    // Matches @page "/path/{paramName}" or @page "/path/{paramName:int}"
    private static readonly Regex PageRouteRegex = new(
        @"@page\s+""[^""]*\{(?<param>\w+)(?::(?<constraint>\w+))?\}[^""]*""",
        RegexOptions.Compiled);

    // Matches existing [Parameter] properties
    private static readonly Regex ExistingParameterRegex = new(
        @"\[Parameter[^\]]*\]\s*(?:\[SupplyParameterFromQuery[^\]]*\]\s*)?public\s+\S+\s+(?<name>\w+)\s*\{",
        RegexOptions.Compiled);

    // Matches any public/protected/private property or field with the same name,
    // including those prefixed with attributes like [Parameter] or [Inject].
    private static readonly Regex AnyMemberRegex = new(
        @"(?:\[[^\]]*\]\s*)*(?:public|protected|private)\s+(?:static\s+)?(?:readonly\s+)?\S+\s+(?<name>\w+)\s*(?:\{|;|=)",
        RegexOptions.Compiled);

    // Matches the opening brace of the class body
    private static readonly Regex ClassBodyRegex = new(
        @"(partial\s+class\s+\w+[^{]*\{)",
        RegexOptions.Compiled);

    public string Name => "RouteParameterWiring";
    public int Order => 135; // After ClassNameAlign (130), NamespaceAlign (125) but before lifecycle transforms

    public string Apply(string content, FileMetadata metadata)
    {
        if (metadata.FileType != FileType.Page)
            return content;

        // Read @page directives from the transformed markup
        var markup = metadata.MarkupContent ?? metadata.OriginalContent ?? "";
        var routeParams = ExtractRouteParameters(markup);
        if (routeParams.Count == 0)
            return content;

        // Find existing [Parameter] property names in code-behind
        var existingParams = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (Match m in ExistingParameterRegex.Matches(content))
        {
            existingParams.Add(m.Groups["name"].Value);
        }

        // Also check for any member (field/property) with the same name to avoid collisions
        foreach (Match m in AnyMemberRegex.Matches(content))
        {
            existingParams.Add(m.Groups["name"].Value);
        }

        // Check the markup @code block for existing [Parameter] declarations too
        foreach (Match m in ExistingParameterRegex.Matches(markup))
        {
            existingParams.Add(m.Groups["name"].Value);
        }

        // Determine which route params are missing
        var missingParams = routeParams
            .Where(p => !existingParams.Contains(p.Name))
            .ToList();

        if (missingParams.Count == 0)
            return content;

        // Build property declarations for missing params
        var classMatch = ClassBodyRegex.Match(content);
        if (!classMatch.Success)
            return content;

        var insertPos = classMatch.Index + classMatch.Length;
        var properties = new System.Text.StringBuilder();

        foreach (var param in missingParams)
        {
            var csharpType = param.Constraint switch
            {
                "int" => "int?",
                "long" => "long?",
                "bool" => "bool?",
                "double" => "double?",
                "decimal" => "decimal?",
                "float" => "float?",
                "guid" or "Guid" => "Guid?",
                "datetime" => "DateTime?",
                _ => "string?"
            };
            properties.AppendLine();
            properties.AppendLine($"    [Parameter] public {csharpType} {param.Name} {{ get; set; }}");
        }

        content = content[..insertPos] + properties.ToString() + content[insertPos..];

        // Ensure Microsoft.AspNetCore.Components using is present
        if (!content.Contains("using Microsoft.AspNetCore.Components;", StringComparison.Ordinal))
        {
            var lastUsing = Regex.Match(content, @"^using\s+[^;]+;\s*$", RegexOptions.Multiline | RegexOptions.RightToLeft);
            if (lastUsing.Success)
            {
                var insertAt = lastUsing.Index + lastUsing.Length;
                content = content[..insertAt] + "\nusing Microsoft.AspNetCore.Components;" + content[insertAt..];
            }
        }

        return content;
    }

    private static List<RouteParam> ExtractRouteParameters(string markup)
    {
        var result = new List<RouteParam>();
        foreach (Match m in PageRouteRegex.Matches(markup))
        {
            var name = m.Groups["param"].Value;
            var constraint = m.Groups["constraint"].Success ? m.Groups["constraint"].Value : null;
            if (!result.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                result.Add(new RouteParam(name, constraint));
            }
        }
        return result;
    }

    private sealed record RouteParam(string Name, string? Constraint);
}
