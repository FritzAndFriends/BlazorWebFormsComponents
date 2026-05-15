using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.Transforms.CodeBehind;

public class EagerLoadNavigationTransformTests
{
    private readonly EagerLoadNavigationTransform _transform = new();

    private static FileMetadata MakeCodeFileMetadata() => new()
    {
        SourceFilePath = "Logic/Test.cs",
        OutputFilePath = "Logic/Test.cs",
        FileType = FileType.CodeFile,
        OriginalContent = string.Empty
    };

    #region Navigation Property Extraction

    [Fact]
    public void ExtractNavigationProperties_FindsBoundFieldDottedPaths()
    {
        var markup = """
            <BoundField DataField="Product.ProductName" HeaderText="Name" />
            <BoundField DataField="Category.CategoryName" HeaderText="Category" />
            <BoundField DataField="UnitPrice" HeaderText="Price" />
            """;

        var result = EagerLoadNavigationTransform.ExtractNavigationProperties(markup);

        Assert.Contains("Product", result);
        Assert.Contains("Category", result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void ExtractNavigationProperties_FindsEvalDottedPaths()
    {
        var markup = """
            <%# Eval("Product.ProductName") %>
            <%# Eval("Product.UnitPrice") %>
            <%# Eval("Quantity") %>
            """;

        var result = EagerLoadNavigationTransform.ExtractNavigationProperties(markup);

        Assert.Contains("Product", result);
        Assert.Single(result);
    }

    [Fact]
    public void ExtractNavigationPropertiesFromCode_FindsLinqAccess()
    {
        var code = """
            var total = (from cartItems in _db.ShoppingCartItems
                         where cartItems.CartId == id
                         select cartItems.Product.UnitPrice).Sum();
            """;

        var result = EagerLoadNavigationTransform.ExtractNavigationPropertiesFromCode(code);

        Assert.Contains("Product", result);
    }

    [Fact]
    public void ExtractNavigationPropertiesFromCode_FindsMemberAccess()
    {
        var code = """
            foreach (var item in cartItems)
            {
                if (item.Product.ProductID == updates[i].ProductId)
                {
                    total += item.Product.UnitPrice;
                }
            }
            """;

        var result = EagerLoadNavigationTransform.ExtractNavigationPropertiesFromCode(code);

        Assert.Contains("Product", result);
    }

    [Fact]
    public void ExtractNavigationPropertiesFromCode_FiltersKnownNonNavProperties()
    {
        var code = """
            var x = item.CartId.ToString();
            var y = item.Quantity.Value;
            """;

        var result = EagerLoadNavigationTransform.ExtractNavigationPropertiesFromCode(code);

        Assert.Empty(result);
    }

    #endregion

    #region Strategy 1: .ToList().AsQueryable()

    [Fact]
    public void Apply_InjectsIncludeBeforeToListAsQueryable()
    {
        var input = """
            using System.Linq;

            public partial class ProductList
            {
                private readonly ProductContext _db;

                public IQueryable<Product> GetProducts()
                {
                    using var db = _contextFactory.CreateDbContext();
                    return db.Products.Where(p => p.Active).ToList().AsQueryable();
                }
            }
            """;

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var markupPath = Path.Combine(tempDir, "ProductList.razor");
        var codeBehindPath = Path.Combine(tempDir, "ProductList.razor.cs");
        File.WriteAllText(markupPath, """<BoundField DataField="Category.CategoryName" />""");

        try
        {
            var metadata = new FileMetadata
            {
                SourceFilePath = "ProductList.aspx.cs",
                OutputFilePath = codeBehindPath,
                FileType = FileType.Page,
                OriginalContent = string.Empty
            };

            var result = _transform.Apply(input, metadata);

            Assert.Contains(".Include(x => x.Category).ToList().AsQueryable()", result);
            Assert.Contains("using Microsoft.EntityFrameworkCore;", result);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    #endregion

    #region Strategy 3: LINQ query syntax

    [Fact]
    public void Apply_InjectsIncludeInLinqQuerySyntax()
    {
        var input = """
            using System.Linq;

            public class ShoppingCartActions
            {
                private readonly ProductContext _db;

                public decimal GetTotal()
                {
                    var total = (from cartItems in _db.ShoppingCartItems
                                 where cartItems.CartId == ShoppingCartId
                                 select cartItems.Product.UnitPrice).Sum();
                    return total;
                }
            }
            """;

        var result = _transform.Apply(input, MakeCodeFileMetadata());

        Assert.Contains("_db.ShoppingCartItems.Include(x => x.Product)", result);
        Assert.Contains("using Microsoft.EntityFrameworkCore;", result);
    }

    #endregion

    #region Strategy 4: Injected DbContext field

    [Fact]
    public void Apply_InjectsIncludeOnInjectedDbFieldBeforeWhere()
    {
        var input = """
            using System.Linq;

            public class ShoppingCartActions
            {
                private readonly ProductContext _db;

                public List<CartItem> GetCartItems()
                {
                    return _db.ShoppingCartItems.Where(
                        c => c.CartId == ShoppingCartId).ToList();
                }

                public void UpdateCart()
                {
                    foreach (var item in GetCartItems())
                    {
                        total += item.Product.UnitPrice;
                    }
                }
            }
            """;

        var result = _transform.Apply(input, MakeCodeFileMetadata());

        Assert.Contains("_db.ShoppingCartItems.Include(x => x.Product).Where(", result);
        Assert.Contains("using Microsoft.EntityFrameworkCore;", result);
    }

    [Fact]
    public void Apply_InjectsIncludeOnInjectedDbFieldBeforeDirectToList()
    {
        var input = """
            using System.Linq;

            public class ShoppingCartActions
            {
                private readonly ProductContext _db;

                public List<CartItem> GetCartItems()
                {
                    return _db.ShoppingCartItems.ToList();
                }

                public decimal GetTotal()
                {
                    decimal total = 0;
                    foreach (var item in GetCartItems())
                    {
                        total += item.Product.UnitPrice;
                    }

                    return total;
                }
            }
            """;

        var result = _transform.Apply(input, MakeCodeFileMetadata());

        Assert.Contains("return _db.ShoppingCartItems.Include(x => x.Product).ToList();", result);
        Assert.Contains("using Microsoft.EntityFrameworkCore;", result);
    }

    [Fact]
    public void Apply_HandlesMultipleQueriesInSameFile()
    {
        var input = """
            using System.Linq;

            public class ShoppingCartActions
            {
                private readonly ProductContext _db;

                public List<CartItem> GetCartItems()
                {
                    return _db.ShoppingCartItems.Where(
                        c => c.CartId == id).ToList();
                }

                public decimal GetTotal()
                {
                    var total = (from cartItems in _db.ShoppingCartItems
                                 where cartItems.CartId == id
                                 select cartItems.Product.UnitPrice).Sum();
                    return total;
                }
            }
            """;

        var result = _transform.Apply(input, MakeCodeFileMetadata());

        Assert.Contains("_db.ShoppingCartItems.Include(x => x.Product).Where(", result);
        Assert.Contains("using Microsoft.EntityFrameworkCore;", result);
    }

    [Fact]
    public void Apply_DoesNotDoubleInjectInclude()
    {
        var input = """
            using System.Linq;
            using Microsoft.EntityFrameworkCore;

            public class ShoppingCartActions
            {
                private readonly ProductContext _db;

                public List<CartItem> GetCartItems()
                {
                    return _db.ShoppingCartItems.Include(c => c.Product).Where(
                        c => c.CartId == id).ToList();
                }

                public void Check()
                {
                    foreach (var item in GetCartItems())
                    {
                        total += item.Product.UnitPrice;
                    }
                }
            }
            """;

        var result = _transform.Apply(input, MakeCodeFileMetadata());

        // Should not add a second .Include()
        var includeCount = result.Split(".Include(").Length - 1;
        Assert.Equal(1, includeCount);
    }

    #endregion

    #region Edge cases

    [Fact]
    public void Apply_NoChangeWhenNoNavProperties()
    {
        var input = """
            using System.Linq;

            public class ProductService
            {
                private readonly ProductContext _db;

                public List<Product> GetProducts()
                {
                    return _db.Products.Where(p => p.Active).ToList();
                }
            }
            """;

        var result = _transform.Apply(input, MakeCodeFileMetadata());

        Assert.Equal(input, result);
    }

    [Fact]
    public void Apply_DoesNotMatchNonDbContextFields()
    {
        var input = """
            using System.Linq;

            public class SomeService
            {
                private readonly ILogger _logger;

                public void Process()
                {
                    var items = _logger.Entries.Where(e => e.Level > 3).ToList();
                    foreach (var item in items)
                    {
                        item.Category.Name = "test";
                    }
                }
            }
            """;

        var result = _transform.Apply(input, MakeCodeFileMetadata());

        // _logger is not a DbContext field, so no Include should be injected
        Assert.DoesNotContain(".Include(", result);
    }

    #endregion
}
