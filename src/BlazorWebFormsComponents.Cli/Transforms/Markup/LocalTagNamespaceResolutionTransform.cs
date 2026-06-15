using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Strips namespace-level tag prefixes registered in Web.config via
/// &lt;add tagPrefix="local" namespace="..." assembly="..." /&gt; entries.
/// Converts &lt;local:SectionPanel ...&gt; → &lt;SectionPanel ...&gt; for all namespace-bound prefixes.
/// Handles both self-closing and block-level element forms.
/// Must run before AjaxToolkitPrefixTransform (Order 600) and AspPrefixTransform (Order 610).
/// </summary>
public class LocalTagNamespaceResolutionTransform : IMarkupTransform
{
    public string Name => "LocalTagNamespaceResolution";
    public int Order => 595;

    // Prefixes handled by other dedicated transforms — skip them here to avoid double-processing
    private static readonly HashSet<string> SkippedPrefixes = new(StringComparer.OrdinalIgnoreCase)
    {
        "asp",
        "ajaxToolkit"
    };

    public string Apply(string content, FileMetadata metadata)
    {
        var prefixMap = metadata.CustomControlPrefixToNamespace;
        if (prefixMap.Count == 0)
            return content;

        foreach (var prefix in prefixMap.Keys)
        {
            if (SkippedPrefixes.Contains(prefix))
                continue;

            var escapedPrefix = Regex.Escape(prefix);

            // Opening tag: <local:SectionPanel → <SectionPanel
            content = Regex.Replace(
                content,
                $"<{escapedPrefix}:(\\w+)",
                "<$1",
                RegexOptions.IgnoreCase);

            // Closing tag: </local:SectionPanel> → </SectionPanel>
            content = Regex.Replace(
                content,
                $"</{escapedPrefix}:(\\w+)>",
                "</$1>",
                RegexOptions.IgnoreCase);
        }

        return content;
    }
}
