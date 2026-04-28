using BlazorWebFormsComponents.Cli.Config;
using BlazorWebFormsComponents.Cli.Scaffolding;

namespace BlazorWebFormsComponents.Cli.Tests;

/// <summary>
/// Tests for the project scaffolding output: .csproj, Program.cs,
/// _Imports.razor, App.razor, GlobalUsings, and shim generators.
/// </summary>
public class ScaffoldingTests : IDisposable
{
    private readonly string _tempDir;
    private readonly ProjectScaffolder _scaffolder;

    public ScaffoldingTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"bwfc-scaffold-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
        _scaffolder = new ProjectScaffolder(new DatabaseProviderDetector());
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            try { Directory.Delete(_tempDir, recursive: true); }
            catch { /* best effort cleanup */ }
        }
    }

    // ───────────────────────────────────────────────────────────────
    //  ProjectScaffolder
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void ProjectScaffolder_GeneratesCsproj()
    {
        // Arrange — empty source so no identity/models detected
        var result = _scaffolder.Scaffold(_tempDir, _tempDir, "TestApp");

        // Act
        var csproj = result.Files["csproj"].Content;

        // Assert — must contain BWFC package reference, target framework, nullable
        Assert.Contains("Fritz.BlazorWebFormsComponents", csproj);
        Assert.Contains("<TargetFramework>net10.0</TargetFramework>", csproj);
        Assert.Contains("<Nullable>enable</Nullable>", csproj);
        Assert.Contains("Microsoft.NET.Sdk.Web", csproj);
    }

    [Fact]
    public void ProjectScaffolder_UsesProjectReference_WhenOutputIsInsideRepo()
    {
        var repoRoot = Path.Combine(_tempDir, "repo");
        var srcDir = Path.Combine(repoRoot, "src", "BlazorWebFormsComponents");
        var outputDir = Path.Combine(repoRoot, "samples", "AfterTestApp");

        Directory.CreateDirectory(srcDir);
        Directory.CreateDirectory(outputDir);
        File.WriteAllText(Path.Combine(srcDir, "BlazorWebFormsComponents.csproj"), "<Project />");

        var result = _scaffolder.Scaffold(repoRoot, outputDir, "TestApp");
        var csproj = result.Files["csproj"].Content;

        Assert.Contains(@"<ProjectReference Include=""..\..\src\BlazorWebFormsComponents\BlazorWebFormsComponents.csproj"" />", csproj);
        Assert.DoesNotContain(@"<PackageReference Include=""Fritz.BlazorWebFormsComponents"" Version=""*"" />", csproj);
    }

    [Fact]
    public void ProjectScaffolder_CsprojFileName_MatchesProjectName()
    {
        var result = _scaffolder.Scaffold(_tempDir, _tempDir, "MyWebApp");

        Assert.Equal("MyWebApp.csproj", result.Files["csproj"].RelativePath);
    }

    [Fact]
    public void ProjectScaffolder_GeneratesProgramCs()
    {
        var result = _scaffolder.Scaffold(_tempDir, _tempDir, "TestApp");

        var program = result.Files["program"].Content;

        Assert.Contains("AddBlazorWebFormsComponents()", program);
        Assert.Contains("AddRazorComponents()", program);
        Assert.DoesNotContain("AddInteractiveServerComponents()", program);
        Assert.Contains("Generated for .NET 10 Blazor static SSR", program);
        Assert.Contains("using BlazorWebFormsComponents;", program);
    }

    [Fact]
    public void ProjectScaffolder_ProgramCs_MapsComponents()
    {
        var result = _scaffolder.Scaffold(_tempDir, _tempDir, "TestApp");

        var program = result.Files["program"].Content;

        Assert.Contains("MapRazorComponents<TestApp.Components.App>()", program);
        Assert.DoesNotContain("AddInteractiveServerRenderMode()", program);
    }

    [Fact]
    public void ProjectScaffolder_GeneratesImportsRazor()
    {
        var result = _scaffolder.Scaffold(_tempDir, _tempDir, "TestApp");

        var imports = result.Files["imports"].Content;

        // Standard Blazor usings
        Assert.Contains("@namespace TestApp", imports);
        Assert.Contains("@using Microsoft.AspNetCore.Components.Web", imports);
        Assert.Contains("@using Microsoft.AspNetCore.Components.Forms", imports);
        Assert.Contains("@using Microsoft.AspNetCore.Components.Routing", imports);
        Assert.Contains("@using Microsoft.JSInterop", imports);
        // BWFC usings
        Assert.Contains("@using BlazorWebFormsComponents", imports);
        Assert.Contains("@using BlazorWebFormsComponents.Enums", imports);
        Assert.Contains("@using BlazorWebFormsComponents.Validations", imports);
        Assert.DoesNotContain("@using static Microsoft.AspNetCore.Components.Web.RenderMode", imports);
        // Project namespace
        Assert.Contains("@using global::TestApp", imports);
        // WebFormsPageBase inherits
        Assert.Contains("@inherits BlazorWebFormsComponents.WebFormsPageBase", imports);
    }

    [Fact]
    public void ProjectScaffolder_GeneratesModelsUsing_WhenModelsExist()
    {
        Directory.CreateDirectory(Path.Combine(_tempDir, "Models"));

        var result = _scaffolder.Scaffold(_tempDir, _tempDir, "TestApp");
        var imports = result.Files["imports"].Content;

        Assert.Contains("@using global::TestApp.Models", imports);
    }

    [Fact]
    public void ProjectScaffolder_GeneratesAppRazor()
    {
        var result = _scaffolder.Scaffold(_tempDir, _tempDir, "TestApp");

        var appRazor = result.Files["app"].Content;

        Assert.Contains("<Routes />", appRazor);
        Assert.Contains("<HeadOutlet />", appRazor);
        Assert.Contains("<!DOCTYPE html>", appRazor);
        Assert.DoesNotContain("blazor.web.js", appRazor);
        Assert.Contains("Generated for .NET 10 static SSR migration output", appRazor);
    }

    [Fact]
    public void ProjectScaffolder_GeneratesRoutesRazor()
    {
        var result = _scaffolder.Scaffold(_tempDir, _tempDir, "TestApp");

        var routes = result.Files["routes"].Content;

        Assert.Contains("<Router", routes);
        Assert.Contains("RouteView", routes);
        Assert.Contains("FocusOnNavigate", routes);
    }

    [Fact]
    public void ProjectScaffolder_GeneratesMainLayout()
    {
        var result = _scaffolder.Scaffold(_tempDir, _tempDir, "TestApp");

        var layout = result.Files["layout"].Content;

        Assert.Contains("@inherits LayoutComponentBase", layout);
        Assert.Contains("@Body", layout);
    }

    [Fact]
    public void ProjectScaffolder_GeneratesLaunchSettings()
    {
        var result = _scaffolder.Scaffold(_tempDir, _tempDir, "TestApp");

        var launch = result.Files["launchSettings"].Content;

        Assert.Contains("TestApp", launch);
        Assert.Contains("applicationUrl", launch);
        Assert.Contains("ASPNETCORE_ENVIRONMENT", launch);
    }

    [Fact]
    public void ProjectScaffolder_DetectsIdentity_WhenAccountFolderExists()
    {
        Directory.CreateDirectory(Path.Combine(_tempDir, "Account"));

        var result = _scaffolder.Scaffold(_tempDir, _tempDir, "TestApp");

        Assert.True(result.HasIdentity);
        Assert.Contains("Identity", result.Files["csproj"].Content);
        Assert.Contains("Identity", result.Files["program"].Content);
    }

    [Fact]
    public void ProjectScaffolder_DetectsIdentity_WhenLoginAspxExists()
    {
        File.WriteAllText(Path.Combine(_tempDir, "Login.aspx"), "<%@ Page %>");

        var result = _scaffolder.Scaffold(_tempDir, _tempDir, "TestApp");

        Assert.True(result.HasIdentity);
    }

    [Fact]
    public void ProjectScaffolder_NoIdentity_WhenNoIndicators()
    {
        var result = _scaffolder.Scaffold(_tempDir, _tempDir, "TestApp");

        Assert.False(result.HasIdentity);
        Assert.DoesNotContain("Identity", result.Files["csproj"].Content);
    }

    [Fact]
    public void ProjectScaffolder_DetectsModels_WhenModelsFolderExists()
    {
        Directory.CreateDirectory(Path.Combine(_tempDir, "Models"));

        var result = _scaffolder.Scaffold(_tempDir, _tempDir, "TestApp");

        Assert.True(result.HasModels);
        Assert.Contains("EntityFrameworkCore", result.Files["csproj"].Content);
    }

    [Fact]
    public void ProjectScaffolder_GeneratesAllExpectedFileKeys()
    {
        var result = _scaffolder.Scaffold(_tempDir, _tempDir, "TestApp");

        Assert.Contains("csproj", result.Files.Keys);
        Assert.Contains("program", result.Files.Keys);
        Assert.Contains("imports", result.Files.Keys);
        Assert.Contains("app", result.Files.Keys);
        Assert.Contains("routes", result.Files.Keys);
        Assert.Contains("layout", result.Files.Keys);
        Assert.Contains("launchSettings", result.Files.Keys);
        Assert.Equal(7, result.Files.Count);
    }

    // ───────────────────────────────────────────────────────────────
    //  GlobalUsingsGenerator
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void GlobalUsingsGenerator_GeneratesExpectedUsings()
    {
        var generator = new GlobalUsingsGenerator();

        var content = generator.Generate(hasIdentity: false);

        Assert.Contains("global using Microsoft.AspNetCore.Components;", content);
        Assert.Contains("global using Microsoft.AspNetCore.Components.Web;", content);
        Assert.Contains("global using Microsoft.AspNetCore.Components.Routing;", content);
        Assert.DoesNotContain("Identity", content);
    }

    [Fact]
    public void GlobalUsingsGenerator_IncludesIdentityUsings_WhenIdentityEnabled()
    {
        var generator = new GlobalUsingsGenerator();

        var content = generator.Generate(hasIdentity: true);

        Assert.Contains("global using Microsoft.AspNetCore.Components.Authorization;", content);
        Assert.Contains("global using Microsoft.AspNetCore.Identity;", content);
    }

    [Fact]
    public void GlobalUsingsGenerator_OmitsIdentityUsings_WhenIdentityDisabled()
    {
        var generator = new GlobalUsingsGenerator();

        var content = generator.Generate(hasIdentity: false);

        Assert.DoesNotContain("Identity", content);
        Assert.DoesNotContain("Authorization", content);
    }

    [Fact]
    public void GlobalUsingsGenerator_ContainsHeaderComment()
    {
        var generator = new GlobalUsingsGenerator();

        var content = generator.Generate();

        Assert.Contains("Layer 1 scaffold", content);
        Assert.Contains("webforms-to-blazor", content);
    }

    // ───────────────────────────────────────────────────────────────
    //  ShimGenerator
    // ───────────────────────────────────────────────────────────────

    [Fact]
    public void ShimGenerator_GeneratesWebFormsShims()
    {
        var generator = new ShimGenerator();

        var content = generator.GenerateWebFormsShims();

        Assert.Contains("ConfigurationManager", content);
        Assert.Contains("using BlazorWebFormsComponents;", content);
        Assert.Contains("Layer 1 scaffold", content);
    }

    [Fact]
    public void ShimGenerator_GeneratesIdentityShims()
    {
        var generator = new ShimGenerator();

        var content = generator.GenerateIdentityShims();

        Assert.Contains("Identity", content);
        Assert.Contains("Membership", content);
        Assert.Contains("using Microsoft.AspNetCore.Identity;", content);
    }

    [Fact]
    public void ShimGenerator_SkipsIdentityShims_WhenNoIdentity()
    {
        // The ShimGenerator.WriteAsync method conditionally writes identity shims.
        // We verify the API contract: GenerateIdentityShims() exists but
        // WriteAsync only writes it when hasIdentity is true.
        var generator = new ShimGenerator();

        // When hasIdentity=false, only WebFormsShims should be produced
        // Verify by checking that the web forms shims don't contain identity content
        var webShims = generator.GenerateWebFormsShims();
        Assert.DoesNotContain("Membership.GetUser", webShims);
        Assert.DoesNotContain("Microsoft.AspNetCore.Identity", webShims);
    }

    [Fact]
    public async Task ShimGenerator_WriteAsync_WritesOnlyWebShims_WhenNoIdentity()
    {
        var generator = new ShimGenerator();
        var writer = new Io.OutputWriter { DryRun = false };

        var outputDir = Path.Combine(_tempDir, "shim-output");
        Directory.CreateDirectory(outputDir);

        await generator.WriteAsync(outputDir, writer, hasIdentity: false);

        Assert.True(File.Exists(Path.Combine(outputDir, "WebFormsShims.cs")));
        Assert.False(File.Exists(Path.Combine(outputDir, "IdentityShims.cs")));
    }

    [Fact]
    public async Task ShimGenerator_WriteAsync_WritesBothShims_WhenIdentity()
    {
        var generator = new ShimGenerator();
        var writer = new Io.OutputWriter { DryRun = false };

        var outputDir = Path.Combine(_tempDir, "shim-output-identity");
        Directory.CreateDirectory(outputDir);

        await generator.WriteAsync(outputDir, writer, hasIdentity: true);

        Assert.True(File.Exists(Path.Combine(outputDir, "WebFormsShims.cs")));
        Assert.True(File.Exists(Path.Combine(outputDir, "IdentityShims.cs")));
    }
}
