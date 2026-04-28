using BlazorWebFormsComponents.Cli.Config;
using BlazorWebFormsComponents.Cli.Io;

namespace BlazorWebFormsComponents.Cli.Scaffolding;

/// <summary>
/// Generates the Blazor project scaffold: .csproj, Program.cs, _Imports.razor, App.razor, Routes.razor, launchSettings.json.
/// Ported from New-ProjectScaffold + New-AppRazorScaffold in bwfc-migrate.ps1.
/// </summary>
public class ProjectScaffolder
{
    private readonly DatabaseProviderDetector _dbDetector;

    public ProjectScaffolder(DatabaseProviderDetector dbDetector)
    {
        _dbDetector = dbDetector;
    }

    public ScaffoldResult Scaffold(string sourcePath, string outputRoot, string projectName)
    {
        var result = new ScaffoldResult { ProjectName = projectName };

        // Detect features
        var hasModels = !string.IsNullOrEmpty(sourcePath) &&
                        Directory.Exists(Path.Combine(sourcePath, "Models"));
        var hasIdentity = DetectIdentity(sourcePath);
        var dbProvider = _dbDetector.Detect(sourcePath);

        result.HasModels = hasModels;
        result.HasIdentity = hasIdentity;
        result.DbProvider = dbProvider;

        // Generate all scaffold files
        result.Files["csproj"] = new ScaffoldFile
        {
            RelativePath = $"{projectName}.csproj",
            Content = GenerateCsproj(projectName, outputRoot, hasModels, hasIdentity, dbProvider)
        };

        result.Files["program"] = new ScaffoldFile
        {
            RelativePath = "Program.cs",
            Content = GenerateProgramCs(projectName, hasModels, hasIdentity, dbProvider)
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
            Content = GenerateMainLayoutRazor()
        };

        result.Files["launchSettings"] = new ScaffoldFile
        {
            RelativePath = Path.Combine("Properties", "launchSettings.json"),
            Content = GenerateLaunchSettings(projectName)
        };

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

    private static bool DetectIdentity(string sourcePath)
    {
        if (string.IsNullOrEmpty(sourcePath))
            return false;

        return Directory.Exists(Path.Combine(sourcePath, "Account")) ||
               File.Exists(Path.Combine(sourcePath, "Login.aspx")) ||
               File.Exists(Path.Combine(sourcePath, "Register.aspx"));
    }

    private static string GenerateCsproj(string projectName, string outputRoot, bool hasModels, bool hasIdentity, DatabaseProviderInfo dbProvider)
    {
        var additionalPackages = "";
        if (hasModels)
        {
            additionalPackages += $"\n    <PackageReference Include=\"{dbProvider.PackageName}\" Version=\"10.0.0\" />";
            additionalPackages += "\n    <PackageReference Include=\"Microsoft.EntityFrameworkCore.Tools\" Version=\"10.0.0\" />";
        }
        if (hasIdentity)
        {
            additionalPackages += "\n    <PackageReference Include=\"Microsoft.AspNetCore.Identity.EntityFrameworkCore\" Version=\"10.0.0\" />";
            additionalPackages += "\n    <PackageReference Include=\"Microsoft.AspNetCore.Identity.UI\" Version=\"10.0.0\" />";
            additionalPackages += "\n    <PackageReference Include=\"Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore\" Version=\"10.0.0\" />";
        }

        var bwfcReference = ResolveBwfcReference(outputRoot);

        return $@"<Project Sdk=""Microsoft.NET.Sdk.Web"">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <BwfcMigrationMode>true</BwfcMigrationMode>
  </PropertyGroup>

  <ItemGroup>
{bwfcReference}{additionalPackages}
  </ItemGroup>

</Project>
";
    }

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

    private static string GenerateProgramCs(string projectName, bool hasModels, bool hasIdentity, DatabaseProviderInfo dbProvider)
    {
        var dbContextLine = !string.IsNullOrEmpty(dbProvider.ConnectionString)
            ? $"// builder.Services.AddDbContextFactory<YourDbContext>(options => options.{dbProvider.ProviderMethod}(\"{dbProvider.ConnectionString.Replace("\\", "\\\\")}\"));"
            : $"// builder.Services.AddDbContextFactory<YourDbContext>(options => options.{dbProvider.ProviderMethod}(\"your-connection-string\"));";

        var identityServiceBlock = "";
        var identityMiddlewareBlock = "";

        if (hasIdentity)
        {
            identityServiceBlock = $@"

// TODO(bwfc-datasource): Configure database connection (use AddDbContextFactory — do NOT also register AddDbContext to avoid DI conflicts)
{dbContextLine}

// TODO(bwfc-identity): Configure Identity
// builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
//     .AddEntityFrameworkStores<ProductContext>();

// TODO(bwfc-session-state): Configure session for cart/state management
// builder.Services.AddDistributedMemoryCache();
// builder.Services.AddSession();
// builder.Services.AddHttpContextAccessor();
// builder.Services.AddCascadingAuthenticationState();
";

            identityMiddlewareBlock = @"

// TODO(bwfc-general): Add middleware in the pipeline
// app.UseSession();
// app.UseAuthentication();
// app.UseAuthorization();
";
        }
        else if (hasModels)
        {
            identityServiceBlock = $@"

// TODO(bwfc-datasource): Configure database connection (use AddDbContextFactory — do NOT also register AddDbContext to avoid DI conflicts)
{dbContextLine}
";
        }

        return $@"// TODO(bwfc-general): Review and adjust this generated Program.cs for your application needs.
// Generated for .NET 10 Blazor static SSR. Keep interactive render modes opt-in and page-specific.
using BlazorWebFormsComponents;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents();

builder.Services.AddBlazorWebFormsComponents();
{identityServiceBlock}
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{{
    app.UseExceptionHandler(""/Error"");
    app.UseHsts();
}}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseAntiforgery();
{identityMiddlewareBlock}
app.MapRazorComponents<{projectName}.Components.App>();

app.Run();
";
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

    private static string GenerateMainLayoutRazor()
    {
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
}

public class ScaffoldResult
{
    public required string ProjectName { get; init; }
    public bool HasModels { get; set; }
    public bool HasIdentity { get; set; }
    public DatabaseProviderInfo? DbProvider { get; set; }
    public Dictionary<string, ScaffoldFile> Files { get; } = [];
}

public class ScaffoldFile
{
    public required string RelativePath { get; init; }
    public required string Content { get; init; }
}
