using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ShoppingCart
{
    private const string CartSessionKey = "CartId";

    [Inject] private IDbContextFactory<ProductContext> ProductContextFactory { get; set; } = default!;

    protected string ShoppingCartTitleText { get; set; } = "Shopping Cart";
    protected string OrderTotalLabelText { get; set; } = "Order Total: ";
    protected string TotalText { get; set; } = string.Empty;
    protected bool ShowActions { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        LoadCartSummary();
    }

    public List<CartItem> GetShoppingCartItems()
    {
        using var db = ProductContextFactory.CreateDbContext();
        return db.ShoppingCartItems
            .Include(c => c.Product)
            .AsNoTracking()
            .Where(c => c.CartId == GetCartId())
            .ToList();
    }

    protected void UpdateBtn_Click()
    {
        LoadCartSummary();
    }

    protected void CheckoutBtn_Click()
    {
        Session["payment_amt"] = GetTotal();
        Response.Redirect("Checkout/CheckoutStart.aspx");
    }

    private void LoadCartSummary()
    {
        var cartTotal = GetTotal();
        if (cartTotal > 0)
        {
            TotalText = string.Format("{0:c}", cartTotal);
            return;
        }

        OrderTotalLabelText = string.Empty;
        TotalText = string.Empty;
        ShoppingCartTitleText = "Shopping Cart is Empty";
        ShowActions = false;
    }

    private decimal GetTotal()
    {
        return GetShoppingCartItems()
            .Sum(item => (decimal)(item.Product?.UnitPrice ?? 0d) * item.Quantity);
    }

    private string GetCartId()
    {
        var existingCartId = Session[CartSessionKey]?.ToString();
        if (!string.IsNullOrWhiteSpace(existingCartId))
        {
            return existingCartId;
        }

        var cartId = Guid.NewGuid().ToString();
        Session[CartSessionKey] = cartId;
        return cartId;
    }
}
