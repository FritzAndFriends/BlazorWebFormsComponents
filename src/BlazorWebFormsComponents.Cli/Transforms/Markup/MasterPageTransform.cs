using System.Text;
using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Converts .master files to BWFC MasterPage component syntax.
/// Preserves named ContentPlaceHolder relationships using BWFC &lt;ContentPlaceHolder ID="X"&gt;,
/// emits a runnable shell contract with a <c>ChildContent</c> parameter, preserves
/// head content inside BWFC <c>&lt;Head&gt;</c>, and strips the outer HTML document
/// scaffold plus server-form wrappers.
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

    // runat="server" on <head> tags
    private static readonly Regex HeadRunatRegex = new(
        @"(<head\b[^>]*?)\s+runat\s*=\s*""server""([^>]*>)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // Server form wrappers should not survive into the generated master-page shell.
    private static readonly Regex ServerFormBlockRegex = new(
        @"[ \t]*<form\b([^>]*?)\s+runat\s*=\s*""server""([^>]*)>([\s\S]*?)</form>[ \t]*\r?\n?",
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

    public string Apply(string content, FileMetadata metadata)
    {
        if (metadata.FileType != FileType.Master)
            return content;

        // Normalise line endings for consistent regex behaviour
        content = content.Replace("\r\n", "\n");

        // Step 1: Strip runat="server" from <head> and unwrap server-form shells
        content = HeadRunatRegex.Replace(content, "$1$2");
        content = ServerFormBlockRegex.Replace(content, m => m.Groups[3].Value.Trim('\n', '\r') + "\n");
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

        // Step 4: Extract <head> section and preserve it in BWFC <Head>
        string? headContent = null;

        var headMatch = HeadSectionRegex.Match(content);
        if (headMatch.Success)
        {
            var innerHead = headMatch.Groups[1].Value;
            var cleanHead = innerHead
                .Replace("~/", "/")
                .Trim('\n', '\r');
            if (!string.IsNullOrWhiteSpace(cleanHead))
            {
                headContent = cleanHead;
            }

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

        const string todoMsg =
            "@* TODO(bwfc-master-page): Review shell scripts, bundle references, and auth/cart chrome for SSR-safe migration. *@";

        sb.AppendLine(todoMsg);
        sb.AppendLine();

        if (headContent != null)
        {
            sb.AppendLine("<MasterPage>");
            sb.AppendLine("<Head>");
            sb.AppendLine(headContent);
            sb.AppendLine("</Head>");
            sb.AppendLine("<ChildContent>");
            if (!string.IsNullOrWhiteSpace(bodyContent))
            {
                sb.AppendLine(bodyContent);
            }
            sb.AppendLine("@ChildContent");
            sb.AppendLine("</ChildContent>");
            sb.AppendLine("</MasterPage>");
        }
        else
        {
            sb.AppendLine("<MasterPage>");
            sb.AppendLine("<ChildContent>");
            if (!string.IsNullOrWhiteSpace(bodyContent))
            {
                sb.AppendLine(bodyContent);
            }
            sb.AppendLine("@ChildContent");
            sb.AppendLine("</ChildContent>");
            sb.AppendLine("</MasterPage>");
        }

        sb.AppendLine();
        sb.AppendLine("@code {");
        sb.AppendLine("    [Parameter]");
        sb.AppendLine("    public RenderFragment? ChildContent { get; set; }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string ExtractId(string attrs)
    {
        var m = TagIdRegex.Match(attrs);
        return m.Success ? m.Groups[1].Value : "Main";
    }
}
