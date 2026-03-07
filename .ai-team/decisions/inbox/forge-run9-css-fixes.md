# Decision: Fix 1a + Fix 1b — CSS Auto-Detection in bwfc-migrate.ps1

**Author:** Forge  
**Date:** 2026-03-06  
**Status:** Implemented  
**Relates to:** Run 9 CSS/Image Failure RCA (RC-1)

## Context

Run 9 produced completely unstyled Blazor output because:
1. `ConvertFrom-MasterPage` silently dropped `<webopt:bundlereference>` tags (ASP.NET Web Optimization bundle syntax)
2. `New-AppRazorScaffold` generated App.razor with zero CSS references
3. Layer 2 had to discover and wire CSS manually — which it did late (after screenshots)

## Decision

Two fixes applied to `bwfc-migrate.ps1`:

### Fix 1a: Extract `<webopt:bundlereference>` in ConvertFrom-MasterPage
- Regex now matches `<webopt:bundlereference>` tags in `<head>`
- Extracts `path` attribute, flags as `[CSSBundle]` manual review item
- Injects `@* TODO *@` comment into HeadContent for Layer 2 visibility
- Also preserves CDN `<link>`/`<script>` tags from `<head>` (Bootstrap, jQuery, etc.)

### Fix 1b: Auto-detect CSS via `Invoke-CssAutoDetection`
- Runs after static file copy step
- Scans `wwwroot/Content/`, `wwwroot/css/`, and `wwwroot/` root for `.css` files
- Scans source `Site.Master` for CDN references
- Injects `<link>` tags into App.razor `<head>` before `<HeadOutlet>`
- Layer 1 output now has CSS wired — Layer 2 doesn't need to discover it

## Impact

- Layer 1 output will have CSS references from the start
- Layer 2 agents see TODO comments for bundle references
- CDN references (Bootstrap, jQuery) are preserved automatically
- Expected to eliminate the "unstyled HTML" failure mode seen in Run 9

## Notes for Team

- **Cyclops:** Your Layer 2 work should see CSS already wired in App.razor — verify and adjust if needed rather than adding from scratch
- **Beast:** Skills can reference that Layer 1 now handles CSS auto-detection — no need for Layer 2 guidance on basic CSS wiring
- New manual review category `CSSBundle` will appear in migration summaries when bundle references are found
