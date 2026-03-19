<#
.SYNOPSIS
    Generates a health-snapshot.json file containing pre-computed Component Health Dashboard data.

.DESCRIPTION
    Builds and runs the GenerateHealthSnapshot console tool, which performs live reflection
    and file scanning against the repository to produce a JSON snapshot of all component
    health reports. This snapshot can be bundled with published apps so the dashboard
    works in environments where the repo filesystem is not available (e.g., Docker).

.PARAMETER OutputPath
    Path to write the snapshot file. Defaults to the repo root's health-snapshot.json.

.EXAMPLE
    .\Generate-HealthSnapshot.ps1
    .\Generate-HealthSnapshot.ps1 -OutputPath ./publish/health-snapshot.json
#>
param(
    [string]$OutputPath
)

$ErrorActionPreference = 'Stop'

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$projectPath = Join-Path $PSScriptRoot 'GenerateHealthSnapshot' 'GenerateHealthSnapshot.csproj'

if (-not $OutputPath) {
    $OutputPath = Join-Path $repoRoot 'health-snapshot.json'
}

Write-Host "Building snapshot generator..." -ForegroundColor Cyan
dotnet build $projectPath -c Release --nologo -v quiet
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed."
    exit 1
}

Write-Host "Generating health snapshot..." -ForegroundColor Cyan
dotnet run --project $projectPath -c Release --no-build -- $repoRoot $OutputPath
if ($LASTEXITCODE -ne 0) {
    Write-Error "Snapshot generation failed."
    exit 1
}

Write-Host "Health snapshot written to: $OutputPath" -ForegroundColor Green
