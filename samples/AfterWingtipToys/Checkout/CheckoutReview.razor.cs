using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Checkout;

public partial class CheckoutReview
{
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private List<CartItem> _orderItems = new();
    private Order? _order;

    protected override async Task OnInitializedAsync()
    {
        // TODO: Implement PayPal GetCheckoutDetails flow
        // Original used Session["token"], NVPAPICaller.GetCheckoutDetails()
        // Placeholder: display empty review page until PayPal integration is complete
        await Task.CompletedTask;
    }

    private void CheckoutConfirm_Click(Microsoft.AspNetCore.Components.Web.MouseEventArgs e)
    {
        // TODO: Implement PayPal DoCheckoutPayment flow
        NavigationManager.NavigateTo("/CheckoutComplete");
    }
}
