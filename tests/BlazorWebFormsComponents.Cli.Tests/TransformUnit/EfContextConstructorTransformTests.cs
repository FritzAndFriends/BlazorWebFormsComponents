using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

public class EfContextConstructorTransformTests
{
    private readonly EfContextConstructorTransform _transform = new();

    private static FileMetadata TestMetadata(string content) => new()
    {
        SourceFilePath = "ProductContext.cs",
        OutputFilePath = "ProductContext.cs",
        FileType = FileType.Page,
        OriginalContent = content
    };

    [Fact]
    public void HasExpectedMetadata()
    {
        Assert.Equal("EfContextConstructor", _transform.Name);
        Assert.Equal(106, _transform.Order);
    }

    [Fact]
    public void RewritesDbContextStringConstructor()
    {
        var input = """
            using System.Data.Entity;
            
            public class ProductContext : DbContext
            {
                public ProductContext() : base("WingtipToys")
                {
                }
            }
            """;

        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("using Microsoft.EntityFrameworkCore;", result);
        Assert.Contains("public ProductContext(DbContextOptions<ProductContext> options) : base(options)", result);
        Assert.DoesNotContain("base(\"WingtipToys\")", result);
    }

    [Fact]
    public void RewritesIdentityDbContextStringConstructor()
    {
        var input = """
            public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
            {
                public ApplicationDbContext() : base("DefaultConnection")
                {
                    Seed();
                }
            }
            """;

        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Contains("using Microsoft.EntityFrameworkCore;", result);
        Assert.Contains("public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)", result);
        Assert.Contains("Seed();", result);
    }

    [Fact]
    public void DoesNotDuplicateEntityFrameworkCoreUsing()
    {
        var input = """
            using Microsoft.EntityFrameworkCore;
            
            public class ProductContext : DbContext
            {
                public ProductContext() : base("WingtipToys")
                {
                }
            }
            """;

        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Equal(1, result.Split("using Microsoft.EntityFrameworkCore;").Length - 1);
    }

    [Fact]
    public void LeavesNonDbContextClassesUnchanged()
    {
        var input = """
            public class NotAContext : SomeBaseClass
            {
                public NotAContext() : base("WingtipToys")
                {
                }
            }
            """;

        var result = _transform.Apply(input, TestMetadata(input));

        Assert.Equal(input, result);
    }
}
