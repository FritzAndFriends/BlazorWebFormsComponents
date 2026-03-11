using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace WingtipToys.Account
{
    public partial class ManagePassword
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        // TODO: ASP.NET Core Identity — inject SignInManager<ApplicationUser> and UserManager<ApplicationUser>

        protected string SuccessMessage { get; private set; } = string.Empty;

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
            // if (HasPassword(manager))
            //     changePasswordHolder visible
            // else
            //     setPassword visible, changePasswordHolder hidden
            //
            // Render success message from query string "m"

            await Task.CompletedTask;
        }

        private void ChangePassword_Click(MouseEventArgs e)
        {
            // TODO: ASP.NET Core Identity — requires UserManager<ApplicationUser>/SignInManager<ApplicationUser> migration
            // Original logic:
            // if (IsValid)
            // {
            //     var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //     IdentityResult result = manager.ChangePassword(User.Identity.GetUserId(), CurrentPassword.Text, NewPassword.Text);
            //     if (result.Succeeded)
            //     {
            //         var user = manager.FindById(User.Identity.GetUserId());
            //         IdentityHelper.SignIn(manager, user, isPersistent: false);
            //         NavigationManager.NavigateTo("/Account/Manage?m=ChangePwdSuccess");
            //     }
            //     else
            //     {
            //         AddErrors(result);
            //     }
            // }
        }

        private void SetPassword_Click(MouseEventArgs e)
        {
            // TODO: ASP.NET Core Identity — requires UserManager<ApplicationUser>/SignInManager<ApplicationUser> migration
            // Original logic:
            // if (IsValid)
            // {
            //     var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //     IdentityResult result = manager.AddPassword(User.Identity.GetUserId(), password.Text);
            //     if (result.Succeeded)
            //     {
            //         NavigationManager.NavigateTo("/Account/Manage?m=SetPwdSuccess");
            //     }
            //     else
            //     {
            //         AddErrors(result);
            //     }
            // }
        }

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
