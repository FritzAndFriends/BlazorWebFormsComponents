# Decision: ContosoUniversity L2 Structural Transform

**Date:** 2026-03-12
**Agent:** Cyclops (Component Dev)
**Status:** Complete

## Context

The AfterContosoUniversity sample needed Layer 2 structural transforms to convert L1-scaffolded Web Forms code-behinds, models, and markup into working Blazor components with EF Core, proper BWFC component usage, and DI patterns.

## Decision

Converted all 5 page files, 5 models, and infrastructure (Program.cs, csproj, _Imports.razor) from Web Forms patterns to Blazor/EF Core. Used SQLite instead of SQL Server LocalDB for .NET 10 preview compatibility.

## Key Choices

1. **SQLite over SQL Server** — `Microsoft.EntityFrameworkCore.Sqlite` chosen because SqlServer package had resolution issues on .NET 10 preview. SQLite is simpler for a sample app anyway.

2. **IDbContextFactory pattern** — All BLL classes use `IDbContextFactory<ContosoUniversityContext>` with `using var context = _dbFactory.CreateDbContext()` instead of injecting DbContext directly. This matches the WingtipToys L2 pattern.

3. **Items binding over SelectMethod** — All data-bound controls use `Items="@_data"` loaded in `OnInitialized()` rather than `SelectMethod` delegates. Simpler and doesn't require understanding SelectHandler signature.

4. **Enrollmet_Logic in BLL namespace only** — Moved to BLL/ folder alongside other logic classes to avoid namespace ambiguity. Models/ folder contains only entity classes and DbContext.

5. **BWFC style sub-components** — Used `<RowStyleContent><GridViewRowStyle .../></RowStyleContent>` pattern, removed unsupported SortedAscending/Descending styles.

## Build Result

0 errors, 0 ContosoUniversity-specific warnings.
