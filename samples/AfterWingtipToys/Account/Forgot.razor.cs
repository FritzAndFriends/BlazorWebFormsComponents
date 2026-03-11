using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace WingtipToys.Account
{
    public partial class Forgot
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        // TODO: ASP.NET Core Identity — inject SignInManager<ApplicationUser> and UserManager<ApplicationUser>

        protected override async Task OnInitializedAsync()
        {
            await Task.CompletedTask;
        }

        private void Forgot_Click(MouseEventArgs e)
        {
            // TODO: ASP.NET Core Identity — requires UserManager<ApplicationUser>/SignInManager<ApplicationUser> migration
            // Original logic:
            // if (IsValid)
            // {
            //     var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //     ApplicationUser user = manager.FindByName(Email.Text);
            //     if (user == null || !manager.IsEmailConfirmed(user.Id))
            //     {
            //         FailureText.Text = "The user either does not exist or is not confirmed.";
            //         ErrorMessage.Visible = true;
            //         return;
            //     }
            //     // Send email with the code and the redirect to reset password page
            //     // string code = manager.GeneratePasswordResetToken(user.Id);
            //     // string callbackUrl = IdentityHelper.GetResetPasswordRedirectUrl(code, Request);
            //     // manager.SendEmail(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>.");
            //     loginForm.Visible = false;
            //     DisplayEmail.Visible = true;
            // }
        }
    }
}
