using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Cli.Tests.Transforms.CodeBehind;

public class DbContextInstantiationTransformTests
{
    private readonly DbContextInstantiationTransform _sut = new();

    [Fact]
    public void SelfInjection_IsExcluded()
    {
        var input = @"public class ShoppingCartActions
{
    public static ShoppingCartActions GetCart()
    {
        var cart = new ShoppingCartActions();
        return cart;
    }
}";
        var metadata = new FileMetadata
        {
            SourceFilePath = "Logic/ShoppingCartActions.cs",
            OutputFilePath = "Logic/ShoppingCartActions.cs",
            FileType = FileType.CodeFile,
            OriginalContent = string.Empty
        };

        var result = _sut.Apply(input, metadata);

        result.ShouldNotContain("private readonly ShoppingCartActions");
        result.ShouldNotContain("ShoppingCartActions shoppingCartActions");
    }

    [Fact]
    public void ExternalDependency_StillInjected()
    {
        var input = @"public class ShoppingCartActions
{
    public void DoWork()
    {
        var db = new ProductContext();
        db.Products.ToList();
    }
}";
        var metadata = new FileMetadata
        {
            SourceFilePath = "Logic/ShoppingCartActions.cs",
            OutputFilePath = "Logic/ShoppingCartActions.cs",
            FileType = FileType.CodeFile,
            OriginalContent = string.Empty
        };

        var result = _sut.Apply(input, metadata);

        result.ShouldContain("ProductContext");
        result.ShouldContain("_productContext");
    }
}
