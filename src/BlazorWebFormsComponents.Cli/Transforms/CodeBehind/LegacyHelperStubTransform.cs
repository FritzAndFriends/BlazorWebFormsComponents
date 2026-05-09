using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Stubs non-page helper/utility classes that reference legacy Web Forms namespaces
/// which are not available in Blazor (e.g., System.Web.Security, Microsoft.AspNet.Identity,
/// System.Web.Providers). These classes would otherwise produce cascading compile errors.
/// </summary>
public class LegacyHelperStubTransform : ICodeBehindTransform
{
    private static readonly Regex LegacyNamespaceRegex = new(
        @"\busing\s+(Microsoft\.AspNet\.Identity|Microsoft\.AspNet\.Identity\.EntityFramework|Microsoft\.AspNet\.Identity\.Owin|System\.Web\.Security|System\.Web\.Providers|System\.Web\.Configuration|System\.Web\.Profile|WebMatrix\.WebData|System\.Configuration)\b",
        RegexOptions.Compiled);

    private static readonly Regex NamespaceRegex = new(
        @"namespace\s+(?<ns>[A-Za-z_][\w.]*)\s*(?:\{|;)",
        RegexOptions.Compiled);

    private static readonly Regex ClassRegex = new(
        @"(?:public|internal)\s+(?:static\s+)?class\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)",
        RegexOptions.Compiled);

    public string Name => "LegacyHelperStub";
    public int Order => 50; // Run before UsingStripTransform (100) so legacy using directives are still present

    public string Apply(string content, FileMetadata metadata)
    {
        // Only process standalone .cs files (not page/master/control code-behinds)
        // Code-behind files have a SourceFilePath ending in .aspx.cs, .ascx.cs, or .master.cs
        var sourcePath = metadata.SourceFilePath ?? metadata.OutputFilePath;
        if (sourcePath.EndsWith(".aspx.cs", StringComparison.OrdinalIgnoreCase)
            || sourcePath.EndsWith(".ascx.cs", StringComparison.OrdinalIgnoreCase)
            || sourcePath.EndsWith(".master.cs", StringComparison.OrdinalIgnoreCase))
            return content;

        if (!LegacyNamespaceRegex.IsMatch(content))
            return content;

        var nsMatch = NamespaceRegex.Match(content);
        var ns = nsMatch.Success ? nsMatch.Groups["ns"].Value : null;

        var classMatch = ClassRegex.Match(content);
        var className = classMatch.Success ? classMatch.Groups["name"].Value : Path.GetFileNameWithoutExtension(metadata.OutputFilePath);
        var isStatic = classMatch.Success && classMatch.Value.Contains("static");

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("// TODO(bwfc-general): This helper class referenced legacy Web Forms APIs and was stubbed during migration.");
        sb.AppendLine("// Rebuild using ASP.NET Core equivalents (Identity, Authorization, Configuration).");

        if (!string.IsNullOrWhiteSpace(ns))
        {
            sb.AppendLine($"namespace {ns};");
            sb.AppendLine();
        }

        var staticModifier = isStatic ? "static " : "";
        sb.AppendLine($"public {staticModifier}class {className}");
        sb.AppendLine("{");
        sb.AppendLine($"    // Stubbed — original file referenced legacy namespaces not available in .NET 10.");
        sb.AppendLine("}");

        return sb.ToString();
    }
}
