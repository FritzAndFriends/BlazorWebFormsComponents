using Microsoft.AspNetCore.Components;

namespace WingtipToys.Checkout;

public partial class CheckoutStart
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        // TODO: Implement PayPal Express Checkout integration
        // Original used Session["payment_amt"], NVPAPICaller.ShortcutExpressCheckout()
        // For now, redirect to error page indicating checkout not yet implemented
        NavigationManager.NavigateTo("/CheckoutError?ErrorCode=NotImplemented&Desc=PayPal+checkout+not+yet+migrated");
        await Task.CompletedTask;
    }
}
