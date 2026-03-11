using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace WingtipToys.Account
{
    public partial class TwoFactorAuthenticationSignIn
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        // TODO: ASP.NET Core Identity — inject SignInManager<ApplicationUser> and UserManager<ApplicationUser>
        // Original class had fields:
        // private ApplicationSignInManager signinManager;
        // private ApplicationUserManager manager;

        protected override async Task OnInitializedAsync()
        {
            // TODO: Migrate Identity logic from Web Forms OWIN to ASP.NET Core Identity
            // Original constructor + Page_Load code:
            // manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            // signinManager = Context.GetOwinContext().GetUserManager<ApplicationSignInManager>();
            // var userId = signinManager.GetVerifiedUserId<ApplicationUser, string>();
            // if (userId == null)
            // {
            //     NavigationManager.NavigateTo("/Account/Error");
            // }
            // var userFactors = manager.GetValidTwoFactorProviders(userId);
            // Providers.DataSource = userFactors.Select(x => x).ToList();
            // Providers.DataBind();

            await Task.CompletedTask;
        }

        private void CodeSubmit_Click(MouseEventArgs e)
        {
            // TODO: ASP.NET Core Identity — requires UserManager<ApplicationUser>/SignInManager<ApplicationUser> migration
            // Original logic:
            // bool rememberMe = false;
            // bool.TryParse(Request.QueryString["RememberMe"], out rememberMe);
            // var result = signinManager.TwoFactorSignIn<ApplicationUser, string>(SelectedProvider.Value, Code.Text, isPersistent: rememberMe, rememberBrowser: RememberBrowser.Checked);
            // switch (result)
            // {
            //     case SignInStatus.Success:
            //         IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
            //         break;
            //     case SignInStatus.LockedOut:
            //         NavigationManager.NavigateTo("/Account/Lockout");
            //         break;
            //     case SignInStatus.Failure:
            //     default:
            //         // FailureText.Text = "Invalid code";
            //         break;
            // }
        }

        private void ProviderSubmit_Click(MouseEventArgs e)
        {
            // TODO: ASP.NET Core Identity — requires UserManager<ApplicationUser>/SignInManager<ApplicationUser> migration
            // Original logic:
            // if (!signinManager.SendTwoFactorCode(Providers.SelectedValue))
            // {
            //     NavigationManager.NavigateTo("/Account/Error");
            // }
            // var user = manager.FindById(signinManager.GetVerifiedUserId<ApplicationUser, string>());
            // if (user != null)
            // {
            //     var code = manager.GenerateTwoFactorToken(user.Id, Providers.SelectedValue);
            // }
            // SelectedProvider.Value = Providers.SelectedValue;
        }
    }
}
