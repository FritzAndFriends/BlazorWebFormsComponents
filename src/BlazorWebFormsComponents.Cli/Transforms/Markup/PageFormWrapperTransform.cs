using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Wraps page content in &lt;WebFormsForm&gt; to replicate Web Forms behavior where
/// all content was inside a server form. Runs late in the pipeline and only wraps
/// pages that don't already have a WebFormsForm or form element.
/// </summary>
public class PageFormWrapperTransform : IMarkupTransform
{
    public string Name => "PageFormWrapper";
    public int Order => 930;

    private static readonly Regex ExistingFormRegex = new(
        @"<(?:WebFormsForm|form\b)[^>]*>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex PageDirectiveRegex = new(
        @"^(@page\s+""[^""]*""\s*\n?)",
        RegexOptions.Compiled | RegexOptions.Multiline);

    public string Apply(string content, FileMetadata metadata)
    {
        // Only wrap page files (not layouts, not user controls)
        if (metadata.FileType != FileType.Page)
            return content;

        // Skip if the page already contains a form or WebFormsForm
        if (ExistingFormRegex.IsMatch(content))
            return content;

        // Skip pages with no meaningful content (quarantined stubs, empty pages)
        var trimmed = content.Trim();
        if (string.IsNullOrWhiteSpace(trimmed) || trimmed.Length < 20)
            return content;

        // Insert <WebFormsForm> after @page directive(s) and wrap remaining content
        var pageMatch = PageDirectiveRegex.Match(content);
        if (pageMatch.Success)
        {
            // Find the end of all @-directives at the top of the file
            var directiveEnd = FindDirectiveEnd(content);
            var directives = content[..directiveEnd];
            var body = content[directiveEnd..];

            return $"{directives}<WebFormsForm>\n{body.TrimStart('\r', '\n')}\n</WebFormsForm>\n";
        }

        // No @page directive — wrap entire content
        return $"<WebFormsForm>\n{content}\n</WebFormsForm>\n";
    }

    private static int FindDirectiveEnd(string content)
    {
        var lines = content.Split('\n');
        var pos = 0;

        foreach (var line in lines)
        {
            var trimmedLine = line.TrimStart();
            if (trimmedLine.StartsWith("@") && !trimmedLine.StartsWith("@{")
                && !trimmedLine.StartsWith("@if") && !trimmedLine.StartsWith("@foreach")
                && !trimmedLine.StartsWith("@code") && !trimmedLine.StartsWith("@*"))
            {
                pos += line.Length + 1; // +1 for \n
            }
            else if (string.IsNullOrWhiteSpace(line))
            {
                pos += line.Length + 1; // skip blank lines between directives
            }
            else
            {
                break;
            }
        }

        return pos;
    }
}
