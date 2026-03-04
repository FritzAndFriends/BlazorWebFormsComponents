using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;

namespace WingtipToys.Account
{
    public partial class Forgot : ComponentBase
    {
        [Inject] private UserManager<IdentityUser> UserManager { get; set; } = default!;

        private string email = "";
        private string errorMessage = "";
        private string successMessage = "";

        private async Task HandleForgot(MouseEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                errorMessage = "Email is required.";
                return;
            }

            var user = await UserManager.FindByEmailAsync(email);
            if (user != null)
            {
                // In production, generate a token and send email
                // For demo, just show a success message
                var token = await UserManager.GeneratePasswordResetTokenAsync(user);
            }

            // Always show success to prevent email enumeration
            successMessage = "If an account with that email exists, a reset link has been sent.";
            errorMessage = "";
        }
    }
}
