// =============================================================================
// TODO(bwfc-general): This code-behind was copied from Web Forms and needs manual migration.
// =============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using WingtipToys.Logic;

namespace WingtipToys
{
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

      string generalErrorMsg = "A problem has occurred on this web site. Please try again. If this error continues, please contact support.";
      string httpErrorMsg = "An HTTP error occurred. Page Not found. Please try again.";
      string unhandledErrorMsg = "The error was unhandled by application code.";

      FriendlyErrorMsg.Text = generalErrorMsg;

      string errorHandler = Request.QueryString["handler"];
      if (errorHandler == null)
      {
        errorHandler = "Error Page";
      }

      Exception? ex = null;

      string errorMsg = Request.QueryString["msg"];
      if (errorMsg == "404")
      {
        ex = new Exception(httpErrorMsg, ex);
        FriendlyErrorMsg.Text = ex.Message;
      }

      if (ex == null)
      {
        ex = new Exception(unhandledErrorMsg);
      }

      if (false)
      {
        ErrorDetailedMsg.Text = ex.Message;
        ErrorHandler.Text = errorHandler;
        DetailedErrorPanel.Visible = true;

        if (ex.InnerException != null)
        {
          InnerMessage.Text = ex.GetType().ToString() + "<br/>" + ex.InnerException.Message;
          InnerTrace.Text = ex.InnerException.StackTrace;
        }
        else
        {
          InnerMessage.Text = ex.GetType().ToString();
          if (ex.StackTrace != null)
          {
            InnerTrace.Text = ex.StackTrace.ToString().TrimStart();
          }
        }
      }

      // TODO: log exception.
    }
  }
}
