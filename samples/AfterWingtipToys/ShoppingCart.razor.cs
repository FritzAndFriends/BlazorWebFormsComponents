using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ShoppingCart : BlazorWebFormsComponents.WebFormsPageBase
{
    [Inject] public ProductContext Db { get; set; } = default!;

    private List<CartItem> cartItems = new();
    private double cartTotal;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var cartId = Session["CartId"]?.ToString();
        if (!string.IsNullOrEmpty(cartId))
        {
            cartItems = await Db.ShoppingCartItems
                .Include(c => c.Product)
                .Where(c => c.CartId == cartId)
                .ToListAsync();
            cartTotal = cartItems.Sum(c => (c.Product?.UnitPrice ?? 0d) * c.Quantity);
        }
    }
}