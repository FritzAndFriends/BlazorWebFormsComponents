<#
.SYNOPSIS
    Sets up and launches the BeforeWebForms sample app under IIS Express for the HTML audit.

.DESCRIPTION
    Automates the BeforeWebForms (.NET Framework 4.8) sample app setup:
    1. Converts CodeBehind= to CodeFile= in .aspx/.ascx/.master files (dynamic compilation)
    2. Adds 'partial' keyword to Global.asax.cs if needed
    3. Restores NuGet packages and copies required DLLs to bin/
    4. Launches IIS Express

    The CodeBehind→CodeFile changes are temporary runtime changes and should NOT be committed.
    Use -Revert to undo them via git checkout.

.PARAMETER Port
    The port for IIS Express to listen on. Default: 55501.

.PARAMETER NoBrowser
    Suppress automatic browser launch.

.PARAMETER Revert
    Revert CodeBehind→CodeFile and Global.asax.cs changes (restores git state) and exit.

.EXAMPLE
    .\Setup-IISExpress.ps1
    # Full setup + launch on port 55501

.EXAMPLE
    .\Setup-IISExpress.ps1 -Port 8080 -NoBrowser
    # Launch on port 8080, no browser

.EXAMPLE
    .\Setup-IISExpress.ps1 -Revert
    # Undo temporary file changes
#>
[CmdletBinding()]
param(
    [int]$Port = 55501,
    [switch]$NoBrowser,
    [switch]$Revert
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# --- Resolve paths ---
$repoRoot = (git -C $PSScriptRoot rev-parse --show-toplevel 2>$null)
if (-not $repoRoot) {
    $repoRoot = Split-Path $PSScriptRoot -Parent
}
$repoRoot = Resolve-Path $repoRoot
$sampleDir = Join-Path $repoRoot 'samples\BeforeWebForms'
$binDir    = Join-Path $sampleDir 'bin'
$pkgDir    = Join-Path $repoRoot 'src\packages'

if (-not (Test-Path $sampleDir)) {
    Write-Error "BeforeWebForms sample not found at: $sampleDir"
    exit 1
}

# --- Helper: Write step status ---
function Write-Step {
    param([string]$Message)
    Write-Host "[Setup] $Message" -ForegroundColor Cyan
}

function Write-OK {
    param([string]$Message)
    Write-Host "  OK: $Message" -ForegroundColor Green
}

function Write-Warn {
    param([string]$Message)
    Write-Host "  WARN: $Message" -ForegroundColor Yellow
}

# ============================================================
# REVERT MODE
# ============================================================
if ($Revert) {
    Write-Step 'Reverting temporary file changes via git checkout...'
    Push-Location $repoRoot
    try {
        git checkout -- "samples/BeforeWebForms/*.aspx" `
                        "samples/BeforeWebForms/*.ascx" `
                        "samples/BeforeWebForms/*.asax" `
                        "samples/BeforeWebForms/*.master" `
                        "samples/BeforeWebForms/*.Master" `
                        "samples/BeforeWebForms/ControlSamples/**/*.aspx" `
                        "samples/BeforeWebForms/Global.asax.cs" 2>$null
        Write-OK 'File changes reverted.'
    }
    catch {
        # git checkout may warn on paths with no changes — that's fine
        Write-Warn "git checkout completed with warnings (some files may not have been modified)."
    }
    finally {
        Pop-Location
    }
    exit 0
}

# ============================================================
# STEP 0: Prerequisites
# ============================================================
Write-Step 'Checking prerequisites...'

# .NET Framework 4.8
$ndpKey = 'HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full'
if (Test-Path $ndpKey) {
    $release = (Get-ItemProperty $ndpKey).Release
    if ($release -ge 528040) {
        Write-OK '.NET Framework 4.8 detected.'
    }
    else {
        Write-Error ".NET Framework 4.8 required (registry release=$release, need >=528040)."
        exit 1
    }
}
else {
    Write-Error '.NET Framework 4.x not found in registry. Install .NET Framework 4.8.'
    exit 1
}

# IIS Express
$iisExe = $null
foreach ($candidate in @(
    "${env:ProgramFiles}\IIS Express\iisexpress.exe",
    "${env:ProgramFiles(x86)}\IIS Express\iisexpress.exe"
)) {
    if (Test-Path $candidate) {
        $iisExe = $candidate
        break
    }
}
if (-not $iisExe) {
    Write-Error 'IIS Express not found. Install IIS Express (comes with Visual Studio or as standalone).'
    exit 1
}
Write-OK "IIS Express found: $iisExe"

# ============================================================
# STEP 1: CodeBehind → CodeFile conversion
# ============================================================
Write-Step 'Converting CodeBehind= to CodeFile= in .aspx, .ascx, .asax, .master files...'

$extensions = @('*.aspx', '*.ascx', '*.asax', '*.master', '*.Master')
$converted = 0

foreach ($ext in $extensions) {
    $files = Get-ChildItem -Path $sampleDir -Filter $ext -Recurse -File
    foreach ($file in $files) {
        $content = Get-Content $file.FullName -Raw
        if ($content -match 'Code[Bb]ehind=') {
            $newContent = $content -replace 'Code[Bb]ehind=', 'CodeFile='
            Set-Content -Path $file.FullName -Value $newContent -NoNewline
            $converted++
        }
    }
}

Write-OK "$converted file(s) converted (CodeBehind/Codebehind → CodeFile)."

# ============================================================
# STEP 2: Add 'partial' to Global.asax.cs
# ============================================================
Write-Step 'Patching Global.asax.cs for dynamic compilation...'

$globalFile = Join-Path $sampleDir 'Global.asax.cs'
if (Test-Path $globalFile) {
    $globalContent = Get-Content $globalFile -Raw
    # Add partial keyword if missing
    if ($globalContent -match 'public\s+class\s+Global\s*:' -and $globalContent -notmatch 'public\s+partial\s+class\s+Global\s*:') {
        $globalContent = $globalContent -replace 'public\s+class\s+Global\s*:', 'public partial class Global :'
        Write-OK 'Added partial keyword to Global class.'
    }
    else {
        Write-OK 'Global.asax.cs already has partial keyword (or pattern not found).'
    }
    # Comment out RouteConfig/BundleConfig calls — App_Start classes can't be dynamically compiled
    # and are not needed for the HTML structure audit
    $globalContent = $globalContent -replace '(?m)^(\s*)(RouteConfig\.)', '$1// $2'
    $globalContent = $globalContent -replace '(?m)^(\s*)(BundleConfig\.)', '$1// $2'
    # Comment out using directives for unavailable assemblies
    $globalContent = $globalContent -replace '(?m)^(\s*)(using System\.Web\.Optimization;)', '$1// $2'
    $globalContent = $globalContent -replace '(?m)^(\s*)(using System\.Web\.Routing;)', '$1// $2'
    Set-Content -Path $globalFile -Value $globalContent -NoNewline
    Write-OK 'Commented out RouteConfig/BundleConfig calls and unused using directives.'
}
else {
    Write-Warn 'Global.asax.cs not found — skipping.'
}

# Patch web.config: remove System.Web.Optimization namespace and control registrations
$webConfigFile = Join-Path $sampleDir 'web.config'
if (Test-Path $webConfigFile) {
    $wcContent = Get-Content $webConfigFile -Raw
    $wcContent = $wcContent -replace '(?s)<add\s+namespace="System\.Web\.Optimization"\s*/>', '<!-- Removed for audit: System.Web.Optimization -->'
    $wcContent = $wcContent -replace '(?s)<add\s+assembly="Microsoft\.AspNet\.Web\.Optimization\.WebForms"[^/]*/>', '<!-- Removed for audit: Optimization.WebForms -->'
    Set-Content -Path $webConfigFile -Value $wcContent -NoNewline
    Write-OK 'Patched web.config: removed Optimization references.'
}

# ============================================================
# STEP 3: NuGet restore
# ============================================================
Write-Step 'Restoring NuGet packages...'

$nugetExe = Get-Command nuget.exe -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Source
if (-not $nugetExe) {
    $nugetExe = Join-Path $repoRoot 'nuget.exe'
    if (-not (Test-Path $nugetExe)) {
        Write-Step '  Downloading nuget.exe...'
        try {
            [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
            Invoke-WebRequest -Uri 'https://dist.nuget.org/win-x86-commandline/latest/nuget.exe' -OutFile $nugetExe -UseBasicParsing
            Write-OK "Downloaded nuget.exe to $nugetExe"
        }
        catch {
            Write-Error "Failed to download nuget.exe: $_"
            exit 1
        }
    }
}

Write-OK "Using nuget.exe: $nugetExe"

$packagesConfig = Join-Path $sampleDir 'packages.config'
if (-not (Test-Path $packagesConfig)) {
    Write-Error "packages.config not found at: $packagesConfig"
    exit 1
}

& $nugetExe restore $packagesConfig -PackagesDirectory $pkgDir -NonInteractive
if ($LASTEXITCODE -ne 0) {
    Write-Error 'NuGet restore failed.'
    exit 1
}
Write-OK 'NuGet packages restored.'

# ============================================================
# STEP 4: Copy DLLs to bin/
# ============================================================
Write-Step 'Copying required DLLs to bin/ ...'

if (-not (Test-Path $binDir)) {
    New-Item -ItemType Directory -Path $binDir -Force | Out-Null
}

# Build a mapping of package folder name patterns → DLL names to copy.
# We search each package's lib/net4* or lib/netstandard* for the assembly.
$packageDlls = @(
    @{ Pkg = 'Antlr';                                          Dll = 'Antlr3.Runtime.dll' }
    @{ Pkg = 'Microsoft.AspNet.FriendlyUrls.Core';              Dll = 'Microsoft.AspNet.FriendlyUrls.dll' }
    @{ Pkg = 'Microsoft.AspNet.Web.Optimization';               Dll = 'System.Web.Optimization.dll' }
    @{ Pkg = 'Microsoft.AspNet.Web.Optimization.WebForms';      Dll = 'Microsoft.AspNet.Web.Optimization.WebForms.dll' }
    @{ Pkg = 'Microsoft.CodeDom.Providers.DotNetCompilerPlatform'; Dll = 'Microsoft.CodeDom.Providers.DotNetCompilerPlatform.dll' }
    @{ Pkg = 'Microsoft.Web.Infrastructure';                    Dll = 'Microsoft.Web.Infrastructure.dll' }
    @{ Pkg = 'Newtonsoft.Json';                                 Dll = 'Newtonsoft.Json.dll' }
    @{ Pkg = 'WebGrease';                                       Dll = 'WebGrease.dll' }
    @{ Pkg = 'WebActivatorEx';                                  Dll = 'WebActivatorEx.dll' }
    @{ Pkg = 'Microsoft.AspNet.ScriptManager.MSAjax';           Dll = 'Microsoft.ScriptManager.MSAjax.dll' }
    @{ Pkg = 'Microsoft.AspNet.ScriptManager.WebForms';         Dll = 'Microsoft.ScriptManager.WebForms.dll' }
    @{ Pkg = 'AspNet.ScriptManager.bootstrap';                  Dll = 'AspNet.ScriptManager.bootstrap.dll' }
    @{ Pkg = 'AspNet.ScriptManager.jQuery';                     Dll = 'AspNet.ScriptManager.jQuery.dll' }
)

$copiedCount = 0
$missingDlls = @()

foreach ($entry in $packageDlls) {
    $pkgPattern = $entry.Pkg
    $dllName    = $entry.Dll

    # Find the package directory (case-insensitive match)
    $pkgFolder = Get-ChildItem -Path $pkgDir -Directory -ErrorAction SilentlyContinue |
        Where-Object { $_.Name -like "$pkgPattern.*" } |
        Sort-Object Name -Descending |
        Select-Object -First 1

    if (-not $pkgFolder) {
        $missingDlls += "$dllName (package $pkgPattern not found)"
        continue
    }

    # Search lib/ for the DLL, preferring net48 > net4* > netstandard*
    $dllFile = $null
    $searchPaths = @(
        (Join-Path $pkgFolder.FullName 'lib\net48'),
        (Join-Path $pkgFolder.FullName 'lib\net472'),
        (Join-Path $pkgFolder.FullName 'lib\net47'),
        (Join-Path $pkgFolder.FullName 'lib\net462'),
        (Join-Path $pkgFolder.FullName 'lib\net46'),
        (Join-Path $pkgFolder.FullName 'lib\net45'),
        (Join-Path $pkgFolder.FullName 'lib\net40'),
        (Join-Path $pkgFolder.FullName 'lib\netstandard2.0'),
        (Join-Path $pkgFolder.FullName 'lib\netstandard1.0'),
        (Join-Path $pkgFolder.FullName 'lib')
    )

    foreach ($sp in $searchPaths) {
        $candidate = Join-Path $sp $dllName
        if (Test-Path $candidate) {
            $dllFile = $candidate
            break
        }
    }

    # Some packages put DLLs in content/ or tools/ — fallback recursive search
    if (-not $dllFile) {
        $dllFile = Get-ChildItem -Path $pkgFolder.FullName -Filter $dllName -Recurse -File -ErrorAction SilentlyContinue |
            Select-Object -First 1 |
            Select-Object -ExpandProperty FullName
    }

    if ($dllFile) {
        Copy-Item -Path $dllFile -Destination $binDir -Force
        $copiedCount++
    }
    else {
        $missingDlls += "$dllName (not found in $($pkgFolder.Name))"
    }
}

# Also copy the Roslyn compiler tools for Microsoft.CodeDom.Providers.DotNetCompilerPlatform
$roslynPkgDir = Get-ChildItem -Path $pkgDir -Directory -ErrorAction SilentlyContinue |
    Where-Object { $_.Name -like 'Microsoft.CodeDom.Providers.DotNetCompilerPlatform.*' } |
    Sort-Object Name -Descending |
    Select-Object -First 1

if ($roslynPkgDir) {
    $roslynToolsDir = Join-Path $roslynPkgDir.FullName 'tools'
    if (Test-Path $roslynToolsDir) {
        $roslynBinDir = Join-Path $binDir 'roslyn'
        if (-not (Test-Path $roslynBinDir)) {
            New-Item -ItemType Directory -Path $roslynBinDir -Force | Out-Null
        }
        $roslynItems = @()
        $roslynItems += @(Get-ChildItem -Path $roslynToolsDir -Filter '*.dll' -File -ErrorAction SilentlyContinue)
        $roslynItems += @(Get-ChildItem -Path $roslynToolsDir -Filter '*.exe' -File -ErrorAction SilentlyContinue)
        $roslynItems | ForEach-Object {
            Copy-Item -Path $_.FullName -Destination $roslynBinDir -Force
        }

        # Also look in tools/RoslynLatest or tools/Roslyn* subdirectories
        $roslynSubDirs = Get-ChildItem -Path $roslynToolsDir -Directory -ErrorAction SilentlyContinue |
            Where-Object { $_.Name -like 'Roslyn*' }
        foreach ($subDir in $roslynSubDirs) {
            Get-ChildItem -Path $subDir.FullName -File -ErrorAction SilentlyContinue | ForEach-Object {
                Copy-Item -Path $_.FullName -Destination $roslynBinDir -Force
            }
        }

        Write-OK 'Roslyn compiler tools copied to bin/roslyn/.'
    }
}

Write-OK "$copiedCount DLL(s) copied to bin/."

if ($missingDlls.Count -gt 0) {
    Write-Warn "Some DLLs were not found (may be optional):"
    foreach ($m in $missingDlls) {
        Write-Host "    - $m" -ForegroundColor Yellow
    }
}

# ============================================================
# STEP 5: Launch IIS Express
# ============================================================
Write-Step "Launching IIS Express on port $Port..."
Write-Host "  Site path: $sampleDir"
Write-Host "  URL:       http://localhost:$Port/"
Write-Host ''

if (-not $NoBrowser) {
    # Start browser after a short delay to let IIS Express initialize
    Start-Job -ScriptBlock {
        Start-Sleep -Seconds 3
        Start-Process "http://localhost:$using:Port/"
    } | Out-Null
}

Write-Host 'Press Q or Ctrl+C to stop IIS Express.' -ForegroundColor Yellow
Write-Host ''

try {
    & $iisExe /path:"$sampleDir" /port:$Port
}
catch {
    Write-Error "IIS Express failed: $_"
    exit 1
}

if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}
