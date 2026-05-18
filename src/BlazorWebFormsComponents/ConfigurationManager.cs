using System;
using System.Collections.Specialized;
using Microsoft.Extensions.Configuration;

namespace BlazorWebFormsComponents;

/// <summary>
/// Shim for <c>System.Configuration.ConfigurationManager</c>.
/// Call <see cref="Initialize"/> in Program.cs (or use
/// <see cref="ServiceCollectionExtensions.UseConfigurationManagerShim"/>)
/// to enable <c>AppSettings["key"]</c> and <c>ConnectionStrings["name"]</c>
/// access from migrated Web Forms BLL / DAL code.
/// <para>
/// Lives in the <c>BlazorWebFormsComponents</c> namespace so the .targets
/// global using shadows <c>System.Configuration.ConfigurationManager</c>
/// without requiring a type alias.
/// </para>
/// </summary>
public static class ConfigurationManager
{
	private static IConfiguration? _configuration;

	/// <summary>
	/// Binds the shim to an ASP.NET Core <see cref="IConfiguration"/> instance.
	/// Typically called once during application startup.
	/// </summary>
	public static void Initialize(IConfiguration configuration)
	{
		_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
	}

	/// <summary>
	/// Provides <c>ConfigurationManager.AppSettings["key"]</c> access.
	/// Reads from <c>IConfiguration["AppSettings:{key}"]</c> first, falling
	/// back to <c>IConfiguration[key]</c>.
	/// </summary>
	public static AppSettingsCollection AppSettings => new(_configuration);

	/// <summary>
	/// Provides <c>ConfigurationManager.ConnectionStrings["name"]</c> access.
	/// Reads from <c>IConfiguration.GetConnectionString(name)</c>.
	/// </summary>
	public static ConnectionStringSettingsCollection ConnectionStrings => new(_configuration);
}

/// <summary>
/// Indexer that emulates <c>NameValueCollection</c>-style access for app settings.
/// </summary>
public sealed class AppSettingsCollection
{
	private readonly IConfiguration? _config;

	internal AppSettingsCollection(IConfiguration? config) => _config = config;

	/// <summary>
	/// Gets the setting value by key. Tries <c>AppSettings:{key}</c> first,
	/// then falls back to <c>{key}</c> directly.
	/// Returns <c>null</c> when the key is not found or configuration is not initialized.
	/// </summary>
	public string? this[string key]
	{
		get
		{
			if (_config is null) return null;
			return _config[$"AppSettings:{key}"] ?? _config[key];
		}
	}
}

/// <summary>
/// Indexer that emulates <c>ConnectionStringSettingsCollection</c> from
/// <c>System.Configuration</c>.
/// </summary>
public sealed class ConnectionStringSettingsCollection
{
	private readonly IConfiguration? _config;

	internal ConnectionStringSettingsCollection(IConfiguration? config) => _config = config;

	/// <summary>
	/// Gets a <see cref="ConnectionStringSettings"/> by name.
	/// Returns <c>null</c> when the name is not found or configuration is not initialized.
	/// </summary>
	public ConnectionStringSettings? this[string name]
	{
		get
		{
			if (_config is null) return null;
			var cs = _config.GetConnectionString(name);
			if (cs is null) return null;
			return new ConnectionStringSettings(name, cs);
		}
	}
}

/// <summary>
/// Minimal emulation of <c>System.Configuration.ConnectionStringSettings</c>.
/// </summary>
public sealed class ConnectionStringSettings
{
	public ConnectionStringSettings(string name, string connectionString, string? providerName = null)
	{
		Name = name;
		ConnectionString = connectionString;
		ProviderName = providerName ?? string.Empty;
	}

	/// <summary>Connection string name.</summary>
	public string Name { get; set; }

	/// <summary>The connection string value.</summary>
	public string ConnectionString { get; set; }

	/// <summary>ADO.NET provider name (e.g. "System.Data.SqlClient").</summary>
	public string ProviderName { get; set; }
}
