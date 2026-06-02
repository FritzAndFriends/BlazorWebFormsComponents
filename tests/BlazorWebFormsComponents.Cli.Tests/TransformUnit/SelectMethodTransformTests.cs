using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for SelectMethodTransform — preserves Web Forms model-binding attributes
/// because BWFC DataBoundComponent already supports them directly.
/// </summary>
public class SelectMethodTransformTests
{
    private readonly SelectMethodTransform _transform = new();

    private static FileMetadata MakeMetadata() => new()
    {
        SourceFilePath = "test.aspx",
        OutputFilePath = "test.razor",
        FileType = FileType.Page,
        OriginalContent = ""
    };

    [Fact]
    public void HasCorrectOrder()
    {
        Assert.Equal(520, _transform.Order);
    }

    [Fact]
    public void HasCorrectName()
    {
        Assert.Equal("SelectMethod", _transform.Name);
    }

    [Fact]
    public void PreservesSelectMethod()
    {
        var input = @"<GridView SelectMethod=""GetItems"" />";
        var result = _transform.Apply(input, MakeMetadata());

        Assert.Equal(input, result);
    }

    [Fact]
    public void PreservesCrudMethodAttributes()
    {
        var input = @"<GridView SelectMethod=""GetItems"" InsertMethod=""AddItem"" UpdateMethod=""SaveItem"" DeleteMethod=""DeleteItem"" />";
        var result = _transform.Apply(input, MakeMetadata());

        Assert.Equal(input, result);
    }

    [Fact]
    public void DoesNotEmitTodoCommentsForCrudMethods()
    {
        var input = @"<GridView UpdateMethod=""SaveItem"" DeleteMethod=""DeleteItem"" />";
        var result = _transform.Apply(input, MakeMetadata());

        Assert.DoesNotContain("TODO(bwfc-select-method)", result);
    }

    [Fact]
    public void LeavesContentWithoutModelBindingUntouched()
    {
        var input = @"<GridView id=""gvProducts"" CssClass=""table"" />";
        var result = _transform.Apply(input, MakeMetadata());

        Assert.Equal(input, result);
    }
}
