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
    public void ConvertsEncodedDisplayExpressionToIdiomaticRazorForSimpleExpression()
    {
        var result = _transform.Apply("<span><%#: Item.Name %></span>", TestMetadata);

        Assert.Equal("<span>@Item.Name</span>", result);
    }

    [Fact]
    public void ConvertsColonEqualsDisplayExpressionToIdiomaticRazorForSimpleExpression()
    {
        var result = _transform.Apply("<span><%=: someVar %></span>", TestMetadata);

        Assert.Equal("<span>@someVar</span>", result);
    }

    [Fact]
    public void KeepsParenthesesForMethodCallExpression()
    {
        var result = _transform.Apply("<span><%#: item.GetPrice() %></span>", TestMetadata);

        Assert.Equal("<span>@(item.GetPrice())</span>", result);
    }

    [Fact]
    public void KeepsParenthesesForOperatorExpression()
    {
        var result = _transform.Apply("<span><%#: Item.UnitPrice + tax %></span>", TestMetadata);

        Assert.Equal("<span>@(Item.UnitPrice + tax)</span>", result);
    }

    [Fact]
    public void FixesBrokenGeneratedRazorDisplayExpressionToIdiomaticRazorForSimpleExpression()
    {
        var result = _transform.Apply("<span>@(: expr)</span>", TestMetadata);

        Assert.Equal("<span>@expr</span>", result);
    }

    [Fact]
    public void ConvertsStringFormatDisplayExpressionToParenthesizedRazor()
    {
        var result = _transform.Apply("<span><%#: String.Format(\"{0:c}\", Item.UnitPrice) %></span>", TestMetadata);

        Assert.Equal("<span>@(String.Format(\"{0:c}\", Item.UnitPrice))</span>", result);
    }

    [Fact]
    public void ConvertsComplexStringFormatDisplayExpressionToParenthesizedRazor()
    {
        var result = _transform.Apply("<span><%#: String.Format(\"{0:c}\", ((Convert.ToDouble(Item.Quantity)) * Convert.ToDouble(Item.Product.UnitPrice)))%></span>", TestMetadata);

        Assert.Equal("<span>@(String.Format(\"{0:c}\", ((Convert.ToDouble(Item.Quantity)) * Convert.ToDouble(Item.Product.UnitPrice))))</span>", result);
    }

    [Fact]
    public void LeavesBindExpressionsForDataBindingAttributeTransform()
    {
        var result = _transform.Apply("<span><%#: Bind(\"Name\") %></span>", TestMetadata);

        Assert.Equal("<span><%#: Bind(\"Name\") %></span>", result);
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

        Assert.Contains("@Item.Name", result);
    }

    [Fact]
    public void PipelinePreservesParenthesesForComplexDisplayExpression()
    {
        var pipeline = TestHelpers.CreateDefaultPipeline();
        var metadata = new FileMetadata
        {
            SourceFilePath = "Product.aspx",
            OutputFilePath = "Product.razor",
            FileType = FileType.Page,
            OriginalContent = string.Empty
        };

        var result = pipeline.TransformMarkup("<ItemTemplate><%#: item.GetPrice() %></ItemTemplate>", metadata);

        Assert.Contains("@(item.GetPrice())", result);
    }

    [Fact]
    public void PipelineConvertsStringFormatDisplayExpression()
    {
        var pipeline = TestHelpers.CreateDefaultPipeline();
        var metadata = new FileMetadata
        {
            SourceFilePath = "Product.aspx",
            OutputFilePath = "Product.razor",
            FileType = FileType.Page,
            OriginalContent = string.Empty
        };

        var result = pipeline.TransformMarkup("<ItemTemplate><%#: String.Format(\"{0:c}\", Item.UnitPrice) %></ItemTemplate>", metadata);

        Assert.Contains("@(String.Format(\"{0:c}\", Item.UnitPrice))", result);
    }
}
