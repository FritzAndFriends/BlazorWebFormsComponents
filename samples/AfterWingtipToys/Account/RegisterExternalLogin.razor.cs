using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace WingtipToys.Account
{
    public partial class RegisterExternalLogin
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        // TODO: ASP.NET Core Identity — inject SignInManager<ApplicationUser> and UserManager<ApplicationUser>

        private string providerName = string.Empty;
        private string ProviderName => providerName;
        private string providerAccountKey = string.Empty;

        private void RedirectOnFail()
        {
            // TODO: Check if user is authenticated
            // NavigationManager.NavigateTo(User.Identity.IsAuthenticated ? "/Account/Manage" : "/Account/Login");
            NavigationManager.NavigateTo("/Account/Login");
        }

        protected override async Task OnInitializedAsync()
        {
            // TODO: Migrate Identity logic from Web Forms OWIN to ASP.NET Core Identity
            // Original Page_Load code:
            // providerName = IdentityHelper.GetProviderNameFromRequest(Request);
            // if (String.IsNullOrEmpty(providerName))
            // {
            //     RedirectOnFail();
            //     return;
            // }
            //
            // var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            // var loginInfo = Context.GetOwinContext().Authentication.GetExternalLoginInfo();
            // if (loginInfo == null)
            // {
            //     RedirectOnFail();
            //     return;
            // }
            // var user = manager.Find(loginInfo.Login);
            // if (user != null)
            // {
            //     IdentityHelper.SignIn(manager, user, isPersistent: false);
            //     IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
            // }
            // else if (User.Identity.IsAuthenticated)
            // {
            //     var verifiedloginInfo = Context.GetOwinContext().Authentication.GetExternalLoginInfo(IdentityHelper.XsrfKey, User.Identity.GetUserId());
            //     if (verifiedloginInfo == null)
            //     {
            //         RedirectOnFail();
            //         return;
            //     }
            //     var result = manager.AddLogin(User.Identity.GetUserId(), verifiedloginInfo.Login);
            //     if (result.Succeeded)
            //         IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
            //     else
            //         AddErrors(result);
            // }
            // else
            // {
            //     email.Text = loginInfo.Email;
            // }

            await Task.CompletedTask;
        }

        private void LogIn_Click(MouseEventArgs e)
        {
            // TODO: ASP.NET Core Identity — requires UserManager<ApplicationUser>/SignInManager<ApplicationUser> migration
            // CreateAndLoginUser();
        }

        // TODO: ASP.NET Core Identity — requires UserManager<ApplicationUser>/SignInManager<ApplicationUser> migration
        // private void CreateAndLoginUser()
        // {
        //     if (!IsValid) return;
        //     var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //     var user = new ApplicationUser() { UserName = email.Text, Email = email.Text };
        //     IdentityResult result = manager.Create(user);
        //     if (result.Succeeded)
        //     {
        //         var loginInfo = Context.GetOwinContext().Authentication.GetExternalLoginInfo();
        //         if (loginInfo == null) { RedirectOnFail(); return; }
        //         result = manager.AddLogin(user.Id, loginInfo.Login);
        //         if (result.Succeeded)
        //         {
        //             IdentityHelper.SignIn(manager, user, isPersistent: false);
        //             IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
        //             return;
        //         }
        //     }
        //     AddErrors(result);
        // }

        // TODO: ASP.NET Core Identity — requires UserManager<ApplicationUser>/SignInManager<ApplicationUser> migration
        // private void AddErrors(IdentityResult result)
        // {
        //     foreach (var error in result.Errors)
        //     {
        //         ModelState.AddModelError("", error);
        //     }
        // }
    }
}
