using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ProductDetails : ComponentBase
{
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "productID")]
    private int? ProductId { get; set; }

    private Product? _product;

    protected override async Task OnParametersSetAsync()
    {
        if (ProductId.HasValue && ProductId > 0)
        {
            using var db = await DbFactory.CreateDbContextAsync();
            _product = await db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductID == ProductId);
        }
    }
}

