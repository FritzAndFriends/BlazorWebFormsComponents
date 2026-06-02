using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Transforms the Web Forms pattern of iterating a collection and calling control.Items.Add()
/// into a Blazor-compatible DataSource assignment pattern.
///
/// Before:
///   foreach (var item in collection) { control.Items.Add(item.Property); }
///
/// After:
///   control.DataSource = collection.Select(item => new ListItem(item.Property)).ToList();
///
/// Also handles simple .Items.Add("string") outside loops by converting to list initialization.
/// </summary>
public class ItemsAddToDataSourceTransform : ICodeBehindTransform
{
    public string Name => "ItemsAddToDataSource";
    public int Order => 715; // After DeadControlTreeCode (710)

    // Matches: foreach (var x in collection) { control.Items.Add(x.Prop); }
    // Captures: indent, varName, collection, controlRef, property access
    private static readonly Regex ForeachItemsAddRegex = new(
        @"(?<indent>[ \t]*)foreach\s*\(\s*var\s+(?<var>\w+)\s+in\s+(?<collection>[^\)]+)\)\s*\r?\n\s*\{\s*\r?\n\s*(?:this\.)?(?<control>\w+)\??\s*\.Items\s*\.Add\s*\(\s*(?<expr>[^)]+)\s*\)\s*;\s*\r?\n\s*\}",
        RegexOptions.Compiled | RegexOptions.Multiline);

    // Matches single-line: control.Items.Add(expr);
    private static readonly Regex SingleItemsAddRegex = new(
        @"(?<indent>[ \t]*)(?:this\.)?(?<control>\w+)\??\s*\.Items\s*\.Add\s*\(\s*(?<expr>[^)]+)\s*\)\s*;",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        if (!content.Contains(".Items.Add(", StringComparison.Ordinal) &&
            !content.Contains(".Items?.Add(", StringComparison.Ordinal))
            return content;

        var modified = content;

        // First: convert foreach + Items.Add patterns to DataSource assignment
        modified = ForeachItemsAddRegex.Replace(modified, match =>
        {
            var indent = match.Groups["indent"].Value;
            var varName = match.Groups["var"].Value;
            var collection = match.Groups["collection"].Value;
            var control = match.Groups["control"].Value;
            var expr = match.Groups["expr"].Value;

            // Build the Select lambda expression
            string selectExpr;
            if (expr.Contains(varName + "."))
            {
                // Expression uses the loop variable's property: item.Name → new ListItem(item.Name)
                selectExpr = $"{varName} => new ListItem({expr})";
            }
            else if (expr == varName)
            {
                // Simple case: adding the variable itself
                selectExpr = $"{varName} => new ListItem({varName})";
            }
            else
            {
                selectExpr = $"{varName} => new ListItem({expr})";
            }

            return $"{indent}{control}.DataSource = {collection}.Select({selectExpr}).ToList();";
        });

        return modified;
    }
}
