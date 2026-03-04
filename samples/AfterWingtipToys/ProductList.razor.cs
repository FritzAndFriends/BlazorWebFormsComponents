using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ProductList : ComponentBase
{
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "id")] private int? CategoryId { get; set; }

    private List<Product>? _products;

    protected override async Task OnInitializedAsync()
    {
        using var db = DbFactory.CreateDbContext();
        IQueryable<Product> query = db.Products;

        if (CategoryId.HasValue && CategoryId > 0)
        {
            query = query.Where(p => p.CategoryID == CategoryId);
        }

        _products = await query.ToListAsync();
    }
}

