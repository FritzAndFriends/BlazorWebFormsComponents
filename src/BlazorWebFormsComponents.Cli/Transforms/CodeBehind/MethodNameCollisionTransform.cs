using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Detects and resolves method name collisions with class names.
/// When a method has the same name as its enclosing class (e.g., class Forgot with method void Forgot()),
/// this creates a CS0542 compiler error. The transform renames such methods to On{ClassName}
/// and updates both the code-behind and markup references.
/// </summary>
public class MethodNameCollisionTransform : ICodeBehindTransform
{
    public string Name => "MethodNameCollision";
    public int Order => 215; // After ClassNameAlignTransform (210)

    private static readonly Regex PartialClassRegex = new(
        @"(?<=\bpartial\s+class\s+)(\w+)",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        // Extract class name
        var classMatch = PartialClassRegex.Match(content);
        if (!classMatch.Success)
            return content;

        var className = classMatch.Value;

        // Look for methods with the same name as the class (excluding constructors)
        // Pattern matches: void/Task/Task<T>/async void/async Task followed by ClassName(
        var methodPattern = $@"(?<returnType>(?:async\s+)?(?:void|Task(?:<[^>]+>)?)\s+)(?<methodName>{Regex.Escape(className)})(?=\s*\()";
        var methodRegex = new Regex(methodPattern, RegexOptions.Compiled);

        var methodMatch = methodRegex.Match(content);
        if (!methodMatch.Success)
            return content; // No collision found

        // Rename method to On{ClassName}
        var newMethodName = $"On{className}";

        // Replace method signature
        content = methodRegex.Replace(content, $"${{returnType}}{newMethodName}");

        // Replace method calls within the code-behind
        // Pattern: this.ClassName( or just ClassName( (word boundary aware)
        var callPattern = $@"(?<=\bthis\.){Regex.Escape(className)}(?=\s*\()";
        content = Regex.Replace(content, callPattern, newMethodName);

        // Also handle direct calls without "this."
        var directCallPattern = $@"(?<!\w){Regex.Escape(className)}(?=\s*\()";
        content = Regex.Replace(content, directCallPattern, m =>
        {
            // Make sure we're not replacing the class name in "partial class ClassName"
            // or constructor declarations
            var beforeContext = content.Substring(Math.Max(0, m.Index - 20), Math.Min(20, m.Index));
            if (beforeContext.Contains("partial class") || beforeContext.Contains("public ") && !beforeContext.Contains("return"))
                return m.Value; // Keep as-is (it's a class or constructor)
            return newMethodName;
        });

        // Update markup content if present
        if (metadata.MarkupContent != null)
        {
            // Replace @ClassName with @On{ClassName} in attribute values
            // Pattern: "@ClassName" (in quotes)
            var markupPattern = $@"""@{Regex.Escape(className)}""";
            metadata.MarkupContent = Regex.Replace(
                metadata.MarkupContent,
                markupPattern,
                $"\"@{newMethodName}\"");
        }

        return content;
    }
}
