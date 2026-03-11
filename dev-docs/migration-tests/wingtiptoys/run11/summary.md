# WingtipToys Run 11 — Summary

| Metric | Value |
|--------|-------|
| **Date** | 2026-03-07 |
| **Branch** | `squad/run8-improvements` |
| **Score** | ❌ **17/25 tests passed (68%)** |
| **Render Mode** | InteractiveServer |
| **Build Errors** | 0 (105 files generated) |

## Key Changes from Run 10

- Fully automated migration from scratch following prescribed order: fresh Blazor project → BWFC library → migration script → static content → C# adaptation → acceptance tests
- Proper process discipline restored (no coordinator domain work violations)
- CSS loading and image serving now working (improvements from Run 9/10 fixes)

## Root Causes of Failures

### RC-7: Scripts/ folder not copied (5 failures)
The migration script detected and copied CSS files from `Content/` but completely ignored JavaScript files from `Scripts/`. jQuery and Bootstrap JS returned 404.

### RC-9: ListView placeholder not converted (5 failures — CRITICAL)
The **most impactful defect**: `<tr id="groupPlaceholder">` inside `LayoutTemplate` was not converted to `@context`. In Web Forms, these are runtime placeholders replaced by the server control. In BWFC Blazor, `@context` renders the child content. Without this conversion, ListView rendered empty.

### RC-10: Missing "Add to Cart" link (2 failures)
ProductDetails.razor was migrated without the action link to add products to the cart.

### RC-11: Auth flow incomplete (1 failure)
Register/Login HTTP POST flow didn't produce authenticated state — the Blazor circuit didn't pick up the new auth cookie after redirect.

## What Worked

- Navigation (7/7 tests) ✅
- CSS loading and Bootstrap styling ✅
- Product images serving correctly ✅
- Page structure and layout ✅

## Script Fixes Recommended

Two critical script fixes were identified and implemented for Run 12:

1. **`Invoke-ScriptAutoDetection`** — Detect and copy JS files from `Scripts/` folder
2. **`Convert-TemplatePlaceholders`** — Convert `<tr id="groupPlaceholder">` → `@context` inside templates

## Full Report

See the complete report at [`dev-docs/migration-tests/wingtiptoys-run11-2026-03-07/REPORT.md`](../../dev-docs/migration-tests/wingtiptoys-run11-2026-03-07/REPORT.md).
