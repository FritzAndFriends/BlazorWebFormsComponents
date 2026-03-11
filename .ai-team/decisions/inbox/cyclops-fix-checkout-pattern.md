# Decision: Fix Test-UnconvertiblePage Checkout Pattern

**Author:** Cyclops
**Date:** 2026-03-11
**Status:** Proposed
**Priority:** P0

## Context

Run 18 confirmed that `ShoppingCart.aspx` is incorrectly stubbed by the Layer 1 migration
script (`bwfc-migrate.ps1`). The `Test-UnconvertiblePage` function (line 1230) matches the
pattern `'Checkout'` against markup content. ShoppingCart.aspx contains `CheckoutImageBtn`
and `CheckoutBtn_Click` which trigger this match, causing the entire page to be emitted as
a stub instead of being converted with its `<asp:GridView>` → `<GridView>` transformation.

This is the exact issue Jeff flagged — the whole point of BWFC is drop-in replacement, and
the script is throwing away a perfectly convertible GridView page.

## Proposed Fix

Narrow the `'Checkout'` pattern in `$unconvertiblePatterns` to avoid false positives.
Recommended approach: **path-based detection** — only stub `.aspx` files whose relative
path starts with `Checkout/` (or `Checkout\`), rather than matching "Checkout" as a
substring in markup content.

Alternative: Remove `'Checkout'` from markup patterns entirely and rely on code-behind
analysis (the Checkout pages' code-behinds contain `Session[`, `PayPal`, etc. which
would still trigger the stub).

## Impact

- Fixes ShoppingCart.razor to use BWFC `<GridView>` component
- No impact on actual Checkout/ pages (they match other patterns like `Session[`, `PayPal`)
- Affects all future migration runs
