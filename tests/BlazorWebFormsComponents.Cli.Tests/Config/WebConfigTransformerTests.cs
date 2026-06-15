using System.Text.Json;
using BlazorWebFormsComponents.Cli.Config;

namespace BlazorWebFormsComponents.Cli.Tests.Config;

public sealed class WebConfigTransformerTests : IDisposable
{
    private readonly string _testRoot = Path.Combine(AppContext.BaseDirectory, "TestArtifacts", $"webconfig-{Guid.NewGuid():N}");

    public WebConfigTransformerTests()
    {
        Directory.CreateDirectory(_testRoot);
    }

    [Fact]
    public void Transform_NormalizesAttachDbFilenameConnectionStrings()
    {
        File.WriteAllText(Path.Combine(_testRoot, "Web.config"), """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <connectionStrings>
                <add name="WingtipToys" connectionString="Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\wingtiptoys.mdf;Integrated Security=True" />
              </connectionStrings>
            </configuration>
            """);

        var transformer = new WebConfigTransformer();

        var result = transformer.Transform(_testRoot);

        Assert.NotNull(result);
        using var json = JsonDocument.Parse(result!.JsonContent!);
        var connectionString = json.RootElement
            .GetProperty("ConnectionStrings")
            .GetProperty("WingtipToys")
            .GetString();

        Assert.Equal("Data Source=(LocalDB)\\v11.0;Initial Catalog=WingtipToys;Integrated Security=True", connectionString);
        Assert.DoesNotContain("AttachDbFilename", connectionString, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Transform_UnwrapsEf6MetadataConnectionStrings()
    {
        File.WriteAllText(Path.Combine(_testRoot, "Web.config"), """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <connectionStrings>
                <add name="ContosoUniversityEntities" connectionString="metadata=res://*/Models.Model1.csdl|res://*/Models.Model1.ssdl|res://*/Models.Model1.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=(LocalDB)\v11.0;initial catalog=ContosoUniversity;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" providerName="System.Data.EntityClient" />
              </connectionStrings>
            </configuration>
            """);

        var transformer = new WebConfigTransformer();

        var result = transformer.Transform(_testRoot);

        Assert.NotNull(result);
        using var json = JsonDocument.Parse(result!.JsonContent!);
        var connectionString = json.RootElement
            .GetProperty("ConnectionStrings")
            .GetProperty("ContosoUniversityEntities")
            .GetString();

        // Should contain the inner SQL connection string, not the EF6 metadata wrapper
        Assert.DoesNotContain("metadata=", connectionString, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("provider connection string", connectionString, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("data source=", connectionString, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("initial catalog=ContosoUniversity", connectionString, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Transform_PlainConnectionString_NotAffectedByEf6Unwrap()
    {
        File.WriteAllText(Path.Combine(_testRoot, "Web.config"), """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <connectionStrings>
                <add name="MyDb" connectionString="Data Source=.;Initial Catalog=MyDb;Integrated Security=True" />
              </connectionStrings>
            </configuration>
            """);

        var transformer = new WebConfigTransformer();

        var result = transformer.Transform(_testRoot);

        Assert.NotNull(result);
        using var json = JsonDocument.Parse(result!.JsonContent!);
        var connectionString = json.RootElement
            .GetProperty("ConnectionStrings")
            .GetProperty("MyDb")
            .GetString();

        Assert.Equal("Data Source=.;Initial Catalog=MyDb;Integrated Security=True", connectionString);
    }

    [Fact]
    public void Transform_DuplicateAppSettingKeys_LastValueWins()
    {
        File.WriteAllText(Path.Combine(_testRoot, "Web.config"), """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <appSettings>
                <add key="FeatureFlag" value="false" />
                <add key="FeatureFlag" value="true" />
              </appSettings>
            </configuration>
            """);

        var transformer = new WebConfigTransformer();

        var result = transformer.Transform(_testRoot);

        Assert.NotNull(result);
        using var json = JsonDocument.Parse(result!.JsonContent!);
        var featureFlag = json.RootElement.GetProperty("FeatureFlag").GetString();
        Assert.Equal("true", featureFlag);
        Assert.Equal(1, result.AppSettingsCount);
    }

    [Fact]
    public void Transform_IgnoresIncompleteConnectionStringEntries()
    {
        File.WriteAllText(Path.Combine(_testRoot, "Web.config"), """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <connectionStrings>
                <add name="MissingConnectionString" providerName="System.Data.SqlClient" />
                <add connectionString="Server=.;Database=MissingName;" providerName="System.Data.SqlClient" />
                <add name="ValidConnection" connectionString="Server=.;Database=ValidDb;" providerName="System.Data.SqlClient" />
              </connectionStrings>
            </configuration>
            """);

        var transformer = new WebConfigTransformer();

        var result = transformer.Transform(_testRoot);

        Assert.NotNull(result);
        Assert.Equal(1, result!.ConnectionStringsCount);
        Assert.Contains("ValidConnection", result.ConnectionStringNames);
        Assert.DoesNotContain("MissingConnectionString", result.ConnectionStringNames);
    }

    [Fact(Skip = "Blocked by #557: Web.config parser should support default-namespace XML sections.")]
    public void Transform_ParsesNamespacedConfigurationSections()
    {
        File.WriteAllText(Path.Combine(_testRoot, "Web.config"), """
            <?xml version="1.0" encoding="utf-8"?>
            <configuration xmlns="urn:test:webconfig">
              <appSettings>
                <add key="SiteName" value="NamespacedApp" />
              </appSettings>
              <connectionStrings>
                <add name="DefaultConnection" connectionString="Server=.;Database=Namespaced;" providerName="System.Data.SqlClient" />
              </connectionStrings>
            </configuration>
            """);

        var transformer = new WebConfigTransformer();

        var result = transformer.Transform(_testRoot);

        Assert.NotNull(result);
        Assert.Equal(1, result!.AppSettingsCount);
        Assert.Equal(1, result.ConnectionStringsCount);
        using var json = JsonDocument.Parse(result.JsonContent!);
        Assert.Equal("NamespacedApp", json.RootElement.GetProperty("SiteName").GetString());
        Assert.Equal("Server=.;Database=Namespaced;", json.RootElement.GetProperty("ConnectionStrings").GetProperty("DefaultConnection").GetString());
    }

    public void Dispose()
    {
        if (Directory.Exists(_testRoot))
            Directory.Delete(_testRoot, true);
    }
}
