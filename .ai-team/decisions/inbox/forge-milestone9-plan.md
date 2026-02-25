# Decision: Milestone 9 Plan — Migration Fidelity & Hardening

**By:** Forge (Lead / Web Forms Reviewer)
**Date:** 2026-02-25
**Status:** Proposed

## Context

v0.14.1 shipped. 51/53 components complete (96%), 1200+ tests passing. Milestones 6–8 closed ~345+ gaps, fixed the deployment pipeline, and shipped release readiness. I verified all 8 known priority gaps from prior audit findings against the current `dev` branch.

## Verification Results

**7 of 8 gaps from prior audits are already fixed:**
- AccessKey on BaseWebFormsComponent ✅
- Image → BaseStyledComponent ✅
- Label → BaseStyledComponent + AssociatedControlID ✅
- Validation Display property ✅
- HyperLink NavigateUrl vs NavigationUrl ✅
- DataBoundComponent chain → BaseStyledComponent ✅
- bUnit test migration (beta → 2.x) ✅ Not needed — already modern API

**1 gap confirmed still open + 2 newly identified:**
1. **ToolTip not on BaseStyledComponent** (P0) — 28+ styled controls lack ToolTip entirely; 8 components implement it individually. Moving to base class is the single highest-leverage remaining change.
2. **ValidationSummary comma-split bug** (P1) — `AspNetValidationSummary.razor.cs` line 27 uses `.Split(',')[1]` which corrupts error messages containing commas. Data corruption risk.
3. **SkinID is `bool` instead of `string`** (P1 — newly identified) — `BaseWebFormsComponent.cs` line 101. Web Forms `SkinID` is a string property. Migration-breaking type mismatch.

## Decision

Milestone 9: 12 work items, ~30 gap closures, themed "Paste and It Works."

### P0 — ToolTip → BaseStyledComponent (4 WIs)
Add ToolTip to BaseStyledComponent, remove from 8 individual components, audit all templates for `title="@ToolTip"` rendering. ~28 gap closures. This is the single remaining base-class sweep — same pattern that AccessKey, Display, and DataBound inheritance used in M6.

### P1 — Bug Fixes (3 WIs)
- Fix ValidationSummary comma-split: use `IndexOf` + `Substring` instead of `Split`
- Fix SkinID type: `bool` → `string` (keep `[Obsolete]` attribute)

### P2 — Housekeeping (5 WIs)
- Clean up 10 stale local branches
- Documentation gap audit for M6–M8 features
- Update stale planning-docs audit files
- Integration test coverage review
- Sample site navigation audit

## Rationale

1. ToolTip base class fix has the highest blast radius of any remaining change (~28 controls)
2. ValidationSummary bug is a data corruption issue that should ship before 1.0
3. SkinID type mismatch means any migrated page with `SkinID="MySkin"` won't compile
4. All P0/P1 changes are low-risk, mechanical, and well-understood patterns
5. Housekeeping keeps the project maintainable as we approach 1.0

## Impact

- Remaining gap count drops by ~30
- All BaseStyledComponent-derived controls gain ToolTip (title attribute)
- ValidationSummary safely handles commas in error messages
- SkinID accepts string values matching Web Forms signature

Full plan at `planning-docs/MILESTONE9-PLAN.md`.
