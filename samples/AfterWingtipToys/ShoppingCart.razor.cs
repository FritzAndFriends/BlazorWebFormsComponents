using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys
{
  public partial class ShoppingCart
  {
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    private List<CartItem>? _cartItems;
    private decimal _cartTotal;
    private string _totalText = "Order Total: ";
    private string _totalDisplay = "";
    private string _cartTitle = "Shopping Cart";
    private bool _showButtons = true;

    protected override async Task OnInitializedAsync()
    {
      Page.Title = "Shopping Cart";
      var cartId = GetCartId();
      using var db = DbFactory.CreateDbContext();

      _cartItems = await db.ShoppingCartItems
          .Where(c => c.CartId == cartId)
          .Include(c => c.Product)
          .ToListAsync();

      _cartTotal = (decimal)_cartItems.Sum(c => c.Quantity * (c.Product?.UnitPrice ?? 0));

      if (_cartTotal > 0)
      {
        _totalDisplay = string.Format("{0:c}", _cartTotal);
      }
      else
      {
        _totalText = "";
        _totalDisplay = "";
        _cartTitle = "Shopping Cart is Empty";
        _showButtons = false;
      }
    }

    private string GetCartId()
    {
      var httpContext = HttpContextAccessor.HttpContext;
      var cartCookie = httpContext?.Request.Cookies["CartId"];
      if (!string.IsNullOrEmpty(cartCookie))
      {
        return cartCookie;
      }
      var newCartId = Guid.NewGuid().ToString();
      httpContext?.Response.Cookies.Append("CartId", newCartId, new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) });
      return newCartId;
    }

    private async Task UpdateBtn_Click(MouseEventArgs e)
    {
      // TODO: Implement cart update logic — read quantities from form, update DB
      var cartId = GetCartId();
      using var db = DbFactory.CreateDbContext();
      _cartItems = await db.ShoppingCartItems
          .Where(c => c.CartId == cartId)
          .Include(c => c.Product)
          .ToListAsync();
      _cartTotal = (decimal)_cartItems.Sum(c => c.Quantity * (c.Product?.UnitPrice ?? 0));
      _totalDisplay = string.Format("{0:c}", _cartTotal);
    }

    private void CheckoutBtn_Click(MouseEventArgs e)
    {
      // TODO: Integrate with payment provider (PayPal checkout flow)
      NavigationManager.NavigateTo("/CheckoutStart");
    }
  }
}
