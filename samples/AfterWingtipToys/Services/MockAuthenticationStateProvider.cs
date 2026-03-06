using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace WingtipToys.Services;

public class MockAuthenticationStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _currentUser;

    public MockAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor)
    {
        // Read auth state from the HTTP context (available at circuit start).
        // The cookie middleware has already populated HttpContext.User.
        var user = httpContextAccessor.HttpContext?.User;
        _currentUser = user?.Identity?.IsAuthenticated == true
            ? user
            : new ClaimsPrincipal(new ClaimsIdentity());
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
        => Task.FromResult(new AuthenticationState(_currentUser));
}
