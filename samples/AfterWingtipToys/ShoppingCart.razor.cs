using System;
using System.Collections.Generic;
using System.Linq;
using WingtipToys.Models;
using WingtipToys.Logic;
using Microsoft.AspNetCore.Components;

namespace WingtipToys
{
  public partial class ShoppingCart
  {
    [Inject] protected ShoppingCartActions _cartActions { get; set; } = default!;

    private GridView<CartItem> CartList = default!;
    private ImageButton CheckoutImageBtn = default!;
    private TextBox PurchaseQuantity = default!;
    private CheckBox Remove = default!;
    private Button UpdateBtn = default!;

    private string _labelTotalText = "Order Total: ";
    private string _totalText = "";
    private bool _showButtons = true;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var cartTotal = _cartActions.GetTotal();
        if (cartTotal > 0)
        {
            _totalText = String.Format("{0:c}", cartTotal);
        }
        else
        {
            _labelTotalText = "";
            _totalText = "";
            _showButtons = false;
        }
    }

    public List<CartItem> GetShoppingCartItems()
    {
        return _cartActions.GetCartItems();
    }

    public List<CartItem> UpdateCartItems()
    {
        var cartId = _cartActions.GetCartId();
        var items = _cartActions.GetCartItems();

        var cartUpdates = new ShoppingCartActions.ShoppingCartUpdates[items.Count];
        for (var i = 0; i < items.Count; i++)
        {
            cartUpdates[i].ProductId = items[i].ProductId;
            cartUpdates[i].PurchaseQuantity = items[i].Quantity;
            cartUpdates[i].RemoveItem = false;
        }

        _cartActions.UpdateShoppingCartDatabase(cartId, cartUpdates);
        _totalText = String.Format("{0:c}", _cartActions.GetTotal());
        return _cartActions.GetCartItems();
    }

    protected void UpdateBtn_Click()
    {
        UpdateCartItems();
    }

    protected void CheckoutBtn_Click()
    {
        Session["payment_amt"] = _cartActions.GetTotal();
        Response.Redirect("Checkout/CheckoutStart.aspx");
    }
  }
}