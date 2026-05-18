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

    private static FileMetadata TestCodeFileMetadata(string content, string sourceFilePath = "Logic\\ExceptionUtility.cs") => new()
    {
        SourceFilePath = sourceFilePath,
        OutputFilePath = sourceFilePath,
        FileType = FileType.CodeFile,
        OriginalContent = content
    };

    [Fact]
    public void RewritesServerMapPathLiteralInCodeFile()
    {
        var input = @"using System;
using System.Web;

namespace MyApp.Logic;

public class ExceptionUtility
{
    public string BuildPath()
    {
        return Server.MapPath(""~/App_Data/ErrorLog.txt"");
    }
}";

        var result = _transform.Apply(input, TestCodeFileMetadata(input));

        Assert.Contains("using System.IO;", result);
        Assert.DoesNotContain("using System.Web;", result);
        Assert.Contains("Path.Combine(AppContext.BaseDirectory, \"App_Data\", \"ErrorLog.txt\")", result);
        Assert.DoesNotContain("TODO(bwfc-server)", result);
    }

    [Fact]
    public void RewritesHttpContextCurrentServerMapPathInCodeFile()
    {
        var input = @"using System.Web;

namespace MyApp.Logic;

public class ExceptionUtility
{
    public string BuildPath()
    {
        return HttpContext.Current.Server.MapPath(""~/Logs/Error/ErrorLog.txt"");
    }
}";

        var result = _transform.Apply(input, TestCodeFileMetadata(input));

        Assert.Contains("Path.Combine(AppContext.BaseDirectory, \"Logs\", \"Error\", \"ErrorLog.txt\")", result);
        Assert.DoesNotContain("HttpContext.Current.Server.MapPath", result);
    }

    [Fact]
    public void RewritesMapPathVariableWhenBoundToLiteralPath()
    {
        var input = @"using System.Web;

namespace MyApp.Logic;

public class ExceptionUtility
{
    public string BuildPath()
    {
        string logFile = ""App_Data/ErrorLog.txt"";
        logFile = HttpContext.Current.Server.MapPath(logFile);
        return logFile;
    }
}";

        var result = _transform.Apply(input, TestCodeFileMetadata(input));

        Assert.Contains("logFile = Path.Combine(AppContext.BaseDirectory, \"App_Data\", \"ErrorLog.txt\");", result);
    }

    [Fact]
    public void AlreadyUsesAppContextBaseDirectory_NoChange()
    {
        var input = @"using System.IO;

namespace MyApp.Logic;

public class ExceptionUtility
{
    public string BuildPath()
    {
        return Path.Combine(AppContext.BaseDirectory, ""App_Data"", ""ErrorLog.txt"");
    }
}";

        var result = _transform.Apply(input, TestCodeFileMetadata(input));

        Assert.Equal(input, result);
    }

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
    public void DetectsServerTransfer_AddsShimGuidance()
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

        Assert.Contains("Methods found: Transfer", result);
        Assert.Contains("NavigationManager.NavigateTo(path)", result);
        Assert.DoesNotContain("has NO SHIM", result);
    }

    [Fact]
    public void DetectsServerGetLastError_AddsShimGuidance()
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

        Assert.Contains("Methods found: GetLastError", result);
        Assert.Contains("returns null", result);
        Assert.DoesNotContain("has NO SHIM", result);
    }

    [Fact]
    public void DetectsServerClearError_AddsShimGuidance()
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

        Assert.Contains("Methods found: ClearError", result);
        Assert.Contains("no-op compatibility stub", result);
        Assert.DoesNotContain("has NO SHIM", result);
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
        Assert.Contains("Transfer", result);
        Assert.Contains("TODO(bwfc-server): Server.* calls work automatically via ServerShim on WebFormsPageBase", result);
        Assert.DoesNotContain("has NO SHIM", result);
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

        var count = result.Split("Methods found: Transfer").Length - 1;
        Assert.Equal(1, count);
    }

    [Fact]
    public void OrderIs330()
    {
        Assert.Equal(330, _transform.Order);
    }
}
