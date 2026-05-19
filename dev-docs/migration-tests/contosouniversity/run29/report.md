# ContosoUniversity Migration Test - Run 29

**Date:** 2026-05-19 08:35 EDT  
**Branch:** `feature/migration-benchmark-speedups`  
**Operator:** Copilot  
**Requested by:** @csharpfritz

---

## Summary

| Metric | Value |
|--------|-------|
| Source project | `samples/ContosoUniversity/ContosoUniversity` |
| Output project | `samples/AfterContosoUniversity` |
| Toolkit entry point | `migration-toolkit/scripts/bwfc-migrate.ps1` |
| Report folder | `dev-docs/migration-tests/contosouniversity/run29` |
| Total wall-clock time | ~20 min |
| Build result | 0 errors |
| Acceptance tests | **40/40 passed** |
| Final status | **SUCCESS** |

## Executive Summary

ContosoUniversity migrated from scratch with L1 producing 53 files (139 transforms, 0 errors) in 7 seconds. L2 repair addressed ~19 build errors (namespace conflicts, BLL constructor wiring, LINQ issues) plus runtime fixes for static SSR form handling, database seeding, and search support. All 40 acceptance tests pass.

## Timing

| Phase | Duration | Notes |
|-------|----------|-------|
| Preparation | ~5s | Clear output, create report folder |
| Layer 1 toolkit migration | 7s | 5 files processed, 139 transforms, 0 errors |
| Layer 2 build repair | ~5 min | 19 errors: namespace conflicts, BLL constructors, LINQ .Key |
| Layer 2 runtime fixes | ~16 min | Form POST handling, seed data, search, GridView Items binding |
| Build validation | 4s | Clean build |
| Acceptance tests | 45s | 40/40 passed |
| **Total** | ~20 min | |

## Layer 1 Results

```
Files processed:    5
Files written:      53
Transforms applied: 139
Semantic patterns:  1
Scaffold files:     9
Static files:       18
Manual items:       9
Errors:             0
```

## Layer 2 Fixes Required

### Build Fixes
| # | Issue | Fix |
|---|-------|-----|
| 1 | Duplicate `Enrollmet_Logic` class (Models/ vs BLL/) | Deleted Models copy |
| 2 | BLL constructors need `ContosoUniversityEntities` param | Passed injected context |
| 3 | `.Key` LINQ references on entity objects | Replaced with `StudentID`/`CourseID` |
| 4 | `DropDownList` items `.Add()` on `IEnumerable<object>` | Changed to `List<ListItem>` |
| 5 | `btnSearchCourse_Click` signature mismatch | Fixed to parameterless |
| 6 | Dead `grv_RowUpdating` code using `Table.Rows`/`Cells` | Removed dead code |

### Runtime Fixes
| # | Issue | Fix |
|---|-------|-----|
| 1 | No seed data → empty GridViews | Added full seed in Program.cs |
| 2 | Form POST not handled in static SSR | Added SupplyParameterFromForm + action dispatch |
| 3 | TextBox names have naming-container prefix | Used `tabAddStud$fieldName` in form binding |
| 4 | Single-word search fails | Added Contains-based fallback in GetStudents |
| 5 | About page GroupBy fails in EF Core | Switched to client-side evaluation |
| 6 | GridView SelectMethod not working | Converted to Items binding |
| 7 | Buttons don't work in static SSR | Replaced with `<input type="submit">` |

## What Worked Well

- L1 produced correct page structure and routing
- Markup transforms handled control conversion cleanly
- WebFormsForm wrapping generated correct form structure
- BLL business logic preserved without rewriting

## What Did Not Work Well

- Duplicate source file copy (Enrollmet_Logic in two locations)
- BLL constructor wiring not automated
- Static SSR form handling requires significant manual work
- GridView SelectMethod→Items conversion not automated
- Database seeding is entirely manual

## Toolkit Gaps

1. **BLL constructor DI wiring** — CLI should detect constructor params and wire injected context
2. **Static SSR form POST automation** — Button→submit conversion and SupplyParameterFromForm generation should be a CLI transform
3. **Naming container prefix detection** — Form field names include parent container IDs
4. **SelectMethod→Items conversion** — Should be automated when SelectMethod pattern is detected
5. **Duplicate source detection** — Same class in multiple locations should be deduplicated
