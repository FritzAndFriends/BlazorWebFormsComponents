using Microsoft.AspNetCore.Http;

namespace BlazorWebFormsComponents;

/// <summary>
/// A no-op <see cref="IResponseCookies"/> returned when <see cref="HttpContext"/>
/// is unavailable (e.g., during interactive WebSocket rendering).
/// <c>Append</c> and <c>Delete</c> silently do nothing. The owning
/// <see cref="ResponseShim"/> logs a warning on first access.
/// </summary>
internal sealed class NullResponseCookies : IResponseCookies
{
	public static readonly NullResponseCookies Instance = new();

	public void Append(string key, string value) { }
	public void Append(string key, string value, CookieOptions options) { }
	public void Delete(string key) { }
	public void Delete(string key, CookieOptions options) { }
}
