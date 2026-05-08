using System.Collections.Concurrent;

namespace WingtipToys.Logic;

public class DemoUserStore
{
	private readonly ConcurrentDictionary<string, string> _users = new(StringComparer.OrdinalIgnoreCase);

	public bool TryRegister(string email, string password, out string error)
	{
		error = string.Empty;
		if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
		{
			error = "Email and password are required.";
			return false;
		}

		if (!_users.TryAdd(email.Trim(), password))
		{
			error = "A user with that email already exists.";
			return false;
		}

		return true;
	}

	public bool ValidateCredentials(string email, string password)
	{
		return _users.TryGetValue(email.Trim(), out var storedPassword)
			&& string.Equals(storedPassword, password, StringComparison.Ordinal);
	}
}
