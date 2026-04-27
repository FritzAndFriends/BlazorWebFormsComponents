using Microsoft.AspNetCore.Components;
using WingtipToys.Models;
using WingtipToys.Services;

namespace WingtipToys;

public partial class ShoppingCart
{
    [Inject] private CartService Cart { get; set; } = default!;

    private IReadOnlyList<CartItem> Items { get; set; } = Array.Empty<CartItem>();
    private decimal Total { get; set; }
    private string Heading { get; set; } = "Shopping Cart";

    protected override void OnParametersSet()
    {
        if (int.TryParse(Request.QueryString["removeProductId"], out var removeProductId))
        {
            Cart.RemoveItem(removeProductId);
        }

        if (string.Equals(Request.QueryString["action"], "update", StringComparison.OrdinalIgnoreCase) &&
            int.TryParse(Request.QueryString["productId"], out var productId) &&
            int.TryParse(Request.QueryString["quantity"], out var quantity))
        {
            Cart.UpdateQuantity(productId, quantity);
        }

        Items = Cart.GetItems();
        Total = Cart.GetTotal();
        Heading = Items.Count == 0 ? "Shopping Cart is Empty" : "Shopping Cart";
    }
}
