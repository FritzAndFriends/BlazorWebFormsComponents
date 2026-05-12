using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WingtipToys
{
    public partial class ErrorPage
    {
        [Inject] public IWebHostEnvironment Env { get; set; } = default!;
        [Inject] public ILogger<ErrorPage> Logger { get; set; } = default!;

        private Panel DetailedErrorPanel = default!;
        private Label ErrorDetailedMsg = default!;
        private Label ErrorHandler = default!;
        private Label FriendlyErrorMsg = default!;
        private Label InnerMessage = default!;
        private Label InnerTrace = default!;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            var generalErrorMsg = "A problem has occurred on this web site. Please try again. If this error continues, please contact support.";
            var httpErrorMsg = "An HTTP error occurred. Page Not found. Please try again.";
            var unhandledErrorMsg = "The error was unhandled by application code.";

            FriendlyErrorMsg.Text = generalErrorMsg;

            var errorHandler = Request.QueryString["handler"].ToString();
            if (string.IsNullOrEmpty(errorHandler))
            {
                errorHandler = "Error Page";
            }

            var ex = Server.GetLastError();
            var errorMsg = Request.QueryString["msg"].ToString();
            if (errorMsg == "404")
            {
                ex = new InvalidOperationException(httpErrorMsg, ex);
                FriendlyErrorMsg.Text = ex.Message;
            }

            if (ex == null)
            {
                ex = new Exception(unhandledErrorMsg);
            }

            if (Env.IsDevelopment())
            {
                ErrorDetailedMsg.Text = ex.Message;
                ErrorHandler.Text = errorHandler;
                DetailedErrorPanel.Visible = true;

                if (ex.InnerException != null)
                {
                    InnerMessage.Text = ex.GetType() + "<br/>" + ex.InnerException.Message;
                    InnerTrace.Text = ex.InnerException.StackTrace;
                }
                else
                {
                    InnerMessage.Text = ex.GetType().ToString();
                    if (ex.StackTrace != null)
                    {
                        InnerTrace.Text = ex.StackTrace.TrimStart();
                    }
                }
            }

            Logger.LogError(ex, "Unhandled error on {ErrorHandler}", errorHandler);
            Server.ClearError();
        }
    }
}
