using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ProductDetails
{
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;
    [SupplyParameterFromQuery(Name = "productID")] private int? ProductId { get; set; }
    private Product? _product;
    private List<Product>? _products;

    protected override async Task OnParametersSetAsync()
    {
        using var db = DbFactory.CreateDbContext();
        if (ProductId.HasValue && ProductId > 0)
        {
            _product = await db.Products.FirstOrDefaultAsync(p => p.ProductID == ProductId);
        }
        _products = _product != null ? new List<Product> { _product } : new();
    }
}
