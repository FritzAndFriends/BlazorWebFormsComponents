using BlazorWebFormsComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ProductList
{
    [Inject] private ProductContext Db { get; set; } = default!;

    [Parameter, SupplyParameterFromQuery(Name = "id")]
    public int? CategoryId { get; set; }

    [Parameter]
    public string? CategoryName { get; set; }

    private ListView<Product> productList = default!;

    private IQueryable<Product> GetProductsQueryDetails_SelectMethod(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
    {
        var query = Db.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .AsQueryable();

        if (CategoryId.HasValue && CategoryId > 0)
        {
            query = query.Where(p => p.CategoryID == CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(CategoryName))
        {
            query = query.Where(p => p.Category != null && p.Category.CategoryName == CategoryName);
        }

        totalRowCount = query.Count();
        return query;
    }
}
