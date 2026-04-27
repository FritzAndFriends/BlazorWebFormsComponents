<#
.SYNOPSIS
    Performs mechanical regex-based transforms on ASP.NET Web Forms files to produce Blazor-ready output.

.DESCRIPTION
    bwfc-migrate.ps1 is Layer 1 of the three-layer Web Forms to Blazor migration pipeline.
    It automates ~60% of the migration by applying safe, mechanical regex transforms:

      - Renames .aspx/.ascx/.master files to .razor
      - Strips Web Forms directives and converts to Razor equivalents
      - Removes asp: prefixes and runat="server" attributes
      - Converts Web Forms expressions to Razor syntax
      - Transforms URL references from ~/ to /
      - Cleans up Web Forms-specific attributes
      - Normalizes boolean, enum, and unit attribute values for Blazor
      - Converts Response.Redirect to NavigationManager.NavigateTo in code-behind
      - Detects Session/ViewState usage and adds migration guidance
      - Flags DataSourceID attributes and data source controls for conversion
      - Copies code-behind files with TODO annotations

    Code-behind transforms include:
      - Page lifecycle methods (Page_Load, Page_Init, Page_PreRender) → Blazor equivalents
      - Event handler signatures → Blazor-compatible signatures (strip sender, adjust EventArgs)
    Remaining semantic transforms (data binding logic, complex state management)
    require the Copilot skill layer (Layer 2).

.PARAMETER Path
    Path to the Web Forms project root directory containing .aspx/.ascx/.master files.

.PARAMETER Output
    Path for the Blazor output project directory. Will be created if it does not exist.

.PARAMETER WhatIf
    Show what transforms would be applied without writing any files.

.PARAMETER Verbose
    Show detailed per-file transform log during processing.

.PARAMETER SkipProjectScaffold
    Skip generating .csproj, Program.cs, and _Imports.razor scaffold files.

.EXAMPLE
    .\bwfc-migrate.ps1 -Path C:\src\MyWebFormsApp -Output C:\src\MyBlazorApp

    Migrates all Web Forms files from MyWebFormsApp into a new Blazor project at MyBlazorApp.

.EXAMPLE
    .\bwfc-migrate.ps1 -Path C:\src\MyWebFormsApp -Output C:\src\MyBlazorApp -WhatIf

    Shows what transforms would be applied without creating any files.

.PARAMETER Prescan
    Scan source files for patterns matching BWFC analyzer rules and output a JSON summary. No migration is performed.

.EXAMPLE
    .\bwfc-migrate.ps1 -Path .\LegacyApp -Output .\BlazorApp -SkipProjectScaffold -Verbose

    Transforms files with detailed logging, skipping project scaffold generation.

.EXAMPLE
    .\bwfc-migrate.ps1 -Path C:\src\MyWebFormsApp -Output . -Prescan

    Scans MyWebFormsApp for migration patterns and outputs JSON summary.
    The -Output parameter is required but unused in prescan mode (early return before output is written).
#>

[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter(Mandatory = $true, HelpMessage = "Path to the Web Forms project root")]
    [string]$Path,

    [Parameter(Mandatory = $true, HelpMessage = "Path for the Blazor output project")]
    [string]$Output,

    [Parameter(HelpMessage = "Skip creating .csproj, Program.cs, and _Imports.razor")]
    [switch]$SkipProjectScaffold,

    [Parameter(HelpMessage = "Scan source files for BWFC analyzer patterns and output JSON summary without migrating")]
    [switch]$Prescan
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Resolve-BwfcCliProject {
    param([string]$StartPath)

    $currentDir = [System.IO.DirectoryInfo]::new([System.IO.Path]::GetFullPath($StartPath))
    while ($null -ne $currentDir) {
        $candidate = Join-Path $currentDir.FullName 'src\BlazorWebFormsComponents.Cli\BlazorWebFormsComponents.Cli.csproj'
        if (Test-Path $candidate) {
            return $candidate
        }

        $currentDir = $currentDir.Parent
    }

    throw "Could not locate BlazorWebFormsComponents.Cli.csproj from '$StartPath'."
}

$cliProject = Resolve-BwfcCliProject -StartPath $PSScriptRoot

$cliArgs = @(
    'run',
    '--project', $cliProject,
    '--'
)

if ($Prescan) {
    $cliArgs += @(
        'prescan',
        '--input', $Path
    )
}
else {
    $cliArgs += @(
        'migrate',
        '--input', $Path,
        '--output', $Output,
        '--overwrite'
    )

    if ($SkipProjectScaffold) {
        $cliArgs += '--skip-scaffold'
    }

    if ($WhatIfPreference) {
        $cliArgs += '--dry-run'
    }

    if ($VerbosePreference -eq 'Continue') {
        $cliArgs += '--verbose'
    }
}

& dotnet @cliArgs
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

return

#region --- Configuration ---

$WebFormsExtensions = @('.aspx', '.ascx', '.master')
$CodeBehindExtensions = @('.aspx.cs', '.ascx.cs', '.master.cs', '.aspx.vb', '.ascx.vb', '.master.vb')
$StaticExtensions = @('.css', '.js', '.png', '.jpg', '.jpeg', '.gif', '.svg', '.ico', '.woff', '.woff2', '.ttf', '.eot')

# Attributes to strip completely (case-insensitive patterns)
$StripAttributes = @(
    'runat\s*=\s*"server"',
    'AutoEventWireup\s*=\s*"(true|false)"',
    'EnableViewState\s*=\s*"(true|false)"',
    'ViewStateMode\s*=\s*"[^"]*"',
    'ValidateRequest\s*=\s*"(true|false)"',
    'MaintainScrollPositionOnPostBack\s*=\s*"(true|false)"',
    'ClientIDMode\s*=\s*"[^"]*"'
)

#endregion

#region --- Pre-Scan ---

function Invoke-BwfcPrescan {
    param(
        [string]$SourcePath
    )
    
    $results = @{
        ScanDate = (Get-Date -Format 'o')
        SourcePath = $SourcePath
        Summary = @{}
        Files = @()
    }
    
    # Scan patterns matching BWFC analyzer rules
    $patterns = @{
        'BWFC001' = @{ Name = 'Missing [Parameter]'; Pattern = 'public\s+\w+\s+\w+\s*\{\s*get\s*;\s*set\s*;\s*\}'; Description = 'Public properties that may need [Parameter] attribute' }
        'BWFC002' = @{ Name = 'ViewState Usage'; Pattern = 'ViewState\s*\['; Description = 'ViewState dictionary access' }
        'BWFC003' = @{ Name = 'IsPostBack'; Pattern = '(Page\.)?IsPostBack'; Description = 'IsPostBack checks' }
        'BWFC004' = @{ Name = 'Response.Redirect'; Pattern = 'Response\.Redirect\s*\('; Description = 'Response.Redirect calls' }
        'BWFC005' = @{ Name = 'Session Usage'; Pattern = 'Session\s*\[|HttpContext\.Current\.Session'; Description = 'Session state access' }
        'BWFC011' = @{ Name = 'Event Handler Signatures'; Pattern = '\(\s*object\s+\w+\s*,\s*EventArgs'; Description = 'Web Forms event handler signatures' }
        'BWFC012' = @{ Name = 'runat="server"'; Pattern = 'runat\s*=\s*"server"'; Description = 'runat="server" artifacts in strings/comments' }
        'BWFC013' = @{ Name = 'Response Object'; Pattern = 'Response\.(Write|WriteFile|Clear|Flush|End)\s*\('; Description = 'Response object method calls' }
        'BWFC014' = @{ Name = 'Request Object'; Pattern = 'Request\.(Form|Cookies|Headers|Files|QueryString|ServerVariables)\s*[\[\.]'; Description = 'Request object property access' }
        'BWFC015' = @{ Name = 'Server Utility'; Pattern = 'Server\.(MapPath|HtmlEncode|HtmlDecode|UrlEncode|UrlDecode)\s*\('; Description = 'Server utility calls — use ServerShim on WebFormsPageBase' }
        'BWFC016' = @{ Name = 'ConfigurationManager'; Pattern = 'ConfigurationManager\.(AppSettings|ConnectionStrings)\s*\['; Description = 'ConfigurationManager access — BWFC provides shim' }
        'BWFC017' = @{ Name = 'ClientScript'; Pattern = '(Page\.)?ClientScript\.(RegisterStartupScript|RegisterClientScriptBlock|RegisterClientScriptInclude|GetPostBackEventReference)\s*\('; Description = 'ClientScript calls — use ClientScriptShim' }
        'BWFC018' = @{ Name = 'Cache Access'; Pattern = '\bCache\s*\['; Description = 'Cache dictionary access — use CacheShim on WebFormsPageBase' }
        'BWFC021' = @{ Name = 'ContentPlaceHolder/MasterPage'; Pattern = '<asp:ContentPlaceHolder|MasterPageFile\s*='; Description = 'Master page placeholder relationships — will be preserved as BWFC ContentPlaceHolder/Content components' }
    }
    
    $csFiles = Get-ChildItem -Path $SourcePath -Filter '*.cs' -Recurse -File
    $totalMatches = 0
    
    foreach ($file in $csFiles) {
        $content = Get-Content -Path $file.FullName -Raw -ErrorAction SilentlyContinue
        if (-not $content) { continue }
        
        $fileMatches = @()
        foreach ($ruleId in $patterns.Keys) {
            $rule = $patterns[$ruleId]
            $matches = [regex]::Matches($content, $rule.Pattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
            if ($matches.Count -gt 0) {
                # Find line numbers
                $lines = @()
                foreach ($m in $matches) {
                    $lineNum = ($content.Substring(0, $m.Index) -split "`n").Count
                    $lines += $lineNum
                }
                $fileMatches += @{
                    Rule = $ruleId
                    Name = $rule.Name
                    Count = $matches.Count
                    Lines = $lines
                }
                
                if (-not $results.Summary.ContainsKey($ruleId)) {
                    $results.Summary[$ruleId] = @{ Name = $rule.Name; Description = $rule.Description; TotalHits = 0; FileCount = 0 }
                }
                $results.Summary[$ruleId].TotalHits += $matches.Count
                $results.Summary[$ruleId].FileCount += 1
                $totalMatches += $matches.Count
            }
        }
        
        if ($fileMatches.Count -gt 0) {
            $relPath = $file.FullName.Substring($SourcePath.Length).TrimStart('\', '/')
            $results.Files += @{
                Path = $relPath
                Matches = $fileMatches
            }
        }
    }
    
    $results.TotalFiles = $csFiles.Count
    $results.FilesWithMatches = $results.Files.Count
    $results.TotalMatches = $totalMatches
    
    return $results
}

#endregion

if ($Prescan) {
    Write-Host "BWFC Pre-Scan: Analyzing $Path for migration patterns..." -ForegroundColor Cyan
    $scanResults = Invoke-BwfcPrescan -SourcePath $Path
    $json = $scanResults | ConvertTo-Json -Depth 10
    Write-Output $json
    
    # Also write summary to stderr for human readability
    Write-Host "`nPre-Scan Summary:" -ForegroundColor Green
    Write-Host "  Files scanned: $($scanResults.TotalFiles)" 
    Write-Host "  Files with matches: $($scanResults.FilesWithMatches)"
    Write-Host "  Total pattern matches: $($scanResults.TotalMatches)"
    if ($scanResults.Summary.Count -gt 0) {
        Write-Host "`n  Rule breakdown:" -ForegroundColor Yellow
        foreach ($ruleId in ($scanResults.Summary.Keys | Sort-Object)) {
            $rule = $scanResults.Summary[$ruleId]
            Write-Host "    $ruleId ($($rule.Name)): $($rule.TotalHits) hits in $($rule.FileCount) files"
        }
    }
    return
}

#region --- Logging & Tracking ---

$script:TransformLog = [System.Collections.Generic.List[PSCustomObject]]::new()
$script:ManualItems = [System.Collections.Generic.List[PSCustomObject]]::new()
$script:RedirectHandlers = [System.Collections.Generic.List[string]]::new()
$script:FilesProcessed = 0
$script:TransformsApplied = 0
$script:HasAjaxToolkitControls = $false

function Write-TransformLog {
    param(
        [string]$File,
        [string]$Transform,
        [string]$Detail
    )
    $entry = [PSCustomObject]@{
        File      = $File
        Transform = $Transform
        Detail    = $Detail
    }
    $script:TransformLog.Add($entry)
    $script:TransformsApplied++
    if ($VerbosePreference -eq 'Continue') {
        Write-Verbose "  [$Transform] $Detail"
    }
}

function Write-ManualItem {
    param(
        [string]$File,
        [string]$Category,
        [string]$Detail
    )
    $entry = [PSCustomObject]@{
        File     = $File
        Category = $Category
        Detail   = $Detail
    }
    $script:ManualItems.Add($entry)
}

#endregion

#region --- Database Provider Detection ---

function Find-DatabaseProvider {
    param(
        [string]$SourcePath
    )

    $default = @{
        PackageName      = 'Microsoft.EntityFrameworkCore.SqlServer'
        ProviderMethod   = 'UseSqlServer'
        DetectedFrom     = 'Default — no Web.config connectionStrings found'
        ConnectionString = ''
    }

    if (-not $SourcePath) { return $default }

    # Look for Web.config in source path and parent directory
    $candidates = @(
        (Join-Path $SourcePath 'Web.config'),
        (Join-Path (Split-Path $SourcePath -Parent) 'Web.config')
    )
    $webConfigPath = $null
    foreach ($candidate in $candidates) {
        if (Test-Path $candidate) {
            $webConfigPath = $candidate
            break
        }
    }
    if (-not $webConfigPath) { return $default }

    try {
        [xml]$webConfig = Get-Content -Path $webConfigPath -Raw -Encoding UTF8
    }
    catch {
        return $default
    }

    $connStrings = $webConfig.configuration.connectionStrings
    if (-not $connStrings -or -not $connStrings.add) { return $default }

    # Provider mapping: Web.config providerName → EF Core package
    $providerMap = @{
        'System.Data.SqlClient'  = @{ PackageName = 'Microsoft.EntityFrameworkCore.SqlServer'; ProviderMethod = 'UseSqlServer' }
        'System.Data.SQLite'     = @{ PackageName = 'Microsoft.EntityFrameworkCore.Sqlite'; ProviderMethod = 'UseSqlite' }
        'Npgsql'                 = @{ PackageName = 'Npgsql.EntityFrameworkCore.PostgreSQL'; ProviderMethod = 'UseNpgsql' }
        'MySql.Data.MySqlClient' = @{ PackageName = 'Pomelo.EntityFrameworkCore.MySql'; ProviderMethod = 'UseMySql' }
    }

    $adds = @($connStrings.add)

    # Pass 1: Non-EntityClient entries with explicit providerName
    # Use GetAttribute() for StrictMode safety — returns '' if attribute missing
    foreach ($entry in $adds) {
        $providerName = $entry.GetAttribute('providerName')
        if (-not $providerName -or $providerName -eq 'System.Data.EntityClient') { continue }
        if ($providerMap.ContainsKey($providerName)) {
            $mapped = $providerMap[$providerName]
            return @{
                PackageName      = $mapped.PackageName
                ProviderMethod   = $mapped.ProviderMethod
                DetectedFrom     = "Web.config providerName=$providerName"
                ConnectionString = $entry.GetAttribute('connectionString')
            }
        }
    }

    # Pass 2: Entries without providerName — detect from connection string content
    foreach ($entry in $adds) {
        $connString = $entry.GetAttribute('connectionString')
        if (-not $connString -or $connString -match '^metadata=') { continue }
        if ($entry.GetAttribute('providerName')) { continue }
        if ($connString -match '(?i)\(LocalDB\)|Server=') {
            return @{
                PackageName      = 'Microsoft.EntityFrameworkCore.SqlServer'
                ProviderMethod   = 'UseSqlServer'
                DetectedFrom     = 'Web.config connection string pattern (SQL Server)'
                ConnectionString = $connString
            }
        }
        if ($connString -match '(?i)Data Source=.*\.db') {
            return @{
                PackageName      = 'Microsoft.EntityFrameworkCore.Sqlite'
                ProviderMethod   = 'UseSqlite'
                DetectedFrom     = 'Web.config connection string pattern (SQLite)'
                ConnectionString = $connString
            }
        }
    }

    # Pass 3: EntityClient entries — extract inner provider (EF6 pattern)
    foreach ($entry in $adds) {
        if ($entry.GetAttribute('providerName') -ne 'System.Data.EntityClient') { continue }
        $connString = $entry.GetAttribute('connectionString')
        if ($connString -match 'provider=([^;"]+)') {
            $innerProvider = $matches[1].Trim()
            if ($providerMap.ContainsKey($innerProvider)) {
                $mapped = $providerMap[$innerProvider]
                return @{
                    PackageName      = $mapped.PackageName
                    ProviderMethod   = $mapped.ProviderMethod
                    DetectedFrom     = "Web.config EntityClient provider=$innerProvider"
                    ConnectionString = ''
                }
            }
        }
    }

    return $default
}

#endregion

#region --- Project Scaffolding ---

function New-ProjectScaffold {
    param(
        [string]$OutputRoot,
        [string]$ProjectName,
        [string]$SourcePath
    )

    # RF-06: Detect features that need additional packages
    $hasModels = $false
    $hasIdentity = $false
    if ($SourcePath) {
        $modelsDir = Join-Path $SourcePath 'Models'
        $hasModels = Test-Path $modelsDir -PathType Container
        $accountDir = Join-Path $SourcePath 'Account'
        $hasIdentity = (Test-Path $accountDir -PathType Container) -or
                       (Test-Path (Join-Path $SourcePath 'Login.aspx')) -or
                       (Test-Path (Join-Path $SourcePath 'Register.aspx'))
    }

    # Detect database provider from Web.config
    $dbProvider = Find-DatabaseProvider -SourcePath $SourcePath
    if ($hasModels -and $dbProvider.DetectedFrom -notlike 'Default*') {
        switch ($dbProvider.ProviderMethod) {
            'UseSqlServer' { $friendlyName = 'SQL Server' }
            'UseSqlite'    { $friendlyName = 'SQLite' }
            'UseNpgsql'    { $friendlyName = 'PostgreSQL' }
            'UseMySql'     { $friendlyName = 'MySQL' }
            default        { $friendlyName = $dbProvider.ProviderMethod }
        }
        $providerDetail = "Detected $friendlyName provider — use $($dbProvider.PackageName)"
        if ($dbProvider.ConnectionString) {
            $providerDetail += " with connection string: $($dbProvider.ConnectionString)"
        }
        Write-ManualItem -File 'Web.config' -Category 'DatabaseProvider' -Detail $providerDetail
    }

    # RF-06: Build conditional package references
    $additionalPackages = ''
    if ($hasModels) {
        $additionalPackages += "`n    <PackageReference Include=`"$($dbProvider.PackageName)`" Version=`"10.0.0`" />"
        $additionalPackages += "`n    <PackageReference Include=`"Microsoft.EntityFrameworkCore.Tools`" Version=`"10.0.0`" />"
    }
    if ($hasIdentity) {
        $additionalPackages += "`n    <PackageReference Include=`"Microsoft.AspNetCore.Identity.EntityFrameworkCore`" Version=`"10.0.0`" />"
        $additionalPackages += "`n    <PackageReference Include=`"Microsoft.AspNetCore.Identity.UI`" Version=`"10.0.0`" />"
        $additionalPackages += "`n    <PackageReference Include=`"Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore`" Version=`"10.0.0`" />"
    }

    $bwfcReference = '<PackageReference Include="Fritz.BlazorWebFormsComponents" Version="*" />'
    $currentDir = [System.IO.DirectoryInfo]::new([System.IO.Path]::GetFullPath($OutputRoot))
    while ($null -ne $currentDir) {
        $candidate = Join-Path $currentDir.FullName 'src\BlazorWebFormsComponents\BlazorWebFormsComponents.csproj'
        if (Test-Path $candidate) {
            $relative = [System.IO.Path]::GetRelativePath([System.IO.Path]::GetFullPath($OutputRoot), $candidate) -replace '/', '\'
            $bwfcReference = "<ProjectReference Include=`"$relative`" />"
            break
        }
        $currentDir = $currentDir.Parent
    }

    # .csproj
    $csprojContent = @"
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <BwfcMigrationMode>true</BwfcMigrationMode>
  </PropertyGroup>

  <ItemGroup>
    ${bwfcReference}${additionalPackages}
  </ItemGroup>

</Project>
"@

    # _Imports.razor — conditionally include BlazorAjaxToolkitComponents
    $ajaxToolkitLine = ''
    # $script:HasAjaxToolkitControls is set during ConvertFrom-AjaxToolkitPrefix
    # At scaffold time it's not yet known, so always include the @using.
    # A post-processing pass after all files are processed will remove it if unused.
    $importsContent = @"
@using System.Net.Http
@using Microsoft.AspNetCore.Authorization
@using Microsoft.AspNetCore.Components.Authorization
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.JSInterop
@using BlazorWebFormsComponents
@using BlazorWebFormsComponents.LoginControls
@using $ProjectName
@using $ProjectName.Models
@inherits BlazorWebFormsComponents.WebFormsPageBase
"@

    # Program.cs
    $programContent = @"
// TODO: Review and adjust this generated Program.cs for your application needs.
// Generated for .NET 10 Blazor static SSR. Keep interactive render modes opt-in and page-specific.
using BlazorWebFormsComponents;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents();

builder.Services.AddBlazorWebFormsComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseAntiforgery();

app.MapRazorComponents<$ProjectName.Components.App>();

app.Run();
"@

    # Build dynamic DbContext line for Program.cs scaffold using detected provider
    if ($dbProvider.ConnectionString) {
        $escapedConnStr = $dbProvider.ConnectionString -replace '\\', '\\'
        $dbContextLine = "// builder.Services.AddDbContextFactory<YourDbContext>(options => options.$($dbProvider.ProviderMethod)(`"$escapedConnStr`"));"
    } else {
        $dbContextLine = "// builder.Services.AddDbContextFactory<YourDbContext>(options => options.$($dbProvider.ProviderMethod)(`"your-connection-string`"));"
    }

    # RF-07: Add Identity/Session boilerplate when detected
    if ($hasIdentity) {
        $identityServiceBlock = @"

// TODO: Configure database connection (use AddDbContextFactory — do NOT also register AddDbContext to avoid DI conflicts)
$dbContextLine

// TODO: Configure Identity
// builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
//     .AddEntityFrameworkStores<ProductContext>();

// TODO: Configure session for cart/state management
// builder.Services.AddDistributedMemoryCache();
// builder.Services.AddSession();
// builder.Services.AddHttpContextAccessor();
// builder.Services.AddCascadingAuthenticationState();
"@
        $programContent = $programContent.Replace(
            'builder.Services.AddBlazorWebFormsComponents();',
            "builder.Services.AddBlazorWebFormsComponents();`n$identityServiceBlock"
        )

        $identityMiddlewareBlock = @"

// TODO: Add middleware in the pipeline
// app.UseSession();
// app.UseAuthentication();
// app.UseAuthorization();
"@
        $programContent = $programContent.Replace(
            'app.UseAntiforgery();',
            "app.UseAntiforgery();`n$identityMiddlewareBlock"
        )
    }
    elseif ($hasModels) {
        # Add DbContext comment when Models/ detected but no Identity
        $modelsServiceBlock = @"

// TODO: Configure database connection (use AddDbContextFactory — do NOT also register AddDbContext to avoid DI conflicts)
$dbContextLine
"@
        $programContent = $programContent.Replace(
            'builder.Services.AddBlazorWebFormsComponents();',
            "builder.Services.AddBlazorWebFormsComponents();`n$modelsServiceBlock"
        )
    }
    # GlobalUsings.cs — additional global usings for migration projects.
    # NOTE: The BWFC NuGet package / ProjectReference automatically provides:
    #   - global using BlazorWebFormsComponents;
    #   - global using BlazorWebFormsComponents.LoginControls;
    #   - Page = WebFormsPageBase, MasterPage = LayoutComponentBase
    #   - ImageClickEventArgs = MouseEventArgs
    # via buildTransitive/Fritz.BlazorWebFormsComponents.targets.
    # This file adds Blazor infrastructure usings not covered by the .targets.
    $globalUsingsContent = @"
// =============================================================================
// Global using directives for Web Forms → Blazor migration.
//
// Type aliases (Page, MasterPage, ImageClickEventArgs) and BWFC namespaces are
// provided automatically by the BlazorWebFormsComponents .targets file.
// This file adds Blazor infrastructure usings for code-behind files.
//
// Generated by bwfc-migrate.ps1 — Layer 1 scaffold
// =============================================================================

// Blazor component infrastructure — provides [Inject], [Parameter],
// [SupplyParameterFromQuery], NavigationManager, ComponentBase, etc.
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Web;
global using Microsoft.AspNetCore.Components.Routing;
"@

    $launchSettingsContent = @"
{
  "profiles": {
    "$ProjectName": {
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
"@

    if ($PSCmdlet.ShouldProcess($OutputRoot, "Create project scaffold")) {
        $csprojPath = Join-Path $OutputRoot "$ProjectName.csproj"
        $importsPath = Join-Path $OutputRoot "_Imports.razor"
        $programPath = Join-Path $OutputRoot "Program.cs"
        $globalUsingsPath = Join-Path $OutputRoot "GlobalUsings.cs"
        $propertiesDir = Join-Path $OutputRoot "Properties"
        $launchSettingsPath = Join-Path $propertiesDir "launchSettings.json"

        Set-Content -Path $csprojPath -Value $csprojContent -Encoding UTF8
        Write-TransformLog -File $csprojPath -Transform 'Scaffold' -Detail "Generated $ProjectName.csproj"

        Set-Content -Path $importsPath -Value $importsContent -Encoding UTF8
        Write-TransformLog -File $importsPath -Transform 'Scaffold' -Detail 'Generated _Imports.razor'

        Set-Content -Path $programPath -Value $programContent -Encoding UTF8
        Write-TransformLog -File $programPath -Transform 'Scaffold' -Detail 'Generated Program.cs'

        Set-Content -Path $globalUsingsPath -Value $globalUsingsContent -Encoding UTF8
        Write-TransformLog -File $globalUsingsPath -Transform 'Scaffold' -Detail 'Generated GlobalUsings.cs (Blazor infrastructure usings)'

        New-Item -ItemType Directory -Force $propertiesDir | Out-Null
        Set-Content -Path $launchSettingsPath -Value $launchSettingsContent -Encoding UTF8
        Write-TransformLog -File $launchSettingsPath -Transform 'Scaffold' -Detail 'Generated Properties/launchSettings.json'
    }
    else {
        Write-Host "[WhatIf] Would create: $ProjectName.csproj"
        Write-Host "[WhatIf] Would create: _Imports.razor"
        Write-Host "[WhatIf] Would create: Program.cs"
        Write-Host "[WhatIf] Would create: GlobalUsings.cs"
        Write-Host "[WhatIf] Would create: Properties/launchSettings.json"
    }
}

function New-AppRazorScaffold {
    param(
        [string]$OutputRoot,
        [string]$ProjectName
    )

    $componentsDir = Join-Path $OutputRoot "Components"

    $appRazorContent = @"
<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <base href="/" />
    <HeadOutlet />
</head>

@* Generated for .NET 10 static SSR migration output. Only opt into interactive render modes deliberately and per page. *@
<body>
    <Routes />
</body>

</html>
"@

    $routesRazorContent = @"
<Router AppAssembly="typeof(Program).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="routeData" DefaultLayout="typeof(Layout.MainLayout)" />
        <FocusOnNavigate RouteData="routeData" Selector="h1" />
    </Found>
</Router>
"@

    if ($PSCmdlet.ShouldProcess($componentsDir, "Create App.razor and Routes.razor scaffold")) {
        if (-not (Test-Path $componentsDir)) {
            New-Item -ItemType Directory -Force $componentsDir | Out-Null
        }

        $appPath = Join-Path $componentsDir "App.razor"
        Set-Content -Path $appPath -Value $appRazorContent -Encoding UTF8
        Write-TransformLog -File $appPath -Transform 'Scaffold' -Detail 'Generated Components/App.razor'

        $routesPath = Join-Path $componentsDir "Routes.razor"
        Set-Content -Path $routesPath -Value $routesRazorContent -Encoding UTF8
        Write-TransformLog -File $routesPath -Transform 'Scaffold' -Detail 'Generated Components/Routes.razor'
    }
    else {
        Write-Host "[WhatIf] Would create: Components/App.razor"
        Write-Host "[WhatIf] Would create: Components/Routes.razor"
    }
}

function Invoke-CssAutoDetection {
    <#
    .SYNOPSIS
        Fix 1b: Scans wwwroot for CSS files and injects <link> tags into App.razor's <head>.
    #>
    param(
        [string]$OutputRoot,
        [string]$SourcePath
    )

    $appRazorPath = Join-Path $OutputRoot "Components" "App.razor"
    if (-not (Test-Path $appRazorPath)) {
        Write-Verbose "Invoke-CssAutoDetection: App.razor not found at $appRazorPath — skipping"
        return
    }

    $wwwroot = Join-Path $OutputRoot "wwwroot"
    if (-not (Test-Path $wwwroot -PathType Container)) {
        Write-Verbose "Invoke-CssAutoDetection: wwwroot not found — skipping"
        return
    }

    $cssLinks = [System.Collections.Generic.List[string]]::new()

    # Scan wwwroot/Content/ for CSS files
    $contentDir = Join-Path $wwwroot "Content"
    if (Test-Path $contentDir -PathType Container) {
        $cssFiles = Get-ChildItem -Path $contentDir -Filter '*.css' -Recurse -File
        foreach ($cf in $cssFiles) {
            $relCss = $cf.FullName.Substring($wwwroot.Length).TrimStart('\', '/') -replace '\\', '/'
            $cssLinks.Add("    <link rel=""stylesheet"" href=""/$relCss"" />")
        }
    }

    # Fallback: scan wwwroot/css/ if Content/ had nothing
    if ($cssLinks.Count -eq 0) {
        $cssDir = Join-Path $wwwroot "css"
        if (Test-Path $cssDir -PathType Container) {
            $cssFiles = Get-ChildItem -Path $cssDir -Filter '*.css' -Recurse -File
            foreach ($cf in $cssFiles) {
                $relCss = $cf.FullName.Substring($wwwroot.Length).TrimStart('\', '/') -replace '\\', '/'
                $cssLinks.Add("    <link rel=""stylesheet"" href=""/$relCss"" />")
            }
        }
    }

    # Also scan for Site.css or other root-level CSS in wwwroot
    $rootCssFiles = Get-ChildItem -Path $wwwroot -Filter '*.css' -File -ErrorAction SilentlyContinue
    foreach ($cf in $rootCssFiles) {
        $relCss = $cf.Name
        $linkTag = "    <link rel=""stylesheet"" href=""/$relCss"" />"
        if ($cssLinks -notcontains $linkTag) {
            $cssLinks.Add($linkTag)
        }
    }

    # Check source Site.Master for CDN references (Bootstrap, jQuery)
    $cdnLinks = [System.Collections.Generic.List[string]]::new()
    $masterFile = Get-ChildItem -Path $SourcePath -Filter 'Site.Master' -Recurse -File | Select-Object -First 1
    if ($masterFile) {
        $masterContent = Get-Content -Path $masterFile.FullName -Raw
        # Extract CDN <link> tags (stylesheets from CDNs)
        $cdnLinkRegex = [regex]'<link\s[^>]*href\s*=\s*"(https?://[^"]*(?:cdn\.|cloudflare|bootstrapcdn|googleapis|jsdelivr|unpkg|cdnjs)[^"]*)"[^>]*>'
        foreach ($m in $cdnLinkRegex.Matches($masterContent)) {
            $cdnLinks.Add("    " + $m.Value.Trim())
        }
        # Extract CDN <script> tags (jQuery, Bootstrap JS)
        $cdnScriptRegex = [regex]'<script\s[^>]*src\s*=\s*"(https?://[^"]*(?:cdn\.|cloudflare|bootstrapcdn|googleapis|jsdelivr|unpkg|cdnjs|jquery)[^"]*)"[^>]*>\s*</script>'
        foreach ($m in $cdnScriptRegex.Matches($masterContent)) {
            $cdnLinks.Add("    " + $m.Value.Trim())
        }
    }

    if ($cssLinks.Count -eq 0 -and $cdnLinks.Count -eq 0) {
        Write-Verbose "Invoke-CssAutoDetection: No CSS files or CDN links found — skipping"
        return
    }

    # Build injection block
    $injectionLines = [System.Collections.Generic.List[string]]::new()
    if ($cdnLinks.Count -gt 0) {
        $injectionLines.Add("    @* CDN references preserved from Site.Master *@")
        foreach ($cdn in $cdnLinks) { $injectionLines.Add($cdn) }
    }
    if ($cssLinks.Count -gt 0) {
        $injectionLines.Add("    @* Auto-detected CSS files from wwwroot *@")
        foreach ($css in $cssLinks) { $injectionLines.Add($css) }
    }
    $injectionBlock = ($injectionLines -join "`n") + "`n"

    # Inject into App.razor before <HeadOutlet>
    $appContent = Get-Content -Path $appRazorPath -Raw
    if ($appContent -match '<HeadOutlet') {
        $appContent = $appContent -replace '(\s*<HeadOutlet)', "`n$injectionBlock`$1"
        Set-Content -Path $appRazorPath -Value $appContent -Encoding UTF8
        $totalInjected = $cssLinks.Count + $cdnLinks.Count
        Write-Host "  Injected $totalInjected CSS/CDN reference(s) into App.razor <head>" -ForegroundColor Green
        Write-TransformLog -File 'Components/App.razor' -Transform 'CSSAutoDetect' -Detail "Injected $($cssLinks.Count) CSS link(s) and $($cdnLinks.Count) CDN reference(s)"
    }
    else {
        Write-Host "  WARNING: Could not find <HeadOutlet> in App.razor — CSS links not injected" -ForegroundColor Yellow
    }
}

function Invoke-ScriptAutoDetection {
    <#
    .SYNOPSIS
        Scans source project for Scripts/ folder, copies JS files to wwwroot/Scripts/,
        and injects <script> tags into App.razor before closing </body>.
    .DESCRIPTION
        Web Forms apps commonly have a Scripts/ folder with jQuery, Bootstrap JS, etc.,
        referenced via <asp:ScriptManager> or <webopt:bundlereference>. This function
        detects those JS files, copies them, and wires them into App.razor so the
        migrated app loads the same client-side scripts.
    #>
    param(
        [string]$OutputRoot,
        [string]$SourcePath
    )

    $appRazorPath = Join-Path $OutputRoot "Components" "App.razor"
    if (-not (Test-Path $appRazorPath)) {
        Write-Verbose "Invoke-ScriptAutoDetection: App.razor not found at $appRazorPath — skipping"
        return
    }

    # Check for Scripts/ folder in the source project
    $sourceScriptsDir = Join-Path $SourcePath "Scripts"
    if (-not (Test-Path $sourceScriptsDir -PathType Container)) {
        Write-Verbose "Invoke-ScriptAutoDetection: No Scripts/ folder in source — skipping"
        return
    }

    # Get all .js files (exclude WebForms-specific scripts and IntelliSense files)
    $jsFiles = Get-ChildItem -Path $sourceScriptsDir -Filter '*.js' -File | Where-Object {
        $_.Name -notlike '*intellisense*' -and
        $_.Name -ne '_references.js' -and
        $_.DirectoryName -notlike '*\WebForms*' -and
        $_.DirectoryName -notlike '*\WebForms'
    }

    if ($jsFiles.Count -eq 0) {
        Write-Verbose "Invoke-ScriptAutoDetection: No relevant JS files found in Scripts/ — skipping"
        return
    }

    # Copy JS files to wwwroot/Scripts/
    $destScriptsDir = Join-Path $OutputRoot "wwwroot" "Scripts"
    if (-not (Test-Path $destScriptsDir)) {
        New-Item -ItemType Directory -Path $destScriptsDir -Force | Out-Null
    }

    $copiedFiles = [System.Collections.Generic.List[string]]::new()
    foreach ($jsFile in $jsFiles) {
        $destFile = Join-Path $destScriptsDir $jsFile.Name
        Copy-Item -Path $jsFile.FullName -Destination $destFile -Force
        $copiedFiles.Add($jsFile.Name)
    }
    Write-Host "  Copied $($copiedFiles.Count) JS file(s) to wwwroot/Scripts/" -ForegroundColor Green
    Write-TransformLog -File 'wwwroot/Scripts/' -Transform 'ScriptAutoDetect' -Detail "Copied $($copiedFiles.Count) JS file(s) from source Scripts/"

    # Build <script> tags — prioritize common libraries in correct load order
    $scriptTags = [System.Collections.Generic.List[string]]::new()

    # Common JS load order: jQuery first, then Modernizr/Respond, then Bootstrap
    $jqueryFile = $copiedFiles | Where-Object { $_ -match '^jquery.*\.min\.js$' } | Select-Object -First 1
    if (-not $jqueryFile) {
        $jqueryFile = $copiedFiles | Where-Object { $_ -match '^jquery-[\d.]+\.js$' } | Select-Object -First 1
    }
    $modernizrFile = $copiedFiles | Where-Object { $_ -match '^modernizr.*\.js$' } | Select-Object -First 1
    $respondFile = $copiedFiles | Where-Object { $_ -match '^respond.*\.min\.js$' } | Select-Object -First 1
    if (-not $respondFile) {
        $respondFile = $copiedFiles | Where-Object { $_ -match '^respond.*\.js$' } | Select-Object -First 1
    }
    $bootstrapFile = $copiedFiles | Where-Object { $_ -match '^bootstrap.*\.min\.js$' } | Select-Object -First 1
    if (-not $bootstrapFile) {
        $bootstrapFile = $copiedFiles | Where-Object { $_ -match '^bootstrap.*\.js$' } | Select-Object -First 1
    }

    # Add in correct dependency order
    if ($jqueryFile) { $scriptTags.Add("    <script src=""/Scripts/$jqueryFile""></script>") }
    if ($modernizrFile) { $scriptTags.Add("    <script src=""/Scripts/$modernizrFile""></script>") }
    if ($respondFile) { $scriptTags.Add("    <script src=""/Scripts/$respondFile""></script>") }
    if ($bootstrapFile) { $scriptTags.Add("    <script src=""/Scripts/$bootstrapFile""></script>") }

    # Add any remaining JS files not already included
    $knownFiles = @($jqueryFile, $modernizrFile, $respondFile, $bootstrapFile) | Where-Object { $_ }
    foreach ($f in $copiedFiles) {
        if ($f -notin $knownFiles) {
            $scriptTags.Add("    <script src=""/Scripts/$f""></script>")
        }
    }

    if ($scriptTags.Count -eq 0) {
        return
    }

    # Inject <script> tags into App.razor before closing </body>
    $appContent = Get-Content -Path $appRazorPath -Raw
    $injectionBlock = "    @* Auto-detected JS files from Scripts/ *@`n" + ($scriptTags -join "`n") + "`n"
    if ($appContent -match '</body>') {
        $appContent = $appContent -replace '(\s*</body>)', "`n$injectionBlock`$1"
        Set-Content -Path $appRazorPath -Value $appContent -Encoding UTF8
        Write-Host "  Injected $($scriptTags.Count) <script> tag(s) into App.razor <body>" -ForegroundColor Green
        Write-TransformLog -File 'Components/App.razor' -Transform 'ScriptAutoDetect' -Detail "Injected $($scriptTags.Count) script tag(s) for JS files from Scripts/"
    }
    else {
        Write-Host "  WARNING: Could not find </body> in App.razor — script tags not injected" -ForegroundColor Yellow
    }

    # Also check Site.Master for <webopt:bundlereference> targeting Scripts and flag
    $masterFile = Get-ChildItem -Path $SourcePath -Filter 'Site.Master' -Recurse -File | Select-Object -First 1
    if ($masterFile) {
        $masterContent = Get-Content -Path $masterFile.FullName -Raw
        $scriptBundleRegex = [regex]'(?i)<webopt:bundlereference[^>]*path\s*=\s*"[^"]*[Ss]cripts[^"]*"[^>]*>'
        foreach ($m in $scriptBundleRegex.Matches($masterContent)) {
            $bundlePath = ''
            if ($m.Value -match 'path\s*=\s*"([^"]*)"') { $bundlePath = $Matches[1] }
            Write-ManualItem -File 'Site.Master' -Category 'ScriptBundle' -Detail "Script bundle reference '$bundlePath' — JS files auto-copied to wwwroot/Scripts/ and wired into App.razor"
            Write-TransformLog -File 'Site.Master' -Transform 'ScriptAutoDetect' -Detail "Flagged <webopt:bundlereference> for scripts: $bundlePath"
        }
    }
}

#endregion

#region --- Web.config → appsettings.json (GAP-12) ---

function Convert-WebConfigToAppSettings {
    <#
    .SYNOPSIS
        Parses Web.config for appSettings and connectionStrings, generates appsettings.json (GAP-12).
    .DESCRIPTION
        Reads <appSettings> keys and <connectionStrings> entries from the source project's Web.config
        and generates an appsettings.json in the output Blazor project. Merges with any existing
        appsettings.json content the scaffold already generates.
    #>
    param(
        [string]$SourcePath,
        [string]$OutputRoot
    )

    # Find Web.config
    $webConfigPath = Join-Path $SourcePath 'Web.config'
    if (-not (Test-Path $webConfigPath)) {
        $webConfigPath = Join-Path $SourcePath 'web.config'
    }
    if (-not (Test-Path $webConfigPath)) {
        Write-Host '  No Web.config found — skipping appsettings.json generation' -ForegroundColor DarkGray
        return
    }

    try {
        [xml]$webConfig = Get-Content -Path $webConfigPath -Raw -Encoding UTF8

        $appSettings = @{}
        $connectionStrings = @{}

        # Parse <appSettings>
        $appSettingsNode = $webConfig.SelectNodes('//appSettings/add')
        if ($appSettingsNode -and $appSettingsNode.Count -gt 0) {
            foreach ($node in $appSettingsNode) {
                $key = $node.GetAttribute('key')
                $value = $node.GetAttribute('value')
                if ($key) {
                    $appSettings[$key] = $value
                }
            }
        }

        # Parse <connectionStrings>
        $connStrNodes = $webConfig.SelectNodes('//connectionStrings/add')
        if ($connStrNodes -and $connStrNodes.Count -gt 0) {
            foreach ($node in $connStrNodes) {
                $name = $node.GetAttribute('name')
                $connStr = $node.GetAttribute('connectionString')
                if ($name -and $connStr) {
                    # Skip LocalSqlServer and other built-in connection strings
                    if ($name -ne 'LocalSqlServer' -and $name -ne 'LocalMySqlServer') {
                        $connectionStrings[$name] = $connStr
                    }
                }
            }
        }

        if ($appSettings.Count -eq 0 -and $connectionStrings.Count -eq 0) {
            Write-Host '  Web.config has no appSettings or connectionStrings — skipping' -ForegroundColor DarkGray
            return
        }

        # Build the appsettings.json structure
        $jsonObj = [ordered]@{}

        # Add connection strings section
        if ($connectionStrings.Count -gt 0) {
            $connStrSection = [ordered]@{}
            foreach ($entry in $connectionStrings.GetEnumerator()) {
                $connStrSection[$entry.Key] = $entry.Value
            }
            $jsonObj['ConnectionStrings'] = $connStrSection
        }

        # Add app settings as top-level keys
        foreach ($entry in $appSettings.GetEnumerator()) {
            $jsonObj[$entry.Key] = $entry.Value
        }

        # Add standard Blazor sections
        if (-not $jsonObj.Contains('Logging')) {
            $jsonObj['Logging'] = [ordered]@{
                'LogLevel' = [ordered]@{
                    'Default' = 'Information'
                    'Microsoft.AspNetCore' = 'Warning'
                }
            }
        }
        if (-not $jsonObj.Contains('AllowedHosts')) {
            $jsonObj['AllowedHosts'] = '*'
        }

        $appSettingsPath = Join-Path $OutputRoot 'appsettings.json'

        # Merge with existing appsettings.json if it was already generated
        if (Test-Path $appSettingsPath) {
            try {
                $existingJson = Get-Content -Path $appSettingsPath -Raw -Encoding UTF8 | ConvertFrom-Json -AsHashtable
                # Merge: existing values take precedence for duplicate keys
                foreach ($key in $existingJson.Keys) {
                    if (-not $jsonObj.Contains($key)) {
                        $jsonObj[$key] = $existingJson[$key]
                    }
                    elseif ($key -eq 'ConnectionStrings' -and $existingJson[$key] -is [hashtable]) {
                        # Merge connection strings
                        foreach ($csKey in $existingJson[$key].Keys) {
                            if (-not $jsonObj['ConnectionStrings'].Contains($csKey)) {
                                $jsonObj['ConnectionStrings'][$csKey] = $existingJson[$key][$csKey]
                            }
                        }
                    }
                }
            }
            catch {
                # If existing file can't be parsed, overwrite it
                Write-Host "  Warning: Could not parse existing appsettings.json — overwriting" -ForegroundColor Yellow
            }
        }

        if ($PSCmdlet.ShouldProcess($appSettingsPath, "Generate appsettings.json from Web.config")) {
            $jsonContent = $jsonObj | ConvertTo-Json -Depth 4
            Set-Content -Path $appSettingsPath -Value $jsonContent -Encoding UTF8

            $totalKeys = $appSettings.Count + $connectionStrings.Count
            Write-Host "  Generated appsettings.json ($($appSettings.Count) app settings, $($connectionStrings.Count) connection strings)" -ForegroundColor Green
            Write-TransformLog -File 'appsettings.json' -Transform 'WebConfig' -Detail "Extracted $($appSettings.Count) appSettings key(s) and $($connectionStrings.Count) connectionString(s) from Web.config"

            if ($appSettings.Count -gt 0) {
                $keyList = ($appSettings.Keys | Sort-Object) -join ', '
                Write-TransformLog -File 'appsettings.json' -Transform 'WebConfig' -Detail "AppSettings keys: $keyList"
            }
            if ($connectionStrings.Count -gt 0) {
                $connList = ($connectionStrings.Keys | Sort-Object) -join ', '
                Write-TransformLog -File 'appsettings.json' -Transform 'WebConfig' -Detail "ConnectionStrings: $connList"
                Write-ManualItem -File 'appsettings.json' -Category 'ConnectionString' -Detail "Connection strings extracted from Web.config — verify and update for target environment: $connList"
            }
        }
        else {
            Write-Host "[WhatIf] Would generate appsettings.json from Web.config"
        }
    }
    catch {
        Write-Host "  Warning: Could not parse Web.config — $($_.Exception.Message)" -ForegroundColor Yellow
        Write-ManualItem -File 'Web.config' -Category 'ParseError' -Detail "Could not parse Web.config for appsettings.json generation: $($_.Exception.Message)"
    }
}

#endregion

#region --- App_Start Directory Copy (GAP-22) ---

function Copy-AppStart {
    <#
    .SYNOPSIS
        Copies App_Start/*.cs files to the output project with cleanup (GAP-22).
    .DESCRIPTION
        Copies C# files from the source project's App_Start/ directory,
        strips Web Forms usings, removes [assembly: ...] attributes,
        and adds TODO comments for patterns needing manual review.
    #>
    param(
        [string]$SourcePath,
        [string]$OutputRoot
    )

    $appStartDir = Join-Path $SourcePath 'App_Start'
    if (-not (Test-Path $appStartDir -PathType Container)) {
        return 0
    }

    $csFiles = @(Get-ChildItem -Path $appStartDir -Filter '*.cs' -File)
    if ($csFiles.Count -eq 0) {
        return 0
    }

    $copied = 0
    Write-Host "Copying $($csFiles.Count) App_Start file(s)..." -ForegroundColor Green

    foreach ($file in $csFiles) {
        $relPath = "App_Start/$($file.Name)"
        # Copy to root of output project (not App_Start/ subfolder — Blazor has no App_Start convention)
        $destFile = Join-Path $OutputRoot $file.Name

        if ($PSCmdlet.ShouldProcess($destFile, "Copy App_Start file")) {
            $csContent = Get-Content -Path $file.FullName -Raw -Encoding UTF8

            # Add TODO header
            $csContent = "// TODO: Review — auto-copied from App_Start/$($file.Name). Blazor has no App_Start convention.`n// TODO: Move relevant configuration to Program.cs or appropriate service registration.`n`n" + $csContent

            # Strip [assembly: ...] attributes (can span multiple lines)
            $assemblyAttrRegex = [regex]'(?m)^\s*\[assembly:\s*[^\]]*\]\s*\r?\n?'
            $assemblyAttrMatches = $assemblyAttrRegex.Matches($csContent)
            if ($assemblyAttrMatches.Count -gt 0) {
                $csContent = $assemblyAttrRegex.Replace($csContent, '')
                Write-TransformLog -File $relPath -Transform 'AppStart' -Detail "Stripped $($assemblyAttrMatches.Count) [assembly:] attribute(s)"
            }

            # Strip System.Web.UI.* usings
            $csContent = $csContent -replace 'using\s+System\.Web\.UI(\.\w+)*;\s*\r?\n?', ''

            # Strip System.Web.Security
            $csContent = $csContent -replace 'using\s+System\.Web\.Security;\s*\r?\n?', ''

            # Selectively handle System.Web.Optimization (BundleConfig stubs in BWFC)
            if ($csContent -match 'Bundle|BundleTable|BundleCollection') {
                $csContent = $csContent -replace 'using\s+System\.Web\.Optimization;\s*\r?\n?', "// using System.Web.Optimization; // BWFC: BundleConfig stubs available via BlazorWebFormsComponents namespace`n"
            } else {
                $csContent = $csContent -replace 'using\s+System\.Web\.Optimization;\s*\r?\n?', ''
            }

            # Selectively handle System.Web.Routing (RouteConfig stubs in BWFC)
            if ($csContent -match 'Route|RouteTable|RouteCollection') {
                $csContent = $csContent -replace 'using\s+System\.Web\.Routing;\s*\r?\n?', "// using System.Web.Routing; // BWFC: RouteConfig stubs available via BlazorWebFormsComponents namespace`n"
            } else {
                $csContent = $csContent -replace 'using\s+System\.Web\.Routing;\s*\r?\n?', ''
            }

            # Strip remaining System.Web.* usings
            $csContent = $csContent -replace 'using\s+System\.Web(\.\w+)*;\s*\r?\n?', ''

            # Strip Microsoft.AspNet.* usings
            $csContent = $csContent -replace 'using\s+Microsoft\.AspNet(\.\w+)*;\s*\r?\n?', ''

            # Strip Microsoft.Owin.* usings
            $csContent = $csContent -replace 'using\s+Microsoft\.Owin(\.\w+)*;\s*\r?\n?', ''

            # Strip Owin usings
            $csContent = $csContent -replace 'using\s+Owin;\s*\r?\n?', ''

            # Add TODO for common patterns that need manual review
            if ($csContent -match 'WebApiConfig|GlobalConfiguration') {
                $csContent = $csContent -replace '(class\s+WebApiConfig)', '// TODO: BWFC — Web API configuration should be migrated to minimal API endpoints in Program.cs`n$1'
                Write-ManualItem -File $relPath -Category 'AppStart' -Detail "WebApiConfig detected — migrate to minimal API endpoints in Program.cs"
            }
            if ($csContent -match 'FilterConfig|GlobalFilters') {
                $csContent = $csContent -replace '(class\s+FilterConfig)', '// TODO: BWFC — Filter configuration should use ASP.NET Core middleware pipeline in Program.cs`n$1'
                Write-ManualItem -File $relPath -Category 'AppStart' -Detail "FilterConfig detected — migrate to middleware pipeline in Program.cs"
            }

            Set-Content -Path $destFile -Value $csContent -Encoding UTF8
            Write-TransformLog -File $relPath -Transform 'AppStart' -Detail "Copied App_Start/$($file.Name) → $($file.Name) (root)"
            $copied++
        }
        else {
            Write-Host "[WhatIf] Would copy App_Start: $relPath → $($file.Name)"
        }
    }

    return $copied
}

#endregion

#region --- Directive Transforms ---

function ConvertFrom-PageDirective {
    param([string]$Content, [string]$FileName, [string]$RelPath, [string]$SourceFile = '')

    # <%@ Page ... %> → @page "/route"
    $route = '/' + [System.IO.Path]::GetFileNameWithoutExtension($FileName)
    $isHomePage = $false
    if ($route -eq '/Default' -or $route -eq '/default' -or $route -eq '/Index' -or $route -eq '/index') {
        $route = '/'
        $isHomePage = $true
    }
    # FIX 2: Also detect home page names for dual-route generation
    if ($FileName -match '^(Home|home)\.aspx$') {
        $isHomePage = $true
    }

    if ($Content -match '<%@\s*Page[^%]*%>') {
        # RF-10: Extract Title attribute before stripping the directive
        $pageTitle = $null
        if ($Content -match '<%@\s*Page[^%]*Title\s*=\s*"([^"]*)"') {
            $pageTitle = $Matches[1]
        }

        $Content = $Content -replace '<%@\s*Page[^%]*%>\s*\r?\n?', ''
        $pageHeader = "@page `"$route`"`n"

        # FIX 2: Add root route for home pages
        if ($isHomePage -and $route -ne '/') {
            $pageHeader += "@page `"/`"`n"
            Write-TransformLog -File $RelPath -Transform 'Directive' -Detail "Added @page `"/`" root route for home page"
        }

        # FIX 3: Use title from asp:Content TitleContent if available
        if (-not $pageTitle -and $script:ExtractedTitleFromContent) {
            $pageTitle = $script:ExtractedTitleFromContent
            $script:ExtractedTitleFromContent = $null  # Clear for next file
        }

        # RF-10: Add <PageTitle> if title was extracted — but skip if code-behind
        # sets Page.Title (L2 will emit that assignment, so emitting <PageTitle>
        # here would create a duplicate)
        if ($pageTitle) {
            $codeBehindSetsTitle = $false
            $cbPath = if ($SourceFile) { $SourceFile + '.cs' } else { '' }
            if ($cbPath -and (Test-Path $cbPath)) {
                $cbContent = Get-Content -Path $cbPath -Raw -Encoding UTF8
                if ($cbContent -match 'Page\.Title\s*=') {
                    $codeBehindSetsTitle = $true
                }
            }

            if ($codeBehindSetsTitle) {
                Write-TransformLog -File $RelPath -Transform 'PageTitle' -Detail "Suppressed <PageTitle> for Title=`"$pageTitle`" — code-behind sets Page.Title (L2 handles)"
            } else {
                $pageHeader += "<PageTitle>$pageTitle</PageTitle>`n"
                Write-TransformLog -File $RelPath -Transform 'PageTitle' -Detail "Extracted Title=`"$pageTitle`" → <PageTitle>"
            }
        }

        $Content = $pageHeader + $Content
        Write-TransformLog -File $RelPath -Transform 'Directive' -Detail "<%@ Page %> → @page `"$route`""
    }
    return $Content
}

function ConvertFrom-MasterDirective {
    param([string]$Content, [string]$RelPath)

    if ($Content -match '<%@\s*Master[^%]*%>') {
        $Content = $Content -replace '<%@\s*Master[^%]*%>\s*\r?\n?', ''
        Write-TransformLog -File $RelPath -Transform 'Directive' -Detail 'Removed <%@ Master %>'
    }
    return $Content
}

function ConvertFrom-ControlDirective {
    param([string]$Content, [string]$RelPath)

    if ($Content -match '<%@\s*Control[^%]*%>') {
        $Content = $Content -replace '<%@\s*Control[^%]*%>\s*\r?\n?', ''
        Write-TransformLog -File $RelPath -Transform 'Directive' -Detail 'Removed <%@ Control %>'
    }
    return $Content
}

function ConvertFrom-RegisterDirective {
    param([string]$Content, [string]$RelPath)

    $regex = [regex]'<%@\s*Register[^%]*%>\s*\r?\n?'
    $matches_ = $regex.Matches($Content)
    foreach ($m in $matches_) {
        Write-TransformLog -File $RelPath -Transform 'Directive' -Detail "Removed <%@ Register %> directive"
        if ($m.Value -match 'TagPrefix\s*=\s*"[Uu][Cc]"') {
            Write-TransformLog -File $RelPath -Transform 'Directive' -Detail "Register directive contained uc: TagPrefix — verify user control tags are converted"
        }
        Write-ManualItem -File $RelPath -Category 'RegisterDirective' -Detail "Register directive removed — verify tag prefixes in converted markup are handled"
    }
    $Content = $regex.Replace($Content, '')
    return $Content
}

function ConvertFrom-ImportDirective {
    param([string]$Content, [string]$RelPath)

    $regex = [regex]'<%@\s*Import\s+Namespace="([^"]+)"\s*%>\s*\r?\n?'
    $matches_ = $regex.Matches($Content)
    foreach ($m in $matches_) {
        $ns = $m.Groups[1].Value
        Write-TransformLog -File $RelPath -Transform 'Directive' -Detail "<%@ Import Namespace=`"$ns`" %> → @using $ns"
    }
    $Content = $regex.Replace($Content, { param($m) "@using $($m.Groups[1].Value)`n" })
    return $Content
}

#endregion

#region --- Content & Form Transforms ---

function ConvertFrom-ContentWrappers {
    param([string]$Content, [string]$RelPath)

    # FIX 3: Extract title from TitleContent asp:Content placeholder
    $titleContentRegex = [regex]'(?si)<asp:Content\s+[^>]*ContentPlaceHolderID\s*=\s*"[^"]*Title[^"]*"[^>]*>\s*([^<]+?)\s*</asp:Content>'
    $titleFromContent = $null
    if ($titleContentRegex.IsMatch($Content)) {
        $titleMatch = $titleContentRegex.Match($Content)
        $titleFromContent = $titleMatch.Groups[1].Value.Trim()
    }

    # HeadContent placeholder → <HeadContent> / </HeadContent>
    $headOpenRegex = [regex]'<asp:Content\s+[^>]*ContentPlaceHolderID\s*=\s*"HeadContent"[^>]*>'
    $headOpenRegex2 = [regex]'<asp:Content\s+[^>]*ContentPlaceHolderID\s*=\s*"head"[^>]*>'
    if ($Content -match $headOpenRegex -or $Content -match $headOpenRegex2) {
        $Content = $headOpenRegex.Replace($Content, '<HeadContent>')
        $Content = $headOpenRegex2.Replace($Content, '<HeadContent>')
        Write-TransformLog -File $RelPath -Transform 'Content' -Detail 'HeadContent placeholder → <HeadContent>'
    }

    # MainContent / other ContentPlaceHolderIDs → strip wrapper entirely
    # Use [ \t]* (horizontal whitespace only) to avoid consuming indentation on the next line
    $mainRegex = [regex]'<asp:Content\s+[^>]*ContentPlaceHolderID\s*=\s*"[^"]*"[^>]*>[ \t]*\r?\n?'
    $matches_ = $mainRegex.Matches($Content)
    foreach ($m in $matches_) {
        Write-TransformLog -File $RelPath -Transform 'Content' -Detail "Removed asp:Content open tag"
    }
    $Content = $mainRegex.Replace($Content, '')

    # Closing </asp:Content> tags
    $closeRegex = [regex]'</asp:Content>\s*\r?\n?'
    $closeCount = $closeRegex.Matches($Content).Count
    if ($closeCount -gt 0) {
        # Keep matching number of </HeadContent> if we converted HeadContent above
        $headContentCount = ([regex]'<HeadContent>').Matches($Content).Count
        if ($headContentCount -gt 0) {
            # Replace first N closing tags with </HeadContent>, remove the rest
            $script:replaced_count = 0
            $Content = $closeRegex.Replace($Content, {
                param($m)
                $script:replaced_count++
                if ($script:replaced_count -le $headContentCount) {
                    return "</HeadContent>`n"
                }
                return ''
            })
            # Reset and redo with proper scoping
            $script:replaced_count = 0
            $tempContent = $Content
            $Content = ''
            $lastIndex = 0
            $closeMatches = $closeRegex.Matches($tempContent)
            $headCount = 0
            foreach ($cm in $closeMatches) {
                $Content += $tempContent.Substring($lastIndex, $cm.Index - $lastIndex)
                $headCount++
                if ($headCount -le $headContentCount) {
                    $Content += "</HeadContent>`n"
                }
                $lastIndex = $cm.Index + $cm.Length
            }
            $Content += $tempContent.Substring($lastIndex)
        }
        else {
            $Content = $closeRegex.Replace($Content, '')
        }
        Write-TransformLog -File $RelPath -Transform 'Content' -Detail "Removed $closeCount </asp:Content> closing tag(s)"
    }

    # FIX 3: Return the extracted title so ConvertFrom-PageDirective can use it
    $script:ExtractedTitleFromContent = $titleFromContent

    return $Content
}

function ConvertFrom-FormWrapper {
    param([string]$Content, [string]$RelPath)

    # Replace <form ... runat="server" ...> with <div> to preserve block formatting context.
    # CSS often depends on the form wrapper for positioning calculations (margin collapse,
    # containing block for position:relative, width inheritance). Stripping the form entirely
    # breaks layouts. Preserve the id attribute if present so existing CSS/JS continues to work.
    $formOpenRegex = [regex]'<form\s+([^>]*)runat\s*=\s*"server"([^>]*)>'
    $formMatch = $formOpenRegex.Match($Content)
    if ($formMatch.Success) {
        # Extract id attribute if present
        $fullAttrs = $formMatch.Groups[1].Value + $formMatch.Groups[2].Value
        $idAttr = ''
        if ($fullAttrs -match 'id\s*=\s*"([^"]*)"') {
            $idAttr = " id=""$($Matches[1])"""
        }
        # Replace opening <form> with <div>, preserving id
        $Content = $formOpenRegex.Replace($Content, "<div$idAttr>", 1)
        # Replace corresponding </form> with </div>
        $formCloseRegex = [regex]'</form>'
        $Content = $formCloseRegex.Replace($Content, '</div>', 1)
        Write-TransformLog -File $RelPath -Transform 'Form' -Detail "Replaced <form runat=""server""> with <div$idAttr> (preserves CSS block context)"
    }
    return $Content
}

#endregion

#region --- Master Page Transforms ---

function ConvertFrom-MasterPage {
    param([string]$Content, [string]$RelPath)

    # 1. Remove <asp:ScriptManager> block (can be multi-line with nested <Scripts>)
    $smRegex = [regex]'(?s)<asp:ScriptManager[^>]*>.*?</asp:ScriptManager>\s*\r?\n?'
    if ($Content -match '<asp:ScriptManager') {
        $Content = $smRegex.Replace($Content, '')
        Write-TransformLog -File $RelPath -Transform 'MasterPage' -Detail 'Removed <asp:ScriptManager> block'
    }
    # Also handle self-closing ScriptManager
    $smSelfRegex = [regex]'<asp:ScriptManager[^>]*/>\s*\r?\n?'
    $Content = $smSelfRegex.Replace($Content, '')

    # 2. Extract head metadata (<meta>, <link>, <title>) before stripping <head> section
    $headContentBlock = ''
    $headSectionRegex = [regex]'(?s)<head[^>]*>(.*?)</head>'
    $headMatch = $headSectionRegex.Match($Content)
    if ($headMatch.Success) {
        $headInner = $headMatch.Groups[1].Value
        $extractedTags = [System.Collections.Generic.List[string]]::new()

        foreach ($m in ([regex]'<meta\s[^>]*>').Matches($headInner)) {
            $extractedTags.Add("    " + $m.Value.Trim())
        }
        foreach ($m in ([regex]'(?s)<title>.*?</title>').Matches($headInner)) {
            $titleContent = $m.Value.Trim()
            # Skip empty <title></title> tags — <Page /> component handles title
            if ($titleContent -match '(?s)^<title>\s*</title>$') { continue }
            $extractedTags.Add("    " + $titleContent)
        }
        foreach ($m in ([regex]'<link\s[^>]*>').Matches($headInner)) {
            $tag = $m.Value.Trim()
            # Fix relative CSS paths to absolute — in Blazor, <HeadContent> renders into <head>
            # and the browser resolves relative paths from the page URL, not the component.
            # e.g. href="CSS/Master_CSS.css" on /Students resolves to /Students/CSS/Master_CSS.css (404)
            if ($tag -match 'href\s*=\s*"([^"]*)"') {
                $href = $Matches[1]
                if ($href -and -not $href.StartsWith('/') -and -not $href.StartsWith('http') -and -not $href.StartsWith('~')) {
                    $absHref = '/' + $href
                    $tag = $tag -replace [regex]::Escape("href=""$href"""), "href=""$absHref"""
                }
            }
            $extractedTags.Add("    " + $tag)
        }

        # Fix 1a: Extract <webopt:bundlereference> tags and flag for manual review
        $bundleRefRegex = [regex]'(?i)<webopt:bundlereference[^>]*>'
        foreach ($m in $bundleRefRegex.Matches($headInner)) {
            $bundlePath = ''
            if ($m.Value -match 'path\s*=\s*"([^"]*)"') {
                $bundlePath = $Matches[1]
            }
            if ($bundlePath) {
                Write-ManualItem -File $RelPath -Category 'CSSBundle' -Detail "CSS bundle reference '$bundlePath' needs manual conversion to <link> tags"
                $extractedTags.Add("    @* TODO: CSS bundle reference '$bundlePath' — convert to explicit <link> tags for each CSS file *@")
            } else {
                Write-ManualItem -File $RelPath -Category 'CSSBundle' -Detail 'webopt:bundlereference tag found without path — needs manual review'
                $extractedTags.Add("    @* TODO: webopt:bundlereference found — convert to explicit <link> tags *@")
            }
            Write-TransformLog -File $RelPath -Transform 'MasterPage' -Detail "Flagged <webopt:bundlereference> for manual CSS conversion"
        }

        # Fix 1a: Preserve CDN references (Bootstrap, jQuery) from <head>
        # Match both self-closing and full <script src="...cdn..."></script> tags
        $cdnScriptFullRegex = [regex]'(?s)<script\s[^>]*(?:cdn\.|cloudflare|bootstrapcdn|googleapis|jsdelivr|unpkg|cdnjs)[^>]*>.*?</script>'
        foreach ($m in $cdnScriptFullRegex.Matches($headInner)) {
            $tag = $m.Value.Trim()
            $extractedTags.Add("    " + $tag)
            Write-TransformLog -File $RelPath -Transform 'MasterPage' -Detail "Preserved CDN script: $($tag.Substring(0, [Math]::Min(80, $tag.Length)))"
        }
        $cdnLinkRegex = [regex]'<link\s[^>]*(?:cdn\.|cloudflare|bootstrapcdn|googleapis|jsdelivr|unpkg|cdnjs)[^>]*>'
        foreach ($m in $cdnLinkRegex.Matches($headInner)) {
            $tag = $m.Value.Trim()
            # Skip if already captured as a <link> tag
            if ($extractedTags -contains ("    " + $tag)) { continue }
            $extractedTags.Add("    " + $tag)
            Write-TransformLog -File $RelPath -Transform 'MasterPage' -Detail "Preserved CDN link: $($tag.Substring(0, [Math]::Min(80, $tag.Length)))"
        }

        if ($extractedTags.Count -gt 0) {
            $headContentBlock = "<HeadContent>`n" + ($extractedTags -join "`n") + "`n</HeadContent>"
            Write-TransformLog -File $RelPath -Transform 'MasterPage' -Detail "Extracted $($extractedTags.Count) head element(s) into <HeadContent>"
        }

        # Remove the entire <head>...</head> section
        $Content = $headSectionRegex.Replace($Content, '')
        Write-TransformLog -File $RelPath -Transform 'MasterPage' -Detail 'Removed <head> section'
    }

    # 3. Strip document wrapper tags
    $Content = $Content -replace '<!DOCTYPE[^>]*>\s*\r?\n?', ''
    $Content = $Content -replace '<html[^>]*>\s*\r?\n?', ''
    $Content = $Content -replace '</html>\s*\r?\n?', ''
    $Content = $Content -replace '<body[^>]*>\s*\r?\n?', ''
    $Content = $Content -replace '</body>\s*\r?\n?', ''
    Write-TransformLog -File $RelPath -Transform 'MasterPage' -Detail 'Stripped document wrapper (DOCTYPE, html, body)'

    # 4. Replace <asp:ContentPlaceHolder ID="MainContent|ContentPlaceHolder1|BodyContent"> → @Body
    $mainCphRegex = [regex]'(?si)<asp:ContentPlaceHolder\s+[^>]*ID\s*=\s*"(MainContent|ContentPlaceHolder1|BodyContent)"[^>]*>.*?</asp:ContentPlaceHolder>'
    if ($mainCphRegex.IsMatch($Content)) {
        $matchedId = $mainCphRegex.Match($Content).Groups[1].Value
        $Content = $mainCphRegex.Replace($Content, '@Body')
        Write-TransformLog -File $RelPath -Transform 'MasterPage' -Detail "ContentPlaceHolder $matchedId → @Body"
    }
    # Self-closing MainContent/ContentPlaceHolder1/BodyContent
    $mainCphSelfRegex = [regex]'(?i)<asp:ContentPlaceHolder\s+[^>]*ID\s*=\s*"(MainContent|ContentPlaceHolder1|BodyContent)"[^>]*/>'
    if ($mainCphSelfRegex.IsMatch($Content)) {
        $matchedId = $mainCphSelfRegex.Match($Content).Groups[1].Value
        $Content = $mainCphSelfRegex.Replace($Content, '@Body')
        Write-TransformLog -File $RelPath -Transform 'MasterPage' -Detail "ContentPlaceHolder $matchedId → @Body (self-closing)"
    }

    # Other ContentPlaceHolders → TODO comment
    $otherCphRegex = [regex]'(?si)<asp:ContentPlaceHolder\s+[^>]*ID\s*=\s*"([^"]+)"[^>]*>.*?</asp:ContentPlaceHolder>'
    foreach ($m in $otherCphRegex.Matches($Content)) {
        Write-ManualItem -File $RelPath -Category 'ContentPlaceHolder' -Detail "BWFC provides <ContentPlaceHolder> component — use <ContentPlaceHolder ID=""$($m.Groups[1].Value)"" /> or convert to @Body"
    }
    $Content = $otherCphRegex.Replace($Content, { param($m) "@* TODO: ContentPlaceHolder '$($m.Groups[1].Value)' — BWFC provides <ContentPlaceHolder> component, use <ContentPlaceHolder ID=""$($m.Groups[1].Value)"" /> or convert to @Body *@" })
    # Self-closing other ContentPlaceHolders
    $otherCphSelfRegex = [regex]'(?i)<asp:ContentPlaceHolder\s+[^>]*ID\s*=\s*"([^"]+)"[^>]*/>'
    foreach ($m in $otherCphSelfRegex.Matches($Content)) {
        Write-ManualItem -File $RelPath -Category 'ContentPlaceHolder' -Detail "BWFC provides <ContentPlaceHolder> component — use <ContentPlaceHolder ID=""$($m.Groups[1].Value)"" /> or convert to @Body (self-closing)"
    }
    $Content = $otherCphSelfRegex.Replace($Content, { param($m) "@* TODO: ContentPlaceHolder '$($m.Groups[1].Value)' — BWFC provides <ContentPlaceHolder> component, use <ContentPlaceHolder ID=""$($m.Groups[1].Value)"" /> or convert to @Body *@" })

    # 5. Flag items needing Layer 2 attention
    if ($Content -match '<RoleGroups>') {
        Write-ManualItem -File $RelPath -Category 'LoginView-RoleGroups' -Detail 'LoginView <RoleGroups> requires manual conversion to @attribute [Authorize(Roles="...")]'
    }
    if ($Content -match 'SelectMethod\s*=') {
        Write-ManualItem -File $RelPath -Category 'SelectMethod' -Detail 'SelectMethod preserved — needs delegate conversion in L2 (BWFC DataBoundComponent supports SelectMethod natively)'
    }

    # 6. Inject @inherits LayoutComponentBase, <Page /> component, and HeadContent at the top
    $header = "@inherits LayoutComponentBase`n`n<BlazorWebFormsComponents.Page />`n"
    if ($headContentBlock) {
        $header += "`n" + $headContentBlock + "`n"
    }
    $Content = $header + "`n" + $Content

    return $Content
}

#endregion

#region --- Expression Transforms ---

function ConvertFrom-Expressions {
    param([string]$Content, [string]$RelPath)

    # Comments: <%-- ... --%> → @* ... *@
    $commentRegex = [regex]'(?s)<%--(.+?)--%>'
    $commentMatches = $commentRegex.Matches($Content)
    if ($commentMatches.Count -gt 0) {
        $Content = $commentRegex.Replace($Content, '@*$1*@')
        Write-TransformLog -File $RelPath -Transform 'Expression' -Detail "Converted $($commentMatches.Count) comment(s) to Razor syntax"
    }

    # --- GAP-13: Bind() → @bind two-way binding ---
    # Bind() is used in EditItemTemplate/InsertItemTemplate for two-way data binding.
    # When used inside an attribute value (e.g., Text='<%# Bind("Name") %>'), convert to @bind-Value.
    # When standalone, convert to @context.PropertyName (same as Eval).

    # Bind() inside attribute values: attr='<%# Bind("Prop") %>' → @bind-Value="context.Prop"
    # Note: We handle the most common case where Bind() is the attribute value for Text, SelectedValue, etc.
    $bindAttrRegex = [regex]'(\w+)\s*=\s*''<%#\s*Bind\("(\w+)"\)\s*%>'''
    $bindAttrMatches = $bindAttrRegex.Matches($Content)
    if ($bindAttrMatches.Count -gt 0) {
        $Content = $bindAttrRegex.Replace($Content, '@bind-Value="context.$2"')
        Write-TransformLog -File $RelPath -Transform 'BindExpression' -Detail "Converted $($bindAttrMatches.Count) Bind() attribute(s) to @bind-Value"
    }

    # Bind() inside attribute values with double quotes: attr="<%# Bind("Prop") %>"
    $bindAttrDblRegex = [regex]'(\w+)\s*=\s*"<%#\s*Bind\("(\w+)"\)\s*%>"'
    $bindAttrDblMatches = $bindAttrDblRegex.Matches($Content)
    if ($bindAttrDblMatches.Count -gt 0) {
        $Content = $bindAttrDblRegex.Replace($Content, '@bind-Value="context.$2"')
        Write-TransformLog -File $RelPath -Transform 'BindExpression' -Detail "Converted $($bindAttrDblMatches.Count) Bind() double-quoted attribute(s) to @bind-Value"
    }

    # Bind() with HTML-encoded delimiter: <%#: Bind("Prop") %> → @context.Prop (read-only context)
    $bindEncodedRegex = [regex]'<%#:\s*Bind\("(\w+)"\)\s*%>'
    $bindEncodedMatches = $bindEncodedRegex.Matches($Content)
    if ($bindEncodedMatches.Count -gt 0) {
        $Content = $bindEncodedRegex.Replace($Content, '@context.$1')
        Write-TransformLog -File $RelPath -Transform 'BindExpression' -Detail "Converted $($bindEncodedMatches.Count) encoded Bind() to @context (read-only fallback)"
    }

    # Standalone Bind(): <%# Bind("Prop") %> → @context.Prop
    $bindStandaloneRegex = [regex]'<%#\s*Bind\("(\w+)"\)\s*%>'
    $bindStandaloneMatches = $bindStandaloneRegex.Matches($Content)
    if ($bindStandaloneMatches.Count -gt 0) {
        $Content = $bindStandaloneRegex.Replace($Content, '@context.$1')
        Write-TransformLog -File $RelPath -Transform 'BindExpression' -Detail "Converted $($bindStandaloneMatches.Count) standalone Bind() to @context"
    }

    # Data binding with Eval and format string: <%#: Eval("prop", "{0:fmt}") %> → @context.prop.ToString("fmt")
    $evalFmtRegex = [regex]'<%#:\s*Eval\("(\w+)",\s*"\{0:([^}]+)\}"\)\s*%>'
    $evalFmtMatches = $evalFmtRegex.Matches($Content)
    if ($evalFmtMatches.Count -gt 0) {
        $Content = $evalFmtRegex.Replace($Content, '@context.$1.ToString("$2")')
        Write-TransformLog -File $RelPath -Transform 'Expression' -Detail "Converted $($evalFmtMatches.Count) Eval() with format string to @context.ToString()"
    }

    # String.Format with Item.Property: <%#: String.Format("{0:fmt}", Item.Prop) %> → @($"{context.Prop:fmt}")
    $strFmtRegex = [regex]'<%#:\s*String\.Format\("\{0:([^}]+)\}",\s*Item\.(\w+)\)\s*%>'
    $strFmtMatches = $strFmtRegex.Matches($Content)
    if ($strFmtMatches.Count -gt 0) {
        $Content = $strFmtRegex.Replace($Content, '@($$"{context.$2:$1}")')
        Write-TransformLog -File $RelPath -Transform 'Expression' -Detail "Converted $($strFmtMatches.Count) String.Format(Item.) to interpolated string"
    }

    # Data binding with Eval: <%#:\s*Eval("prop")\s*%> → @context.prop
    $evalRegex = [regex]'<%#:\s*Eval\("(\w+)"\)\s*%>'
    $evalMatches = $evalRegex.Matches($Content)
    if ($evalMatches.Count -gt 0) {
        $Content = $evalRegex.Replace($Content, '@context.$1')
        Write-TransformLog -File $RelPath -Transform 'Expression' -Detail "Converted $($evalMatches.Count) Eval() binding(s) to @context"
    }

    # Data binding with Item: <%#:\s*Item.Prop\s*%> → @context.Prop
    $itemRegex = [regex]'<%#:\s*Item\.(\w+)\s*%>'
    $itemMatches = $itemRegex.Matches($Content)
    if ($itemMatches.Count -gt 0) {
        $Content = $itemRegex.Replace($Content, '@context.$1')
        Write-TransformLog -File $RelPath -Transform 'Expression' -Detail "Converted $($itemMatches.Count) Item binding(s) to @context"
    }

    # Bare Item binding: <%#: Item %> → @context
    $bareItemRegex = [regex]'<%#:\s*Item\s*%>'
    $bareItemMatches = $bareItemRegex.Matches($Content)
    if ($bareItemMatches.Count -gt 0) {
        $Content = $bareItemRegex.Replace($Content, '@context')
        Write-TransformLog -File $RelPath -Transform 'Expression' -Detail "Converted $($bareItemMatches.Count) bare Item binding(s) to @context"
    }

    # Encoded expressions: <%: expr %> → @(expr)
    $encodedRegex = [regex]'<%:\s*(.+?)\s*%>'
    $encodedMatches = $encodedRegex.Matches($Content)
    if ($encodedMatches.Count -gt 0) {
        $Content = $encodedRegex.Replace($Content, '@($1)')
        Write-TransformLog -File $RelPath -Transform 'Expression' -Detail "Converted $($encodedMatches.Count) encoded expression(s)"
    }

    # Unencoded expressions: <%= expr %> → @(expr)
    $unencodedRegex = [regex]'<%=\s*(.+?)\s*%>'
    $unencodedMatches = $unencodedRegex.Matches($Content)
    if ($unencodedMatches.Count -gt 0) {
        $Content = $unencodedRegex.Replace($Content, '@($1)')
        Write-TransformLog -File $RelPath -Transform 'Expression' -Detail "Converted $($unencodedMatches.Count) unencoded expression(s)"
    }

    # Flag any remaining <% ... %> blocks as manual
    $remainingRegex = [regex]'<%[^@].*?%>'
    $remainingMatches = $remainingRegex.Matches($Content)
    foreach ($m in $remainingMatches) {
        Write-ManualItem -File $RelPath -Category 'CodeBlock' -Detail "Unconverted code block: $($m.Value.Substring(0, [Math]::Min(80, $m.Value.Length)))"
    }

    return $Content
}

#endregion

#region --- LoginView Conversion ---

function ConvertFrom-LoginView {
    param([string]$Content, [string]$RelPath)

    # Flag <RoleGroups> as manual — use BWFC RoleGroup component
    if ($Content -match '<RoleGroups>') {
        Write-ManualItem -File $RelPath -Category 'LoginView-RoleGroups' -Detail 'LoginView <RoleGroups> should use the BWFC RoleGroup child component inside <LoginView>'
    }

    # <asp:LoginView ...> → <LoginView> (strip asp: prefix and all attributes)
    $openRegex = [regex]'(?i)<asp:LoginView\b[^>]*>'
    $openMatches = $openRegex.Matches($Content)
    if ($openMatches.Count -gt 0) {
        $Content = $openRegex.Replace($Content, '<LoginView>')
        Write-TransformLog -File $RelPath -Transform 'LoginView' -Detail "Converted $($openMatches.Count) <asp:LoginView> to <LoginView>"
    }

    # </asp:LoginView> → </LoginView>
    $closeRegex = [regex]'(?i)</asp:LoginView\s*>'
    $closeMatches = $closeRegex.Matches($Content)
    if ($closeMatches.Count -gt 0) {
        $Content = $closeRegex.Replace($Content, '</LoginView>')
        Write-TransformLog -File $RelPath -Transform 'LoginView' -Detail "Converted $($closeMatches.Count) </asp:LoginView> to </LoginView>"
    }

    # AnonymousTemplate and LoggedInTemplate are already the correct
    # BWFC LoginView parameter names — no conversion needed.

    return $Content
}

#endregion

#region --- GetRouteUrl Conversion ---

function ConvertFrom-GetRouteUrl {
    param([string]$Content, [string]$RelPath)

    $transformed = $false

    # Page.GetRouteUrl( → GetRouteUrlHelper.GetRouteUrl(
    $pageRouteRegex = [regex]'Page\.GetRouteUrl\s*\('
    $pageRouteMatches = $pageRouteRegex.Matches($Content)
    if ($pageRouteMatches.Count -gt 0) {
        $Content = $pageRouteRegex.Replace($Content, 'GetRouteUrlHelper.GetRouteUrl(')
        Write-TransformLog -File $RelPath -Transform 'GetRouteUrl' -Detail "Converted $($pageRouteMatches.Count) Page.GetRouteUrl() to GetRouteUrlHelper.GetRouteUrl()"
        $transformed = $true
    }

    # Standalone GetRouteUrl( (not already prefixed with Helper.) → GetRouteUrlHelper.GetRouteUrl(
    $standaloneRegex = [regex]'(?<![\w.])GetRouteUrl\s*\('
    $standaloneMatches = $standaloneRegex.Matches($Content)
    if ($standaloneMatches.Count -gt 0) {
        $Content = $standaloneRegex.Replace($Content, 'GetRouteUrlHelper.GetRouteUrl(')
        Write-TransformLog -File $RelPath -Transform 'GetRouteUrl' -Detail "Converted $($standaloneMatches.Count) GetRouteUrl() to GetRouteUrlHelper.GetRouteUrl()"
        $transformed = $true
    }

    # Inside GetRouteUrl calls only, convert Eval("Prop") to context.Prop.
    # Scoped to lines containing GetRouteUrl to avoid corrupting <%#: Eval("Name") %> expressions.
    if ($transformed) {
        $evalInRouteRegex = [regex]'Eval\("(\w+)"\)'
        $lines = $Content -split "`n"
        $convertedCount = 0
        for ($i = 0; $i -lt $lines.Count; $i++) {
            if ($lines[$i] -match 'GetRouteUrl') {
                $matches_ = $evalInRouteRegex.Matches($lines[$i])
                if ($matches_.Count -gt 0) {
                    $lines[$i] = $evalInRouteRegex.Replace($lines[$i], 'context.$1')
                    $convertedCount += $matches_.Count
                }
            }
        }
        if ($convertedCount -gt 0) {
            $Content = $lines -join "`n"
            Write-TransformLog -File $RelPath -Transform 'GetRouteUrl' -Detail "Converted $convertedCount Eval() in route values to context.Property"
        }
    }

    # Flag RouteValueDictionary usage as manual
    if ($Content -match 'RouteValueDictionary') {
        Write-ManualItem -File $RelPath -Category 'GetRouteUrl' -Detail 'RouteValueDictionary usage detected — works but consider simplifying to anonymous object'
    }

    # Note the required @inject directive
    if ($transformed) {
        Write-ManualItem -File $RelPath -Category 'GetRouteUrl' -Detail 'BWFC provides GetRouteUrlHelper — add @inject GetRouteUrlHelper GetRouteUrlHelper and use GetRouteUrlHelper.GetRouteUrl()'
    }

    # RF-11: Flag GetRouteUrl patterns with concrete replacement hints
    $routeNameRegex = [regex]'GetRouteUrlHelper\.GetRouteUrl\s*\(\s*"([^"]+)"'
    $routeNameMatches = $routeNameRegex.Matches($Content)
    foreach ($m in $routeNameMatches) {
        $routeName = $m.Groups[1].Value
        Write-ManualItem -File $RelPath -Category 'GetRouteUrl' -Detail "BWFC GetRouteUrlHelper supports route name '$routeName' — verify route is registered, or replace with direct URL pattern, e.g., /ProductDetails?ProductID=@Item.ProductID"
    }

    return $Content
}

#endregion

#region --- SelectMethod Conversion ---

function ConvertFrom-SelectMethod {
    param([string]$Content, [string]$RelPath)

    # Preserve SelectMethod="MethodName" in markup (BWFC DataBoundComponent supports it natively)
    # Add a TODO comment after the tag noting the string needs delegate conversion in L2
    $selectMethodRegex = [regex]'(?si)(<[^>]*?\s+SelectMethod\s*=\s*"([^"]+)"[^>]*>)'
    $selectMethodMatches = $selectMethodRegex.Matches($Content)
    if ($selectMethodMatches.Count -gt 0) {
        $Content = $selectMethodRegex.Replace($Content, {
            param($m)
            $fullTag = $m.Groups[1].Value
            $methodName = $m.Groups[2].Value
            "${fullTag}`n@* TODO: SelectMethod=""${methodName}"" preserved — convert to delegate: SelectMethod=""@((maxRows, startRow, sort, out total) => YourService.${methodName}(maxRows, startRow, sort, out total))"" *@"
        })
        Write-TransformLog -File $RelPath -Transform 'SelectMethod' -Detail "Preserved $($selectMethodMatches.Count) SelectMethod attribute(s) with TODO for delegate conversion"
        foreach ($m in $selectMethodMatches) {
            Write-ManualItem -File $RelPath -Category 'SelectMethod' -Detail "SelectMethod='$($m.Groups[2].Value)' preserved — needs delegate conversion in L2"
        }
    }

    return $Content
}

#endregion

#region --- Tag & Attribute Transforms ---

function ConvertFrom-AspPrefix {
    param([string]$Content, [string]$RelPath)

    # Opening tags: <asp:Button → <Button
    $openRegex = [regex]'<asp:(\w+)'
    $openMatches = $openRegex.Matches($Content)
    if ($openMatches.Count -gt 0) {
        $Content = $openRegex.Replace($Content, '<$1')
        Write-TransformLog -File $RelPath -Transform 'TagPrefix' -Detail "Removed asp: prefix from $($openMatches.Count) opening tag(s)"
    }

    # Closing tags: </asp:Button> → </Button>
    $closeRegex = [regex]'</asp:(\w+)>'
    $closeMatches = $closeRegex.Matches($Content)
    if ($closeMatches.Count -gt 0) {
        $Content = $closeRegex.Replace($Content, '</$1>')
        Write-TransformLog -File $RelPath -Transform 'TagPrefix' -Detail "Removed asp: prefix from $($closeMatches.Count) closing tag(s)"
    }

    # FIX 1: Strip ContentTemplate wrappers (content preserved)
    # UpdatePanel's ContentTemplate is a Web Forms concept — in Blazor, child content goes directly inside the component
    $contentTemplateOpenCount = ([regex]'<ContentTemplate>').Matches($Content).Count
    if ($contentTemplateOpenCount -gt 0) {
        $Content = $Content -replace '<ContentTemplate>', ''
        $Content = $Content -replace '</ContentTemplate>', ''
        Write-TransformLog -File $RelPath -Transform 'TagPrefix' -Detail "Stripped $contentTemplateOpenCount ContentTemplate wrapper(s)"
    }

    # Opening tags: <uc1:Control → <Control (handles uc, uc1, uc2, etc.)
    $ucOpenRegex = [regex]'<uc\d*:(\w+)'
    $ucOpenMatches = $ucOpenRegex.Matches($Content)
    if ($ucOpenMatches.Count -gt 0) {
        $Content = $ucOpenRegex.Replace($Content, '<$1')
    }

    # Closing tags: </uc1:Control> → </Control>
    $ucCloseRegex = [regex]'</uc\d*:(\w+)>'
    $ucCloseMatches = $ucCloseRegex.Matches($Content)
    if ($ucCloseMatches.Count -gt 0) {
        $Content = $ucCloseRegex.Replace($Content, '</$1>')
    }

    $ucTotal = $ucOpenMatches.Count + $ucCloseMatches.Count
    if ($ucTotal -gt 0) {
        Write-TransformLog -File $RelPath -Transform 'TagPrefix' -Detail "Removed uc: prefix from $ucTotal opening/closing tag(s)"
    }

    return $Content
}

function ConvertFrom-AjaxToolkitPrefix {
    param([string]$Content, [string]$RelPath)

    # Known Ajax Control Toolkit components with BWFC equivalents
    $knownControls = @(
        'Accordion', 'AccordionPane',
        'TabContainer', 'TabPanel',
        'ConfirmButtonExtender', 'FilteredTextBoxExtender',
        'ModalPopupExtender', 'CollapsiblePanelExtender',
        'CalendarExtender', 'AutoCompleteExtender',
        'MaskedEditExtender', 'NumericUpDownExtender',
        'SliderExtender', 'ToggleButtonExtender',
        'PopupControlExtender', 'HoverMenuExtender'
    )

    # 1. Strip ToolkitScriptManager entirely (not needed in Blazor)
    # Handle block form: <ajaxToolkit:ToolkitScriptManager ...>...</ajaxToolkit:ToolkitScriptManager>
    $tsmBlockRegex = [regex]'(?s)<ajaxToolkit:ToolkitScriptManager(?:[^>]|(?:%>))*?(?:>.*?</ajaxToolkit:ToolkitScriptManager>)\s*\r?\n?'
    if ($Content -match '<ajaxToolkit:ToolkitScriptManager') {
        $tsmBlockBefore = $Content
        $Content = $tsmBlockRegex.Replace($Content, '')
        # Handle self-closing form: <ajaxToolkit:ToolkitScriptManager ... />
        $tsmSelfRegex = [regex]'<ajaxToolkit:ToolkitScriptManager(?:[^>]|(?:%>))*?/>\s*\r?\n?'
        $Content = $tsmSelfRegex.Replace($Content, '')
        if ($Content -ne $tsmBlockBefore) {
            Write-TransformLog -File $RelPath -Transform 'AjaxToolkit' -Detail 'Removed <ajaxToolkit:ToolkitScriptManager> (not needed in Blazor)'
        }
    }

    # 2. Convert known ajaxToolkit: controls → Blazor equivalents (strip prefix)
    $knownPattern = ($knownControls | ForEach-Object { [regex]::Escape($_) }) -join '|'

    # Opening tags: <ajaxToolkit:Accordion → <Accordion
    $openRegex = [regex]"<ajaxToolkit:($knownPattern)"
    $openMatches = $openRegex.Matches($Content)
    if ($openMatches.Count -gt 0) {
        $Content = $openRegex.Replace($Content, '<$1')
        Write-TransformLog -File $RelPath -Transform 'AjaxToolkit' -Detail "Converted ajaxToolkit: prefix on $($openMatches.Count) opening tag(s)"
        $script:HasAjaxToolkitControls = $true
    }

    # Closing tags: </ajaxToolkit:Accordion> → </Accordion>
    $closeRegex = [regex]"</ajaxToolkit:($knownPattern)>"
    $closeMatches = $closeRegex.Matches($Content)
    if ($closeMatches.Count -gt 0) {
        $Content = $closeRegex.Replace($Content, '</$1>')
        Write-TransformLog -File $RelPath -Transform 'AjaxToolkit' -Detail "Converted ajaxToolkit: prefix on $($closeMatches.Count) closing tag(s)"
        $script:HasAjaxToolkitControls = $true
    }

    # 3. Graceful fallback: any remaining unrecognized ajaxToolkit: controls → TODO comment
    # Self-closing tags: <ajaxToolkit:UnknownControl ... />
    $unknownSelfRegex = [regex]'(?s)<ajaxToolkit:(\w+)(?:[^>]|(?:%>))*?/>'
    $unknownSelfMatches = $unknownSelfRegex.Matches($Content)
    if ($unknownSelfMatches.Count -gt 0) {
        foreach ($m in $unknownSelfMatches) {
            $controlName = $m.Groups[1].Value
            Write-ManualItem -File $RelPath -Category 'AjaxToolkit-Unknown' -Detail "Unrecognized ajaxToolkit control: $controlName — no BWFC equivalent available"
        }
        $Content = $unknownSelfRegex.Replace($Content, '@* TODO: Convert ajaxToolkit:$1 — no BWFC equivalent yet *@')
        Write-TransformLog -File $RelPath -Transform 'AjaxToolkit' -Detail "Replaced $($unknownSelfMatches.Count) unrecognized self-closing ajaxToolkit control(s) with TODO"
    }

    # Block tags: <ajaxToolkit:UnknownControl ...>...</ajaxToolkit:UnknownControl>
    $unknownBlockRegex = [regex]'(?s)<ajaxToolkit:(\w+)(?:[^>]|(?:%>))*?>.*?</ajaxToolkit:\1>'
    $unknownBlockMatches = $unknownBlockRegex.Matches($Content)
    if ($unknownBlockMatches.Count -gt 0) {
        foreach ($m in $unknownBlockMatches) {
            $controlName = $m.Groups[1].Value
            Write-ManualItem -File $RelPath -Category 'AjaxToolkit-Unknown' -Detail "Unrecognized ajaxToolkit control: $controlName — no BWFC equivalent available"
        }
        $Content = $unknownBlockRegex.Replace($Content, '@* TODO: Convert ajaxToolkit:$1 — no BWFC equivalent yet *@')
        Write-TransformLog -File $RelPath -Transform 'AjaxToolkit' -Detail "Replaced $($unknownBlockMatches.Count) unrecognized block ajaxToolkit control(s) with TODO"
    }

    return $Content
}

function Remove-WebFormsAttributes {
    param([string]$Content, [string]$RelPath)

    foreach ($pattern in $StripAttributes) {
        $attrRegex = [regex]"\s*$pattern"
        $attrMatches = $attrRegex.Matches($Content)
        if ($attrMatches.Count -gt 0) {
            $Content = $attrRegex.Replace($Content, '')
            # Extract a friendly name from the pattern for logging
            $friendlyName = $pattern -replace '\\s\*=\\s\*.*', '' -replace '\\s\*', ' ' -replace '\\', ''
            Write-TransformLog -File $RelPath -Transform 'Attribute' -Detail "Removed $($attrMatches.Count) '$friendlyName' attribute(s)"
        }
    }

    # ItemType="Namespace.Class" → TItem="Class"
    $itemTypeRegex = [regex]'ItemType="(?:[^"]*\.)?([^"]+)"'
    $itemTypeMatches = $itemTypeRegex.Matches($Content)
    if ($itemTypeMatches.Count -gt 0) {
        $Content = $itemTypeRegex.Replace($Content, 'TItem="$1"')
        Write-TransformLog -File $RelPath -Transform 'Attribute' -Detail "Converted $($itemTypeMatches.Count) ItemType to TItem"
    }

    # Add ItemType="object" fallback to generic BWFC components that lack an explicit ItemType
    $genericComponents = @('GridView', 'DetailsView', 'DropDownList', 'BoundField', 'BulletedList',
        'Repeater', 'ListView', 'FormView', 'RadioButtonList', 'CheckBoxList', 'ListBox',
        'HyperLinkField', 'ButtonField', 'TemplateField', 'DataList', 'DataGrid')
    $addedCount = 0
    foreach ($comp in $genericComponents) {
        # Match opening tags for this component that do NOT already have ItemType or TItem
        $tagRegex = [regex]"(<${comp}\s)(?![^>]*(?:ItemType|TItem)=)([^/>]*)(>|/>)"
        $tagMatches = $tagRegex.Matches($Content)
        if ($tagMatches.Count -gt 0) {
            $Content = $tagRegex.Replace($Content, '${1}ItemType="object" ${2}${3}')
            $addedCount += $tagMatches.Count
        }
    }
    if ($addedCount -gt 0) {
        Write-TransformLog -File $RelPath -Transform 'Attribute' -Detail "Added ItemType=`"object`" fallback to $addedCount generic component tag(s)"
    }

    # FIX 4: Convert Web Forms ID attribute to lowercase id for HTML compatibility
    # BWFC components accept both ID and id, so broad replacement is safe
    $idRegex = [regex]'\bID="([^"]*)"'
    $idMatches = $idRegex.Matches($Content)
    if ($idMatches.Count -gt 0) {
        $Content = $idRegex.Replace($Content, 'id="$1"')
        Write-TransformLog -File $RelPath -Transform 'Attribute' -Detail "Converted $($idMatches.Count) ID= to id="
    }

    return $Content
}

function ConvertFrom-UrlReferences {
    param([string]$Content, [string]$RelPath)

    $urlPatterns = @(
        @{ Pattern = 'href="~/';       Replacement = 'href="/';       Name = 'href' }
        @{ Pattern = 'NavigateUrl="~/'; Replacement = 'NavigateUrl="/'; Name = 'NavigateUrl' }
        @{ Pattern = 'ImageUrl="~/';    Replacement = 'ImageUrl="/';    Name = 'ImageUrl' }
    )

    foreach ($up in $urlPatterns) {
        $count = ([regex]::Matches($Content, [regex]::Escape($up.Pattern))).Count
        if ($count -gt 0) {
            $Content = $Content.Replace($up.Pattern, $up.Replacement)
            Write-TransformLog -File $RelPath -Transform 'URL' -Detail "Converted $count $($up.Name) ~/ reference(s) to /"
        }
    }

    return $Content
}

#endregion

#region --- Attribute Value Normalization ---

function Normalize-AttributeValues {
    <#
    .SYNOPSIS
        Normalizes boolean, enum, and unit attribute values in converted Blazor markup.
    .DESCRIPTION
        Mechanical transforms for BWFC component attribute values:
        1. Booleans: Visible="True" → Visible="true" (Razor uses lowercase C# booleans)
        2. Enums: GridLines="Both" → GridLines="@GridLines.Both" (type-qualified for Razor)
        3. Units: Width="100px" → Width="100" (BWFC Unit.cs treats bare integers as pixels)
    #>
    param([string]$Content, [string]$RelPath)

    # --- Boolean normalization ---
    # Web Forms uses PascalCase True/False; C#/Razor uses lowercase true/false.
    # Exclude known text-content attributes to avoid false positives.
    $boolRegex = [regex]'(\w+)="(True|False)"'
    $boolMatches = $boolRegex.Matches($Content)
    if ($boolMatches.Count -gt 0) {
        $textAttrs = @('Text', 'Title', 'Value', 'ToolTip', 'HeaderText', 'FooterText',
                        'CommandName', 'CommandArgument', 'ErrorMessage', 'InitialValue',
                        'DataField', 'DataFormatString', 'SortExpression', 'NavigateUrl',
                        'DataTextField', 'DataValueField', 'ValidationExpression')
        $Content = $boolRegex.Replace($Content, {
            param($m)
            $attr = $m.Groups[1].Value
            if ($attr -in $textAttrs) { return $m.Value }
            return "$attr=`"$($m.Groups[2].Value.ToLower())`""
        })
        Write-TransformLog -File $RelPath -Transform 'BoolNormalize' -Detail "Normalized up to $($boolMatches.Count) True/False attribute value(s) to lowercase"
    }

    # --- Enum type-qualifying ---
    # Map of BWFC component attribute names → their enum type names.
    # GridLines="Both" becomes GridLines="@GridLines.Both" so Razor evaluates the C# enum
    # instead of relying on EnumParameter<T> string parsing at runtime.
    $enumAttrMap = @{
        'GridLines'         = 'GridLines'
        'BorderStyle'       = 'BorderStyle'
        'HorizontalAlign'   = 'HorizontalAlign'
        'VerticalAlign'     = 'VerticalAlign'
        'TextAlign'         = 'TextAlign'
        'TextMode'          = 'TextBoxMode'
        'ImageAlign'        = 'ImageAlign'
        'Orientation'       = 'Orientation'
        'BulletStyle'       = 'BulletStyle'
        'CaptionAlign'      = 'TableCaptionAlign'
        'SortDirection'     = 'SortDirection'
        'ScrollBars'        = 'ScrollBars'
        'ContentDirection'  = 'ContentDirection'
        'DayNameFormat'     = 'DayNameFormat'
        'TitleFormat'       = 'TitleFormat'
        'InsertItemPosition'= 'InsertItemPosition'
        'UpdateMode'        = 'UpdatePanelUpdateMode'
        'FontSize'          = 'FontSize'
    }
    $enumNormCount = 0
    foreach ($attrName in $enumAttrMap.Keys) {
        $enumType = $enumAttrMap[$attrName]
        # Match AttrName="PascalCaseValue" — value starts with uppercase, not already @-prefixed
        $enumRegex = [regex]"(?<!\w)${attrName}=`"([A-Z][a-zA-Z0-9]*)`""
        $enumMatches = $enumRegex.Matches($Content)
        if ($enumMatches.Count -gt 0) {
            $Content = $enumRegex.Replace($Content, {
                param($m)
                $val = $m.Groups[1].Value
                # Skip boolean values (already lowercased by prior transform, but guard edge cases)
                if ($val -match '^(True|False|true|false)$') { return $m.Value }
                return "${attrName}=`"@${enumType}.${val}`""
            })
            $enumNormCount += $enumMatches.Count
        }
    }
    if ($enumNormCount -gt 0) {
        Write-TransformLog -File $RelPath -Transform 'EnumNormalize' -Detail "Type-qualified $enumNormCount enum attribute value(s) (e.g., GridLines=`"@GridLines.Both`")"
    }

    # --- Unit normalization (strip "px" suffix) ---
    # BWFC Unit.cs treats bare integers as pixels. Stripping "px" keeps markup clean.
    # Only strip "px" — other units (%, em, pt) carry distinct meaning and are preserved.
    $unitAttrs = @('Width', 'Height', 'BorderWidth', 'CellPadding', 'CellSpacing')
    $unitNormCount = 0
    foreach ($attr in $unitAttrs) {
        $unitRegex = [regex]"${attr}=`"(\d+)px`""
        $unitMatches = $unitRegex.Matches($Content)
        if ($unitMatches.Count -gt 0) {
            $Content = $unitRegex.Replace($Content, "${attr}=`"`$1`"")
            $unitNormCount += $unitMatches.Count
        }
    }
    if ($unitNormCount -gt 0) {
        Write-TransformLog -File $RelPath -Transform 'UnitNormalize' -Detail "Stripped 'px' suffix from $unitNormCount unit attribute(s)"
    }

    return $Content
}

function Add-DataSourceIDWarning {
    <#
    .SYNOPSIS
        Detects DataSourceID attributes and data source controls, adds TODO warnings.
    .DESCRIPTION
        BWFC uses SelectMethod/Items binding instead of ASP.NET data source controls.
        DataSourceID="SqlDataSource1" has no Blazor equivalent — the attribute is removed
        and data source control declarations are replaced with TODO comments.
    #>
    param([string]$Content, [string]$RelPath)

    # Remove DataSourceID attributes and log migration warnings
    $dsAttrRegex = [regex]'\s*DataSourceID="([^"]+)"'
    $dsMatches = $dsAttrRegex.Matches($Content)
    if ($dsMatches.Count -gt 0) {
        foreach ($m in $dsMatches) {
            Write-ManualItem -File $RelPath -Category 'DataSourceID' -Detail "DataSourceID='$($m.Groups[1].Value)' removed — convert to SelectMethod or Items binding in code-behind"
        }
        $Content = $dsAttrRegex.Replace($Content, '')
        Write-TransformLog -File $RelPath -Transform 'DataSourceID' -Detail "Removed $($dsMatches.Count) DataSourceID attribute(s) — BWFC uses SelectMethod/Items instead"
    }

    # Replace data source control declarations with TODO comments.
    # asp: prefix is already stripped at this point, so match bare control names.
    # Use (?s) single-line mode because tags often span multiple lines and may contain
    # Web Forms expressions like <%$ ... %> whose %> breaks simple [^>]* patterns.
    $dsControls = @('SqlDataSource', 'ObjectDataSource', 'LinqDataSource',
                     'EntityDataSource', 'XmlDataSource', 'SiteMapDataSource', 'AccessDataSource')
    foreach ($ctrl in $dsControls) {
        # Self-closing: <SqlDataSource ... /> (may span lines, may contain %> in expressions)
        $selfCloseRegex = [regex]"(?s)<${ctrl}\b.*?/>"
        $selfCloseMatches = $selfCloseRegex.Matches($Content)
        if ($selfCloseMatches.Count -gt 0) {
            $Content = $selfCloseRegex.Replace($Content, "@* TODO: <${ctrl}> has no Blazor equivalent — wire data through code-behind service injection and SelectMethod/Items *@")
            Write-TransformLog -File $RelPath -Transform 'DataSourceControl' -Detail "Replaced $($selfCloseMatches.Count) <${ctrl}/> with TODO comment"
            Write-ManualItem -File $RelPath -Category 'DataSourceControl' -Detail "<${ctrl}> removed — wire data through service injection"
        }
        # Open+close: <SqlDataSource ...>...</SqlDataSource>
        $openCloseRegex = [regex]"(?s)<${ctrl}\b.*?</${ctrl}\s*>"
        $openCloseMatches = $openCloseRegex.Matches($Content)
        if ($openCloseMatches.Count -gt 0) {
            $Content = $openCloseRegex.Replace($Content, "@* TODO: <${ctrl}> has no Blazor equivalent — wire data through code-behind service injection and SelectMethod/Items *@")
            Write-TransformLog -File $RelPath -Transform 'DataSourceControl' -Detail "Replaced $($openCloseMatches.Count) <${ctrl}>...</${ctrl}> with TODO comment"
            Write-ManualItem -File $RelPath -Category 'DataSourceControl' -Detail "<${ctrl}> block removed — wire data through service injection"
        }
    }

    return $Content
}

#endregion

#region --- Code-Behind Handling ---

function Remove-IsPostBackGuards {
    <#
    .SYNOPSIS
        Unwraps IsPostBack guard patterns from code-behind content (GAP-06).
    .DESCRIPTION
        In Web Forms, Page_Load commonly wraps first-load logic in if (!IsPostBack) { ... }.
        In Blazor, OnInitializedAsync runs only once, so the guard is unnecessary.
        Simple guards (no else) are unwrapped. Complex guards (with else) get a TODO comment.
    #>
    param(
        [string]$Content,
        [string]$RelPath
    )

    $unwrapCount = 0
    $todoCount = 0

    # --- Simple IsPostBack guards (no else clause) ---
    # Matches: if (!IsPostBack) { body }, if (!Page.IsPostBack) { body }, if (!this.IsPostBack) { body }
    # Also: if (IsPostBack == false), if (Page.IsPostBack == false), etc.
    # Uses brace-counting to find the matching close brace.

    # Patterns that represent "if not postback"
    $guardPatterns = @(
        'if\s*\(\s*!(?:Page\.|this\.)?IsPostBack\s*\)',
        'if\s*\(\s*(?:Page\.|this\.)?IsPostBack\s*==\s*false\s*\)',
        'if\s*\(\s*false\s*==\s*(?:Page\.|this\.)?IsPostBack\s*\)'
    )
    $combinedPattern = '(?:' + ($guardPatterns -join '|') + ')'

    # Process the content iteratively — find each guard and unwrap or annotate
    $guardRegex = [regex]$combinedPattern
    $iterations = 0
    $maxIterations = 50  # safety limit

    while ($guardRegex.IsMatch($Content) -and $iterations -lt $maxIterations) {
        $iterations++
        $match = $guardRegex.Match($Content)
        $matchStart = $match.Index
        $afterMatch = $matchStart + $match.Length

        # Skip whitespace to find the opening brace
        $braceStart = $afterMatch
        while ($braceStart -lt $Content.Length -and $Content[$braceStart] -match '\s') { $braceStart++ }

        if ($braceStart -ge $Content.Length -or $Content[$braceStart] -ne '{') {
            # No brace found — single-statement guard, skip for safety
            $Content = $Content.Substring(0, $matchStart) + "/* TODO: IsPostBack guard — review for Blazor */ " + $Content.Substring($matchStart)
            $todoCount++
            continue
        }

        # Brace-count to find matching close brace
        $depth = 1
        $pos = $braceStart + 1
        while ($pos -lt $Content.Length -and $depth -gt 0) {
            if ($Content[$pos] -eq '{') { $depth++ }
            elseif ($Content[$pos] -eq '}') { $depth-- }
            $pos++
        }

        if ($depth -ne 0) {
            # Unbalanced braces — skip
            $Content = $Content.Substring(0, $matchStart) + "/* TODO: IsPostBack guard — could not parse */ " + $Content.Substring($matchStart)
            $todoCount++
            continue
        }

        $braceEnd = $pos - 1  # position of closing brace

        # Check for else clause after the closing brace
        $afterClose = $braceEnd + 1
        $checkPos = $afterClose
        while ($checkPos -lt $Content.Length -and $Content[$checkPos] -match '\s') { $checkPos++ }

        $hasElse = ($checkPos + 3 -lt $Content.Length) -and ($Content.Substring($checkPos, 4) -match '^else\b')

        if ($hasElse) {
            # Complex case — add TODO comment instead of unwrapping
            $todoComment = "// TODO: BWFC — IsPostBack guard with else clause. In Blazor, OnInitializedAsync runs once (no postback).`n            // Review: move 'if' body to OnInitializedAsync and 'else' body to an event handler or remove.`n            "
            $Content = $Content.Substring(0, $matchStart) + $todoComment + $Content.Substring($matchStart)
            $todoCount++
        }
        else {
            # Simple case — unwrap the guard: extract body, dedent one level, add comment
            $body = $Content.Substring($braceStart + 1, $braceEnd - $braceStart - 1)

            # Dedent: remove one level of leading whitespace (4 spaces or 1 tab) from each line
            $bodyLines = $body -split "`n"
            $dedentedLines = foreach ($line in $bodyLines) {
                $line -replace '^(    |\t)', ''
            }
            $dedentedBody = ($dedentedLines -join "`n").Trim()

            # Determine the indentation of the original if statement
            $lineStart = $matchStart
            while ($lineStart -gt 0 -and $Content[$lineStart - 1] -ne "`n") { $lineStart-- }
            $indent = ''
            if ($Content.Substring($lineStart, $matchStart - $lineStart) -match '^(\s+)') {
                $indent = $Matches[1]
            }

            $replacement = "${indent}// BWFC: IsPostBack guard unwrapped — Blazor re-renders on every state change`n"
            # Re-indent the dedented body
            foreach ($line in $dedentedBody -split "`n") {
                if ($line.Trim().Length -gt 0) {
                    $replacement += "${indent}${line}`n"
                } else {
                    $replacement += "`n"
                }
            }

            $Content = $Content.Substring(0, $matchStart) + $replacement.TrimEnd("`n") + $Content.Substring($braceEnd + 1)
            $unwrapCount++
        }
    }

    if ($unwrapCount -gt 0) {
        Write-TransformLog -File $RelPath -Transform 'IsPostBack' -Detail "Unwrapped $unwrapCount simple IsPostBack guard(s)"
    }
    if ($todoCount -gt 0) {
        Write-TransformLog -File $RelPath -Transform 'IsPostBack' -Detail "Added TODO for $todoCount complex IsPostBack guard(s)"
        Write-ManualItem -File $RelPath -Category 'IsPostBack' -Detail "$todoCount IsPostBack guard(s) with else clauses need manual review"
    }

    return $Content
}

function Convert-PageLifecycleMethods {
    <#
    .SYNOPSIS
        Transforms Web Forms page lifecycle methods to Blazor equivalents (GAP-05).
    .DESCRIPTION
        Converts:
          Page_Load(object sender, EventArgs e) → protected override async Task OnInitializedAsync()
          Page_Init(object sender, EventArgs e) → protected override void OnInitialized()
          Page_PreRender(object sender, EventArgs e) → protected override async Task OnAfterRenderAsync(bool firstRender)
    #>
    param(
        [string]$Content,
        [string]$RelPath
    )

    $convertCount = 0

    # --- Page_Load → OnInitializedAsync ---
    # Matches any access modifier combination + void + Page_Load (case-insensitive method name)
    $pageLoadRegex = [regex]'(?m)([ \t]*)(?:(?:protected|private|public|internal)\s+)?(?:(?:virtual|override|new|static|sealed|abstract)\s+)*void\s+(?i:Page_Load)\s*\(\s*object\s+\w+\s*,\s*EventArgs\s+\w+\s*\)'

    if ($pageLoadRegex.IsMatch($Content)) {
        $match = $pageLoadRegex.Match($Content)
        $indent = $match.Groups[1].Value
        $matchStart = $match.Index
        $matchEnd = $matchStart + $match.Length

        $newSig = "${indent}protected override async Task OnInitializedAsync()"
        $Content = $Content.Substring(0, $matchStart) + $newSig + $Content.Substring($matchEnd)

        # Find opening brace after the new signature
        $sigEnd = $matchStart + $newSig.Length
        $bracePos = $sigEnd
        while ($bracePos -lt $Content.Length -and $Content[$bracePos] -match '\s') { $bracePos++ }

        if ($bracePos -lt $Content.Length -and $Content[$bracePos] -eq '{') {
            $injection = "`n${indent}    // TODO: Review lifecycle conversion — verify async behavior`n${indent}    await base.OnInitializedAsync();`n"
            $Content = $Content.Substring(0, $bracePos + 1) + $injection + $Content.Substring($bracePos + 1)
        }

        $convertCount++
        Write-TransformLog -File $RelPath -Transform 'LifecycleConvert' -Detail "Page_Load → OnInitializedAsync"
    }

    # --- Page_Init → OnInitialized ---
    $pageInitRegex = [regex]'(?m)([ \t]*)(?:(?:protected|private|public|internal)\s+)?(?:(?:virtual|override|new|static|sealed|abstract)\s+)*void\s+(?i:Page_Init)\s*\(\s*object\s+\w+\s*,\s*EventArgs\s+\w+\s*\)'

    if ($pageInitRegex.IsMatch($Content)) {
        $match = $pageInitRegex.Match($Content)
        $indent = $match.Groups[1].Value
        $matchStart = $match.Index
        $matchEnd = $matchStart + $match.Length

        $newSig = "${indent}protected override void OnInitialized()"
        $Content = $Content.Substring(0, $matchStart) + $newSig + $Content.Substring($matchEnd)

        # Find opening brace and inject TODO comment
        $sigEnd = $matchStart + $newSig.Length
        $bracePos = $sigEnd
        while ($bracePos -lt $Content.Length -and $Content[$bracePos] -match '\s') { $bracePos++ }

        if ($bracePos -lt $Content.Length -and $Content[$bracePos] -eq '{') {
            $injection = "`n${indent}    // TODO: Review lifecycle conversion — verify async behavior`n"
            $Content = $Content.Substring(0, $bracePos + 1) + $injection + $Content.Substring($bracePos + 1)
        }

        $convertCount++
        Write-TransformLog -File $RelPath -Transform 'LifecycleConvert' -Detail "Page_Init → OnInitialized"
    }

    # --- Page_PreRender → OnAfterRenderAsync ---
    $preRenderRegex = [regex]'(?m)([ \t]*)(?:(?:protected|private|public|internal)\s+)?(?:(?:virtual|override|new|static|sealed|abstract)\s+)*void\s+(?i:Page_PreRender)\s*\(\s*object\s+\w+\s*,\s*EventArgs\s+\w+\s*\)'

    if ($preRenderRegex.IsMatch($Content)) {
        $match = $preRenderRegex.Match($Content)
        $indent = $match.Groups[1].Value
        $matchStart = $match.Index
        $matchEnd = $matchStart + $match.Length

        $newSig = "${indent}protected override async Task OnAfterRenderAsync(bool firstRender)"
        $Content = $Content.Substring(0, $matchStart) + $newSig + $Content.Substring($matchEnd)

        # Find opening brace
        $sigEnd = $matchStart + $newSig.Length
        $braceStart = $sigEnd
        while ($braceStart -lt $Content.Length -and $Content[$braceStart] -match '\s') { $braceStart++ }

        if ($braceStart -lt $Content.Length -and $Content[$braceStart] -eq '{') {
            # Brace-count to find matching close brace
            $depth = 1
            $pos = $braceStart + 1
            while ($pos -lt $Content.Length -and $depth -gt 0) {
                if ($Content[$pos] -eq '{') { $depth++ }
                elseif ($Content[$pos] -eq '}') { $depth-- }
                $pos++
            }

            if ($depth -eq 0) {
                $braceEnd = $pos - 1
                $body = $Content.Substring($braceStart + 1, $braceEnd - $braceStart - 1)
                $bodyIndent = "${indent}    "

                # Build wrapped body with firstRender guard
                $newBody = "`n${bodyIndent}// TODO: Review lifecycle conversion — verify async behavior"
                $newBody += "`n${bodyIndent}if (firstRender)"
                $newBody += "`n${bodyIndent}{"

                # Re-indent original body lines by one level
                $bodyLines = $body -split "`n"
                foreach ($line in $bodyLines) {
                    $trimmed = $line.TrimEnd()
                    if ($trimmed.Length -gt 0) {
                        $newBody += "`n    $trimmed"
                    }
                }

                $newBody += "`n${bodyIndent}}"
                $newBody += "`n${indent}"

                $Content = $Content.Substring(0, $braceStart + 1) + $newBody + $Content.Substring($braceEnd)
            }
        }

        $convertCount++
        Write-TransformLog -File $RelPath -Transform 'LifecycleConvert' -Detail "Page_PreRender → OnAfterRenderAsync with firstRender guard"
    }

    if ($convertCount -gt 0) {
        Write-TransformLog -File $RelPath -Transform 'LifecycleConvert' -Detail "Converted $convertCount page lifecycle method(s) to Blazor equivalents"
    }

    return $Content
}

function Convert-EventHandlerSignatures {
    <#
    .SYNOPSIS
        Transforms Web Forms event handler signatures to Blazor-compatible signatures (GAP-07).
    .DESCRIPTION
        Rules:
          - If EventArgs type is EXACTLY 'EventArgs' → strip both params: Handler()
          - If EventArgs type is a specialized subclass (e.g., GridViewCommandEventArgs) →
            strip 'object sender', keep the specialized EventArgs: Handler(GridViewCommandEventArgs e)
          - Access modifiers are preserved as-is
          - async is NOT added unless already present
    #>
    param(
        [string]$Content,
        [string]$RelPath
    )

    $stripCount = 0
    $keepCount = 0

    # Match methods with (object sender, *EventArgs param) signature.
    # Group 1: everything before the parens (modifiers + return type + method name)
    # Group 2: the EventArgs type name (must end with 'EventArgs')
    # Group 3: the EventArgs parameter name
    $handlerRegex = [regex]'((?:(?:protected|private|public|internal)\s+)?(?:(?:static|virtual|override|new|sealed|abstract|async)\s+)*(?:void|Task(?:<[^>]+>)?)\s+\w+)\s*\(\s*object\s+\w+\s*,\s*(\w*EventArgs)\s+(\w+)\s*\)'

    $maxIterations = 200
    $iterations = 0

    while ($handlerRegex.IsMatch($Content) -and $iterations -lt $maxIterations) {
        $iterations++
        $match = $handlerRegex.Match($Content)
        $prefix = $match.Groups[1].Value
        $eventArgsType = $match.Groups[2].Value
        $eventArgsParam = $match.Groups[3].Value

        if ($eventArgsType -eq 'EventArgs') {
            # Standard EventArgs — strip both params entirely
            $replacement = "${prefix}()"
            $Content = $Content.Substring(0, $match.Index) + $replacement + $Content.Substring($match.Index + $match.Length)
            $stripCount++
        }
        else {
            # Specialized EventArgs — strip sender, keep the EventArgs param
            $replacement = "${prefix}($eventArgsType $eventArgsParam)"
            $Content = $Content.Substring(0, $match.Index) + $replacement + $Content.Substring($match.Index + $match.Length)
            $keepCount++
        }
    }

    if ($stripCount -gt 0) {
        Write-TransformLog -File $RelPath -Transform 'EventHandler' -Detail "Stripped sender+EventArgs from $stripCount standard event handler(s)"
    }
    if ($keepCount -gt 0) {
        Write-TransformLog -File $RelPath -Transform 'EventHandler' -Detail "Stripped sender, kept specialized EventArgs in $keepCount event handler(s)"
    }

    return $Content
}

function Copy-CodeBehind {
    param(
        [string]$SourceFile,
        [string]$OutputFile,
        [string]$RelPath
    )

    if ($PSCmdlet.ShouldProcess($OutputFile, "Copy code-behind with TODO annotations")) {
        $content = Get-Content -Path $SourceFile -Raw -Encoding UTF8

        $todoHeader = @"
// =============================================================================
// TODO: This code-behind was copied from Web Forms and needs manual migration.
//
// Common transforms needed (use the BWFC Copilot skill for assistance):
//   - Page_Load / Page_Init → OnInitializedAsync / OnParametersSetAsync
//   - Page_PreRender → OnAfterRenderAsync
//   - IsPostBack checks → remove or convert to state logic
//   - ViewState usage → component [Parameter] or private fields
//   - Session/Cache access → auto-wired on WebFormsPageBase via SessionShim/CacheShim
//   - Response.Redirect → auto-wired on WebFormsPageBase via ResponseShim
//   - Request.Form["key"] → auto-wired on WebFormsPageBase via FormShim (use <WebFormsForm> for interactive mode)
//   - Server.MapPath/HtmlEncode → auto-wired on WebFormsPageBase via ServerShim
//   - ConfigurationManager.AppSettings → BWFC shim (call app.UseConfigurationManagerShim() in Program.cs)
//   - ClientScript.RegisterStartupScript → auto-wired on WebFormsPageBase via ClientScriptShim
//   - Event handlers (Button_Click, etc.) → convert to Blazor event callbacks
//   - Data binding (DataBind, DataSource) → component parameters or OnInitialized
//   - ScriptManager code-behind references → use ScriptManagerShim via ScriptManager.GetCurrent(this)
//   - UpdatePanel markup preserved by BWFC (ContentTemplate supported) — remove only code-behind API calls
//   - User controls → Blazor component references
// =============================================================================

"@

        $annotatedContent = $todoHeader + $content

        # --- GAP-09: Selective using retention ---
        # Before stripping System.Web.* usings, preserve specific ones that have BWFC shims.
        # Convert retained usings to their BWFC-compatible equivalents.
        $retainedUsingCount = 0

        # Keep using System.Configuration; — BWFC provides ConfigurationManager shim
        # (No transformation needed — the shim lives in the BlazorWebFormsComponents namespace
        # which is already in GlobalUsings.cs via the .targets file)
        $sysConfigRegex = [regex]'using\s+System\.Configuration;\s*\r?\n?'
        $sysConfigMatches = $sysConfigRegex.Matches($annotatedContent)
        if ($sysConfigMatches.Count -gt 0) {
            # Replace with a comment — ConfigurationManager is available via BWFC namespace
            $annotatedContent = $sysConfigRegex.Replace($annotatedContent, "// using System.Configuration; // BWFC: ConfigurationManager shim available via BlazorWebFormsComponents namespace`n")
            $retainedUsingCount += $sysConfigMatches.Count
            Write-TransformLog -File $RelPath -Transform 'UsingRetention' -Detail "Retained System.Configuration reference (BWFC ConfigurationManager shim)"
        }

        # Keep using System.Web.Optimization; when BundleConfig patterns exist
        $webOptRegex = [regex]'using\s+System\.Web\.Optimization;\s*\r?\n?'
        $webOptMatches = $webOptRegex.Matches($annotatedContent)
        if ($webOptMatches.Count -gt 0) {
            $annotatedContent = $webOptRegex.Replace($annotatedContent, "// using System.Web.Optimization; // BWFC: BundleConfig stubs available via BlazorWebFormsComponents namespace`n")
            $retainedUsingCount += $webOptMatches.Count
            Write-TransformLog -File $RelPath -Transform 'UsingRetention' -Detail "Retained System.Web.Optimization reference (BWFC BundleConfig stubs)"
        }

        # Keep using System.Web.Routing; when RouteConfig patterns exist
        $webRoutingRegex = [regex]'using\s+System\.Web\.Routing;\s*\r?\n?'
        $webRoutingMatches = $webRoutingRegex.Matches($annotatedContent)
        if ($webRoutingMatches.Count -gt 0) {
            $annotatedContent = $webRoutingRegex.Replace($annotatedContent, "// using System.Web.Routing; // BWFC: RouteConfig stubs available via BlazorWebFormsComponents namespace`n")
            $retainedUsingCount += $webRoutingMatches.Count
            Write-TransformLog -File $RelPath -Transform 'UsingRetention' -Detail "Retained System.Web.Routing reference (BWFC RouteConfig stubs)"
        }

        if ($retainedUsingCount -gt 0) {
            Write-TransformLog -File $RelPath -Transform 'UsingRetention' -Detail "Selectively retained $retainedUsingCount using(s) with BWFC shim equivalents"
        }

        # Strip System.Web.UI.* usings — Web Forms UI namespaces with no Blazor equivalent
        $webUIUsingsRegex = [regex]'using\s+System\.Web\.UI(\.\w+)*;\s*\r?\n?'
        $webUIUsingMatches = $webUIUsingsRegex.Matches($annotatedContent)
        if ($webUIUsingMatches.Count -gt 0) {
            $annotatedContent = $webUIUsingsRegex.Replace($annotatedContent, '')
            Write-TransformLog -File $RelPath -Transform 'CodeBehind' -Detail "Stripped $($webUIUsingMatches.Count) System.Web.UI.* using(s)"
        }

        # Strip System.Web.Security — no Blazor equivalent
        $webSecurityRegex = [regex]'using\s+System\.Web\.Security;\s*\r?\n?'
        $webSecurityMatches = $webSecurityRegex.Matches($annotatedContent)
        if ($webSecurityMatches.Count -gt 0) {
            $annotatedContent = $webSecurityRegex.Replace($annotatedContent, '')
            Write-TransformLog -File $RelPath -Transform 'CodeBehind' -Detail "Stripped $($webSecurityMatches.Count) System.Web.Security using(s)"
        }

        # Strip remaining System.Web.* usings that weren't selectively retained above
        # (System.Web.Optimization and System.Web.Routing were already handled)
        $webUsingsRegex = [regex]'using\s+System\.Web(\.\w+)*;\s*\r?\n?'
        $webUsingMatches = $webUsingsRegex.Matches($annotatedContent)
        if ($webUsingMatches.Count -gt 0) {
            $annotatedContent = $webUsingsRegex.Replace($annotatedContent, '')
            Write-TransformLog -File $RelPath -Transform 'CodeBehind' -Detail "Stripped $($webUsingMatches.Count) System.Web.* using(s)"
        }

        # Strip Microsoft.AspNet.* usings — OWIN Identity namespaces; replaced by GlobalUsings + shims
        $aspnetUsingsRegex = [regex]'using\s+Microsoft\.AspNet(\.\w+)*;\s*\r?\n?'
        $aspnetUsingMatches = $aspnetUsingsRegex.Matches($annotatedContent)
        if ($aspnetUsingMatches.Count -gt 0) {
            $annotatedContent = $aspnetUsingsRegex.Replace($annotatedContent, '')
            Write-TransformLog -File $RelPath -Transform 'CodeBehind' -Detail "Stripped $($aspnetUsingMatches.Count) Microsoft.AspNet.* using(s)"
        }

        # Strip Microsoft.Owin.* usings — OWIN middleware namespaces; no Blazor equivalent
        $owinUsingsRegex = [regex]'using\s+Microsoft\.Owin(\.\w+)*;\s*\r?\n?'
        $owinUsingMatches = $owinUsingsRegex.Matches($annotatedContent)
        if ($owinUsingMatches.Count -gt 0) {
            $annotatedContent = $owinUsingsRegex.Replace($annotatedContent, '')
            Write-TransformLog -File $RelPath -Transform 'CodeBehind' -Detail "Stripped $($owinUsingMatches.Count) Microsoft.Owin.* using(s)"
        }

        # Strip Owin usings — bare OWIN namespace
        $bareOwinRegex = [regex]'using\s+Owin;\s*\r?\n?'
        $bareOwinMatches = $bareOwinRegex.Matches($annotatedContent)
        if ($bareOwinMatches.Count -gt 0) {
            $annotatedContent = $bareOwinRegex.Replace($annotatedContent, '')
            Write-TransformLog -File $RelPath -Transform 'CodeBehind' -Detail "Stripped $($bareOwinMatches.Count) Owin using(s)"
        }

        # Strip Web Forms base class declarations from code-behind classes.
        # The .razor file already has @inherits WebFormsPageBase, so the code-behind
        # partial class must NOT also declare a base class (CS0263).
        # Handle both FQN (System.Web.UI.Page) and unqualified (Page) forms.
        $baseClassPatterns = @(
            'System\.Web\.UI\.Page',
            'System\.Web\.UI\.MasterPage',
            'System\.Web\.UI\.UserControl',
            '(?<!\w)Page(?!\w)',
            '(?<!\w)MasterPage(?!\w)',
            '(?<!\w)UserControl(?!\w)'
        )
        $baseClassRegex = [regex]('(partial\s+class\s+\w+)\s*:\s*(' + ($baseClassPatterns -join '|') + ')')
        $baseClassMatches = $baseClassRegex.Matches($annotatedContent)
        if ($baseClassMatches.Count -gt 0) {
            $annotatedContent = $baseClassRegex.Replace($annotatedContent, '$1')
            Write-TransformLog -File $RelPath -Transform 'CodeBehind' -Detail "Stripped $($baseClassMatches.Count) Web Forms base class declaration(s) — .razor @inherits handles inheritance"
        }

        # RF-12: [QueryString] and [RouteData] parameter attributes
        # BWFC provides [QueryString] and [RouteData] attributes that target
        # AttributeTargets.Parameter, so these compile as-is on method params.
        # Leave them in place — L2 promotes them to component properties.
        $qsRegex = [regex]'\[QueryString\('
        $qsMatches = $qsRegex.Matches($annotatedContent)
        if ($qsMatches.Count -gt 0) {
            Write-TransformLog -File $RelPath -Transform 'ParameterAttr' -Detail "Preserved $($qsMatches.Count) [QueryString] attribute(s) — BWFC shim compiles; L2 promotes to [SupplyParameterFromQuery] property"
        }
        $rdRegex = [regex]'\[RouteData\]'
        $rdMatches = $rdRegex.Matches($annotatedContent)
        if ($rdMatches.Count -gt 0) {
            Write-TransformLog -File $RelPath -Transform 'ParameterAttr' -Detail "Preserved $($rdMatches.Count) [RouteData] attribute(s) — BWFC shim compiles; L2 promotes to [Parameter] property"
        }

        # --- Response.Redirect → NavigationManager.NavigateTo conversion ---
        # Converts server-side redirects to Blazor navigation. Preserves .aspx in URLs
        # since AspxRewriteMiddleware handles rewriting at runtime. Strips ~/ prefix.
        $hasRedirectConversion = $false

        # Pattern 1: Response.Redirect("url", bool) — literal URL with endResponse parameter
        $redirectLitBoolRegex = [regex]'Response\.Redirect\(\s*"([^"]*)"\s*,\s*(?:true|false)\s*\)'
        $redirectLitBoolMatches = $redirectLitBoolRegex.Matches($annotatedContent)
        if ($redirectLitBoolMatches.Count -gt 0) {
            $annotatedContent = $redirectLitBoolRegex.Replace($annotatedContent, {
                param($m)
                $url = $m.Groups[1].Value -replace '^~/', '/'
                "NavigationManager.NavigateTo(`"$url`")"
            })
            $hasRedirectConversion = $true
            Write-TransformLog -File $RelPath -Transform 'ResponseRedirect' -Detail "Converted $($redirectLitBoolMatches.Count) Response.Redirect(url, bool) → NavigationManager.NavigateTo()"
        }

        # Pattern 2: Response.Redirect("url") — simple literal URL
        $redirectLitRegex = [regex]'Response\.Redirect\(\s*"([^"]*)"\s*\)'
        $redirectLitMatches = $redirectLitRegex.Matches($annotatedContent)
        if ($redirectLitMatches.Count -gt 0) {
            $annotatedContent = $redirectLitRegex.Replace($annotatedContent, {
                param($m)
                $url = $m.Groups[1].Value -replace '^~/', '/'
                "NavigationManager.NavigateTo(`"$url`")"
            })
            $hasRedirectConversion = $true
            Write-TransformLog -File $RelPath -Transform 'ResponseRedirect' -Detail "Converted $($redirectLitMatches.Count) Response.Redirect(url) → NavigationManager.NavigateTo()"
        }

        # Pattern 3: Response.Redirect(expr, bool) — expression URL with endResponse
        $redirectExprBoolRegex = [regex]'Response\.Redirect\(\s*([^,)]+)\s*,\s*(?:true|false)\s*\)'
        $redirectExprBoolMatches = $redirectExprBoolRegex.Matches($annotatedContent)
        if ($redirectExprBoolMatches.Count -gt 0) {
            $annotatedContent = $redirectExprBoolRegex.Replace($annotatedContent, 'NavigationManager.NavigateTo($1) /* TODO: Verify navigation target */')
            $hasRedirectConversion = $true
            Write-TransformLog -File $RelPath -Transform 'ResponseRedirect' -Detail "Converted $($redirectExprBoolMatches.Count) Response.Redirect(expr, bool) → NavigationManager.NavigateTo()"
        }

        # Pattern 4: Response.Redirect(expr) — remaining expression URLs
        $redirectExprRegex = [regex]'Response\.Redirect\(\s*([^)]+)\s*\)'
        $redirectExprMatches = $redirectExprRegex.Matches($annotatedContent)
        if ($redirectExprMatches.Count -gt 0) {
            $annotatedContent = $redirectExprRegex.Replace($annotatedContent, 'NavigationManager.NavigateTo($1) /* TODO: Verify navigation target */')
            $hasRedirectConversion = $true
            Write-TransformLog -File $RelPath -Transform 'ResponseRedirect' -Detail "Converted $($redirectExprMatches.Count) Response.Redirect(expr) → NavigationManager.NavigateTo()"
        }

        # Inject [Inject] NavigationManager if any redirect conversions were made
        if ($hasRedirectConversion) {
            $classOpenRegex = [regex]'((?:public|internal|private)\s+(?:partial\s+)?class\s+\w+[^{]*\{)'
            if ($classOpenRegex.IsMatch($annotatedContent)) {
                $injectLine = "`n    [Inject] private NavigationManager NavigationManager { get; set; } // TODO: Add @using Microsoft.AspNetCore.Components to _Imports.razor if needed`n"
                $annotatedContent = $classOpenRegex.Replace($annotatedContent, "`$1$injectLine", 1)
                Write-TransformLog -File $RelPath -Transform 'ResponseRedirect' -Detail "Injected [Inject] NavigationManager into class"
            }
            Write-ManualItem -File $RelPath -Category 'ResponseRedirect' -Detail "Response.Redirect converted to NavigationManager.NavigateTo — verify navigation targets"
        }

        # --- Session["key"] detection ---
        # Session state has no direct Blazor equivalent. Detect usage and add migration guidance.
        $sessionKeyRegex = [regex]'Session\["([^"]*)"\]'
        $sessionMatches = $sessionKeyRegex.Matches($annotatedContent)
        if ($sessionMatches.Count -gt 0) {
            $sessionKeys = [System.Collections.Generic.List[string]]::new()
            foreach ($m in $sessionMatches) {
                $key = $m.Groups[1].Value
                if (-not $sessionKeys.Contains($key)) { $sessionKeys.Add($key) }
            }
            # Insert guidance block after the existing TODO header
            $sessionBlock = "// --- Session State Migration ---`n"
            $sessionBlock += "// Session keys found: $($sessionKeys -join ', ')`n"
            $sessionBlock += "// Options:`n"
            $sessionBlock += "//   (1) ProtectedSessionStorage (Blazor Server) — persists across circuits`n"
            $sessionBlock += "//   (2) Scoped service via DI — lifetime matches user circuit`n"
            $sessionBlock += "//   (3) Cascading parameter from a root-level state provider`n"
            $sessionBlock += "// See: https://learn.microsoft.com/aspnet/core/blazor/state-management`n`n"
            $todoEndMarker = '// ============================================================================='
            $lastTodoIdx = $annotatedContent.LastIndexOf($todoEndMarker)
            if ($lastTodoIdx -ge 0) {
                $insertPos = $lastTodoIdx + $todoEndMarker.Length
                # Skip past the newlines after the marker
                while ($insertPos -lt $annotatedContent.Length -and $annotatedContent[$insertPos] -match '[\r\n]') { $insertPos++ }
                $annotatedContent = $annotatedContent.Substring(0, $insertPos) + "`n" + $sessionBlock + $annotatedContent.Substring($insertPos)
            }
            Write-TransformLog -File $RelPath -Transform 'SessionDetect' -Detail "Detected $($sessionMatches.Count) Session[`"key`"] usage(s) — keys: $($sessionKeys -join ', ')"
            Write-ManualItem -File $RelPath -Category 'SessionState' -Detail "Session keys used: $($sessionKeys -join ', ') — convert to scoped service or ProtectedSessionStorage"
        }

        # --- ViewState["key"] detection ---
        # ViewState is in-memory only in Blazor (does not survive navigation).
        # BaseWebFormsComponent and WebFormsPageBase have [Obsolete] ViewState dictionaries
        # for compatibility, but proper migration converts to private fields.
        $viewStateKeyRegex = [regex]'ViewState\["([^"]*)"\]'
        $vsMatches = $viewStateKeyRegex.Matches($annotatedContent)
        if ($vsMatches.Count -gt 0) {
            $vsKeys = [System.Collections.Generic.List[string]]::new()
            foreach ($m in $vsMatches) {
                $key = $m.Groups[1].Value
                if (-not $vsKeys.Contains($key)) { $vsKeys.Add($key) }
            }
            # Generate suggested private field declarations
            $vsBlock = "// --- ViewState Migration ---`n"
            $vsBlock += "// ViewState is in-memory only in Blazor (does not survive navigation).`n"
            $vsBlock += "// Convert to private fields or [Parameter] properties:`n"
            foreach ($key in $vsKeys) {
                $fieldName = '_' + $key.Substring(0,1).ToLower() + $key.Substring(1)
                $vsBlock += "//   private object $fieldName; // was ViewState[`"$key`"]`n"
            }
            $vsBlock += "// Note: BaseWebFormsComponent.ViewState exists as an [Obsolete] compatibility shim.`n`n"
            $todoEndMarker2 = '// ============================================================================='
            $lastTodoIdx2 = $annotatedContent.LastIndexOf($todoEndMarker2)
            if ($lastTodoIdx2 -ge 0) {
                $insertPos2 = $lastTodoIdx2 + $todoEndMarker2.Length
                while ($insertPos2 -lt $annotatedContent.Length -and $annotatedContent[$insertPos2] -match '[\r\n]') { $insertPos2++ }
                $annotatedContent = $annotatedContent.Substring(0, $insertPos2) + "`n" + $vsBlock + $annotatedContent.Substring($insertPos2)
            }
            Write-TransformLog -File $RelPath -Transform 'ViewStateDetect' -Detail "Detected $($vsMatches.Count) ViewState[`"key`"] usage(s) — keys: $($vsKeys -join ', ')"
            Write-ManualItem -File $RelPath -Category 'ViewState' -Detail "ViewState keys used: $($vsKeys -join ', ') — convert to private fields (see generated suggestions in code-behind)"
        }

        # --- GAP-06: IsPostBack guard unwrapping ---
        $annotatedContent = Remove-IsPostBackGuards -Content $annotatedContent -RelPath $RelPath

        # --- GAP-20: .aspx URL cleanup in code-behind string literals ---
        # Replace "~/SomePage.aspx" → "/SomePage" and "~/SomePage.aspx?param=val" → "/SomePage?param=val"
        # Also handle relative .aspx references without ~/
        $aspxUrlCount = 0

        # Pattern 1: "~/SomePage.aspx?query" → "/SomePage?query" (with query string)
        $aspxTildeQsRegex = [regex]'"~/([^"]*?)\.aspx\?([^"]*)"'
        $aspxTildeQsMatches = $aspxTildeQsRegex.Matches($annotatedContent)
        if ($aspxTildeQsMatches.Count -gt 0) {
            $annotatedContent = $aspxTildeQsRegex.Replace($annotatedContent, '"/$1?$2"')
            $aspxUrlCount += $aspxTildeQsMatches.Count
        }

        # Pattern 2: "~/SomePage.aspx" → "/SomePage" (no query string)
        $aspxTildeRegex = [regex]'"~/([^"]*?)\.aspx"'
        $aspxTildeMatches = $aspxTildeRegex.Matches($annotatedContent)
        if ($aspxTildeMatches.Count -gt 0) {
            $annotatedContent = $aspxTildeRegex.Replace($annotatedContent, '"/$1"')
            $aspxUrlCount += $aspxTildeMatches.Count
        }

        # Pattern 3: Relative .aspx references in NavigateTo/Redirect calls: "SomePage.aspx?q" → "/SomePage?q"
        $aspxRelQsRegex = [regex]'(NavigationManager\.NavigateTo\(\s*")([^"~/][^"]*?)\.aspx\?([^"]*)"'
        $aspxRelQsMatches = $aspxRelQsRegex.Matches($annotatedContent)
        if ($aspxRelQsMatches.Count -gt 0) {
            $annotatedContent = $aspxRelQsRegex.Replace($annotatedContent, '$1/$2?$3"')
            $aspxUrlCount += $aspxRelQsMatches.Count
        }

        # Pattern 4: Relative .aspx references in NavigateTo/Redirect calls: "SomePage.aspx" → "/SomePage"
        $aspxRelRegex = [regex]'(NavigationManager\.NavigateTo\(\s*")([^"~/][^"]*?)\.aspx"'
        $aspxRelMatches = $aspxRelRegex.Matches($annotatedContent)
        if ($aspxRelMatches.Count -gt 0) {
            $annotatedContent = $aspxRelRegex.Replace($annotatedContent, '$1/$2"')
            $aspxUrlCount += $aspxRelMatches.Count
        }

        if ($aspxUrlCount -gt 0) {
            Write-TransformLog -File $RelPath -Transform 'AspxUrlCleanup' -Detail "Cleaned $aspxUrlCount .aspx URL reference(s) in string literals"
        }

        # --- GAP-05: Page lifecycle method conversion ---
        $annotatedContent = Convert-PageLifecycleMethods -Content $annotatedContent -RelPath $RelPath

        # --- GAP-07: Event handler signature conversion ---
        $annotatedContent = Convert-EventHandlerSignatures -Content $annotatedContent -RelPath $RelPath

        $outputDir = Split-Path $OutputFile -Parent
        if (-not (Test-Path $outputDir)) {
            New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
        }

        Set-Content -Path $OutputFile -Value $annotatedContent -Encoding UTF8
        Write-TransformLog -File $RelPath -Transform 'CodeBehind' -Detail "Copied with TODO annotations → $OutputFile"
    }
    else {
        Write-Host "[WhatIf] Would copy code-behind: $RelPath → $OutputFile"
    }
}

#endregion

#region --- Unconvertible Page Stubs ---

function Test-UnconvertiblePage {
    param(
        [string]$Content,
        [string]$RelativePath = ''
    )

    # P0-1: Eliminated all stubbing. Pages are ALWAYS converted with BWFC components.
    # If a page has concerns (Identity, Checkout, PayPal), TODO comments are injected
    # at the call site instead of replacing the entire page with a stub.
    return $false
}

function Test-RedirectHandler {
    param(
        [string]$MarkupContent,
        [string]$CodeBehindPath
    )

    if (-not (Test-Path $CodeBehindPath)) { return $false }
    $cbContent = Get-Content -Path $CodeBehindPath -Raw -Encoding UTF8
    if ($cbContent -notmatch 'Response\.Redirect') { return $false }

    # Check if markup is minimal (strip directives, check remaining content)
    $stripped = $MarkupContent -replace '<%@\s*\w+[^%]*%>\s*', ''
    $stripped = $stripped.Trim()
    return ($stripped.Length -lt 100)
}

function New-CompilableStub {
    param(
        [string]$Route,
        [string]$RelPath,
        [string]$OutputFile,
        [string]$OutputDir
    )

    $displayName = $RelPath -replace '\\', '/' -replace '\.aspx$', ''
    $stubContent = @"
@page "$Route"
<h3>$displayName — not yet migrated</h3>
@* TODO: Implement using ASP.NET Core Identity scaffolding *@
@code {
}
"@

    if ($PSCmdlet.ShouldProcess($OutputFile, "Write compilable stub for unconvertible page")) {
        if (-not (Test-Path $OutputDir)) {
            New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
        }
        Set-Content -Path $OutputFile -Value $stubContent -Encoding UTF8
    }
    else {
        Write-Host "[WhatIf] Would write stub: $RelPath"
    }

    Write-TransformLog -File $RelPath -Transform 'Stub' -Detail "Generated compilable stub (unconvertible page)"
    Write-ManualItem -File $RelPath -Category 'UnconvertibleStub' -Detail "Page contains Identity/Auth/Payment code — stubbed for clean build"
}

#endregion

#region --- Template Placeholder Conversion (Fix 2) ---

function Convert-TemplatePlaceholders {
    <#
    .SYNOPSIS
        Converts placeholder elements inside *Template blocks to @context.
    .DESCRIPTION
        In ASP.NET Web Forms, elements like <tr id="groupPlaceholder"> inside LayoutTemplate
        and <td id="itemPlaceholder"> inside GroupTemplate are runtime placeholders — the server
        replaces them with rendered content. In BWFC Blazor, LayoutTemplate and GroupTemplate are
        RenderFragment<RenderFragment>, so @context must be used to render children.

        This function finds any element whose id contains "Placeholder" (case-insensitive)
        and replaces it with @context. Handles both self-closing tags and open+close tags
        with optional whitespace content.
    #>
    param(
        [string]$Content,
        [string]$RelPath
    )

    $replacementsMade = 0

    # Pattern 1: Self-closing tags with placeholder ID
    # e.g., <td id="itemPlaceholder" /> or <tr id="groupPlaceholder"/>
    $selfClosingPattern = [regex]'<\w+\s+[^>]*?id\s*=\s*"[^"]*[Pp]laceholder[^"]*"[^>]*/>'
    $selfClosingMatches = $selfClosingPattern.Matches($Content)
    if ($selfClosingMatches.Count -gt 0) {
        $Content = $selfClosingPattern.Replace($Content, '@context')
        $replacementsMade += $selfClosingMatches.Count
    }

    # Pattern 2: Open+close tags with placeholder ID (with optional whitespace/empty content)
    # e.g., <tr id="groupPlaceholder"></tr> or <td id="itemPlaceholder" runat="server">  </td>
    $openClosePattern = [regex]'<(\w+)\s+[^>]*?id\s*=\s*"[^"]*[Pp]laceholder[^"]*"[^>]*>\s*</\1>'
    $openCloseMatches = $openClosePattern.Matches($Content)
    if ($openCloseMatches.Count -gt 0) {
        $Content = $openClosePattern.Replace($Content, '@context')
        $replacementsMade += $openCloseMatches.Count
    }

    if ($replacementsMade -gt 0) {
        Write-TransformLog -File $RelPath -Transform 'PlaceholderToContext' -Detail "Replaced $replacementsMade placeholder element(s) with @context"
    }

    return $Content
}

#endregion

#region --- Main File Transform Pipeline ---

function Convert-WebFormsFile {
    param(
        [string]$SourceFile,
        [string]$OutputRoot,
        [string]$SourceRoot
    )

    $relativePath = $SourceFile.Substring($SourceRoot.Length).TrimStart('\', '/')
    $extension = [System.IO.Path]::GetExtension($SourceFile).ToLower()
    $fileName = [System.IO.Path]::GetFileName($SourceFile)

    if ($VerbosePreference -eq 'Continue') {
        Write-Verbose "Processing: $relativePath"
    }

    # Determine output path with .razor extension
    $razorRelPath = $relativePath
    switch ($extension) {
        '.aspx'   { $razorRelPath = $razorRelPath -replace '\.aspx$', '.razor' }
        '.ascx'   { $razorRelPath = $razorRelPath -replace '\.ascx$', '.razor' }
        '.master' {
            $baseName = [System.IO.Path]::GetFileNameWithoutExtension($fileName)
            if ($baseName -eq 'Site') {
                $razorRelPath = 'Components\Layout\MainLayout.razor'
            } else {
                $razorRelPath = "Components\Layout\${baseName}Layout.razor"
            }
        }
    }

    $outputFile = Join-Path $OutputRoot $razorRelPath
    $outputDir = Split-Path $outputFile -Parent

    # Read source content
    $content = Get-Content -Path $SourceFile -Raw -Encoding UTF8
    if ([string]::IsNullOrEmpty($content)) {
        Write-Warning "Skipping empty file: $relativePath"
        return
    }

    $script:FilesProcessed++

    # RF-08: Check for redirect-only pages
    if ($extension -eq '.aspx') {
        $cbPath = $SourceFile + '.cs'
        if (Test-RedirectHandler -MarkupContent $content -CodeBehindPath $cbPath) {
            $pageName = [System.IO.Path]::GetFileNameWithoutExtension($fileName)
            $script:RedirectHandlers.Add($pageName)
            Write-ManualItem -File $relativePath -Category 'RedirectHandler' -Detail "$pageName was a redirect handler (Response.Redirect in code-behind) — convert to minimal API endpoint"
            Write-TransformLog -File $relativePath -Transform 'RedirectHandler' -Detail "Detected redirect handler — flagged for minimal API conversion"
        }
    }

    # P0-1: Pages are ALWAYS converted — no more stubbing. For pages that need
    # manual attention (Checkout flow, Identity/Auth patterns), inject a TODO
    # comment at the top so developers know to review them post-migration.
    if ($extension -eq '.aspx') {
        $needsTodo = $false
        $todoReason = ''
        if ($relativePath -match '^Checkout[/\\]') {
            $needsTodo = $true
            $todoReason = 'Checkout flow page — requires payment/auth code review after migration'
        }
        elseif ($content -match 'SignInManager|UserManager|FormsAuthentication|Session\[') {
            $needsTodo = $true
            $todoReason = 'Contains Identity/Auth API calls — verify authentication logic after migration'
        }
        if ($needsTodo) {
            $content = "@* TODO: $todoReason *@`n" + $content
            Write-ManualItem -File $relativePath -Category 'NeedsReview' -Detail $todoReason
            Write-TransformLog -File $relativePath -Transform 'TodoAnnotation' -Detail "Injected review TODO — $todoReason"
        }
    }

    # Apply transform pipeline in order
    switch ($extension) {
        '.aspx' {
            $content = ConvertFrom-PageDirective -Content $content -FileName $fileName -RelPath $relativePath -SourceFile $SourceFile
        }
        '.master' {
            $content = ConvertFrom-MasterDirective -Content $content -RelPath $relativePath
            $content = ConvertFrom-MasterPage -Content $content -RelPath $relativePath
        }
        '.ascx' {
            $content = ConvertFrom-ControlDirective -Content $content -RelPath $relativePath
        }
    }

    $content = ConvertFrom-ImportDirective -Content $content -RelPath $relativePath
    $content = ConvertFrom-RegisterDirective -Content $content -RelPath $relativePath
    $content = ConvertFrom-ContentWrappers -Content $content -RelPath $relativePath
    $content = ConvertFrom-FormWrapper -Content $content -RelPath $relativePath
    $content = ConvertFrom-GetRouteUrl -Content $content -RelPath $relativePath
    $content = ConvertFrom-Expressions -Content $content -RelPath $relativePath
    $content = ConvertFrom-LoginView -Content $content -RelPath $relativePath
    $content = ConvertFrom-SelectMethod -Content $content -RelPath $relativePath

    # RF-13: Detect ListView GroupItemCount before asp: prefix is stripped
    $lvGroupRegex = [regex]'(?i)<asp:ListView[^>]*GroupItemCount\s*=\s*"(\d+)"'
    $lvGroupMatches = $lvGroupRegex.Matches($content)
    foreach ($m in $lvGroupMatches) {
        $groupCount = $m.Groups[1].Value
        Write-ManualItem -File $relativePath -Category 'ListView-GroupItemCount' -Detail "ListView with GroupItemCount=$groupCount — use BWFC <ListView GroupItemCount='$groupCount' Items='...' TItem='...'> with GroupTemplate and ItemTemplate"
    }

    $content = ConvertFrom-AjaxToolkitPrefix -Content $content -RelPath $relativePath
    $content = ConvertFrom-AspPrefix -Content $content -RelPath $relativePath
    $content = Remove-WebFormsAttributes -Content $content -RelPath $relativePath
    $content = ConvertFrom-UrlReferences -Content $content -RelPath $relativePath

    # Fix 2: Convert placeholder elements inside *Template blocks to @context
    $content = Convert-TemplatePlaceholders -Content $content -RelPath $relativePath

    # Normalize attribute values: booleans → lowercase, enums → type-qualified, units → strip px
    $content = Normalize-AttributeValues -Content $content -RelPath $relativePath

    # DataSourceID removal and data source control replacement with TODO comments
    $content = Add-DataSourceIDWarning -Content $content -RelPath $relativePath

    # Clean up leftover blank lines from removed directives (collapse 3+ consecutive blank lines to 2)
    $content = $content -replace '(\r?\n){3,}', "`n`n"
    $content = $content.TrimStart("`r", "`n")

    Write-TransformLog -File $relativePath -Transform 'Rename' -Detail "$extension → .razor"

    # Write output
    if ($PSCmdlet.ShouldProcess($outputFile, "Write transformed Razor file")) {
        if (-not (Test-Path $outputDir)) {
            New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
        }
        Set-Content -Path $outputFile -Value $content -Encoding UTF8
    }
    else {
        Write-Host "[WhatIf] Would write: $razorRelPath ($($script:TransformLog.Count) transforms)"
    }

    # Handle code-behind files (.aspx.cs, .aspx.vb, etc.)
    $relevantCbExtensions = $CodeBehindExtensions | Where-Object { $_.StartsWith($extension + '.') }
    foreach ($cbExt in $relevantCbExtensions) {
        $cbSuffix = $cbExt.Substring($extension.Length)  # e.g., ".cs" or ".vb"
        $cbSource = $SourceFile + $cbSuffix

        if (Test-Path $cbSource) {
            $cbRelPath = $cbSource.Substring($SourceRoot.Length).TrimStart('\', '/')
            $cbOutputRel = $cbRelPath
            switch ($extension) {
                '.aspx'   { $cbOutputRel = $cbOutputRel -replace '\.aspx\.', '.razor.' }
                '.ascx'   { $cbOutputRel = $cbOutputRel -replace '\.ascx\.', '.razor.' }
                '.master' { $cbOutputRel = $razorRelPath + $cbSuffix }
            }
            $cbOutputFile = Join-Path $OutputRoot $cbOutputRel
            Copy-CodeBehind -SourceFile $cbSource -OutputFile $cbOutputFile -RelPath $cbRelPath
        }
    }
}

#endregion

#region --- Entry Point ---

# Resolve paths
$Path = (Resolve-Path $Path -ErrorAction Stop).Path
if (-not (Test-Path $Path -PathType Container)) {
    Write-Error "Source path is not a directory: $Path"
    return
}

$projectName = Split-Path $Path -Leaf
if (-not $projectName) {
    $projectName = 'BlazorApp'
}
# Sanitize project name for C# namespace
$projectName = $projectName -replace '[^a-zA-Z0-9_]', ''
if ($projectName -match '^\d') {
    $projectName = '_' + $projectName
}
if ([string]::IsNullOrEmpty($projectName)) {
    $projectName = 'BlazorApp'
}

Write-Host ''
Write-Host '============================================================' -ForegroundColor Cyan
Write-Host '  BWFC Migration Tool — Layer 1: Mechanical Transforms' -ForegroundColor Cyan
Write-Host '============================================================' -ForegroundColor Cyan
Write-Host "  Source:  $Path"
Write-Host "  Output:  $Output"
Write-Host "  Project: $projectName"
if ($WhatIfPreference) {
    Write-Host '  Mode:    WhatIf (dry run)' -ForegroundColor Yellow
}
Write-Host ''

# Create output directory
if (-not $WhatIfPreference) {
    if (-not (Test-Path $Output)) {
        New-Item -ItemType Directory -Path $Output -Force | Out-Null
        Write-Host "Created output directory: $Output"
    }
    $Output = (Resolve-Path $Output).Path
}

# Project scaffolding
if (-not $SkipProjectScaffold) {
    Write-Host 'Generating project scaffold...' -ForegroundColor Green
    if (-not $WhatIfPreference) {
        New-ProjectScaffold -OutputRoot $Output -ProjectName $projectName -SourcePath $Path
        New-AppRazorScaffold -OutputRoot $Output -ProjectName $projectName
    }
    else {
        Write-Host '[WhatIf] Would generate .csproj, _Imports.razor, Program.cs, App.razor, Routes.razor'
    }
    Write-Host ''
}

# GAP-12: Web.config → appsettings.json (after scaffold, before code-behind copy)
Write-Host 'Extracting Web.config settings...' -ForegroundColor Green
if (-not $WhatIfPreference) {
    Convert-WebConfigToAppSettings -SourcePath $Path -OutputRoot $Output
}
else {
    Write-Host '[WhatIf] Would extract appSettings and connectionStrings from Web.config'
}
Write-Host ''

# Discover and transform Web Forms files
Write-Host 'Discovering Web Forms files...' -ForegroundColor Green
$sourceFiles = Get-ChildItem -Path $Path -Recurse -File | Where-Object {
    $ext = $_.Extension.ToLower()
    $ext -in $WebFormsExtensions
}

$fileCount = ($sourceFiles | Measure-Object).Count
Write-Host "Found $fileCount Web Forms file(s) to transform."
Write-Host ''

if ($fileCount -gt 0) {
    Write-Host 'Applying transforms...' -ForegroundColor Green
    foreach ($file in $sourceFiles) {
        Convert-WebFormsFile -SourceFile $file.FullName -OutputRoot $Output -SourceRoot $Path
    }
    Write-Host ''
}

# Copy static files (css, js, images)
$staticFiles = Get-ChildItem -Path $Path -Recurse -File | Where-Object {
    $ext = $_.Extension.ToLower()
    $ext -in $StaticExtensions
}
$staticCount = ($staticFiles | Measure-Object).Count
if ($staticCount -gt 0) {
    Write-Host "Copying $staticCount static file(s)..." -ForegroundColor Green
    foreach ($sf in $staticFiles) {
        $relPath = $sf.FullName.Substring($Path.Length).TrimStart('\', '/')
        $destPath = Join-Path $Output "wwwroot" $relPath
        $destDir = Split-Path $destPath -Parent

        if ($PSCmdlet.ShouldProcess($destPath, "Copy static file")) {
            if (-not (Test-Path $destDir)) {
                New-Item -ItemType Directory -Path $destDir -Force | Out-Null
            }
            Copy-Item -Path $sf.FullName -Destination $destPath -Force
        }
        else {
            Write-Host "[WhatIf] Would copy: $relPath"
        }
    }
    Write-Host ''
}

# Fix 1b: Auto-detect CSS files and inject <link> tags into App.razor
if (-not $WhatIfPreference -and -not $SkipProjectScaffold) {
    Invoke-CssAutoDetection -OutputRoot $Output -SourcePath $Path
}

# NuGet Static Asset Extraction: Extract CSS/JS/fonts from NuGet packages to wwwroot/lib/
if (-not $WhatIfPreference -and -not $SkipProjectScaffold) {
    $nugetAssetScript = Join-Path $PSScriptRoot 'Migrate-NugetStaticAssets.ps1'
    if (Test-Path $nugetAssetScript) {
        & $nugetAssetScript -SourcePath $Path -OutputPath $Output
    }
}

# Fix 1 (Run 11): Auto-detect JS files in Scripts/ and inject <script> tags into App.razor
if (-not $WhatIfPreference -and -not $SkipProjectScaffold) {
    Invoke-ScriptAutoDetection -OutputRoot $Output -SourcePath $Path
}

#endregion

#region --- Models Copy & DbContext Transform (RF-03 / RF-04) ---

$modelsDir = Join-Path $Path 'Models'
$modelsCopied = 0
if (Test-Path $modelsDir -PathType Container) {
    $modelsOutDir = Join-Path $Output 'Models'

    # --- EDMX Detection & EF Core Generation ---
    $edmxFiles = @(Get-ChildItem -Path $modelsDir -Filter '*.edmx' -File)
    $edmxGeneratedFiles = @()
    $edmxSkipNames = @()

    if ($edmxFiles.Count -gt 0) {
        foreach ($edmxFile in $edmxFiles) {
            Write-Host "Detected EDMX: $($edmxFile.Name) — generating EF Core entities..." -ForegroundColor Cyan

            if (-not (Test-Path $modelsOutDir)) {
                New-Item -ItemType Directory -Path $modelsOutDir -Force | Out-Null
            }

            $edmxResult = & "$PSScriptRoot/Convert-EdmxToEfCore.ps1" -EdmxPath $edmxFile.FullName -OutputPath $modelsOutDir -Namespace "$projectName.Models"

            Write-TransformLog -File "Models/$($edmxFile.Name)" -Transform 'EDMX' -Detail "Parsed EDMX and generated $($edmxResult.EntitiesGenerated) EF Core entities + DbContext"

            if ($edmxResult.Warnings.Count -gt 0) {
                foreach ($w in $edmxResult.Warnings) {
                    Write-ManualItem -File "Models/$($edmxFile.Name)" -Category 'EDMX' -Detail $w
                }
            }

            # Track generated files so the .cs copy loop skips them
            $edmxGeneratedFiles += Get-ChildItem -Path $modelsOutDir -Filter '*.cs' -File | Select-Object -ExpandProperty Name

            # Build skip list for EDMX artifacts: *.Designer.cs and the T4 bootstrap (EdmxStem.cs)
            $edmxStem = [System.IO.Path]::GetFileNameWithoutExtension($edmxFile.Name)
            $edmxSkipNames += "$edmxStem.cs"
        }
    }

    # --- Copy .cs files (skip EDMX artifacts and already-generated files) ---
    $modelFiles = Get-ChildItem -Path $modelsDir -Filter '*.cs' -File
    if ($modelFiles.Count -gt 0) {
        Write-Host "Copying $($modelFiles.Count) model file(s) from Models/..." -ForegroundColor Green

        foreach ($mf in $modelFiles) {
            # Skip EDMX artifacts: *.Designer.cs, T4 bootstrap (e.g., Model1.cs)
            if ($mf.Name -like '*.Designer.cs' -or $mf.Name -in $edmxSkipNames) {
                Write-TransformLog -File "Models/$($mf.Name)" -Transform 'EDMXSkip' -Detail "Skipped EDMX artifact: $($mf.Name)"
                continue
            }

            # Skip files already generated by the EDMX converter
            if ($mf.Name -in $edmxGeneratedFiles) {
                Write-TransformLog -File "Models/$($mf.Name)" -Transform 'EDMXSkip' -Detail "Skipped — replaced by EDMX-generated version: $($mf.Name)"
                continue
            }

            $destFile = Join-Path $modelsOutDir $mf.Name
            $relPath = "Models/$($mf.Name)"

            if ($PSCmdlet.ShouldProcess($destFile, "Copy model file")) {
                if (-not (Test-Path $modelsOutDir)) {
                    New-Item -ItemType Directory -Path $modelsOutDir -Force | Out-Null
                }

                $csContent = Get-Content -Path $mf.FullName -Raw -Encoding UTF8

                # RF-04: Check if this is a DbContext file
                $isDbContext = $mf.Name -like '*Context.cs'

                if ($isDbContext) {
                    # RF-04: Transform DbContext for EF Core
                    $csContent = $csContent -replace 'using System\.Data\.Entity;', 'using Microsoft.EntityFrameworkCore; // TODO: Verify EF Core using directive'
                    $csContent = $csContent -replace 'using System\.Data\.Entity\.ModelConfiguration\.Conventions;\s*\r?\n?', ''
                    $csContent = $csContent -replace 'using System\.Web(\.\w+)*;\s*\r?\n?', ''
                    $csContent = $csContent -replace 'using Microsoft\.AspNet(\.\w+)*;\s*\r?\n?', ''
                    $csContent = $csContent -replace 'using Microsoft\.Owin(\.\w+)*;\s*\r?\n?', ''
                    $csContent = $csContent -replace 'using Owin;\s*\r?\n?', ''

                    # Extract class name
                    $ctxClassName = $null
                    if ($csContent -match 'class\s+(\w+)') {
                        $ctxClassName = $Matches[1]
                    }

                    # Remove constructors with string connectionName parameter
                    $ctorRegex = [regex]'(?s)\s*public\s+\w+\s*\(\s*string\s+\w+\s*\)\s*:\s*base\s*\([^)]*\)\s*\{[^}]*\}'
                    $csContent = $ctorRegex.Replace($csContent, '')

                    # Also remove simpler string constructors without base call
                    $simpleCtorRegex = [regex]'(?s)\s*public\s+\w+\s*\(\s*string\s+\w+\s*\)\s*\{[^}]*\}'
                    $csContent = $simpleCtorRegex.Replace($csContent, '')

                    # Remove parameterless constructors that pass connection name to base
                    $paramlessCtorRegex = [regex]'(?s)\s*public\s+\w+\s*\(\s*\)\s*:\s*base\s*\("[^"]*"\)\s*\{[^}]*\}'
                    $csContent = $paramlessCtorRegex.Replace($csContent, '')

                    # Add EF Core constructor after class opening brace
                    if ($ctxClassName) {
                        $classOpenRegex = [regex]('(class\s+' + [regex]::Escape($ctxClassName) + '[^{]*\{)')
                        $newCtor = "`n    // TODO: EF Core constructor — auto-generated by migration script`n    public ${ctxClassName}(DbContextOptions<${ctxClassName}> options) : base(options) { }`n"
                        $csContent = $classOpenRegex.Replace($csContent, "`$1$newCtor", 1)
                    }

                    $todoHeader = "// TODO: Review — auto-copied and transformed DbContext from Web Forms source`n// TODO: Verify EF Core configuration and connection string setup`n`n"
                    $csContent = $todoHeader + $csContent

                    Write-TransformLog -File $relPath -Transform 'DbContext' -Detail "Transformed DbContext for EF Core: $($mf.Name)"
                    Write-ManualItem -File $relPath -Category 'DbContext' -Detail 'DbContext auto-transformed — verify EF Core configuration'
                }
                else {
                    # RF-03: Standard model file — strip legacy usings and add header
                    $csContent = $csContent -replace 'using System\.Data\.Entity;\s*\r?\n?', ''
                    $csContent = $csContent -replace 'using System\.Data\.Entity\.ModelConfiguration\.Conventions;\s*\r?\n?', ''
                    $csContent = $csContent -replace 'using System\.Web(\.\w+)*;\s*\r?\n?', ''
                    $csContent = $csContent -replace 'using Microsoft\.AspNet(\.\w+)*;\s*\r?\n?', ''
                    $csContent = $csContent -replace 'using Microsoft\.Owin(\.\w+)*;\s*\r?\n?', ''
                    $csContent = $csContent -replace 'using Owin;\s*\r?\n?', ''
                    $csContent = "// TODO: Review — auto-copied from Web Forms source`n`n" + $csContent
                }

                Set-Content -Path $destFile -Value $csContent -Encoding UTF8
                Write-TransformLog -File $relPath -Transform 'ModelCopy' -Detail "Copied model file → $destFile"
                $modelsCopied++
            }
            else {
                Write-Host "[WhatIf] Would copy model: $relPath"
            }
        }
        Write-Host ''
    }
}

#endregion

#region --- Business Logic Directory Copy (BLL, Logic, etc.) ---

$bllDirNames = @('BLL', 'BusinessLogic', 'Logic', 'Services')
$bllCopied = 0
foreach ($dirName in $bllDirNames) {
    $bllDir = Join-Path $Path $dirName
    if (Test-Path $bllDir -PathType Container) {
        $bllFiles = Get-ChildItem -Path $bllDir -Filter '*.cs' -File -Recurse
        if ($bllFiles.Count -gt 0) {
            Write-Host "Copying $($bllFiles.Count) business logic file(s) from $dirName/..." -ForegroundColor Green
            foreach ($bf in $bllFiles) {
                $relBllPath = $bf.FullName.Substring($bllDir.Length).TrimStart('\', '/')
                $destFile = Join-Path (Join-Path $Output $dirName) $relBllPath

                if ($PSCmdlet.ShouldProcess($destFile, "Copy business logic file")) {
                    $destDir = Split-Path $destFile -Parent
                    if (-not (Test-Path $destDir)) {
                        New-Item -ItemType Directory -Path $destDir -Force | Out-Null
                    }
                    $csContent = Get-Content -Path $bf.FullName -Raw -Encoding UTF8
                    $csContent = $csContent -replace 'using System\.Web(\.\w+)*;\s*\r?\n?', ''
                    $csContent = $csContent -replace 'using Microsoft\.AspNet(\.\w+)*;\s*\r?\n?', ''
                    $csContent = $csContent -replace 'using Microsoft\.Owin(\.\w+)*;\s*\r?\n?', ''
                    $csContent = $csContent -replace 'using Owin;\s*\r?\n?', ''
                    $csContent = "// TODO: Review — auto-copied business logic from Web Forms source`n`n" + $csContent
                    Set-Content -Path $destFile -Value $csContent -Encoding UTF8
                    Write-TransformLog -File "$dirName/$relBllPath" -Transform 'BLLCopy' -Detail "Copied business logic file → $destFile"
                    $bllCopied++
                }
                else {
                    Write-Host "[WhatIf] Would copy business logic: $dirName/$relBllPath"
                }
            }
            Write-Host ''
        }
    }
}

#endregion

#region --- App_Start Directory Copy (GAP-22) ---

$appStartCopied = 0
if (-not $WhatIfPreference) {
    $appStartCopied = Copy-AppStart -SourcePath $Path -OutputRoot $Output
    if ($appStartCopied -gt 0) {
        Write-Host ''
    }
}
else {
    $appStartDir = Join-Path $Path 'App_Start'
    if (Test-Path $appStartDir -PathType Container) {
        $appStartFiles = @(Get-ChildItem -Path $appStartDir -Filter '*.cs' -File)
        if ($appStartFiles.Count -gt 0) {
            Write-Host "[WhatIf] Would copy $($appStartFiles.Count) App_Start file(s)" -ForegroundColor Yellow
        }
    }
}

#endregion

#region --- Redirect Handler Program.cs Annotations (RF-08) ---

if ($script:RedirectHandlers.Count -gt 0 -and -not $WhatIfPreference) {
    $programPath = Join-Path $Output 'Program.cs'
    if (Test-Path $programPath) {
        $programContent = Get-Content -Path $programPath -Raw -Encoding UTF8
        $redirectComments = "`n// --- Redirect Handler Pages (convert to minimal API endpoints) ---"
        foreach ($handler in $script:RedirectHandlers) {
            $redirectComments += "`n// TODO: $handler was a redirect handler — convert to minimal API endpoint"
        }
        $redirectComments += "`n"
        $programContent = $programContent.Replace('app.Run();', "${redirectComments}`napp.Run();")
        Set-Content -Path $programPath -Value $programContent -Encoding UTF8
        Write-TransformLog -File 'Program.cs' -Transform 'RedirectHandler' -Detail "Added $($script:RedirectHandlers.Count) redirect handler TODO(s) to Program.cs"
    }
}

#endregion

#region --- Post-Processing: Conditional AJAX Toolkit Import ---

# Only add @using BlazorAjaxToolkitComponents to _Imports.razor if AJAX controls were detected
if (-not $WhatIfPreference -and -not $SkipProjectScaffold -and $script:HasAjaxToolkitControls) {
    $importsPath = Join-Path $Output "_Imports.razor"
    if (Test-Path $importsPath) {
        $importsContent = Get-Content -Path $importsPath -Raw
        $ajaxLine = "@using BlazorAjaxToolkitComponents"
        if ($importsContent -notmatch 'BlazorAjaxToolkitComponents') {
            # Insert after BlazorWebFormsComponents.LoginControls
            $importsContent = $importsContent -replace '(@using BlazorWebFormsComponents\.LoginControls)', "`$1`n$ajaxLine"
            Set-Content -Path $importsPath -Value $importsContent -Encoding UTF8
            Write-TransformLog -File '_Imports.razor' -Transform 'AjaxToolkit' -Detail 'Added @using BlazorAjaxToolkitComponents (AJAX Toolkit controls detected)'
        }
    }
}

#endregion

#region --- Summary ---

Write-Host '============================================================' -ForegroundColor Cyan
Write-Host '  Migration Summary' -ForegroundColor Cyan
Write-Host '============================================================' -ForegroundColor Cyan
Write-Host "  Files processed:       $($script:FilesProcessed)"
Write-Host "  Transforms applied:    $($script:TransformsApplied)"
Write-Host "  Static files copied:   $staticCount"
Write-Host "  Model files copied:    $modelsCopied"
Write-Host "  BLL files copied:      $bllCopied"
Write-Host "  App_Start files copied: $appStartCopied"
Write-Host "  Items needing review:  $($script:ManualItems.Count)"
Write-Host ''

if ($script:ManualItems.Count -gt 0) {
    Write-Host '--- Items Needing Manual Attention ---' -ForegroundColor Yellow
    $grouped = $script:ManualItems | Group-Object -Property Category
    foreach ($group in $grouped) {
        Write-Host "  [$($group.Name)] ($($group.Count) item(s)):" -ForegroundColor Yellow
        foreach ($item in $group.Group) {
            Write-Host "    • $($item.File): $($item.Detail)"
        }
    }
    Write-Host ''
}

if ($VerbosePreference -eq 'Continue' -and $script:TransformLog.Count -gt 0) {
    Write-Host '--- Detailed Transform Log ---' -ForegroundColor DarkGray
    $groupedByFile = $script:TransformLog | Group-Object -Property File
    foreach ($fileGroup in $groupedByFile) {
        Write-Host "  $($fileGroup.Name):" -ForegroundColor DarkGray
        foreach ($entry in $fileGroup.Group) {
            Write-Host "    [$($entry.Transform)] $($entry.Detail)" -ForegroundColor DarkGray
        }
    }
    Write-Host ''
}

if (-not $WhatIfPreference) {
    Write-Host 'Migration complete. Next steps:' -ForegroundColor Green
    Write-Host '  1. Review items flagged above for manual attention'
    Write-Host '  2. Use the BWFC Copilot skill for code-behind transforms (Layer 2)'
    Write-Host '  3. Build and test: dotnet build && dotnet run'
}
else {
    Write-Host 'Dry run complete. Run without -WhatIf to apply transforms.' -ForegroundColor Yellow
}

Write-Host ''

#endregion
