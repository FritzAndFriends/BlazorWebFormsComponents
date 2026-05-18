using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

internal static partial class FormAntiforgeryPostProcessor
{
    private static readonly Regex FormRegex = new(
        @"<(?<tag>form|EditForm|WebFormsForm)\b(?<attributes>[^>]*)>(?<content>[\s\S]*?)</\k<tag>>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex AntiforgeryTokenRegex = new(
        @"<AntiforgeryToken\b[^>]*/>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex PostMethodRegex = new(
        @"\bmethod\s*=\s*(?:\""post\""|'post'|post)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static string Apply(string markup, FileMetadata? metadata = null)
    {
        if (string.IsNullOrWhiteSpace(markup))
        {
            return markup;
        }

        var formCounter = 0;
        var formNameBase = BuildFormNameBase(metadata);

        return FormRegex.Replace(markup, match =>
        {
            var tagName = match.Groups["tag"].Value;
            var attributes = match.Groups["attributes"].Value;
            var content = match.Groups["content"].Value;

            if (!RequiresContract(tagName, attributes))
            {
                return match.Value;
            }

            if (!HasFormName(tagName, attributes))
            {
                formCounter++;
                var formName = formCounter == 1 ? formNameBase : $"{formNameBase}{formCounter}";
                attributes += tagName.Equals("EditForm", StringComparison.OrdinalIgnoreCase)
                    ? $" FormName=\"{formName}\""
                    : $" @formname=\"{formName}\"";
            }

            if (!AntiforgeryTokenRegex.IsMatch(content))
            {
                content = InsertAntiforgeryToken(content);
            }

            return $"<{tagName}{attributes}>{content}</{tagName}>";
        });
    }

    private static bool RequiresContract(string tagName, string attributes)
    {
        return !tagName.Equals("form", StringComparison.OrdinalIgnoreCase)
            || PostMethodRegex.IsMatch(attributes);
    }

    private static bool HasFormName(string tagName, string attributes)
    {
        return tagName.Equals("EditForm", StringComparison.OrdinalIgnoreCase)
            ? attributes.Contains("FormName=", StringComparison.OrdinalIgnoreCase)
            : attributes.Contains("@formname=", StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildFormNameBase(FileMetadata? metadata)
    {
        var path = metadata?.OutputFilePath ?? metadata?.SourceFilePath;
        var pageName = string.IsNullOrWhiteSpace(path)
            ? "Page"
            : Path.GetFileNameWithoutExtension(path);

        pageName = Regex.Replace(pageName, @"[^A-Za-z0-9_]", string.Empty);
        return string.IsNullOrWhiteSpace(pageName) ? "PageForm" : $"{pageName}Form";
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
