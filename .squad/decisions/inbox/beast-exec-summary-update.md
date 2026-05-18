# Decision: Executive Summary Update Pattern

**Date:** 2026-05-17T21:45:23-04:00
**Author:** Beast (Technical Writer)
**Status:** Established

## Decision

When updating the executive summary with new benchmark run data:

1. **Count transforms from source, not docs.** Always grep `AddSingleton<I(Markup|CodeBehind)Transform` in `src/BlazorWebFormsComponents.Cli/Program.cs` for authoritative counts. The copilot-instructions count lags implementation.

2. **Verify screenshot paths before linking.** Check which image files actually exist in the run folder before referencing them. Run 90 had `01–06` images but no `07-cart-with-item.png` — the cart screenshot was `04-shopping-cart.png` instead.

3. **Regenerate charts from generate-charts.py, not by editing SVGs.** The Python file is the single source of truth for chart data. Update it with new data points and rerun — do not hand-edit the SVG output.

4. **The dual-benchmark chart format is the canonical multi-app visual.** Two panels: (1) pipeline phases by app (L1/L2/other stacked bars) and (2) acceptance test stability over recent runs for both apps side by side. Use this for any future multi-benchmark executive summary updates.

5. **Headline framing should reflect what improved most.** When build repair time drops more dramatically than total wall clock (e.g. 3:03 → 1:01 in Run 90), lead with error count reduction and repair time, not just total duration. The total wall clock includes report-writing overhead that does not reflect automation progress.

## Why

This update cycle (Run 90 + CU Run 30) revealed that transform counts in copilot-instructions.md and the exec summary were significantly out of date (24→37 markup, 27→48 code-behind), screenshots were app/run-specific rather than stable, and the single-app headline no longer captured the dual-benchmark story. Encoding these rules prevents the same staleness in future updates.

## Scope

Applies to all future executive summary updates in `dev-docs/migration-tests/EXECUTIVE-SUMMARY.md`.
