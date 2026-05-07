using System.Collections.Concurrent;

namespace WingtipToys.Logic;

public sealed class RegisteredUserStore
{
    private readonly ConcurrentDictionary<string, string> _users = new(StringComparer.OrdinalIgnoreCase);

    public bool Register(string email, string password, out string error)
    {
        if (_users.ContainsKey(email))
        {
            error = "A user with that email already exists.";
            return false;
        }

        _users[email] = password;
        error = string.Empty;
        return true;
    }

    public bool Validate(string email, string password)
        => _users.TryGetValue(email, out var storedPassword)
           && string.Equals(storedPassword, password, StringComparison.Ordinal);
}
