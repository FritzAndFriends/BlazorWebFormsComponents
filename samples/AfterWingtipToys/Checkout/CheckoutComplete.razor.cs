using Microsoft.AspNetCore.Components;

namespace WingtipToys.Checkout;

public partial class CheckoutComplete
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private string _transactionId = "";

    protected override async Task OnInitializedAsync()
    {
        // TODO: Implement PayPal DoCheckoutPayment confirmation
        // Original used Session["token"], Session["payerId"], Session["payment_amt"]
        // and NVPAPICaller.DoCheckoutPayment() to finalize order
        _transactionId = "TODO: PayPal checkout not yet migrated";
        await Task.CompletedTask;
    }

    private void Continue_Click(Microsoft.AspNetCore.Components.Web.MouseEventArgs e)
    {
        NavigationManager.NavigateTo("/");
    }
}
