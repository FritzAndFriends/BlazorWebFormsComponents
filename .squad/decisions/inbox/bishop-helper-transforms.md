# Decision: Helper-class transforms for MapPath and self-instantiation

**Date:** 2026-05-17T19:20:41-04:00  
**Author:** Bishop  
**Status:** Proposed

## Decision

Teach the CLI helper-file path to fix two recurring non-page migration gaps automatically:

1. Extend `ServerShimTransform` so `FileType.CodeFile` inputs rewrite resolvable `Server.MapPath(...)` and `HttpContext.Current.Server.MapPath(...)` calls to `Path.Combine(AppContext.BaseDirectory, ...)`.
2. Tighten `SelfInstantiationTransform` so it only rewrites self-construction in DI-managed classes, then collapse `new CurrentClass()` helper patterns to `this` and unwrap `using` blocks that would otherwise dispose the current service instance.
3. Add both transforms to `SourceFileCopier` so copied `Logic/` and `BLL/` helper classes receive the same helper-gap repairs as page code-behind.

## Why

WingtipToys and ContosoUniversity both still needed manual Layer 2 cleanup in helper classes after otherwise successful CLI runs. The recurring pain points were path resolution in utilities like `ExceptionUtility` and DI-managed helpers like `ShoppingCartActions` constructing themselves again instead of reusing the current scoped instance.

Fixing these in the CLI removes two benchmark-repeatable manual edits and keeps the migration contract aligned with generated DI registration. The additional copier coverage matters because page-only transforms do not reach standalone `.cs` files.

## Verification

- Added focused transform tests for literal/nested `MapPath`, `HttpContext.Current.Server.MapPath`, literal-backed variable paths, DI-only self-instantiation rewrites, and no-op cases.
- Added `SourceFileCopier` tests proving helper-file rewrites run on copied `Logic/` classes.
- `dotnet test tests\BlazorWebFormsComponents.Cli.Tests --nologo` passes.
