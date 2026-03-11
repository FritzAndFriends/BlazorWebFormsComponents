# WingtipToys Migration — Run 18 Report

**Date:** 2026-03-11 11:29:00 -04:00
**Branch:** `squad/post-implicit-conversions`
**Operator:** Cyclops (Component Dev)
**Requested by:** Jeffrey T. Fritz

---

## Run 18b — After Script Fix

**Date:** 2026-03-11
**Trigger:** Re-run after `Test-UnconvertiblePage` script fix (path-based Checkout detection)

### Script Fix Applied

The `Test-UnconvertiblePage` function was updated to replace the overly broad
`'Checkout'` content pattern with path-based detection:

```powershell
# BEFORE (Run 18a):
$unconvertiblePatterns = @(
    'SignInManager', 'UserManager', 'FormsAuthentication',
    'Session\[', 'PayPal', 'Checkout'
)

# AFTER (Run 18b):
# Path-based stubs: pages under Checkout/ folder need manual payment/auth work
if ($RelativePath -match '^Checkout[/\\]') {
    return $true
}
$unconvertiblePatterns = @(
    'SignInManager', 'UserManager', 'FormsAuthentication',
    'Session\[', 'PayPal'
)
```

This correctly prevents `CheckoutImageBtn`/`CheckoutBtn_Click` in ShoppingCart.aspx
from triggering a false-positive stub.

### ShoppingCart GridView Status: ❌ STILL FAIL — New Root Cause

**Result:** ShoppingCart.razor is **still stubbed** after the Checkout fix.

**New root cause:** The `'PayPal'` pattern in `Test-UnconvertiblePage` matches:
- Line 43: `ImageUrl="https://www.paypal.com/en_US/i/btn/btn_xpressCheckout.gif"`
- Line 44: `AlternateText="Check out with PayPal"`

These are **UI references only** (an image URL and alt text for a PayPal Express
Checkout button). The ShoppingCart.aspx code-behind (`ShoppingCart.aspx.cs`) has
**zero** PayPal SDK references — no actual payment processing code.

**ShoppingCart.razor output (still a stub):**
```razor
@page "/ShoppingCart"
<h3>ShoppingCart — not yet migrated</h3>
@* TODO: Implement using ASP.NET Core Identity scaffolding *@
@code {
}
```

### UnconvertibleStub Count: 6 (unchanged)

| Page | Stub Reason | Correct? |
|------|-------------|----------|
| ShoppingCart.aspx | `'PayPal'` matches image URL/alt text | ❌ False positive |
| Checkout\CheckoutCancel.aspx | Path: `Checkout/` folder | ✅ |
| Checkout\CheckoutComplete.aspx | Path: `Checkout/` folder | ✅ |
| Checkout\CheckoutError.aspx | Path: `Checkout/` folder | ✅ |
| Checkout\CheckoutReview.aspx | Path: `Checkout/` folder | ✅ |
| Checkout\CheckoutStart.aspx | Path: `Checkout/` folder | ✅ |

**Expected after full fix:** 5 stubs (Checkout/ folder only). ShoppingCart should
convert with `<GridView>`, `<BoundField>`, `<TemplateField>` components.

### Recommended Fix for PayPal Pattern

The `'PayPal'` pattern should be narrowed to match only actual PayPal SDK usage:
1. **Code-behind only:** Check `.aspx.cs` for PayPal references, not markup
2. **Pattern refinement:** Use `'PayPal\.'` or `'PayPalApi'` to match SDK calls, not image URLs
3. **Path-based:** Move PayPal detection to a known-pages list like Checkout was

### Layer 1 Timing (Run 18b)

| Phase | Duration |
|-------|----------|
| Layer 1 (bwfc-migrate.ps1) | **1.58 seconds** |
| Build (with ProjectReference fix) | ~3.14 seconds |
| **Total pipeline** | **~4.7 seconds** |

### Build Result (Run 18b)

**Status:** ❌ FAIL (same errors as Run 18a)
**Errors:** 6 (3 unique × 2 files — ProductDetails.razor.cs, ProductList.razor.cs)
**Warnings:** 8 (NU1510 from BWFC csproj)

Same `[Parameter]` TODO annotation bug — no change from Run 18a.

---

## Run 18a — Original Run (below)

## Executive Summary

Layer 1 migration completed successfully (32 files, 303 transforms, 1.86s), but
**ShoppingCart.razor was incorrectly stubbed** instead of being converted with BWFC
`<GridView>`. Build fails with 6 errors (all in Layer 1 code-behinds needing Layer 2 fixes).

## ShoppingCart.razor GridView Status: ❌ FAIL

### Problem

The original `ShoppingCart.aspx` uses a proper `<asp:GridView>` with:
- `<asp:BoundField>` for ProductID, ProductName, UnitPrice
- `<asp:TemplateField>` for Quantity (TextBox), Item Total (calculated), Remove (CheckBox)
- `SelectMethod="GetShoppingCartItems"`
- `ShowFooter="True"`, `GridLines="Vertical"`, `CellPadding="4"`

### What the Script Produced

```razor
@page "/ShoppingCart"
<h3>ShoppingCart — not yet migrated</h3>
@* TODO: Implement using ASP.NET Core Identity scaffolding *@
@code {
}
```

A **stub** — no GridView, no BoundFields, no TemplateFields, nothing.

### Root Cause

The `Test-UnconvertiblePage` function in `bwfc-migrate.ps1` (line 1230–1247) checks
markup content against these patterns:

```powershell
$unconvertiblePatterns = @(
    'SignInManager', 'UserManager', 'FormsAuthentication',
    'Session\[', 'PayPal', 'Checkout'
)
```

ShoppingCart.aspx contains:
- `CheckoutImageBtn` (line 42) — matches `'Checkout'`
- `CheckoutBtn_Click` (line 45) — matches `'Checkout'`

The `'Checkout'` pattern is **too broad** — it matches any ID or event handler name
containing "Checkout", not just actual checkout-flow pages. This causes ShoppingCart.aspx
(a shopping **cart** page that merely links to checkout) to be stubbed.

### Recommended Fix

Change the `'Checkout'` pattern to match only standalone Checkout page references:
- Use `'^\s*<%@.*Checkout'` to match only Checkout page directives
- Or remove `'Checkout'` from the unconvertible patterns and handle checkout pages
  via path-based detection (e.g., files under `Checkout/` folder)
- Or use `'\bCheckout\b'` with word boundaries and exclude attribute values

## Successes

1. **Layer 1 script executed cleanly** — 32 files processed, 303 transforms applied
2. **Static asset copy** — 79 static files copied to wwwroot (CSS, JS, images, fonts)
3. **CSS auto-detection** — 5 CSS/CDN references injected into App.razor `<head>`
4. **JS auto-detection** — 7 JS files copied to wwwroot/Scripts/, 7 `<script>` tags injected
5. **Model files** — 8 model files copied from Models/
6. **Placeholder conversion** — ListView placeholder elements correctly converted to `@context`
7. **Project scaffold** — csproj, Program.cs, _Imports.razor, App.razor all generated
8. **BWFC component preservation** — asp: prefix stripping produces correct BWFC tags
   (GridView, ListView, BoundField, TemplateField, etc.) for non-stubbed pages
9. **Data binding conversion** — `<%#: Item.X %>` → `@context.X` in templates
10. **35 manual review items flagged** — proper Layer 2 guidance generated

## Failures

1. **ShoppingCart.razor STUBBED** — `Test-UnconvertiblePage` false positive due to
   overly broad `'Checkout'` pattern matching `CheckoutImageBtn`/`CheckoutBtn_Click`
   in markup. **This is the critical issue Jeff reported.**

2. **Build fails (6 errors)** — All in Layer 1 code-behinds (expected for Layer 1-only):
   - `ProductDetails.razor.cs:36` — Malformed `[Parameter]` TODO annotation swallowed
     the `string productName)` parameter declaration
   - `ProductList.razor.cs:37` — Same issue: `[Parameter]` TODO swallowed
     `string categoryName)` parameter declaration
   - Each file produces CS1031 (Type expected), CS1001 (Identifier expected),
     CS1026 (') expected') — 3 errors × 2 files = 6 errors

3. **8 NuGet warnings (NU1510)** — BWFC PackageReference warnings about prunable
   packages (from upstream BWFC csproj, not the migration output)

4. **6 pages stubbed total** — ShoppingCart, CheckoutCancel, CheckoutComplete,
   CheckoutError, CheckoutReview, CheckoutStart. Of these, only ShoppingCart should
   NOT have been stubbed (the Checkout/ pages are correctly identified as needing
   manual auth/payment implementation).

## Build Result

**Status:** ❌ FAIL
**Errors:** 6 (3 unique × 2 files)
**Warnings:** 8 (NU1510 from BWFC csproj — not from migration output)

### Error Details

```
ProductDetails.razor.cs(36,36): error CS1031: Type expected
ProductDetails.razor.cs(36,36): error CS1001: Identifier expected
ProductDetails.razor.cs(36,36): error CS1026: ) expected
ProductList.razor.cs(37,36): error CS1031: Type expected
ProductList.razor.cs(37,36): error CS1001: Identifier expected
ProductList.razor.cs(37,36): error CS1026: ) expected
```

**Root cause:** The script's RouteData → `[Parameter]` TODO annotation is malformed:
```csharp
// Generated (broken):
[Parameter] // TODO: Verify RouteData → [Parameter] conversion — ensure @page route template has matching {parameter} string productName)

// Should be:
[Parameter] string productName) // TODO: Verify RouteData → [Parameter] conversion
```

The TODO comment consumes the parameter type and name, leaving the method signature incomplete.

### Note on csproj Fix

The generated csproj had `<PackageReference Include="Fritz.BlazorWebFormsComponents" Version="*" />`
which fails in this repo (no auth to GitHub Packages feed). Changed to
`<ProjectReference Include="..\..\src\BlazorWebFormsComponents\BlazorWebFormsComponents.csproj" />`
for local builds. This is a standard Layer 2 fix per Run 12/13 patterns.

## Timing

| Phase | Duration |
|-------|----------|
| Layer 1 (bwfc-migrate.ps1) | 1.86 seconds |
| Layer 2 (bwfc-migrate-layer2.ps1) | N/A — script does not exist |
| Build (with ProjectReference fix) | 5.42 seconds |
| **Total pipeline** | **~7.3 seconds** |

## File Counts

| Category | Count |
|----------|-------|
| Web Forms files processed | 32 |
| Transforms applied | 303 |
| Static files copied | 79 |
| Model files copied | 8 |
| Manual review items | 35 |
| **Total output files** | **151** |
| .razor files | 35 |
| .cs files | 35 |
| Static assets | 79 |

## Migration Script Output (Full)

```
============================================================
  BWFC Migration Tool — Layer 1: Mechanical Transforms
============================================================
  Source:  D:\BlazorWebFormsComponents\samples\WingtipToys\WingtipToys
  Output:  D:\BlazorWebFormsComponents\samples\AfterWingtipToys
  Project: WingtipToys

Generating project scaffold...
Discovering Web Forms files...
Found 32 Web Forms file(s) to transform.
Applying transforms...
Copying 79 static file(s)...
  Injected 5 CSS/CDN reference(s) into App.razor <head>
  Copied 7 JS file(s) to wwwroot/Scripts/
  Injected 7 <script> tag(s) into App.razor <body>
Copying 8 model file(s) from Models/...

============================================================
  Migration Summary
============================================================
  Files processed:       32
  Transforms applied:    303
  Static files copied:   79
  Model files copied:    8
  Items needing review:  35

--- Items Needing Manual Attention ---
  [CodeBlock] (7 items)
  [ContentPlaceHolder] (1 item)
  [CSSBundle] (1 item)
  [DbContext] (1 item)
  [GetRouteUrl] (5 items)
  [ListView-GroupItemCount] (1 item)
  [RedirectHandler] (1 item)
  [RegisterDirective] (4 items)
  [SelectMethod] (8 items)
  [UnconvertibleStub] (6 items) ← ShoppingCart incorrectly included
```

## Recommendations

### P0 — Fix ShoppingCart Stubbing (Script Bug)

The `Test-UnconvertiblePage` function's `'Checkout'` pattern must be narrowed to avoid
false positives on pages that merely reference checkout (button names, event handlers)
but are not checkout-flow pages themselves.

**Options:**
1. **Path-based detection:** Only stub pages under `Checkout/` folder, not pages that
   mention "checkout" in their markup
2. **Code-behind analysis:** Check the `.aspx.cs` file for checkout patterns instead of
   the markup file (the ShoppingCart code-behind has `Session[` and `Response.Redirect`
   to Checkout, but those are in the code-behind, not the markup)
3. **Whitelist approach:** Maintain a list of known page types that should always be
   converted (GridView-containing pages)

### P1 — Fix RouteData Parameter TODO Annotation

The `[Parameter]` TODO annotation in code-behinds swallows subsequent parameter
declarations. The TODO should be placed as a trailing comment after the parameter,
not replacing it.

### P2 — Layer 2 Script

No `bwfc-migrate-layer2.ps1` exists. Layer 2 is currently manual. Consider creating
an automated Layer 2 for common patterns (csproj ProjectReference, _Imports.razor
additions, EF6→EF Core model transforms).
