# Decision: PayPal Pattern in Test-UnconvertiblePage Needs Narrowing

**Date:** 2026-03-11
**Author:** Cyclops (Component Dev)
**Context:** Run 18b — WingtipToys migration re-run after Checkout pattern fix

## Problem

The `Test-UnconvertiblePage` function's `'PayPal'` pattern is too broad, causing
ShoppingCart.aspx to be stubbed even after the Checkout pattern was fixed.

**False positive match in ShoppingCart.aspx:**
- Line 43: `ImageUrl="https://www.paypal.com/en_US/i/btn/btn_xpressCheckout.gif"`
- Line 44: `AlternateText="Check out with PayPal"`

These are UI references (image URL + alt text), not PayPal SDK code.
ShoppingCart.aspx.cs has zero PayPal references.

## Impact

ShoppingCart.razor is generated as a stub instead of being converted with BWFC
`<GridView>`, `<BoundField>`, and `<TemplateField>` components. This is the
primary validation target for the migration pipeline.

## Options

1. **Narrow the regex** — Use `'PayPal\.'` or `'PayPalApi|PayPalService'` to match
   SDK calls only, not image URLs
2. **Code-behind analysis** — Check `.aspx.cs` for PayPal references instead of
   markup (ShoppingCart.aspx.cs has none)
3. **Path-based** — Remove `'PayPal'` from content patterns entirely; rely on
   path-based detection for known payment pages (like the Checkout fix)
4. **Combined** — Use code-behind analysis AND narrow regex for defense in depth

## Recommendation

Option 2 or 4. Code-behind analysis is the most accurate — actual PayPal SDK usage
will be in code-behind, not markup. The markup `PayPal` references are just UI
chrome (button images, alt text) that should be preserved during conversion.

## Status

**Awaiting decision** from Jeff / team lead.
