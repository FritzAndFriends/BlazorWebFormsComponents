using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Renames the partial class declaration to match the output .razor filename.
/// Web Forms code-behind class names often differ from the file name
/// (e.g., SiteMaster in Site.Master.cs, _Default in Default.aspx.cs).
/// Blazor requires the partial class name to match the .razor filename exactly.
/// </summary>
public class ClassNameAlignTransform : ICodeBehindTransform
{
    public string Name => "ClassNameAlign";
    public int Order => 210; // After BaseClassStripTransform (200)

    private static readonly Regex PartialClassRegex = new(
        @"(?<=\bpartial\s+class\s+)(\w+)",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        // Derive the expected class name from the .razor output filename
        var razorFileName = Path.GetFileNameWithoutExtension(metadata.OutputFilePath);
        if (razorFileName.EndsWith(".razor", StringComparison.OrdinalIgnoreCase))
            razorFileName = Path.GetFileNameWithoutExtension(razorFileName);

        // Sanitize for C# identifier (dots → underscores, leading digits → underscore prefix)
        var targetClassName = razorFileName.Replace('.', '_').Replace('-', '_');
        if (targetClassName.Length > 0 && char.IsDigit(targetClassName[0]))
            targetClassName = "_" + targetClassName;

        // Blazor components must be PascalCase; also avoids C# reserved keywords like "default"
        targetClassName = ToPascalCase(targetClassName);

        var match = PartialClassRegex.Match(content);
        if (!match.Success)
            return content;

        var currentClassName = match.Value;
        if (currentClassName == targetClassName)
            return content; // Already matches

        // Only rename in code structure positions — not in strings or comments.
        // Target: partial class declaration, constructor names, typeof(), nameof(), and new ClassName()
        var patterns = new[]
        {
            $@"(?<=\bpartial\s+class\s+){Regex.Escape(currentClassName)}\b",          // class declaration
            $@"(?<=\bnew\s+){Regex.Escape(currentClassName)}(?=\s*\()",                // new ClassName()
            $@"(?<=\btypeof\s*\(\s*){Regex.Escape(currentClassName)}(?=\s*\))",        // typeof(ClassName)
            $@"(?<=\bnameof\s*\(\s*){Regex.Escape(currentClassName)}(?=\s*\))",        // nameof(ClassName)
            $@"(?<=\bpublic\s+){Regex.Escape(currentClassName)}(?=\s*\()",             // constructor
            $@"(?<=\bprotected\s+){Regex.Escape(currentClassName)}(?=\s*\()",          // protected constructor
            $@"(?<=\bprivate\s+){Regex.Escape(currentClassName)}(?=\s*\()",            // private constructor
        };

        foreach (var pattern in patterns)
        {
            content = Regex.Replace(content, pattern, targetClassName);
        }

        return content;
    }

    /// <summary>
    /// Converts a name like "default" or "site_mobile" to PascalCase ("Default", "Site_Mobile").
    /// Ensures the result is a valid C# identifier and not a reserved keyword.
    /// </summary>
    private static string ToPascalCase(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        // Capitalize first letter
        var result = char.ToUpperInvariant(name[0]) + name[1..];

        // Also capitalize after underscores (e.g., site_mobile → Site_Mobile)
        result = Regex.Replace(result, @"(?<=_)([a-z])", m => m.Value.ToUpperInvariant());

        // Safety check: if still a C# keyword (shouldn't happen after PascalCase), prefix with @
        if (CSharpKeywords.Contains(result))
            result = "@" + result;

        return result;
    }

    private static readonly HashSet<string> CSharpKeywords = new(StringComparer.Ordinal)
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char",
        "checked", "class", "const", "continue", "decimal", "default", "delegate", "do",
        "double", "else", "enum", "event", "explicit", "extern", "false", "finally",
        "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int",
        "interface", "internal", "is", "lock", "long", "namespace", "new", "null",
        "object", "operator", "out", "override", "params", "private", "protected",
        "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof",
        "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true",
        "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using",
        "virtual", "void", "volatile", "while"
    };
}
