using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ProductDetails
{
    private FormView<Product> productDetail = default!;

    [Parameter] public string? productName { get; set; }
    [Parameter, SupplyParameterFromQuery(Name = "ProductID")] public int? ProductId { get; set; }
    [Inject] private IDbContextFactory<ProductContext> ProductContextFactory { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    public IQueryable<Product> GetProduct()
    {
        using var db = ProductContextFactory.CreateDbContext();

        var query = db.Products.AsNoTracking().AsQueryable();
        if (ProductId.HasValue && ProductId > 0)
        {
            query = query.Where(p => p.ProductID == ProductId);
        }
        else if (!string.IsNullOrEmpty(productName))
        {
            query = query.Where(p => string.Compare(p.ProductName, productName) == 0);
        }
        else
        {
            return Array.Empty<Product>().AsQueryable();
        }

        return query.ToList().AsQueryable();
    }
}
