using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

public class MarkupReferencedMemberStubTransformTests
{
    private readonly MarkupReferencedMemberStubTransform _transform = new();

    [Fact]
    public void AddsFieldRenderMethodAndEventHandlerStubs_WhenMarkupReferencesAreMissing()
    {
        var metadata = new FileMetadata
        {
            SourceFilePath = "CheckoutReview.aspx",
            OutputFilePath = "CheckoutReview.razor.cs",
            FileType = FileType.Page,
            OriginalContent = string.Empty,
            MarkupContent = "<Button OnClick=\"@PlaceOrder\" />\n<div>@_orderTotal</div>\n<p>@FormatTotal()</p>"
        };

        var input = "namespace TestApp;\n\npublic partial class CheckoutReview\n{\n}";

        var result = _transform.Apply(input, metadata);

        Assert.Contains("private object? _orderTotal; // TODO: migrate from Web Forms code-behind", result);
        Assert.Contains("protected object? FormatTotal()", result);
        Assert.Contains("return null;", result);
        Assert.Contains("protected void PlaceOrder()", result);
    }

    [Fact]
    public void DoesNotDuplicateMembers_ThatAlreadyExist()
    {
        var metadata = new FileMetadata
        {
            SourceFilePath = "ShoppingCart.aspx",
            OutputFilePath = "ShoppingCart.razor.cs",
            FileType = FileType.Page,
            OriginalContent = string.Empty,
            MarkupContent = "<Button OnClick=\"@Checkout\" />\n<div>@_cartTotal</div>\n<p>@FormatTotal()</p>"
        };

        var input = "namespace TestApp;\n\npublic partial class ShoppingCart\n{\n    private object? _cartTotal;\n\n    protected object? FormatTotal()\n    {\n        return _cartTotal;\n    }\n\n    protected void Checkout(object? sender, EventArgs e)\n    {\n    }\n}";

        var result = _transform.Apply(input, metadata);

        Assert.Equal(input, result);
    }

    [Fact]
    public void AddsPropertyStub_ForParenthesizedPascalCaseReferences()
    {
        var metadata = new FileMetadata
        {
            SourceFilePath = "Register.aspx",
            OutputFilePath = "Register.razor.cs",
            FileType = FileType.Page,
            OriginalContent = string.Empty,
            MarkupContent = "<h3>Register with your @(ProviderName) account</h3>"
        };

        var input = "namespace TestApp;\n\npublic partial class Register : BlazorWebFormsComponents.WebFormsPageBase\n{\n}";

        var result = _transform.Apply(input, metadata);

        Assert.Contains("private string ProviderName { get; set; } = \"\"; // TODO: migrate from Web Forms code-behind", result);
    }

    [Fact]
    public void DoesNotAddPropertyStub_WhenAlreadyDeclared()
    {
        var metadata = new FileMetadata
        {
            SourceFilePath = "Register.aspx",
            OutputFilePath = "Register.razor.cs",
            FileType = FileType.Page,
            OriginalContent = string.Empty,
            MarkupContent = "<h3>Register with your @(ProviderName) account</h3>"
        };

        var input = "namespace TestApp;\n\npublic partial class Register\n{\n    private string ProviderName { get; set; } = \"External\";\n}";

        var result = _transform.Apply(input, metadata);

        Assert.Equal(input, result);
    }
}
