using WingtipToys.Logic;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ShoppingCart
{
    private List<CartItem> CartList { get; set; } = [];

    private string ShoppingCartTitle { get; set; } = "Shopping Cart";

    private string TotalText { get; set; } = "$0.00";

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        LoadCart();
    }

    private void LoadCart()
    {
        using var usersShoppingCart = new ShoppingCartActions();
        CartList = usersShoppingCart.GetCartItems();
        var cartTotal = usersShoppingCart.GetTotal();
        TotalText = string.Format("{0:c}", cartTotal);
        ShoppingCartTitle = CartList.Count > 0 ? "Shopping Cart" : "Shopping Cart is Empty";
    }

    private void UpdateBtn_Click()
    {
        LoadCart();
    }

    private void CheckoutBtn_Click()
    {
        using var usersShoppingCart = new ShoppingCartActions();
        Session["payment_amt"] = usersShoppingCart.GetTotal().ToString();
        Response.Redirect("/Checkout/CheckoutStart");
    }
}
