using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Resolves bare color name attributes to string expressions that use WebColor's implicit conversion.
/// Web Forms: BackColor="Transparent" → Blazor: BackColor='@("Transparent")'
/// Without this, the Razor compiler treats bare color names as C# identifiers (CS0103).
/// </summary>
public class ColorAttributeTransform : IMarkupTransform
{
    public string Name => "ColorAttribute";
    public int Order => 350; // After expression transforms, before prefix stripping

    // Matches color attributes with bare PascalCase word values.
    // In Web Forms, color attributes are always literal color names — never variable references
    // (those use <%# %> binding syntax, already transformed by earlier passes).
    private static readonly Regex ColorAttributeRegex = new(
        @"(?<attr>(?:Back|Fore|Border)Color)\s*=\s*""(?<value>[A-Z][a-zA-Z]+)""",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        return ColorAttributeRegex.Replace(content, match =>
        {
            var attr = match.Groups["attr"].Value;
            var value = match.Groups["value"].Value;

            // Skip values that are already Razor expressions or WebColor references
            if (value.StartsWith("WebColor") || value.StartsWith("@"))
                return match.Value;

            // Wrap as string expression — WebColor's implicit operator handles the conversion
            return $"{attr}='@(\"{value}\")'";
        });
    }
}

