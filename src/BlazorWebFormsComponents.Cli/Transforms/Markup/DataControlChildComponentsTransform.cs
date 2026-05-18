using System.Text;
using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Wraps style child elements of data controls (GridView, FormView, ListView, DataList,
/// DetailsView, Repeater, DataGrid) inside a &lt;ChildComponents&gt; element.
/// The existing TemplateFieldChildComponentsTransform (620) handles style elements inside
/// TemplateField blocks; this transform handles style elements that are direct children
/// of the data controls themselves.
/// Must run after AspPrefixTransform (610) and TemplateFieldChildComponentsTransform (620).
/// </summary>
public sealed class DataControlChildComponentsTransform : IMarkupTransform
{
    public string Name => "DataControlChildComponents";
    public int Order => 625;

    private static readonly string[] DataControlNames =
    [
        "GridView", "FormView", "ListView", "DataList",
        "DetailsView", "Repeater", "DataGrid"
    ];

    private static readonly string[] StyleElementNames =
    [
        "ItemStyle", "HeaderStyle", "FooterStyle",
        "ControlStyle", "EditItemStyle", "AlternatingItemStyle",
        "RowStyle", "AlternatingRowStyle", "SelectedRowStyle",
        "PagerStyle", "EmptyDataRowStyle",
        "EditRowStyle", "InsertRowStyle",
        "SortedAscendingCellStyle", "SortedAscendingHeaderStyle",
        "SortedDescendingCellStyle", "SortedDescendingHeaderStyle"
    ];

    public string Apply(string content, FileMetadata metadata)
    {
        foreach (var controlName in DataControlNames)
        {
            var regex = new Regex(
                $@"(?s)(<{controlName}\b[^>]*>)(.*?)(</{controlName}>)",
                RegexOptions.Compiled);

            var previous = string.Empty;
            do
            {
                previous = content;
                content = regex.Replace(content, m =>
                    m.Groups[1].Value + ProcessInner(m.Groups[2].Value) + m.Groups[3].Value);
            } while (!string.Equals(previous, content, StringComparison.Ordinal));
        }

        return content;
    }

    private static string ProcessInner(string inner)
    {
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
                    collectedStyles.Add((leadingIndent, trimmed));
                    i++;
                    continue;
                }

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

        var block = sb.ToString().TrimEnd('\n', '\r');
        block += $"\n{indent}</ChildComponents>";

        var remainingContent = string.Join("\n", remainingLines).TrimEnd('\n', '\r');
        return remainingContent + block + "\n";
    }

    private static bool TryMatchStyleElementName(string trimmed, out string? matchedName)
    {
        foreach (var name in StyleElementNames)
        {
            if (!trimmed.StartsWith($"<{name}", StringComparison.OrdinalIgnoreCase))
                continue;

            var afterNamePos = name.Length + 1;
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
