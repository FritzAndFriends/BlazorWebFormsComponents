# Decision: Border CSS is ALL-OR-NOTHING in style builder

**By:** Rogue (QA Analyst)
**Date:** 2026-07-25
**Context:** Issues #15, #16, #17, #18 — base class property tests

## Decision

Tests for border-related style properties (BorderWidth, BorderColor, BorderStyle) must always set **all three** properties simultaneously. The style builder (`HasStyleExtensions.ToStyle()`) only emits a `border:` CSS property when ALL of these conditions are true:

1. `BorderStyle != None` and `BorderStyle != NotSet`
2. `BorderWidth.Value > 0`
3. `BorderColor != default(WebColor)`

If any single condition fails, **no border CSS is generated at all**. Tests that set only `BorderWidth` or only `BorderColor` will see no style output — this is correct behavior, not a bug.

## Why This Matters

- Future component tests and theme skin tests must follow this pattern
- The output format is combined: `border: 2px solid Black` (not separate border-width/border-color/border-style properties)
- Cyclops should be aware when implementing GridView style rendering (Issue #17)

## Test Status

10 expected failures awaiting Cyclops's implementation:
- 5 AccessKey rendering (TextBox, Image, Panel, HyperLink, CheckBox)
- 5 GridView style merging (BackColor, ForeColor, Width, Height, MultipleStyles)
