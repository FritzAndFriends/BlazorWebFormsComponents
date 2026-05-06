using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Adds explicit generic type arguments to validator components whose BWFC API
/// requires them and defaults migrated form validators to string when the input
/// type cannot be inferred mechanically.
/// </summary>
public class ValidatorGenericTypeTransform : IMarkupTransform
{
    public string Name => "ValidatorGenericType";
    public int Order => 615;

    private static readonly (Regex Pattern, string TypeAttribute)[] ValidatorPatterns =
    [
        (new Regex(@"<RequiredFieldValidator\b(?!(?:(?!>).)*\bType=)", RegexOptions.Compiled | RegexOptions.IgnoreCase), "Type"),
        (new Regex(@"<CompareValidator\b(?!(?:(?!>).)*\bInputType=)", RegexOptions.Compiled | RegexOptions.IgnoreCase), "InputType"),
        (new Regex(@"<RangeValidator\b(?!(?:(?!>).)*\bInputType=)", RegexOptions.Compiled | RegexOptions.IgnoreCase), "InputType")
    ];

    public string Apply(string content, FileMetadata metadata)
    {
        foreach (var (pattern, typeAttribute) in ValidatorPatterns)
        {
            content = pattern.Replace(content, match => $"{match.Value} {typeAttribute}=\"string\"");
        }

        return content;
    }
}
