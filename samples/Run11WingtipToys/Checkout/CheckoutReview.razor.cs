using Microsoft.AspNetCore.Components;
using WingtipToys.Models;
using WingtipToys.Services;

namespace WingtipToys.Checkout;

public partial class CheckoutReview
{
    [Inject] private CartStateService Cart { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;

    private List<CartItem> _cartItems = new();
    private List<OrderShipInfo> _shipInfoList = new();

    protected override void OnInitialized()
    {
        _cartItems = Cart.Items.ToList();

        // Placeholder shipping info bound to the DetailsView
        _shipInfoList = new List<OrderShipInfo>
        {
            new OrderShipInfo
            {
                FirstName = "John",
                LastName = "Doe",
                Address = "123 Main St",
                City = "Redmond",
                State = "WA",
                PostalCode = "98052",
                Total = Cart.GetTotal()
            }
        };
    }

    private void OnCheckoutConfirm(EventArgs args)
    {
        Nav.NavigateTo("/CheckoutComplete");
    }
}
