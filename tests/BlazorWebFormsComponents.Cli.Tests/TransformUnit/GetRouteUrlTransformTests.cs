using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for GetRouteUrlTransform — converts Page.GetRouteUrl() to GetRouteUrl()
/// with a TODO marker for developer review.
/// </summary>
public class GetRouteUrlTransformTests
{
    private readonly GetRouteUrlTransform _transform = new();

    private static FileMetadata TestMetadata(string content) => new()
    {
        SourceFilePath = "Default.aspx.cs",
        OutputFilePath = "Default.razor.cs",
        FileType = FileType.Page,
        OriginalContent = content
    };

    [Fact]
    public void ConvertsPageGetRouteUrl()
    {
        var input = @"var url = Page.GetRouteUrl(""Products"", new { id = 1 });";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("Page.GetRouteUrl", result);
        Assert.Contains("GetRouteUrl(", result);
        Assert.Contains("TODO(bwfc-route-url)", result);
    }

    [Fact]
    public void ConvertsThisPageGetRouteUrl()
    {
        var input = @"var url = this.Page.GetRouteUrl(""Products"", new { id = 1 });";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("this.Page.GetRouteUrl", result);
        Assert.Contains("GetRouteUrl(", result);
        Assert.Contains("TODO(bwfc-route-url)", result);
    }

    [Fact]
    public void PreservesContentWithoutGetRouteUrl()
    {
        var input = @"var x = 42; // no route url here";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Equal(input, result);
    }

    [Fact]
    public void HandlesMultipleGetRouteUrlCalls()
    {
        var input = "var url1 = Page.GetRouteUrl(\"Route1\", new { id = 1 });\nvar url2 = Page.GetRouteUrl(\"Route2\", new { id = 2 });";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("Page.GetRouteUrl", result);
        var count = result.Split("TODO(bwfc-route-url)").Length - 1;
        Assert.Equal(2, count);
    }

    [Fact]
    public void DoesNotTouchNonPageGetRouteUrl()
    {
        var input = @"var url = someObject.GetRouteUrl(""Test"");";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Equal(input, result);
    }

    [Fact]
    public void HandlesWhitespaceBeforeParen()
    {
        var input = @"var url = Page.GetRouteUrl (""Products"");";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("Page.GetRouteUrl", result);
        Assert.Contains("GetRouteUrl(", result);
    }

    [Fact]
    public void PreservesRouteArguments()
    {
        var input = @"var url = Page.GetRouteUrl(""ProductDetail"", new RouteValueDictionary { { ""id"", productId } });";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("ProductDetail", result);
        Assert.Contains("RouteValueDictionary", result);
    }
}
