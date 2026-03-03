using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;

namespace WingtipToys.Account
{
    public partial class ResetPassword : ComponentBase
    {
        [Inject] private UserManager<IdentityUser> UserManager { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        [SupplyParameterFromQuery] private string? Code { get; set; }

        private string email = "";
        private string password = "";
        private string confirmPassword = "";
        private string errorMessage = "";

        private async Task HandleReset(MouseEventArgs args)
        {
            if (password != confirmPassword)
            {
                errorMessage = "Passwords do not match.";
                return;
            }

            var user = await UserManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                Navigation.NavigateTo("/Account/ResetPasswordConfirmation");
                return;
            }

            var result = await UserManager.ResetPasswordAsync(user, Code ?? "", password);
            if (result.Succeeded)
            {
                Navigation.NavigateTo("/Account/ResetPasswordConfirmation");
            }
            else
            {
                errorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
            }
        }
    }
}
