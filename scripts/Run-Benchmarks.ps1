<#
.SYNOPSIS
    Performance benchmark suite comparing .NET Framework Web Forms with .NET 10 Blazor equivalents.

.DESCRIPTION
    Launches sample applications and measures HTTP response times for key pages.
    Compares original .NET Framework Web Forms apps (via IIS Express) against
    their migrated .NET 10 Blazor counterparts (via dotnet run).

    Metrics collected per page:
      - Cold-start response time (first request after launch)
      - Warm average response time (50 iterations)
      - P95 response time
      - Response size in bytes
      - HTTP status code

.PARAMETER SkipFramework
    Skip .NET Framework / IIS Express benchmarks (useful when IIS Express is unavailable).

.PARAMETER OutputPath
    Directory for benchmark result files. Default: dev-docs/benchmarks/

.PARAMETER Iterations
    Number of warm requests per page. Default: 50.

.PARAMETER WhatIf
    Validate script parsing and print the benchmark plan without executing anything.

.EXAMPLE
    .\Run-Benchmarks.ps1
    # Full benchmark run

.EXAMPLE
    .\Run-Benchmarks.ps1 -SkipFramework
    # Blazor-only benchmarks

.EXAMPLE
    .\Run-Benchmarks.ps1 -WhatIf
    # Dry-run validation
#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [switch]$SkipFramework,
    [string]$OutputPath,
    [int]$Iterations = 50
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# ── Resolve paths ────────────────────────────────────────────
$repoRoot = Split-Path $PSScriptRoot -Parent
if (-not $OutputPath) {
    $OutputPath = Join-Path $repoRoot 'dev-docs\benchmarks'
}
if (-not (Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
}

# ── App definitions ──────────────────────────────────────────
$apps = @(
    @{
        Name          = 'WingtipToys'
        Kind          = 'Framework'
        SitePath      = Join-Path $repoRoot 'samples\WingtipToys\WingtipToys'
        Port          = 55502
        BaseUrl       = 'http://localhost:55502'
        Pages         = @(
            @{ Name = 'Home';        Path = '/' }
            @{ Name = 'ProductList'; Path = '/ProductList' }
            @{ Name = 'About';       Path = '/About' }
        )
    },
    @{
        Name          = 'AfterWingtipToys'
        Kind          = 'Blazor'
        ProjectFile   = Join-Path $repoRoot 'samples\AfterWingtipToys\WingtipToys.csproj'
        Port          = 55504
        BaseUrl       = 'http://localhost:55504'
        Pages         = @(
            @{ Name = 'Home';        Path = '/' }
            @{ Name = 'ProductList'; Path = '/ProductList' }
            @{ Name = 'About';       Path = '/About' }
        )
    },
    @{
        Name          = 'ContosoUniversity'
        Kind          = 'Framework'
        SitePath      = Join-Path $repoRoot 'samples\ContosoUniversity\ContosoUniversity'
        Port          = 55503
        BaseUrl       = 'http://localhost:55503'
        Pages         = @(
            @{ Name = 'Home';     Path = '/Home.aspx' }
            @{ Name = 'Students'; Path = '/Students.aspx' }
            @{ Name = 'About';    Path = '/About.aspx' }
        )
    },
    @{
        Name          = 'AfterContosoUniversity'
        Kind          = 'Blazor'
        ProjectFile   = Join-Path $repoRoot 'samples\AfterContosoUniversity\ContosoUniversity.csproj'
        Port          = 55505
        BaseUrl       = 'http://localhost:55505'
        Pages         = @(
            @{ Name = 'Home';     Path = '/' }
            @{ Name = 'Students'; Path = '/Students' }
            @{ Name = 'About';    Path = '/About' }
        )
    }
)

# ── Console helpers ──────────────────────────────────────────
function Write-Step  { param([string]$Msg) Write-Host "`n[$((Get-Date).ToString('HH:mm:ss'))] $Msg" -ForegroundColor Cyan }
function Write-OK    { param([string]$Msg) Write-Host "  OK  $Msg" -ForegroundColor Green }
function Write-Warn  { param([string]$Msg) Write-Host "  WARN $Msg" -ForegroundColor Yellow }
function Write-Fail  { param([string]$Msg) Write-Host "  FAIL $Msg" -ForegroundColor Red }

# ── MSBuild discovery ────────────────────────────────────────
function Find-MSBuild {
    # VS 2026 / VS 18 (any edition, Program Files)
    $vs2026 = Get-ChildItem "${env:ProgramFiles}\Microsoft Visual Studio\18\*\MSBuild\Current\Bin\MSBuild.exe" -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($vs2026) { return $vs2026.FullName }

    # VS 2022 (any edition)
    $vs2022 = Get-ChildItem "${env:ProgramFiles}\Microsoft Visual Studio\2022\*\MSBuild\Current\Bin\MSBuild.exe" -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($vs2022) { return $vs2022.FullName }

    # VS 2019 (any edition, x86 program files)
    $vs2019 = Get-ChildItem "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\*\MSBuild\Current\Bin\MSBuild.exe" -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($vs2019) { return $vs2019.FullName }

    # VS 2017 (any edition, x86 program files — MSBuild 15.0)
    $vs2017 = Get-ChildItem "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2017\*\MSBuild\15.0\Bin\MSBuild.exe" -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($vs2017) { return $vs2017.FullName }

    # vswhere fallback (finds any VS installation with MSBuild)
    $vswhereExe = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
    if (Test-Path $vswhereExe) {
        $found = & $vswhereExe -all -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' -sort 2>$null | Select-Object -First 1
        if ($found -and (Test-Path $found)) { return $found }
    }

    return $null
}

# ── WhatIf: just print plan and exit ─────────────────────────
if ($WhatIfPreference) {
    Write-Step 'Benchmark Plan (WhatIf mode — no actions taken)'
    Write-Host ''
    Write-Host '  Iterations per page : ' -NoNewline; Write-Host $Iterations -ForegroundColor White
    Write-Host '  Output directory    : ' -NoNewline; Write-Host $OutputPath -ForegroundColor White
    Write-Host '  Skip Framework      : ' -NoNewline; Write-Host $SkipFramework -ForegroundColor White
    Write-Host ''
    foreach ($app in $apps) {
        $skip = ($app.Kind -eq 'Framework' -and $SkipFramework)
        $status = if ($skip) { '(SKIP)' } else { '' }
        Write-Host "  [$($app.Kind)] $($app.Name) on port $($app.Port) $status" -ForegroundColor $(if ($skip) { 'Yellow' } else { 'White' })
        foreach ($pg in $app.Pages) {
            Write-Host "    - $($pg.Name): $($app.BaseUrl)$($pg.Path)"
        }
    }
    Write-Host ''
    Write-OK 'Script parsed and validated successfully.'
    exit 0
}

# ── Process tracking for cleanup ─────────────────────────────
$launchedProcesses = [System.Collections.Generic.List[System.Diagnostics.Process]]::new()

function Stop-AllLaunched {
    foreach ($proc in $launchedProcesses) {
        if (-not $proc.HasExited) {
            Write-Warn "Stopping process $($proc.Id) ($($proc.ProcessName))..."
            try { $proc.Kill($true) } catch { }
            try { $proc.WaitForExit(5000) } catch { }
        }
    }
}

# Kill any stale processes on benchmark ports from previous runs
function Clear-BenchmarkPorts {
    $benchmarkPorts = $apps | ForEach-Object { $_.Port }
    foreach ($port in $benchmarkPorts) {
        $listeners = netstat -ano 2>$null | Select-String "TCP\s+\S+:$port\s+\S+\s+LISTENING\s+(\d+)"
        foreach ($match in $listeners) {
            $pid = [int]$match.Matches[0].Groups[1].Value
            if ($pid -gt 4) {
                $proc = Get-Process -Id $pid -ErrorAction SilentlyContinue
                if ($proc) {
                    Write-Warn "Killing stale process on port ${port}: $($proc.ProcessName) (PID $pid)"
                    Stop-Process -Id $pid -Force -ErrorAction SilentlyContinue
                }
            }
        }
    }
    Start-Sleep -Seconds 2
}

# ── Measurement function ─────────────────────────────────────
function Measure-PagePerformance {
    param(
        [string]$Url,
        [int]$WarmIterations = 50
    )

    $result = @{
        Url            = $Url
        ColdStartMs    = $null
        AverageMs      = $null
        P95Ms          = $null
        MinMs          = $null
        MaxMs          = $null
        ResponseBytes  = $null
        StatusCode     = $null
        Error          = $null
    }

    try {
        # Cold-start request
        $sw = [System.Diagnostics.Stopwatch]::StartNew()
        $response = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 30
        $sw.Stop()
        $result.ColdStartMs   = $sw.ElapsedMilliseconds
        $result.StatusCode    = [int]$response.StatusCode
        $result.ResponseBytes = if ($response.RawContentLength) { $response.RawContentLength } else { $response.Content.Length }

        # Warm iterations
        $times = [System.Collections.Generic.List[long]]::new($WarmIterations)
        for ($i = 0; $i -lt $WarmIterations; $i++) {
            $sw = [System.Diagnostics.Stopwatch]::StartNew()
            $resp = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 30
            $sw.Stop()
            $times.Add($sw.ElapsedMilliseconds)
        }

        $sorted = $times | Sort-Object
        $result.AverageMs = [math]::Round(($times | Measure-Object -Average).Average, 1)
        $result.P95Ms     = $sorted[[math]::Floor($sorted.Count * 0.95)]
        $result.MinMs     = $sorted[0]
        $result.MaxMs     = $sorted[-1]
    }
    catch {
        $result.Error = $_.Exception.Message
    }

    return $result
}

# ── Wait for an HTTP endpoint to become available ────────────
function Wait-ForEndpoint {
    param(
        [string]$Url,
        [int]$TimeoutSeconds = 60
    )
    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    while ((Get-Date) -lt $deadline) {
        try {
            $null = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 5
            return $true
        }
        catch {
            Start-Sleep -Seconds 1
        }
    }
    return $false
}

# ── Launch a .NET Framework app via IIS Express ──────────────
function Start-FrameworkApp {
    param([hashtable]$App)

    # Check IIS Express
    $iisExe = $null
    foreach ($candidate in @(
        "${env:ProgramFiles}\IIS Express\iisexpress.exe",
        "${env:ProgramFiles(x86)}\IIS Express\iisexpress.exe"
    )) {
        if (Test-Path $candidate) { $iisExe = $candidate; break }
    }
    if (-not $iisExe) {
        return @{ Success = $false; Reason = 'IIS Express not found' }
    }

    # Check site path
    if (-not (Test-Path $App.SitePath)) {
        return @{ Success = $false; Reason = "Site path not found: $($App.SitePath)" }
    }

    # Start LocalDB if sqllocaldb is available (needed for SQL Server–backed Framework apps)
    $sqllocaldb = Get-Command sqllocaldb -ErrorAction SilentlyContinue
    if ($sqllocaldb) {
        Write-Host "    Starting LocalDB instance..."
        & sqllocaldb start MSSQLLocalDB 2>&1 | Out-Null
    }

    # NuGet restore if packages.config exists
    $pkgConfig = Join-Path $App.SitePath 'packages.config'
    if (Test-Path $pkgConfig) {
        $nugetExe = Join-Path $repoRoot 'nuget.exe'
        if (Test-Path $nugetExe) {
            $pkgDir = Join-Path $repoRoot 'src\packages'
            Write-Host "    Restoring NuGet packages for $($App.Name)..."
            & $nugetExe restore $pkgConfig -PackagesDirectory $pkgDir -NonInteractive 2>&1 | Out-Null
        }
    }

    # MSBuild pre-compilation — compile the project before IIS Express launch
    # so that first-request JIT time is dramatically reduced
    $msbuildExe = Find-MSBuild
    $csproj = Get-ChildItem -Path $App.SitePath -Filter "*.csproj" -Recurse | Select-Object -First 1
    if ($csproj -and $msbuildExe) {
        Write-Host "    Pre-compiling $($App.Name) with MSBuild..."
        $buildOutput = & $msbuildExe $csproj.FullName /p:Configuration=Debug /v:minimal /nologo 2>&1
        if ($LASTEXITCODE -ne 0) {
            Write-Warn "MSBuild pre-compilation failed (non-fatal) — IIS Express will compile on demand"
            $buildOutput | ForEach-Object { Write-Host "      $_" -ForegroundColor DarkYellow }
        } else {
            Write-OK "MSBuild pre-compilation succeeded"
        }
    } elseif (-not $msbuildExe) {
        Write-Warn "MSBuild not found — skipping pre-compilation (IIS Express will JIT on first request)"
    }

    # ASP.NET view pre-compilation — pre-compiles .aspx/.ascx/.master files
    $aspnetCompiler = Join-Path $env:windir "Microsoft.NET\Framework64\v4.0.30319\aspnet_compiler.exe"
    if (Test-Path $aspnetCompiler) {
        Write-Host "    Pre-compiling ASP.NET views..."
        & $aspnetCompiler -v / -p $App.SitePath -fixednames 2>&1 | Out-Null
        if ($LASTEXITCODE -ne 0) {
            Write-Warn "ASP.NET view pre-compilation failed (non-fatal)"
        } else {
            Write-OK "ASP.NET view pre-compilation succeeded"
        }
    }

    # Launch IIS Express
    try {
        $iisStderrLog = Join-Path ([System.IO.Path]::GetTempPath()) "iisexpress-$($App.Name)-stderr.log"
        $proc = Start-Process -FilePath $iisExe `
            -ArgumentList "/path:`"$($App.SitePath)`" /port:$($App.Port)" `
            -PassThru -WindowStyle Hidden `
            -RedirectStandardError $iisStderrLog
        $launchedProcesses.Add($proc)

        # Use the first page URL for readiness check (some Framework apps return 403 on /)
        $readinessUrl = if ($App.Pages -and $App.Pages.Count -gt 0) {
            "$($App.BaseUrl)$($App.Pages[0].Path)"
        } else {
            $App.BaseUrl
        }
        $ready = Wait-ForEndpoint -Url $readinessUrl -TimeoutSeconds 180
        if (-not $ready) {
            # Capture diagnostic info to help debug startup failures
            $reason = 'IIS Express started but app did not respond within 180s'
            if (Test-Path $iisStderrLog) {
                $stderr = Get-Content $iisStderrLog -Raw -ErrorAction SilentlyContinue
                if ($stderr) {
                    $reason += "`n    IIS Express stderr:`n$stderr"
                    Write-Fail "IIS Express stderr for $($App.Name):"
                    $stderr -split "`n" | ForEach-Object { Write-Host "      $_" -ForegroundColor DarkRed }
                }
            }
            if ($proc -and -not $proc.HasExited) {
                Write-Warn "IIS Express PID $($proc.Id) is still running — process did not crash"
            } elseif ($proc) {
                Write-Fail "IIS Express exited with code $($proc.ExitCode)"
            }
            return @{ Success = $false; Reason = $reason }
        }
        return @{ Success = $true; Process = $proc }
    }
    catch {
        return @{ Success = $false; Reason = $_.Exception.Message }
    }
}

# ── Launch a Blazor app via dotnet run ───────────────────────
function Start-BlazorApp {
    param([hashtable]$App)

    if (-not (Test-Path $App.ProjectFile)) {
        return @{ Success = $false; Reason = "Project file not found: $($App.ProjectFile)" }
    }

    try {
        $proc = Start-Process -FilePath 'dotnet' `
            -ArgumentList "run --project `"$($App.ProjectFile)`" --urls `"http://localhost:$($App.Port)`"" `
            -PassThru -WindowStyle Hidden
        $launchedProcesses.Add($proc)

        $ready = Wait-ForEndpoint -Url $App.BaseUrl -TimeoutSeconds 120
        if (-not $ready) {
            return @{ Success = $false; Reason = 'dotnet run started but app did not respond within 120s' }
        }
        return @{ Success = $true; Process = $proc }
    }
    catch {
        return @{ Success = $false; Reason = $_.Exception.Message }
    }
}

# ══════════════════════════════════════════════════════════════
# MAIN EXECUTION
# ══════════════════════════════════════════════════════════════
try {
    $benchmarkStart = Get-Date
    $allResults = @{}

    Write-Step 'Performance Benchmark Suite'
    Write-Host "  Repo root  : $repoRoot"
    Write-Host "  Output     : $OutputPath"
    Write-Host "  Iterations : $Iterations per page"
    Write-Host "  Skip .NET Framework: $SkipFramework"

    # ── Collect environment info ─────────────────────────────
    $envInfo = @{
        OS             = [System.Runtime.InteropServices.RuntimeInformation]::OSDescription
        MachineName    = $env:COMPUTERNAME
        ProcessorCount = [Environment]::ProcessorCount
        DotNetVersion  = (& dotnet --version 2>$null) ?? 'N/A'
        PowerShell     = $PSVersionTable.PSVersion.ToString()
        Timestamp      = (Get-Date -Format 'yyyy-MM-ddTHH:mm:ssZ')
    }

    # ── Clean up stale processes on benchmark ports ─────────
    Clear-BenchmarkPorts

    # ── Run benchmarks per app ───────────────────────────────
    foreach ($app in $apps) {
        Write-Step "Benchmarking: $($app.Name) ($($app.Kind))"

        # Skip Framework apps if requested
        if ($app.Kind -eq 'Framework' -and $SkipFramework) {
            Write-Warn "Skipped (SkipFramework flag)"
            $allResults[$app.Name] = @{
                App     = $app.Name
                Kind    = $app.Kind
                Port    = $app.Port
                Skipped = $true
                Reason  = 'SkipFramework flag set'
                Pages   = @()
            }
            continue
        }

        # Launch the app
        Write-Host "  Launching $($app.Name) on port $($app.Port)..."

        # ContosoUniversity DB compatibility: EF6 expects 'Enrollment', EF Core uses 'Enrollments'
        $needsTableRename = ($app.Name -eq 'ContosoUniversity' -and $app.Kind -eq 'Framework')
        if ($needsTableRename) {
            $sqlcmd = Get-Command sqlcmd -ErrorAction SilentlyContinue
            if ($sqlcmd) {
                Write-Host "    Adjusting DB table names for EF6 compatibility..."
                & sqlcmd -S "(localdb)\MSSQLLocalDB" -d ContosoUniversity -Q "IF OBJECT_ID('dbo.Enrollments') IS NOT NULL EXEC sp_rename 'dbo.Enrollments', 'Enrollment'" 2>&1 | Out-Null
            }
        }

        $launchResult = if ($app.Kind -eq 'Framework') {
            Start-FrameworkApp -App $app
        } else {
            Start-BlazorApp -App $app
        }

        if (-not $launchResult.Success) {
            Write-Fail "Could not start $($app.Name): $($launchResult.Reason)"
            $allResults[$app.Name] = @{
                App     = $app.Name
                Kind    = $app.Kind
                Port    = $app.Port
                Skipped = $true
                Reason  = $launchResult.Reason
                Pages   = @()
            }
            continue
        }

        Write-OK "$($app.Name) is running (PID $($launchResult.Process.Id))"

        # Benchmark each page
        $pageResults = @()
        foreach ($pg in $app.Pages) {
            $url = "$($app.BaseUrl)$($pg.Path)"
            Write-Host "  Measuring $($pg.Name) ($url) ..."

            $metrics = Measure-PagePerformance -Url $url -WarmIterations $Iterations

            if ($metrics.Error) {
                Write-Fail "  Error on $($pg.Name): $($metrics.Error)"
            } else {
                Write-OK "$($pg.Name): avg=$($metrics.AverageMs)ms p95=$($metrics.P95Ms)ms cold=$($metrics.ColdStartMs)ms"
            }

            $pageResults += @{
                Page          = $pg.Name
                Url           = $url
                ColdStartMs   = $metrics.ColdStartMs
                AverageMs     = $metrics.AverageMs
                P95Ms         = $metrics.P95Ms
                MinMs         = $metrics.MinMs
                MaxMs         = $metrics.MaxMs
                ResponseBytes = $metrics.ResponseBytes
                StatusCode    = $metrics.StatusCode
                Error         = $metrics.Error
            }
        }

        $allResults[$app.Name] = @{
            App     = $app.Name
            Kind    = $app.Kind
            Port    = $app.Port
            Skipped = $false
            Pages   = $pageResults
        }

        # Restore table names after Framework ContosoUniversity benchmark
        if ($needsTableRename) {
            $sqlcmd = Get-Command sqlcmd -ErrorAction SilentlyContinue
            if ($sqlcmd) {
                Write-Host "    Restoring DB table names for EF Core compatibility..."
                & sqlcmd -S "(localdb)\MSSQLLocalDB" -d ContosoUniversity -Q "IF OBJECT_ID('dbo.Enrollment') IS NOT NULL EXEC sp_rename 'dbo.Enrollment', 'Enrollments'" 2>&1 | Out-Null
            }
        }
    }

    $benchmarkEnd = Get-Date
    $duration = ($benchmarkEnd - $benchmarkStart).TotalSeconds

    # ── Build output payload ─────────────────────────────────
    $output = @{
        Environment    = $envInfo
        Configuration  = @{
            Iterations     = $Iterations
            SkipFramework  = [bool]$SkipFramework
        }
        DurationSeconds = [math]::Round($duration, 1)
        Results         = $allResults
    }

    # ── Save JSON ────────────────────────────────────────────
    $jsonFile = Join-Path $OutputPath 'benchmark-results.json'
    $output | ConvertTo-Json -Depth 10 | Set-Content -Path $jsonFile -Encoding UTF8
    Write-Step "Results saved to $jsonFile"

    # ── Print summary table ──────────────────────────────────
    Write-Step 'Summary'
    Write-Host ''
    $headerFmt = '{0,-25} {1,-12} {2,10} {3,10} {4,10} {5,12} {6,8}'
    $rowFmt    = '{0,-25} {1,-12} {2,10} {3,10} {4,10} {5,12} {6,8}'

    Write-Host ($headerFmt -f 'App / Page', 'Kind', 'Cold(ms)', 'Avg(ms)', 'P95(ms)', 'Size(B)', 'Status')
    Write-Host ($headerFmt -f ('-' * 25), ('-' * 12), ('-' * 10), ('-' * 10), ('-' * 10), ('-' * 12), ('-' * 8))

    foreach ($appName in @('WingtipToys','AfterWingtipToys','ContosoUniversity','AfterContosoUniversity')) {
        $r = $allResults[$appName]
        if (-not $r) { continue }
        if ($r.Skipped) {
            Write-Host ($rowFmt -f $appName, $r.Kind, 'N/A', 'N/A', 'N/A', 'N/A', 'SKIP') -ForegroundColor Yellow
            continue
        }
        foreach ($pg in $r.Pages) {
            $label = "$appName/$($pg.Page)"
            if ($pg.Error) {
                Write-Host ($rowFmt -f $label, $r.Kind, 'ERR', 'ERR', 'ERR', 'ERR', 'ERR') -ForegroundColor Red
            } else {
                Write-Host ($rowFmt -f $label, $r.Kind, $pg.ColdStartMs, $pg.AverageMs, $pg.P95Ms, $pg.ResponseBytes, $pg.StatusCode)
            }
        }
    }

    Write-Host ''
    Write-OK "Benchmark completed in $([math]::Round($duration, 1))s"

    # ── Invoke report generation ─────────────────────────────
    $reportScript = Join-Path $PSScriptRoot 'Generate-BenchmarkReport.ps1'
    if (Test-Path $reportScript) {
        Write-Step 'Generating benchmark report...'
        & $reportScript -InputPath $jsonFile -OutputPath $OutputPath
    }

} finally {
    # ── Cleanup: stop all launched processes ──────────────────
    Write-Step 'Cleaning up launched processes...'
    Stop-AllLaunched
    Write-OK 'All processes stopped.'
}
