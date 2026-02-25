# Decision: Theme Integration in BaseStyledComponent

**Date:** 2026-02-26
**Author:** Cyclops
**Issue:** #366

## Context

With core theme types (#364) and SkinID/EnableTheming activation (#365) complete, the final wiring step connects the `ThemeConfiguration` cascading parameter to `BaseStyledComponent` so all styled components automatically participate in theming.

## Decisions Made

1. **CascadingParameter on BaseStyledComponent, not BaseWebFormsComponent** — Only styled components have visual properties (BackColor, ForeColor, etc.) to skin. Placing the `[CascadingParameter] ThemeConfiguration Theme` here keeps the concern scoped correctly.

2. **OnParametersSet, not OnInitialized** — Theme values must be applied every time parameters change (e.g., if the theme is swapped at runtime). `OnParametersSet` is the correct lifecycle hook. Early-return when `EnableTheming == false` or `Theme == null` ensures zero impact on existing code.

3. **StyleSheetTheme semantics (defaults, not overrides)** — Each property is only applied when the component's current value equals its type default (`default(WebColor)`, `default(Unit)`, `default(BorderStyle)`, `string.IsNullOrEmpty`, `FontUnit.Empty`, `false` for booleans). This matches ASP.NET Web Forms StyleSheetTheme behavior where themes provide defaults and explicit attribute values take precedence.

4. **No logging for missing named skins** — Per project convention (Jeff's decision on #364), missing SkinID returns null and processing silently continues. ILogger injection deferred to M11 to avoid scope creep.

5. **Font properties checked individually** — Since `Font` is always initialized to `new FontInfo()` in `BaseStyledComponent`, we cannot use a null check. Instead, each font sub-property (Name, Size, Bold, Italic, Underline) is checked against its own default.

## Impact

- All components inheriting `BaseStyledComponent` now automatically receive theme skins when wrapped in `<ThemeProvider>`.
- Existing tests are unaffected — without a `ThemeProvider` ancestor, `Theme` is null and the early-return fires.
- Future work: add Overline/Strikeout font properties, ILogger for missing skin warnings, Theme property override semantics.
