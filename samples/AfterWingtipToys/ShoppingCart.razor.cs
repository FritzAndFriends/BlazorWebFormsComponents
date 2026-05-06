using System.Collections.Generic;
using System.Linq;
using BlazorWebFormsComponents;
using Microsoft.AspNetCore.Components;
using WingtipToys.Logic;
using WingtipToys.Models;

namespace WingtipToys
{
    public partial class ShoppingCart : WebFormsPageBase
    {
        [Inject] private CartService Cart { get; set; } = default!;

        private List<CartEntry> _cartItems = new();
        private Dictionary<int, int> _quantities = new();
        private decimal _total;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            RefreshCart();
        }

        private void RefreshCart()
        {
            _cartItems = Cart.GetItems().ToList();
            _quantities = _cartItems.ToDictionary(e => e.Product.ProductID, e => e.Quantity);
            _total = Cart.GetTotal();
        }

        private void UpdateQty(int productId, ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out int qty) && qty >= 0)
                _quantities[productId] = qty;
        }

        private void UpdateCart()
        {
            foreach (var (productId, qty) in _quantities)
                Cart.UpdateQuantity(productId, qty);
            RefreshCart();
        }

        private void RemoveItem(int productId)
        {
            Cart.RemoveItem(productId);
            RefreshCart();
        }

        private void Checkout() => Response.Redirect("/Checkout/CheckoutStart");
    }
}
