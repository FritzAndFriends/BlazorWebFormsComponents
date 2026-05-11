using BlazorWebFormsComponents.Cli.Config;
using BlazorWebFormsComponents.Cli.Scaffolding;

namespace BlazorWebFormsComponents.Cli.Tests.Scaffolding;

public class ProgramCsEmitterTests
{
    private static DatabaseProviderInfo DefaultDbProvider => new()
    {
        ProviderMethod = "UseSqlServer",
        PackageName = "Microsoft.EntityFrameworkCore.SqlServer",
        DetectedFrom = "test",
        ConnectionString = "Server=.;Database=Test"
    };

    [Fact]
    public void Generate_WhenIdentityEnabled_EmitsAuthEndpoints()
    {
        var emitter = new ProgramCsEmitter();
        var profile = new RuntimeProfile
        {
            NeedsIdentity = true,
            NeedsEntityFramework = true
        };

        var result = emitter.Generate("TestApp", profile, DefaultDbProvider);

        Assert.Contains("PerformLogin", result);
        Assert.Contains("RegisterHandler", result);
        Assert.Contains("PerformLogout", result);
    }

    [Fact]
    public void Generate_WhenIdentityDisabled_DoesNotEmitAuthEndpoints()
    {
        var emitter = new ProgramCsEmitter();

        var result = emitter.Generate("TestApp", new RuntimeProfile(), DefaultDbProvider);

        Assert.DoesNotContain("PerformLogin", result);
        Assert.DoesNotContain("RegisterHandler", result);
        Assert.DoesNotContain("PerformLogout", result);
    }
}
