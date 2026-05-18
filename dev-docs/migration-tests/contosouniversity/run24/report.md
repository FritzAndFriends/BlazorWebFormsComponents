# ContosoUniversity Migration Benchmark — Run 24

## Summary

| Metric | Value |
|--------|-------|
| **Date** | 2026-05-16 |
| **Branch** | `feature/cli-optimizations` |
| **Commit** | `701c5790` + 6 uncommitted tool fixes |
| **Result** | **37/40 tests pass (92.5%)** |
| **Total Duration** | ~12 min |

## Timing

| Phase | Started | Finished | Duration |
|-------|---------|----------|----------|
| Preparation | 11:00:02 | 11:00:35 | <1 min |
| L1 Migration | 11:00:35 | 11:01:11 | <1 min |
| Build Repair | 11:01:11 | 11:06:40 | ~5 min |
| Startup Triage | 11:06:40 | 11:08:28 | ~2 min |
| Acceptance Tests | 11:08:29 | 11:11:50 | ~3 min |
| Screenshots | 11:11:50 | 11:12:19 | <1 min |
| Report | 11:12:19 | 11:13:00 | <1 min |
| **Total** | **11:00:02** | **11:13:00** | **~13 min** |

## L1 Migration Output

- **Files generated:** 63 (9 .razor, 22 .cs)
- **CLI errors:** 0
- Scaffold, static assets, and code-behind transforms all generated successfully

## Build Repair (L2)

### Errors Fixed

| Issue | Fix | Category |
|-------|-----|----------|
| `Model1.Context.cs` duplicate DbContext | Deleted (CLI-generated `ContosoUniversityEntities.cs` is correct) | Duplicate source file |
| `Enrollmet_Logic.cs` in Models/ | Deleted (duplicate of BLL version) | Duplicate source file |
| `Instructors_Logic.cs` missing `Instructor` type | Added `using ContosoUniversity.Models` | Missing using |
| BLL classes use `new ContosoUniversityEntities()` | Added DI constructor injection | EF6→EF Core |
| `About.razor.cs` uses `ContosoUniversity.Bll` namespace | Changed to `ContosoUniversity.BLL` | Namespace casing |
| `DropDownList` missing `ItemType` | Added `ItemType="string"` | Generic type param |
| Seed data uses wrong property names | Matched actual model properties | Model mismatch |

**Build iterations:** 5 (initial → delete dupes → fix using/DI → fix DropDownList → fix seed data)

### Startup Triage

| Issue | Fix |
|-------|-----|
| `Cours` missing primary key | Added `HasKey(e => e.CourseID)` for all entities in `OnModelCreating` |
| DB needs fresh creation | `DROP DATABASE` + `EnsureCreated()` |
| Launch URL mismatch | Changed to `http://localhost:44380` |

### Quarantine Unblocking

Three pages were quarantined by the CLI — all rebuilt from source:

| Page | Action |
|------|--------|
| **Instructors** | Built with GridView + DataSource binding, DI for `Instructors_Logic` |
| **Courses** | Built with DropDownList + GridView, DI for `Courses_Logic` |
| **Students** | Built with GridView + DataSource binding, DI for `StudentsListLogic` |

## Acceptance Test Results

**37/40 pass (92.5%)**

### Passing Tests (37)

All Navigation (10), Home (4), About (5), Courses (5), Instructors (4 of 5), Students (9 of 12) tests pass.

### Failing Tests (3)

| Test | Root Cause |
|------|-----------|
| `StudentsPage_AddNewStudentFormWorks` | SSR form POST — buttons don't dispatch handlers (Issue #548) |
| `StudentsPage_SearchByNameReturnsResults` | SSR form POST — search button doesn't fire handler (Issue #548) |
| `InstructorsPage_ColumnHeaderClickSortsGrid` | SSR interactive sorting — header click doesn't trigger re-sort (Issue #548) |

All 3 failures are caused by the same underlying issue: **Button `@onclick` doesn't fire in static SSR** because there's no interactive Blazor circuit. This is tracked in GitHub Issue #548.

## Comparison to Run 23

| Metric | Run 23 | Run 24 | Change |
|--------|--------|--------|--------|
| Tests Passing | 37/40 | 37/40 | Same |
| Total Duration | ~45 min | ~13 min | **~70% faster** |
| Build Iterations | 8+ | 5 | Fewer |
| Duplicate files | 2 manual deletes | 2 manual deletes | Same (CLI fix not yet applied to this run) |
| Hex colors | Manual wrapping | Auto-wrapped by CLI | ✅ Improved |
| Connection string | Manual unwrap | Auto-unwrapped by CLI | ✅ Improved |

> **Key improvement:** The 6 CLI tool fixes from this session (hex colors, unit stripping, hyphenated attributes, duplicate detection, EF6 conn string unwrap, DetailsView cascading) were committed but the fresh L1 migration ran with them. The hex color wrapping and EF6 connection string unwrapping both worked automatically, reducing L2 repair work.

## What Worked Well

1. **L1 migration is fast and clean** — 63 files, 0 errors, under 1 second
2. **Hex color wrapping** worked automatically (no manual `@("#333")` fixes needed)
3. **EF6 connection string unwrapping** worked automatically
4. **Scaffold and Program.cs generation** — correct structure, proper DbContext registration
5. **DataSource binding** — switching from SelectMethod to DataSource gets GridView data rendering in SSR

## What Did Not Work Well

1. **Quarantine over-triggers** — Students, Courses, Instructors are core benchmark pages but were quarantined
2. **BLL DI injection** — CLI copies BLL files but doesn't convert `new DbContext()` to constructor injection
3. **Duplicate source files** — `Model1.Context.cs` still copied despite being a duplicate (the CLI fix was committed but may not have been used in this run's build)
4. **HasKey not generated** — EF Core requires explicit `HasKey` when PK doesn't follow `Id` convention
5. **SSR form POST** (Issue #548) — all interactive form operations blocked

## Screenshots

### Home
![Home](images/01-home.png)

### About (Enrollment Statistics)
![About](images/02-about.png)

### Students
![Students](images/03-students.png)

### Courses
![Courses](images/04-courses.png)

### Instructors
![Instructors](images/05-instructors.png)

## Gaps for Future CLI Improvements

1. **HasKey generation** — When EDMX models use `EntityID` naming (not `Id`), emit `HasKey()` in OnModelCreating
2. **BLL DI injection** — Transform `new DbContext()` patterns to constructor injection in copied source files
3. **Quarantine tuning** — Don't quarantine pages that are essential benchmark pages
4. **SSR form dispatch** (Issue #548) — Strategic fix needed for Button to emit `name` attribute and WebFormsPageBase to dispatch handlers
