using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Converts BWFC <c>&lt;Button&gt;</c> components inside <c>&lt;form&gt;</c> or
/// <c>&lt;WebFormsForm&gt;</c> elements to native <c>&lt;input type="submit"&gt;</c> elements
/// for static SSR compatibility. In static SSR, Blazor components cannot handle click events —
/// only HTML form submissions work. Each button gets <c>name="__action"</c> with <c>value</c>
/// set to the button's Text, enabling server-side action dispatch via SupplyParameterFromForm.
/// </summary>
public class ButtonToSubmitTransform : IMarkupTransform
{
    public string Name => "ButtonToSubmit";
    public int Order => 945; // After SsrFormContract (940) so forms already have @formname

    // Match <Button ... /> or <Button ...>...</Button> inside content
    // Captures: ID, Text, OnClick, CssClass attributes
    private static readonly Regex ButtonRegex = new(
        @"<Button\b(?<attrs>[^>]*?)(?:/>|>(?<inner>[^<]*)</Button>)",
        RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex TextAttrRegex = new(
        @"\bText\s*=\s*""(?<val>[^""]*)""",
        RegexOptions.Compiled);

    private static readonly Regex IdAttrRegex = new(
        @"\bID\s*=\s*""(?<val>[^""]*)""",
        RegexOptions.Compiled);

    private static readonly Regex CssClassAttrRegex = new(
        @"\bCssClass\s*=\s*""(?<val>[^""]*)""",
        RegexOptions.Compiled);

    // Detect if content has a plain HTML POST form (not WebFormsForm which handles events via JS)
    private static readonly Regex FormWrapperRegex = new(
        @"<form\b[^>]*method\s*=\s*""post""[^>]*>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Apply(string content, FileMetadata metadata)
    {
        // Only apply to pages with plain HTML POST forms (not WebFormsForm which uses JS interop)
        if (!FormWrapperRegex.IsMatch(content))
            return content;

        // Skip pages that use WebFormsForm — those handle button events via __doPostBack JS interop
        if (content.Contains("<WebFormsForm", StringComparison.Ordinal))
            return content;

        // Don't apply to pages that use interactive render mode
        if (content.Contains("@rendermode", StringComparison.OrdinalIgnoreCase)
            && (content.Contains("InteractiveServer", StringComparison.Ordinal)
                || content.Contains("InteractiveWebAssembly", StringComparison.Ordinal)
                || content.Contains("InteractiveAuto", StringComparison.Ordinal)))
            return content;

        return ButtonRegex.Replace(content, match =>
        {
            var attrs = match.Groups["attrs"].Value;

            // Skip buttons that don't have OnClick (they're just display buttons)
            if (!attrs.Contains("OnClick", StringComparison.OrdinalIgnoreCase))
                return match.Value;

            var textMatch = TextAttrRegex.Match(attrs);
            var idMatch = IdAttrRegex.Match(attrs);
            var cssMatch = CssClassAttrRegex.Match(attrs);

            var text = textMatch.Success ? textMatch.Groups["val"].Value : "Submit";
            var id = idMatch.Success ? $" id=\"{idMatch.Groups["val"].Value}\"" : "";
            var css = cssMatch.Success ? $" class=\"{cssMatch.Groups["val"].Value}\"" : "";

            return $"<input type=\"submit\"{id}{css} name=\"__action\" value=\"{text}\" />";
        });
    }
}
