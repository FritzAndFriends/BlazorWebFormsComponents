# Analyzer NuGet Integration + Prescan Switch

**Date:** 2026-03-20
**Author:** Cyclops

## Decision 1: Analyzer bundled into main NuGet package

Added a `ProjectReference` with `OutputItemType="Analyzer"` from `BlazorWebFormsComponents.csproj` → `BlazorWebFormsComponents.Analyzers.csproj`. This means consumers who reference `Fritz.BlazorWebFormsComponents` automatically get all BWFC Roslyn analyzers — no separate package install needed.

**Rationale:** Single NuGet reference gives migration guidance out of the box. The analyzer DLL ships in `analyzers/dotnet/cs` inside the package, not as a library dependency.

## Decision 2: `-Prescan` switch uses same rule IDs as Roslyn analyzers

The `Invoke-BwfcPrescan` function in `bwfc-migrate.ps1` uses BWFC001–BWFC014 rule IDs matching the Roslyn analyzer diagnostic IDs. This creates a consistent vocabulary between the PowerShell pre-scan (for quick assessment) and the IDE-based Roslyn analyzers (for precise in-editor guidance).

**Rationale:** Teams can triage a codebase with `-Prescan` first, then drill into specific BWFC0XX rules in Visual Studio.
