using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Converts event handler attribute values: OnClick="Handler" → OnClick="@Handler".
/// Only applies to known Web Forms server-side event attributes.
/// </summary>
public class EventWiringTransform : IMarkupTransform
{
    public string Name => "EventWiring";
    public int Order => 710;

    // Known Web Forms server-side event attributes
    private static readonly string[] EventAttributes =
    [
        "OnClick", "OnCommand", "OnTextChanged", "OnSelectedIndexChanged",
        "OnCheckedChanged", "OnRowCommand", "OnRowEditing", "OnRowUpdating",
        "OnRowCancelingEdit", "OnRowDeleting", "OnRowDataBound",
        "OnPageIndexChanging", "OnSorting", "OnItemCommand", "OnItemDataBound",
        "OnDataBound", "OnLoad", "OnInit", "OnPreRender",
        "OnSelectedDateChanged", "OnDayRender", "OnVisibleMonthChanged",
        "OnServerValidate", "OnCreatingUser", "OnCreatedUser",
        "OnAuthenticate", "OnLoggedIn", "OnLoggingIn"
    ];

    public string Apply(string content, FileMetadata metadata)
    {
        foreach (var attr in EventAttributes)
        {
            // Match attr="HandlerName" where value is NOT already @-prefixed
            var regex = new Regex($@"{Regex.Escape(attr)}=""(?!@)(\w+)""");
            content = regex.Replace(content, $"{attr}=\"@$1\"");
        }

        return content;
    }
}
