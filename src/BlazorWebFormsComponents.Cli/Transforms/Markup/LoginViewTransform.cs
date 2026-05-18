using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Converts asp:LoginView to Blazor AuthorizeView.
/// AnonymousTemplate → NotAuthorized, LoggedInTemplate → Authorized,
/// RoleGroups → TODO comment for policy-based AuthorizeView.
/// Must run before AspPrefixTransform (order 610) to match asp: prefix.
/// </summary>
public class LoginViewTransform : IMarkupTransform
{
    public string Name => "LoginView";
    public int Order => 510;

    // Opening <asp:LoginView ...> — capture attributes to strip runat/ID
    private static readonly Regex OpenTagRegex = new(
        @"<asp:LoginView\b[^>]*?>",
        RegexOptions.Compiled | RegexOptions.Singleline);

    // Self-closing <asp:LoginView ... />
    private static readonly Regex SelfCloseRegex = new(
        @"<asp:LoginView\b[^>]*/\s*>",
        RegexOptions.Compiled | RegexOptions.Singleline);

    // Closing tag
    private static readonly Regex CloseTagRegex = new(
        @"</asp:LoginView\s*>",
        RegexOptions.Compiled);

    // Template tags
    private static readonly Regex AnonOpenRegex = new(
        @"<AnonymousTemplate\s*>",
        RegexOptions.Compiled);

    private static readonly Regex AnonCloseRegex = new(
        @"</AnonymousTemplate\s*>",
        RegexOptions.Compiled);

    private static readonly Regex LoggedInOpenRegex = new(
        @"<LoggedInTemplate\s*>",
        RegexOptions.Compiled);

    private static readonly Regex LoggedInCloseRegex = new(
        @"</LoggedInTemplate\s*>",
        RegexOptions.Compiled);

    // RoleGroups block (entire content between tags)
    private static readonly Regex RoleGroupsRegex = new(
        @"(?s)<RoleGroups\b[^>]*>.*?</RoleGroups\s*>",
        RegexOptions.Compiled);

    // Self-closing RoleGroups
    private static readonly Regex RoleGroupsSelfCloseRegex = new(
        @"<RoleGroups\b[^>]*/\s*>",
        RegexOptions.Compiled);

    // Attributes to strip from the opening tag
    private static readonly Regex RunatAttrRegex = new(
        @"\s+runat\s*=\s*""[^""]*""",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex IdAttrRegex = new(
        @"\s+ID\s*=\s*""[^""]*""",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Apply(string content, FileMetadata metadata)
    {
        // Handle self-closing form first: <asp:LoginView ... /> → <AuthorizeView />
        content = SelfCloseRegex.Replace(content, m =>
        {
            var tag = m.Value;
            tag = Regex.Replace(tag, @"<asp:LoginView", "<AuthorizeView");
            tag = RunatAttrRegex.Replace(tag, "");
            tag = IdAttrRegex.Replace(tag, "");
            return tag;
        });

        // Handle opening tag: <asp:LoginView ...> → <AuthorizeView>
        content = OpenTagRegex.Replace(content, m =>
        {
            var tag = m.Value;
            tag = Regex.Replace(tag, @"<asp:LoginView", "<AuthorizeView");
            tag = RunatAttrRegex.Replace(tag, "");
            tag = IdAttrRegex.Replace(tag, "");
            return tag;
        });

        // Closing tag
        content = CloseTagRegex.Replace(content, "</AuthorizeView>");

        // Template conversions
        content = AnonOpenRegex.Replace(content, "<NotAuthorized>");
        content = AnonCloseRegex.Replace(content, "</NotAuthorized>");
        content = LoggedInOpenRegex.Replace(content, "<Authorized>");
        content = LoggedInCloseRegex.Replace(content, "</Authorized>");

        // RoleGroups → TODO comment
        content = RoleGroupsRegex.Replace(content,
            "@* TODO(bwfc-identity): Convert RoleGroups to policy-based AuthorizeView *@");
        content = RoleGroupsSelfCloseRegex.Replace(content,
            "@* TODO(bwfc-identity): Convert RoleGroups to policy-based AuthorizeView *@");

        return content;
    }
}
