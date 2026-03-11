using Microsoft.AspNetCore.Components;

namespace WingtipToys.Checkout
{
  public partial class CheckoutCancel
  {
    protected override async Task OnInitializedAsync()
    {
      Page.Title = "Checkout Cancelled";
    }
  }
}
