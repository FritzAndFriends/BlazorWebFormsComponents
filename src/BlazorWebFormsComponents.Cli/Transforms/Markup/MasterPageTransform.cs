using System.Text;
using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Converts .master files to BWFC MasterPage component syntax.
/// Preserves named ContentPlaceHolder relationships using BWFC &lt;ContentPlaceHolder ID="X"&gt;,
/// wraps layout body in &lt;MasterPage&gt;, extracts CSS &lt;link&gt; refs for App.razor,
/// and strips the outer HTML document scaffold (DOCTYPE / html / body).
/// </summary>
public class MasterPageTransform : IMarkupTransform
{
    public string Name => "MasterPage";
    public int Order => 250;

    // Block: <asp:ContentPlaceHolder attrs>...</asp:ContentPlaceHolder>
    private static readonly Regex ContentPlaceHolderBlockRegex = new(
        @"<asp:ContentPlaceHolder\s([^>]*)>([\s\S]*?)</asp:ContentPlaceHolder>[ \t]*\r?\n?",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // Self-closing: <asp:ContentPlaceHolder attrs />
    private static readonly Regex ContentPlaceHolderSelfClosingRegex = new(
        @"<asp:ContentPlaceHolder\s+([^>]*?)/>[ \t]*\r?\n?",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // Extract ID="..." from a tag attribute string
    private static readonly Regex TagIdRegex = new(
        @"\bID\s*=\s*""([^""]*)""",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // runat="server" on <head> and <form> tags
    private static readonly Regex HeadRunatRegex = new(
        @"(<head\b[^>]*?)\s+runat\s*=\s*""server""([^>]*>)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex FormRunatRegex = new(
        @"(<form\b[^>]*?)\s+runat\s*=\s*""server""([^>]*>)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // Outer HTML document boilerplate
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

    // <head>...</head> section (captures inner content)
    private static readonly Regex HeadSectionRegex = new(
        @"[ \t]*<head\b[^>]*>([\s\S]*?)</head>[ \t]*\r?\n?",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // CSS <link rel="stylesheet"> elements (full line)
    private static readonly Regex CssLinkRegex = new(
        @"[ \t]*<link\b[^>]*\brel\s*=\s*""stylesheet""[^>]*/?>[ \t]*\r?\n?",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // href="..." value extraction
    private static readonly Regex LinkHrefRegex = new(
        @"\bhref\s*=\s*""([^""]*)""",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Apply(string content, FileMetadata metadata)
    {
        if (metadata.FileType != FileType.Master)
            return content;

        // Normalise line endings for consistent regex behaviour
        content = content.Replace("\r\n", "\n");

        // Step 1: Strip runat="server" from <head> and <form> structural tags
        content = HeadRunatRegex.Replace(content, "$1$2");
        content = FormRunatRegex.Replace(content, "$1$2");

        // Step 2: Convert block ContentPlaceHolder → named BWFC component (preserve default content)
        content = ContentPlaceHolderBlockRegex.Replace(content, m =>
        {
            var attrs = m.Groups[1].Value;
            var inner = m.Groups[2].Value;
            var id = ExtractId(attrs);
            return $"<ContentPlaceHolder ID=\"{id}\">{inner}</ContentPlaceHolder>\n";
        });

        // Step 3: Convert self-closing ContentPlaceHolder → named BWFC component
        content = ContentPlaceHolderSelfClosingRegex.Replace(content, m =>
        {
            var attrs = m.Groups[1].Value;
            var id = ExtractId(attrs);
            return $"<ContentPlaceHolder ID=\"{id}\" />\n";
        });

        // Step 4: Extract <head> section — split CSS links (→ App.razor) from other head content
        string? headContent = null;
        var cssLinks = new List<string>();

        var headMatch = HeadSectionRegex.Match(content);
        if (headMatch.Success)
        {
            var innerHead = headMatch.Groups[1].Value;

            foreach (Match linkMatch in CssLinkRegex.Matches(innerHead))
            {
                var hrefMatch = LinkHrefRegex.Match(linkMatch.Value);
                var href = hrefMatch.Success ? hrefMatch.Groups[1].Value : linkMatch.Value.Trim();
                cssLinks.Add(href.Replace("~/", "/"));
            }

            // Remove CSS links; keep the rest for the <Head> parameter
            var cleanHead = CssLinkRegex.Replace(innerHead, "").Trim('\n', '\r');
            if (!string.IsNullOrWhiteSpace(cleanHead))
                headContent = cleanHead;

            content = HeadSectionRegex.Replace(content, "");
        }

        // Step 5: Strip outer HTML boilerplate
        content = DocTypeRegex.Replace(content, "");
        content = HtmlOpenRegex.Replace(content, "");
        content = HtmlCloseRegex.Replace(content, "");
        content = BodyOpenRegex.Replace(content, "");
        content = BodyCloseRegex.Replace(content, "");

        // Step 6: Peel off leading Razor directive lines (e.g. @using) — keep outside <MasterPage>
        var directives = new StringBuilder();
        var lines = content.Split('\n');
        var bodyStart = 0;
        for (var i = 0; i < lines.Length; i++)
        {
            var trimmed = lines[i].TrimEnd();
            if (trimmed.StartsWith("@") || string.IsNullOrWhiteSpace(trimmed))
            {
                if (!string.IsNullOrWhiteSpace(trimmed))
                    directives.Append(lines[i].TrimEnd() + "\n");
                bodyStart = i + 1;
            }
            else
            {
                break;
            }
        }

        var bodyContent = string.Join("\n", lines.Skip(bodyStart)).Trim('\n', '\r');

        // Step 7: Assemble output
        var sb = new StringBuilder();

        if (directives.Length > 0)
            sb.Append(directives.ToString());

        var todoMsg = cssLinks.Count > 0
            ? $"@* TODO(bwfc-master-page): CSS <link> refs from <head> — move to App.razor: {string.Join(", ", cssLinks)} *@"
            : "@* TODO(bwfc-master-page): Review head content extraction for App.razor *@";

        sb.AppendLine(todoMsg);
        sb.AppendLine();

        if (headContent != null)
        {
            sb.AppendLine("<MasterPage>");
            sb.AppendLine("<Head>");
            sb.AppendLine(headContent);
            sb.AppendLine("</Head>");
            sb.AppendLine("<ChildContent>");
            sb.AppendLine(bodyContent);
            sb.AppendLine("</ChildContent>");
            sb.AppendLine("</MasterPage>");
        }
        else
        {
            sb.AppendLine("<MasterPage>");
            sb.AppendLine(bodyContent);
            sb.AppendLine("</MasterPage>");
        }

        return sb.ToString();
    }

    private static string ExtractId(string attrs)
    {
        var m = TagIdRegex.Match(attrs);
        return m.Success ? m.Groups[1].Value : "Main";
    }
}
