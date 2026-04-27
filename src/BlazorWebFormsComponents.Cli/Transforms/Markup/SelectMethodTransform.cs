using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Preserves SelectMethod on BWFC data-bound controls because DataBoundComponent already
/// supports delegate-style SelectMethod binding in markup. Insert/Update/Delete methods
/// still need manual review and therefore get TODO comments.
/// </summary>
public class SelectMethodTransform : IMarkupTransform
{
    public string Name => "SelectMethod";
    public int Order => 520;

    private static readonly Regex CrudMethodAttrRegex = new(
        @"(InsertMethod|UpdateMethod|DeleteMethod)=""([^""]+)""",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        var lines = content.Split('\n');
        var result = new List<string>();

        foreach (var line in lines)
        {
            result.Add(line);

            var matches = CrudMethodAttrRegex.Matches(line);
            foreach (Match match in matches)
            {
                var attrName = match.Groups[1].Value;
                var methodName = match.Groups[2].Value;
                result.Add(
                    $"@* TODO(bwfc-select-method): Review {attrName}=\"{methodName}\" migration for BWFC event/CRUD handling *@");
            }
        }

        return string.Join('\n', result);
    }
}
