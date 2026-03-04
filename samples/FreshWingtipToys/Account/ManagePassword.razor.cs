using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;

namespace WingtipToys.Account
{
    public partial class ManagePassword : ComponentBase
    {
        [Inject] private UserManager<IdentityUser> UserManager { get; set; } = default!;
        [Inject] private SignInManager<IdentityUser> SignInManager { get; set; } = default!;

        private string currentPassword = "";
        private string newPassword = "";
        private string confirmNewPassword = "";
        private string errorMessage = "";
        private string successMessage = "";

        private async Task HandleChangePassword(MouseEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                errorMessage = "New password is required.";
                return;
            }

            if (newPassword != confirmNewPassword)
            {
                errorMessage = "The new password and confirmation password do not match.";
                return;
            }

            var user = await UserManager.GetUserAsync(SignInManager.Context.User);
            if (user == null)
            {
                errorMessage = "Unable to load user.";
                return;
            }

            var result = await UserManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                await SignInManager.RefreshSignInAsync(user);
                successMessage = "Your password has been changed.";
                errorMessage = "";
                currentPassword = "";
                newPassword = "";
                confirmNewPassword = "";
            }
            else
            {
                errorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
            }
        }
    }
}
