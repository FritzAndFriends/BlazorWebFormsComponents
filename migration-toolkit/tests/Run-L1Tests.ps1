<#
.SYNOPSIS
    L1 Migration Script Test Harness — measures quality metrics for bwfc-migrate.ps1.

.DESCRIPTION
    Runs bwfc-migrate.ps1 against focused test cases and compares actual output
    to expected output. Reports pass rate, line-level accuracy, and timing.

.PARAMETER Verbose
    Show detailed diff output for failing test cases.

.EXAMPLE
    .\Run-L1Tests.ps1
    .\Run-L1Tests.ps1 -Verbose
#>

[CmdletBinding()]
param()

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$TestRoot     = $PSScriptRoot
$InputDir     = Join-Path $TestRoot 'inputs'
$ExpectedDir  = Join-Path $TestRoot 'expected'
$MigrateScript = Join-Path $TestRoot '..\scripts\bwfc-migrate.ps1'

if (-not (Test-Path $MigrateScript)) {
    Write-Error "Migration script not found: $MigrateScript"
    return
}

$MigrateScript = (Resolve-Path $MigrateScript).Path

# ============================================================
#  Helpers
# ============================================================

function Normalize-Content {
    param([string]$Text)
    # Normalize line endings to LF, trim trailing whitespace per line, trim trailing blank lines
    $lines = ($Text -replace "`r`n", "`n") -split "`n"
    $lines = @($lines | ForEach-Object { $_.TrimEnd() })
    # Remove trailing empty lines
    while ($lines.Count -gt 0 -and [string]::IsNullOrWhiteSpace($lines[-1])) {
        if ($lines.Count -eq 1) { $lines = @(); break }
        $lines = @($lines[0..($lines.Count - 2)])
    }
    return ($lines -join "`n")
}

function Get-LineDiff {
    param(
        [string[]]$Expected,
        [string[]]$Actual
    )
    $diffs = @()
    $maxLines = [Math]::Max($Expected.Count, $Actual.Count)
    for ($i = 0; $i -lt $maxLines; $i++) {
        $exp = if ($i -lt $Expected.Count) { $Expected[$i] } else { '<missing>' }
        $act = if ($i -lt $Actual.Count) { $Actual[$i] } else { '<missing>' }
        if ($exp -ne $act) {
            $diffs += [PSCustomObject]@{
                Line     = $i + 1
                Expected = $exp
                Actual   = $act
            }
        }
    }
    return $diffs
}

# ============================================================
#  Discover test cases
# ============================================================

$inputFiles = Get-ChildItem -Path $InputDir -Filter '*.aspx' | Sort-Object Name
if ($inputFiles.Count -eq 0) {
    Write-Error "No .aspx test inputs found in: $InputDir"
    return
}

Write-Host ''
Write-Host '================================================================' -ForegroundColor Cyan
Write-Host '  L1 Migration Script — Test Harness' -ForegroundColor Cyan
Write-Host '================================================================' -ForegroundColor Cyan
Write-Host "  Test cases:  $($inputFiles.Count)"
Write-Host "  Script:      $MigrateScript"
Write-Host ''

# ============================================================
#  Run each test case
# ============================================================

$results = [System.Collections.Generic.List[PSCustomObject]]::new()
$totalStopwatch = [System.Diagnostics.Stopwatch]::StartNew()

foreach ($inputFile in $inputFiles) {
    $tcName = $inputFile.BaseName
    $expectedFile = Join-Path $ExpectedDir "$tcName.razor"

    if (-not (Test-Path $expectedFile)) {
        $results.Add([PSCustomObject]@{
            Name           = $tcName
            Status         = 'SKIP'
            MatchingLines  = 0
            TotalLines     = 0
            DiffCount      = 0
            RuntimeMs      = 0
            Error          = 'No expected output file'
            Diffs          = @()
        })
        continue
    }

    # Create temp directories for isolated execution
    $tmpIn  = Join-Path ([System.IO.Path]::GetTempPath()) "l1test_in_$tcName"
    $tmpOut = Join-Path ([System.IO.Path]::GetTempPath()) "l1test_out_$tcName"
    if (Test-Path $tmpIn)  { Remove-Item -Recurse -Force $tmpIn }
    if (Test-Path $tmpOut) { Remove-Item -Recurse -Force $tmpOut }
    New-Item -ItemType Directory -Path $tmpIn -Force | Out-Null
    Copy-Item $inputFile.FullName -Destination $tmpIn

    $sw = [System.Diagnostics.Stopwatch]::StartNew()
    $scriptError = $null

    try {
        # Run the L1 script (suppress all console output — capture errors via catch)
        $null = & $MigrateScript -Path $tmpIn -Output $tmpOut -SkipProjectScaffold *>&1
        $sw.Stop()

        # Find the output .razor file
        $razorFile = Get-ChildItem $tmpOut -Filter '*.razor' -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1

        if (-not $razorFile) {
            $results.Add([PSCustomObject]@{
                Name           = $tcName
                Status         = 'ERROR'
                MatchingLines  = 0
                TotalLines     = 0
                DiffCount      = 0
                RuntimeMs      = $sw.ElapsedMilliseconds
                Error          = 'No .razor file produced'
                Diffs          = @()
            })
            continue
        }

        # Compare actual vs expected
        $actualRaw   = Get-Content $razorFile.FullName -Raw -Encoding UTF8
        $expectedRaw = Get-Content $expectedFile -Raw -Encoding UTF8

        $actualNorm   = Normalize-Content $actualRaw
        $expectedNorm = Normalize-Content $expectedRaw

        $actualLines   = $actualNorm -split "`n"
        $expectedLines = $expectedNorm -split "`n"

        $diffs = @(Get-LineDiff -Expected $expectedLines -Actual $actualLines)

        $totalLines    = [Math]::Max($expectedLines.Count, $actualLines.Count)
        $matchingLines = $totalLines - $diffs.Count

        $status = if ($diffs.Count -eq 0) { 'PASS' } else { 'FAIL' }

        $results.Add([PSCustomObject]@{
            Name           = $tcName
            Status         = $status
            MatchingLines  = $matchingLines
            TotalLines     = $totalLines
            DiffCount      = $diffs.Count
            RuntimeMs      = $sw.ElapsedMilliseconds
            Error          = $null
            Diffs          = $diffs
        })

    } catch {
        $sw.Stop()
        $results.Add([PSCustomObject]@{
            Name           = $tcName
            Status         = 'ERROR'
            MatchingLines  = 0
            TotalLines     = 0
            DiffCount      = 0
            RuntimeMs      = $sw.ElapsedMilliseconds
            Error          = $_.Exception.Message
            Diffs          = @()
        })
    } finally {
        if (Test-Path $tmpIn)  { Remove-Item -Recurse -Force $tmpIn }
        if (Test-Path $tmpOut) { Remove-Item -Recurse -Force $tmpOut }
    }
}

$totalStopwatch.Stop()

# ============================================================
#  Report results
# ============================================================

$passCount  = @($results | Where-Object Status -eq 'PASS').Count
$failCount  = @($results | Where-Object Status -eq 'FAIL').Count
$errorCount = @($results | Where-Object Status -eq 'ERROR').Count
$skipCount  = @($results | Where-Object Status -eq 'SKIP').Count
$totalCount = $results.Count
$runnableCount = $totalCount - $skipCount

$totalMatchingLines = ($results | Measure-Object -Property MatchingLines -Sum).Sum
$totalTotalLines    = ($results | Measure-Object -Property TotalLines -Sum).Sum
$lineAccuracy = if ($totalTotalLines -gt 0) { [Math]::Round(($totalMatchingLines / $totalTotalLines) * 100, 1) } else { 0 }

Write-Host ''
Write-Host '================================================================' -ForegroundColor Cyan
Write-Host '  Test Results' -ForegroundColor Cyan
Write-Host '================================================================' -ForegroundColor Cyan
Write-Host ''

# Results table
$statusColors = @{ PASS = 'Green'; FAIL = 'Red'; ERROR = 'Yellow'; SKIP = 'DarkGray' }
$maxNameLen = ($results | ForEach-Object { $_.Name.Length } | Measure-Object -Maximum).Maximum
$headerFmt = "{0,-$maxNameLen}  {1,-6}  {2,11}  {3,6}  {4,8}"
$rowFmt    = "{0,-$maxNameLen}  {1,-6}  {2,5}/{3,-5}  {4,6}  {5,5}ms"

Write-Host ($headerFmt -f 'Test Case', 'Status', 'Lines Match', 'Diffs', 'Time')
Write-Host ('-' * ($maxNameLen + 42))

foreach ($r in $results) {
    $color = $statusColors[$r.Status]
    $line = $rowFmt -f $r.Name, $r.Status, $r.MatchingLines, $r.TotalLines, $r.DiffCount, $r.RuntimeMs
    Write-Host $line -ForegroundColor $color
}

Write-Host ''

# Show diffs for failures
$failures = @($results | Where-Object { $_.Status -eq 'FAIL' -and $_.Diffs.Count -gt 0 })
if ($failures.Count -gt 0) {
    Write-Host '================================================================' -ForegroundColor Yellow
    Write-Host '  Differences (FAIL details)' -ForegroundColor Yellow
    Write-Host '================================================================' -ForegroundColor Yellow

    foreach ($f in $failures) {
        Write-Host ''
        Write-Host "  $($f.Name):" -ForegroundColor Yellow
        foreach ($d in $f.Diffs) {
            Write-Host "    Line $($d.Line):" -ForegroundColor DarkGray
            Write-Host "      Expected: $($d.Expected)" -ForegroundColor Green
            Write-Host "      Actual:   $($d.Actual)" -ForegroundColor Red
        }
    }
    Write-Host ''
}

# Show errors
$errors = @($results | Where-Object Status -eq 'ERROR')
if ($errors.Count -gt 0) {
    Write-Host '================================================================' -ForegroundColor Red
    Write-Host '  Errors' -ForegroundColor Red
    Write-Host '================================================================' -ForegroundColor Red
    foreach ($e in $errors) {
        Write-Host "  $($e.Name): $($e.Error)" -ForegroundColor Red
    }
    Write-Host ''
}

# Summary metrics
Write-Host '================================================================' -ForegroundColor Cyan
Write-Host '  Summary Metrics' -ForegroundColor Cyan
Write-Host '================================================================' -ForegroundColor Cyan
Write-Host ''
Write-Host "  Pass rate:       $passCount / $runnableCount ($([Math]::Round(($passCount / [Math]::Max($runnableCount, 1)) * 100, 0))%)"
Write-Host "  Line accuracy:   $totalMatchingLines / $totalTotalLines ($lineAccuracy%)"
Write-Host "  Failures:        $failCount"
Write-Host "  Errors:          $errorCount"
Write-Host "  Skipped:         $skipCount"
Write-Host "  Total time:      $($totalStopwatch.ElapsedMilliseconds)ms ($([Math]::Round($totalStopwatch.Elapsed.TotalSeconds, 1))s)"
Write-Host "  Avg per file:    $([Math]::Round($totalStopwatch.ElapsedMilliseconds / [Math]::Max($runnableCount, 1)))ms"
Write-Host ''

# Return structured results for programmatic use
return [PSCustomObject]@{
    PassRate      = [Math]::Round(($passCount / [Math]::Max($runnableCount, 1)) * 100, 1)
    LineAccuracy  = $lineAccuracy
    PassCount     = $passCount
    FailCount     = $failCount
    ErrorCount    = $errorCount
    TotalCount    = $totalCount
    TotalTimeMs   = $totalStopwatch.ElapsedMilliseconds
    Results       = $results
}
