using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

namespace WingtipToys.Account;

public partial class Register : BlazorWebFormsComponents.WebFormsPageBase
{
    [Inject] private UserManager<IdentityUser> UserManager { get; set; } = default!;
    [Inject] private SignInManager<IdentityUser> SignInManager { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;

    private string email = "";
    private string password = "";
    private string confirmPassword = "";
    private string? errorMessage;

    private async Task CreateUser_Click()
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            errorMessage = "Email and password are required.";
            return;
        }

        if (password != confirmPassword)
        {
            errorMessage = "The password and confirmation password do not match.";
            return;
        }

        var user = new IdentityUser { UserName = email, Email = email };
        var result = await UserManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await SignInManager.SignInAsync(user, isPersistent: false);
            Nav.NavigateTo("/", forceLoad: true);
        }
        else
        {
            errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
        }
    }
}
