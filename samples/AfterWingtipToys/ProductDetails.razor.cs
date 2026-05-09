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

    [Parameter]
    public string? ProductName { get; set; }

    protected IReadOnlyList<Product> Products { get; set; } = Array.Empty<Product>();

    protected override async Task OnParametersSetAsync()
    {
        IQueryable<Product> query = Db.Products.AsNoTracking();

        if (ProductId is > 0)
        {
            query = query.Where(product => product.ProductID == ProductId.Value);
        }
        else if (!string.IsNullOrWhiteSpace(ProductName))
        {
            query = query.Where(product => product.ProductName == ProductName);
        }
        else
        {
            Products = Array.Empty<Product>();
            return;
        }

        Products = await query.ToListAsync();
    }
}
