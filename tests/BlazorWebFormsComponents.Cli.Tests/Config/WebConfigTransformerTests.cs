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

    public void Dispose()
    {
        if (Directory.Exists(_testRoot))
            Directory.Delete(_testRoot, true);
    }
}
