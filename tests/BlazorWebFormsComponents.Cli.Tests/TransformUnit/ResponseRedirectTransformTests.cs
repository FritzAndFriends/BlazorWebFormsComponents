using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for ResponseRedirectTransform — ThreadAbortException dead code
/// detection and endResponse=true warning.
/// </summary>
public class ResponseRedirectTransformTests
{
    private readonly ResponseRedirectTransform _transform = new();

    private static FileMetadata TestMetadata(string content) => new()
    {
        SourceFilePath = "Default.aspx.cs",
        OutputFilePath = "Default.razor.cs",
        FileType = FileType.Page,
        OriginalContent = content
    };

    [Fact]
    public void DetectsThreadAbortCatch_EmitsDeadCodeWarning()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Process()
        {
            try
            {
                Response.Redirect(""~/Login.aspx"");
            }
            catch (ThreadAbortException)
            {
                // Web Forms pattern — absorb redirect exception
            }
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("TODO(bwfc-navigation): DEAD CODE", result);
        Assert.Contains("dead code after migration", result);
    }

    [Fact]
    public void DetectsEndResponseTrue_EmitsWarning()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Process()
        {
            Response.Redirect(""~/Login.aspx"", true);
            // This code would NOT execute in Web Forms
            DoSomething();
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("TODO(bwfc-navigation): endResponse=true", result);
        Assert.Contains("silently ignored by ResponseShim", result);
    }

    [Fact]
    public void NoRedirect_NoChanges()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Process() { var x = 42; }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Equal(input, result);
    }

    [Fact]
    public void ThreadAbortCatch_Idempotent()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Process()
        {
            try
            {
                Response.Redirect(""~/Login.aspx"");
            }
            catch (ThreadAbortException)
            {
            }
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));
        result = _transform.Apply(result, TestMetadata(result));

        var count = result.Split("DEAD CODE").Length - 1;
        Assert.Equal(1, count);
    }

    [Fact]
    public void EndResponseTrue_Idempotent()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Process()
        {
            Response.Redirect(""~/Login.aspx"", true);
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));
        result = _transform.Apply(result, TestMetadata(result));

        var count = result.Split("endResponse=true").Length - 1;
        Assert.Equal(1, count);
    }

    [Fact]
    public void OrderIs300()
    {
        Assert.Equal(300, _transform.Order);
    }
}
