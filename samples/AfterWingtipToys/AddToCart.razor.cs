using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys
{
    public partial class AddToCart
    {
        [Inject] public ProductContext Db { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            var rawId = Request.QueryString["ProductID"].ToString();
            if (string.IsNullOrEmpty(rawId))
            {
                rawId = Request.QueryString["productID"].ToString();
            }

            if (!string.IsNullOrEmpty(rawId) && int.TryParse(rawId, out var productId))
            {
                var cartId = GetCartId();
                var cartItem = await Db.ShoppingCartItems
                    .SingleOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);
                if (cartItem == null)
                {
                    cartItem = new CartItem
                    {
                        ItemId = Guid.NewGuid().ToString(),
                        ProductId = productId,
                        CartId = cartId,
                        Product = await Db.Products.SingleOrDefaultAsync(p => p.ProductID == productId),
                        Quantity = 1,
                        DateCreated = DateTime.UtcNow
                    };
                    Db.ShoppingCartItems.Add(cartItem);
                }
                else
                {
                    cartItem.Quantity++;
                }

                await Db.SaveChangesAsync();
            }
            else
            {
                Debug.Fail("ERROR : We should never get to AddToCart without a ProductId.");
                throw new Exception("ERROR : It is illegal to load AddToCart without setting a ProductId.");
            }

            Response.Redirect("ShoppingCart");
        }

        private string GetCartId()
        {
            const string cartSessionKey = "CartId";
            var existingCartId = Session[cartSessionKey]?.ToString();
            if (!string.IsNullOrWhiteSpace(existingCartId))
            {
                return existingCartId;
            }

            var newCartId = Guid.NewGuid().ToString();
            Session[cartSessionKey] = newCartId;
            return newCartId;
        }
    }
}
