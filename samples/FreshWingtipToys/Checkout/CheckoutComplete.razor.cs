using Microsoft.AspNetCore.Components;
using WingtipToys.Services;

namespace WingtipToys.Checkout
{
    public partial class CheckoutComplete : ComponentBase
    {
        private string _transactionId = "";

        protected override void OnInitialized()
        {
            _transactionId = CheckoutState.GetTransactionId() ?? "N/A";
        }

        private void ContinueShopping_Click()
        {
            CheckoutState.ClearCheckoutState();
            Navigation.NavigateTo("/");
        }
    }
}