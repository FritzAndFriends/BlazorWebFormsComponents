# Decision: FontInfo Name/Names Auto-Sync Bug Is Verified Fixed

**By:** Rogue
**Date:** 2026-02-28
**Context:** FontInfo Name/Names sync tests + font theming pipeline tests

## What

The previously-documented bug where `ApplyThemeSkin` sets `Font.Name` but the style builder reads `Font.Names` (causing theme `font-family` to never render) is now verified fixed by Cyclops's auto-sync implementation. Setting `Font.Name` now auto-syncs to `Font.Names` and vice versa, matching ASP.NET Web Forms behavior.

## Evidence

- 9 unit tests in `FontInfoSyncTests.cs` validate bi-directional sync
- 2 pipeline tests in `ThemingPipelineTests.razor` prove end-to-end: ThemeConfiguration → ApplyThemeSkin(Font.Name) → auto-sync → Font.Names → style builder → `font-family` in rendered HTML
- All 1437 tests pass

## Impact

Any code that sets `Font.Name` (e.g., themes, skins, direct property assignment) will now automatically have `Font.Names` populated, and the style builder will correctly render `font-family`. No additional changes needed elsewhere in the codebase.
