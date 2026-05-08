using System.Text.RegularExpressions;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

internal static partial class FormAntiforgeryPostProcessor
{
    private static readonly Regex FormRegex = new(
        @"<(?<tag>form|EditForm)\b(?<attributes>[^>]*)>(?<content>[\s\S]*?)</\k<tag>>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex AntiforgeryTokenRegex = new(
        @"<AntiforgeryToken\s*/>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static string Apply(string markup)
    {
        if (string.IsNullOrWhiteSpace(markup))
        {
            return markup;
        }

        var formCounter = 0;
        return FormRegex.Replace(markup, match =>
        {
            var tagName = match.Groups["tag"].Value;
            var attributes = match.Groups["attributes"].Value;
            var content = match.Groups["content"].Value;

            if (!HasFormName(tagName, attributes))
            {
                formCounter++;
                attributes += tagName.Equals("EditForm", StringComparison.OrdinalIgnoreCase)
                    ? $" FormName=\"bwfc-form-{formCounter}\""
                    : $" @formname=\"bwfc-form-{formCounter}\"";
            }

            if (!AntiforgeryTokenRegex.IsMatch(content))
            {
                content = InsertAntiforgeryToken(content);
            }

            return $"<{tagName}{attributes}>{content}</{tagName}>";
        });
    }

    private static bool HasFormName(string tagName, string attributes)
    {
        return tagName.Equals("EditForm", StringComparison.OrdinalIgnoreCase)
            ? attributes.Contains("FormName=", StringComparison.OrdinalIgnoreCase)
            : attributes.Contains("@formname=", StringComparison.OrdinalIgnoreCase);
    }

    private static string InsertAntiforgeryToken(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return "\n    <AntiforgeryToken />\n";
        }

        if (content.StartsWith("\r\n", StringComparison.Ordinal))
        {
            return "\r\n    <AntiforgeryToken />" + content;
        }

        if (content.StartsWith("\n", StringComparison.Ordinal))
        {
            return "\n    <AntiforgeryToken />" + content;
        }

        return "\n    <AntiforgeryToken />\n" + content;
    }
}
