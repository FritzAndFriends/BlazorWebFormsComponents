using WingtipToys.Logic;
using WingtipToys.Models;

namespace WingtipToys.Checkout;

public partial class CheckoutReview
{
    private List<CartItem> OrderItems { get; set; } = [];

    private string ShippingSummary { get; set; } = "Wingtip Toys Warehouse, Redmond, WA";

    private string TotalText { get; set; } = "$0.00";

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        using var cart = new ShoppingCartActions();
        OrderItems = cart.GetCartItems();
        TotalText = string.Format("{0:c}", cart.GetTotal());
    }

    private void CheckoutConfirm_Click()
    {
        using var cart = new ShoppingCartActions();
        cart.EmptyCart();
        Response.Redirect("/Checkout/CheckoutComplete");
    }
}
