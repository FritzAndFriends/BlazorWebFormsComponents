using Microsoft.AspNetCore.Components;

namespace WingtipToys;

public partial class ErrorPage : ComponentBase
{
    [SupplyParameterFromQuery(Name = "handler")]
    public string? Handler { get; set; }

    [SupplyParameterFromQuery(Name = "msg")]
    public string? Msg { get; set; }

    private string _friendlyErrorMsg = "A problem has occurred on this web site. Please try again. If this error continues, please contact support.";

    protected override void OnInitialized()
    {
        if (Msg == "404")
        {
            _friendlyErrorMsg = "An HTTP error occurred. Page Not found. Please try again.";
        }
    }
}
