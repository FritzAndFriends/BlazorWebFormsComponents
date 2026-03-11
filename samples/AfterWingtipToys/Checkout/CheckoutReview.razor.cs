using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Checkout
{
  public partial class CheckoutReview
  {
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private List<CartItem>? _orderItems;
    private Order? _orderInfo;

    protected override async Task OnInitializedAsync()
    {
      Page.Title = "Order Review";
      // TODO: Integrate with PayPal NVPAPICaller to get checkout details
      // The original code uses Session["token"], PayPal GetCheckoutDetails, creates Order, etc.
      // For now, load cart items as a placeholder
      using var db = DbFactory.CreateDbContext();
      _orderItems = await db.ShoppingCartItems.Include(c => c.Product).ToListAsync();
    }

    private void CheckoutConfirm_Click(MouseEventArgs e)
    {
      // TODO: Complete PayPal checkout flow
      // Original: Session["userCheckoutCompleted"] = "true"; Response.Redirect("CheckoutComplete")
      NavigationManager.NavigateTo("/CheckoutComplete");
    }
  }
}
