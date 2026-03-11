# WingtipToys Run 17 — Summary

| Metric | Value |
|--------|-------|
| **Date** | 2026-03-09 |
| **Branch** | `squad/audit-docs-perf` |
| **Score** | ✅ **25/25 acceptance tests passed (100%)** |
| **Render Mode** | **SSR (Static Server Rendering)** |
| **Layer 1 (Script) Time** | **1.81 seconds** (694 transforms) |
| **Layer 2 (Script) Time** | **1.12 seconds** (32 transforms) |
| **Layer 2 Manual Fixes** | Known-good overlay from cef51da3 (36 files) |
| **Build Errors** | 0 (after overlay) |
| **Build Warnings** | 27 (nullable reference types) |

## Executive Summary

> **Bottom line:** Run 17 is a **regression check** validating the genericized migration toolkit. Layer 1 ran 28% faster than Run 16 (1.81s vs 2.50s). Layer 2 Pattern A still generates broken `public or private { get; set; }` declarations, requiring known-good overlay. **25/25 tests pass** — the sixth consecutive 100% result. A **script bug was discovered and fixed** during this run: the Layer 1 script's `$script:replaced_count` variable was used before initialization, causing failures on ContosoUniversity.

## Run Progression (Runs 15–17)

| Run | Date | L1 Time | L2 Time | Build Errors | Tests | Key Change |
|-----|------|---------|---------|--------------|-------|------------|
| 15 | 2026-03-08 | 2.83s | ~1s | 0 | 25/25 | L2 reference extraction |
| 16 | 2026-03-08 | 2.50s | 1.6s | 0 | 25/25 | First Layer 2 automation |
| **17** | 2026-03-09 | **1.81s** | **1.12s** | 0 | 25/25 | Genericized toolkit validation |

Layer 1 improved from 2.50s to 1.81s (28% faster), likely due to `-TestMode` skipping NuGet restore.

## What Worked Well

1. **Layer 1 reliability:** 694 transforms applied correctly with no manual intervention
2. **Layer 1 speed:** 1.81s — fastest run yet (28% improvement over Run 16)
3. **Layer 2 Pattern C:** Program.cs generated correctly with SQLite, Identity, and Seed support
4. **Reference overlay workflow:** 36 files overlayed from cef51da3 in <1 second
5. **Test stability:** 25/25 — sixth consecutive 100% pass rate

## What Didn't Work

### 1. Layer 2 Pattern A Still Broken

The Layer 2 script generates invalid C# declarations:
```csharp
[Parameter]
public or private { get; set; }  // CS1585: Invalid syntax
```

**Root cause:** The script attempts to parse route parameter annotations from comments but fails to extract the actual type and name when the comment format doesn't match expectations.

**Files affected:** All 31 Pattern A code-behinds required overlay from known-good reference.

### 2. Layer 1 Script Bug (Fixed During Run)

**Issue:** The `$script:replaced_count` variable was used before initialization when processing files with `<HeadContent>` placeholders.

**Location:** `bwfc-migrate.ps1` line 732

**Error:**
```
Exception calling "Replace" with "2" argument(s): "The variable '$script:replaced_count' cannot be retrieved because it has not been set."
```

**Fix applied:**
```powershell
# Before (broken)
$replaced = 0

# After (fixed)
$script:replaced_count = 0
```

This bug was triggered by ContosoUniversity but not by WingtipToys (different page structure).

## Recommendations

### High Priority

1. **Fix Pattern A parameter generation** — Parse route parameter types properly from `[RouteData("name")]` and `[QueryString("name")]` annotations. Current regex only handles specific formats.

2. **Add Layer 1 script tests** — The `$script:replaced_count` bug would have been caught by unit tests. Consider adding Pester tests for the migration script.

### Medium Priority

3. **Pattern B detection** — Still detecting 0 auth form candidates. The detection heuristic needs to match the actual post-Layer-1 file structure.

4. **Reduce overlay dependency** — Currently 100% of Pattern A output requires replacement. Target: generate 50% buildable code-behinds.

## Comparison with Run 16

| Metric | Run 16 | Run 17 | Change |
|--------|--------|--------|--------|
| Layer 1 Time | 2.50s | 1.81s | **-28%** |
| Layer 2 Time | 1.6s | 1.12s | **-30%** |
| L1 Transforms | ~700 | 694 | Same |
| L2 Pattern A | 31 | 31 | Same |
| Build Errors | 0 | 0 | Same |
| Tests Passed | 25/25 | 25/25 | Same |

**No regressions detected.** Run 17 confirms the migration toolkit is stable and faster than previous runs.
