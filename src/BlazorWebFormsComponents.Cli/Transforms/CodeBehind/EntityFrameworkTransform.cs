using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Replaces Entity Framework 6 namespace references with Entity Framework Core equivalents.
///   System.Data.Entity            → Microsoft.EntityFrameworkCore
///   System.Data.Entity.Validation → Microsoft.EntityFrameworkCore
///   System.Data.Entity.*          → Microsoft.EntityFrameworkCore
/// </summary>
public class EntityFrameworkTransform : ICodeBehindTransform
{
    public string Name => "EntityFramework";
    public int Order => 105; // After UsingStripTransform (100), before ConfigurationManager (110)

    private static readonly Regex Ef6UsingRegex = new(
        @"using\s+System\.Data\.Entity(\.\w+)*;\s*\r?\n?",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        if (!Ef6UsingRegex.IsMatch(content))
            return content;

        // Remove all EF6 using directives
        content = Ef6UsingRegex.Replace(content, "");

        // Add EF Core using if not already present
        if (!content.Contains("using Microsoft.EntityFrameworkCore"))
        {
            // Insert after the last existing using directive, or at the top
            var lastUsing = Regex.Match(content, @"^using\s+[^;]+;\s*$", RegexOptions.Multiline | RegexOptions.RightToLeft);
            if (lastUsing.Success)
            {
                var insertPos = lastUsing.Index + lastUsing.Length;
                content = content[..insertPos] + "\nusing Microsoft.EntityFrameworkCore;\n" + content[insertPos..];
            }
            else
            {
                content = "using Microsoft.EntityFrameworkCore;\n" + content;
            }
        }

        return content;
    }
}
