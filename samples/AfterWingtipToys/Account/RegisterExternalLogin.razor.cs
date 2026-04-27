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

// --- ViewState Migration ---
// ViewState is in-memory only in Blazor (does not survive navigation).
// Convert to private fields or [Parameter] properties:
//   private object _providerName; // was ViewState["ProviderName"]
//   private object _providerAccountKey; // was ViewState["ProviderAccountKey"]
// Note: BaseWebFormsComponent.ViewState exists as an [Obsolete] compatibility shim.

using System;
using WingtipToys.Models;

using BlazorWebFormsComponents.Identity;
using IdentityResult = BlazorWebFormsComponents.Identity.IdentityResult;

namespace WingtipToys.Account
{
    public partial class RegisterExternalLogin
    {
    // TODO(bwfc-general): ClientScript calls preserved — works via WebFormsPageBase (no injection needed). ScriptManagerShim may need @inject ScriptManagerShim ScriptManager for non-page classes.

    // --- Request.Form Migration ---
    // TODO(bwfc-form): Request.Form calls work automatically via RequestShim on WebFormsPageBase.
    // For interactive mode, wrap your form in <WebFormsForm OnSubmit="SetRequestFormData">.
    // Form keys found: key
    // For non-page classes, inject RequestShim via DI.

    // --- Response.Redirect Migration ---
    // TODO(bwfc-navigation): Response.Redirect() works via ResponseShim on WebFormsPageBase. Handles ~/ and .aspx automatically.
    // For non-page classes, inject ResponseShim via DI.

    private TextBox email = default!;
    // --- ConfigurationManager Migration ---
    // TODO(bwfc-config): ConfigurationManager calls work via BWFC shim.
    // Ensure app.UseConfigurationManagerShim() is called in Program.cs.

        protected string ProviderName
        {
            get { return (string)ViewState["ProviderName"] ?? String.Empty; }
            private set { ViewState["ProviderName"] = value; }
        }

        protected string ProviderAccountKey
        {
            get { return (string)ViewState["ProviderAccountKey"] ?? String.Empty; }
            private set { ViewState["ProviderAccountKey"] = value; }
        }

        private void RedirectOnFail()
        {
            Response.Redirect((User.Identity.IsAuthenticated) ? "~/Account/Manage" : "~/Account/Login");
        }

        protected void Page_Load()
        {
            // Process the result from an auth provider in the request
            ProviderName = IdentityHelper.GetProviderNameFromRequest(Request);
            if (String.IsNullOrEmpty(ProviderName))
            {
                RedirectOnFail();
                return;
            }
                        // BWFC: IsPostBack guard unwrapped — Blazor re-renders on every state change
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                        var loginInfo = Context.GetOwinContext().Authentication.GetExternalLoginInfo();
                        if (loginInfo == null)
                        {
                            RedirectOnFail();
                            return;
                        }
                        var user = manager.Find(loginInfo.Login);
                        if (user != null)
                        {
                            IdentityHelper.SignIn(manager, user, isPersistent: false);
                            IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                        }
                        else if (User.Identity.IsAuthenticated)
                        {
                            // Apply Xsrf check when linking
                            var verifiedloginInfo = Context.GetOwinContext().Authentication.GetExternalLoginInfo(IdentityHelper.XsrfKey, User.Identity.GetUserId());
                            if (verifiedloginInfo == null)
                            {
                                RedirectOnFail();
                                return;
                            }

                            var result = manager.AddLogin(User.Identity.GetUserId(), verifiedloginInfo.Login);
                            if (result.Succeeded)
                            {
                                IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                            }
                            else
                            {
                                AddErrors(result);
                                return;
                            }
                        }
                        else
                        {
                            email.Text = loginInfo.Email;
                        }
        }        
        
        protected void LogIn_Click()
        {
            CreateAndLoginUser();
        }

        private void CreateAndLoginUser()
        {
            if (!IsValid)
            {
                return;
            }
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = new ApplicationUser() { UserName = email.Text, Email = email.Text };
            IdentityResult result = manager.Create(user);
            if (result.Succeeded)
            {
                var loginInfo = Context.GetOwinContext().Authentication.GetExternalLoginInfo();
                if (loginInfo == null)
                {
                    RedirectOnFail();
                    return;
                }
                result = manager.AddLogin(user.Id, loginInfo.Login);
                if (result.Succeeded)
                {
                    IdentityHelper.SignIn(manager, user, isPersistent: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // var code = manager.GenerateEmailConfirmationToken(user.Id);
                    // Send this link via email: IdentityHelper.GetUserConfirmationRedirectUrl(code, user.Id)

                    IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                    return;
                }
            }
            AddErrors(result);
        }

        private void AddErrors(IdentityResult result) 
        {
            foreach (var error in result.Errors) 
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}