using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ShoppingCart
{
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;
    [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private List<CartItem> _cartItems = new();
    private string _cartTotal = "";
    private string _totalLabelText = "Order Total: ";
    private string _cartTitle = "Shopping Cart";
    private bool _showButtons = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadCart();
    }

    private async Task LoadCart()
    {
        using var db = DbFactory.CreateDbContext();
        var cartId = GetCartId();
        _cartItems = await db.ShoppingCartItems
            .Where(c => c.CartId == cartId)
            .Include(c => c.Product)
            .ToListAsync();

        var cartTotal = _cartItems.Sum(c => c.Quantity * (c.Product?.UnitPrice ?? 0));
        if (cartTotal > 0)
        {
            _cartTotal = string.Format("{0:c}", cartTotal);
        }
        else
        {
            _totalLabelText = "";
            _cartTotal = "";
            _cartTitle = "Shopping Cart is Empty";
            _showButtons = false;
        }
    }

    private IQueryable<CartItem> GetShoppingCartItems(
        int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
    {
        var db = DbFactory.CreateDbContext();
        var cartId = GetCartId();
        var query = db.ShoppingCartItems
            .Where(c => c.CartId == cartId)
            .Include(c => c.Product);
        totalRowCount = query.Count();
        var results = query.ToList();
        db.Dispose();
        return results.AsQueryable();
    }

    private string GetCartId()
    {
        var httpContext = HttpContextAccessor.HttpContext;
        if (httpContext?.Request.Cookies.TryGetValue("CartId", out var cartId) == true && !string.IsNullOrEmpty(cartId))
        {
            return cartId;
        }

        var newCartId = Guid.NewGuid().ToString();
        httpContext?.Response.Cookies.Append("CartId", newCartId, new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) });
        return newCartId;
    }

    private async Task UpdateBtn_Click(EventArgs e)
    {
        // TODO: Implement cart update logic — read quantities from form, update DB
        await LoadCart();
    }

    private void CheckoutBtn_Click(EventArgs e)
    {
        // TODO: Implement PayPal checkout start — store payment amount and redirect
        NavigationManager.NavigateTo("/CheckoutStart");
    }
}
