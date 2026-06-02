using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Cli.Tests.Transforms.CodeBehind;

public class HttpContextAccessorTransformTests
{
    private readonly HttpContextAccessorTransform _sut = new();

    private static FileMetadata MakeMetadata(string source = "Logic/Test.cs") => new()
    {
        SourceFilePath = source,
        OutputFilePath = "Test.cs",
        FileType = FileType.CodeFile,
        OriginalContent = string.Empty
    };

    [Fact]
    public void SessionComparison_NotGarbled()
    {
        var input = @"using System;
public class CartActions
{
    public string GetCartId()
    {
        if (HttpContext.Current.Session[""CartKey""] == null)
        {
            HttpContext.Current.Session[""CartKey""] = Guid.NewGuid().ToString();
        }
        return HttpContext.Current.Session[""CartKey""].ToString();
    }
}";

        var result = _sut.Apply(input, MakeMetadata());

        result.ShouldNotContain("SetString(\"CartKey\", = null");
        result.ShouldNotContain("SetString(\"CartKey\", == null");
        result.ShouldContain("GetString(\"CartKey\") == null");
    }

    [Fact]
    public void SessionAssignment_StillConverted()
    {
        var input = @"public class CartActions
{
    public void SetCart()
    {
        HttpContext.Current.Session[""CartKey""] = ""abc"";
    }
}";

        var result = _sut.Apply(input, MakeMetadata());

        result.ShouldContain("SetString(\"CartKey\"");
    }

    [Fact]
    public void PageCodeBehind_Skipped()
    {
        var input = @"public class MyPage : WebFormsPageBase
{
    public void Test()
    {
        HttpContext.Current.Session[""key""] = ""val"";
    }
}";

        var result = _sut.Apply(input, MakeMetadata("Pages/MyPage.aspx.cs"));

        result.ShouldBe(input);
    }

    [Fact]
    public void StaticMethod_UsesStaticFieldAndConfigure()
    {
        var input = @"using System;
using System.Web;

public sealed class ExceptionUtility
{
    private ExceptionUtility() { }

    public static void LogException(Exception exc, string source)
    {
        var user = HttpContext.Current.User.Identity.Name;
    }
}";

        var result = _sut.Apply(input, MakeMetadata("Logic/ExceptionUtility.cs"));

        // Should use static field, not readonly instance field
        result.ShouldContain("private static IHttpContextAccessor _httpContextAccessor");
        result.ShouldNotContain("private readonly IHttpContextAccessor");
        // Should generate Configure() method, not constructor
        result.ShouldContain("public static void Configure(IHttpContextAccessor httpContextAccessor)");
        result.ShouldNotContain("public ExceptionUtility(IHttpContextAccessor");
    }

    [Fact]
    public void ServerMapPath_SkippedForServerShimTransform()
    {
        // HttpContext.Current.Server.MapPath should NOT be replaced by this transform
        // because ServerShimTransform handles it specifically
        var input = @"using System;
using System.Web;

public sealed class ExceptionUtility
{
    private ExceptionUtility() { }

    public static void LogException(Exception exc, string source)
    {
        string logFile = ""App_Data/ErrorLog.txt"";
        logFile = HttpContext.Current.Server.MapPath(logFile);
    }
}";

        var result = _sut.Apply(input, MakeMetadata("Logic/ExceptionUtility.cs"));

        // HttpContext.Current.Server should be preserved for ServerShimTransform
        result.ShouldContain("HttpContext.Current.Server.MapPath");
        // Should NOT inject accessor since no replacements were made
        result.ShouldNotContain("IHttpContextAccessor");
    }

    [Fact]
    public void MixedStaticAndServerMapPath_OnlyServerSkipped()
    {
        // File with both Server.MapPath (skip) AND Session access (transform)
        var input = @"using System;
using System.Web;

public class Logger
{
    public static void Log(string msg)
    {
        var path = HttpContext.Current.Server.MapPath(""~/logs"");
        var user = HttpContext.Current.User.Identity.Name;
    }
}";

        var result = _sut.Apply(input, MakeMetadata("Logic/Logger.cs"));

        // Server.MapPath preserved for ServerShimTransform
        result.ShouldContain("HttpContext.Current.Server.MapPath");
        // User access transformed
        result.ShouldContain("_httpContextAccessor.HttpContext?.User?.Identity?.Name");
        // Since usage is in a static method, should use static Configure pattern
        result.ShouldContain("private static IHttpContextAccessor _httpContextAccessor");
        result.ShouldContain("public static void Configure(");
    }

    [Fact]
    public void ExceptionUtility_PipelineIntegration_ProducesCorrectOutput()
    {
        // Simulates the full ExceptionUtility.cs transformation through both
        // HttpContextAccessorTransform (order 108) and ServerShimTransform (order 330)
        var input = @"using System;
using System.IO;
using System.Web;

namespace WingtipToys.Logic
{
    public sealed class ExceptionUtility
    {
        private ExceptionUtility() { }

        public static void LogException(Exception exc, string source)
        {
            string logFile = ""App_Data/ErrorLog.txt"";
            logFile = HttpContext.Current.Server.MapPath(logFile);
            StreamWriter sw = new StreamWriter(logFile, true);
            sw.Close();
        }
    }
}";

        var metadata = MakeMetadata("Logic/ExceptionUtility.cs");
        var serverShim = new ServerShimTransform();

        // Run in pipeline order: HttpContextAccessor (108) then ServerShim (330)
        var afterAccessor = _sut.Apply(input, metadata);
        var result = serverShim.Apply(afterAccessor, metadata);

        // Server.MapPath should be resolved to Path.Combine by ServerShimTransform
        result.ShouldContain("Path.Combine(AppContext.BaseDirectory");
        // Should NOT have the malformed _httpContextAccessor.HttpContext?.Path.Combine pattern
        result.ShouldNotContain("_httpContextAccessor.HttpContext?.Path");
        // Should NOT inject IHttpContextAccessor (nothing left to inject for)
        result.ShouldNotContain("IHttpContextAccessor");
        // System.IO using should remain for Path.Combine
        result.ShouldContain("using System.IO;");
    }
}
