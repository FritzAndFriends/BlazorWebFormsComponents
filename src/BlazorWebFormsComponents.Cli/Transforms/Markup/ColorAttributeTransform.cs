using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Resolves bare color name attributes and hex color attributes to string expressions
/// that use WebColor's implicit conversion.
/// Web Forms: BackColor="Transparent" → Blazor: BackColor='@("Transparent")'
/// Web Forms: ForeColor="#333333" → Blazor: ForeColor='@("#333333")'
/// Without this, the Razor compiler treats bare color names as C# identifiers (CS0103)
/// and hex values as preprocessor directives.
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

    // Matches color attributes with hex values (#RGB, #RRGGBB, #RRGGBBAA).
    private static readonly Regex HexColorAttributeRegex = new(
        @"(?<attr>(?:Back|Fore|Border)Color)\s*=\s*""(?<value>#[0-9A-Fa-f]{3,8})""",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        // Handle hex color values first
        content = HexColorAttributeRegex.Replace(content, match =>
        {
            var attr = match.Groups["attr"].Value;
            var value = match.Groups["value"].Value;
            return $"{attr}='@(\"{value}\")'";
        });

        // Handle named color values
        content = ColorAttributeRegex.Replace(content, match =>
        {
            var attr = match.Groups["attr"].Value;
            var value = match.Groups["value"].Value;

            // Skip values that are already Razor expressions or WebColor references
            if (value.StartsWith("WebColor") || value.StartsWith("@"))
                return match.Value;

            // Wrap as string expression — WebColor's implicit operator handles the conversion
            return $"{attr}='@(\"{value}\")'";
        });

        return content;
    }
}

