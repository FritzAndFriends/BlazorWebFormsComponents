using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for ServerShimTransform — detects Server.MapPath/HtmlEncode/etc.
/// and emits migration guidance for ServerShim.
/// </summary>
public class ServerShimTransformTests
{
    private readonly ServerShimTransform _transform = new();

    private static FileMetadata TestMetadata(string content) => new()
    {
        SourceFilePath = "Default.aspx.cs",
        OutputFilePath = "Default.razor.cs",
        FileType = FileType.Page,
        OriginalContent = content
    };

    [Fact]
    public void DetectsMapPath_AddsGuidance()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Process()
        {
            var path = Server.MapPath(""~/uploads"");
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("TODO(bwfc-server)", result);
        Assert.Contains("ServerShim", result);
        Assert.Contains("MapPath", result);
        Assert.Contains("WebRootPath", result);
    }

    [Fact]
    public void DetectsHtmlEncode_AddsGuidance()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Process()
        {
            var safe = Server.HtmlEncode(userInput);
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("TODO(bwfc-server)", result);
        Assert.Contains("HtmlEncode", result);
    }

    [Fact]
    public void DetectsMultipleMethods_ListsAll()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Process()
        {
            var path = Server.MapPath(""~/data"");
            var safe = Server.HtmlEncode(userInput);
            var encoded = Server.UrlEncode(query);
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("MapPath", result);
        Assert.Contains("HtmlEncode", result);
        Assert.Contains("UrlEncode", result);
    }

    [Fact]
    public void NoServerCalls_NoChanges()
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
    public void DetectsHttpServerUtility()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Process()
        {
            HttpServerUtility server = this.Server;
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("TODO(bwfc-server)", result);
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
            var path = Server.MapPath(""~/uploads"");
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));
        result = _transform.Apply(result, TestMetadata(result));

        var count = result.Split("Server Utility Migration").Length - 1;
        Assert.Equal(1, count);
    }

    [Fact]
    public void DetectsServerTransfer_AddsNoShimTodo()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Process()
        {
            Server.Transfer(""~/OtherPage.aspx"");
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("TODO(bwfc-server): Server.Transfer has NO SHIM", result);
        Assert.Contains("NavigationManager.NavigateTo()", result);
    }

    [Fact]
    public void DetectsServerGetLastError_AddsNoShimTodo()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Process()
        {
            var ex = Server.GetLastError();
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("TODO(bwfc-server): Server.GetLastError has NO SHIM", result);
        Assert.Contains("ILogger", result);
    }

    [Fact]
    public void DetectsServerClearError_AddsNoShimTodo()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Process()
        {
            Server.ClearError();
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("TODO(bwfc-server): Server.ClearError has NO SHIM", result);
        Assert.Contains("middleware", result);
    }

    [Fact]
    public void DetectsTransferAndMapPath_BothHandled()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Process()
        {
            var path = Server.MapPath(""~/uploads"");
            Server.Transfer(""~/Other.aspx"");
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("MapPath", result);
        Assert.Contains("TODO(bwfc-server): Server.Transfer has NO SHIM", result);
        Assert.Contains("TODO(bwfc-server): Server.* calls work automatically via ServerShim on WebFormsPageBase", result);
    }

    [Fact]
    public void ServerTransfer_Idempotent()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Process()
        {
            Server.Transfer(""~/Other.aspx"");
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));
        result = _transform.Apply(result, TestMetadata(result));

        var count = result.Split("Server.Transfer has NO SHIM").Length - 1;
        Assert.Equal(1, count);
    }

    [Fact]
    public void OrderIs330()
    {
        Assert.Equal(330, _transform.Order);
    }
}
