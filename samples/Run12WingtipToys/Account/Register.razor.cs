using Microsoft.AspNetCore.Components;
using WingtipToys.Services;

namespace WingtipToys.Account;

public partial class Register
{
    [Inject] private MockAuthenticationStateProvider AuthState { get; set; } = default!;
    [Inject] private MockAuthService AuthService { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;

    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;
    private string _errorMessage = string.Empty;

    private async Task CreateUser_Click(EventArgs args)
    {
        var (success, error) = await AuthService.CreateUserAsync(_email, _password);
        if (success)
        {
            await AuthState.LoginAsync(_email, _password, AuthService);
            Nav.NavigateTo("/Account/Login");
        }
        else
        {
            _errorMessage = error ?? "Registration failed.";
        }
    }
}
