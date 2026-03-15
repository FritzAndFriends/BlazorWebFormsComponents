---
name: "performance-benchmarks"
description: "Run and re-run .NET Framework vs .NET 10 performance benchmarks, diagnose failures, generate reports with SVG charts"
domain: "performance-testing"
confidence: "high"
source: "earned"
---

## Context

The BlazorWebFormsComponents project maintains performance benchmarks comparing the original .NET Framework 4.5.2 Web Forms sample apps (WingtipToys, ContosoUniversity) against their migrated .NET 10 Blazor equivalents. These benchmarks are used to validate that migration delivers measurable performance improvements.

Benchmarks live on the `benchmark/424-performance-comparison` branch and produce results in `dev-docs/benchmarks/`.

## Scripts

| Script | Purpose |
|--------|---------|
| `scripts/Run-Benchmarks.ps1` | Main orchestrator — launches apps, measures response times, outputs JSON |
| `scripts/Generate-BenchmarkReport.ps1` | Reads JSON → generates Markdown report + SVG bar charts |

## Running Benchmarks

### Full run (all 4 apps)

```powershell
pwsh -File scripts/Run-Benchmarks.ps1 -Iterations 50 -OutputPath dev-docs/benchmarks
```

### Blazor-only (skip .NET Framework / IIS Express)

```powershell
pwsh -File scripts/Run-Benchmarks.ps1 -SkipFramework -Iterations 50 -OutputPath dev-docs/benchmarks
```

### Dry-run validation

```powershell
pwsh -File scripts/Run-Benchmarks.ps1 -WhatIf
```

### Regenerate report from existing JSON

```powershell
pwsh -File scripts/Generate-BenchmarkReport.ps1 -InputPath dev-docs/benchmarks/benchmark-results.json -OutputPath dev-docs/benchmarks
```

## App Matrix

| App | Framework | Port | Launch Method | Status |
|-----|-----------|------|---------------|--------|
| WingtipToys | .NET 4.5.2 | 55502 | IIS Express | ✅ Works — NuGet restore + MSBuild pre-compilation |
| AfterWingtipToys | .NET 10 | 55504 | `dotnet run` | ✅ Works — fixed `@using Microsoft.AspNetCore.Identity` |
| ContosoUniversity | .NET 4.5.2 | 55503 | IIS Express | ✅ Works — requires MSBuild pre-compilation, `.aspx` URLs, and EF6 table rename |
| AfterContosoUniversity | .NET 10 | 55505 | `dotnet run` | ✅ Works — cold start on Students page (~900ms) due to EF Core DB initialization |

## Key Fixes Applied

### ContosoUniversity Framework — Three Issues Resolved

1. **MSBuild pre-compilation**: VS 2026 Enterprise MSBuild compiles the DLL. Without this, IIS Express tries to JIT-compile everything on first request and times out. VS 2017 BuildTools lack WebApplication targets — need VS 2019+.

2. **URL paths**: ContosoUniversity has no default document at `/` (returns 403). Pages must be accessed as `/Home.aspx`, `/Students.aspx`, `/About.aspx`. The readiness check now uses the first page URL.

3. **EF6/EF Core table conflict**: Both apps share the same LocalDB database. EF6 EDMX expects table `dbo.Enrollment` (singular), EF Core expects `dbo.Enrollments` (plural). The script renames the table before/after the Framework benchmark.

### Port Cleanup

Stale IIS Express processes from previous runs can hold benchmark ports open. The script now runs `Clear-BenchmarkPorts` before starting, killing any process listening on ports 55502-55505.

## Metrics Collected

Per page, per app:
- **Cold start (ms)**: First request after launch (includes JIT, DB init)
- **Warm average (ms)**: Mean of N sequential requests
- **P95 (ms)**: 95th percentile response time
- **Min/Max (ms)**: Range boundaries
- **Response size (bytes)**: HTTP response body length
- **Status code**: HTTP status (200 = success)

## Output Files

| File | Content |
|------|---------|
| `dev-docs/benchmarks/benchmark-results.json` | Raw JSON data — all metrics |
| `dev-docs/benchmarks/performance-report.md` | Markdown report with tables + chart references |
| `dev-docs/benchmarks/response-times-wingtiptoys.svg` | Bar chart: Framework vs Blazor response times |
| `dev-docs/benchmarks/response-times-contosouniversity.svg` | Bar chart: Framework vs Blazor response times |

## Patterns

### Pre-flight checks before running

1. **Verify .NET SDK**: `dotnet --version` (should be 10.x)
2. **Verify IIS Express**: `Test-Path "${env:ProgramFiles(x86)}\IIS Express\iisexpress.exe"`
3. **Verify MSBuild**: The script auto-discovers MSBuild via VS 2026/2022/2019 or vswhere. Requires WebApplication targets (VS workload, not just BuildTools).
4. **Pre-build Blazor apps**: `dotnet build samples\AfterWingtipToys\WingtipToys.csproj` and `dotnet build samples\AfterContosoUniversity\ContosoUniversity.csproj`
5. **Check for port conflicts**: The script auto-clears ports 55502-55505, but verify manually if issues persist
6. **Check LocalDB** (for ContosoUniversity): `sqllocaldb info MSSQLLocalDB` — both Framework and Blazor apps share this database
7. **Verify ContosoUniversity DB exists**: `sqlcmd -S "(localdb)\MSSQLLocalDB" -d ContosoUniversity -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES"`

### Pre-compilation (Framework apps)

The `Start-FrameworkApp` function performs pre-compilation to avoid JIT timeouts under IIS Express:

1. **MSBuild pre-compilation**: Discovers MSBuild.exe (VS 2026 → 2022 → 2019 → 2017 → vswhere) and runs `msbuild /p:Configuration=Debug` against the `.csproj`. This compiles code-behind DLLs so IIS Express doesn't JIT on first request. **Requires WebApplication targets** — VS Enterprise/Professional or Web dev workload.

2. **ASP.NET view pre-compilation**: Uses `aspnet_compiler.exe -v / -p <sitepath>` to pre-compile ASPX views. Falls back gracefully if it fails.

3. **LocalDB auto-start**: Runs `sqllocaldb start MSSQLLocalDB` for EF6 database connectivity.

4. **EF6/EF Core table rename**: ContosoUniversity Framework needs `dbo.Enrollment` (singular per EDMX), but the shared DB has `dbo.Enrollments` (plural per EF Core). Script renames before/after Framework benchmark.

All three steps are non-fatal — if any tool is missing or fails, the script logs a warning and continues. IIS Express will fall back to on-demand compilation.

### Interpreting results

- **Cold start** varies wildly between runs (JIT, DB init). Compare warm averages for steady-state performance.
- **P95** is more stable than max — use it for latency SLA discussions.
- **Response size** differs structurally: Framework returns complete HTML; Blazor returns a shell + SignalR bootstrap. Don't compare sizes directly.
- Focus on **relative speedup ratios** (Framework avg / Blazor avg), not absolute numbers.
- A speedup > 1.0x means Blazor is faster; < 1.0x means Framework is faster.

### Re-running after fixes

After fixing a build error or timeout issue:
1. Delete stale results: `Remove-Item dev-docs/benchmarks/benchmark-results.json`
2. Re-run: `pwsh -File scripts/Run-Benchmarks.ps1 -Iterations 50`
3. Commit updated results: `git add dev-docs/benchmarks/ && git commit -m "perf: Update benchmark results"`

## Anti-Patterns

- **Don't compare response sizes between Framework and Blazor** — they serve fundamentally different HTML structures (complete page vs Blazor shell).
- **Don't run benchmarks on battery power** — CPU throttling skews results.
- **Don't skip the cold-start measurement** — it captures real-world first-visit latency including JIT and DB initialization.
- **Don't benchmark with other intensive processes running** — close IDE, browsers, etc. for clean numbers.
- **Don't treat single-run results as authoritative** — run at least 3 times and use the median report.
