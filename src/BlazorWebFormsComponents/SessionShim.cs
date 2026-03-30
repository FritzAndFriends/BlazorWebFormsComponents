using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BlazorWebFormsComponents;

/// <summary>
/// Drop-in replacement for <c>HttpSessionState</c>. Provides dictionary-style
/// <c>Session["key"]</c> access backed by ASP.NET Core <see cref="ISession"/>
/// with JSON serialization.
/// Falls back to an in-memory <see cref="ConcurrentDictionary{TKey,TValue}"/>
/// when <see cref="IHttpContextAccessor"/> is unavailable (e.g., in interactive
/// Blazor Server mode where there is no HTTP context).
/// <para>
/// Register as a scoped service so each user/circuit gets its own instance.
/// </para>
/// </summary>
public class SessionShim
{
	private readonly IHttpContextAccessor? _httpContextAccessor;
	private readonly ILogger<SessionShim> _logger;
	private readonly ConcurrentDictionary<string, object?> _fallbackStore = new();
	private bool _fallbackWarningLogged;

	/// <summary>
	/// Initializes a new instance of the <see cref="SessionShim"/> class.
	/// </summary>
	/// <param name="logger">Logger for diagnostics.</param>
	/// <param name="httpContextAccessor">
	/// Optional accessor for the current HTTP context. When the context or its
	/// session is unavailable, the shim degrades to in-memory storage.
	/// </param>
	public SessionShim(ILogger<SessionShim> logger, IHttpContextAccessor? httpContextAccessor = null)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_httpContextAccessor = httpContextAccessor;
	}

	/// <summary>
	/// Gets or sets a session value by key, emulating
	/// <c>Session["key"]</c> from ASP.NET Web Forms.
	/// Values are stored in the in-memory fallback dictionary as the primary store
	/// and optionally synced to <see cref="ISession"/> for cross-request persistence.
	/// This "fallback-first" design ensures reliable operation in interactive
	/// Blazor Server mode where ISession may be unreliable (no active HTTP pipeline).
	/// </summary>
	/// <param name="key">The session key.</param>
	/// <returns>The stored value, or <c>null</c> if the key is not found.</returns>
	public object? this[string key]
	{
		get
		{
			// Primary: in-memory store (always up-to-date in current scope/circuit)
			if (_fallbackStore.TryGetValue(key, out var fallbackValue))
				return fallbackValue;

			// Secondary: hydrate from ISession (cross-request persistence)
			if (TryGetSession(out var session))
			{
				try
				{
					var json = session.GetString(key);
					if (json is null) return null;
					var value = JsonSerializer.Deserialize<object>(json);
					_fallbackStore[key] = value;
					return value;
				}
				catch (Exception ex)
				{
					_logger.LogDebug(ex, "SessionShim: ISession read failed for key '{Key}'.", key);
				}
			}

			return null;
		}
		set
		{
			// Always store in primary (in-memory)
			if (value is null)
				_fallbackStore.TryRemove(key, out _);
			else
				_fallbackStore[key] = value;

			// Best-effort sync to ISession for cross-request persistence
			if (TryGetSession(out var session))
			{
				try
				{
					if (value is null)
						session.Remove(key);
					else
						session.SetString(key, JsonSerializer.Serialize(value));
				}
				catch (Exception ex)
				{
					_logger.LogDebug(ex, "SessionShim: ISession write failed for key '{Key}'.", key);
				}
			}
		}
	}

	/// <summary>
	/// Gets a strongly-typed value from session storage.
	/// </summary>
	/// <typeparam name="T">The expected type of the stored value.</typeparam>
	/// <param name="key">The session key.</param>
	/// <returns>The deserialized value, or <c>default</c> if the key is not found.</returns>
	public T? Get<T>(string key)
	{
		// Primary: in-memory store
		if (_fallbackStore.TryGetValue(key, out var value))
		{
			if (value is T typed) return typed;
			if (value is JsonElement element)
				return JsonSerializer.Deserialize<T>(element.GetRawText());
			if (value is null) return default;

			// Attempt conversion via JSON round-trip for other types
			var serialized = JsonSerializer.Serialize(value);
			return JsonSerializer.Deserialize<T>(serialized);
		}

		// Secondary: hydrate from ISession
		if (TryGetSession(out var session))
		{
			try
			{
				var json = session.GetString(key);
				if (json is null) return default;
				var result = JsonSerializer.Deserialize<T>(json);
				_fallbackStore[key] = result;
				return result;
			}
			catch (Exception ex)
			{
				_logger.LogDebug(ex, "SessionShim: ISession typed read failed for key '{Key}'.", key);
			}
		}

		return default;
	}

	/// <summary>
	/// Removes the value associated with the specified key.
	/// </summary>
	/// <param name="key">The session key to remove.</param>
	public void Remove(string key)
	{
		_fallbackStore.TryRemove(key, out _);

		if (TryGetSession(out var session))
		{
			try { session.Remove(key); }
			catch (Exception ex)
			{
				_logger.LogDebug(ex, "SessionShim: ISession remove failed for key '{Key}'.", key);
			}
		}
	}

	/// <summary>
	/// Removes all keys and values from the session.
	/// </summary>
	public void Clear()
	{
		_fallbackStore.Clear();

		if (TryGetSession(out var session))
		{
			try { session.Clear(); }
			catch (Exception ex)
			{
				_logger.LogDebug(ex, "SessionShim: ISession clear failed.");
			}
		}
	}

	/// <summary>
	/// Determines whether the session contains the specified key.
	/// </summary>
	/// <param name="key">The key to check.</param>
	/// <returns><c>true</c> if the key exists; otherwise <c>false</c>.</returns>
	public bool ContainsKey(string key)
	{
		if (_fallbackStore.ContainsKey(key))
			return true;

		if (TryGetSession(out var session))
		{
			try { return session.Keys.Any(k => k == key); }
			catch { return false; }
		}

		return false;
	}

	/// <summary>
	/// Gets the number of items in the session.
	/// </summary>
	public int Count => _fallbackStore.Count;

	/// <summary>
	/// Attempts to get the ASP.NET Core <see cref="ISession"/> from the
	/// current HTTP context. Returns <c>false</c> and logs a one-time
	/// warning when the session is unavailable (interactive/WebSocket mode).
	/// </summary>
	private bool TryGetSession(out ISession session)
	{
		session = null!;
		var httpContext = _httpContextAccessor?.HttpContext;

		if (httpContext is null)
		{
			LogFallbackWarning();
			return false;
		}

		try
		{
			session = httpContext.Session;
			return true;
		}
		catch (InvalidOperationException)
		{
			// Session middleware not configured or session unavailable
			LogFallbackWarning();
			return false;
		}
	}

	private void LogFallbackWarning()
	{
		if (_fallbackWarningLogged) return;
		_fallbackWarningLogged = true;

		_logger.LogWarning(
			"SessionShim: ISession is unavailable (interactive/WebSocket mode). " +
			"Using in-memory fallback storage scoped to this circuit. " +
			"Add session middleware for HTTP-backed session persistence.");
	}
}
