using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys
{
  public partial class ErrorPage
  {
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "handler")]
    public string? Handler { get; set; }

    [SupplyParameterFromQuery(Name = "msg")]
    public string? ErrorMsg { get; set; }

    private string _friendlyErrorMsg = "";
    private string _errorDetailedMsg = "";
    private string _errorHandler = "";
    private string _innerMessage = "";
    private string _innerTrace = "";
    private bool _showDetailedError = false;

    protected override async Task OnInitializedAsync()
    {
      Page.Title = "Error";

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
      var httpContext = HttpContextAccessor.HttpContext;
      if (httpContext != null)
      {
        var env = httpContext.RequestServices.GetService<IWebHostEnvironment>();
        _showDetailedError = env?.IsDevelopment() ?? false;
      }
    }
  }
}
