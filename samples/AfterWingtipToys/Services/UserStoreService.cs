using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;

namespace WingtipToys.Services;

public sealed class UserStoreService
{
    public const string CurrentUserSessionKey = "Wingtip.CurrentUserEmail";

    private static readonly ConcurrentDictionary<string, string> Users = new(StringComparer.OrdinalIgnoreCase);

    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserStoreService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetCurrentUserEmail()
    {
        return _httpContextAccessor.HttpContext?.Session.GetString(CurrentUserSessionKey);
    }

    public bool Register(string email, string password, out string? error)
    {
        if (!Users.TryAdd(email, password))
        {
            error = "That account already exists.";
            return false;
        }

        error = null;
        return true;
    }

    public bool Login(string email, string password, out string? error)
    {
        if (!Users.TryGetValue(email, out var storedPassword) || !string.Equals(storedPassword, password, StringComparison.Ordinal))
        {
            error = "Invalid email or password.";
            return false;
        }

        _httpContextAccessor.HttpContext?.Session.SetString(CurrentUserSessionKey, email);
        error = null;
        return true;
    }

    public void Logout()
    {
        _httpContextAccessor.HttpContext?.Session.Remove(CurrentUserSessionKey);
    }
}
