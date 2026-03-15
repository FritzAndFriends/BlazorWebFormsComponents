---
name: "performance-benchmarks"
description: "Run and re-run .NET Framework vs .NET 10 performance benchmarks, diagnose failures, generate reports with SVG charts"
domain: "performance-testing"
confidence: "medium"
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

| App | Framework | Port | Launch Method | Known Issues |
|-----|-----------|------|---------------|--------------|
| WingtipToys | .NET 4.5.2 | 55502 | IIS Express | ✅ Works — NuGet restore required |
| AfterWingtipToys | .NET 10 | 55504 | `dotnet run` | ✅ Fixed — added `@using Microsoft.AspNetCore.Identity` to resolve `UserLoginInfo` |
| ContosoUniversity | .NET 4.5.2 | 55503 | IIS Express | ✅ MSBuild pre-compilation handles JIT timeout. Script auto-starts LocalDB and pre-compiles with MSBuild + `aspnet_compiler.exe` before IIS Express launch. |
| AfterContosoUniversity | .NET 10 | 55505 | `dotnet run` | ✅ Works — uses EnsureCreated() for SQLite DB. Cold start for Students page is slow (1763ms) due to DB initialization. |

## Fixing Known Issues

### AfterWingtipToys build error (FIXED)

The `Account/ManageLogins.razor` page was missing a `@using Microsoft.AspNetCore.Identity` directive. This has been added and the app now builds successfully.

### ContosoUniversity IIS Express timeout (MITIGATED)

ContosoUniversity uses Entity Framework 6 with SQL Server. The `Start-FrameworkApp` function now:
1. Starts LocalDB automatically via `sqllocaldb start MSSQLLocalDB`
2. Pre-compiles the project with MSBuild before launching IIS Express
3. Pre-compiles ASP.NET views with `aspnet_compiler.exe`
4. Uses a 180s timeout to allow for remaining first-request initialization
5. Captures IIS Express stderr for diagnostics if the app fails to respond

**Remaining issue:** On this machine, IIS Express reports `Failed to register URL "http://localhost:55503/"` with error `0x800700b7` (port already registered). This is a Windows URL reservation conflict, not a JIT timeout. MSBuild pre-compilation also requires VS 2019+ with WebApplication targets — VS 2017 BuildTools lack them.

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
3. **Verify MSBuild**: The script auto-discovers MSBuild via VS 2022/2019 or vswhere. Manual check: `& "${env:ProgramFiles}\Microsoft Visual Studio\2022\*\MSBuild\Current\Bin\MSBuild.exe" -version`
4. **Verify aspnet_compiler.exe**: `Test-Path "$env:windir\Microsoft.NET\Framework64\v4.0.30319\aspnet_compiler.exe"`
5. **Pre-build Blazor apps**: `dotnet build samples\AfterWingtipToys\WingtipToys.csproj` and `dotnet build samples\AfterContosoUniversity\ContosoUniversity.csproj` — fix any build errors BEFORE running benchmarks
6. **Check for port conflicts**: Ensure ports 55502-55505 are free
7. **Check LocalDB** (for ContosoUniversity Framework): `sqllocaldb info MSSQLLocalDB`

### Pre-compilation (Framework apps)

The `Start-FrameworkApp` function performs three pre-compilation steps to avoid JIT timeouts under IIS Express:

1. **MSBuild pre-compilation**: Discovers MSBuild.exe (VS 2022 → VS 2019 → vswhere fallback) and runs `msbuild /p:Configuration=Debug` against the `.csproj`. This compiles C# code and references so IIS Express doesn't need to invoke the Roslyn compiler on first request.

2. **ASP.NET view pre-compilation**: Uses `aspnet_compiler.exe -v / -p <sitepath> -fixednames` from the .NET Framework directory to pre-compile `.aspx`, `.ascx`, and `.master` view files. This eliminates the per-page compilation delay on first visit.

3. **LocalDB auto-start**: Runs `sqllocaldb start MSSQLLocalDB` so EF6 apps can connect on first request without a connection timeout.

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
