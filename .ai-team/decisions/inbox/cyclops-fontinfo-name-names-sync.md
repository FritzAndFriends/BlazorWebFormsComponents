# Decision: FontInfo.Name and FontInfo.Names are auto-synced

**Author:** Cyclops  
**Date:** 2026-03-01  
**Status:** Implemented  

## Context

ASP.NET Web Forms `FontInfo.Name` and `FontInfo.Names` are bidirectionally synced — setting one automatically updates the other. Our library had them as independent auto-properties, causing theme font-family to silently not render (ApplyThemeSkin sets Name, style builder reads Names).

## Decision

`FontInfo.Name` and `FontInfo.Names` now use backing fields with auto-sync:
- Setting `Name` → `Names` gets same value
- Setting `Names` → `Name` gets first comma-separated entry (trimmed)
- Setting either to null/empty → both cleared

`BaseStyledComponent.ApplyThemeSkin` now checks both `Font.Name` AND `Font.Names` are empty before applying theme font.

## Impact

- Any code setting `Font.Name` will now also have `Font.Names` populated (and vice versa)
- Theme font-family now renders correctly via the style pipeline
- All 1437 existing tests pass unchanged
