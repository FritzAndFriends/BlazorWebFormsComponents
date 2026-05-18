using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Replaces Web Forms base class declarations with WebFormsPageBase for Page/Master types,
/// or strips them entirely for Control/CodeFile types.
/// Page and Master code-behind classes get ": WebFormsPageBase" so pages automatically have
/// access to Request, Response, Session, Server, Cache, ClientScript, IsPostBack, and ViewState shims.
/// </summary>
public class BaseClassStripTransform : ICodeBehindTransform
{
    public string Name => "BaseClassStrip";
    public int Order => 200;

    private static readonly Regex BaseClassRegex = new(
        @"(partial\s+class\s+\w+)\s*:\s*(System\.Web\.UI\.Page|System\.Web\.UI\.MasterPage|System\.Web\.UI\.UserControl|Microsoft\.AspNetCore\.Components\.ComponentBase|(?<!\w)Page(?!\w)|(?<!\w)MasterPage(?!\w)|(?<!\w)UserControl(?!\w)|(?<!\w)ComponentBase(?!\w))",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        if (metadata.FileType is FileType.Page or FileType.Master)
        {
            // Replace the Web Forms base class with WebFormsPageBase
            return BaseClassRegex.Replace(content, "$1 : WebFormsPageBase");
        }

        // For Controls and CodeFiles, strip the base class entirely
        return BaseClassRegex.Replace(content, "$1");
    }
}
