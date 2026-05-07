using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

public class CompileSurfaceStubTransformTests
{
    private readonly CompileSurfaceStubTransform _transform = new();

    [Fact]
    public void IdentityReferences_AreStubbedWithQuarantinePlaceholder()
    {
        var metadata = CreateMetadata(
            @"D:\input\Checkout.aspx",
            @"D:\output\Checkout.razor",
            "@page \"/Checkout\"\n<div>Original</div>",
            "using Microsoft.AspNet.Identity;\nnamespace TestApp; public partial class Checkout { }");
        var input = "namespace TestApp; public partial class Checkout { }";

        var result = _transform.Apply(input, metadata);

        Assert.Contains("@page \"/Checkout\"", metadata.MarkupContent);
        Assert.Contains("@inherits BlazorWebFormsComponents.WebFormsPageBase", metadata.MarkupContent);
        Assert.Contains("Page Not Yet Migrated", metadata.MarkupContent);
        Assert.Contains("Original Web Forms features used: ASP.NET Identity or membership APIs", metadata.MarkupContent);
        Assert.Contains("public partial class Checkout : BlazorWebFormsComponents.WebFormsPageBase", result);
        Assert.Equal(input, metadata.CompileSurfaceOriginalCodeBehind);
        Assert.Contains("ASP.NET Identity", metadata.CompileSurfaceStubReason);
    }

    [Fact]
    public void PaymentReferences_AreStubbedWithMigrationGuidance()
    {
        var metadata = CreateMetadata(
            @"D:\input\Checkout.aspx",
            @"D:\output\Checkout.razor",
            "@page \"/Checkout\"\n<div>Original</div>",
            "using Stripe;\nnamespace TestApp; public partial class Checkout { }");
        var input = "namespace TestApp; public partial class Checkout { }";

        var result = _transform.Apply(input, metadata);

        Assert.True(metadata.CompileSurfaceStubReason!.Contains("payment-provider integration", StringComparison.OrdinalIgnoreCase));
        Assert.Contains("Suggested migration approach:", metadata.MarkupContent);
        Assert.Contains("public partial class Checkout : BlazorWebFormsComponents.WebFormsPageBase", result);
    }

    [Fact]
    public void LoginPage_IsExempt()
    {
        var metadata = CreateMetadata(
            @"D:\input\Account\Login.aspx",
            @"D:\output\Account\Login.razor",
            "@page \"/Account/Login\"\n<div>Original</div>",
            "using Microsoft.AspNet.Identity;\nnamespace TestApp.Account; public partial class Login { }");
        var input = "namespace TestApp.Account; public partial class Login { }";

        var result = _transform.Apply(input, metadata);

        Assert.Equal(input, result);
        Assert.Equal("@page \"/Account/Login\"\n<div>Original</div>", metadata.MarkupContent);
        Assert.Null(metadata.CompileSurfaceOriginalCodeBehind);
        Assert.Null(metadata.CompileSurfaceStubReason);
    }

    [Fact]
    public void NormalPage_IsNotStubbed()
    {
        var metadata = CreateMetadata(
            @"D:\input\Products.aspx",
            @"D:\output\Products.razor",
            "@page \"/Products\"\n<div>Original</div>",
            "namespace TestApp; public partial class Products { }");
        var input = "namespace TestApp; public partial class Products { }";

        var result = _transform.Apply(input, metadata);

        Assert.Equal(input, result);
        Assert.Equal("@page \"/Products\"\n<div>Original</div>", metadata.MarkupContent);
        Assert.Null(metadata.CompileSurfaceOriginalCodeBehind);
        Assert.Null(metadata.CompileSurfaceStubReason);
    }

    private static FileMetadata CreateMetadata(string sourcePath, string outputPath, string markup, string originalCodeBehind) => new()
    {
        SourceFilePath = sourcePath,
        OutputFilePath = outputPath,
        OutputRootPath = @"D:\output",
        SourceRootPath = @"D:\input",
        FileType = FileType.Page,
        OriginalContent = string.Empty,
        MarkupContent = markup,
        CodeBehindContent = originalCodeBehind,
        ProjectNamespace = "TestApp"
    };
}
