# WingtipToys Migration Test — Run 24

**Date:** 2026-03-26  
**Branch:** `feature/l1-migration-scaffold-improvements`  
**Type:** L1 Quick Wins — EntityFramework shims, base class stripping, migration-mode suppression

## Summary

| Metric | Value |
|--------|-------|
| Source project | `samples/WingtipToys/WingtipToys` |
| Output project | `samples/AfterWingtipToys` |
| L1 script | `migration-toolkit/scripts/bwfc-migrate.ps1` |
| **Build errors (run 22 — baseline)** | **372** |
| **Build errors (run 23 — first improvements)** | **32** |
| **Build errors (this run)** | **3** |
| **Error reduction** | **99.2%** |

## Changes Since Run 23

### 1. EntityFramework Shims (BWFC Library)
Created `BlazorWebFormsComponents.EntityFramework` namespace with:
- **`DropCreateDatabaseIfModelChanges<TContext>`** — Stub for EF6 database initializer pattern. `protected virtual void Seed(TContext)` matches EF6's signature so `override` compiles.
- **`Database`** — Static stub with `SetInitializer<TContext>()` no-op.

Both marked `[Obsolete]` with guidance to use EF Core migrations.

### 2. Base Class Stripping in Code-Behinds
Updated `Copy-CodeBehind` to **strip** `: Page`, `: MasterPage`, `: UserControl` (both FQN and unqualified) from `partial class` declarations. Previously we replaced them with aliases, but this caused CS0263 conflicts since `.razor` files already have `@inherits WebFormsPageBase`.

### 3. Migration-Mode Warning Suppression
Updated `.targets` `BwfcMigrationMode` block to also suppress:
- **RZ9980/RZ9981** — Unclosed/unexpected HTML tags from un-converted `<% %>` blocks
- **RZ9996** — Unrecognized child content in BWFC templated components
- **CS0612** — `[Obsolete]` usage warnings for EF6 shims

### 4. `BwfcMigrationMode` in Generated .csproj
The L1 script now sets `<BwfcMigrationMode>true</BwfcMigrationMode>` in the generated project file, enabling migration-mode warning suppression automatically.

### 5. `.targets` EntityFramework Namespace
Added `<Using Include="BlazorWebFormsComponents.EntityFramework" />` to `.targets`.

## Remaining 3 Errors (Genuine L2)

| Error | File | Description |
|-------|------|-------------|
| CS0592 | `ProductList.razor.cs:31` | `[SupplyParameterFromQuery]` on method parameter — needs promotion to property |
| CS0592 | `ProductDetails.razor.cs:31` | Same — `[SupplyParameterFromQuery]` on method parameter |
| CS0305 | `ShoppingCart.razor.cs:95` | `GridViewRow` used without type argument — BWFC has `GridViewRow<T>` |

All three require semantic understanding for resolution (L2 tasks).

## Error Reduction Timeline

| Run | Errors | Change |
|-----|--------|--------|
| 22 (baseline) | 372 | — |
| 23 (first improvements) | 32 | −340 (91.4%) |
| **24 (this run)** | **3** | **−29 (99.2% from baseline)** |

## What Worked Well

1. **EF6 shim matching EF6 signatures** — `protected virtual Seed()` matches `protected override Seed()` in migrated code
2. **Base class stripping** — Eliminating `: Page` instead of aliasing it avoids CS0263 partial class conflicts
3. **Migration-mode suppression** — Auto-opting into RZ9980/RZ9981/RZ9996 suppression removes all Razor structural noise
4. **Correct source path** — Must point at inner project folder (`WingtipToys/WingtipToys`), not solution root

## What Needs Improvement

1. **`[SupplyParameterFromQuery]` promotion** — L1 script should detect `[QueryString]` on method params and generate a `[Parameter] [SupplyParameterFromQuery]` property on the class, not just replace the attribute in-place
2. **Non-generic `GridViewRow`** — Consider a BWFC type alias or non-generic wrapper
3. **Nested project detection** — Script should auto-detect when `$Path` is a solution folder containing a single project subfolder
4. **ProjectReference .targets import** — For local development, the generated .csproj should support importing `.targets` from a ProjectReference (currently requires manual `<Import>`)
