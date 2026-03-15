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
| AfterWingtipToys | .NET 10 | 55504 | `dotnet run` | ❌ **Build error**: `UserLoginInfo` missing in `Account/ManageLogins.razor` — Identity API migration gap. Must fix before benchmarking. |
| ContosoUniversity | .NET 4.5.2 | 55503 | IIS Express | ⚠️ **Timeout**: Needs SQL Server LocalDB running + longer startup timeout (30s may be insufficient for EF6 first-run). Try increasing `Wait-ForEndpoint -TimeoutSeconds 60`. |
| AfterContosoUniversity | .NET 10 | 55505 | `dotnet run` | ✅ Works — uses EnsureCreated() for SQLite DB. Cold start for Students page is slow (1763ms) due to DB initialization. |

## Fixing Known Issues

### AfterWingtipToys build error

The `Account/ManageLogins.razor` page references `UserLoginInfo` which doesn't exist in the .NET 10 Identity packages. Options:
1. **Stub the type**: Add a `UserLoginInfo` record/class to the project's Models folder
2. **Remove the page**: If `ManageLogins` isn't needed for benchmarking, exclude it from the build
3. **Fix the migration**: Replace with the .NET 10 Identity equivalent (`Microsoft.AspNetCore.Identity.UserLoginInfo` exists in `Microsoft.AspNetCore.Identity.Core`)

### ContosoUniversity IIS Express timeout

ContosoUniversity uses Entity Framework 6 with SQL Server. On first run:
1. Ensure SQL Server LocalDB is running: `sqllocaldb start MSSQLLocalDB`
2. The 30s timeout in `Start-FrameworkApp` may be too short — increase to 60s
3. If LocalDB isn't available, this app cannot run

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
3. **Pre-build Blazor apps**: `dotnet build samples\AfterWingtipToys\WingtipToys.csproj` and `dotnet build samples\AfterContosoUniversity\ContosoUniversity.csproj` — fix any build errors BEFORE running benchmarks
4. **Check for port conflicts**: Ensure ports 55502-55505 are free
5. **Check LocalDB** (for ContosoUniversity Framework): `sqllocaldb info MSSQLLocalDB`

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
