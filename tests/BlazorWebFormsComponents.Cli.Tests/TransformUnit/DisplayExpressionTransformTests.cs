using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

public class DisplayExpressionTransformTests
{
    private readonly DisplayExpressionTransform _transform = new();

    private static FileMetadata TestMetadata => new()
    {
        SourceFilePath = "test.aspx",
        OutputFilePath = "test.razor",
        FileType = FileType.Page,
        OriginalContent = string.Empty
    };

    [Fact]
    public void HasExpectedMetadata()
    {
        Assert.Equal("DisplayExpression", _transform.Name);
        Assert.Equal(490, _transform.Order);
    }

    [Fact]
    public void ConvertsEncodedDisplayExpression()
    {
        var result = _transform.Apply("<span><%#: Item.Name %></span>", TestMetadata);

        Assert.Equal("<span>@(Item.Name)</span>", result);
    }

    [Fact]
    public void ConvertsColonEqualsDisplayExpression()
    {
        var result = _transform.Apply("<span><%=: someVar %></span>", TestMetadata);

        Assert.Equal("<span>@(someVar)</span>", result);
    }

    [Fact]
    public void FixesBrokenGeneratedRazorDisplayExpression()
    {
        var result = _transform.Apply("<span>@(: expr)</span>", TestMetadata);

        Assert.Equal("<span>@(expr)</span>", result);
    }

    [Fact]
    public void LeavesEvalExpressionsForExpressionTransform()
    {
        var result = _transform.Apply("<span><%#: Eval(\"Name\") %></span>", TestMetadata);

        Assert.Equal("<span><%#: Eval(\"Name\") %></span>", result);
    }

    [Fact]
    public void PipelinePreservesNormalizedDisplayExpression()
    {
        var pipeline = TestHelpers.CreateDefaultPipeline();
        var metadata = new FileMetadata
        {
            SourceFilePath = "Product.aspx",
            OutputFilePath = "Product.razor",
            FileType = FileType.Page,
            OriginalContent = string.Empty
        };

        var result = pipeline.TransformMarkup("<ItemTemplate><%#: Item.Name %></ItemTemplate>", metadata);

        Assert.Contains("@(Item.Name)", result);
    }
}
