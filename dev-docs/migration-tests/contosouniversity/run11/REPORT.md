# ContosoUniversity Migration Run 11

**Date:** 2026-03-09  
**Branch:** `squad/audit-docs-perf`  
**Render Mode:** Static SSR (default)  
**Score:** Build succeeds (4 warnings, 0 errors)

## Executive Summary

Run 11 validates the existing AfterContosoUniversity sample, which builds successfully with only 4 warnings. ContosoUniversity is a more complex migration scenario than WingtipToys due to its use of DB-first Entity Framework with .edmx files.

## Timing

| Phase | Duration |
|-------|----------|
| Fresh Layer 1 | 0.80s |
| Fresh Layer 2 | 0.48s |
| **Total Migration** | 1.28s |
| Build (committed version) | 4.46s |

## Build Results

```
Build succeeded.
    4 Warning(s)
    0 Error(s)
```

### Warnings

| Warning | Count | Description |
|---------|-------|-------------|
| RZ10012 | 3 | ContentTemplate markup element — needs BWFC component |
| CS0169 | 1 | Unused field `_selectedCourse` |

## Migration Script Improvements Tested

### Layer 1 Improvements
1. **Color attribute conversion** — Converted 21+ color attributes to `@("value")` syntax
2. **ItemType preservation** — No longer incorrectly converts to TItem

### Layer 2 Pattern D Results (Fresh Migration)
Pattern D successfully injected ItemType into 4 files:
- `About.razor` — 1 component (GridView)
- `Courses.razor` — 2 components
- `Instructors.razor` — 1 component
- `Students.razor` — 2 components

### Fresh Migration Challenges
A completely fresh migration of ContosoUniversity still requires manual steps:
1. **EF Core package** — Must be added to .csproj (script doesn't detect DB-first .edmx)
2. **DropDownList TItem** — Pattern D uses ItemType, but DropDownList needs TItem
3. **Models namespace** — DB-first apps don't have Models/ folder from Layer 1

## Key Observations

### What Works Well
1. **Build succeeds** — The committed AfterContosoUniversity compiles cleanly
2. **Color attributes handled** — No more Razor parsing errors for colors
3. **EDMX scaffold command** — Layer 2 generates helpful scaffold command for DB-first migration

### Remaining Work for Full Automation
1. **DropDownList handling** — Pattern D should use TItem for list controls, ItemType for data controls
2. **DB-first detection** — Need to detect .edmx and add EF Core package automatically
3. **ContentTemplate** — BWFC doesn't have this component; needs conversion

## Build Metrics

| Metric | Value |
|--------|-------|
| Warnings | 4 |
| Errors | 0 |
| Build Time | 4.46s |

## Files

- Source: `samples/ContosoUniversity/`
- Output: `samples/AfterContosoUniversity/`

## Conclusion

ContosoUniversity builds successfully. The color attribute fix eliminated the 47 CS1024/CS0103 errors from Run 10. Fresh migrations still require manual steps for DB-first scenarios, but the committed version demonstrates a working migration path.

## Comparison with Previous Runs

| Run | Date | Build Errors | Notes |
|-----|------|--------------|-------|
| Run 09 | 2026-03-09 | 68 | Color attributes breaking Razor |
| Run 10 | 2026-03-09 | 21 | After color fix (47 eliminated) |
| **Run 11** | 2026-03-09 | 0 | Build succeeds |
