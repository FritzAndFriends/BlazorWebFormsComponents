using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests;

public class HttpExceptionTransformTests
{
    private readonly HttpExceptionTransform _transform = new();

    private static FileMetadata PageMetadata() => new()
    {
        SourceFilePath = "test.aspx.cs",
        OutputFilePath = "test.razor.cs",
        FileType = FileType.Page,
        OriginalContent = ""
    };

    [Fact]
    public void ThreeArg_ReplacedWithInvalidOperationException()
    {
        var input = """
            ex = new HttpException(404, httpErrorMsg, ex);
            """;

        var result = _transform.Apply(input, PageMetadata());

        Assert.Contains("new InvalidOperationException(httpErrorMsg, ex)", result);
        Assert.DoesNotContain("HttpException", result);
    }

    [Fact]
    public void TwoArg_ReplacedWithInvalidOperationException()
    {
        var input = """
            throw new HttpException(500, "Server error");
            """;

        var result = _transform.Apply(input, PageMetadata());

        Assert.Contains("new InvalidOperationException(\"Server error\")", result);
        Assert.DoesNotContain("HttpException", result);
    }

    [Fact]
    public void OneArg_ReplacedWithInvalidOperationException()
    {
        var input = """
            throw new HttpException("Not found");
            """;

        var result = _transform.Apply(input, PageMetadata());

        Assert.Contains("new InvalidOperationException(\"Not found\")", result);
        Assert.DoesNotContain("HttpException", result);
    }

    [Fact]
    public void CatchBlock_ReplacedWithInvalidOperationException()
    {
        var input = """
            catch (HttpException hex)
            {
                // handle
            }
            """;

        var result = _transform.Apply(input, PageMetadata());

        Assert.Contains("catch (InvalidOperationException hex)", result);
        Assert.DoesNotContain("HttpException", result);
    }

    [Fact]
    public void NoHttpException_ReturnsUnchanged()
    {
        var input = "var x = new Exception(\"test\");";

        var result = _transform.Apply(input, PageMetadata());

        Assert.Equal(input, result);
    }
}
