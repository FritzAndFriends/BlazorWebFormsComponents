# Decision: SkinID and EnableTheming Property Defaults

**Date:** 2026-02-26
**Author:** Cyclops
**Issue:** #365

## Context

The `SkinID` and `EnableTheming` properties on `BaseWebFormsComponent` were previously marked `[Obsolete]` with the message "Theming is not available in Blazor". As part of the Skins & Themes PoC, these properties need to become functional.

## Decision

Per Jeff's confirmed decisions:

- **EnableTheming defaults to `true`** — follows StyleSheetTheme semantics where the theme sets defaults and explicit component values override.
- **SkinID defaults to `""` (empty string)** — meaning "use default skin". This matches Web Forms behavior where an unset SkinID applies the default skin for the control type.
- **`[Obsolete]` attributes removed** — these are now functional `[Parameter]` properties ready for #366 integration.

## Impact

- All components inheriting from `BaseWebFormsComponent` now have `EnableTheming = true` and `SkinID = ""` by default.
- No breaking changes — existing code that doesn't use theming is unaffected since there's no theme provider yet (#364/#366 will add that).
- #366 (Forge's base class integration) can now wire these properties into the theme resolution pipeline.
