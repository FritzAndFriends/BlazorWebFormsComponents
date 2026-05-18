using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Strips Web Forms-specific using declarations from code-behind files.
/// Removes System.Web.UI.*, System.Web.Security, System.Web.*, Microsoft.AspNet.*, Microsoft.Owin.*, and Owin.
/// </summary>
public class UsingStripTransform : ICodeBehindTransform
{
    public string Name => "UsingStrip";
    public int Order => 100;

    // Ordered from most specific to least specific to avoid double-matching
    private static readonly Regex WebUIUsingsRegex = new(@"using\s+System\.Web\.UI(\.\w+)*;\s*\r?\n?", RegexOptions.Compiled);
    private static readonly Regex WebSecurityRegex = new(@"using\s+System\.Web\.Security;\s*\r?\n?", RegexOptions.Compiled);
    private static readonly Regex WebUsingsRegex = new(@"using\s+System\.Web(\.\w+)*;\s*\r?\n?", RegexOptions.Compiled);
    private static readonly Regex AspNetUsingsRegex = new(@"using\s+Microsoft\.AspNet(\.\w+)*;\s*\r?\n?", RegexOptions.Compiled);
    private static readonly Regex OwinUsingsRegex = new(@"using\s+Microsoft\.Owin(\.\w+)*;\s*\r?\n?", RegexOptions.Compiled);
    private static readonly Regex BareOwinRegex = new(@"using\s+Owin;\s*\r?\n?", RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        content = WebUIUsingsRegex.Replace(content, "");
        content = WebSecurityRegex.Replace(content, "");
        content = WebUsingsRegex.Replace(content, "");
        content = AspNetUsingsRegex.Replace(content, "");
        content = OwinUsingsRegex.Replace(content, "");
        content = BareOwinRegex.Replace(content, "");
        return content;
    }
}
