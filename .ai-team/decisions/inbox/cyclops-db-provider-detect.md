# Decision: L1 Script Auto-Detects Database Provider from Web.config

**Date:** 2026-03-12
**Decided by:** Cyclops (requested by Jeffrey T. Fritz)
**Status:** Implemented

## Context

The L1 migration script (`bwfc-migrate.ps1`) previously hardcoded `Microsoft.EntityFrameworkCore.SqlServer` when scaffolding the EF Core package reference. This was correct for most Web Forms apps but wrong for projects using SQLite, PostgreSQL, or MySQL.

## Decision

Added `Find-DatabaseProvider` function that parses the source project's `Web.config` `<connectionStrings>` to detect the actual database provider and scaffold the matching EF Core package.

### Detection Order (three-pass)

1. **Explicit `providerName`** — `System.Data.SqlClient` → SqlServer, `System.Data.SQLite` → Sqlite, `Npgsql` → PostgreSQL, `MySql.Data.MySqlClient` → MySQL
2. **Connection string content** — `(LocalDB)` or `Server=` → SqlServer, `Data Source=*.db` → Sqlite
3. **EntityClient inner provider** — Extracts `provider=` from EF6 EDMX connection strings
4. **Fallback** — SqlServer if no Web.config or no connectionStrings found

### What Gets Scaffolded

- **csproj**: Detected EF Core package (e.g., `Microsoft.EntityFrameworkCore.SqlServer`)
- **Program.cs**: Commented-out `AddDbContextFactory` with detected provider method and actual connection string
- **Summary**: `[DatabaseProvider]` review item so L2 agents see the detection result

## Impact

- L2 agents now see the correct provider and connection string — no guessing
- Falls back safely to SqlServer for projects without Web.config
- Tested against ContosoUniversity sample (detects SQL Server via `(LocalDB)` pattern)
