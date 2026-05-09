using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Adds <c>data-enhance-nav="false"</c> to links and forms that should bypass
/// Blazor's enhanced navigation. This prevents silent failures when enhanced
/// navigation intercepts links to non-Razor endpoints (logout, cart mutations,
/// form POST actions).
/// </summary>
public class EnhancedNavAnnotationTransform : IMarkupTransform
{
    public string Name => "EnhancedNavAnnotation";
    public int Order => 830;

    // <a ... href="...Logout..." ...> or <a ... href="...SignOut..." ...>
    private static readonly Regex LogoutLinkRegex = new(
        @"<a\s+(?=[^>]*href\s*=\s*""[^""]*(?:Logout|SignOut|Log_?Out)[^""]*"")(?!(?:[^>]*data-enhance-nav))([^>]*)>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // <a ... href="...AddToCart..." ...> or RemoveFromCart or UpdateCart
    private static readonly Regex CartMutationLinkRegex = new(
        @"<a\s+(?=[^>]*href\s*=\s*""[^""]*(?:AddToCart|RemoveFromCart|UpdateCart)[^""]*"")(?!(?:[^>]*data-enhance-nav))([^>]*)>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // <form method="post" ...> without data-enhance-nav
    private static readonly Regex FormPostRegex = new(
        @"<form\s+(?=[^>]*method\s*=\s*""post"")(?!(?:[^>]*data-enhance-nav))([^>]*)>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Apply(string content, FileMetadata metadata)
    {
        if (metadata.FileType != FileType.Page && metadata.FileType != FileType.Control)
            return content;

        // Add data-enhance-nav="false" to logout links
        content = LogoutLinkRegex.Replace(content, m =>
            InsertAttribute(m.Value, @"data-enhance-nav=""false"""));

        // Add data-enhance-nav="false" to cart mutation links
        content = CartMutationLinkRegex.Replace(content, m =>
            InsertAttribute(m.Value, @"data-enhance-nav=""false"""));

        // Add data-enhance-nav="false" to POST forms
        content = FormPostRegex.Replace(content, m =>
            InsertAttribute(m.Value, @"data-enhance-nav=""false"""));

        return content;
    }

    private static string InsertAttribute(string tag, string attribute)
    {
        // Insert attribute before the closing >
        var closeIndex = tag.LastIndexOf('>');
        if (closeIndex < 0) return tag;
        return tag[..closeIndex] + " " + attribute + tag[closeIndex..];
    }
}
