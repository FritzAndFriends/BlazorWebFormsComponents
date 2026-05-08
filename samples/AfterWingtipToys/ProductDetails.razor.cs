using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ProductDetails
{
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;

    [Parameter, SupplyParameterFromQuery(Name = "productId")]
    public int? ProductId { get; set; }

    [Parameter, SupplyParameterFromQuery(Name = "ProductID")]
    public int? ProductIdLegacy { get; set; }

    [Parameter, SupplyParameterFromQuery(Name = "productName")]
    public string? ProductName { get; set; }

    private Product? Product { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        Title = "Product Details";

        var requestedId = ProductId ?? ProductIdLegacy;

        await using var db = await DbFactory.CreateDbContextAsync();
        if (requestedId is > 0)
        {
            Product = await db.Products.Include(product => product.Category)
                .FirstOrDefaultAsync(product => product.ProductID == requestedId.Value);
        }
        else if (!string.IsNullOrWhiteSpace(ProductName))
        {
            Product = await db.Products.Include(product => product.Category)
                .FirstOrDefaultAsync(product => product.ProductName == ProductName);
        }
        else
        {
            Product = await db.Products.Include(product => product.Category)
                .OrderBy(product => product.ProductID)
                .FirstOrDefaultAsync();
        }
    }
}
