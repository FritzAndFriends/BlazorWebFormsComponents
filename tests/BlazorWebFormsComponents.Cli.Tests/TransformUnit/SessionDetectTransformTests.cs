using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for SessionDetectTransform — detects Session["key"] and Cache["key"]
/// patterns, injects [Inject] SessionShim / CacheShim properties, and generates
/// migration guidance.
/// </summary>
public class SessionDetectTransformTests
{
    private readonly SessionDetectTransform _transform = new();

    private static FileMetadata TestMetadata(string content) => new()
    {
        SourceFilePath = "Default.aspx.cs",
        OutputFilePath = "Default.razor.cs",
        FileType = FileType.Page,
        OriginalContent = content
    };

    [Fact]
    public void InjectsSessionShimProperty()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Load() { var x = Session[""CartId""]; }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("[Inject] private SessionShim Session { get; set; }", result);
    }

    [Fact]
    public void InjectsCacheShimProperty()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Load() { var x = Cache[""Products""]; }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("[Inject] private CacheShim Cache { get; set; }", result);
    }

    [Fact]
    public void InjectsBothSessionAndCacheShims()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Load()
        {
            var x = Session[""CartId""];
            var y = Cache[""Products""];
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("[Inject] private SessionShim Session { get; set; }", result);
        Assert.Contains("[Inject] private CacheShim Cache { get; set; }", result);
    }

    [Fact]
    public void SessionGuidanceBlockIncludesAutoWiringNote()
    {
        var input = @"// =============================================================================
namespace MyApp
{
    public partial class MyPage
    {
        void Load() { var x = Session[""CartId""]; }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("// --- Session State Migration ---", result);
        Assert.Contains("SessionShim auto-wired via [Inject]", result);
        Assert.Contains("Session keys found: CartId", result);
        Assert.Contains("Options for long-term replacement:", result);
    }

    [Fact]
    public void CacheGuidanceBlockIncludesAutoWiringNote()
    {
        var input = @"// =============================================================================
namespace MyApp
{
    public partial class MyPage
    {
        void Load() { var x = Cache[""Products""]; }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("// --- Cache Migration ---", result);
        Assert.Contains("CacheShim auto-wired via [Inject]", result);
        Assert.Contains("Cache keys found: Products", result);
        Assert.Contains("CacheShim wraps IMemoryCache", result);
    }

    [Fact]
    public void CollectsMultipleUniqueSessionKeys()
    {
        var input = @"// =============================================================================
namespace MyApp
{
    public partial class MyPage
    {
        void Load()
        {
            var a = Session[""CartId""];
            var b = Session[""UserName""];
            var c = Session[""CartId""];
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("Session keys found: CartId, UserName", result);
        // CartId should appear only once in keys
        var keysLine = result.Split('\n').First(l => l.Contains("Session keys found:"));
        Assert.Equal(1, keysLine.Split("CartId").Length - 1);
    }

    [Fact]
    public void IdempotentDoesNotDuplicateInjectProperty()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Load() { var x = Session[""CartId""]; }
    }
}";
        var firstPass = _transform.Apply(input, TestMetadata(input));
        var secondPass = _transform.Apply(firstPass, TestMetadata(firstPass));

        // Count occurrences of the inject line
        var injectCount = secondPass.Split("[Inject] private SessionShim Session").Length - 1;
        Assert.Equal(1, injectCount);
    }

    [Fact]
    public void IdempotentDoesNotDuplicateGuidanceBlock()
    {
        var input = @"// =============================================================================
namespace MyApp
{
    public partial class MyPage
    {
        void Load() { var x = Session[""CartId""]; }
    }
}";
        var firstPass = _transform.Apply(input, TestMetadata(input));
        var secondPass = _transform.Apply(firstPass, TestMetadata(firstPass));

        var markerCount = secondPass.Split("// --- Session State Migration ---").Length - 1;
        Assert.Equal(1, markerCount);
    }

    [Fact]
    public void PreservesContentWithoutSessionOrCache()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Load() { var x = 42; }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Equal(input, result);
    }

    [Fact]
    public void DetectsVariableKeySessionAccess()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Load() { var x = Session[key]; }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("[Inject] private SessionShim Session { get; set; }", result);
    }

    [Fact]
    public void DoesNotMatchMemoryCacheIndexer()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Load() { var x = MemoryCache[""key""]; }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("[Inject] private CacheShim Cache", result);
    }

    [Fact]
    public void DoesNotInjectViewStateShim()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Load() { var x = ViewState[""key""]; }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.DoesNotContain("[Inject]", result);
    }

    [Fact]
    public void InjectPropertyAppearsAfterClassBrace()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Load() { var x = Session[""CartId""]; }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        var classIdx = result.IndexOf("public partial class MyPage");
        var braceIdx = result.IndexOf('{', classIdx);
        var injectIdx = result.IndexOf("[Inject] private SessionShim Session", braceIdx);
        Assert.True(injectIdx > braceIdx, "[Inject] should appear after the class opening brace");
    }

    [Fact]
    public void SessionCallsPreservedAsIs()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Load()
        {
            var cartId = Session[""CartId""];
            Session[""UserName""] = ""test"";
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains(@"Session[""CartId""]", result);
        Assert.Contains(@"Session[""UserName""] = ""test""", result);
    }

    [Fact]
    public void DetectsHttpContextCurrentSession_ReplacesWithSession()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Load() { var x = HttpContext.Current.Session[""CartId""]; }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        // The actual code line should have HttpContext.Current.Session replaced
        Assert.DoesNotContain(@"var x = HttpContext.Current.Session[""CartId""]", result);
        Assert.Contains(@"var x = Session[""CartId""]", result);
        Assert.Contains("TODO(bwfc-session-state): HttpContext.Current.Session was replaced", result);
    }

    [Fact]
    public void HttpContextCurrentSession_StillInjectsSessionShim()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Load() { var x = HttpContext.Current.Session[""CartId""]; }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        // After replacement, Session[ is detected → [Inject] is added
        Assert.Contains("[Inject] private SessionShim Session { get; set; }", result);
    }

    [Fact]
    public void HttpContextCurrentSession_EmitsDiGuidanceForNonPage()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Load() { var x = HttpContext.Current.Session[""CartId""]; }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("non-page classes, inject SessionShim via constructor DI", result);
    }

    [Fact]
    public void HttpContextCurrentSession_Idempotent()
    {
        var input = @"namespace MyApp
{
    public partial class MyPage
    {
        void Load() { var x = HttpContext.Current.Session[""CartId""]; }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));
        result = _transform.Apply(result, TestMetadata(result));

        var count = result.Split("HttpContext.Current.Session was replaced").Length - 1;
        Assert.Equal(1, count);
    }

    [Fact]
    public void OrderIs400()
    {
        Assert.Equal(400, _transform.Order);
    }
}
