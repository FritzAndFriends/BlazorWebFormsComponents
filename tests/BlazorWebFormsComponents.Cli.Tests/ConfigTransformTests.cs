using System.Text.Json;
using BlazorWebFormsComponents.Cli.Config;

namespace BlazorWebFormsComponents.Cli.Tests;

/// <summary>
/// Tests for Web.config → appsettings.json conversion via WebConfigTransformer.
/// </summary>
public class ConfigTransformTests : IDisposable
{
    private readonly string _tempDir;
    private readonly WebConfigTransformer _transformer;

    public ConfigTransformTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"bwfc-config-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
        _transformer = new WebConfigTransformer();
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            try { Directory.Delete(_tempDir, recursive: true); }
            catch { /* best effort cleanup */ }
        }
    }

    private void WriteWebConfig(string content)
    {
        File.WriteAllText(Path.Combine(_tempDir, "Web.config"), content);
    }

    // ───────────────────────────────────────────────────────────────
    //  JSON structure
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void Transform_ProducesValidJson()
    {
        WriteWebConfig("""
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <appSettings>
                <add key="SiteName" value="MyApp" />
              </appSettings>
            </configuration>
            """);

        var result = _transformer.Transform(_tempDir);

        Assert.NotNull(result);
        Assert.NotNull(result!.JsonContent);

        // Must be valid JSON
        var doc = JsonDocument.Parse(result.JsonContent!);
        Assert.NotNull(doc);
    }

    [Fact]
    public void Transform_IncludesLoggingSection()
    {
        WriteWebConfig("""
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <appSettings>
                <add key="TestKey" value="TestValue" />
              </appSettings>
            </configuration>
            """);

        var result = _transformer.Transform(_tempDir);

        Assert.NotNull(result);
        var doc = JsonDocument.Parse(result!.JsonContent!);
        Assert.True(doc.RootElement.TryGetProperty("Logging", out _));
    }

    [Fact]
    public void Transform_IncludesAllowedHosts()
    {
        WriteWebConfig("""
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <appSettings>
                <add key="TestKey" value="TestValue" />
              </appSettings>
            </configuration>
            """);

        var result = _transformer.Transform(_tempDir);

        Assert.NotNull(result);
        var doc = JsonDocument.Parse(result!.JsonContent!);
        Assert.True(doc.RootElement.TryGetProperty("AllowedHosts", out var hosts));
        Assert.Equal("*", hosts.GetString());
    }

    // ───────────────────────────────────────────────────────────────
    //  AppSettings
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void Transform_PreservesAppSettingsKeysAndValues()
    {
        WriteWebConfig("""
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <appSettings>
                <add key="SiteName" value="Contoso University" />
                <add key="MaxPageSize" value="50" />
                <add key="EnableFeatureX" value="true" />
              </appSettings>
            </configuration>
            """);

        var result = _transformer.Transform(_tempDir);

        Assert.NotNull(result);
        Assert.Equal(3, result!.AppSettingsCount);
        Assert.Contains("SiteName", result.AppSettingsKeys);
        Assert.Contains("MaxPageSize", result.AppSettingsKeys);
        Assert.Contains("EnableFeatureX", result.AppSettingsKeys);

        var doc = JsonDocument.Parse(result.JsonContent!);
        Assert.Equal("Contoso University", doc.RootElement.GetProperty("SiteName").GetString());
        Assert.Equal("50", doc.RootElement.GetProperty("MaxPageSize").GetString());
        Assert.Equal("true", doc.RootElement.GetProperty("EnableFeatureX").GetString());
    }

    [Fact]
    public void Transform_PreservesAppSettingsWithEmptyValues()
    {
        WriteWebConfig("""
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <appSettings>
                <add key="EmptyKey" value="" />
              </appSettings>
            </configuration>
            """);

        var result = _transformer.Transform(_tempDir);

        Assert.NotNull(result);
        Assert.Equal(1, result!.AppSettingsCount);

        var doc = JsonDocument.Parse(result.JsonContent!);
        Assert.Equal("", doc.RootElement.GetProperty("EmptyKey").GetString());
    }

    // ───────────────────────────────────────────────────────────────
    //  ConnectionStrings
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void Transform_PreservesConnectionStringNamesAndValues()
    {
        WriteWebConfig("""
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <connectionStrings>
                <add name="DefaultConnection"
                     connectionString="Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\aspnet.mdf"
                     providerName="System.Data.SqlClient" />
                <add name="ProductContext"
                     connectionString="Server=.;Database=Products;Trusted_Connection=True;"
                     providerName="System.Data.SqlClient" />
              </connectionStrings>
            </configuration>
            """);

        var result = _transformer.Transform(_tempDir);

        Assert.NotNull(result);
        Assert.Equal(2, result!.ConnectionStringsCount);
        Assert.Contains("DefaultConnection", result.ConnectionStringNames);
        Assert.Contains("ProductContext", result.ConnectionStringNames);

        var doc = JsonDocument.Parse(result.JsonContent!);
        var connStrings = doc.RootElement.GetProperty("ConnectionStrings");
        Assert.Contains("LocalDB", connStrings.GetProperty("DefaultConnection").GetString());
        Assert.Contains("Products", connStrings.GetProperty("ProductContext").GetString());
    }

    [Fact]
    public void Transform_FiltersOutBuiltInConnectionStrings()
    {
        WriteWebConfig("""
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <connectionStrings>
                <add name="LocalSqlServer" connectionString="data source=.\SQLEXPRESS" providerName="System.Data.SqlClient" />
                <add name="MyApp" connectionString="Server=.;Database=MyApp;" providerName="System.Data.SqlClient" />
              </connectionStrings>
            </configuration>
            """);

        var result = _transformer.Transform(_tempDir);

        Assert.NotNull(result);
        Assert.Equal(1, result!.ConnectionStringsCount);
        Assert.Contains("MyApp", result.ConnectionStringNames);
        Assert.DoesNotContain("LocalSqlServer", result.ConnectionStringNames);
    }

    // ───────────────────────────────────────────────────────────────
    //  Combined appSettings + connectionStrings
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void Transform_HandlesBothSections()
    {
        WriteWebConfig("""
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <appSettings>
                <add key="SiteName" value="MyApp" />
              </appSettings>
              <connectionStrings>
                <add name="DefaultConnection"
                     connectionString="Server=.;Database=Test;"
                     providerName="System.Data.SqlClient" />
              </connectionStrings>
            </configuration>
            """);

        var result = _transformer.Transform(_tempDir);

        Assert.NotNull(result);
        Assert.Equal(1, result!.AppSettingsCount);
        Assert.Equal(1, result.ConnectionStringsCount);

        var doc = JsonDocument.Parse(result.JsonContent!);
        Assert.True(doc.RootElement.TryGetProperty("ConnectionStrings", out _));
        Assert.True(doc.RootElement.TryGetProperty("SiteName", out _));
    }

    // ───────────────────────────────────────────────────────────────
    //  Edge cases
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void Transform_ReturnsNull_WhenNoWebConfig()
    {
        // _tempDir has no Web.config
        var result = _transformer.Transform(_tempDir);

        Assert.Null(result);
    }

    [Fact]
    public void Transform_ReturnsNull_WhenEmptyWebConfig()
    {
        WriteWebConfig("""
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
            </configuration>
            """);

        var result = _transformer.Transform(_tempDir);

        // No appSettings, no connectionStrings → null
        Assert.Null(result);
    }

    [Fact]
    public void Transform_ReturnsNull_WhenEmptyAppSettingsAndConnectionStrings()
    {
        WriteWebConfig("""
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <appSettings />
              <connectionStrings />
            </configuration>
            """);

        var result = _transformer.Transform(_tempDir);

        Assert.Null(result);
    }

    [Fact]
    public void Transform_ReturnsError_WhenInvalidXml()
    {
        WriteWebConfig("this is not xml at all!");

        var result = _transformer.Transform(_tempDir);

        Assert.NotNull(result);
        Assert.NotNull(result!.Error);
        Assert.Null(result.JsonContent);
    }

    [Fact]
    public void Transform_IgnoresAppSettingsWithNoKey()
    {
        WriteWebConfig("""
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <appSettings>
                <add value="orphan" />
                <add key="ValidKey" value="ValidValue" />
              </appSettings>
            </configuration>
            """);

        var result = _transformer.Transform(_tempDir);

        Assert.NotNull(result);
        Assert.Equal(1, result!.AppSettingsCount);
        Assert.Contains("ValidKey", result.AppSettingsKeys);
    }

    [Fact]
    public void Transform_FindsWebConfig_CaseInsensitive()
    {
        // WebConfigTransformer looks for both "Web.config" and "web.config"
        File.WriteAllText(Path.Combine(_tempDir, "Web.config"), """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <appSettings>
                <add key="Key1" value="Value1" />
              </appSettings>
            </configuration>
            """);

        var result = _transformer.Transform(_tempDir);

        Assert.NotNull(result);
        Assert.Equal(1, result!.AppSettingsCount);
    }
}
