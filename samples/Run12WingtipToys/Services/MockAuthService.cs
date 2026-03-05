namespace WingtipToys.Services;

public class MockAuthService
{
    private readonly Dictionary<string, string> _users = new(StringComparer.OrdinalIgnoreCase)
    {
        ["admin@wingtiptoys.com"] = "Pass@word1"
    };

    public Task<bool> AuthenticateAsync(string email, string password)
    {
        return Task.FromResult(_users.TryGetValue(email, out var stored) && stored == password);
    }

    public Task<(bool Success, string? Error)> CreateUserAsync(string email, string password)
    {
        if (_users.ContainsKey(email))
            return Task.FromResult((false, (string?)"A user with that email already exists."));
        _users[email] = password;
        return Task.FromResult((true, (string?)null));
    }
}
