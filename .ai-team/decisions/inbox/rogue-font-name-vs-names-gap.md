# Decision: Font.Name vs Font.Names gap in ApplyThemeSkin

**Date:** 2026-03-01
**By:** Rogue (QA Analyst)
**Scope:** `BaseStyledComponent.ApplyThemeSkin` + `HasStyleExtensions`

## Finding

`ApplyThemeSkin` in `BaseStyledComponent` sets `Font.Name` from the theme's `ControlSkin.Font.Name`, but the style builder (`HasStyleExtensions.ToStyle()`) reads `Font.Names` (plural) for the `font-family` CSS property. These are separate properties on `FontInfo` with no synchronization.

**Result:** Setting `Font.Name` via a theme skin does NOT produce `font-family` in the rendered HTML.

## Impact

- Theme font-family is silently ignored at render time
- All other font properties (Bold, Italic, Underline, Size) work correctly
- Not blocking for M20 PoC — documented and tested around

## Recommendation

In M11 (full Skins & Themes implementation), either:
1. Sync `Font.Name` → `Font.Names` in `ApplyThemeSkin`, or
2. Have `ApplyThemeSkin` set `Font.Names` instead of `Font.Name`, or
3. Add a getter to `FontInfo` that returns `Names ?? Name` for the style builder
