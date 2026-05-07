using System.Collections.Concurrent;

namespace WingtipToys.Services;

public class SimpleUserStore
{
    private readonly ConcurrentDictionary<string, string> _users = new(StringComparer.OrdinalIgnoreCase);

    public bool TryRegister(string email, string password) => _users.TryAdd(email.Trim(), password);

    public bool Validate(string email, string password)
        => _users.TryGetValue(email.Trim(), out var storedPassword) && string.Equals(storedPassword, password, StringComparison.Ordinal);
}
