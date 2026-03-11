using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace WingtipToys.Account
{
    public partial class ResetPassword
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        // TODO: ASP.NET Core Identity — inject SignInManager<ApplicationUser> and UserManager<ApplicationUser>

        protected string StatusMessage { get; private set; } = string.Empty;

        private void Reset_Click(MouseEventArgs e)
        {
            // TODO: ASP.NET Core Identity — requires UserManager<ApplicationUser>/SignInManager<ApplicationUser> migration
            // Original logic:
            // string code = IdentityHelper.GetCodeFromRequest(Request);
            // if (code != null)
            // {
            //     var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //     var user = manager.FindByName(Email.Text);
            //     if (user == null)
            //     {
            //         ErrorMessage.Text = "No user found";
            //         return;
            //     }
            //     var result = manager.ResetPassword(user.Id, code, Password.Text);
            //     if (result.Succeeded)
            //     {
            //         NavigationManager.NavigateTo("/Account/ResetPasswordConfirmation");
            //         return;
            //     }
            //     ErrorMessage.Text = result.Errors.FirstOrDefault();
            //     return;
            // }
            // ErrorMessage.Text = "An error has occurred";
        }
    }
}
