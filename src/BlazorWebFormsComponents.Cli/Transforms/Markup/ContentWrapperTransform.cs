using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Converts &lt;asp:Content&gt; wrappers to BWFC &lt;Content ContentPlaceHolderID="X"&gt; components
/// and wraps all Content elements in the master-page component (e.g. &lt;Site&gt;...&lt;/Site&gt;).
/// Reads MasterPageFile from metadata.OriginalContent (before PageDirectiveTransform stripped it).
/// </summary>
public class ContentWrapperTransform : IMarkupTransform
{
    public string Name => "ContentWrapper";
    public int Order => 300;

    // Captures ContentPlaceHolderID value from the open tag
    private static readonly Regex ContentOpenRegex = new(
        @"<asp:Content\s+[^>]*ContentPlaceHolderID\s*=\s*""([^""]*)""[^>]*>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex ContentCloseRegex = new(
        @"</asp:Content>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex MasterPageFileRegex = new(
        @"MasterPageFile\s*=\s*""([^""]*)""",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Apply(string content, FileMetadata metadata)
    {
        if (metadata.FileType == FileType.Master)
            return content;

        // Convert <asp:Content ... ContentPlaceHolderID="X" ...> → <Content ContentPlaceHolderID="X">
        content = ContentOpenRegex.Replace(content, m =>
            $"<Content ContentPlaceHolderID=\"{m.Groups[1].Value}\">");

        // Convert </asp:Content> → </Content>
        content = ContentCloseRegex.Replace(content, "</Content>");

        // Determine master page component name from original content (before directive was stripped)
        var masterMatch = MasterPageFileRegex.Match(metadata.OriginalContent);
        if (!masterMatch.Success)
            return content;

        var masterFile = masterMatch.Groups[1].Value;
        var componentName = Path.GetFileNameWithoutExtension(masterFile);

        // Wrap all Content elements inside the master component
        var firstContentIndex = content.IndexOf("<Content ContentPlaceHolderID=", StringComparison.OrdinalIgnoreCase);
        var lastContentCloseIndex = content.LastIndexOf("</Content>", StringComparison.OrdinalIgnoreCase);

        if (firstContentIndex < 0 || lastContentCloseIndex < 0)
            return content;

        var endOfLastClose = lastContentCloseIndex + "</Content>".Length;

        content = content.Substring(0, firstContentIndex)
                  + $"<{componentName}>\n"
                  + content.Substring(firstContentIndex, endOfLastClose - firstContentIndex)
                  + $"\n</{componentName}>"
                  + content.Substring(endOfLastClose);

        return content;
    }
}
