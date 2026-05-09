using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ShoppingCart
{
    [Inject]
    public ProductContext Db { get; set; } = default!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    public IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    private List<CartItem> cartItems = new();
    private decimal cartTotal;
    private string shoppingCartTitle = "Shopping Cart";
    private string? _cartId;

    protected override async Task OnInitializedAsync()
    {
        _cartId = GetCartId();
        await LoadCartAsync();
    }

    private async Task LoadCartAsync()
    {
        var cartId = _cartId ?? GetCartId();
        cartItems = await Db.ShoppingCartItems
            .Include(item => item.Product)
            .Where(item => item.CartId == cartId)
            .OrderBy(item => item.DateCreated)
            .ToListAsync();

        cartTotal = cartItems.Sum(item => (decimal)(item.Product?.UnitPrice ?? 0) * item.Quantity);
        shoppingCartTitle = cartItems.Count == 0 ? "Shopping Cart is Empty" : "Shopping Cart";
    }

    private string GetCartId()
    {
        var session = HttpContextAccessor.HttpContext?.Session;
        if (session is null) return _cartId ?? Guid.NewGuid().ToString();

        var cartId = session.GetString("CartId");
        if (string.IsNullOrEmpty(cartId))
        {
            cartId = Guid.NewGuid().ToString();
            session.SetString("CartId", cartId);
        }
        return cartId;
    }

    private async Task UpdateBtn_Click()
    {
        await Db.SaveChangesAsync();
        await LoadCartAsync();
    }

    private void CheckoutBtn_Click()
    {
        NavigationManager.NavigateTo("/Checkout/CheckoutStart");
    }
}