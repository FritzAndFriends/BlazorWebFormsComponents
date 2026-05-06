using BlazorWebFormsComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Logic;
using WingtipToys.Models;

namespace WingtipToys
{
    public partial class AddToCart : WebFormsPageBase
    {
        [Parameter, SupplyParameterFromQuery(Name = "productID")]
        public int? ProductId { get; set; }

        [Inject] private CartService Cart { get; set; } = default!;
        [Inject] private ProductContext Db { get; set; } = default!;

        // Add the item and redirect only after the interactive circuit connects —
        // avoids SSR pre-render sending a 302 before CartService is circuit-scoped.
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            if (ProductId is int productId && productId > 0)
            {
                var product = await Db.Products.FirstOrDefaultAsync(p => p.ProductID == productId);
                if (product != null)
                    Cart.AddToCart(product);
            }

            Response.Redirect("/ShoppingCart");
        }
    }
}
