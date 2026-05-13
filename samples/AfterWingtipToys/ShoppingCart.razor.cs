using Microsoft.AspNetCore.Components;
using WingtipToys.Logic;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ShoppingCart
{
    [Inject] public ShoppingCartActions CartActions { get; set; } = default!;

    private List<CartItem> CartItems { get; set; } = new();
    private string TotalText { get; set; } = string.Empty;
    private string ShoppingCartTitleText { get; set; } = "Shopping Cart";
    private bool HasItems => CartItems.Count > 0;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        LoadCart();
    }

    public List<CartItem> GetShoppingCartItems()
    {
        LoadCart();
        return CartItems;
    }

    private void LoadCart()
    {
        CartItems = CartActions.GetCartItems();
        TotalText = CartActions.GetTotal().ToString("c");

        if (!HasItems)
        {
            TotalText = string.Empty;
            ShoppingCartTitleText = "Shopping Cart is Empty";
        }
    }

    private string GetItemTotal(CartItem item)
    {
        return string.Format("{0:c}", item.Product.UnitPrice * item.Quantity);
    }

    private void UpdateBtn_Click(EventArgs _)
    {
        LoadCart();
    }

    private void CheckoutBtn_Click(EventArgs _)
    {
        Session["payment_amt"] = CartActions.GetTotal();
        Response.Redirect("Checkout/CheckoutStart.aspx");
    }
}
