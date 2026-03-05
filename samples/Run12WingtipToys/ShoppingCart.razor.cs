using Microsoft.AspNetCore.Components;
using WingtipToys.Models;
using WingtipToys.Services;

namespace WingtipToys;

public partial class ShoppingCart
{
    [Inject] private CartStateService Cart { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;

    private List<CartItem> _cartItems = new();
    private string _cartTotal = "$0.00";
    private Dictionary<string, int> _quantities = new();
    private Dictionary<string, bool> _removals = new();

    protected override void OnInitialized()
    {
        LoadCart();
    }

    private void LoadCart()
    {
        _cartItems = Cart.Items.ToList();
        _cartTotal = Cart.GetTotal().ToString("c");

        _quantities.Clear();
        _removals.Clear();
        foreach (var item in _cartItems)
        {
            _quantities[item.ItemId] = item.Quantity;
            _removals[item.ItemId] = false;
        }
    }

    private string GetQuantity(string itemId)
        => _quantities.TryGetValue(itemId, out var qty) ? qty.ToString() : "0";

    private void OnQuantityChanged(string itemId, string value)
    {
        if (int.TryParse(value, out var qty) && qty > 0)
            _quantities[itemId] = qty;
    }

    private bool IsMarkedForRemoval(string itemId)
        => _removals.TryGetValue(itemId, out var marked) && marked;

    private void OnRemovalChanged(string itemId, bool value)
    {
        _removals[itemId] = value;
    }

    private void OnUpdateCart(EventArgs args)
    {
        foreach (var kvp in _removals.Where(r => r.Value))
        {
            Cart.RemoveItem(kvp.Key);
        }

        foreach (var kvp in _quantities)
        {
            if (!_removals.GetValueOrDefault(kvp.Key))
            {
                Cart.UpdateQuantity(kvp.Key, kvp.Value);
            }
        }

        LoadCart();
    }

    private void OnCheckout(EventArgs args)
    {
        Nav.NavigateTo("/CheckoutReview");
    }
}
