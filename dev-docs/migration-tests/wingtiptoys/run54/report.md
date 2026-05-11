# WingtipToys Migration Benchmark — Run 54

## Summary

| Metric | Value |
|--------|-------|
| **Date** | 2026-05-09 |
| **Branch** | `feature/cli-optimizations` |
| **Key Change** | Template-based ProgramCsEmitter refactor |
| **L1 Migration Time** | 16.2 seconds |
| **L2 Build Repair** | ~6.5 minutes |
| **L2 Page Repair** | ~5.5 minutes + iterative manual fixes |
| **Total Wall Clock** | ~45 minutes (including terminal interruption) |
| **Initial Build Errors** | 31 |
| **Final Test Result** | **25/25 passing** |

## What Changed Since Run 53

- **ProgramCsEmitter refactored** to template-based architecture with 4 sections:
  - `EmitUsings`, `EmitBuilderSection`, `EmitServiceRegistrations`, `EmitAppBuildAndStartup`
  - Generates a proper Blazor SSR Program.cs baseline from dotnet template content
  - Layers features (EF, Identity, static files, antiforgery) onto the standard baseline

## L2 Repair Issues Encountered

### 1. LocalDB Connection Strings (CLI Gap)
- `WebConfigTransformer` copies LocalDB connection strings verbatim from Web.config
- `(LocalDB)\MSSQLLocalDB` with `AttachDbFilename` never works without SQL Server installed
- **Fix needed**: ProgramCsEmitter should detect LocalDB and auto-convert to SQLite

### 2. Seed Data NOT NULL Constraints
- Category model requires `Description` but seed data omitted it
- SQLite is strict about NOT NULL — SQL Server was lenient
- **Fix needed**: Seed data generation should inspect model constraints

### 3. MainLayout Missing Navbar
- Layout scaffold produced `@Body` only — no navigation structure
- Original `Site.Master` had full Bootstrap navbar with nav links
- **Fix needed**: CLI should extract nav links from Site.Master and scaffold navbar

### 4. AddToCart Page Empty
- `AddToCart.aspx` migration produced empty HTML shell
- Original had code-behind calling `ShoppingCartActions` and redirecting
- **Fix needed**: Detect cart mutation pattern and generate working redirect page

### 5. ProductDetails Missing AddToCart Link
- Original ProductDetails had link to `AddToCart.aspx?productID=X`
- Migrated version dropped the link during markup transform
- **Fix needed**: Preserve AddToCart href in data control templates

### 6. Homepage Styling (navbar `.container` match)
- Test locator `.jumbotron, .carousel, [role='main'], main, .container` matched navbar container first
- Fixed by using `container-fluid` in navbar (doesn't affect functionality)

## Test Progression

| Round | Passing | Key Fix |
|-------|---------|---------|
| 1 | 7/25 | Baseline after build repair |
| 2 | 20/25 | MainLayout navbar + page routing + seed data |
| 3 | 21/25 | Register page UserName field removal |
| 4 | 24/25 | AddToCart link on ProductDetails |
| 5 | **25/25** | `container-fluid` in navbar layout |

## CLI Improvements Identified

1. **P1**: Auto-convert LocalDB connection strings to SQLite in ProgramCsEmitter
2. **P2**: Extract Site.Master navbar structure into MainLayout scaffold
3. **P3**: Generate working AddToCart redirect pages (cart mutation pattern)
4. **P4**: Preserve AddToCart hrefs in data control template migration
5. **P5**: Add `EnsureCreated()` bootstrap when EF contexts are detected
6. **P6**: Seed data should respect NOT NULL model constraints
