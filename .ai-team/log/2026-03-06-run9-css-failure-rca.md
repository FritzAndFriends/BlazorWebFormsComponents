# Run 9 CSS/Image Failure — Root Cause Analysis

**Date:** 2026-03-06
**Requested by:** Jeffrey T. Fritz
**Agent:** Forge (investigation)

## Summary

Forge investigated why Run 9 WingtipToys migration has no CSS styling or product images despite 14/14 acceptance tests passing.

## Root Causes

1. **RC-1 (P0, Layer 1):** `ConvertFrom-MasterPage` in `bwfc-migrate.ps1` only extracts `<meta>`, `<title>`, and `<link>` tags from `<head>`. `<webopt:bundlereference>` CSS tags are silently dropped. `New-AppRazorScaffold` generates App.razor with zero CSS `<link>` tags.
2. **RC-2 (P0, Layer 2):** Layer 2 rewrote image paths from `/Catalog/Images/` to `/Images/Products/` (FreshWingtipToys convention) without creating the directory or moving files. All product images return 404.
3. **RC-3 (P1, Tests):** All 14 acceptance tests check page loads, navigation, and form submissions — none verify CSS loading, image rendering, or visual output. Completely unstyled app scores 14/14.

## Proposed Fixes

| # | Fix | Severity | Target |
|---|-----|----------|--------|
| 1 | Script: extract CSS from bundle refs, inject `<link>` tags into App.razor | P0 | `bwfc-migrate.ps1` |
| 2 | Layer 2 skill: preserve source image paths, don't rewrite without moving files | P0 | `migration-standards.md` |
| 3 | Add visual smoke test (CSS 200, no broken images) | P1 | Acceptance tests |
| 4 | Fix current AfterWingtipToys image paths (`/Images/Products/` → `/Catalog/Images/`) | P0 | Sample code |
| 5 | Run 9 report mischaracterizes screenshots — auto-generated without visual validation | P2 | Documentation |

## Key Evidence

- Run 8 screenshots show proper Bootstrap styling; Run 9 shows unstyled bullet lists
- `GET /Content/bootstrap.min.css` → 200 OK (files exist, just no `<link>` references at screenshot time)
- `GET /Images/Products/carconvert.png` → 404; `GET /Catalog/Images/carconvert.png` → 200
- wwwroot content identical between Run 8 and Run 9 — only references differ
