# Build VSIX Extension for BlazorWebFormsComponents Snippets
# This script must be run on Windows with Visual Studio 2022 installed

param(
    [Parameter()]
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

Write-Host "Building BlazorWebFormsComponents.Snippets VSIX..." -ForegroundColor Cyan

# Check if MSBuild is available
$msbuild = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" `
    -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe `
    -prerelease | Select-Object -First 1

if (-not $msbuild) {
    Write-Error "MSBuild not found. Please install Visual Studio 2022 with VSSDK."
    exit 1
}

Write-Host "Using MSBuild: $msbuild" -ForegroundColor Green

# Build the project
& $msbuild BlazorWebFormsComponents.Snippets.csproj /p:Configuration=$Configuration /v:minimal

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nBuild successful!" -ForegroundColor Green
    $vsixPath = "bin\$Configuration\BlazorWebFormsComponents.Snippets.vsix"
    if (Test-Path $vsixPath) {
        Write-Host "VSIX file created at: $vsixPath" -ForegroundColor Green
    } else {
        Write-Warning "Build succeeded but VSIX file not found at expected location."
    }
} else {
    Write-Error "Build failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}
