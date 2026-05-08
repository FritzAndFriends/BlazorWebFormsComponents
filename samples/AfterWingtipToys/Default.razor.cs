using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys;

public partial class Default
{
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;

    private IReadOnlyList<Product> FeaturedProducts { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Title = "Wingtip Toys";

        await using var db = await DbFactory.CreateDbContextAsync();
        FeaturedProducts = await db.Products.OrderBy(product => product.ProductID).Take(4).ToListAsync();
    }
}
