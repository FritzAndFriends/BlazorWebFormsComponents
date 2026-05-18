using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Directives;

/// <summary>
/// Removes &lt;%@ Register ... %&gt; directives (tag prefixes are handled by component transforms).
/// </summary>
public class RegisterDirectiveTransform : IMarkupTransform
{
    public string Name => "RegisterDirective";
    public int Order => 210;

    private static readonly Regex RegisterRegex = new(@"<%@\s*Register[^%]*%>\s*\r?\n?", RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        return RegisterRegex.Replace(content, "");
    }
}
