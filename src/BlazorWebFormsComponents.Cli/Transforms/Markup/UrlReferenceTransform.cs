using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Converts ~/ URL references to / for URL-bearing attributes.
/// </summary>
public class UrlReferenceTransform : IMarkupTransform
{
    public string Name => "UrlReference";
    public int Order => 720;

    private static readonly (string Pattern, string Replacement)[] UrlPatterns =
    [
        ("href=\"~/", "href=\"/"),
        ("NavigateUrl=\"~/", "NavigateUrl=\"/"),
        ("ImageUrl=\"~/", "ImageUrl=\"/"),
        ("src=\"~/", "src=\"/"),
        ("BackImageUrl=\"~/", "BackImageUrl=\"/"),
        ("PostBackUrl=\"~/", "PostBackUrl=\"/"),
        ("DataNavigateUrlFormatString=\"~/", "DataNavigateUrlFormatString=\"/"),
        ("HelpPageUrl=\"~/", "HelpPageUrl=\"/")
    ];

    public string Apply(string content, FileMetadata metadata)
    {
        foreach (var (pattern, replacement) in UrlPatterns)
        {
            content = content.Replace(pattern, replacement);
        }

        return content;
    }
}
