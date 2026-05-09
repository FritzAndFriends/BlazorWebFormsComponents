using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Removes Web Forms and Blazor base class declarations from code-behind partial classes.
/// The .razor file handles inheritance via @inherits (typically WebFormsPageBase from _Imports.razor),
/// so the partial class must not declare a conflicting base class.
/// Strips: System.Web.UI.Page/MasterPage/UserControl, ComponentBase, and their short forms.
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
        return BaseClassRegex.Replace(content, "$1");
    }
}
