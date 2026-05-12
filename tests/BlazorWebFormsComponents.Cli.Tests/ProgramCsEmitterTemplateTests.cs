using BlazorWebFormsComponents.Cli.Config;
using BlazorWebFormsComponents.Cli.Scaffolding;

namespace BlazorWebFormsComponents.Cli.Tests;

public class WebConfigRuntimeSignalDetectorTests : IDisposable
{
    private readonly string _tempDir;

    public WebConfigRuntimeSignalDetectorTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"bwfc-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    [Fact]
    public void DetectsFormsAuthentication_AndLoginUrl()
    {
        File.WriteAllText(Path.Combine(_tempDir, "Web.config"), """
            <?xml version="1.0"?>
            <configuration>
              <system.web>
                <authentication mode="Forms">
                  <forms loginUrl="~/Account/Login.aspx" timeout="2880" />
                </authentication>
              </system.web>
            </configuration>
            """);

        var detector = new WebConfigRuntimeSignalDetector();
        var profile = new RuntimeProfile();
        detector.Apply(_tempDir, profile);

        Assert.True(profile.NeedsIdentity);
        Assert.Equal("Forms", profile.AuthenticationMode);
        Assert.Equal("/Account/Login.aspx", profile.AuthLoginPath);
    }

    [Fact]
    public void DetectsCustomErrors_DefaultRedirect()
    {
        File.WriteAllText(Path.Combine(_tempDir, "Web.config"), """
            <?xml version="1.0"?>
            <configuration>
              <system.web>
                <customErrors mode="RemoteOnly" defaultRedirect="~/ErrorPage.aspx" />
              </system.web>
            </configuration>
            """);

        var detector = new WebConfigRuntimeSignalDetector();
        var profile = new RuntimeProfile();
        detector.Apply(_tempDir, profile);

        Assert.Equal("/ErrorPage.aspx", profile.CustomErrorRedirect);
    }

    [Fact]
    public void DetectsSessionState_InProcess()
    {
        File.WriteAllText(Path.Combine(_tempDir, "Web.config"), """
            <?xml version="1.0"?>
            <configuration>
              <system.web>
                <sessionState mode="InProc" customProvider="DefaultSessionProvider" />
              </system.web>
            </configuration>
            """);

        var detector = new WebConfigRuntimeSignalDetector();
        var profile = new RuntimeProfile();
        detector.Apply(_tempDir, profile);

        Assert.True(profile.NeedsSession);
    }

    [Fact]
    public void IgnoresSessionState_Off()
    {
        File.WriteAllText(Path.Combine(_tempDir, "Web.config"), """
            <?xml version="1.0"?>
            <configuration>
              <system.web>
                <sessionState mode="Off" />
              </system.web>
            </configuration>
            """);

        var detector = new WebConfigRuntimeSignalDetector();
        var profile = new RuntimeProfile();
        detector.Apply(_tempDir, profile);

        Assert.False(profile.NeedsSession);
    }

    [Fact]
    public void NoWebConfig_DoesNothing()
    {
        var detector = new WebConfigRuntimeSignalDetector();
        var profile = new RuntimeProfile();
        detector.Apply(_tempDir, profile);

        Assert.False(profile.NeedsIdentity);
        Assert.False(profile.NeedsSession);
        Assert.Null(profile.AuthenticationMode);
        Assert.Null(profile.AuthLoginPath);
        Assert.Null(profile.CustomErrorRedirect);
    }
}

public class ProgramCsEmitterTemplateTests
{
    private static DatabaseProviderInfo DefaultDbProvider => new()
    {
        ProviderMethod = "UseSqlServer",
        PackageName = "Microsoft.EntityFrameworkCore.SqlServer",
        DetectedFrom = "test",
        ConnectionString = "Server=.;Database=Test"
    };

    [Fact]
    public void MinimalApp_MatchesBlazorBaseline()
    {
        var emitter = new ProgramCsEmitter();
        var profile = new RuntimeProfile();

        var result = emitter.Generate("TestApp", profile, DefaultDbProvider);

        // Standard Blazor baseline elements
        Assert.Contains("var builder = WebApplication.CreateBuilder(args);", result);
        Assert.Contains("AddRazorComponents()", result);
        Assert.DoesNotContain("AddInteractiveServerComponents()", result);
        Assert.Contains("var app = builder.Build();", result);
        Assert.Contains("app.UseHttpsRedirection();", result);
        Assert.Contains("app.MapStaticAssets();", result);
        Assert.Contains("app.UseAntiforgery();", result);
        Assert.Contains("MapRazorComponents<TestApp.Components.App>()", result);
        Assert.Contains("app.Run();", result);
        Assert.Contains("AddBlazorWebFormsComponents()", result);

        // Should NOT contain feature-specific code when nothing is detected
        Assert.DoesNotContain("AddDbContext", result);
        Assert.DoesNotContain("AddSession", result);
        Assert.DoesNotContain("AddDefaultIdentity", result);
        Assert.DoesNotContain("EnsureCreated", result);
    }

    [Fact]
    public void EmitsServicesComment_BeforeRazorComponents()
    {
        var emitter = new ProgramCsEmitter();
        var profile = new RuntimeProfile();

        var result = emitter.Generate("TestApp", profile, DefaultDbProvider);

        Assert.Contains("// Add services to the container.", result);
    }

    [Fact]
    public void EmitsHttpContextAccessor_WhenSessionOrIdentityDetected()
    {
        var emitter = new ProgramCsEmitter();
        var profile = new RuntimeProfile { NeedsSession = true };

        var result = emitter.Generate("TestApp", profile, DefaultDbProvider);

        Assert.Contains("AddHttpContextAccessor()", result);
    }

    [Fact]
    public void EmitsCustomErrorComment_WhenDetected()
    {
        var emitter = new ProgramCsEmitter();
        var profile = new RuntimeProfile
        {
            CustomErrorRedirect = "/ErrorPage.aspx"
        };

        var result = emitter.Generate("TestApp", profile, DefaultDbProvider);

        Assert.Contains("Custom error page detected", result);
        Assert.Contains("/ErrorPage.aspx", result);
    }

    [Fact]
    public void UsesDetectedLoginPath_InCookieConfig()
    {
        var emitter = new ProgramCsEmitter();
        var profile = new RuntimeProfile
        {
            NeedsIdentity = true,
            NeedsEntityFramework = true,
            AuthLoginPath = "/Account/Login.aspx"
        };

        var result = emitter.Generate("TestApp", profile, DefaultDbProvider);

        Assert.Contains("options.LoginPath = \"/Account/Login.aspx\"", result);
    }

    [Fact]
    public void MiddlewareOrder_IsCorrect()
    {
        var emitter = new ProgramCsEmitter();
        var profile = new RuntimeProfile
        {
            NeedsSession = true,
            NeedsIdentity = true,
            NeedsEntityFramework = true
        };

        var result = emitter.Generate("TestApp", profile, DefaultDbProvider);

        // Verify middleware ordering: HttpsRedirection → StaticAssets → Session → Auth → Antiforgery
        var httpsIdx = result.IndexOf("app.UseHttpsRedirection()");
        var staticIdx = result.IndexOf("app.MapStaticAssets()");
        var sessionIdx = result.IndexOf("app.UseSession()");
        var authIdx = result.IndexOf("app.UseAuthentication()");
        var authzIdx = result.IndexOf("app.UseAuthorization()");
        var antiforgeryIdx = result.IndexOf("app.UseAntiforgery()");

        Assert.True(httpsIdx < staticIdx, "HttpsRedirection should come before StaticAssets");
        Assert.True(staticIdx < sessionIdx, "StaticAssets should come before Session");
        Assert.True(sessionIdx < authIdx, "Session should come before Authentication");
        Assert.True(authIdx < authzIdx, "Authentication should come before Authorization");
        Assert.True(authzIdx < antiforgeryIdx, "Authorization should come before Antiforgery");
    }

    [Fact]
    public void FullApp_HasCorrectStructure()
    {
        var emitter = new ProgramCsEmitter();
        var profile = new RuntimeProfile
        {
            NeedsSession = true,
            NeedsIdentity = true,
            NeedsEntityFramework = true,
            AdditionalDbContextNames = ["ProductContext"],
            DetectedRoleNames = ["canEdit"],
            DetectedSeedUsers = [("admin@test.com", "Pa$$word1", "canEdit")],
            CustomErrorRedirect = "/Error",
            ApplicationStartPatterns = ["RouteConfig.RegisterRoutes", "Database.SetInitializer"]
        };

        var result = emitter.Generate("TestApp", profile, DefaultDbProvider);

        // All major sections present
        Assert.Contains("using BlazorWebFormsComponents;", result);
        Assert.Contains("using Microsoft.EntityFrameworkCore;", result);
        Assert.Contains("using Microsoft.AspNetCore.Identity;", result);
        Assert.Contains("var builder = WebApplication.CreateBuilder(args);", result);
        Assert.Contains("AddDbContext<ApplicationDbContext>", result);
        Assert.Contains("AddDbContextFactory<ProductContext>", result);
        Assert.Contains("AddDefaultIdentity<ApplicationUser>", result);
        Assert.Contains("AddBlazorWebFormsComponents()", result);
        Assert.Contains("var app = builder.Build();", result);
        Assert.Contains("EnsureCreated()", result);
        Assert.Contains("UseExceptionHandler", result);
        Assert.Contains("MapRazorComponents", result);
        Assert.Contains("RoleExistsAsync(\"canEdit\")", result);
        Assert.Contains("FindByEmailAsync(\"admin@test.com\")", result);
        Assert.Contains("app.Run();", result);

        // TODO comments for unhandled patterns
        Assert.Contains("RouteConfig.RegisterRoutes", result);
        Assert.Contains("Database.SetInitializer", result);
    }
}
