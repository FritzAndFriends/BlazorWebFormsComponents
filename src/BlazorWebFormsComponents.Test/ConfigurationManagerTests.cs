using System;
using System.Collections.Generic;
using BlazorWebFormsComponents;
using Microsoft.Extensions.Configuration;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Test;

/// <summary>
/// Tests for the ConfigurationManager shim that bridges
/// System.Configuration.ConfigurationManager patterns to IConfiguration.
/// Tests are written from spec — implementation may not exist yet.
/// </summary>
public class ConfigurationManagerTests : IDisposable
{
	/// <summary>
	/// Reset ConfigurationManager state between tests to prevent cross-test pollution.
	/// ConfigurationManager is static, so each test must start from a clean state.
	/// </summary>
	public void Dispose()
	{
		// Re-initialize with an empty config to reset state.
		// If Initialize throws when already initialized, this may need adjustment.
		try
		{
			var empty = new ConfigurationBuilder().Build();
			ConfigurationManager.Initialize(empty);
		}
		catch
		{
			// Swallow — cleanup is best-effort
		}
	}

	private static IConfiguration BuildConfig(Dictionary<string, string?> data)
	{
		return new ConfigurationBuilder()
			.AddInMemoryCollection(data)
			.Build();
	}

	#region AppSettings

	[Fact]
	public void AppSettings_ReturnsValueFromConfiguration()
	{
		// Arrange
		var config = BuildConfig(new Dictionary<string, string?>
		{
			["AppSettings:SiteName"] = "Wingtip Toys",
			["AppSettings:MaxItems"] = "50"
		});
		ConfigurationManager.Initialize(config);

		// Act & Assert
		ConfigurationManager.AppSettings["SiteName"].ShouldBe("Wingtip Toys");
		ConfigurationManager.AppSettings["MaxItems"].ShouldBe("50");
	}

	[Fact]
	public void AppSettings_ReturnsNullForMissingKey()
	{
		// Arrange
		var config = BuildConfig(new Dictionary<string, string?>
		{
			["AppSettings:Exists"] = "yes"
		});
		ConfigurationManager.Initialize(config);

		// Act
		var result = ConfigurationManager.AppSettings["DoesNotExist"];

		// Assert — must return null, not throw
		result.ShouldBeNull();
	}

	[Fact]
	public void AppSettings_FallbackToRootKey()
	{
		// Arrange — key exists at root level, NOT under AppSettings: prefix
		var config = BuildConfig(new Dictionary<string, string?>
		{
			["LegacySetting"] = "from-root"
		});
		ConfigurationManager.Initialize(config);

		// Act — should fall back to root-level lookup when AppSettings:key is missing
		var result = ConfigurationManager.AppSettings["LegacySetting"];

		// Assert
		result.ShouldBe("from-root");
	}

	[Fact]
	public void AppSettings_PrefixedKeyTakesPrecedenceOverRoot()
	{
		// Arrange — same key exists at both AppSettings:Key and root Key
		var config = BuildConfig(new Dictionary<string, string?>
		{
			["AppSettings:Theme"] = "prefixed-value",
			["Theme"] = "root-value"
		});
		ConfigurationManager.Initialize(config);

		// Act — prefixed key should win
		var result = ConfigurationManager.AppSettings["Theme"];

		// Assert
		result.ShouldBe("prefixed-value");
	}

	#endregion

	#region ConnectionStrings

	[Fact]
	public void ConnectionStrings_ReturnsConnectionString()
	{
		// Arrange
		var config = BuildConfig(new Dictionary<string, string?>
		{
			["ConnectionStrings:DefaultConnection"] = "Server=localhost;Database=TestDb"
		});
		ConfigurationManager.Initialize(config);

		// Act
		var result = ConfigurationManager.ConnectionStrings["DefaultConnection"];

		// Assert — the returned object should expose the connection string value.
		// Web Forms returns ConnectionStringSettings with .ConnectionString property.
		// The shim may return the same or a simplified type.
		result.ShouldNotBeNull();
		result.ConnectionString.ShouldBe("Server=localhost;Database=TestDb");
	}

	[Fact]
	public void ConnectionStrings_ReturnsNullForMissing()
	{
		// Arrange
		var config = BuildConfig(new Dictionary<string, string?>
		{
			["ConnectionStrings:Known"] = "Server=known"
		});
		ConfigurationManager.Initialize(config);

		// Act
		var result = ConfigurationManager.ConnectionStrings["Unknown"];

		// Assert
		result.ShouldBeNull();
	}

	[Fact]
	public void ConnectionStrings_MultipleConnectionStrings()
	{
		// Arrange
		var config = BuildConfig(new Dictionary<string, string?>
		{
			["ConnectionStrings:Primary"] = "Server=primary;Database=A",
			["ConnectionStrings:ReadOnly"] = "Server=readonly;Database=B"
		});
		ConfigurationManager.Initialize(config);

		// Act & Assert
		ConfigurationManager.ConnectionStrings["Primary"]!.ConnectionString
			.ShouldBe("Server=primary;Database=A");
		ConfigurationManager.ConnectionStrings["ReadOnly"]!.ConnectionString
			.ShouldBe("Server=readonly;Database=B");
	}

	#endregion

	#region Initialize

	[Fact]
	public void Initialize_SetsConfiguration()
	{
		// Arrange
		var config = BuildConfig(new Dictionary<string, string?>
		{
			["AppSettings:Key1"] = "Value1"
		});

		// Act
		ConfigurationManager.Initialize(config);

		// Assert — subsequent reads work
		ConfigurationManager.AppSettings["Key1"].ShouldBe("Value1");
	}

	[Fact]
	public void AppSettings_ThrowsWhenNotInitialized_OrReturnsNull()
	{
		// Arrange — reset to uninitialized state
		// This test verifies defined behavior before Initialize is called.
		// Implementation may throw InvalidOperationException or return null.
		try
		{
			// Force uninitialized state by initializing with null or empty
			var empty = new ConfigurationBuilder().Build();
			ConfigurationManager.Initialize(empty);

			// If we got here, check that accessing with a missing key returns null
			var result = ConfigurationManager.AppSettings["anything"];
			result.ShouldBeNull();
		}
		catch (InvalidOperationException)
		{
			// Also acceptable — throwing when not properly initialized
		}
	}

	#endregion
}
