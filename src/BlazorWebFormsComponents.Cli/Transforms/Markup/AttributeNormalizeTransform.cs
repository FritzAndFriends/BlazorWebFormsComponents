using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Normalizes boolean, enum, and unit attribute values in converted Blazor markup.
/// 1. Booleans: Visible="True" → Visible="true"
/// 2. Enums: GridLines="Both" → GridLines="@GridLines.Both"
/// 3. Units: Width="100px" → Width="100"
/// </summary>
public class AttributeNormalizeTransform : IMarkupTransform
{
    public string Name => "AttributeNormalize";
    public int Order => 810;

    private static readonly Regex BoolRegex = new(
        @"(\w+)=""(True|False)""",
        RegexOptions.Compiled);

    // Attributes that contain text content — not booleans even if "True"/"False"
    private static readonly HashSet<string> TextAttributes = new(StringComparer.OrdinalIgnoreCase)
    {
        "Text", "Title", "Value", "ToolTip", "HeaderText", "FooterText",
        "CommandName", "CommandArgument", "ErrorMessage", "InitialValue",
        "DataField", "DataFormatString", "SortExpression", "NavigateUrl",
        "DataTextField", "DataValueField", "ValidationExpression"
    };

    // Enum attribute → enum type mappings
    private static readonly Dictionary<string, string> EnumAttrMap = new()
    {
        ["GridLines"] = "GridLines",
        ["BorderStyle"] = "BorderStyle",
        ["HorizontalAlign"] = "HorizontalAlign",
        ["VerticalAlign"] = "VerticalAlign",
        ["TextAlign"] = "TextAlign",
        ["TextMode"] = "TextBoxMode",
        ["ImageAlign"] = "ImageAlign",
        ["Orientation"] = "Orientation",
        ["BulletStyle"] = "BulletStyle",
        ["CaptionAlign"] = "TableCaptionAlign",
        ["SortDirection"] = "SortDirection",
        ["ScrollBars"] = "ScrollBars",
        ["ContentDirection"] = "ContentDirection",
        ["DayNameFormat"] = "DayNameFormat",
        ["TitleFormat"] = "TitleFormat",
        ["InsertItemPosition"] = "InsertItemPosition",
        ["UpdateMode"] = "UpdatePanelUpdateMode",
        ["FontSize"] = "FontSize",
        ["Display"] = "ValidatorDisplay"
    };

    // Dimension attributes for px stripping
    private static readonly string[] UnitAttributes =
        ["Width", "Height", "BorderWidth", "CellPadding", "CellSpacing"];

    public string Apply(string content, FileMetadata metadata)
    {
        // Boolean normalization: True/False → true/false (skip text attributes)
        content = BoolRegex.Replace(content, m =>
        {
            var attr = m.Groups[1].Value;
            if (TextAttributes.Contains(attr))
                return m.Value;
            return $"{attr}=\"{m.Groups[2].Value.ToLowerInvariant()}\"";
        });

        // Enum type-qualifying
        foreach (var (attrName, enumType) in EnumAttrMap)
        {
            var enumRegex = new Regex($@"(?<!\w){Regex.Escape(attrName)}=""([A-Z][a-zA-Z0-9]*)""");
            content = enumRegex.Replace(content, m =>
            {
                var val = m.Groups[1].Value;
                // Skip boolean values
                if (val is "True" or "False" or "true" or "false")
                    return m.Value;
                return $"{attrName}=\"@{enumType}.{val}\"";
            });
        }

        // Unit normalization: strip "px" suffix
        foreach (var attr in UnitAttributes)
        {
            var unitRegex = new Regex($@"{Regex.Escape(attr)}=""(\d+)px""");
            content = unitRegex.Replace(content, $"{attr}=\"$1\"");
        }

        return content;
    }
}
