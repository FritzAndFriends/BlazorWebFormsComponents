using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for UrlReferenceTransform — converts ~/ URLs to root-relative paths.
/// Corresponds to TC07-UrlTilde test case.
/// </summary>
public class UrlReferenceTransformTests
{
    private readonly UrlReferenceTransform _transform = new();
    private readonly FileMetadata _metadata = new()
    {
        SourceFilePath = "test.aspx",
        OutputFilePath = "test.razor",
        FileType = FileType.Page,
        OriginalContent = ""
    };

    [Fact]
    public void ConvertsTildeInHref()
    {
        var input = @"<link href=""~/Styles/Site.css"" rel=""stylesheet"" />";
        var expected = @"<link href=""/Styles/Site.css"" rel=""stylesheet"" />";

        var result = _transform.Apply(input, _metadata);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertsTildeInNavigateUrl()
    {
        var input = @"<HyperLink NavigateUrl=""~/Products/List.aspx"" Text=""Products"" />";
        var expected = @"<HyperLink NavigateUrl=""/Products/List.aspx"" Text=""Products"" />";

        var result = _transform.Apply(input, _metadata);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertsTildeInImageUrl()
    {
        var input = @"<Image ImageUrl=""~/Images/logo.png"" />";
        var expected = @"<Image ImageUrl=""/Images/logo.png"" />";

        var result = _transform.Apply(input, _metadata);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertsTildeInSrc()
    {
        var input = @"<script src=""~/Scripts/app.js""></script>";
        var expected = @"<script src=""/Scripts/app.js""></script>";

        var result = _transform.Apply(input, _metadata);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertsTildeInBackImageUrl()
    {
        var input = @"<Panel BackImageUrl=""~/Images/bg.png"">";
        var expected = @"<Panel BackImageUrl=""/Images/bg.png"">";

        var result = _transform.Apply(input, _metadata);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertsTildeInPostBackUrl()
    {
        var input = @"<Button PostBackUrl=""~/Results.aspx"" Text=""Submit"" />";
        var expected = @"<Button PostBackUrl=""/Results.aspx"" Text=""Submit"" />";

        var result = _transform.Apply(input, _metadata);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertsTildeInDataNavigateUrlFormatString()
    {
        var input = @"<HyperLinkField DataNavigateUrlFormatString=""~/Details.aspx?id={0}"" />";
        var expected = @"<HyperLinkField DataNavigateUrlFormatString=""/Details.aspx?id={0}"" />";

        var result = _transform.Apply(input, _metadata);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertsTildeInHelpPageUrl()
    {
        var input = @"<WebPartManager HelpPageUrl=""~/Help/Index.aspx"" />";
        var expected = @"<WebPartManager HelpPageUrl=""/Help/Index.aspx"" />";

        var result = _transform.Apply(input, _metadata);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void LeavesNonTildeUrlsUnchanged()
    {
        var input = @"<link href=""/Styles/Site.css"" rel=""stylesheet"" />";

        var result = _transform.Apply(input, _metadata);

        Assert.Equal(input, result);
    }

    [Fact]
    public void ConvertsMultipleAttributesInSameContent()
    {
        var input = @"<link href=""~/Styles/Site.css"" /><script src=""~/Scripts/app.js""></script>";
        var expected = @"<link href=""/Styles/Site.css"" /><script src=""/Scripts/app.js""></script>";

        var result = _transform.Apply(input, _metadata);

        Assert.Equal(expected, result);
    }
}
