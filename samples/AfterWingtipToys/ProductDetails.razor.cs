using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ProductDetails
{
    [Inject] public ProductContext Db { get; set; } = default!;

    [Parameter, SupplyParameterFromQuery(Name = "productId")]
    public int? ProductId { get; set; }

    [Parameter, SupplyParameterFromQuery(Name = "ProductID")]
    public int? LegacyProductId { get; set; }

    [Parameter, SupplyParameterFromQuery(Name = "productName")]
    public string? ProductName { get; set; }

    protected List<Product> Products { get; private set; } = new();

    protected override async Task OnParametersSetAsync()
    {
        IQueryable<Product> query = Db.Products.AsNoTracking();
        var selectedProductId = ProductId ?? LegacyProductId;

        if (selectedProductId.HasValue && selectedProductId > 0)
        {
            query = query.Where(product => product.ProductID == selectedProductId.Value);
        }
        else if (!string.IsNullOrWhiteSpace(ProductName))
        {
            query = query.Where(product => product.ProductName == ProductName);
        }
        else
        {
            Products = new();
            return;
        }

        Products = await query.ToListAsync();
        Title = "Product Details";
    }
}
