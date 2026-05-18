using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.Transforms.CodeBehind;

public class DuplicateRouteParameterTransformTests
{
    private readonly DuplicateRouteParameterTransform _transform = new();

    private static FileMetadata MakeMetadata(string markup) => new()
    {
        SourceFilePath = "ProductList.aspx.cs",
        OutputFilePath = "ProductList.razor.cs",
        FileType = FileType.Page,
        OriginalContent = string.Empty,
        MarkupContent = markup,
    };

    [Fact]
    public void Apply_RemovesDuplicateParameterWhenRouteTokenMatchesCaseInsensitiveNames()
    {
        var markup = "@page \"/Category/{categoryName}\"";
        var input = """
            using Microsoft.AspNetCore.Components;

            public partial class ProductList
            {
                [Parameter] public string? categoryName { get; set; }
                [Parameter] public string? CategoryName { get; set; }
            }
            """;

        var result = _transform.Apply(input, MakeMetadata(markup));

        Assert.Contains("[Parameter] public string? categoryName { get; set; }", result);
        Assert.DoesNotContain("[Parameter] public string? CategoryName { get; set; }", result);
    }

    [Fact]
    public void Apply_KeepsSingleMatchingParameter()
    {
        var markup = "@page \"/Category/{categoryName}\"";
        var input = """
            using Microsoft.AspNetCore.Components;

            public partial class ProductList
            {
                [Parameter] public string? CategoryName { get; set; }
            }
            """;

        var result = _transform.Apply(input, MakeMetadata(markup));

        Assert.Equal(input, result);
    }

    [Fact]
    public void Apply_IgnoresNonRouteParameters()
    {
        var markup = "@page \"/Category/{categoryName}\"";
        var input = """
            using Microsoft.AspNetCore.Components;

            public partial class ProductList
            {
                [Parameter] public string? categoryName { get; set; }
                [Parameter] public int? CategoryId { get; set; }
            }
            """;

        var result = _transform.Apply(input, MakeMetadata(markup));

        Assert.Equal(input, result);
    }
}
