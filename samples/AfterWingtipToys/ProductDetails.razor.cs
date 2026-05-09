using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ProductDetails
{
    [Inject]
    public ProductContext Db { get; set; } = default!;

    [Parameter, SupplyParameterFromQuery(Name = "ProductID")]
    public int? ProductId { get; set; }

    [Parameter, SupplyParameterFromQuery(Name = "productName")]
    public string? ProductName { get; set; }

    private Product? product;

    protected override async Task OnParametersSetAsync()
    {
        var query = Db.Products.AsQueryable();

        if (ProductId.HasValue && ProductId.Value > 0)
        {
            query = query.Where(item => item.ProductID == ProductId.Value);
        }
        else if (!string.IsNullOrWhiteSpace(ProductName))
        {
            query = query.Where(item => item.ProductName == ProductName);
        }
        else
        {
            product = null;
            return;
        }

        product = await query.FirstOrDefaultAsync();
    }
}
