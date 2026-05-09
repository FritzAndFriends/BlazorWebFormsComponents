using Microsoft.AspNetCore.Components;

namespace WingtipToys;

public partial class ErrorPage
{
    [Parameter, SupplyParameterFromQuery(Name = "handler")]
    public string? Handler { get; set; }

    [Parameter, SupplyParameterFromQuery(Name = "msg")]
    public string? Message { get; set; }

    protected string FriendlyMessage => string.Equals(Message, "404", StringComparison.Ordinal)
        ? "The requested page could not be found."
        : "A problem has occurred on this web site. Please try again.";
}
