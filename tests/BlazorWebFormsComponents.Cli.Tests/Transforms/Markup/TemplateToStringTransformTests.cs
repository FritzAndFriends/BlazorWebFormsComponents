using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.Transforms.Markup;

public class TemplateToStringTransformTests
{
    private readonly TemplateToStringTransform _transform = new();

    private static FileMetadata MakeMetadata() => new()
    {
        SourceFilePath = "Cart.aspx",
        OutputFilePath = "Cart.razor",
        FileType = FileType.Page,
        OriginalContent = string.Empty
    };

    [Fact]
    public void Apply_AddsToStringToSimpleItemTextBindingInsideTemplate()
    {
        var input = """
            <ItemTemplate Context="Item">
                <TextBox Text="@Item.Quantity"></TextBox>
            </ItemTemplate>
            """;

        var result = _transform.Apply(input, MakeMetadata());

        Assert.Contains("Text=\"@Item.Quantity.ToString()\"", result);
    }

    [Fact]
    public void Apply_DoesNotDoubleApplyExistingToString()
    {
        var input = """
            <ItemTemplate Context="Item">
                <TextBox Text="@Item.Quantity.ToString()"></TextBox>
            </ItemTemplate>
            """;

        var result = _transform.Apply(input, MakeMetadata());

        Assert.Equal(input, result);
    }

    [Fact]
    public void Apply_DoesNotChangeBindingsOutsideTemplates()
    {
        var input = "<TextBox Text=\"@Item.Quantity\"></TextBox>";

        var result = _transform.Apply(input, MakeMetadata());

        Assert.Equal(input, result);
    }
}
