using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys;

public partial class AddToCart : BlazorWebFormsComponents.WebFormsPageBase
{
    [Inject] public ProductContext Db { get; set; } = default!;
    [Parameter, SupplyParameterFromQuery(Name = "productID")] public int ProductId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var cartId = Session["CartId"]?.ToString();
        if (string.IsNullOrEmpty(cartId))
        {
            cartId = Guid.NewGuid().ToString();
            Session["CartId"] = cartId;
        }

        var product = await Db.Products.FindAsync(ProductId);
        if (product != null)
        {
            var existingItem = await Db.ShoppingCartItems
                .FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductId == ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                Db.ShoppingCartItems.Add(new CartItem
                {
                    ItemId = Guid.NewGuid().ToString(),
                    CartId = cartId,
                    ProductId = ProductId,
                    Product = product,
                    Quantity = 1,
                    DateCreated = DateTime.Now
                });
            }

            await Db.SaveChangesAsync();
        }

        Response.Redirect("/ShoppingCart");
    }
}