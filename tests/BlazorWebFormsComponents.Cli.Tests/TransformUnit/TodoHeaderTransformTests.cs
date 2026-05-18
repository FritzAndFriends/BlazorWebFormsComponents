using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for TodoHeaderTransform — injects the TODO migration guidance header
/// at the top of code-behind files.
/// </summary>
public class TodoHeaderTransformTests
{
    private readonly TodoHeaderTransform _transform = new();

    private static FileMetadata TestMetadata(string content) => new()
    {
        SourceFilePath = "Default.aspx.cs",
        OutputFilePath = "Default.razor.cs",
        FileType = FileType.Page,
        OriginalContent = content
    };

    [Fact]
    public void InjectsHeader_BeforeExistingContent()
    {
        var input = "using System;\nnamespace MyApp { }";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.StartsWith("// ====", result.TrimStart());
        Assert.Contains("using System;", result);
    }

    [Fact]
    public void HeaderContains_GeneralTodoMarker()
    {
        var result = _transform.Apply("", TestMetadata(""));

        Assert.Contains("TODO(bwfc-general)", result);
    }

    [Fact]
    public void HeaderContains_LifecycleTodo()
    {
        var result = _transform.Apply("", TestMetadata(""));

        Assert.Contains("TODO(bwfc-lifecycle)", result);
        Assert.Contains("Page_Load", result);
    }

    [Fact]
    public void HeaderContains_IsPostBackTodo()
    {
        var result = _transform.Apply("", TestMetadata(""));

        Assert.Contains("TODO(bwfc-ispostback)", result);
    }

    [Fact]
    public void HeaderContains_ViewStateTodo()
    {
        var result = _transform.Apply("", TestMetadata(""));

        Assert.Contains("TODO(bwfc-viewstate)", result);
    }

    [Fact]
    public void HeaderContains_SessionStateTodo()
    {
        var result = _transform.Apply("", TestMetadata(""));

        Assert.Contains("TODO(bwfc-session-state)", result);
    }

    [Fact]
    public void HeaderContains_NavigationTodo()
    {
        var result = _transform.Apply("", TestMetadata(""));

        Assert.Contains("TODO(bwfc-navigation)", result);
        Assert.Contains("ResponseShim", result);
    }

    [Fact]
    public void HeaderContains_DataSourceTodo()
    {
        var result = _transform.Apply("", TestMetadata(""));

        Assert.Contains("TODO(bwfc-datasource)", result);
    }

    [Fact]
    public void HeaderContains_ClosingDelimiter()
    {
        var result = _transform.Apply("", TestMetadata(""));

        // Count the delimiter lines — should have opening and closing
        var delimiterCount = result.Split("// =============================================================================").Length - 1;
        Assert.True(delimiterCount >= 2, "Header should have opening and closing delimiter lines");
    }

    [Fact]
    public void PreservesOriginalContent_AfterHeader()
    {
        var input = @"namespace MyApp
{
    public partial class Default
    {
        void Page_Load() { }
    }
}";
        var result = _transform.Apply(input, TestMetadata(input));

        Assert.EndsWith(input, result);
    }

    [Fact]
    public void EmptyInput_ProducesHeaderOnly()
    {
        var result = _transform.Apply("", TestMetadata(""));

        Assert.Contains("TODO(bwfc-general)", result);
        Assert.Contains("=============", result);
    }

    [Fact]
    public void OrderIs10()
    {
        Assert.Equal(10, _transform.Order);
    }
}
