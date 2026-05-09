namespace WingtipToys;

public partial class ErrorPage
{
    protected string FriendlyErrorMessage { get; set; } =
        "A problem has occurred on this web site. Please try again. If this error continues, please contact support.";

    protected bool ShowDetailedError { get; set; }

    protected string DetailedErrorMessage { get; set; } = string.Empty;

    protected string ErrorHandlerText { get; set; } = "Error Page";

    protected string InnerMessageText { get; set; } = string.Empty;

    protected string InnerTraceText { get; set; } = string.Empty;

    protected override Task OnInitializedAsync()
    {
        var exception = Server.GetLastError() ?? new Exception("The error was unhandled by application code.");
        ErrorHandlerText = Request.QueryString["handler"].ToString();
        if (string.IsNullOrWhiteSpace(ErrorHandlerText))
        {
            ErrorHandlerText = "Error Page";
        }

        if (Request.QueryString["msg"] == "404")
        {
            FriendlyErrorMessage = "An HTTP error occurred. Page not found. Please try again.";
        }

        ShowDetailedError = Request.Url.IsLoopback;
        if (ShowDetailedError)
        {
            DetailedErrorMessage = exception.Message;
            InnerMessageText = exception.InnerException?.Message ?? exception.GetType().ToString();
            InnerTraceText = exception.InnerException?.StackTrace ?? exception.StackTrace ?? string.Empty;
        }

        Console.Error.WriteLine($"WingtipToys error handled by {ErrorHandlerText}: {exception}");
        Server.ClearError();
        return Task.CompletedTask;
    }
}
