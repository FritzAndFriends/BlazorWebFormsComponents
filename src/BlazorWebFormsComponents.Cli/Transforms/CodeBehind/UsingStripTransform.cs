using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Strips Web Forms-specific using declarations from code-behind files.
/// Replaces System.Web.UI and System.Web.UI.WebControls with BlazorWebFormsComponents.CustomControls
/// (which provides WebControl, CompositeControl, UserControl, Control, INamingContainer).
/// Removes System.Web.Security, System.Web.*, Microsoft.AspNet.*, Microsoft.Owin.*, and Owin.
/// </summary>
public class UsingStripTransform : ICodeBehindTransform
{
    public string Name => "UsingStrip";
    public int Order => 100;

    // Replace with BWFC CustomControls namespace (provides WebControl, CompositeControl, UserControl, Control, etc.)
    private static readonly Regex WebUIWebControlsRegex = new(@"using\s+System\.Web\.UI\.WebControls;\s*\r?\n?", RegexOptions.Compiled);
    private static readonly Regex WebUIBareRegex = new(@"using\s+System\.Web\.UI;\s*\r?\n?", RegexOptions.Compiled);
    // Sub-namespaces of System.Web.UI (e.g., System.Web.UI.HtmlControls) — also remap
    private static readonly Regex WebUIOtherRegex = new(@"using\s+System\.Web\.UI\.\w+;\s*\r?\n?", RegexOptions.Compiled);

    // Strip entirely — no equivalent in Blazor
    private static readonly Regex WebSecurityRegex = new(@"using\s+System\.Web\.Security;\s*\r?\n?", RegexOptions.Compiled);
    private static readonly Regex WebUsingsRegex = new(@"using\s+System\.Web(\.\w+)*;\s*\r?\n?", RegexOptions.Compiled);
    private static readonly Regex AspNetUsingsRegex = new(@"using\s+Microsoft\.AspNet(\.\w+)*;\s*\r?\n?", RegexOptions.Compiled);
    private static readonly Regex OwinUsingsRegex = new(@"using\s+Microsoft\.Owin(\.\w+)*;\s*\r?\n?", RegexOptions.Compiled);
    private static readonly Regex BareOwinRegex = new(@"using\s+Owin;\s*\r?\n?", RegexOptions.Compiled);

    private const string CustomControlsUsing = "using BlazorWebFormsComponents.CustomControls;\n";

    public string Apply(string content, FileMetadata metadata)
    {
        // Replace UI namespaces with BWFC CustomControls (must run before the general System.Web.* strip)
        var needsCustomControls = WebUIWebControlsRegex.IsMatch(content)
            || WebUIBareRegex.IsMatch(content)
            || WebUIOtherRegex.IsMatch(content);

        content = WebUIWebControlsRegex.Replace(content, "");
        content = WebUIBareRegex.Replace(content, "");
        content = WebUIOtherRegex.Replace(content, "");

        // Strip the rest entirely
        content = WebSecurityRegex.Replace(content, "");
        content = WebUsingsRegex.Replace(content, "");
        content = AspNetUsingsRegex.Replace(content, "");
        content = OwinUsingsRegex.Replace(content, "");
        content = BareOwinRegex.Replace(content, "");

        // Add the CustomControls using if we replaced any UI namespace and it's not already there
        if (needsCustomControls && !content.Contains("using BlazorWebFormsComponents.CustomControls;", StringComparison.Ordinal))
        {
            // Insert after the last remaining using statement
            var lastUsing = Regex.Match(content, @"^using\s+[^;]+;\s*$", RegexOptions.Multiline | RegexOptions.RightToLeft);
            if (lastUsing.Success)
            {
                var insertAt = lastUsing.Index + lastUsing.Length;
                content = content[..insertAt] + "\n" + CustomControlsUsing + content[insertAt..];
            }
            else if (content.Length > 0)
            {
                content = CustomControlsUsing + content;
            }
        }

        return content;
    }
}
