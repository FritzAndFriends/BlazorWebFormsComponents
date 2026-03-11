using Microsoft.AspNetCore.Components;

namespace WingtipToys.Checkout
{
  public partial class CheckoutStart
  {
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
      Page.Title = "Checkout";
      // TODO: Integrate with PayPal ShortcutExpressCheckout
      // Original code reads Session["payment_amt"], calls NVPAPICaller.ShortcutExpressCheckout,
      // stores token in Session, and redirects to PayPal URL
      NavigationManager.NavigateTo("/CheckoutError?ErrorCode=NotImplemented&Desc=PayPal+integration+required");
    }
  }
}
