using Microsoft.AspNetCore.Components;

namespace WingtipToys.Checkout;

public partial class CheckoutError
{
    [SupplyParameterFromQuery(Name = "ErrorCode")]
    public string? ErrorCode { get; set; }

    [SupplyParameterFromQuery(Name = "Desc")]
    public string? Desc { get; set; }

    [SupplyParameterFromQuery(Name = "Desc2")]
    public string? Desc2 { get; set; }
}
