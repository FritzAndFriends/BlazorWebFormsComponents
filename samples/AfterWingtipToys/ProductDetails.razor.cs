using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ProductDetails
{
    [Inject] private ProductContext Db { get; set; } = default!;

    private FormView<Product> productDetail = default!;

    private IQueryable<Product> GetProduct(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
    {
        var query = Db.Products.AsNoTracking().AsQueryable();

        var productIdText = Request.QueryString["ProductID"];
        string? productName = Request.QueryString["productName"];

        if (int.TryParse(productIdText, out var productId) && productId > 0)
        {
            query = query.Where(p => p.ProductID == productId);
        }
        else if (!string.IsNullOrWhiteSpace(productName))
        {
            query = query.Where(p => p.ProductName == productName);
        }
        else
        {
            query = Enumerable.Empty<Product>().AsQueryable();
        }

        totalRowCount = query.Count();
        return query;
    }
}
