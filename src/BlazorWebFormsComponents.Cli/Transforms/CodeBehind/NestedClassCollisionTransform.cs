using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Detects and renames nested classes that have the same name as their enclosing type.
/// This causes CS0542: member names cannot be the same as their enclosing type.
/// Common in auto-generated code (PayPal integrations, WCF proxies, etc.).
/// </summary>
public class NestedClassCollisionTransform : ICodeBehindTransform
{
    public string Name => "NestedClassCollision";
    public int Order => 95; // Early — before other transforms that might be confused by duplicates

    // Matches class declarations to find the outer class name
    private static readonly Regex ClassDeclRegex = new(
        @"(?<access>public|internal|private|protected)\s+(?:partial\s+)?class\s+(?<name>\w+)",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        var classMatches = ClassDeclRegex.Matches(content);
        if (classMatches.Count < 2)
            return content;

        // Find the first (outer) class name
        var outerClassName = classMatches[0].Groups["name"].Value;

        // Check subsequent class declarations for collisions
        for (var i = 1; i < classMatches.Count; i++)
        {
            var innerClassName = classMatches[i].Groups["name"].Value;
            if (string.Equals(innerClassName, outerClassName, StringComparison.Ordinal))
            {
                // Rename the inner class to avoid collision
                var renamedClass = outerClassName + "Inner";
                var matchStart = classMatches[i].Index;
                var originalDecl = classMatches[i].Value;
                var renamedDecl = originalDecl.Replace(
                    $"class {innerClassName}",
                    $"class {renamedClass}");
                content = content[..matchStart] + renamedDecl + content[(matchStart + originalDecl.Length)..];

                // Re-match since content changed
                classMatches = ClassDeclRegex.Matches(content);
            }
        }

        return content;
    }
}
