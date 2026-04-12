using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for RequestFormTransform — detects Request.Form patterns and
/// emits migration guidance for FormShim / WebFormsForm.
/// </summary>
public class RequestFormTransformTests
{
    private readonly RequestFormTransform _transform = new();

    private static FileMetadata TestMetadata(string content) => new()
    {
        SourceFilePath = "Default.aspx.cs",
        OutputFilePath = "Default.razor.cs",
        FileType = FileType.Page,
        OriginalContent = content
    };

    [Fact]
    public void DetectsLiteralFormKey_AddsGuidance()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Process()
        {
            var name = Request.Form[""txtName""];
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("TODO(bwfc-form)", result);
        Assert.Contains("FormShim", result);
        Assert.Contains("WebFormsForm", result);
        Assert.Contains("txtName", result);
    }

    [Fact]
    public void DetectsMultipleKeys_ListsAllKeys()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Process()
        {
            var name = Request.Form[""txtName""];
            var email = Request.Form[""txtEmail""];
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("txtName", result);
        Assert.Contains("txtEmail", result);
    }

    [Fact]
    public void DetectsFormMemberAccess()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Process()
        {
            var keys = Request.Form.AllKeys;
            var count = Request.Form.Count;
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("TODO(bwfc-form)", result);
    }

    [Fact]
    public void NoFormAccess_NoChanges()
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
    public void Idempotent_DoesNotDuplicateGuidance()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Process()
        {
            var name = Request.Form[""txtName""];
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));
        result = _transform.Apply(result, TestMetadata(result));

        var count = result.Split("Request.Form Migration").Length - 1;
        Assert.Equal(1, count);
    }

    [Fact]
    public void OrderIs320()
    {
        Assert.Equal(320, _transform.Order);
    }
}
