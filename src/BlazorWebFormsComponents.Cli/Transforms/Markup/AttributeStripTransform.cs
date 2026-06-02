using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Removes Web Forms-specific attributes (runat="server", AutoEventWireup, EnableViewState, etc.).
/// Preserves ItemType for BWFC generic components and adds ItemType="object" fallback when missing.
/// Converts ID= to id=.
/// </summary>
public class AttributeStripTransform : IMarkupTransform
{
    public string Name => "AttributeStrip";
    public int Order => 700;

    private static readonly string[] StripPatterns =
    [
        @"runat\s*=\s*""server""",
        @"AutoEventWireup\s*=\s*""(true|false)""",
        @"EnableViewState\s*=\s*""(true|false)""",
        @"ViewStateMode\s*=\s*""[^""]*""",
        @"ValidateRequest\s*=\s*""(true|false)""",
        @"MaintainScrollPositionOnPostBack\s*=\s*""(true|false)""",
        @"ClientIDMode\s*=\s*""[^""]*"""
    ];

    // ItemType="Namespace.Class" → ItemType="Class"
    private static readonly Regex ItemTypeRegex = new(
        @"ItemType=""(?:[^""]*\.)?([^""]+)""",
        RegexOptions.Compiled);

    // Generic BWFC components that need ItemType="object" fallback
    private static readonly string[] GenericComponents =
    [
        "GridView", "DetailsView", "DropDownList", "BoundField", "BulletedList",
        "Repeater", "ListView", "FormView", "RadioButtonList", "CheckBoxList", "ListBox",
        "HyperLinkField", "ButtonField", "TemplateField", "DataList", "DataGrid"
    ];

    // ID="..." → id="..."
    private static readonly Regex IdRegex = new(@"\bID=""([^""]*)""", RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        // Strip Web Forms attributes
        foreach (var pattern in StripPatterns)
        {
            var regex = new Regex($@"\s*{pattern}");
            content = regex.Replace(content, "");
        }

        // Normalize ItemType values to simple BWFC generic type names
        content = ItemTypeRegex.Replace(content, "ItemType=\"$1\"");

        // Add ItemType="object" fallback to generic components missing it
        foreach (var comp in GenericComponents)
        {
            var tagRegex = new Regex($@"(<{comp}\b)(?![^>]*ItemType=)([^/>]*)(>|/>)");
            content = tagRegex.Replace(content, "${1} ItemType=\"object\"${2}${3}");
        }

        // ID → id
        content = IdRegex.Replace(content, "id=\"$1\"");

        return content;
    }
}
