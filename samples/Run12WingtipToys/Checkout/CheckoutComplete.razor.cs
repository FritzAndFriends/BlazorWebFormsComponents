using Microsoft.AspNetCore.Components;

namespace WingtipToys.Checkout;

public partial class CheckoutComplete
{
    [Inject] private NavigationManager Nav { get; set; } = default!;

    private void OnContinueShopping(EventArgs args)
    {
        Nav.NavigateTo("/");
    }
}
