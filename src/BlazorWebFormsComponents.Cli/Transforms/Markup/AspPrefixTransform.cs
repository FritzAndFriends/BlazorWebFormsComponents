using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Removes asp: prefix from tags, strips ContentTemplate wrappers,
/// and removes uc: (user control) prefixes.
/// </summary>
public class AspPrefixTransform : IMarkupTransform
{
    public string Name => "AspPrefix";
    public int Order => 610;

    // Opening tags: <asp:Button → <Button
    private static readonly Regex OpenRegex = new(@"<asp:(\w+)", RegexOptions.Compiled);

    // Closing tags: </asp:Button> → </Button>
    private static readonly Regex CloseRegex = new(@"</asp:(\w+)>", RegexOptions.Compiled);

    // ContentTemplate wrappers
    private static readonly Regex ContentTemplateOpenRegex = new(@"<ContentTemplate>", RegexOptions.Compiled);
    private static readonly Regex ContentTemplateCloseRegex = new(@"</ContentTemplate>", RegexOptions.Compiled);

    // User control prefixes: <uc1:Control → <Control
    private static readonly Regex UcOpenRegex = new(@"<uc\d*:(\w+)", RegexOptions.Compiled);
    private static readonly Regex UcCloseRegex = new(@"</uc\d*:(\w+)>", RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        // asp: prefix
        content = OpenRegex.Replace(content, "<$1");
        content = CloseRegex.Replace(content, "</$1>");

        // Strip ContentTemplate wrappers
        content = ContentTemplateOpenRegex.Replace(content, "");
        content = ContentTemplateCloseRegex.Replace(content, "");

        // uc: prefix
        content = UcOpenRegex.Replace(content, "<$1");
        content = UcCloseRegex.Replace(content, "</$1>");

        return content;
    }
}
