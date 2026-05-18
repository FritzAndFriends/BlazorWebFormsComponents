using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Directives;

/// <summary>
/// Removes &lt;%@ Control ... %&gt; directives.
/// </summary>
public class ControlDirectiveTransform : IMarkupTransform
{
    public string Name => "ControlDirective";
    public int Order => 120;

    private static readonly Regex ControlDirectiveRegex = new(@"<%@\s*Control[^%]*%>\s*\r?\n?", RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        return ControlDirectiveRegex.Replace(content, "");
    }
}
