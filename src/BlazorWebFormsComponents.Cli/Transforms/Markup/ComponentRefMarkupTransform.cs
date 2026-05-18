using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Adds @ref bindings to BWFC component tags that have an id attribute.
/// In Web Forms, ID="X" on a server control auto-generates a code-behind field.
/// In Blazor, @ref="X" is needed for code-behind to reference the component instance.
/// Only applies when a code-behind file exists (otherwise @ref would have no backing field).
/// Skips controls inside template blocks (ItemTemplate, EditItemTemplate, etc.)
/// because @ref cannot capture per-row instances inside repeating templates.
/// </summary>
public class ComponentRefMarkupTransform : IMarkupTransform
{
    public string Name => "ComponentRef";
    public int Order => 750; // After AttributeStripTransform (700) which lowercases ID→id

    // Match opening tags for PascalCase components (Blazor/BWFC components start with uppercase)
    // Captures: (1) tag name, (2) all attributes, (3) tag closing /> or >
    private static readonly Regex ComponentTagRegex = new(
        @"<([A-Z]\w+)(\s[^>]*?)(\s*/?>)",
        RegexOptions.Compiled | RegexOptions.Singleline);

    // Match id="value" within attributes
    private static readonly Regex IdAttrRegex = new(
        @"\bid=""(\w+)""",
        RegexOptions.Compiled);

    // Generic BWFC components that need a type parameter in field declarations
    private static readonly HashSet<string> GenericComponents = new(StringComparer.Ordinal)
    {
        "GridView", "DetailsView", "DropDownList", "BulletedList",
        "Repeater", "ListView", "FormView", "RadioButtonList",
        "CheckBoxList", "ListBox", "DataList", "DataGrid",
        "RequiredFieldValidator", "CompareValidator", "RangeValidator"
    };

    // Template element names where @ref should NOT be added to child controls.
    // Controls inside these blocks are instantiated per data row — a single @ref field
    // cannot capture multiple instances.
    private static readonly HashSet<string> TemplateElements = new(StringComparer.OrdinalIgnoreCase)
    {
        "ItemTemplate", "EditItemTemplate", "AlternatingItemTemplate",
        "HeaderTemplate", "FooterTemplate", "InsertItemTemplate",
        "EmptyDataTemplate", "GroupTemplate", "LayoutTemplate",
        "SeparatorTemplate", "ContentTemplate", "SelectedItemTemplate"
    };

    // Matches opening and closing template tags to compute nesting ranges.
    private static readonly Regex TemplateTagRegex = new(
        @"<(/?)(" + string.Join("|", TemplateElements) + @")(?:\s[^>]*)?>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Apply(string content, FileMetadata metadata)
    {
        // Only add @ref if there's a code-behind to hold the field declarations
        if (metadata.CodeBehindContent == null)
            return content;

        var templateRanges = BuildTemplateRanges(content);

        return ComponentTagRegex.Replace(content, match =>
        {
            var tagName = match.Groups[1].Value;
            var attrs = match.Groups[2].Value;
            var closing = match.Groups[3].Value;

            var idMatch = IdAttrRegex.Match(attrs);
            if (!idMatch.Success)
                return match.Value;

            var controlId = idMatch.Groups[1].Value;

            // Skip if @ref already present
            if (attrs.Contains("@ref="))
                return match.Value;

            // Skip controls inside template blocks — @ref can't capture per-row instances
            if (IsInsideTemplate(match.Index, templateRanges))
                return match.Value;

            // Resolve the BWFC type for the code-behind field declaration
            var fieldType = ResolveFieldType(tagName, attrs);
            metadata.ComponentRefs[controlId] = fieldType;

            // Insert @ref="controlId" right after the id="controlId" attribute
            var refAttr = $" @ref=\"{controlId}\"";
            var newAttrs = attrs.Insert(idMatch.Index + idMatch.Length, refAttr);

            return $"<{tagName}{newAttrs}{closing}";
        });
    }

    /// <summary>
    /// Builds a list of (start, end) ranges for template block content.
    /// A position inside any range is considered "inside a template".
    /// Handles nested templates correctly via a depth counter per template name.
    /// </summary>
    internal static List<(int Start, int End)> BuildTemplateRanges(string content)
    {
        var ranges = new List<(int Start, int End)>();
        var openStack = new Stack<int>();

        foreach (Match m in TemplateTagRegex.Matches(content))
        {
            var isClosing = m.Groups[1].Value == "/";
            if (!isClosing)
            {
                // Opening tag — push the position just after the tag
                openStack.Push(m.Index + m.Length);
            }
            else if (openStack.Count > 0)
            {
                // Closing tag — pop the most recent opening and record the range
                var start = openStack.Pop();
                ranges.Add((start, m.Index));
            }
        }

        return ranges;
    }

    /// <summary>
    /// Returns true if the given position falls within any template range.
    /// </summary>
    internal static bool IsInsideTemplate(int position, List<(int Start, int End)> ranges)
    {
        foreach (var (start, end) in ranges)
        {
            if (position >= start && position < end)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Determines the Blazor field type for a component reference.
    /// For generic BWFC components, extracts the type parameter from ItemType attributes.
    /// </summary>
    public static string ResolveFieldType(string tagName, string tagAttributes)
    {
        if (!GenericComponents.Contains(tagName))
            return tagName;

        // Look for ItemType="Type" (normalized by AttributeStripTransform)
        // or the ItemType="object" fallback added for generic components.
        var typeMatch = Regex.Match(tagAttributes, @"(?:TItem|ItemType)=""([^""]+)""");
        var typeParam = typeMatch.Success ? typeMatch.Groups[1].Value : "object";
        return $"{tagName}<{typeParam}>";
    }
}
