using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace WingtipToys.Services;

public class MockAuthenticationStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
        => Task.FromResult(new AuthenticationState(_currentUser));

    public Task<bool> LoginAsync(string email, string password, MockAuthService authService)
    {
        return Task.Run(async () =>
        {
            if (await authService.AuthenticateAsync(email, password))
            {
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Email, email)
                }, "MockAuth");
                _currentUser = new ClaimsPrincipal(identity);
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                return true;
            }
            return false;
        });
    }

    public Task LogoutAsync()
    {
        _currentUser = new(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return Task.CompletedTask;
    }
}
