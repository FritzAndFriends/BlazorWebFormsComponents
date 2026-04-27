using Microsoft.AspNetCore.Components;
using WingtipToys.Services;

namespace WingtipToys;

public partial class AddToCart
{
    [Inject] private CartService Cart { get; set; } = default!;
    private bool _handled;

    protected override void OnParametersSet()
    {
        if (_handled)
        {
            return;
        }

        _handled = true;

        if (int.TryParse(Request.QueryString["productID"], out var productId))
        {
            Cart.AddItem(productId);
        }

        Response.Redirect("/ShoppingCart");
    }
}
