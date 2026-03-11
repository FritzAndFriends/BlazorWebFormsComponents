<#
.SYNOPSIS
    Performs mechanical regex-based transforms on ASP.NET Web Forms files to produce Blazor-ready output.

.DESCRIPTION
    bwfc-migrate.ps1 is Layer 1 of the three-layer Web Forms to Blazor migration pipeline.
    It automates ~40% of the migration by applying safe, mechanical regex transforms:

      - Renames .aspx/.ascx/.master files to .razor
      - Strips Web Forms directives and converts to Razor equivalents
      - Removes asp: prefixes and runat="server" attributes
      - Converts Web Forms expressions to Razor syntax
      - Transforms URL references from ~/ to /
      - Cleans up Web Forms-specific attributes
      - Copies code-behind files with TODO annotations

    Semantic transforms (lifecycle methods, data binding logic, event handlers)
    are NOT handled here — those require the Copilot skill layer (Layer 2).

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

.EXAMPLE
    .\bwfc-migrate.ps1 -Path .\LegacyApp -Output .\BlazorApp -SkipProjectScaffold -Verbose

    Transforms files with detailed logging, skipping project scaffold generation.
#>

[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter(Mandatory = $true, HelpMessage = "Path to the Web Forms project root")]
    [string]$Path,

    [Parameter(Mandatory = $true, HelpMessage = "Path for the Blazor output project")]
    [string]$Output,

    [Parameter(HelpMessage = "Skip creating .csproj, Program.cs, and _Imports.razor")]
    [switch]$SkipProjectScaffold
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

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

#region --- Logging & Tracking ---

$script:TransformLog = [System.Collections.Generic.List[PSCustomObject]]::new()
$script:ManualItems = [System.Collections.Generic.List[PSCustomObject]]::new()
$script:RedirectHandlers = [System.Collections.Generic.List[string]]::new()
$script:FilesProcessed = 0
$script:TransformsApplied = 0

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

    # RF-06: Build conditional package references
    $additionalPackages = ''
    if ($hasModels) {
        $additionalPackages += "`n    <PackageReference Include=`"Microsoft.EntityFrameworkCore.Sqlite`" Version=`"10.0.0`" />"
        $additionalPackages += "`n    <PackageReference Include=`"Microsoft.EntityFrameworkCore.Tools`" Version=`"10.0.0`" />"
    }
    if ($hasIdentity) {
        $additionalPackages += "`n    <PackageReference Include=`"Microsoft.AspNetCore.Identity.EntityFrameworkCore`" Version=`"10.0.0`" />"
        $additionalPackages += "`n    <PackageReference Include=`"Microsoft.AspNetCore.Identity.UI`" Version=`"10.0.0`" />"
        $additionalPackages += "`n    <PackageReference Include=`"Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore`" Version=`"10.0.0`" />"
    }

    # .csproj
    $csprojContent = @"
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Fritz.BlazorWebFormsComponents" Version="*" />${additionalPackages}
  </ItemGroup>

</Project>
"@

    # _Imports.razor
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
@using static Microsoft.AspNetCore.Components.Web.RenderMode
@using $ProjectName
"@

    # Program.cs
    $programContent = @"
// TODO: Review and adjust this generated Program.cs for your application needs.
using BlazorWebFormsComponents;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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

app.MapRazorComponents<$ProjectName.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
"@

    # RF-07: Add Identity/Session boilerplate when detected
    if ($hasIdentity) {
        $identityServiceBlock = @"

// TODO: Configure database connection (use AddDbContextFactory — do NOT also register AddDbContext to avoid DI conflicts)
// builder.Services.AddDbContextFactory<ProductContext>(options => options.UseSqlite("Data Source=app.db"));

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

    # Properties/launchSettings.json
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
        $propertiesDir = Join-Path $OutputRoot "Properties"
        $launchSettingsPath = Join-Path $propertiesDir "launchSettings.json"

        Set-Content -Path $csprojPath -Value $csprojContent -Encoding UTF8
        Write-TransformLog -File $csprojPath -Transform 'Scaffold' -Detail "Generated $ProjectName.csproj"

        Set-Content -Path $importsPath -Value $importsContent -Encoding UTF8
        Write-TransformLog -File $importsPath -Transform 'Scaffold' -Detail 'Generated _Imports.razor'

        Set-Content -Path $programPath -Value $programContent -Encoding UTF8
        Write-TransformLog -File $programPath -Transform 'Scaffold' -Detail 'Generated Program.cs'

        New-Item -ItemType Directory -Force $propertiesDir | Out-Null
        Set-Content -Path $launchSettingsPath -Value $launchSettingsContent -Encoding UTF8
        Write-TransformLog -File $launchSettingsPath -Transform 'Scaffold' -Detail 'Generated Properties/launchSettings.json'
    }
    else {
        Write-Host "[WhatIf] Would create: $ProjectName.csproj"
        Write-Host "[WhatIf] Would create: _Imports.razor"
        Write-Host "[WhatIf] Would create: Program.cs"
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

@* SSR by default — add @rendermode="InteractiveServer" to pages that need interactivity *@
<body>
    <Routes />
    <script src="_framework/blazor.web.js"></script>
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

    # Inject <script> tags into App.razor before closing </body> (after blazor.web.js)
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

#region --- Directive Transforms ---

function ConvertFrom-PageDirective {
    param([string]$Content, [string]$FileName, [string]$RelPath)

    # <%@ Page ... %> → @page "/route"
    $route = '/' + [System.IO.Path]::GetFileNameWithoutExtension($FileName)
    if ($route -eq '/Default' -or $route -eq '/default' -or $route -eq '/Index' -or $route -eq '/index') {
        $route = '/'
    }

    if ($Content -match '<%@\s*Page[^%]*%>') {
        # RF-10: Extract Title attribute before stripping the directive
        $pageTitle = $null
        if ($Content -match '<%@\s*Page[^%]*Title\s*=\s*"([^"]*)"') {
            $pageTitle = $Matches[1]
        }

        $Content = $Content -replace '<%@\s*Page[^%]*%>\s*\r?\n?', ''
        $pageHeader = "@page `"$route`"`n"

        # RF-10: Add <PageTitle> if title was extracted
        if ($pageTitle) {
            $pageHeader += "<PageTitle>$pageTitle</PageTitle>`n"
            Write-TransformLog -File $RelPath -Transform 'PageTitle' -Detail "Extracted Title=`"$pageTitle`" → <PageTitle>"
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

    # HeadContent placeholder → <HeadContent> / </HeadContent>
    $headOpenRegex = [regex]'<asp:Content\s+[^>]*ContentPlaceHolderID\s*=\s*"HeadContent"[^>]*>'
    $headOpenRegex2 = [regex]'<asp:Content\s+[^>]*ContentPlaceHolderID\s*=\s*"head"[^>]*>'
    if ($Content -match $headOpenRegex -or $Content -match $headOpenRegex2) {
        $Content = $headOpenRegex.Replace($Content, '<HeadContent>')
        $Content = $headOpenRegex2.Replace($Content, '<HeadContent>')
        Write-TransformLog -File $RelPath -Transform 'Content' -Detail 'HeadContent placeholder → <HeadContent>'
    }

    # MainContent / other ContentPlaceHolderIDs → strip wrapper entirely
    $mainRegex = [regex]'<asp:Content\s+[^>]*ContentPlaceHolderID\s*=\s*"[^"]*"[^>]*>\s*\r?\n?'
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

    return $Content
}

function ConvertFrom-FormWrapper {
    param([string]$Content, [string]$RelPath)

    # Remove <form ... runat="server" ...> and its closing </form> — but only the server form
    $formOpenRegex = [regex]'<form\s+[^>]*runat\s*=\s*"server"[^>]*>\s*\r?\n?'
    if ($Content -match $formOpenRegex) {
        $Content = $formOpenRegex.Replace($Content, '', 1)
        # Remove the corresponding closing </form> (the last one, or the first one after removal)
        # Simple approach: remove one </form> tag
        $formCloseRegex = [regex]'</form>\s*\r?\n?'
        $Content = $formCloseRegex.Replace($Content, '', 1)
        Write-TransformLog -File $RelPath -Transform 'Form' -Detail 'Removed <form runat="server"> and </form>'
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
            $extractedTags.Add("    " + $m.Value.Trim())
        }
        foreach ($m in ([regex]'<link\s[^>]*>').Matches($headInner)) {
            $extractedTags.Add("    " + $m.Value.Trim())
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
        $cdnLinkRegex = [regex]'<(?:link|script)\s[^>]*(?:cdn\.|cloudflare|bootstrapcdn|googleapis|jsdelivr|unpkg|cdnjs)[^>]*>'
        foreach ($m in $cdnLinkRegex.Matches($headInner)) {
            $tag = $m.Value.Trim()
            # Skip if already captured as a <link> tag
            if ($tag -match '^<link' -and $extractedTags -contains ("    " + $tag)) { continue }
            $extractedTags.Add("    " + $tag)
            Write-TransformLog -File $RelPath -Transform 'MasterPage' -Detail "Preserved CDN reference: $($tag.Substring(0, [Math]::Min(80, $tag.Length)))"
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

    # 4. Replace <asp:ContentPlaceHolder ID="MainContent"> → @Body
    $mainCphRegex = [regex]'(?si)<asp:ContentPlaceHolder\s+[^>]*ID\s*=\s*"MainContent"[^>]*>.*?</asp:ContentPlaceHolder>'
    if ($mainCphRegex.IsMatch($Content)) {
        $Content = $mainCphRegex.Replace($Content, '@Body')
        Write-TransformLog -File $RelPath -Transform 'MasterPage' -Detail 'ContentPlaceHolder MainContent → @Body'
    }
    # Self-closing MainContent
    $mainCphSelfRegex = [regex]'(?i)<asp:ContentPlaceHolder\s+[^>]*ID\s*=\s*"MainContent"[^>]*/>'
    if ($mainCphSelfRegex.IsMatch($Content)) {
        $Content = $mainCphSelfRegex.Replace($Content, '@Body')
        Write-TransformLog -File $RelPath -Transform 'MasterPage' -Detail 'ContentPlaceHolder MainContent → @Body (self-closing)'
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

    # 6. Inject @inherits LayoutComponentBase and HeadContent at the top
    $header = "@inherits LayoutComponentBase`n"
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

    # Inside route value arguments, convert Eval("Prop") to context.Prop
    $evalInRouteRegex = [regex]'Eval\("(\w+)"\)'
    $evalInRouteMatches = $evalInRouteRegex.Matches($Content)
    if ($evalInRouteMatches.Count -gt 0) {
        $Content = $evalInRouteRegex.Replace($Content, 'context.$1')
        Write-TransformLog -File $RelPath -Transform 'GetRouteUrl' -Detail "Converted $($evalInRouteMatches.Count) Eval() in route values to context.Property"
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

#region --- Code-Behind Handling ---

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
//   - Session/Cache access → inject IHttpContextAccessor or use DI
//   - Response.Redirect → NavigationManager.NavigateTo
//   - Event handlers (Button_Click, etc.) → convert to Blazor event callbacks
//   - Data binding (DataBind, DataSource) → component parameters or OnInitialized
//   - UpdatePanel / ScriptManager references → remove (Blazor handles updates)
//   - User controls → Blazor component references
// =============================================================================

"@

        $annotatedContent = $todoHeader + $content

        # RF-12: Convert [QueryString] and [RouteData] parameter attributes
        $qsRegex = [regex]'\[QueryString\("([^"]+)"\)\]'
        $qsMatches = $qsRegex.Matches($annotatedContent)
        if ($qsMatches.Count -gt 0) {
            $annotatedContent = $qsRegex.Replace($annotatedContent, '[SupplyParameterFromQuery(Name = "$1")]')
            Write-TransformLog -File $RelPath -Transform 'ParameterAttr' -Detail "Converted $($qsMatches.Count) [QueryString] to [SupplyParameterFromQuery]"
        }

        $rdRegex = [regex]'([ \t]*)\[RouteData\]'
        $rdMatches = $rdRegex.Matches($annotatedContent)
        if ($rdMatches.Count -gt 0) {
            # P0-2: Put TODO on a separate line ABOVE [Parameter] so it doesn't
            # swallow the rest of the line (property type/name) into a comment.
            $rdReplacement = '${1}// TODO: Verify RouteData → [Parameter] conversion — ensure @page route has matching {parameter}' + "`n" + '${1}[Parameter]'
            $annotatedContent = $rdRegex.Replace($annotatedContent, $rdReplacement)
            Write-TransformLog -File $RelPath -Transform 'ParameterAttr' -Detail "Converted $($rdMatches.Count) [RouteData] to [Parameter]"
        }

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
            $content = ConvertFrom-PageDirective -Content $content -FileName $fileName -RelPath $relativePath
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

    $content = ConvertFrom-AspPrefix -Content $content -RelPath $relativePath
    $content = Remove-WebFormsAttributes -Content $content -RelPath $relativePath
    $content = ConvertFrom-UrlReferences -Content $content -RelPath $relativePath

    # Fix 2: Convert placeholder elements inside *Template blocks to @context
    $content = Convert-TemplatePlaceholders -Content $content -RelPath $relativePath

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

# Fix 1 (Run 11): Auto-detect JS files in Scripts/ and inject <script> tags into App.razor
if (-not $WhatIfPreference -and -not $SkipProjectScaffold) {
    Invoke-ScriptAutoDetection -OutputRoot $Output -SourcePath $Path
}

#endregion

#region --- Models Copy & DbContext Transform (RF-03 / RF-04) ---

$modelsDir = Join-Path $Path 'Models'
$modelsCopied = 0
if (Test-Path $modelsDir -PathType Container) {
    $modelFiles = Get-ChildItem -Path $modelsDir -Filter '*.cs' -File
    if ($modelFiles.Count -gt 0) {
        Write-Host "Copying $($modelFiles.Count) model file(s) from Models/..." -ForegroundColor Green
        $modelsOutDir = Join-Path $Output 'Models'

        foreach ($mf in $modelFiles) {
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
                    # RF-03: Standard model file — strip EF6 usings and add header
                    $csContent = $csContent -replace 'using System\.Data\.Entity;\s*\r?\n?', ''
                    $csContent = $csContent -replace 'using System\.Data\.Entity\.ModelConfiguration\.Conventions;\s*\r?\n?', ''
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

#region --- Summary ---

Write-Host '============================================================' -ForegroundColor Cyan
Write-Host '  Migration Summary' -ForegroundColor Cyan
Write-Host '============================================================' -ForegroundColor Cyan
Write-Host "  Files processed:       $($script:FilesProcessed)"
Write-Host "  Transforms applied:    $($script:TransformsApplied)"
Write-Host "  Static files copied:   $staticCount"
Write-Host "  Model files copied:    $modelsCopied"
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
