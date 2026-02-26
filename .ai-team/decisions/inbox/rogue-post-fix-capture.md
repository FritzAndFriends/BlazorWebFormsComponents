# Decision: Post-Bug-Fix Capture Results — Sample Parity is the Primary Blocker

**Date:** 2026-02-26
**Author:** Rogue (QA)
**Status:** proposed
**Scope:** HTML fidelity audit pipeline

## Context

Re-ran the full HTML capture pipeline after 14 bug fixes across 10 controls (Button, BulletedList, LinkButton, Calendar, TreeView, CheckBox, RadioButtonList, FileUpload, Image, DataList, GridView). Previous run (M13) showed 132 divergences with 0 exact matches.

## Results

- **All 11 targeted controls show verified structural improvements** in Blazor output
- Exact matches: 0 → 1 (Literal-3)
- Missing Blazor captures: 75 → 64 (11 new captures gained)
- Calendar variants now at 55–73% word similarity (was much lower)
- Bug fixes confirmed working: Button `<button>`→`<input>`, BulletedList span removal, LinkButton href addition, Calendar border styling, CheckBox span removal, Image longdesc removal, GridView rules/border addition

## Decision

**The #1 blocker for achieving meaningful match rates is sample data parity, not component bugs.**

Nearly every remaining divergence is caused by the WebForms and Blazor samples using completely different text, data, IDs, styling, and configuration. Examples: Label shows "Hello World" vs "Hello, World!", Button shows "Blue Button" vs "Click me!", HyperLink links to bing.com vs github.com.

### Recommended priority order:
1. **P0:** Align Blazor sample data to match WebForms samples (could convert 20+ divergences to exact matches)
2. **P1:** Add audit markers to 64 missing Blazor sample pages
3. **P2:** Enhance normalizer (GUID ID stripping, empty style="" removal)
4. **P3:** Fix remaining structural bugs (BulletedList `<ol>` for numbered, missing `list-style-type`)

## Impact

Without sample alignment, the pipeline cannot distinguish between "component renders wrong HTML" and "samples show different content." Any future bug-fix measurement will be equally blocked.

## Evidence

Full analysis: `planning-docs/POST-FIX-CAPTURE-RESULTS.md`
Diff report: `audit-output/diff-report-post-fix.md`
