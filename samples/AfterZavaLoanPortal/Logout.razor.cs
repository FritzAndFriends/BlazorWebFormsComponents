using ConfigurationManager = BlazorWebFormsComponents.ConfigurationManager;

namespace ZavaLoanPortal;

public partial class Logout : BlazorWebFormsComponents.WebFormsPageBase
{
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var logoutUrl = ConfigurationManager.AppSettings["AuthGatewayLogoutUrl"] ?? "http://localhost:8000/Logout.aspx";
        Response.Redirect(logoutUrl, false);
    }
}