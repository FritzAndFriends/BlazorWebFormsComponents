using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for SelectMethodTransform — preserves SelectMethod as working BWFC markup
/// and only adds TODO comments for CRUD method attributes that still need review.
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
    public void PreservesSelectMethodWithoutTodo()
    {
        var input = @"<GridView SelectMethod=""GetItems"" />";
        var result = _transform.Apply(input, MakeMetadata());

        Assert.Equal(input, result);
    }

    [Fact]
    public void AddsInsertMethodTodo()
    {
        var input = @"<GridView InsertMethod=""AddItem"" />";
        var result = _transform.Apply(input, MakeMetadata());

        Assert.Contains(@"InsertMethod=""AddItem""", result);
        Assert.Contains(@"TODO(bwfc-select-method): Review InsertMethod=""AddItem"" migration for BWFC event/CRUD handling", result);
    }

    [Fact]
    public void AddsUpdateMethodTodo()
    {
        var input = @"<ListView UpdateMethod=""SaveChanges"" />";
        var result = _transform.Apply(input, MakeMetadata());

        Assert.Contains(@"UpdateMethod=""SaveChanges""", result);
        Assert.Contains(@"TODO(bwfc-select-method): Review UpdateMethod=""SaveChanges"" migration for BWFC event/CRUD handling", result);
    }

    [Fact]
    public void AddsDeleteMethodTodo()
    {
        var input = @"<FormView DeleteMethod=""RemoveItem"" />";
        var result = _transform.Apply(input, MakeMetadata());

        Assert.Contains(@"DeleteMethod=""RemoveItem""", result);
        Assert.Contains(@"TODO(bwfc-select-method): Review DeleteMethod=""RemoveItem"" migration for BWFC event/CRUD handling", result);
    }

    [Fact]
    public void PreservesOriginalAttribute()
    {
        var input = @"<GridView SelectMethod=""GetProducts"" id=""gvProducts"" />";
        var result = _transform.Apply(input, MakeMetadata());

        Assert.Contains(@"<GridView SelectMethod=""GetProducts"" id=""gvProducts"" />", result);
    }

    [Fact]
    public void HandlesMutipleCrudMethodAttributesOnSameLine()
    {
        var input = @"<GridView SelectMethod=""GetItems"" InsertMethod=""AddItem"" DeleteMethod=""RemoveItem"" />";
        var result = _transform.Apply(input, MakeMetadata());

        var lines = result.Split('\n');
        var todoLines = lines.Where(l => l.Contains("TODO(bwfc-select-method)")).ToList();
        Assert.Equal(2, todoLines.Count);
        Assert.Contains(todoLines, l => l.Contains(@"InsertMethod=""AddItem"""));
        Assert.Contains(todoLines, l => l.Contains(@"DeleteMethod=""RemoveItem"""));
        Assert.Contains(@"SelectMethod=""GetItems""", lines[0]);
    }

    [Fact]
    public void DoesNotModifyContentWithoutMethodAttributes()
    {
        var input = @"<GridView id=""gvProducts"" CssClass=""table"" />";
        var result = _transform.Apply(input, MakeMetadata());

        Assert.Equal(input, result);
    }

    [Fact]
    public void CrudTodoAppearsAfterOriginalLine()
    {
        var input = @"<GridView SelectMethod=""GetItems"" DeleteMethod=""DeleteItem"" />";
        var result = _transform.Apply(input, MakeMetadata());

        var lines = result.Split('\n');
        Assert.Equal(2, lines.Length);
        Assert.Contains(@"SelectMethod=""GetItems""", lines[0]);
        Assert.StartsWith("@* TODO(bwfc-select-method):", lines[1]);
    }

    [Fact]
    public void LeavesStandaloneSelectMethodUntouched()
    {
        var input = @"<ListView SelectMethod=""LoadCustomers"" />";
        var result = _transform.Apply(input, MakeMetadata());

        Assert.Equal(input, result);
    }
}
