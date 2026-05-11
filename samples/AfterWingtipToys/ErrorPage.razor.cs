using WingtipToys.Logic;

namespace WingtipToys;

public partial class ErrorPage
{
    private Panel DetailedErrorPanel = default!;
    private Label ErrorDetailedMsg = default!;
    private Label ErrorHandler = default!;
    private Label FriendlyErrorMsg = default!;
    private Label InnerMessage = default!;
    private Label InnerTrace = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        const string generalErrorMsg = "A problem has occurred on this web site. Please try again. If this error continues, please contact support.";
        const string httpErrorMsg = "An HTTP error occurred. Page Not found. Please try again.";
        const string unhandledErrorMsg = "The error was unhandled by application code.";

        FriendlyErrorMsg.Text = generalErrorMsg;

        var errorHandler = Request.QueryString["handler"].ToString();
        if (string.IsNullOrWhiteSpace(errorHandler))
        {
            errorHandler = "Error Page";
        }

        var ex = Server.GetLastError() ?? new Exception(unhandledErrorMsg);
        var errorMsg = Request.QueryString["msg"].ToString();

        if (errorMsg == "404")
        {
            ex = new InvalidOperationException(httpErrorMsg, ex);
            FriendlyErrorMsg.Text = ex.Message;
        }

        ErrorDetailedMsg.Text = ex.Message;
        ErrorHandler.Text = errorHandler;
        DetailedErrorPanel.Visible = true;

        if (ex.InnerException != null)
        {
            InnerMessage.Text = ex.GetType() + "<br/>" + ex.InnerException.Message;
            InnerTrace.Text = ex.InnerException.StackTrace ?? string.Empty;
        }
        else
        {
            InnerMessage.Text = ex.GetType().ToString();
            InnerTrace.Text = ex.StackTrace?.TrimStart() ?? string.Empty;
        }

        ExceptionUtility.LogException(ex, errorHandler);
        Server.ClearError();
    }
}
