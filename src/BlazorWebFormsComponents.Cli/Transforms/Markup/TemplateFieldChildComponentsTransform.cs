using System.Text;
using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Wraps TemplateField child style elements (ItemStyle, HeaderStyle, FooterStyle, etc.)
/// inside a &lt;ChildComponents&gt; element as required by BWFC's TemplateField API.
/// Template fragments (ItemTemplate, HeaderTemplate, etc.) are left in place.
/// Must run after AspPrefixTransform (610) so asp: prefixes have been stripped.
/// </summary>
public class TemplateFieldChildComponentsTransform : IMarkupTransform
{
    public string Name => "TemplateFieldChildComponents";
    public int Order => 620;

    private static readonly string[] StyleElementNames =
    [
        "ItemStyle", "HeaderStyle", "FooterStyle",
        "ControlStyle", "EditItemStyle", "AlternatingItemStyle"
    ];

    // Matches a TemplateField block — non-greedy so adjacent siblings are handled correctly.
    // Applied iteratively so that any edge-case nesting is also processed.
    private static readonly Regex TemplateFieldRegex = new(
        @"(?s)(<TemplateField\b[^>]*>)(.*?)(</TemplateField>)",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        // Iterate until stable so that any nested TemplateField structures are fully processed.
        var previous = string.Empty;
        do
        {
            previous = content;
            content = TemplateFieldRegex.Replace(content, m =>
                m.Groups[1].Value + ProcessInner(m.Groups[2].Value) + m.Groups[3].Value);
        } while (content != previous);

        return content;
    }

    private static string ProcessInner(string inner)
    {
        // Already wrapped — don't add a second ChildComponents.
        if (inner.Contains("<ChildComponents"))
            return inner;

        var rawLines = inner.Split('\n');
        var remainingLines = new List<string>();
        var collectedStyles = new List<(string LeadingIndent, string Element)>();

        var i = 0;
        while (i < rawLines.Length)
        {
            var line = rawLines[i];
            var trimmed = line.TrimStart().TrimEnd('\r');
            var leadingIndent = line.Length - line.TrimStart().Length > 0
                ? line[..(line.Length - line.TrimStart().Length)].TrimEnd('\r')
                : string.Empty;

            if (TryMatchStyleElementName(trimmed, out var matchedName))
            {
                if (IsSingleLineElement(trimmed, matchedName!))
                {
                    // Self-closing or complete open/close on one line
                    collectedStyles.Add((leadingIndent, trimmed));
                    i++;
                    continue;
                }

                // Multi-line open/close: accumulate until we see the closing tag
                var elementParts = new List<string> { trimmed };
                i++;
                while (i < rawLines.Length)
                {
                    var part = rawLines[i].TrimEnd('\r');
                    elementParts.Add(part.TrimStart());
                    if (part.TrimStart().StartsWith($"</{matchedName}>",
                            StringComparison.OrdinalIgnoreCase))
                        break;
                    i++;
                }
                collectedStyles.Add((leadingIndent, string.Join("\n", elementParts)));
                i++;
                continue;
            }

            remainingLines.Add(line);
            i++;
        }

        if (collectedStyles.Count == 0)
            return inner;

        // Inherit the indentation level of the first collected style element.
        var indent = collectedStyles[0].LeadingIndent;
        var extraIndent = indent + "    ";

        var sb = new StringBuilder();
        sb.Append('\n');
        sb.AppendLine($"{indent}<ChildComponents>");
        foreach (var (_, element) in collectedStyles)
        {
            foreach (var part in element.Split('\n'))
                sb.AppendLine($"{extraIndent}{part.TrimStart()}");
        }

        // Strip the trailing newline that AppendLine added, then close the wrapper.
        var block = sb.ToString().TrimEnd('\n', '\r');
        block += $"\n{indent}</ChildComponents>";

        // Reassemble: remaining lines first, then the ChildComponents block.
        var remainingContent = string.Join("\n", remainingLines).TrimEnd('\n', '\r');
        return remainingContent + block + "\n";
    }

    /// <summary>
    /// Returns true if <paramref name="trimmed"/> starts a known style element tag.
    /// Sets <paramref name="matchedName"/> to the element name on a match.
    /// </summary>
    private static bool TryMatchStyleElementName(string trimmed, out string? matchedName)
    {
        foreach (var name in StyleElementNames)
        {
            if (!trimmed.StartsWith($"<{name}", StringComparison.OrdinalIgnoreCase))
                continue;

            // Ensure the character after the tag name is a valid tag delimiter,
            // not part of a longer identifier (e.g. <ItemStyleExtra>).
            var afterNamePos = name.Length + 1; // length of "<name"
            if (afterNamePos < trimmed.Length)
            {
                var c = trimmed[afterNamePos];
                if (char.IsLetterOrDigit(c) || c == '_')
                    continue;
            }

            matchedName = name;
            return true;
        }

        matchedName = null;
        return false;
    }

    private static bool IsSingleLineElement(string trimmed, string tagName)
    {
        var t = trimmed.TrimEnd();
        return t.EndsWith("/>") || t.Contains($"</{tagName}>");
    }
}
