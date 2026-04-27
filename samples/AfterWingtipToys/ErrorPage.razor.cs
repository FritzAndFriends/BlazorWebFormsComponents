// =============================================================================
// TODO(bwfc-general): This code-behind was copied from Web Forms and needs manual migration.
//
// Common transforms needed (use the BWFC Copilot skill for assistance):
//   TODO(bwfc-lifecycle): Page_Load / Page_Init → OnInitializedAsync / OnParametersSetAsync
//   TODO(bwfc-lifecycle): Page_PreRender → OnAfterRenderAsync
//   TODO(bwfc-ispostback): IsPostBack checks → remove or convert to state logic
//   TODO(bwfc-viewstate): ViewState usage → component [Parameter] or private fields
//   TODO(bwfc-session-state): Session/Cache access → auto-wired on WebFormsPageBase via SessionShim/CacheShim
//   TODO(bwfc-navigation): Response.Redirect → auto-wired on WebFormsPageBase via ResponseShim
//   TODO(bwfc-form): Request.Form["key"] → auto-wired on WebFormsPageBase via FormShim (use <WebFormsForm> for interactive mode)
//   TODO(bwfc-server): Server.MapPath/HtmlEncode → auto-wired on WebFormsPageBase via ServerShim
//   TODO(bwfc-config): ConfigurationManager.AppSettings → BWFC shim (call app.UseConfigurationManagerShim() in Program.cs)
//   TODO(bwfc-general): ClientScript.RegisterStartupScript → auto-wired on WebFormsPageBase via ClientScriptShim
//   TODO(bwfc-general): Event handlers (Button_Click, etc.) → convert to Blazor event callbacks
//   TODO(bwfc-datasource): Data binding (DataBind, DataSource) → component parameters or OnInitialized
//   TODO(bwfc-general): ScriptManager code-behind references → use ScriptManagerShim via ScriptManager.GetCurrent(this)
//   TODO(bwfc-general): UpdatePanel markup preserved by BWFC (ContentTemplate supported) — remove only code-behind API calls
//   TODO(bwfc-general): User controls → Blazor component references
// =============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using WingtipToys.Logic;

namespace WingtipToys
{
  public partial class ErrorPage
  {
    // TODO(bwfc-general): ClientScript calls preserved — works via WebFormsPageBase (no injection needed). ScriptManagerShim may need @inject ScriptManagerShim ScriptManager for non-page classes.

    // --- Server Utility Migration ---
    // TODO(bwfc-server): Server.* calls work automatically via ServerShim on WebFormsPageBase.
    // For non-page classes, inject ServerShim via DI.

    // --- Request.Form Migration ---
    // TODO(bwfc-form): Request.Form calls work automatically via RequestShim on WebFormsPageBase.
    // For interactive mode, wrap your form in <WebFormsForm OnSubmit="SetRequestFormData">.
    // Form keys found: key
    // For non-page classes, inject RequestShim via DI.

    private Panel DetailedErrorPanel = default!;
    private Label ErrorDetailedMsg = default!;
    private Label ErrorHandler = default!;
    private Label FriendlyErrorMsg = default!;
    private Label InnerMessage = default!;
    private Label InnerTrace = default!;
    // --- ConfigurationManager Migration ---
    // TODO(bwfc-config): ConfigurationManager calls work via BWFC shim.
    // Ensure app.UseConfigurationManagerShim() is called in Program.cs.

    protected override async Task OnInitializedAsync()
    {
        // TODO(bwfc-lifecycle): Review lifecycle conversion — verify async behavior
        await base.OnInitializedAsync();

      // Create safe error messages.
      string generalErrorMsg = "A problem has occurred on this web site. Please try again. " +
          "If this error continues, please contact support.";
      string httpErrorMsg = "An HTTP error occurred. Page Not found. Please try again.";
      string unhandledErrorMsg = "The error was unhandled by application code.";

      // Display safe error message.
      FriendlyErrorMsg.Text = generalErrorMsg;

      // Determine where error was handled.
      string errorHandler = Request.QueryString["handler"];
      if (errorHandler == null)
      {
        errorHandler = "Error Page";
      }

      // Get the last error from the server.
      Exception ex = Server.GetLastError( /* TODO(bwfc-server): Server.GetLastError has NO SHIM. Use ILogger and middleware-based error handling (app.UseExceptionHandler). */ );

      // Get the error number passed as a querystring value.
      string errorMsg = Request.QueryString["msg"];
      if (errorMsg == "404")
      {
        ex = new HttpException(404, httpErrorMsg, ex);
        FriendlyErrorMsg.Text = ex.Message;
      }

      // If the exception no longer exists, create a generic exception.
      if (ex == null)
      {
        ex = new Exception(unhandledErrorMsg);
      }

      // Show error details to only you (developer). LOCAL ACCESS ONLY.
      if (Request.IsLocal)
      {
        // Detailed Error Message.
        ErrorDetailedMsg.Text = ex.Message;

        // Show where the error was handled.
        ErrorHandler.Text = errorHandler;

        // Show local access details.
        DetailedErrorPanel.Visible = true;

        if (ex.InnerException != null)
        {
          InnerMessage.Text = ex.GetType().ToString() + "<br/>" +
              ex.InnerException.Message;
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

      // Log the exception.
      ExceptionUtility.LogException(ex, errorHandler);

      // Clear the error from the server.
      Server.ClearError( /* TODO(bwfc-server): Server.ClearError has NO SHIM. Error clearing is handled by middleware in ASP.NET Core. */ );
    }
  }
}