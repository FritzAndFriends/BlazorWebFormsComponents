using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.Transforms.CodeBehind;

public class SelfInstantiationTransformTests
{
    private readonly SelfInstantiationTransform _transform = new();

    private static FileMetadata MakeMetadata(string sourceFilePath = "Logic\\ShoppingCartActions.cs") => new()
    {
        SourceFilePath = sourceFilePath,
        OutputFilePath = sourceFilePath,
        FileType = FileType.CodeFile,
        OriginalContent = string.Empty
    };

    [Fact]
    public void Apply_RewritesServiceSelfInstantiationReturnPattern()
    {
        var input = """
            public partial class ShoppingCartActions
            {
                public ShoppingCartActions(ProductContext db)
                {
                }

                public string ShoppingCartId { get; set; } = string.Empty;

                public ShoppingCartActions GetCart(string id)
                {
                    var cart = new ShoppingCartActions();
                    cart.ShoppingCartId = id;
                    return cart;
                }
            }
            """;

        var result = _transform.Apply(input, MakeMetadata());

        Assert.Contains("this.ShoppingCartId = id;", result);
        Assert.Contains("return this;", result);
        Assert.DoesNotContain("new ShoppingCartActions()", result);
        Assert.DoesNotContain("var cart = this;", result);
    }

    [Fact]
    public void Apply_UnwrapsUsingBlockForLogicService()
    {
        var input = """
            public partial class ShoppingCartActions
            {
                public string ShoppingCartId { get; set; } = string.Empty;

                public string GetCartId() => "cart-1";

                public ShoppingCartActions GetCart()
                {
                    using (var cart = new ShoppingCartActions())
                    {
                        cart.ShoppingCartId = cart.GetCartId();
                        return cart;
                    }
                }
            }
            """;

        var result = _transform.Apply(input, MakeMetadata());

        Assert.Contains("this.ShoppingCartId = this.GetCartId();", result);
        Assert.Contains("return this;", result);
        Assert.DoesNotContain("using (var cart =", result);
        Assert.DoesNotContain("new ShoppingCartActions()", result);
    }

    [Fact]
    public void Apply_DoesNotRewriteNonDiClassWithSelfInstantiation()
    {
        var input = """
            public class ShoppingCartActions
            {
                public ShoppingCartActions Create()
                {
                    var cart = new ShoppingCartActions();
                    cart.ShoppingCartId = "abc";
                    return cart;
                }

                public string ShoppingCartId { get; set; } = string.Empty;
            }
            """;

        var result = _transform.Apply(input, MakeMetadata("Utilities\\ShoppingCartActions.cs"));

        Assert.Equal(input, result);
    }

    [Fact]
    public void Apply_LeavesExistingThisPatternUnchanged()
    {
        var input = """
            public partial class ShoppingCartActions
            {
                public ShoppingCartActions(ProductContext db)
                {
                }

                public string ShoppingCartId { get; set; } = string.Empty;

                public ShoppingCartActions GetCart(string id)
                {
                    this.ShoppingCartId = id;
                    return this;
                }
            }
            """;

        var result = _transform.Apply(input, MakeMetadata());

        Assert.Equal(input, result);
    }

    [Fact]
    public void Apply_DoesNotRewriteConstructorCallsWithArguments()
    {
        var input = """
            public partial class ShoppingCartActions
            {
                public ShoppingCartActions(ProductContext db)
                {
                }

                public void Update(ProductContext db)
                {
                    var actions = new ShoppingCartActions(db);
                }
            }
            """;

        var result = _transform.Apply(input, MakeMetadata());

        Assert.Contains("new ShoppingCartActions(db)", result);
        Assert.DoesNotContain("return this;", result);
    }
}
