using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Removes Web Forms script/bundle infrastructure from migrated master-page shells.
/// </summary>
public class ScriptManagerStripTransform : IMarkupTransform
{
    private const string ScriptManagerComment = "@* Framework scripts are managed by Blazor — no ScriptManager needed. *@";

    private static readonly Regex ScriptManagerBlockRegex = new(
        @"[ \t]*<asp:ScriptManager\b[\s\S]*?</asp:ScriptManager>[ \t]*\r?\n?",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex BundleReferenceRegex = new(
        @"[ \t]*<webopt:bundlereference\b[^>]*/>[ \t]*\r?\n?",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex ScriptsRenderPlaceholderRegex = new(
        @"[ \t]*<asp:PlaceHolder\b[^>]*\brunat\s*=\s*""server""[^>]*>\s*(?:<%\s*:?\s*Scripts\.Render\s*\([^%]*?\)\s*%>\s*)+</asp:PlaceHolder>[ \t]*\r?\n?",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

    private static readonly Regex ExcessBlankLinesRegex = new(
        @"\n{3,}",
        RegexOptions.Compiled);

    public string Name => "ScriptManagerStrip";
    public int Order => 255;

    public string Apply(string content, FileMetadata metadata)
    {
        if (metadata.FileType != FileType.Master)
        {
            return content;
        }

        content = content.Replace("\r\n", "\n");

        var emittedComment = false;
        content = ScriptManagerBlockRegex.Replace(content, _ =>
        {
            if (emittedComment)
            {
                return string.Empty;
            }

            emittedComment = true;
            return ScriptManagerComment + "\n";
        });

        content = BundleReferenceRegex.Replace(content, string.Empty);
        content = ScriptsRenderPlaceholderRegex.Replace(content, string.Empty);
        content = ExcessBlankLinesRegex.Replace(content, "\n\n");

        return content.TrimEnd('\n') + "\n";
    }
}
