using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.Transforms.CodeBehind;

public class SelfInstantiationTransformTests
{
    private readonly SelfInstantiationTransform _transform = new();

    private static FileMetadata MakeMetadata() => new()
    {
        SourceFilePath = "ShoppingCartActions.aspx.cs",
        OutputFilePath = "ShoppingCartActions.razor.cs",
        FileType = FileType.CodeFile,
        OriginalContent = string.Empty
    };

    [Fact]
    public void Apply_RewritesZeroArgSelfInstantiationToThis()
    {
        var input = """
            public partial class ShoppingCartActions
            {
                private readonly ProductContext _db;

                public ShoppingCartActions(ProductContext db)
                {
                    _db = db;
                }

                public void Update()
                {
                    var actions = new ShoppingCartActions();
                }
            }
            """;

        var result = _transform.Apply(input, MakeMetadata());

        Assert.Contains("var actions = this;", result);
        Assert.DoesNotContain("new ShoppingCartActions()", result);
    }

    [Fact]
    public void Apply_DoesNotRewriteDifferentTypeInstantiation()
    {
        var input = """
            public partial class ShoppingCartActions
            {
                public void Update()
                {
                    var other = new CartItem();
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
        Assert.DoesNotContain("var actions = this;", result);
    }
}
