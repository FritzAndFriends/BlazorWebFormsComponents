using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Adds @ref bindings to BWFC component tags that have an id attribute.
/// In Web Forms, ID="X" on a server control auto-generates a code-behind field.
/// In Blazor, @ref="X" is needed for code-behind to reference the component instance.
/// Only applies when a code-behind file exists (otherwise @ref would have no backing field).
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

    public string Apply(string content, FileMetadata metadata)
    {
        // Only add @ref if there's a code-behind to hold the field declarations
        if (metadata.CodeBehindContent == null)
            return content;

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
    /// Determines the Blazor field type for a component reference.
    /// For generic BWFC components, extracts the type parameter from TItem or ItemType attributes.
    /// </summary>
    public static string ResolveFieldType(string tagName, string tagAttributes)
    {
        if (!GenericComponents.Contains(tagName))
            return tagName;

        // Look for TItem="Type" (already converted from ItemType by AttributeStripTransform)
        // or ItemType="object" (fallback added by AttributeStripTransform for generic components)
        var typeMatch = Regex.Match(tagAttributes, @"(?:TItem|ItemType)=""([^""]+)""");
        var typeParam = typeMatch.Success ? typeMatch.Groups[1].Value : "object";
        return $"{tagName}<{typeParam}>";
    }
}
