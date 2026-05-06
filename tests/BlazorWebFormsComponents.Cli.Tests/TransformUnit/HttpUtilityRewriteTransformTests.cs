using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

public class HttpUtilityRewriteTransformTests
{
    private readonly HttpUtilityRewriteTransform _transform = new();

    private static FileMetadata TestMetadata(string content) => new()
    {
        SourceFilePath = "Utility.cs",
        OutputFilePath = "Utility.cs",
        FileType = FileType.Page,
        OriginalContent = content
    };

    [Fact]
    public void HasExpectedMetadata()
    {
        Assert.Equal("HttpUtilityRewrite", _transform.Name);
        Assert.Equal(104, _transform.Order);
    }

    [Fact]
    public void RewritesUnqualifiedHttpUtilityCalls()
    {
        var input = """
            using System.Web;
            
            public class UrlHelper
            {
                public string Encode(string value) => HttpUtility.UrlEncode(value);
            }
            """;

        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("using System.Net;", result);
        Assert.DoesNotContain("using System.Web;", result);
        Assert.Contains("WebUtility.UrlEncode(value)", result);
    }

    [Fact]
    public void RewritesFullyQualifiedHttpUtilityCalls()
    {
        var input = """
            public class HtmlHelper
            {
                public string Encode(string value) => System.Web.HttpUtility.HtmlEncode(value);
            }
            """;

        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("using System.Net;", result);
        Assert.Contains("WebUtility.HtmlEncode(value)", result);
        Assert.DoesNotContain("System.Web.HttpUtility", result);
    }

    [Fact]
    public void PreservesSystemWebUsingWhenOtherTypesStillAppear()
    {
        var input = """
            using System.Web;
            
            public class UrlHelper
            {
                public HttpContext? Context { get; set; }
                public string Encode(string value) => HttpUtility.UrlEncode(value);
            }
            """;

        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("using System.Web;", result);
        Assert.Contains("WebUtility.UrlEncode(value)", result);
    }

    [Fact]
    public void LeavesFilesWithoutHttpUtilityUnchanged()
    {
        var input = """
            using System;
            
            public class PlainHelper
            {
                public string Echo(string value) => value;
            }
            """;

        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Equal(input, result);
    }
}
