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
using WingtipToys.Models;

using WingtipToys.Logic;
using BlazorWebFormsComponents.Identity;

using System.Net;
using Microsoft.AspNetCore.Components;
namespace WingtipToys.Account
{
    public partial class Login : WebFormsPageBase
    {
	protected string Title { get; set; } = "Log in";

    // TODO(bwfc-general): ClientScript calls preserved — works via WebFormsPageBase (no injection needed). ScriptManagerShim may need @inject ScriptManagerShim ScriptManager for non-page classes.

    // --- Request.Form Migration ---
    // TODO(bwfc-form): Request.Form calls work automatically via RequestShim on WebFormsPageBase.
    // For interactive mode, wrap your form in <WebFormsForm OnSubmit="SetRequestFormData">.
    // Form keys found: key
    // For non-page classes, inject RequestShim via DI.

    // --- Response.Redirect Migration ---
    // TODO(bwfc-navigation): Response.Redirect() works via ResponseShim on WebFormsPageBase. Handles ~/ and .aspx automatically.
    // For non-page classes, inject ResponseShim via DI.

    private TextBox Email = default!;
    private PlaceHolder ErrorMessage = default!;
    private Literal FailureText = default!;
    private HyperLink ForgotPasswordHyperLink = default!;
    private OpenAuthProviders OpenAuthLogin = default!;
    private TextBox Password = default!;
    private HyperLink RegisterHyperLink = default!;
    private CheckBox RememberMe = default!;
    [Inject]
    protected ShoppingCartActions _shoppingCartActions { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            // Identity flow handled by generated ASP.NET Core endpoints.
            return;
        }

        protected void LogIn(EventArgs e)
        {
            // Identity flow handled by generated ASP.NET Core endpoints.
        }
    
    [Parameter, SupplyParameterFromQuery(Name = "error")] public string? Error { get; set; }
    [Parameter, SupplyParameterFromQuery(Name = "registered")] public int? Registered { get; set; }
    [Parameter, SupplyParameterFromQuery(Name = "returnUrl")] public string? ReturnUrl { get; set; }
}
}