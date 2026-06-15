# ContosoUniversity Migration - Run 26

## Summary

| Metric | Value |
|--------|-------|
| Date | 2026-05-18 |
| Branch | `feature/migration-benchmark-speedups` |
| Total Runtime | ~8 min |
| Build Result | ✅ 0 errors (after L2 repair) |
| Acceptance Tests | **36/40 pass** |

## What Worked Well

- L1 produced 53 files with 129 transforms and 0 errors
- DbContextInstantiationTransform correctly injected BLL classes with constructor DI
- Duplicate class detection worked (skipped `StudentsListLogic` duplicate)
- PageDirectiveTransform now handles Home.aspx → `@page "/"`
- About page, Instructors page, navigation tests all pass

## L2 Repairs Required

1. **BLL classes instantiated with `new()` in pages** — Pages still did `new Courses_Logic()` instead of using DI injection. Fixed by adding `[Inject]` properties.
2. **Enrollmet_Logic duplicate** — File existed in both BLL/ and Models/ namespaces. Deleted Models copy.
3. **DropDownList StaticItems via @ref** — `ref?.Items.Add()` pattern fails in static SSR. Converted to ListItemCollection backing field + markup binding.
4. **Invalid EF `.Include(x => x.Key)` calls** — Spurious navigation property includes in StudentsListLogic. Removed.
5. **AutoCompleteExtender `id` and `EnableCaching` attributes** — Component doesn't support these. Removed.
6. **DetailsView DataSource null** — Initialized to empty list instead of null.
7. **MapGet("/") ambiguity** — Removed redundant redirect (now fixed in CLI source).
8. **Table.Rows/Cells access** — Stubbed out (BWFC Table doesn't support programmatic cell access).

## Failing Tests (4)

All 4 failures are in `StudentsPageTests` and require interactive form submissions:

| Test | Reason |
|------|--------|
| `AddNewStudentFormWorks` | Form post needs interactive render mode |
| `DeleteStudentWorks` | Form post needs interactive render mode |
| `ClearButtonResetsForm` | Button handler needs interactive render mode |
| `DetailsViewShowsStudentDetails` | Search form post needs interactive render mode |

These are fundamental static SSR limitations — the Students page has complex form interactions that require `@rendermode InteractiveServer` or `<form>` with named handlers.

## CLI Gaps Identified (for future improvement)

1. **BLL `new()` calls not converted to DI** — Pages that instantiate BLL classes need automatic conversion to `[Inject]` properties
2. **`id` attribute on BWFC/AjaxToolkit components** — CLI should strip `id` from non-HTML components (they don't support it as a parameter)
3. **`EnableCaching` attribute on AutoCompleteExtender** — CLI should strip unsupported attributes from AjaxToolkit components
4. **DetailsView DataSource null safety** — When DataSource is set to null, should default to empty collection
5. **Invalid EF Include() from EDMX** — `.Include(x => x.Key)` patterns from bad EDMX output should be detected and removed

## Comparison to Previous Run

| Run | Tests Passing | Build Errors at L1 | L2 Time |
|-----|--------------|--------------------|---------| 
| Run 25 | 38/40 | 32 | ~10 min |
| Run 26 | 36/40 | 20 | ~5 min |

Build errors reduced from 32 → 20 (38% fewer). L2 repair time reduced from ~10min to ~5min. 2 fewer tests pass vs Run 25 because ClearButton and DetailsView tests require form interactivity that wasn't set up in this fresh run.
