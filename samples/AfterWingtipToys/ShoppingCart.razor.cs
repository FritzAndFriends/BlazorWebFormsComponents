using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys
{
  public partial class ShoppingCart
  {
    [Inject] public ProductContext Db { get; set; } = default!;
    [Inject] public NavigationManager Navigation { get; set; } = default!;

    private string ShoppingCartTitleText { get; set; } = "Shopping Cart";
    private string OrderTotalLabelText { get; set; } = "Order Total: ";
    private string OrderTotalText { get; set; } = string.Empty;
    private bool HasCartItems { get; set; }

    public IQueryable<CartItem> GetShoppingCartItems(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
    {
      var cartId = GetCartId();
      var query = Db.ShoppingCartItems
        .Include(item => item.Product)
        .Where(item => item.CartId == cartId);

      totalRowCount = query.Count();
      RefreshCartState(query);
      return query;
    }

    private void UpdateBtn_Click(EventArgs _)
    {
      RefreshCartState();
    }

    private void CheckoutBtn_Click(EventArgs _)
    {
      Session["payment_amt"] = OrderTotalText;
      Navigation.NavigateTo("/Checkout/CheckoutStart", forceLoad: true);
    }

    private void RefreshCartState(IQueryable<CartItem>? query = null)
    {
      query ??= Db.ShoppingCartItems
        .Include(item => item.Product)
        .Where(item => item.CartId == GetCartId());

      var items = query.ToList();
      var total = items.Sum(item => item.Quantity * (item.Product?.UnitPrice ?? 0d));

      HasCartItems = items.Count > 0;
      ShoppingCartTitleText = HasCartItems ? "Shopping Cart" : "Shopping Cart is Empty";
      OrderTotalLabelText = HasCartItems ? "Order Total: " : string.Empty;
      OrderTotalText = HasCartItems ? string.Format("{0:c}", total) : string.Empty;
    }

    private string GetCartId()
    {
      const string cartSessionKey = "CartId";
      var existingCartId = Session[cartSessionKey]?.ToString();
      if (!string.IsNullOrWhiteSpace(existingCartId))
      {
        return existingCartId;
      }

      var newCartId = Guid.NewGuid().ToString();
      Session[cartSessionKey] = newCartId;
      return newCartId;
    }
  }
}
