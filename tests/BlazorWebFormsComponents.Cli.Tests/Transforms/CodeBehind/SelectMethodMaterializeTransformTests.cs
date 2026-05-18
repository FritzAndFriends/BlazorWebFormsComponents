using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.Transforms.CodeBehind;

public class SelectMethodMaterializeTransformTests
{
    private readonly SelectMethodMaterializeTransform _transform = new();

    [Fact]
    public void Apply_MaterializesQueryableSelectMethodsUsingCreateDbContext()
    {
        const string input = """
            using System.Linq;

            public partial class Products
            {
                public IQueryable<Product> GetProducts(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
                {
                    using var db = DbFactory.CreateDbContext();
                    var query = db.Products.OrderBy(p => p.ProductName);
                    totalRowCount = query.Count();
                    return query;
                }
            }
            """;

        var result = _transform.Apply(input, CreateMetadata());

        Assert.Contains("var db = DbFactory.CreateDbContext();", result);
        Assert.DoesNotContain("using var db = DbFactory.CreateDbContext();", result);
        Assert.Contains("return query.ToList().AsQueryable();", result);
    }

    [Fact]
    public void Apply_DoesNotChangeNonQueryableMethods()
    {
        const string input = """
            public partial class Products
            {
                public List<Product> GetProducts()
                {
                    using var db = DbFactory.CreateDbContext();
                    return db.Products.ToList();
                }
            }
            """;

        var result = _transform.Apply(input, CreateMetadata());

        Assert.Equal(input, result);
    }

    private static FileMetadata CreateMetadata() => new()
    {
        SourceFilePath = "Products.aspx.cs",
        OutputFilePath = "Products.razor.cs",
        FileType = FileType.Page,
        OriginalContent = string.Empty
    };
}
