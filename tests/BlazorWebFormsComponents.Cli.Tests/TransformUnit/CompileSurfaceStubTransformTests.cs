using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

public class CompileSurfaceStubTransformTests
{
    private readonly CompileSurfaceStubTransform _transform = new();

    [Fact]
    public void IdentityReferences_AreStubbed()
    {
        var metadata = CreateMetadata(
            @"D:\input\Checkout.aspx",
            @"D:\output\Checkout.razor",
            "@page \"/Checkout\"\n<div>Original</div>",
            "using Microsoft.AspNet.Identity;\nnamespace TestApp { public partial class Checkout { } }");
        var input = "namespace TestApp { public partial class Checkout { } }";

        var result = _transform.Apply(input, metadata);

        Assert.Contains("@page \"/Checkout\"", metadata.MarkupContent);
        Assert.Contains("This page is under migration", metadata.MarkupContent);
        Assert.Contains("preserved in migration-artifacts", metadata.MarkupContent);
        Assert.Contains("public partial class Checkout : ComponentBase", result);
        Assert.Equal(input, metadata.CompileSurfaceOriginalCodeBehind);
        Assert.Contains("ASP.NET Identity references", metadata.CompileSurfaceStubReason);
    }

    [Fact]
    public void AccountPath_IsStubbed()
    {
        var metadata = CreateMetadata(
            @"D:\input\Account\Manage.aspx",
            @"D:\output\Account\Manage.razor",
            "@page \"/Account/Manage\"\n<div>Original</div>",
            "namespace TestApp.Account { public partial class Manage { } }");
        var input = "namespace TestApp.Account { public partial class Manage { } }";

        var result = _transform.Apply(input, metadata);

        Assert.Contains("@page \"/Account/Manage\"", metadata.MarkupContent);
        Assert.Contains("public partial class Manage : ComponentBase", result);
        Assert.Contains("Account/Admin page infrastructure", metadata.CompileSurfaceStubReason);
    }

    [Fact]
    public void LoginPage_IsExempt()
    {
        var metadata = CreateMetadata(
            @"D:\input\Account\Login.aspx",
            @"D:\output\Account\Login.razor",
            "@page \"/Account/Login\"\n<div>Original</div>",
            "using Microsoft.AspNet.Identity;\nnamespace TestApp.Account { public partial class Login { } }");
        var input = "namespace TestApp.Account { public partial class Login { } }";

        var result = _transform.Apply(input, metadata);

        Assert.Equal(input, result);
        Assert.Equal("@page \"/Account/Login\"\n<div>Original</div>", metadata.MarkupContent);
        Assert.Null(metadata.CompileSurfaceOriginalCodeBehind);
        Assert.Null(metadata.CompileSurfaceStubReason);
    }

    [Fact]
    public void RegisterPage_IsExempt()
    {
        var metadata = CreateMetadata(
            @"D:\input\Account\Register.aspx",
            @"D:\output\Account\Register.razor",
            "@page \"/Account/Register\"\n<div>Original</div>",
            "using Owin;\nnamespace TestApp.Account { public partial class Register { } }");
        var input = "namespace TestApp.Account { public partial class Register { } }";

        var result = _transform.Apply(input, metadata);

        Assert.Equal(input, result);
        Assert.Equal("@page \"/Account/Register\"\n<div>Original</div>", metadata.MarkupContent);
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
            "namespace TestApp { public partial class Products { } }");
        var input = "namespace TestApp { public partial class Products { } }";

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
        FileType = FileType.Page,
        OriginalContent = string.Empty,
        MarkupContent = markup,
        CodeBehindContent = originalCodeBehind
    };
}
