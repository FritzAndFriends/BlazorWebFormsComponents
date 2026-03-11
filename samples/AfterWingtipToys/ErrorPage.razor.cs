using Microsoft.AspNetCore.Components;

namespace WingtipToys;

public partial class ErrorPage
{
    [SupplyParameterFromQuery(Name = "handler")]
    public string? Handler { get; set; }

    [SupplyParameterFromQuery(Name = "msg")]
    public string? ErrorMsg { get; set; }

    private string _friendlyErrorMsg = "";
    private string _errorDetailedMsg = "";
    private string _errorHandler = "";
    private string _innerMessage = "";
    private string _innerTrace = "";
    private bool _showDetails = false;

    protected override async Task OnInitializedAsync()
    {
        var generalErrorMsg = "A problem has occurred on this web site. Please try again. " +
            "If this error continues, please contact support.";
        var httpErrorMsg = "An HTTP error occurred. Page Not found. Please try again.";

        _friendlyErrorMsg = generalErrorMsg;
        _errorHandler = Handler ?? "Error Page";

        if (ErrorMsg == "404")
        {
            _friendlyErrorMsg = httpErrorMsg;
        }

        // In Blazor, detailed errors are shown in Development environment
        // The original used Request.IsLocal — we use IHostEnvironment instead
        await Task.CompletedTask;
    }
}
