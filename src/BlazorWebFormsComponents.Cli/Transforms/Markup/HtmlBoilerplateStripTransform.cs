using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Strips HTML document boilerplate (DOCTYPE, html, head, body tags) from content pages
/// that don't use a master page. Pages using a master page already have this handled by
/// MasterPageTransform. This transform ensures that non-master pages using their own
/// HTML document structure get cleaned down to just their content for Blazor layouts.
/// Also handles redirect-only pages by generating minimal markup with WebFormsPageBase inheritance.
/// </summary>
public class HtmlBoilerplateStripTransform : IMarkupTransform
{
    public string Name => "HtmlBoilerplateStrip";
    public int Order => 260; // Run after MasterPageTransform (250), ContentPage (255)

    private static readonly Regex DocTypeRegex = new(
        @"[ \t]*<!DOCTYPE[^>]*>[ \t]*\r?\n?",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex HtmlOpenRegex = new(
        @"[ \t]*<html(?:\s[^>]*)?>[ \t]*\r?\n?",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex HtmlCloseRegex = new(
        @"[ \t]*</html>[ \t]*\r?\n?",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex BodyOpenRegex = new(
        @"[ \t]*<body(?:\s[^>]*)?>[ \t]*\r?\n?",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex BodyCloseRegex = new(
        @"[ \t]*</body>[ \t]*\r?\n?",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex HeadSectionRegex = new(
        @"[ \t]*<head\b[^>]*>[\s\S]*?</head>[ \t]*\r?\n?",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // Server form wrappers: <form runat="server">...</form>
    private static readonly Regex ServerFormOpenRegex = new(
        @"[ \t]*<form\b[^>]*\s+runat\s*=\s*""server""[^>]*>[ \t]*\r?\n?",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex ServerFormCloseRegex = new(
        @"[ \t]*</form>[ \t]*\r?\n?",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Apply(string content, FileMetadata metadata)
    {
        // Only process page files (not master pages — those are handled by MasterPageTransform)
        if (metadata.FileType != FileType.Page)
            return content;

        // Only strip if the content actually has HTML document structure
        if (!DocTypeRegex.IsMatch(content) && !HtmlOpenRegex.IsMatch(content))
            return content;

        // Strip outer HTML document boilerplate
        content = DocTypeRegex.Replace(content, "");
        content = HeadSectionRegex.Replace(content, "");
        content = HtmlOpenRegex.Replace(content, "");
        content = HtmlCloseRegex.Replace(content, "");
        content = BodyOpenRegex.Replace(content, "");
        content = BodyCloseRegex.Replace(content, "");

        // Strip server form wrappers (unwrap content)
        content = ServerFormOpenRegex.Replace(content, "");
        content = ServerFormCloseRegex.Replace(content, "");

        // Clean up excessive blank lines left by stripping
        content = Regex.Replace(content, @"\n{3,}", "\n\n");

        return content.Trim() + "\n";
    }
}
