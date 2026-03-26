<#
.SYNOPSIS
    Extracts static assets (CSS, JS, fonts, images) from NuGet packages to wwwroot/lib/.

.DESCRIPTION
    Web Forms apps reference NuGet packages via packages.config. Those packages often
    include static assets in Content/ and Scripts/ folders. Modern .NET projects don't
    have this pattern — assets must be explicitly placed in wwwroot/.

    This script:
    1. Reads packages.config to identify installed packages
    2. Scans the packages/ folder for Content/ and Scripts/ subdirectories
    3. Copies assets to wwwroot/lib/{PackageName}/ preserving folder structure
    4. Generates asset-manifest.json for auditability
    5. Generates AssetReferences.html snippet for App.razor

    IMPORTANT: This performs exact-version extraction. No version upgrades,
    no CDN substitution. The migrated app gets the exact same files.

.PARAMETER SourcePath
    Path to the Web Forms project root (containing packages.config).

.PARAMETER PackagesPath
    Path to the packages/ folder. Defaults to {SourcePath}/../packages or {SourcePath}/packages.

.PARAMETER OutputPath
    Path to the Blazor output project root (wwwroot/lib/ will be created here).

.PARAMETER ManifestOnly
    If set, generates the manifest without copying files. Useful for dry-run analysis.

.EXAMPLE
    .\Migrate-NugetStaticAssets.ps1 -SourcePath C:\MyWebApp -OutputPath C:\MyBlazorApp

.EXAMPLE
    .\Migrate-NugetStaticAssets.ps1 -SourcePath C:\MyWebApp -OutputPath C:\MyBlazorApp -ManifestOnly
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$SourcePath,

    [Parameter()]
    [string]$PackagesPath,

    [Parameter(Mandatory)]
    [string]$OutputPath,

    [switch]$ManifestOnly
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# --- Resolve packages.config ---
$packagesConfigPath = Join-Path $SourcePath 'packages.config'
if (-not (Test-Path $packagesConfigPath)) {
    Write-Host "  No packages.config found at $packagesConfigPath — skipping NuGet asset extraction" -ForegroundColor Yellow
    return
}

# --- Resolve packages/ folder ---
if (-not $PackagesPath) {
    # Try sibling of source (solution-level packages/)
    $candidate = Join-Path (Split-Path $SourcePath -Parent) 'packages'
    if (Test-Path $candidate -PathType Container) {
        $PackagesPath = $candidate
    }
    else {
        # Try inside source
        $candidate = Join-Path $SourcePath 'packages'
        if (Test-Path $candidate -PathType Container) {
            $PackagesPath = $candidate
        }
    }
}

if (-not $PackagesPath -or -not (Test-Path $PackagesPath -PathType Container)) {
    Write-Host "  packages/ folder not found — skipping NuGet asset extraction" -ForegroundColor Yellow
    Write-Host "  Searched: $(Split-Path $SourcePath -Parent)\packages and $SourcePath\packages" -ForegroundColor DarkYellow
    return
}

Write-Host "`n📦 NuGet Static Asset Extraction" -ForegroundColor Cyan
Write-Host "  Source:   $packagesConfigPath" -ForegroundColor DarkGray
Write-Host "  Packages: $PackagesPath" -ForegroundColor DarkGray
Write-Host "  Output:   $OutputPath" -ForegroundColor DarkGray

# --- Parse packages.config ---
[xml]$packagesXml = Get-Content -Path $packagesConfigPath -Raw
$packages = @()
foreach ($pkg in $packagesXml.packages.package) {
    $packages += [PSCustomObject]@{
        Id      = [string]$pkg.id
        Version = [string]$pkg.version
    }
}
Write-Host "  Found $($packages.Count) package(s) in packages.config" -ForegroundColor DarkGray

# --- Known packages that NEVER have useful static assets (build tools, runtime, etc.) ---
$skipPrefixes = @(
    'Microsoft.AspNet.Identity'
    'Microsoft.AspNet.Providers'
    'Microsoft.AspNet.ScriptManager'
    'Microsoft.AspNet.Web.Optimization'
    'Microsoft.AspNet.FriendlyUrls'
    'Microsoft.Owin'
    'Microsoft.Web.Infrastructure'
    'Microsoft.CodeDom'
    'Microsoft.Net.Compilers'
    'Microsoft.ApplicationInsights'
    'EntityFramework'
    'Newtonsoft.Json'
    'Owin'
    'Antlr'
    'WebGrease'
    'elmah'
    'AspNet.ScriptManager'
)

# --- Asset file extensions we care about ---
$assetExtensions = @('.css', '.js', '.map', '.woff', '.woff2', '.ttf', '.eot', '.svg', '.png', '.jpg', '.gif', '.ico')

# --- Scan each package for static assets ---
$manifest = [System.Collections.ArrayList]::new()
$totalFilesCopied = 0
$totalPackagesWithAssets = 0

foreach ($pkg in $packages) {
    # Check if this is a known non-asset package
    $skip = $false
    foreach ($prefix in $skipPrefixes) {
        if ($pkg.Id -eq $prefix -or $pkg.Id.StartsWith("$prefix.")) {
            $skip = $true
            break
        }
    }

    # Build expected package folder name: {Id}.{Version}
    $pkgFolderName = "$($pkg.Id).$($pkg.Version)"
    $pkgFolder = Join-Path $PackagesPath $pkgFolderName

    if (-not (Test-Path $pkgFolder -PathType Container)) {
        # Try case-insensitive match
        $match = Get-ChildItem -Path $PackagesPath -Directory -ErrorAction SilentlyContinue |
            Where-Object { $_.Name -ieq $pkgFolderName } |
            Select-Object -First 1
        if ($match) {
            $pkgFolder = $match.FullName
        }
        else {
            $null = $manifest.Add([PSCustomObject]@{
                PackageId = $pkg.Id
                Version   = $pkg.Version
                Status    = 'not-found'
                Reason    = "Package folder not found in packages/"
                Files     = @()
            })
            continue
        }
    }
    else {
        $pkgFolder = (Resolve-Path $pkgFolder).Path
    }

    if ($skip) {
        $null = $manifest.Add([PSCustomObject]@{
            PackageId = $pkg.Id
            Version   = $pkg.Version
            Status    = 'skipped'
            Reason    = "Build/runtime dependency — no static assets expected"
            Files     = @()
        })
        continue
    }

    # Scan for Content/ and Scripts/ directories
    $assetFiles = [System.Collections.ArrayList]::new()
    $searchDirs = @('Content', 'Scripts', 'lib', 'dist')
    $visitedDirs = @{}

    foreach ($subDir in $searchDirs) {
        $assetDir = Join-Path $pkgFolder $subDir
        if (Test-Path $assetDir -PathType Container) {
            $assetDir = (Resolve-Path $assetDir).Path
            # Skip if we already visited this directory (case-insensitive match on Windows)
            if ($visitedDirs.ContainsKey($assetDir.ToLower())) { continue }
            $visitedDirs[$assetDir.ToLower()] = $true
            $files = Get-ChildItem -Path $assetDir -File -Recurse -ErrorAction SilentlyContinue |
                Where-Object { $assetExtensions -contains $_.Extension.ToLower() }

            foreach ($file in $files) {
                # Skip IntelliSense files and WebForms-specific scripts
                if ($file.Name -like '*intellisense*') { continue }
                if ($file.Name -like '*-vsdoc*') { continue }
                if ($file.Name -eq '_references.js') { continue }
                if ($file.FullName -like '*\WebForms\*') { continue }
                if ($file.Name -like '*.min.map') { continue }

                $relativePath = $file.FullName.Substring($assetDir.Length).TrimStart('\', '/')
                # Strip redundant container folders (NuGet Content/ contains Content/, Scripts/ mirroring project)
                # e.g., Content/bootstrap.css → bootstrap.css, Scripts/jquery.js → jquery.js
                if ($relativePath -match '^(?:Content|Scripts)[/\\](.+)$') {
                    $relativePath = $Matches[1]
                }
                $null = $assetFiles.Add([PSCustomObject]@{
                    SourceFile   = $file.FullName
                    RelativePath = $relativePath
                    SubDir       = $subDir
                    Extension    = $file.Extension.ToLower()
                    SizeBytes    = $file.Length
                })
            }
        }
    }

    if ($assetFiles.Count -eq 0) {
        $null = $manifest.Add([PSCustomObject]@{
            PackageId = $pkg.Id
            Version   = $pkg.Version
            Status    = 'no-assets'
            Reason    = "No static asset files found in Content/, Scripts/, lib/, or dist/"
            Files     = @()
        })
        continue
    }

    # --- Copy assets to wwwroot/lib/{PackageId}/ ---
    $destBase = Join-Path $OutputPath 'wwwroot' 'lib' $pkg.Id
    $copiedFiles = [System.Collections.ArrayList]::new()

    foreach ($asset in $assetFiles) {
        $destFile = Join-Path $destBase $asset.RelativePath

        if (-not $ManifestOnly) {
            $destDir = Split-Path $destFile -Parent
            if (-not (Test-Path $destDir)) {
                New-Item -ItemType Directory -Path $destDir -Force | Out-Null
            }
            Copy-Item -Path $asset.SourceFile -Destination $destFile -Force
        }
        $relPath = ("lib/$($pkg.Id)/$($asset.RelativePath)") -replace '\\', '/'
        $null = $copiedFiles.Add($relPath)
    }

    $totalFilesCopied += $copiedFiles.Count
    $totalPackagesWithAssets++

    $null = $manifest.Add([PSCustomObject]@{
        PackageId = $pkg.Id
        Version   = $pkg.Version
        Status    = if ($ManifestOnly) { 'analyzed' } else { 'extracted' }
        Reason    = "$($copiedFiles.Count) asset file(s) extracted to wwwroot/lib/$($pkg.Id)/"
        Files     = $copiedFiles.ToArray()
    })

    $verb = if ($ManifestOnly) { 'Found' } else { 'Extracted' }
    Write-Host "  ✓ $($pkg.Id) $($pkg.Version): $verb $($copiedFiles.Count) file(s)" -ForegroundColor Green
}

# --- Generate asset-manifest.json ---
$manifestOutput = [PSCustomObject]@{
    timestamp          = (Get-Date -Format 'o')
    sourceProject      = $SourcePath
    packagesFolder     = $PackagesPath
    outputProject      = $OutputPath
    totalPackages      = $packages.Count
    packagesWithAssets = $totalPackagesWithAssets
    totalFilesExtracted = $totalFilesCopied
    manifestOnly       = [bool]$ManifestOnly
    packages           = $manifest.ToArray()
}

$manifestPath = Join-Path $OutputPath 'asset-manifest.json'
$manifestOutput | ConvertTo-Json -Depth 4 | Set-Content -Path $manifestPath -Encoding UTF8
Write-Host "  📋 Wrote asset-manifest.json ($($manifest.Count) packages cataloged)" -ForegroundColor Cyan

# --- Generate AssetReferences.html snippet ---
$cssRefs = [System.Collections.ArrayList]::new()
$jsRefs = [System.Collections.ArrayList]::new()

foreach ($entry in $manifest) {
    if ($entry.Status -notin @('extracted', 'analyzed')) { continue }
    foreach ($file in $entry.Files) {
        $ext = [System.IO.Path]::GetExtension($file).ToLower()
        # Skip .min versions if non-min exists (prefer min for production)
        if ($ext -eq '.css') {
            $null = $cssRefs.Add("    <link rel=""stylesheet"" href=""/$file"" />")
        }
        elseif ($ext -eq '.js') {
            $null = $jsRefs.Add("    <script src=""/$file""></script>")
        }
    }
}

# De-duplicate: prefer .min.css over .css, .min.js over .js
$cssDeduped = [System.Collections.ArrayList]::new()
foreach ($ref in $cssRefs) {
    $minVariant = $ref -replace '\.css"', '.min.css"'
    if ($ref -notmatch '\.min\.css' -and $cssRefs -contains $minVariant) {
        continue  # Skip non-minified if minified exists
    }
    $null = $cssDeduped.Add($ref)
}

$jsDeduped = [System.Collections.ArrayList]::new()
foreach ($ref in $jsRefs) {
    $minVariant = $ref -replace '\.js"', '.min.js"'
    if ($ref -notmatch '\.min\.js' -and $jsRefs -contains $minVariant) {
        continue  # Skip non-minified if minified exists
    }
    $null = $jsDeduped.Add($ref)
}

$snippetLines = [System.Collections.ArrayList]::new()
$null = $snippetLines.Add("<!-- NuGet Static Assets — extracted by Migrate-NugetStaticAssets.ps1 -->")
$null = $snippetLines.Add("<!-- Paste CSS references into App.razor <head> section -->")
if ($cssDeduped.Count -gt 0) {
    foreach ($css in $cssDeduped) { $null = $snippetLines.Add($css) }
}
else {
    $null = $snippetLines.Add("<!-- No CSS assets found in NuGet packages -->")
}
$null = $snippetLines.Add("")
$null = $snippetLines.Add("<!-- Paste JS references before </body> in App.razor -->")
if ($jsDeduped.Count -gt 0) {
    foreach ($js in $jsDeduped) { $null = $snippetLines.Add($js) }
}
else {
    $null = $snippetLines.Add("<!-- No JS assets found in NuGet packages -->")
}

$snippetPath = Join-Path $OutputPath 'AssetReferences.html'
$snippetLines -join "`n" | Set-Content -Path $snippetPath -Encoding UTF8
Write-Host "  📄 Wrote AssetReferences.html ($($cssDeduped.Count) CSS + $($jsDeduped.Count) JS references)" -ForegroundColor Cyan

# --- Summary ---
Write-Host ""
if ($ManifestOnly) {
    Write-Host "  DRY RUN: $totalPackagesWithAssets package(s) with $totalFilesCopied asset file(s) found" -ForegroundColor Yellow
    Write-Host "  Re-run without -ManifestOnly to extract files" -ForegroundColor Yellow
}
else {
    Write-Host "  ✅ Extracted $totalFilesCopied file(s) from $totalPackagesWithAssets package(s) to wwwroot/lib/" -ForegroundColor Green
}



