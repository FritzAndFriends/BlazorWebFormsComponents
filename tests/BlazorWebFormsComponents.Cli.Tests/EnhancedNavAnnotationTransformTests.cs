using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests;

public class EnhancedNavAnnotationTransformTests
{
    private readonly EnhancedNavAnnotationTransform _transform = new();

    private static FileMetadata MakeMetadata(FileType type) => new()
    {
        SourceFilePath = "test.aspx",
        OutputFilePath = "test.razor",
        FileType = type,
        OriginalContent = ""
    };

    [Fact]
    public void LogoutLink_GetsAnnotation()
    {
        var input = """<a href="/Account/Logout">Log out</a>""";

        var result = _transform.Apply(input, MakeMetadata(FileType.Page));

        Assert.Contains(@"data-enhance-nav=""false""", result);
    }

    [Fact]
    public void SignOutLink_GetsAnnotation()
    {
        var input = """<a href="/SignOut">Sign out</a>""";

        var result = _transform.Apply(input, MakeMetadata(FileType.Page));

        Assert.Contains(@"data-enhance-nav=""false""", result);
    }

    [Fact]
    public void AddToCartLink_GetsAnnotation()
    {
        var input = """<a href="/AddToCart?id=5">Add to Cart</a>""";

        var result = _transform.Apply(input, MakeMetadata(FileType.Page));

        Assert.Contains(@"data-enhance-nav=""false""", result);
    }

    [Fact]
    public void FormPost_GetsAnnotation()
    {
        var input = """<form method="post" action="/UpdateCart">""";

        var result = _transform.Apply(input, MakeMetadata(FileType.Page));

        Assert.Contains(@"data-enhance-nav=""false""", result);
    }

    [Fact]
    public void NormalLink_NoAnnotation()
    {
        var input = """<a href="/About">About</a>""";

        var result = _transform.Apply(input, MakeMetadata(FileType.Page));

        Assert.DoesNotContain("data-enhance-nav", result);
    }

    [Fact]
    public void AlreadyAnnotated_NoDoubleAnnotation()
    {
        var input = """<a href="/Account/Logout" data-enhance-nav="false">Log out</a>""";

        var result = _transform.Apply(input, MakeMetadata(FileType.Page));

        var count = result.Split(@"data-enhance-nav=""false""").Length - 1;
        Assert.Equal(1, count);
    }

    [Fact]
    public void NonPageFile_Skipped()
    {
        var input = """<a href="/Account/Logout">Log out</a>""";

        var result = _transform.Apply(input, MakeMetadata(FileType.Master));

        Assert.DoesNotContain("data-enhance-nav", result);
    }
}
