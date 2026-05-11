using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Config;
using BlazorWebFormsComponents.Cli.Io;

namespace BlazorWebFormsComponents.Cli.Scaffolding;

/// <summary>
/// Generates the Blazor project scaffold: .csproj, Program.cs, _Imports.razor, App.razor, Routes.razor, launchSettings.json.
/// Ported from New-ProjectScaffold + New-AppRazorScaffold in bwfc-migrate.ps1.
/// </summary>
public class ProjectScaffolder
{
    private static readonly Regex SystemWebHttpUtilityPackageRegex = new(
        @"^\s*<PackageReference Include=""System\.Web\.HttpUtility"".*?/?>\s*$\r?\n?",
        RegexOptions.Compiled | RegexOptions.Multiline);

    private readonly DatabaseProviderDetector _dbDetector;
    private readonly RuntimeDetector _runtimeDetector;
    private readonly ProgramCsEmitter _programCsEmitter;
    private readonly MasterPageToLayoutConverter _masterPageConverter;

    public ProjectScaffolder(
        DatabaseProviderDetector dbDetector,
        RuntimeDetector runtimeDetector,
        ProgramCsEmitter programCsEmitter,
        MasterPageToLayoutConverter masterPageConverter)
    {
        _dbDetector = dbDetector;
        _runtimeDetector = runtimeDetector;
        _programCsEmitter = programCsEmitter;
        _masterPageConverter = masterPageConverter;
    }

    public ScaffoldResult Scaffold(string sourcePath, string outputRoot, string projectName)
    {
        var result = new ScaffoldResult { ProjectName = projectName };

        // Detect features
        var hasModels = !string.IsNullOrEmpty(sourcePath) &&
                        Directory.Exists(Path.Combine(sourcePath, "Models"));
        var runtimeProfile = _runtimeDetector.Detect(sourcePath);
        var dbProvider = _dbDetector.Detect(sourcePath);

        result.HasModels = hasModels;
        result.HasIdentity = runtimeProfile.NeedsIdentity;
        result.DbProvider = dbProvider;
        result.RuntimeProfile = runtimeProfile;

        // Generate all scaffold files
        result.Files["csproj"] = new ScaffoldFile
        {
            RelativePath = $"{projectName}.csproj",
            Content = GenerateCsproj(projectName, outputRoot, runtimeProfile, dbProvider)
        };

        result.Files["program"] = new ScaffoldFile
        {
            RelativePath = "Program.cs",
            Content = _programCsEmitter.Generate(projectName, runtimeProfile, dbProvider)
        };

        result.Files["imports"] = new ScaffoldFile
        {
            RelativePath = "_Imports.razor",
            Content = GenerateImportsRazor(projectName, hasModels)
        };

        result.Files["app"] = new ScaffoldFile
        {
            RelativePath = Path.Combine("Components", "App.razor"),
            Content = GenerateAppRazor()
        };

        result.Files["routes"] = new ScaffoldFile
        {
            RelativePath = Path.Combine("Components", "Routes.razor"),
            Content = GenerateRoutesRazor()
        };

        result.Files["layout"] = new ScaffoldFile
        {
            RelativePath = Path.Combine("Components", "Layout", "MainLayout.razor"),
            Content = GenerateMainLayoutRazor(sourcePath)
        };

        result.Files["launchSettings"] = new ScaffoldFile
        {
            RelativePath = Path.Combine("Properties", "launchSettings.json"),
            Content = GenerateLaunchSettings(projectName)
        };

        if (runtimeProfile.NeedsIdentity)
        {
            result.Files["applicationUser"] = new ScaffoldFile
            {
                RelativePath = Path.Combine("Models", "ApplicationUser.cs"),
                Content = GenerateApplicationUser(projectName)
            };

            result.Files["applicationDbContext"] = new ScaffoldFile
            {
                RelativePath = Path.Combine("Models", "ApplicationDbContext.cs"),
                Content = GenerateApplicationDbContext(projectName)
            };
        }

        return result;
    }

    public async Task WriteAsync(ScaffoldResult result, string outputRoot, OutputWriter writer)
    {
        foreach (var file in result.Files.Values)
        {
            var fullPath = Path.Combine(outputRoot, file.RelativePath);
            await writer.WriteFileAsync(fullPath, file.Content, $"Scaffold: {file.RelativePath}");
        }
    }

    private static string GenerateCsproj(string projectName, string outputRoot, RuntimeProfile runtimeProfile, DatabaseProviderInfo dbProvider)
    {
        var additionalPackages = "";
        if (runtimeProfile.NeedsEntityFramework)
        {
            additionalPackages += $"\n    <PackageReference Include=\"{dbProvider.PackageName}\" Version=\"10.0.0\" />";
            additionalPackages += "\n    <PackageReference Include=\"Microsoft.EntityFrameworkCore.Tools\" Version=\"10.0.0\" />";
        }
        if (runtimeProfile.NeedsIdentity)
        {
            additionalPackages += "\n    <PackageReference Include=\"Microsoft.AspNetCore.Identity.EntityFrameworkCore\" Version=\"10.0.0\" />";
            additionalPackages += "\n    <PackageReference Include=\"Microsoft.AspNetCore.Identity.UI\" Version=\"10.0.0\" />";
            additionalPackages += "\n    <PackageReference Include=\"Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore\" Version=\"10.0.0\" />";
        }

        var bwfcReference = ResolveBwfcReference(outputRoot);

        var csproj = $@"<Project Sdk=""Microsoft.NET.Sdk.Web"">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <BwfcMigrationMode>true</BwfcMigrationMode>
    <EnforceCodeStyleInBuild>false</EnforceCodeStyleInBuild>
  </PropertyGroup>

  <ItemGroup>
{bwfcReference}{additionalPackages}
  </ItemGroup>

</Project>
";

        return StripUnsupportedPackages(csproj);
    }

    private static string StripUnsupportedPackages(string csproj)
        => SystemWebHttpUtilityPackageRegex.Replace(csproj, string.Empty);

    private static string ResolveBwfcReference(string outputRoot)
    {
        var outputFullPath = Path.GetFullPath(outputRoot);
        var current = new DirectoryInfo(outputFullPath);

        while (current is not null)
        {
            var candidate = Path.Combine(current.FullName, "src", "BlazorWebFormsComponents", "BlazorWebFormsComponents.csproj");
            if (File.Exists(candidate))
            {
                var relativePath = Path.GetRelativePath(outputFullPath, candidate)
                    .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                return $@"    <ProjectReference Include=""{relativePath}"" />";
            }

            current = current.Parent;
        }

        return @"    <PackageReference Include=""Fritz.BlazorWebFormsComponents"" Version=""*"" />";
    }


    private static string GenerateImportsRazor(string projectName, bool hasModels)
    {
        var modelsUsing = hasModels
            ? $@"
@using global::{projectName}.Models"
            : string.Empty;

        return $@"@namespace {projectName}
@using System.Net.Http
@using Microsoft.AspNetCore.Authorization
@using Microsoft.AspNetCore.Components.Authorization
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.JSInterop
@using BlazorWebFormsComponents
@using BlazorWebFormsComponents.Enums
@using BlazorWebFormsComponents.LoginControls
@using BlazorWebFormsComponents.Validations
@using global::{projectName}{modelsUsing}
@inherits BlazorWebFormsComponents.WebFormsPageBase
";
    }

    private static string GenerateAppRazor()
    {
        return @"<!DOCTYPE html>
<html lang=""en"">

<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <base href=""/"" />
    <HeadOutlet />
</head>

@* Generated for .NET 10 static SSR migration output. Only opt into interactive render modes deliberately and per page. *@
<body>
    <Routes />
    <script src=""_framework/blazor.web.js""></script>
    <script src=""_content/Fritz.BlazorWebFormsComponents/js/Basepage.js""></script>
</body>

</html>
";
    }

    private static string GenerateRoutesRazor()
    {
        return @"<Router AppAssembly=""typeof(Program).Assembly"">
    <Found Context=""routeData"">
        <RouteView RouteData=""routeData"" DefaultLayout=""typeof(Layout.MainLayout)"" />
        <FocusOnNavigate RouteData=""routeData"" Selector=""h1"" />
    </Found>
</Router>
";
    }

    private string GenerateMainLayoutRazor(string sourcePath)
    {
        // Try to convert the original Site.Master into a proper layout
        var masterPath = MasterPageToLayoutConverter.FindMasterPage(sourcePath);
        if (masterPath != null)
        {
            var masterContent = File.ReadAllText(masterPath);
            var converted = _masterPageConverter.Convert(masterContent);
            if (converted != null)
                return converted;
        }

        // Fallback: minimal layout
        return @"@inherits LayoutComponentBase

<main>
    @Body
</main>
";
    }

    private static string GenerateLaunchSettings(string projectName)
    {
        return $$"""
{
  "profiles": {
    "{{projectName}}": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:5001;http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
""";
    }

    private static string GenerateApplicationUser(string projectName)
    {
        return $@"using Microsoft.AspNetCore.Identity;

namespace {projectName}.Models;

/// <summary>
/// Stub generated by BWFC migration CLI.
/// Add custom user properties as needed for your application.
/// </summary>
public class ApplicationUser : IdentityUser
{{
}}
";
    }

    private static string GenerateApplicationDbContext(string projectName)
    {
        return $@"using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace {projectName}.Models;

/// <summary>
/// Stub generated by BWFC migration CLI.
/// Add DbSet properties for your application entities.
/// </summary>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{{
}}
";
    }
}

public class ScaffoldResult
{
    public required string ProjectName { get; init; }
    public bool HasModels { get; set; }
    public bool HasIdentity { get; set; }
    public DatabaseProviderInfo? DbProvider { get; set; }
    public RuntimeProfile RuntimeProfile { get; set; } = new();
    public Dictionary<string, ScaffoldFile> Files { get; } = [];
}

public class ScaffoldFile
{
    public required string RelativePath { get; init; }
    public required string Content { get; init; }
}
