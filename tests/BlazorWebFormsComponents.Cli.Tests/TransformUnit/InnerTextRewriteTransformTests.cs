using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

public class InnerTextRewriteTransformTests
{
    private readonly InnerTextRewriteTransform _transform = new();

    [Fact]
    public void RewritesInnerTextAssignment()
    {
        var metadata = new FileMetadata
        {
            SourceFilePath = "ShoppingCart.aspx.cs",
            OutputFilePath = "ShoppingCart.razor.cs",
            FileType = FileType.Page,
            OriginalContent = string.Empty
        };

        var input = @"namespace TestApp;

public partial class ShoppingCart
{
    private void UpdateCart()
    {
        ShoppingCartTitle.InnerText = ""Shopping Cart is Empty"";
    }
}";

        var result = _transform.Apply(input, metadata);

        Assert.Contains("ShoppingCartTitle = \"Shopping Cart is Empty\"", result);
        Assert.DoesNotContain(".InnerText", result);
    }

    [Fact]
    public void RewritesInnerHtmlAssignment()
    {
        var metadata = new FileMetadata
        {
            SourceFilePath = "Default.aspx.cs",
            OutputFilePath = "Default.razor.cs",
            FileType = FileType.Page,
            OriginalContent = string.Empty
        };

        var input = @"namespace TestApp;

public partial class Default
{
    private void Load()
    {
        cartCount.InnerHtml = cartStr;
    }
}";

        var result = _transform.Apply(input, metadata);

        Assert.Contains("cartCount = cartStr", result);
        Assert.DoesNotContain(".InnerHtml", result);
    }

    [Fact]
    public void DoesNotModifyContent_WithoutInnerTextOrInnerHtml()
    {
        var metadata = new FileMetadata
        {
            SourceFilePath = "About.aspx.cs",
            OutputFilePath = "About.razor.cs",
            FileType = FileType.Page,
            OriginalContent = string.Empty
        };

        var input = @"namespace TestApp;

public partial class About
{
    private string title = ""About"";
}";

        var result = _transform.Apply(input, metadata);

        Assert.Equal(input, result);
    }
}
