using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys
{
  public partial class AddToCart
  {
    [SupplyParameterFromQuery(Name = "productID")]
    public int? ProductID { get; set; }

    protected override async Task OnInitializedAsync()
    {
      if (ProductID.HasValue && ProductID > 0)
      {
        var cartId = GetCartId();
        using var db = DbFactory.CreateDbContext();

        var cartItem = await db.ShoppingCartItems
            .FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductId == ProductID.Value);

        if (cartItem != null)
        {
          cartItem.Quantity++;
        }
        else
        {
          cartItem = new CartItem
          {
            ItemId = Guid.NewGuid().ToString(),
            CartId = cartId,
            ProductId = ProductID.Value,
            Quantity = 1,
            DateCreated = DateTime.Now
          };
          db.ShoppingCartItems.Add(cartItem);
        }
        await db.SaveChangesAsync();
      }

      NavigationManager.NavigateTo("/ShoppingCart");
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
  }
}
