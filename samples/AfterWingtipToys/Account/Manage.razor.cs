using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace WingtipToys.Account
{
    public partial class Manage
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        // TODO: ASP.NET Core Identity — inject SignInManager<ApplicationUser> and UserManager<ApplicationUser>

        protected string SuccessMessage { get; private set; } = string.Empty;

        public bool HasPhoneNumber { get; private set; }

        public bool TwoFactorEnabled { get; private set; }

        public bool TwoFactorBrowserRemembered { get; private set; }

        public int LoginsCount { get; set; }

        // TODO: ASP.NET Core Identity — requires UserManager<ApplicationUser>/SignInManager<ApplicationUser> migration
        // private bool HasPassword(UserManager<ApplicationUser> manager)
        // {
        //     return manager.HasPasswordAsync(userId).Result;
        // }

        protected override async Task OnInitializedAsync()
        {
            // TODO: Migrate Identity logic from Web Forms OWIN to ASP.NET Core Identity
            // Original Page_Load code:
            // var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            // HasPhoneNumber = String.IsNullOrEmpty(manager.GetPhoneNumber(User.Identity.GetUserId()));
            // TwoFactorEnabled = manager.GetTwoFactorEnabled(User.Identity.GetUserId());
            // LoginsCount = manager.GetLogins(User.Identity.GetUserId()).Count;
            // var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            //
            // Determine the sections to render
            // if (HasPassword(manager)) { ChangePassword visible } else { CreatePassword visible }
            //
            // Render success message from query string "m"
            // SuccessMessage = message switch for ChangePwdSuccess, SetPwdSuccess, RemoveLoginSuccess, etc.

            await Task.CompletedTask;
        }

        // TODO: ASP.NET Core Identity — requires UserManager<ApplicationUser>/SignInManager<ApplicationUser> migration
        // private void AddErrors(IdentityResult result)
        // {
        //     foreach (var error in result.Errors)
        //     {
        //         ModelState.AddModelError("", error);
        //     }
        // }

        private void RemovePhone_Click(MouseEventArgs e)
        {
            // TODO: ASP.NET Core Identity — requires UserManager<ApplicationUser>/SignInManager<ApplicationUser> migration
            // Original logic:
            // var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            // var result = manager.SetPhoneNumber(User.Identity.GetUserId(), null);
            // if (!result.Succeeded) return;
            // var user = manager.FindById(User.Identity.GetUserId());
            // if (user != null)
            // {
            //     IdentityHelper.SignIn(manager, user, isPersistent: false);
            //     NavigationManager.NavigateTo("/Account/Manage?m=RemovePhoneNumberSuccess");
            // }
        }

        private void TwoFactorDisable_Click(MouseEventArgs e)
        {
            // TODO: ASP.NET Core Identity — requires UserManager<ApplicationUser>/SignInManager<ApplicationUser> migration
            // Original logic:
            // var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            // manager.SetTwoFactorEnabled(User.Identity.GetUserId(), false);
            // NavigationManager.NavigateTo("/Account/Manage");
        }

        private void TwoFactorEnable_Click(MouseEventArgs e)
        {
            // TODO: ASP.NET Core Identity — requires UserManager<ApplicationUser>/SignInManager<ApplicationUser> migration
            // Original logic:
            // var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            // manager.SetTwoFactorEnabled(User.Identity.GetUserId(), true);
            // NavigationManager.NavigateTo("/Account/Manage");
        }
    }
}
