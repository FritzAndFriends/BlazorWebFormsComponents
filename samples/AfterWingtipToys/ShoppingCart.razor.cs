using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using WingtipToys.Models;
using WingtipToys.Services;

namespace WingtipToys;

public partial class ShoppingCart : ComponentBase
{
    [Inject] private ShoppingCartService CartService { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;

    private List<CartItem> _cartItems = new();
    private decimal _total;
    private string _title = "Shopping Cart";
    private Dictionary<int, bool> _removeFlags = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadCart();
    }

    private async Task LoadCart()
    {
        _cartItems = await CartService.GetCartItemsAsync();
        _total = await CartService.GetTotalAsync();
        _removeFlags = _cartItems.ToDictionary(c => c.ProductId, _ => false);
        _title = _cartItems.Any() ? "Shopping Cart" : "Shopping Cart is Empty";
    }

    private void OnQuantityChanged(CartItem item, ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var qty))
        {
            item.Quantity = qty;
        }
    }

    private void OnRemoveChanged(CartItem item, ChangeEventArgs e)
    {
        if (e.Value is bool val)
        {
            _removeFlags[item.ProductId] = val;
        }
    }

    private async Task UpdateBtn_Click(EventArgs e)
    {
        foreach (var item in _cartItems)
        {
            if (_removeFlags.GetValueOrDefault(item.ProductId))
            {
                await CartService.RemoveCartItemAsync(item.ProductId);
            }
            else
            {
                await CartService.UpdateCartItemAsync(item.ProductId, item.Quantity);
            }
        }
        await LoadCart();
    }

    private async Task CheckoutBtn_Click(EventArgs e)
    {
        Nav.NavigateTo("/Checkout/CheckoutStart");
    }
}

