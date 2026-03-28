# Decision: Theming Integration Tests Written Ahead of Sections 7 & 8

**Author:** Colossus  
**Date:** 2026-03-22  
**Status:** Implemented

## Context

Jubilee is building Section 7 (ThemeMode — StyleSheetTheme vs Theme) and Section 8 (Sub-Styles — GridView header/footer theming) on the `/ControlSamples/Theming` page. Tests were requested in advance so they're ready when the markup lands.

## Decision

Two new Playwright tests added to `InteractiveComponentTests.cs`:

1. **`Theming_ThemeMode_StyleSheetThemeVsTheme`** — Uses regex heading match (`Theme\s*Mode`) and text content assertions to verify both StyleSheetTheme and Theme panels render with buttons.
2. **`Theming_SubStyles_GridViewHeaderAndFooter`** — Uses regex heading match for the sub-styles section, verifies `<table>` and `<th>` header cells exist (GridView structure).

Both tests use flexible selectors (regex for headings, semantic HTML elements like `table`, `th`, `button`) rather than class names or exact text to tolerate minor markup changes from Jubilee.

## Impact

These tests will fail with clear assertion messages until Jubilee's sections are added. Once the markup is present, they should pass without modification.
