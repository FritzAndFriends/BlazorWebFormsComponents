using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Removes &lt;asp:Content&gt; wrappers, keeping only their inner content.
/// The Blazor layout system (DefaultLayout in Routes.razor) replaces
/// the Web Forms MasterPage/ContentPlaceHolder pattern, so no component
/// wrapper is needed.
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

    public string Apply(string content, FileMetadata metadata)
    {
        if (metadata.FileType == FileType.Master)
            return content;

        // Strip <asp:Content ...> open tags — the content IS the page body
        content = ContentOpenRegex.Replace(content, string.Empty);

        // Strip </asp:Content> close tags
        content = ContentCloseRegex.Replace(content, string.Empty);

        return content;
    }
}
