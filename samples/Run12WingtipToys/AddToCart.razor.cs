using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;
using WingtipToys.Services;

namespace WingtipToys;

public partial class AddToCart
{
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;
    [Inject] private CartStateService Cart { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [SupplyParameterFromQuery(Name = "productID")] private int? ProductId { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (ProductId.HasValue)
        {
            using var db = DbFactory.CreateDbContext();
            var product = await db.Products.FirstOrDefaultAsync(p => p.ProductID == ProductId);
            if (product != null)
            {
                Cart.AddItem(product);
            }
        }
        Nav.NavigateTo("/ShoppingCart");
    }
}
