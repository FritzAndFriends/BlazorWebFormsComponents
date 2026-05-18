using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Converts ajaxToolkit: prefix controls. Known controls get prefix stripped;
/// unknown controls are replaced with TODO comments.
/// ToolkitScriptManager is removed entirely.
/// Must run BEFORE AspPrefixTransform.
/// </summary>
public class AjaxToolkitPrefixTransform : IMarkupTransform
{
    public string Name => "AjaxToolkitPrefix";
    public int Order => 600;

    private static readonly string[] KnownControls =
    [
        "Accordion", "AccordionPane",
        "TabContainer", "TabPanel",
        "ConfirmButtonExtender", "FilteredTextBoxExtender",
        "ModalPopupExtender", "CollapsiblePanelExtender",
        "CalendarExtender", "AutoCompleteExtender",
        "MaskedEditExtender", "NumericUpDownExtender",
        "SliderExtender", "ToggleButtonExtender",
        "PopupControlExtender", "HoverMenuExtender"
    ];

    // ToolkitScriptManager — block form
    private static readonly Regex TsmBlockRegex = new(
        @"(?s)<ajaxToolkit:ToolkitScriptManager(?:[^>]|(?:%>))*?(?:>.*?</ajaxToolkit:ToolkitScriptManager>)\s*\r?\n?",
        RegexOptions.Compiled);

    // ToolkitScriptManager — self-closing form
    private static readonly Regex TsmSelfRegex = new(
        @"<ajaxToolkit:ToolkitScriptManager(?:[^>]|(?:%>))*?/>\s*\r?\n?",
        RegexOptions.Compiled);

    // Unknown self-closing
    private static readonly Regex UnknownSelfRegex = new(
        @"(?s)<ajaxToolkit:(\w+)(?:[^>]|(?:%>))*?/>",
        RegexOptions.Compiled);

    // Unknown block
    private static readonly Regex UnknownBlockRegex = new(
        @"(?s)<ajaxToolkit:(\w+)(?:[^>]|(?:%>))*?>.*?</ajaxToolkit:\1>",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        // 1. Strip ToolkitScriptManager entirely
        content = TsmBlockRegex.Replace(content, "");
        content = TsmSelfRegex.Replace(content, "");

        // 2. Convert known controls — strip prefix
        var knownPattern = string.Join("|", KnownControls.Select(Regex.Escape));
        var openRegex = new Regex($"<ajaxToolkit:({knownPattern})");
        var closeRegex = new Regex($"</ajaxToolkit:({knownPattern})>");

        content = openRegex.Replace(content, "<$1");
        content = closeRegex.Replace(content, "</$1>");

        // 3. Unknown self-closing → TODO
        content = UnknownSelfRegex.Replace(content,
            "@* TODO(bwfc-ajax-toolkit): Convert ajaxToolkit:$1 — no BWFC equivalent yet *@");

        // 4. Unknown block → TODO
        content = UnknownBlockRegex.Replace(content,
            "@* TODO(bwfc-ajax-toolkit): Convert ajaxToolkit:$1 — no BWFC equivalent yet *@");

        return content;
    }
}
