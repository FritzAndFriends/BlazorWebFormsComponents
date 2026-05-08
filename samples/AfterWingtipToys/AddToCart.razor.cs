using System.Diagnostics;
using WingtipToys.Logic;

namespace WingtipToys;

public partial class AddToCart
{
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        string rawId = Request.QueryString["ProductID"];
        if (!string.IsNullOrEmpty(rawId) && int.TryParse(rawId, out var productId))
        {
            using var usersShoppingCart = new ShoppingCartActions();
            usersShoppingCart.AddToCart(productId);
        }
        else
        {
            Debug.Fail("ERROR : We should never get to AddToCart without a ProductId.");
            throw new Exception("ERROR : It is illegal to load AddToCart without setting a ProductId.");
        }

        Response.Redirect("/ShoppingCart");
    }
}
