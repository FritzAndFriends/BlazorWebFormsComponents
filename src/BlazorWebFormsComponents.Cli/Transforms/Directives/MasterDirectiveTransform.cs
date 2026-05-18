using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Directives;

/// <summary>
/// Removes &lt;%@ Master ... %&gt; directives.
/// </summary>
public class MasterDirectiveTransform : IMarkupTransform
{
    public string Name => "MasterDirective";
    public int Order => 110;

    private static readonly Regex MasterDirectiveRegex = new(@"<%@\s*Master[^%]*%>\s*\r?\n?", RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        return MasterDirectiveRegex.Replace(content, "");
    }
}
