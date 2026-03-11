using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace WingtipToys.Checkout
{
  public partial class CheckoutComplete
  {
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private string _transactionId = "";

    protected override async Task OnInitializedAsync()
    {
      Page.Title = "Checkout Complete";
      // TODO: Integrate with PayPal DoCheckoutPayment
      // Original code uses Session["token"], Session["payerId"], Session["payment_amt"]
      // and calls payPalCaller.DoCheckoutPayment to finalize payment
      _transactionId = "TODO: PayPal integration required";
    }

    private void Continue_Click(MouseEventArgs e)
    {
      NavigationManager.NavigateTo("/");
    }
  }
}
