using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Strips Web Forms-specific using declarations from code-behind files.
/// Removes System.Web.UI, System.Web.UI.WebControls, System.Web.Security, System.Web.*,
/// Microsoft.AspNet.*, Microsoft.Owin.*, and Owin.
/// The CustomControls using is added by BaseClassStripTransform only when needed.
/// </summary>
public class UsingStripTransform : ICodeBehindTransform
{
    public string Name => "UsingStrip";
    public int Order => 100;

    // Strip all System.Web.UI.* namespaces
    private static readonly Regex WebUIWebControlsRegex = new(@"using\s+System\.Web\.UI\.WebControls;\s*\r?\n?", RegexOptions.Compiled);
    private static readonly Regex WebUIBareRegex = new(@"using\s+System\.Web\.UI;\s*\r?\n?", RegexOptions.Compiled);
    private static readonly Regex WebUIOtherRegex = new(@"using\s+System\.Web\.UI\.\w+;\s*\r?\n?", RegexOptions.Compiled);

    // Strip entirely — no equivalent in Blazor
    private static readonly Regex WebSecurityRegex = new(@"using\s+System\.Web\.Security;\s*\r?\n?", RegexOptions.Compiled);
    private static readonly Regex WebUsingsRegex = new(@"using\s+System\.Web(\.\w+)*;\s*\r?\n?", RegexOptions.Compiled);
    private static readonly Regex AspNetUsingsRegex = new(@"using\s+Microsoft\.AspNet(\.\w+)*;\s*\r?\n?", RegexOptions.Compiled);
    private static readonly Regex OwinUsingsRegex = new(@"using\s+Microsoft\.Owin(\.\w+)*;\s*\r?\n?", RegexOptions.Compiled);
    private static readonly Regex BareOwinRegex = new(@"using\s+Owin;\s*\r?\n?", RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        // Replace UI namespaces — these are removed (BWFC components are in BlazorWebFormsComponents namespace)
        content = WebUIWebControlsRegex.Replace(content, "");
        content = WebUIBareRegex.Replace(content, "");
        content = WebUIOtherRegex.Replace(content, "");

        // Strip the rest entirely
        content = WebSecurityRegex.Replace(content, "");
        content = WebUsingsRegex.Replace(content, "");
        content = AspNetUsingsRegex.Replace(content, "");
        content = OwinUsingsRegex.Replace(content, "");
        content = BareOwinRegex.Replace(content, "");

        return content;
    }
}
