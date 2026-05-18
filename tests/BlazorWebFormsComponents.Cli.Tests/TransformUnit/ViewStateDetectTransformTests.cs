using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for ViewStateDetectTransform — detects ViewState["key"] patterns
/// and generates migration guidance with suggested field declarations.
/// </summary>
public class ViewStateDetectTransformTests
{
    private readonly ViewStateDetectTransform _transform = new();

    private static FileMetadata TestMetadata(string content) => new()
    {
        SourceFilePath = "Default.aspx.cs",
        OutputFilePath = "Default.razor.cs",
        FileType = FileType.Page,
        OriginalContent = content
    };

    [Fact]
    public void DetectsViewStateKey_InjectsGuidance()
    {
        var input = @"// =============================================================================
namespace MyApp
{
    public partial class MyPage
    {
        void Load() { var x = ViewState[""SortOrder""]; }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("// --- ViewState Migration ---", result);
        Assert.Contains("ViewState is in-memory only in Blazor", result);
    }

    [Fact]
    public void SuggestsFieldDeclaration_WithCorrectCamelCase()
    {
        var input = @"// =============================================================================
namespace MyApp
{
    public partial class MyPage
    {
        void Load() { var x = ViewState[""SortOrder""]; }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains(@"private object _sortOrder; // was ViewState[""SortOrder""]", result);
    }

    [Fact]
    public void DetectsMultipleUniqueKeys()
    {
        var input = @"// =============================================================================
namespace MyApp
{
    public partial class MyPage
    {
        void Load()
        {
            var a = ViewState[""SortOrder""];
            var b = ViewState[""PageIndex""];
            var c = ViewState[""SortOrder""];
        }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("_sortOrder", result);
        Assert.Contains("_pageIndex", result);
        // SortOrder appears twice but should only generate one field
        var sortOrderCount = result.Split("_sortOrder").Length - 1;
        Assert.Equal(1, sortOrderCount);
    }

    [Fact]
    public void IncludesObsoleteShimNote()
    {
        var input = @"// =============================================================================
namespace MyApp
{
    public partial class MyPage
    {
        void Load() { var x = ViewState[""Key1""]; }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("BaseWebFormsComponent.ViewState", result);
        Assert.Contains("[Obsolete]", result);
    }

    [Fact]
    public void InsertsGuidance_AfterTodoHeaderEndMarker()
    {
        var input = @"// =============================================================================
namespace MyApp
{
    public partial class MyPage
    {
        void Load() { var x = ViewState[""Key1""]; }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        var markerIdx = result.LastIndexOf("// =============================================================================");
        var guidanceIdx = result.IndexOf("// --- ViewState Migration ---");
        Assert.True(guidanceIdx > markerIdx, "Guidance should appear after the TODO end marker");
    }

    [Fact]
    public void PreservesContent_WithoutViewState()
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
    public void PreservesContent_NoViewStateMentions()
    {
        var input = "public partial class MyPage { void DoWork() { } }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Equal(input, result);
        Assert.DoesNotContain("ViewState Migration", result);
    }

    [Fact]
    public void OrderIs410()
    {
        Assert.Equal(410, _transform.Order);
    }
}
