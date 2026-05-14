using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Replaces &lt;Label @ref="fieldName"&gt; components with field-bound &lt;span&gt; elements.
/// Label components are simple text-display controls. When their Text property is set
/// during OnInitializedAsync, @ref fields are null (component tree hasn't rendered yet),
/// making the assignment a silent no-op. Replacing with a span bound to a string field
/// ensures the text displays correctly.
///
/// Tracks replaced labels in FileMetadata.LabelFieldBindings for the code-behind transform.
/// </summary>
public class LabelFieldBindTransform : IMarkupTransform
{
    public string Name => "LabelFieldBind";
    public int Order => 760; // After ComponentRefMarkupTransform (750)

    // Matches <Label @ref="fieldName" ...> or <Label @ref="fieldName" .../> or <Label @ref="fieldName">text</Label>
    private static readonly Regex LabelWithRefRegex = new(
        @"<Label\b(?<attrs>[^>]*?)@ref=""(?<field>\w+)""(?<rest>[^>]*?)(?:/>|>(?<inner>[^<]*?)</Label>)",
        RegexOptions.Compiled | RegexOptions.Singleline);

    // Also match <Label id="fieldName" ...> that hasn't been converted to @ref yet
    private static readonly Regex LabelWithIdRegex = new(
        @"<Label\b(?<attrs>[^>]*?)\bid=""(?<field>\w+)""(?<rest>[^>]*?)(?:/>|>(?<inner>[^<]*?)</Label>)",
        RegexOptions.Compiled | RegexOptions.Singleline);

    // Extract Text="value" attribute
    private static readonly Regex TextAttrRegex = new(
        @"\bText=""(?<text>[^""]*)""",
        RegexOptions.Compiled);

    // Extract CssClass="value" attribute
    private static readonly Regex CssClassAttrRegex = new(
        @"\bCssClass=""(?<css>[^""]*)""",
        RegexOptions.Compiled);

    // Check if the code-behind references fieldName.Text
    private static readonly string TextAccessPattern = @"\.Text\b";

    public string Apply(string content, FileMetadata metadata)
    {
        // Only apply if there's a code-behind that references .Text on a label field
        if (metadata.CodeBehindContent == null)
            return content;

        content = ReplaceLabels(content, LabelWithRefRegex, metadata);
        content = ReplaceLabels(content, LabelWithIdRegex, metadata);

        return content;
    }

    private static string ReplaceLabels(string content, Regex regex, FileMetadata metadata)
    {
        return regex.Replace(content, match =>
        {
            var fieldName = match.Groups["field"].Value;

            // Only replace if the code-behind actually accesses fieldName.Text
            if (metadata.CodeBehindContent != null)
            {
                var textAccessRegex = new Regex($@"\b{Regex.Escape(fieldName)}\??\s*\.\s*Text\b");
                if (!textAccessRegex.IsMatch(metadata.CodeBehindContent))
                    return match.Value; // Leave as-is — code-behind doesn't set .Text
            }

            var attrs = match.Groups["attrs"].Value + match.Groups["rest"].Value;
            var inner = match.Groups["inner"].Success ? match.Groups["inner"].Value : "";

            // Extract Text attribute if present
            var textMatch = TextAttrRegex.Match(attrs);
            var defaultText = textMatch.Success ? textMatch.Groups["text"].Value : inner;

            // Extract CssClass
            var cssMatch = CssClassAttrRegex.Match(attrs);
            var cssClass = cssMatch.Success ? $" class=\"{cssMatch.Groups["css"].Value}\"" : "";

            // Generate the backing field name
            var backingField = $"_{fieldName}_Text";

            // Track the replacement for the code-behind transform
            metadata.LabelFieldBindings[fieldName] = backingField;

            // Remove from ComponentRefs if it was added by ComponentRefMarkupTransform
            metadata.ComponentRefs.Remove(fieldName);

            // Replace with field-bound span
            if (!string.IsNullOrEmpty(defaultText))
            {
                return $"<span{cssClass}>@{backingField}</span>";
            }
            return $"<span{cssClass}>@{backingField}</span>";
        });
    }
}
