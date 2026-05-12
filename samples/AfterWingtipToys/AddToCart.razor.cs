using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys;

public partial class AddToCart
{
    private const string CartSessionKey = "CartId";
    private WebFormsForm form1 = default!;

    [Inject] private IDbContextFactory<ProductContext> ProductContextFactory { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var rawId = Request.QueryString["ProductID"].ToString();
        if (string.IsNullOrWhiteSpace(rawId))
        {
            rawId = Request.QueryString["productID"].ToString();
        }
        if (!string.IsNullOrWhiteSpace(rawId) && int.TryParse(rawId, out var productId))
        {
            using var db = ProductContextFactory.CreateDbContext();
            var cartId = GetCartId();
            var cartItem = db.ShoppingCartItems.SingleOrDefault(c => c.CartId == cartId && c.ProductId == productId);
            if (cartItem == null)
            {
                var product = db.Products.SingleOrDefault(p => p.ProductID == productId)
                    ?? throw new InvalidOperationException($"Unable to locate product {productId}.");

                db.ShoppingCartItems.Add(new CartItem
                {
                    ItemId = Guid.NewGuid().ToString(),
                    ProductId = productId,
                    CartId = cartId,
                    Product = product,
                    Quantity = 1,
                    DateCreated = DateTime.Now
                });
            }
            else
            {
                cartItem.Quantity++;
            }

            db.SaveChanges();
        }
        else
        {
            Debug.Fail("ERROR : We should never get to AddToCart.aspx without a ProductId.");
            throw new Exception("ERROR : It is illegal to load AddToCart.aspx without setting a ProductId.");
        }

        Response.Redirect("ShoppingCart");
    }

    private string GetCartId()
    {
        var cartId = Session[CartSessionKey]?.ToString();
        if (!string.IsNullOrWhiteSpace(cartId))
        {
            return cartId;
        }

        cartId = Guid.NewGuid().ToString();
        Session[CartSessionKey] = cartId;
        return cartId;
    }
}
