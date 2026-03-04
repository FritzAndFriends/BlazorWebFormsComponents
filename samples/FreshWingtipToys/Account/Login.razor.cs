using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;

namespace WingtipToys.Account
{
    public partial class Login : ComponentBase
    {
        [Inject] private SignInManager<IdentityUser> SignInManager { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        private string email = "";
        private string password = "";
        private string errorMessage = "";

        private async Task HandleLogin(MouseEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Email and password are required.";
                return;
            }

            var result = await SignInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                Navigation.NavigateTo("/", forceLoad: true);
            }
            else if (result.IsLockedOut)
            {
                Navigation.NavigateTo("/Account/Lockout");
            }
            else
            {
                errorMessage = "Invalid login attempt.";
            }
        }
    }
}
