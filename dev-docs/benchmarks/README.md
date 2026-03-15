# Performance Benchmarks

Automated benchmark suite comparing original .NET Framework Web Forms sample apps
with their migrated .NET 10 Blazor equivalents.

## What Is Measured

| Metric | Description |
| --- | --- |
| **Cold start** | First HTTP response time after the app is launched (includes JIT, startup) |
| **Warm average** | Mean response time over 50 sequential requests |
| **P95** | 95th-percentile response time from warm requests |
| **Response size** | HTTP response body size in bytes |
| **Status code** | HTTP status returned by the page |

All requests are plain HTTP using `Invoke-WebRequest -UseBasicParsing` (no JavaScript execution).

## How to Run

### Full benchmark (Framework + Blazor)

```powershell
pwsh scripts/Run-Benchmarks.ps1
```

### Blazor-only (skip .NET Framework / IIS Express)

```powershell
pwsh scripts/Run-Benchmarks.ps1 -SkipFramework
```

### Dry run (validate without executing)

```powershell
pwsh scripts/Run-Benchmarks.ps1 -WhatIf
```

### Custom iterations and output

```powershell
pwsh scripts/Run-Benchmarks.ps1 -Iterations 100 -OutputPath ./my-results
```

### Generate report from existing results

```powershell
pwsh scripts/Generate-BenchmarkReport.ps1 -InputPath dev-docs/benchmarks/benchmark-results.json
```

## Output Files

| File | Description |
| --- | --- |
| `benchmark-results.json` | Raw results (machine-readable) |
| `performance-report.md` | Full Markdown report with tables and analysis |
| `response-times-wingtiptoys.svg` | Bar chart — WingtipToys comparison |
| `response-times-contosouniversity.svg` | Bar chart — ContosoUniversity comparison |

## Interpreting Results

- **Focus on relative comparisons**, not absolute numbers. Results vary by machine,
  background load, and whether antivirus is active.
- **Cold-start times** for Blazor apps include `dotnet run` compilation on first launch.
  Production deployments (published apps) will be faster.
- **Response size differences** are expected: Blazor returns an interactive shell with
  SignalR scripts, while Web Forms returns fully-rendered HTML.
- A speedup ratio > 1 means Blazor is faster; < 1 means Framework is faster.

## Known Limitations

- **Single-client sequential requests** — no concurrent-load testing.
- **Server-side only** — no browser rendering, Time-to-Interactive, or Largest Contentful
  Paint measurements.
- **.NET Framework apps require IIS Express** (Windows + Visual Studio workload).
  If unavailable, those results appear as N/A.
- Blazor apps may require database setup for pages that read data. Pages that fail are
  reported with error details.

## Prerequisites

| Requirement | For |
| --- | --- |
| .NET 10 SDK | Blazor apps |
| PowerShell 7+ (pwsh) | Benchmark scripts |
| IIS Express | .NET Framework apps (optional — use `-SkipFramework` to skip) |
| nuget.exe (repo root) | NuGet restore for Framework apps |
