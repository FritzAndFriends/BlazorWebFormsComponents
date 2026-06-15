namespace WingtipToys.Checkout;

public partial class CheckoutReview : BlazorWebFormsComponents.WebFormsPageBase
{
    protected override Task OnInitializedAsync()
    {
        Title = "Order Review";
        return base.OnInitializedAsync();
    }
}
