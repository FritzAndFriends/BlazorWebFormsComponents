using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Replaces Web Forms base class declarations with WebFormsPageBase for Page/Master types,
/// or strips them for unknown base classes. Preserves WebControl, CompositeControl, UserControl,
/// and Control base classes since those have BWFC equivalents in BlazorWebFormsComponents.CustomControls.
/// Page and Master code-behind classes get ": WebFormsPageBase" so pages automatically have
/// access to Request, Response, Session, Server, Cache, ClientScript, IsPostBack, and ViewState shims.
/// </summary>
public class BaseClassStripTransform : ICodeBehindTransform
{
    public string Name => "BaseClassStrip";
    public int Order => 200;

    // Matches page/master base classes that should become WebFormsPageBase
    private static readonly Regex PageBaseClassRegex = new(
        @"(partial\s+class\s+\w+)\s*:\s*(System\.Web\.UI\.Page|System\.Web\.UI\.MasterPage|Microsoft\.AspNetCore\.Components\.ComponentBase|(?<!\w)Page(?!\w)|(?<!\w)MasterPage(?!\w)|(?<!\w)ComponentBase(?!\w))",
        RegexOptions.Compiled);

    // Matches control base classes that should be preserved (they exist in BWFC.CustomControls)
    private static readonly Regex PreservedBaseClassRegex = new(
        @":\s*(System\.Web\.UI\.)?(?:WebControl|CompositeControl|UserControl|Control)\b",
        RegexOptions.Compiled);

    // Matches UserControl base class with fully-qualified or bare name (for stripping the namespace prefix)
    private static readonly Regex UserControlBaseRegex = new(
        @"(partial\s+class\s+\w+)\s*:\s*System\.Web\.UI\.UserControl\b",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        if (metadata.FileType is FileType.Page or FileType.Master)
        {
            // Replace the Web Forms base class with WebFormsPageBase
            return PageBaseClassRegex.Replace(content, "$1 : WebFormsPageBase");
        }

        // For Controls and CodeFiles: preserve custom control base classes
        // Strip the System.Web.UI. prefix but keep the type name
        content = UserControlBaseRegex.Replace(content, "$1 : UserControl");

        // For files inheriting from preserved base classes, don't strip
        if (PreservedBaseClassRegex.IsMatch(content))
            return content;

        // Strip any other Web Forms base classes (e.g., bare Page in a control context)
        return PageBaseClassRegex.Replace(content, "$1");
    }
}
