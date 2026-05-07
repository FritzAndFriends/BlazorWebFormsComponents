namespace WingtipToys.Services;

public sealed class UserAccountStore
{
    private readonly Dictionary<string, string> _users = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _sync = new();

    public bool Register(string email, string password, out string? error)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            error = "Email and password are required.";
            return false;
        }

        lock (_sync)
        {
            if (_users.ContainsKey(email))
            {
                error = "A user with that email already exists.";
                return false;
            }

            _users[email] = password;
        }

        error = null;
        return true;
    }

    public bool Validate(string email, string password)
    {
        lock (_sync)
        {
            return _users.TryGetValue(email, out var storedPassword) && storedPassword == password;
        }
    }
}
