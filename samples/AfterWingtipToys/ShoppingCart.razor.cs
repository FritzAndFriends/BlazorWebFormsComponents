using WingtipToys.Logic;
using WingtipToys.Models;

namespace WingtipToys
{
  public partial class ShoppingCart
  {
    protected string ShoppingCartTitleText { get; private set; } = "Shopping Cart";
    protected string OrderTotalText { get; private set; } = string.Empty;
    protected List<CartItem> ShoppingCartItems { get; private set; } = [];

    protected override async Task OnParametersSetAsync()
    {
      await base.OnParametersSetAsync();
      RefreshTotals();
    }

    private void RefreshTotals()
    {
      using var usersShoppingCart = new ShoppingCartActions();
      ShoppingCartItems = usersShoppingCart.GetCartItems();
      var cartTotal = usersShoppingCart.GetTotal();
      OrderTotalText = cartTotal > 0 ? $"{cartTotal:c}" : string.Empty;
      ShoppingCartTitleText = cartTotal > 0 ? "Shopping Cart" : "Shopping Cart is Empty";
    }
  }
}
