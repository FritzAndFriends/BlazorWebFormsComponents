using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ProductList
{
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;

    [Parameter, SupplyParameterFromQuery(Name = "id")]
    public int? CategoryId { get; set; }

    [Parameter, SupplyParameterFromQuery(Name = "categoryName")]
    public string? CategoryName { get; set; }

    private IReadOnlyList<Product> Products { get; set; } = [];

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        Title = "Products";

        await using var db = await DbFactory.CreateDbContextAsync();
        var query = db.Products.Include(product => product.Category).AsQueryable();

        if (CategoryId is > 0)
        {
            query = query.Where(product => product.CategoryID == CategoryId);
        }

        if (!string.IsNullOrWhiteSpace(CategoryName))
        {
            query = query.Where(product => product.Category != null && product.Category.CategoryName == CategoryName);
        }

        Products = await query.OrderBy(product => product.ProductName).ToListAsync();
    }
}
