using System.Collections.Concurrent;

namespace WingtipToys.Services;

public class UserStoreService
{
    public const string CurrentUserSessionKey = "WingtipCurrentUserEmail";

    private readonly ConcurrentDictionary<string, string> _users = new(StringComparer.OrdinalIgnoreCase);

    public bool TryRegister(string email, string password)
    {
        return _users.TryAdd(email, password);
    }

    public bool ValidateCredentials(string email, string password)
    {
        return _users.TryGetValue(email, out var storedPassword) &&
               string.Equals(storedPassword, password, StringComparison.Ordinal);
    }
}
