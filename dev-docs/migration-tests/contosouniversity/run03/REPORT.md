# ContosoUniversity Run 03 — Generic Fixes + EF Scaffolding

| Metric | Value |
|--------|-------|
| **Date** | 2026-03-08 |
| **Branch** | `squad/audit-docs-perf` |
| **Test Score** | 0/40 (tests require running app + page migrations) |
| **Render Mode** | N/A (scaffolding phase) |
| **Total Time** | ~15 minutes |

## Executive Summary

This run focused on implementing **generic migration fixes** based on Run 02 recommendations and **scaffolding EF Core models** from the ContosoUniversity database. The build now passes with 0 errors, but acceptance tests cannot run until page migrations are applied.

## What Was Accomplished

### Generic Migration Fixes (Layer 1 + Layer 2)

| Fix | Script | Description | Commit |
|-----|--------|-------------|--------|
| Relative `.aspx` href conversion | Layer 1 | `href="Page.aspx"` → `href="/Page"` | `413c55b1` |
| Button OnClick handler | Layer 1 | `OnClick="Handler"` → `@onclick="Handler"` | `413c55b1` |
| Parameter parsing | Layer 2 | Handles method params AND class property syntax | `413c55b1` |
| TItem regex fix | Layer 2 | Added `\b` word boundary to prevent false matches | `630d0ac3` |

### EF Core Scaffolding

**Command used:**
```powershell
dotnet ef dbcontext scaffold "Server=(localdb)\MSSQLLocalDB;AttachDbFilename=D:\BlazorWebFormsComponents\samples\ContosoUniversity\ContosoUniversity.mdf;Database=ContosoUniversity;Trusted_Connection=True;MultipleActiveResultSets=true" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context-dir Data --context SchoolContext --force
```

**Files generated:**
- `Data/SchoolContext.cs` — EF Core DbContext with Fluent API
- `Models/Course.cs`, `Department.cs`, `Enrollment.cs`, `Instructor.cs`, `Student.cs`

**Key differences from EF6:**
| EF6 Pattern | EF Core Pattern |
|-------------|-----------------|
| `Cours` (typo in original) | `Course` (correct) |
| `StudentID` (uppercase) | `StudentId` (PascalCase) |
| `DateTime` | `DateOnly` for date columns |
| No nullable annotations | Proper `?` nullable annotations |

### Build Status

✅ **Build succeeds with 0 errors, 0 warnings**

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

## What Didn't Work

### Acceptance Tests (0/40)

All 40 tests fail with `net::ERR_CONNECTION_REFUSED` because:
1. The app isn't running during test execution
2. Tests point to `.aspx` URLs (designed for Web Forms, not Blazor)

**Example failing test:**
```
ContosoUniversity.AcceptanceTests.AboutPageTests.AboutPage_HasPageTitle
net::ERR_CONNECTION_REFUSED at http://localhost:44380/About.aspx
```

**To fix:** Tests need the migrated Blazor app running AND updated URLs (e.g., `/About` instead of `/About.aspx`).

## Timings

| Phase | Duration |
|-------|----------|
| Generic fix implementation | ~5 min |
| EF Core scaffolding | ~3 min |
| Build verification | ~2 min |
| Test execution (failed) | ~2 min |
| Documentation | ~3 min |
| **Total** | **~15 min** |

## Recommendations for Next Steps

### P0: Complete Page Migration
1. Run full Layer 1 + Layer 2 migration scripts on ContosoUniversity
2. Verify generated pages compile
3. Wire up EF Core models to page code-behinds

### P1: Update Acceptance Tests
1. Change test URLs from `.aspx` to Blazor routes
2. Ensure test fixture starts the Blazor app
3. Add `TestConfiguration` for AfterContosoUniversity port

### P2: Automate EF Scaffolding in Toolkit
The `AttachDbFilename` pattern works reliably for LocalDB `.mdf` files. Consider adding:
- Auto-detection of `.mdf` files in source project
- Optional `--scaffold-ef` flag to migration toolkit
- Connection string template generation

## Commits This Run

| SHA | Message |
|-----|---------|
| `413c55b1` | feat(migration): Add generic fixes for href, onclick, params |
| `630d0ac3` | fix(layer2): TItem regex word boundary + Run 03 setup |
| `a7134873` | feat(contoso): Add EF Core scaffolded models from database |

## See Also

- [Run 02 Report](contoso-run02.md) — Recommendations source
- [Migration Toolkit](../../migration-toolkit/README.md)
- [Generic Fixes Session Log](../log/2026-03-09-generic-migration-fixes.md)
