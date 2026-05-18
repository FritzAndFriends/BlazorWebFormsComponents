using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Rewrites <c>identifier.InnerText = value</c> and <c>identifier.InnerHtml = value</c>
/// to plain assignment <c>identifier = value</c>.  HTML server controls with <c>runat="server"</c>
/// become simple string-backed fields in Blazor; their <c>InnerText</c>/<c>InnerHtml</c>
/// properties no longer exist.
/// Also injects a <c>private string Identifier = "";</c> stub declaration for any
/// PascalCase control ID that was referenced via InnerText/InnerHtml but is not already
/// declared in the class body, so the migrated code compiles without CS0103.
/// </summary>
public class InnerTextRewriteTransform : ICodeBehindTransform
{
    public string Name => "InnerTextRewrite";
    public int Order => 750; // After EventHandlerSignature (700), before CompileSurfaceStub (850)

    private static readonly Regex InnerTextAccessRegex = new(
        @"(?<prefix>\b[A-Za-z_]\w*)\.(?:InnerText|InnerHtml)\b",
        RegexOptions.Compiled);

    // Matches the class body opening brace — uses [^{]* to survive base-class/interface lists
    // injected by earlier transforms (e.g., BaseClassStripTransform at Order 200).
    private static readonly Regex ClassOpenRegex = new(
        @"partial\s+class\s+\w+[^{]*\{",
        RegexOptions.Compiled);

    // Detects an existing member declaration with an access modifier.
    private static bool HasDeclaredMember(string content, string name)
    {
        var escaped = Regex.Escape(name);
        return Regex.IsMatch(
            content,
            $@"(?:public|protected|private|internal)\s+(?:static\s+|readonly\s+|const\s+|volatile\s+|new\s+)*[\w<>,?.\[\]]+\s+{escaped}\s*(?:[;={{])",
            RegexOptions.Multiline);
    }

    public string Apply(string content, FileMetadata metadata)
    {
        if (!content.Contains("InnerText", StringComparison.Ordinal) &&
            !content.Contains("InnerHtml", StringComparison.Ordinal))
        {
            return content;
        }

        // Collect PascalCase identifiers that appear as InnerText/InnerHtml targets.
        // Only PascalCase IDs come from HTML server controls; camelCase/underscore
        // identifiers are local variables or fields already declared in code-behind.
        var controlIds = InnerTextAccessRegex.Matches(content)
            .Select(m => m.Groups["prefix"].Value)
            .Where(id => id.Length > 0 && char.IsUpper(id[0]))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        content = InnerTextAccessRegex.Replace(content, "${prefix}");

        // Inject a private string stub for each control ID that isn't already declared.
        if (controlIds.Count > 0)
        {
            content = EnsureStringFieldDeclarations(content, controlIds);
        }

        return content;
    }

    private static string EnsureStringFieldDeclarations(string content, IReadOnlyList<string> identifiers)
    {
        var classMatch = ClassOpenRegex.Match(content);
        if (!classMatch.Success)
            return content;

        var insertAt = content.IndexOf('{', classMatch.Index) + 1;
        if (insertAt <= 0)
            return content;

        var declarations = identifiers
            .Where(id => !HasDeclaredMember(content, id))
            .Select(id => $"\n    private string {id} = \"\"; // TODO: HTML server control field — render via @{id} or bind in markup")
            .ToList();

        if (declarations.Count == 0)
            return content;

        return content.Insert(insertAt, string.Join("", declarations));
    }
}
