using BlazorWebFormsComponents.Cli.Pipeline;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Cli.Tests.Pipeline;

public class CodeBehindInjectorTests
{
    [Fact]
    public void DeduplicateParameters_DuplicateOnSameLine_DoesNotEatFollowingMethod()
    {
        // Simulates the exact ProductList scenario:
        // - Code-behind already has [Parameter] public string? categoryName
        // - Injected members include duplicate CategoryName + a wrapper method
        var existingCode = @"
namespace WingtipToys
{
  public partial class ProductList : WebFormsPageBase
  {
    [Parameter] public string? categoryName { get; set; }

    public IQueryable<Product> GetProducts(int? categoryId, string categoryName)
    {
      return query;
    }
  }
}";

        var members = @"    [Parameter, SupplyParameterFromQuery(Name = ""id"")] public int? CategoryId { get; set; }
    [Parameter] public string? CategoryName { get; set; }

    private global::System.Linq.IQueryable<Product> GetProductsQueryDetails_SelectMethod(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
    {
        totalRowCount = 0;
        var query = GetProducts(CategoryId, CategoryName);
        if (query != null) totalRowCount = query.Count();
        return query ?? global::System.Linq.Enumerable.Empty<Product>().AsQueryable();
    }";

        var result = CodeBehindInjector.DeduplicateParameters(existingCode, members);

        // The duplicate CategoryName parameter should be removed
        result.ShouldNotContain("[Parameter] public string? CategoryName");

        // The non-duplicate CategoryId parameter should be kept
        result.ShouldContain("CategoryId");

        // CRITICAL: The method declaration MUST survive deduplication
        result.ShouldContain("GetProductsQueryDetails_SelectMethod");
        result.ShouldContain("totalRowCount = 0;");
        result.ShouldContain("return query");
    }

    [Fact]
    public void DeduplicateParameters_MultiLineAttribute_SkipsCorrectly()
    {
        var existingCode = @"[Parameter] public string? Name { get; set; }";

        var members = @"    [Parameter]
    public string? Name { get; set; }
    public void SomeMethod() { }";

        var result = CodeBehindInjector.DeduplicateParameters(existingCode, members);

        // Duplicate should be removed (both attribute and property lines)
        result.ShouldNotContain("public string? Name");

        // Following method should survive
        result.ShouldContain("SomeMethod");
    }

    [Fact]
    public void InjectMembers_BlockScopedNamespace_InsertsBeforeClassBrace()
    {
        var codeBehind = @"namespace WingtipToys
{
  public partial class MyPage
  {
    public void Existing() { }
  }
}";

        var members = "    public string NewProp { get; set; }";

        var result = CodeBehindInjector.InjectMembers(codeBehind, members);

        result.ShouldContain("NewProp");
        // New member should appear inside the class body (before final braces)
        var newPropIndex = result.IndexOf("NewProp");
        var lastBrace = result.LastIndexOf('}');
        newPropIndex.ShouldBeLessThan(lastBrace);
    }
}
