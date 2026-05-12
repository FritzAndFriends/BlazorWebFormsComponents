using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ProductDetails : BlazorWebFormsComponents.WebFormsPageBase
{
    [Inject] public ProductContext Db { get; set; } = default!;
    [Parameter, SupplyParameterFromQuery(Name = "productID")] public int? ProductId { get; set; }
    [Parameter] public string? productName { get; set; }

    private List<Product> products = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        IQueryable<Product> query = Db.Products.Include(p => p.Category);
        if (ProductId.HasValue && ProductId > 0)
        {
            query = query.Where(p => p.ProductID == ProductId.Value);
        }
        else if (!string.IsNullOrEmpty(productName))
        {
            query = query.Where(p => p.ProductName == productName);
        }
        else
        {
            query = query.Where(_ => false);
        }

        products = await query.ToListAsync();
    }
}