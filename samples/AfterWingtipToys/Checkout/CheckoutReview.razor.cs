namespace WingtipToys.Checkout;

public partial class CheckoutReview
{
    protected override Task OnInitializedAsync()
    {
        Title = "Order Review";
        return base.OnInitializedAsync();
    }
}
