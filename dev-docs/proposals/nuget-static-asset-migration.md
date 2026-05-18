# NuGet Static Asset Migration Strategy for BWFC

**Author:** Forge (Lead / Web Forms Reviewer)  
**Date:** 2026-03-08  
**Status:** Proposal  
**Related Issues:** #TBD (GitHub issue pending creation)

---

## Executive Summary

ASP.NET Web Forms applications commonly reference CSS and JavaScript via NuGet packages using `packages.config`, which auto-extracts package contents into the `/packages` folder and references them via `BundleConfig.cs` or direct `<link>` / `<script>` tags. Blazor Server applications use `wwwroot/` for static assets and have no built-in bundling configuration.

This document proposes a **multi-strategy approach** for migrating static assets from NuGet packages to Blazor applications, with a recommendation to implement **Option C (NuGet Extraction Tool)** as a `bwfc migrate-assets` migration-toolkit command that intelligently handles common packages, maps known CDN alternatives, and automates wwwroot deployment.

---

## Problem Analysis: How Web Forms Apps Reference Static Assets

### The Web Forms Flow (DepartmentPortal Example)

1. **packages.config** declares NuGet dependencies:
   ```xml
   <package id="Microsoft.CodeDom.Providers.DotNetCompilerPlatform" version="2.0.1" targetFramework="net48" />
   ```

2. **NuGet Package Restore** (implicit):
   - Extracts all `.nupkg` files to `packages/` folder
   - Common patterns:
     - `packages/jQuery.3.6.0/Content/Scripts/jquery-3.6.0.js`
     - `packages/Bootstrap.4.6.0/Content/bootstrap.css`
     - `packages/Modernizr.2.8.3/Scripts/modernizr-2.8.3.js`

3. **BundleConfig.cs** (if used) aggregates assets:
   ```csharp
   bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
       "~/Scripts/jquery-{version}.js"));
   
   bundles.Add(new StyleBundle("~/Content/css").Include(
       "~/Content/bootstrap.css",
       "~/Content/site.css"));
   ```
   - Renders as `<script src="/bundles/jquery?v=xyz">` in _Layout.cshtml
   - Bundles minify and cache-bust automatically

4. **Content/Scripts auto-copy** (convention over configuration):
   - Project files may include `<Content>` items from NuGet-extracted packages
   - Some custom apps manually copy NuGet assets to `Content/` or `Scripts/` folders

5. **Manual `<link>` / `<script>` tags** (DepartmentPortal pattern):
   - If no BundleConfig, web forms projects reference assets directly:
   ```html
   <link rel="stylesheet" href="~/Content/Site.css" />
   <script src="~/Scripts/jquery.js"></script>
   ```
   - Site.css is a custom app stylesheet in `/Content` folder (DepartmentPortal example)

### Current DepartmentPortal State

**Original (Web Forms):**
- `packages.config`: Only `Microsoft.CodeDom.Providers.DotNetCompilerPlatform` (build tool, no static assets)
- `Content/Site.css`: Custom app stylesheet (~291 lines, UI framework styles)
- No `BundleConfig.cs` found (manual linking pattern)
- No `Scripts/` folder with external libraries

**Migrated (Blazor):**
- `AfterDepartmentPortal/wwwroot/css/site.css`: Custom stylesheet copied
- No build-time bundling configuration needed (Blazor uses standard static file serving)

### Key Insight

DepartmentPortal is a **minimal NuGet scenario** — only custom CSS, no jQuery/Bootstrap/other OSS libraries. However, enterprise Web Forms apps typically have 5–15 NuGet packages with extensive CSS/JS assets. The problem scales rapidly:
- **Scenario A (DepartmentPortal):** 1 custom CSS file → simple `wwwroot/css/` copy
- **Scenario B (typical enterprise):** 10–20 NuGet packages (jQuery, Bootstrap, DataTables, SignalR, etc.) + custom CSS/JS
- **Scenario C (legacy monolith):** 50+ packages, custom BundleConfig, dynamically loaded assets

---

## Detection: Identifying NuGet Packages with Static Assets

### Strategy 1: Analyze packages.config

```powershell
# Read packages.config and extract package IDs + versions
[xml]$config = Get-Content packages.config
$packages = $config.packages.package | Select-Object @{N='Id';E={$_.id}}, @{N='Version';E={$_.version}}

# Cross-reference with known CDN / npm equivalents
$knownPackages = @{
    'jQuery' = @{ cdnUrl = 'https://code.jquery.com/jquery-3.6.0.min.js'; npmName = 'jquery' }
    'Bootstrap' = @{ cdnUrl = 'https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/bootstrap.min.css'; npmName = 'bootstrap' }
    'Modernizr' = @{ cdnUrl = 'https://cdnjs.cloudflare.com/ajax/libs/modernizr/2.8.3/modernizr.min.js'; npmName = 'modernizr' }
}
```

### Strategy 2: Inspect packages/ Folder

```powershell
# Find packages with Content/ or Scripts/ folders
Get-ChildItem packages/ -Directory | ForEach-Object {
    $contentPath = Join-Path $_.FullName "Content"
    $scriptsPath = Join-Path $_.FullName "Scripts"
    
    if ((Test-Path $contentPath) -or (Test-Path $scriptsPath)) {
        Write-Host "$($_.Name) has static assets"
    }
}
```

### Strategy 3: Scan BundleConfig.cs

```powershell
# Extract bundle definitions
if (Test-Path App_Start/BundleConfig.cs) {
    $bundleConfig = Get-Content App_Start/BundleConfig.cs
    # Regex to find bundle.Include("~/path/file.js")
    $files = [regex]::Matches($bundleConfig, '\.Include\("([^"]+)"\)') | ForEach-Object { $_.Groups[1].Value }
}
```

### Detection Output Example

```
NuGet Package Analysis Results:
- jQuery.3.6.0 → Content/Scripts/jquery-3.6.0.js (OSS, CDN available)
- Bootstrap.4.6.0 → Content/bootstrap.css, Content/bootstrap.js (OSS, CDN available)
- DataTables.1.11.0 → Content/DataTables/* (OSS, CDN available)
- MyCompany.Reports.1.0.0 → Content/reports.css, Scripts/reports.js (Custom, no CDN)
- SignalR.2.4.0 → Scripts/signalr*.js (OSS, npm available)
- Custom project assets → Content/Site.css, Content/App.css (Custom, manual)
```

---

## Extraction Strategy: Four Options

### Option A: CDN Replacement (Lowest Lift)

**Concept:** Map known NuGet packages to CDN equivalents, eliminate local files.

**Pros:**
- ✅ Minimal disk footprint
- ✅ Leverages global CDN infrastructure
- ✅ No file extraction overhead
- ✅ Industry standard (most SaaS apps)

**Cons:**
- ❌ Internet dependency (offline not supported)
- ❌ Breaking change if CDN version doesn't match NuGet version exactly
- ❌ Doesn't work for custom/private NuGet packages
- ❌ No fallback if CDN is unavailable

**Implementation Example:**
```html
<!-- Before (Web Forms + BundleConfig) -->
<script src="/bundles/jquery"></script>
<link href="/Content/bootstrap" rel="stylesheet" />

<!-- After (Blazor + CDN) -->
<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
<link href="https://stackpath.bootstrapcdn.com/bootstrap/4.6.0/bootstrap.min.css" rel="stylesheet" />
```

**When to Use:**
- Public OSS libraries only (jQuery, Bootstrap, DataTables, etc.)
- Apps with reliable internet access
- Modern cloud-first deployments
- Development teams comfortable with external CDN dependencies

---

### Option B: LibMan (Visual Studio Library Manager)

**Concept:** Use Visual Studio's `libman.json` to declaratively restore client-side libraries to `wwwroot/lib/`.

**Pros:**
- ✅ Native Visual Studio integration (UI + CLI)
- ✅ Automatic version management
- ✅ Supports CDN, npm, filesystem sources
- ✅ Build-time library resolution (offline-capable with cache)

**Cons:**
- ❌ Limited to public libraries (npm, cdnjs, unpkg)
- ❌ Custom packages still require manual handling
- ❌ Requires Visual Studio or `libman` CLI
- ❌ Learning curve for developers unfamiliar with LibMan

**Implementation Example:**

```json
// libman.json
{
  "version": "1.0",
  "defaultProvider": "unpkg",
  "libraries": [
    {
      "library": "jquery@3.6.0",
      "destination": "wwwroot/lib/jquery/",
      "files": ["dist/jquery.min.js"]
    },
    {
      "library": "bootstrap@4.6.0",
      "destination": "wwwroot/lib/bootstrap/",
      "files": ["dist/css/bootstrap.min.css", "dist/js/bootstrap.min.js"]
    }
  ]
}
```

**When to Use:**
- Teams already using Visual Studio
- Mix of OSS + custom assets
- Need offline support but small library set
- Projects transitioning from full framework to modern .NET

---

### Option C: NuGet Extraction Tool (Recommended)

**Concept:** PowerShell script that reads `packages.config`, finds `.nupkg` files, extracts `Content/` and `Scripts/` folders, places them in `wwwroot/`, and generates asset reference HTML.

**Pros:**
- ✅ Handles all NuGet packages (OSS + custom)
- ✅ Preserves exact versions from `packages.config`
- ✅ Integrated into `bwfc migrate-assets` command
- ✅ Supports intelligent mapping (known packages → CDN/npm suggestions)
- ✅ Generates Blazor-compatible asset references
- ✅ Works offline (packages already downloaded)
- ✅ Repeatable and automatable

**Cons:**
- ❌ Larger wwwroot footprint (copies all assets)
- ❌ Requires PowerShell scripting infrastructure
- ❌ Dependency on accurate `packages.config` maintenance

**Detailed Implementation:**

```powershell
# Parse packages.config
$packagesConfig = "packages.config"
$packagesDir = "packages"

# Map known NuGet packages to extraction paths
$packageMappings = @{
    'jQuery' = @{ contentPath = 'Content/Scripts'; pattern = 'jquery*.js' }
    'Bootstrap' = @{ contentPath = 'Content'; pattern = 'bootstrap*' }
    'Modernizr' = @{ contentPath = 'Scripts'; pattern = 'modernizr*.js' }
    'DataTables' = @{ contentPath = 'content'; pattern = '*.css;*.js' }
}

# Extract logic
foreach ($package in Get-ChildItem $packagesDir -Directory) {
    $pkgName = $package.Name
    $contentPath = Join-Path $package.FullName $mappings[$pkgName].contentPath
    
    if (Test-Path $contentPath) {
        # Copy to wwwroot/{lib,css,js}
        Copy-Item $contentPath -Destination "wwwroot/lib/$pkgName" -Recurse
        Write-Host "✓ Extracted $pkgName to wwwroot/lib/"
    }
}

# Generate asset references
$assets | ForEach-Object {
    Write-Host "<link rel='stylesheet' href='/_framework/lib/$($_.Name)/$($_.File)' />"
}
```

**When to Use:**
- All migration scenarios
- Enterprise apps with custom + OSS packages
- Teams needing full control and auditability
- Migration toolkit integration required

---

### Option D: npm Equivalents (Modern Approach)

**Concept:** Map NuGet packages to npm equivalents, use `package.json` + npm install, commit node_modules or use build-time tooling.

**Pros:**
- ✅ Modern JavaScript ecosystem
- ✅ Supports all public libraries
- ✅ Familiar to web developers
- ✅ Integrates with webpack, esbuild, Vite for bundling/minification
- ✅ Peer dependency resolution

**Cons:**
- ❌ Introduces Node.js/npm toolchain dependency
- ❌ npm-only packages (no custom NuGet libs)
- ❌ Requires build step (webpack/esbuild setup)
- ❌ Larger learning curve for backend-first teams
- ❌ node_modules bloat (if committed) or build-time complexity (if generated)

**Implementation Example:**

```json
// package.json
{
  "name": "departmentportal",
  "version": "1.0.0",
  "dependencies": {
    "jquery": "^3.6.0",
    "bootstrap": "^4.6.0",
    "datatables.net": "^1.11.0",
    "datatables.net-bs4": "^1.11.0"
  }
}
```

```bash
# Install dependencies
npm install

# Build with bundler (webpack/esbuild)
npm run build
# Outputs to wwwroot/dist/ or similar
```

**When to Use:**
- Teams with Node.js expertise
- New projects (not legacy Web Forms)
- Full-stack JavaScript shops
- Projects planning extensive JavaScript development

---

## BundleConfig Translation

### Current Pattern (Web Forms)

```csharp
public class BundleConfig
{
    public static void RegisterBundles(BundleCollection bundles)
    {
        bundles.Add(new ScriptBundle("~/bundles/jquery")
            .Include("~/Scripts/jquery-{version}.js"));
        
        bundles.Add(new StyleBundle("~/Content/css")
            .Include("~/Content/bootstrap.css",
                     "~/Content/site.css"));
        
        BundleTable.EnableOptimizations = true; // Minify + cache-bust
    }
}
```

Rendered in _Layout.cshtml:
```html
<!-- Bundles render as single compressed file with cache-bust query string -->
<script src="/bundles/jquery?v=abc123def456"></script>
<link href="/Content/css?v=xyz789abc000" rel="stylesheet" />
```

### Translation Options for Blazor

#### Option 1: Manual Link/Script Tags in App.razor or _Host.cshtml

**Simplest approach, works for most apps:**

```html
<!-- App.razor or _Host.cshtml <head> -->
<link href="_framework/lib/bootstrap/bootstrap.min.css" rel="stylesheet" />
<link href="css/site.css" rel="stylesheet" />

<script src="_framework/lib/jquery/jquery.min.js"></script>
<script src="_framework/lib/bootstrap/bootstrap.min.js"></script>
```

**Pros:**
- Simple, no dependencies
- Works immediately
- No build tool required

**Cons:**
- No minification/bundling
- Cache-busting manual
- Multiple HTTP requests

---

#### Option 2: WebOptimizer (Drop-in ASP.NET Core Bundler)

**Use `BuildBundlerMinifier` or `LigerShark.WebOptimizer`:**

```csharp
// Program.cs
builder.Services.AddWebOptimizer(env => {
    env.AddCssBundle("css/bundle.css",
        "css/site.css",
        "lib/bootstrap/bootstrap.min.css");
    
    env.AddJavaScriptBundle("js/bundle.js",
        "lib/jquery/jquery.min.js",
        "lib/bootstrap/bootstrap.bundle.min.js");
});

app.UseWebOptimizer();
```

```html
<!-- Rendered as cache-busted single file -->
<link href="css/bundle.css?v=xyz789" rel="stylesheet" />
<script src="js/bundle.js?v=abc123"></script>
```

**Pros:**
- Automatic minification + cache-busting
- Drop-in replacement for BundleConfig mindset
- No external toolchain required

**Cons:**
- Additional NuGet dependency
- Build-time bundling (not development convenience)

---

#### Option 3: ASP.NET Razor Assets Build (`Asp.Razor.Assets`)

**Modern .NET approach (preview feature, .NET 8+):**

Use declarative bundle definitions in a `.assets.json` file or generate via code:

```json
// bundleconfig.json (for bundler tool)
[
  {
    "outputFileName": "wwwroot/css/site.min.css",
    "inputFiles": ["wwwroot/css/site.css", "wwwroot/lib/bootstrap/bootstrap.min.css"]
  },
  {
    "outputFileName": "wwwroot/js/site.min.js",
    "inputFiles": ["wwwroot/lib/jquery/jquery.min.js", "wwwroot/lib/bootstrap/bootstrap.bundle.min.js"]
  }
]
```

**Pros:**
- Native .NET tooling
- Forward-looking approach
- Future-proof

**Cons:**
- Requires latest .NET version
- Preview/evolving API
- Documentation sparse

---

#### Option 4: No Bundling (Embrace Modern Browser Caching)

**Load individual files, rely on browser HTTP/2 multiplexing + CDN:**

```html
<link href="css/site.css" rel="stylesheet" />
<link href="lib/bootstrap/bootstrap.min.css" rel="stylesheet" />

<script src="lib/jquery/jquery.min.js"></script>
<script src="lib/bootstrap/bootstrap.bundle.min.js"></script>
```

**Pros:**
- Simplest, no tooling
- HTTP/2 multiplexing handles multiple requests efficiently
- Granular caching (each file cached separately)
- Works immediately

**Cons:**
- More HTTP requests (overhead on slow networks)
- No built-in minification
- Manual cache-busting if needed

---

## Automation: `bwfc migrate-assets` Command Design

### Proposed Command

```bash
# Full automation: detect packages, extract, generate references
bwfc migrate-assets --source C:\MyProject

# Or with options
bwfc migrate-assets `
  --source C:\MyProject `
  --strategy cdn `
  --known-packages-only `
  --output-format "manual-links"
```

### Command Flow

```
1. Read packages.config
2. Scan packages/ folder for Content/, Scripts/ directories
3. Build package inventory (known OSS + custom)
4. For each package:
   a. If known OSS (jQuery, Bootstrap, etc.) AND --strategy cdn:
      → Suggest CDN URL, skip extraction
   b. Else:
      → Extract to wwwroot/lib/{PackageName}/
      → Generate <link> / <script> tags
5. Generate asset manifest (JSON or HTML snippet)
6. Output Blazor-compatible asset references (for App.razor or _Host.cshtml)
7. Report summary (X packages extracted, Y CDN suggested, Z custom)
```

### Generated Output Example

**Console Output:**
```
✓ NuGet Static Asset Migration
========================================

Detected 12 packages:
  ✓ jQuery.3.6.0         → CDN suggestion (https://code.jquery.com/...)
  ✓ Bootstrap.4.6.0      → CDN suggestion
  ⓘ MyApp.Reports.1.0.0  → Extracted to wwwroot/lib/MyApp.Reports/
  ⓘ custom App.css       → Copied to wwwroot/css/

Generated Asset Reference (_Host.cshtml <head>):
---
<link href="https://code.jquery.com/jquery-3.6.0.min.js" rel="stylesheet" />
<link href="/_framework/lib/bootstrap/bootstrap.min.css" rel="stylesheet" />
<link href="/_framework/lib/MyApp.Reports/reports.css" rel="stylesheet" />
<link href="css/app.css" rel="stylesheet" />

<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
<script src="/_framework/lib/bootstrap/bootstrap.min.js"></script>
<script src="/_framework/lib/MyApp.Reports/reports.js"></script>
---

⚠ Manual steps:
  1. Paste asset references into App.razor <head> or _Host.cshtml <head>
  2. Run 'dotnet build' to validate
  3. Test in browser (open DevTools → Network tab)

Summary:
  Extracted: 2 packages
  CDN mapped: 2 packages
  Custom: 1 package
  Total: 5 assets
```

**Generated File (asset-manifest.json):**
```json
{
  "timestamp": "2026-03-08T14:32:00Z",
  "strategy": "hybrid",
  "packages": [
    {
      "id": "jQuery",
      "version": "3.6.0",
      "type": "extracted",
      "files": ["lib/jQuery/jquery-3.6.0.min.js"],
      "references": ["<script src=\"/_framework/lib/jQuery/jquery-3.6.0.min.js\"></script>"]
    },
    {
      "id": "Bootstrap",
      "version": "4.6.0",
      "type": "cdn",
      "cdnUrl": "https://stackpath.bootstrapcdn.com/bootstrap/4.6.0/",
      "references": [
        "<link href=\"https://stackpath.bootstrapcdn.com/bootstrap/4.6.0/css/bootstrap.min.css\" rel=\"stylesheet\" />",
        "<script src=\"https://stackpath.bootstrapcdn.com/bootstrap/4.6.0/js/bootstrap.min.js\"></script>"
      ]
    }
  ]
}
```

### Implementation Sketch (PowerShell)

```powershell
# migration-toolkit/scripts/Migrate-NugetStaticAssets.ps1

param(
    [string]$SourcePath = (Get-Location),
    [ValidateSet('extract', 'cdn', 'hybrid')]
    [string]$Strategy = 'hybrid',
    [switch]$KnownPackagesOnly,
    [string]$OutputFormat = 'html' # or 'json'
)

# Known CDN mappings
$cdnMappings = @{
    'jQuery' = 'https://code.jquery.com/jquery-{VERSION}.min.js'
    'Bootstrap' = 'https://stackpath.bootstrapcdn.com/bootstrap/{VERSION}/'
    'Modernizr' = 'https://cdnjs.cloudflare.com/ajax/libs/modernizr/{VERSION}/modernizr.min.js'
    # ... more mappings
}

# 1. Parse packages.config
$packagesConfig = "$SourcePath\packages.config"
if (-not (Test-Path $packagesConfig)) {
    Write-Error "packages.config not found at $packagesConfig"
    return
}

[xml]$xml = Get-Content $packagesConfig
$packages = $xml.packages.package

# 2. Build inventory
$inventory = @()
foreach ($pkg in $packages) {
    $pkgDir = Join-Path "$SourcePath\packages" "$($pkg.id).$($pkg.version)"
    $contentDir = Join-Path $pkgDir 'Content'
    $scriptsDir = Join-Path $pkgDir 'Scripts'
    
    if ((Test-Path $contentDir) -or (Test-Path $scriptsDir)) {
        $inventory += [PSCustomObject]@{
            Id = $pkg.id
            Version = $pkg.version
            Path = $pkgDir
            HasContent = Test-Path $contentDir
            HasScripts = Test-Path $scriptsDir
            IsKnown = $cdnMappings.ContainsKey($pkg.id)
        }
    }
}

# 3. Extract or map to CDN
$assetReferences = @()
foreach ($item in $inventory) {
    if ($item.IsKnown -and ($Strategy -eq 'cdn' -or $Strategy -eq 'hybrid')) {
        $cdnTemplate = $cdnMappings[$item.Id]
        $cdnUrl = $cdnTemplate -replace '{VERSION}', $item.Version
        $assetReferences += "<script src='$cdnUrl'></script>"
    } else {
        # Extract to wwwroot/lib/
        $wwwrootLib = Join-Path $SourcePath "wwwroot\lib\$($item.Id)"
        New-Item -ItemType Directory -Path $wwwrootLib -Force | Out-Null
        
        if ($item.HasContent) {
            Copy-Item (Join-Path $item.Path 'Content\*') -Destination $wwwrootLib -Recurse -Force
        }
        if ($item.HasScripts) {
            Copy-Item (Join-Path $item.Path 'Scripts\*') -Destination $wwwrootLib -Recurse -Force
        }
        
        # Generate reference
        Get-ChildItem $wwwrootLib -Recurse -Include '*.js', '*.css' | ForEach-Object {
            $relPath = $_.FullName -replace [regex]::Escape($wwwrootLib), "/_framework/lib/$($item.Id)"
            $relPath = $relPath -replace '\\', '/'
            
            if ($_.Extension -eq '.css') {
                $assetReferences += "<link href='$relPath' rel='stylesheet' />"
            } elseif ($_.Extension -eq '.js') {
                $assetReferences += "<script src='$relPath'></script>"
            }
        }
    }
}

# 4. Output
if ($OutputFormat -eq 'html') {
    Write-Host @"
<!-- Paste into App.razor <head> or _Host.cshtml <head> -->
$($assetReferences -join "`n")
"@
} elseif ($OutputFormat -eq 'json') {
    $assetReferences | ConvertTo-Json | Write-Host
}

Write-Host "`n✓ Migration complete: $($assetReferences.Count) assets"
```

---

## Recommendation: Hybrid Option C + Build-Time Bundling

### Proposed Approach for BWFC

**Combine Option C (NuGet Extraction) with lightweight build-time bundling:**

1. **Phase 1: Extraction (Migration)**
   - `bwfc migrate-assets` reads `packages.config`
   - Extracts all NuGet Content/Scripts to `wwwroot/lib/`
   - Generates asset manifest (`asset-manifest.json`)
   - Suggests CDN replacements for known OSS (jQuery, Bootstrap, etc.)
   - Outputs HTML snippet for `App.razor` or `_Host.cshtml`

2. **Phase 2: Optional Bundling (Post-Migration)**
   - Teams can integrate **WebOptimizer** or **esbuild** for minification + cache-busting
   - Or stick with manual links (acceptable for most apps, especially with HTTP/2)

3. **Phase 3: Documentation**
   - Migration guide: "Translating NuGet Assets to Blazor"
   - Performance guide: "When to Bundle, When to Serve Individual Files"
   - Security: "CDN Risk Assessment & Fallback Strategies"

### Why This Approach

| Criterion | Reason |
|-----------|--------|
| **Handles all scenarios** | Works for OSS packages, custom packages, hybrid setups |
| **Automation-friendly** | Integrates into `bwfc migrate-assets` toolkit command |
| **Low barrier to entry** | Teams don't need to learn npm/webpack/build tooling unless they want to |
| **Preserves fidelity** | Exact same assets as original Web Forms app |
| **Repeatable** | Can re-run on `packages.config` changes |
| **Observable** | Generated asset manifest makes decisions auditable |

---

## Migration Toolkit Integration

### New Command in `migration-toolkit/`

**File:** `migration-toolkit/scripts/Migrate-NugetStaticAssets.ps1`

**Callable from:** `bwfc-migrate.ps1` static assets phase

```powershell
# bwfc-migrate.ps1 (excerpt)
# ...
# Phase: Static Assets
# ...
if ($detectedAssets.Count -gt 0) {
    Write-Host "🔍 Detected NuGet static assets, extracting..."
    & "$PSScriptRoot\scripts\Migrate-NugetStaticAssets.ps1" `
        -SourcePath $sourceDir `
        -OutputPath $outputDir `
        -Strategy 'hybrid'
    
    if ($?) {
        Write-Host "✓ Static assets migrated"
    }
}
```

### Acceptance Criteria for Shipping

1. ✅ Detects `packages.config` in source app
2. ✅ Scans `packages/` folder for `Content/` and `Scripts/` directories
3. ✅ Extracts known OSS packages to `wwwroot/lib/{PackageName}/`
4. ✅ Suggests CDN URLs for 10+ common packages (jQuery, Bootstrap, DataTables, etc.)
5. ✅ Preserves custom packages (no CDN mapping)
6. ✅ Generates `asset-manifest.json` with extraction summary
7. ✅ Outputs `AssetReferences.html` snippet for `App.razor` or `_Host.cshtml`
8. ✅ Tested on DepartmentPortal (custom CSS extraction)
9. ✅ Tested on WingtipToys (mixed OSS + custom packages if present)
10. ✅ Documented in migration guide with performance implications

---

## DepartmentPortal Case Study

### Original (Web Forms)

```
DepartmentPortal/
├── packages.config
│   └── Microsoft.CodeDom.Providers.DotNetCompilerPlatform (build tool, no assets)
├── Content/
│   └── Site.css (custom app stylesheet, 291 lines)
├── Scripts/
│   └── (empty or custom JS)
└── No BundleConfig.cs (manual link pattern)
```

**In _Layout.cshtml or Site.Master:**
```html
<link rel="stylesheet" href="~/Content/Site.css" />
<!-- References in markup, not bundled -->
```

### Migrated (Blazor) - Current State

```
AfterDepartmentPortal/
├── wwwroot/
│   └── css/
│       └── site.css (custom app stylesheet copied)
└── App.razor or _Host.cshtml
    └── <link href="css/site.css" rel="stylesheet" />
```

### With `bwfc migrate-assets`

```
AfterDepartmentPortal/
├── wwwroot/
│   ├── css/
│   │   └── site.css
│   └── lib/
│       └── (no external packages in DepartmentPortal, only custom CSS)
├── asset-manifest.json
│   └── { packages: [], customAssets: ['css/site.css'] }
└── AssetReferences.html
    └── <!-- No external CDN suggestions, only custom CSS link -->
```

**Result:** DepartmentPortal is a **minimal case** — zero external NuGet assets to migrate. The `bwfc migrate-assets` command would simply copy the custom CSS and report "No external packages detected."

---

## Security & Compliance Considerations

### CDN Risk Assessment

**When using CDN (Option A):**
- ⚠️ Internet dependency (app fails if CDN unavailable)
- ⚠️ Version mismatch risk (app expects v3.6.0, CDN serves v4.0.0)
- ⚠️ SRI (Subresource Integrity) headers recommended for mitigation
- ⚠️ CSP (Content Security Policy) must allow CDN origin

**Mitigation:**
```html
<!-- SRI: Ensures browser discards file if hash doesn't match -->
<script
  src="https://code.jquery.com/jquery-3.6.0.min.js"
  integrity="sha384-TSQFrpgM8IXTHhg1z8zw2gR7T0Ye9fwkYjYpWWGrMxMJ7wD5Ux6H6O7h4gR6Y7c"
  crossorigin="anonymous">
</script>

<!-- CSP: Restrict which origins can load scripts/styles -->
<meta http-equiv="Content-Security-Policy"
      content="script-src 'self' https://code.jquery.com; style-src 'self' https://stackpath.bootstrapcdn.com" />
```

### Custom Package Security (Option C)

**When extracting custom NuGet packages:**
- ✅ Assets are code-reviewed (part of NuGet package build process)
- ✅ No external network dependency
- ✅ Supports air-gapped / restricted environments
- ⚠️ wwwroot footprint grows with package count
- ⚠️ Dependency on `packages/` folder being present and clean

---

## Rollout Plan (Q2 2026)

### Milestone 1: Foundation (Week 1–2)
- ✅ Implement `Migrate-NugetStaticAssets.ps1` with Option C extraction
- ✅ Add CDN mapping for 15+ common packages
- ✅ Generate `asset-manifest.json` + `AssetReferences.html`
- ✅ Test on DepartmentPortal (custom CSS) and WingtipToys (mixed packages)

### Milestone 2: Toolkit Integration (Week 2–3)
- ✅ Integrate into `bwfc migrate-assets` command
- ✅ Add `--strategy` and `--cdn-only` options
- ✅ Update `bwfc-migrate.ps1` to call static assets phase
- ✅ Add error handling and logging

### Milestone 3: Documentation (Week 3–4)
- ✅ Create `docs/Migration/Static-Assets.md`
- ✅ Document all four options (CDN, LibMan, Extraction, npm)
- ✅ Provide decision tree: "Which option is right for my app?"
- ✅ Add performance benchmarks (BundleConfig vs. modern approaches)
- ✅ Security best practices (SRI, CSP, CDN fallbacks)

### Milestone 4: Hardening (Week 4–5)
- ✅ Support `.nupkg` file inspection (fallback if packages folder unavailable)
- ✅ Handle edge cases (symlinks, UNC paths, corrupted packages)
- ✅ Add diagnostics: `bwfc diagnose-assets` command
- ✅ Beta test with team members on real Web Forms projects

### Milestone 5: GA Release (Week 5–6)
- ✅ Feature-complete `bwfc migrate-assets`
- ✅ Included in next BWFC release
- ✅ Blog post: "Migrating NuGet Assets to Blazor: Strategy & Tools"

---

## Conclusion

**Recommended Direction:** Implement **Option C (NuGet Extraction Tool) + WebOptimizer (optional bundling)** as the default migration strategy for BWFC.

**Benefits:**
- ✅ Works for all NuGet packages (OSS + custom)
- ✅ Zero external dependencies during migration
- ✅ Intelligent CDN suggestions for known packages (reduces wwwroot size)
- ✅ Automated, auditable, repeatable
- ✅ Integrates seamlessly into `bwfc migrate-assets` command
- ✅ Supports teams at all skill levels (backend-focused → modern web stack)

**Next Steps:**
1. Jeff Fritz approves strategy
2. Implement `Migrate-NugetStaticAssets.ps1`
3. Create GitHub issue (#TBD) to track implementation
4. Add to M22 sprint plan
5. Coordinate with Beast (docs) and Jubilee (sample pages)

---

**Document Owner:** Forge  
**Last Updated:** 2026-03-08  
**Status:** Awaiting Team Review & Approval
