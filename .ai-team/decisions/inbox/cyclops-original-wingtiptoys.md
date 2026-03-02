# Decision: Original WingtipToys Build & Run Configuration

**Author:** Cyclops  
**Date:** 2026-03-02  
**Status:** Implemented

## Context

The original WingtipToys ASP.NET Web Forms application in `samples/WingtipToys/` needed to be built and run locally for screenshot capture. The app targets .NET Framework 4.5.2 with packages.config-style NuGet and uses SQL Server LocalDB.

## Decisions

1. **Connection strings:** `(LocalDb)\v11.0` → `(LocalDb)\MSSQLLocalDB` in `Web.config` (both connection strings and EF defaultConnectionFactory). This is machine-specific — any dev without a v11.0 LocalDB instance needs MSSQLLocalDB.

2. **NBGV isolation:** Created empty `samples/WingtipToys/Directory.Build.props` to block inheritance of repo-root NBGV and analyzer configuration. The WingtipToys project has its own `AssemblyInfo.cs` with version attributes that conflict with NBGV auto-generation.

3. **NuGet restore method:** For packages.config projects, use `nuget install <packages.config> -OutputDirectory <dir>` rather than `nuget restore <sln>`, which only generates PackageReference-style restore files.

4. **IIS Express:** Running on port 5200 via `iisexpress.exe /path:<dir> /port:5200`. No custom applicationhost.config needed.

## Impact

These changes are confined to `samples/WingtipToys/` and do not affect the main BlazorWebFormsComponents library or its tests.
