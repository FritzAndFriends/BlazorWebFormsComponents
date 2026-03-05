using Microsoft.AspNetCore.Components;
using WingtipToys.Services;

namespace WingtipToys.Account;

public partial class Login
{
    [Inject] private MockAuthenticationStateProvider AuthState { get; set; } = default!;
    [Inject] private MockAuthService AuthService { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;

    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _failureText = string.Empty;
    private bool _errorVisible = false;

    private async Task LogIn(EventArgs args)
    {
        var success = await AuthState.LoginAsync(_email, _password, AuthService);
        if (success)
        {
            Nav.NavigateTo("/");
        }
        else
        {
            _failureText = "Invalid login attempt.";
            _errorVisible = true;
        }
    }
}
