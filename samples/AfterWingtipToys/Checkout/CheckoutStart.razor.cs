using Microsoft.AspNetCore.Components;
using WingtipToys.Services;

namespace WingtipToys.Checkout
{
    public partial class CheckoutStart : ComponentBase
    {
        protected override async Task OnInitializedAsync()
        {
            await CheckoutState.CreateOrderFromCartAsync(CartService, "DemoUser");

            var order = CheckoutState.GetCurrentOrder();
            if (order != null)
            {
                var (redirectUrl, token) = await PayPalService.SetExpressCheckoutAsync(order);
                Navigation.NavigateTo("/CheckoutReview");
            }
        }
    }
}
