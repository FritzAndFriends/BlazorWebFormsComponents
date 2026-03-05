using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Services;

namespace WingtipToys
{
    public partial class AddToCart : ComponentBase
    {
        [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;
        [Inject] private CartStateService CartService { get; set; } = default!;

        [SupplyParameterFromQuery(Name = "productID")]
        private int? ProductId { get; set; }

        private bool _added;
        private bool _notFound;

        protected override async Task OnInitializedAsync()
        {
            if (ProductId.HasValue)
            {
                using var db = DbFactory.CreateDbContext();
                var product = await db.Products.FindAsync(ProductId.Value);
                if (product != null)
                {
                    await CartService.AddToCartAsync(product.ProductID);
                    _added = true;
                }
                else
                {
                    _notFound = true;
                }
            }
        }
    }
}
