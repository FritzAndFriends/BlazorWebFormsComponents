# WingtipToys Migration Test - Run 92

**Date:** 2026-05-19 08:31 EDT  
**Branch:** `feature/migration-benchmark-speedups`  
**Operator:** Copilot  
**Requested by:** @csharpfritz

---

## Summary

| Metric | Value |
|--------|-------|
| Source project | `samples/WingtipToys/WingtipToys` |
| Output project | `samples/AfterWingtipToys` |
| Toolkit entry point | `migration-toolkit/scripts/bwfc-migrate.ps1` |
| Report folder | `dev-docs/migration-tests/wingtiptoys/run92` |
| Total wall-clock time | ~4 min |
| Build result | 0 errors, 0 warnings |
| Acceptance tests | **26/26 passed** |
| Final status | **SUCCESS** |

## Executive Summary

WingtipToys migrated cleanly from scratch with only 3 L2 fixes required (missing `using` directives, a variable name mismatch, and a static/instance field conflict). The toolkit produced 204 files with 647 transforms and 0 errors. All 26 acceptance tests pass.

## Timing

| Phase | Duration | Notes |
|-------|----------|-------|
| Preparation | ~5s | Clear output, create report folder |
| Layer 1 toolkit migration | 24s | 29 files processed, 647 transforms, 0 errors |
| Layer 2 repair | ~3 min | 3 fixes: missing usings, variable rename, static field |
| Build validation | 4s | Clean build |
| Acceptance tests | 22s | 26/26 passed |
| **Total** | ~4 min | |

## Layer 1 Results

```
Files processed:    29
Files written:      204
Transforms applied: 647
Semantic patterns:  7
Scaffold files:     12
Static files:       80
Manual items:       35
Errors:             0
```

## Layer 2 Fixes Required

| # | File | Issue | Fix |
|---|------|-------|-----|
| 1 | `Logic/PayPalFunctions.cs`, `Account/Login.razor.cs`, `Account/Register.razor.cs` | Missing `using WingtipToys.Logic;` | Added using directive |
| 2 | `ShoppingCart.razor.cs` | `actions` variable not in scope (should be `usersShoppingCart`) | Renamed reference |
| 3 | `Logic/ExceptionUtility.cs` | Static method accessing instance field + invalid `HttpContext.Path` | Made field static, fixed path call |

## What Worked Well

- L1 toolkit produced a near-complete migration with 0 errors
- Only 3 trivial L2 fixes needed (all namespace/naming issues)
- All 26 acceptance tests passed on first run after fixes
- Identity, shopping cart, product catalog all functional

## What Did Not Work Well

- CLI still generates code referencing `actions` variable instead of the declared `usersShoppingCart` field
- `ExceptionUtility` static/instance mismatch not caught by CLI transforms
- Missing `using` directives for cross-namespace references within the same project

## Toolkit Gaps

1. **Cross-namespace using resolution** — When source files reference types in other project namespaces, the CLI should ensure the appropriate `using` directive is present
2. **Variable name consistency** — `ShoppingCart.razor.cs` references `actions` but the declared field is `usersShoppingCart`
3. **Static method access patterns** — `ExceptionUtility.LogException()` is static but accesses an instance field
