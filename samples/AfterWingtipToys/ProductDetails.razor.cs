using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ProductDetails
{
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "ProductID")]
    public int? ProductId { get; set; }

    [SupplyParameterFromQuery(Name = "productName")]
    public string? ProductName { get; set; }

    private IQueryable<Product> GetProduct(
        int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
    {
        var db = DbFactory.CreateDbContext();
        IQueryable<Product> query = db.Products;

        if (ProductId.HasValue && ProductId > 0)
        {
            query = query.Where(p => p.ProductID == ProductId);
        }
        else if (!string.IsNullOrEmpty(ProductName))
        {
            query = query.Where(p =>
                string.Compare(p.ProductName, ProductName) == 0);
        }
        else
        {
            totalRowCount = 0;
            db.Dispose();
            return Enumerable.Empty<Product>().AsQueryable();
        }

        totalRowCount = query.Count();
        var results = query.ToList();
        db.Dispose();
        return results.AsQueryable();
    }

    protected override async Task OnInitializedAsync()
    {
        Page.Title = "Product Details";
        await Task.CompletedTask;
    }
}
