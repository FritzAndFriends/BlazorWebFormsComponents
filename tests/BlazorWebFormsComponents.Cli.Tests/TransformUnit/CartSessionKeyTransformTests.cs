using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

public class CartSessionKeyTransformTests
{
    private readonly CartSessionKeyTransform _transform = new();

    private static FileMetadata TestMetadata(string content) => new()
    {
        SourceFilePath = "ShoppingCart.aspx.cs",
        OutputFilePath = "ShoppingCart.razor.cs",
        FileType = FileType.Page,
        OriginalContent = content
    };

    [Fact]
    public void SessionIdUsedAsCartIdentifier_RewritesToStableCartKeyHelper()
    {
        var input = @"namespace MyApp
{
    public partial class ShoppingCart
    {
        void LoadCart()
        {
            var cartId = Session.Id;
            var cart = _cartService.GetCart(cartId);
        }
    }
}";

        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("private string GetOrCreateCartKey()", result);
        Assert.Contains("Session[\"cart-key\"]?.ToString()", result);
        Assert.Contains("Session[\"cart-key\"] = cartKey;", result);
        Assert.Contains("var cartId = GetOrCreateCartKey();", result);
        Assert.DoesNotContain("var cartId = Session.Id;", result);
    }

    [Fact]
    public void HttpContextSessionIdPassedToCartService_RewritesCall()
    {
        var input = @"namespace MyApp
{
    public partial class ShoppingCart
    {
        void LoadCart()
        {
            var cart = _cartService.GetCart(HttpContext.Session.Id);
        }
    }
}";

        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("_cartService.GetCart(GetOrCreateCartKey())", result);
        Assert.DoesNotContain("HttpContext.Session.Id", result);
        Assert.Contains("Session[\"cart-key\"]?.ToString()", result);
    }

    [Fact]
    public void SessionIdInNonCartContext_IsLeftAlone()
    {
        var input = @"namespace MyApp
{
    public partial class AnalyticsPage
    {
        void TrackVisit()
        {
            var sessionId = Session.Id;
            Audit(sessionId);
        }
    }
}";

        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Equal(input, result);
    }

    [Fact]
    public void MultipleCartLookups_InjectHelperOnlyOnce()
    {
        var input = @"namespace MyApp
{
    public partial class ShoppingCart
    {
        void LoadCart()
        {
            var cartId = Session.Id;
            var count = _cartService.GetCount(Session.Id);
        }
    }
}";

        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Equal(1, result.Split("private string GetOrCreateCartKey()", StringSplitOptions.None).Length - 1);
        Assert.Equal(2, result.Split("GetOrCreateCartKey()", StringSplitOptions.None).Length - 1);
        Assert.DoesNotContain("Session.Id", result);
    }

    [Fact]
    public void DefaultPipeline_RewritesCartSessionIdPattern()
    {
        var input = @"namespace MyApp
{
    public partial class ShoppingCart
    {
        void LoadCart()
        {
            var cart = _cartService.GetCart(Session.Id);
        }
    }
}";

        var pipeline = TestHelpers.CreateDefaultPipeline();
        var result = pipeline.TransformCodeBehind(input, TestMetadata(input));

        Assert.Contains("GetOrCreateCartKey", result);
        Assert.Contains("Session[\"cart-key\"]", result);
        Assert.DoesNotContain("GetCart(Session.Id)", result);
    }

    [Fact]
    public void OrderIs390()
    {
        Assert.Equal(390, _transform.Order);
    }
}
