using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

namespace WingtipToys.Account;

public partial class Login : BlazorWebFormsComponents.WebFormsPageBase
{
    [Inject] private SignInManager<IdentityUser> SignInManager { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;

    private string email = "";
    private string password = "";
    private string? errorMessage;

    private async Task LogIn()
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            errorMessage = "Email and password are required.";
            return;
        }

        var result = await SignInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            Nav.NavigateTo("/", forceLoad: true);
        }
        else
        {
            errorMessage = "Invalid login attempt";
        }
    }
}
