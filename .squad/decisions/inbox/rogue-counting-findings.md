# Rogue — Counting Algorithm Findings

**Date:** 2026-07-25
**Source:** ComponentHealthCountingTests verification against BWFC assembly

## Summary

The PRD §2.7 worked examples use approximate counts ("~7", "~18"). Running the exact §5.4 algorithm against the real assembly produces slightly different numbers. These are NOT bugs — the algorithm is correct. The PRD estimates were rounded.

## Exact Counts vs PRD Estimates

| Component | PRD Estimate | Actual Count | Delta | Notes |
|-----------|-------------|--------------|-------|-------|
| **Button** | ~7 props, 2 events | **8 props, 2 events** | +1 prop | PostBackUrl on ButtonBaseComponent is counted (see below) |
| **GridView** | ~18 props, ~10 events | **21 props, 10 events** | +3 props | All 21 are legitimate component-specific properties |
| **Repeater** | 0 props, 0 events | **0 props, 0 events** | — | Exact match ✓ |

## Button PostBackUrl Detail

The PRD §2.7 table lists 8 properties for Button (including PostBackUrl from ButtonBaseComponent) but then says "Result: 7 properties, 2 events." This is a typo in the PRD — the table itself shows 8.

**Why 8:** ButtonBaseComponent declares `[Parameter] public virtual string PostBackUrl`. Button overrides it with `[Obsolete] public override string PostBackUrl` (no `[Parameter]` on the override). The algorithm correctly:
1. **Skips** PostBackUrl at the Button level (no `[Parameter]` attribute on the override)
2. **Counts** PostBackUrl at the ButtonBaseComponent level (has `[Parameter]`, no `[Obsolete]`)

## GridView 21 Properties (not ~18)

All 21 properties are legitimate GridView-specific `[Parameter]` declarations:
AutoGenerateColumns, EmptyDataText, DataKeyNames, EditIndex, SelectedIndex, AutoGenerateSelectButton, ShowHeader, ShowFooter, ShowHeaderWhenEmpty, Caption, CaptionAlign, GridLines, UseAccessibleHeader, CellPadding, CellSpacing, AllowSorting, SortDirection, SortExpression, AllowPaging, PageSize, PageIndex

The 12 RenderFragment templates were correctly excluded. The PRD's "~18" was a rough estimate — all 21 are real.

## CascadingParameter Discovery

ButtonBaseComponent.Coordinator is `protected`, not `public`. The `BindingFlags.Public | Instance | DeclaredOnly` reflection correctly excludes it without needing the `[CascadingParameter]` check. This is actually a stronger exclusion — even if someone accidentally puts both `[Parameter]` and `[CascadingParameter]` on a protected property, it won't leak into counts.

## Recommendation

1. **PRD §2.7 Button example:** Fix "7 properties" → "8 properties" to match the actual table
2. **PRD §2.7 GridView example:** Update "~18 properties" → "21 properties" or note that the exact count will be determined by reflection
3. **Reference baselines (dev-docs/reference-baselines.json):** When creating baselines, use the actual reflection counts (8, 21) not the PRD estimates (~7, ~18)
4. **Tests pass with ranges:** The acceptance tests use ranges (7-8 for Button, 10-25 for GridView) to accommodate minor future changes. If exact-match assertions are preferred, set Button=8, GridView=21.
