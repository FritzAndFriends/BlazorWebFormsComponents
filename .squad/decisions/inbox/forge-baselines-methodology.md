# Decision: Reference Baseline Sourcing Methodology

**By:** Forge (Lead / Web Forms Reviewer)
**Date:** 2026-07-25
**Status:** Decided — Jeff's directive

## Context

The Component Health Dashboard (PRD #439) needs reference baselines — expected property and event counts for each of the 54+ tracked Web Forms controls. These baselines are the denominators in the health scoring formula.

The PRD originally offered two methods:
1. **MSDN manual curation** (Preferred) — manually research each control's declared properties/events from the .NET Framework 4.8 API documentation
2. **Reflection tool** (Acceptable fallback) — build a .NET Fx 4.8 console app in `tools/WebFormsPropertyCounter/` that uses `Type.GetProperties(BindingFlags.DeclaredOnly)` against System.Web.dll

## Decision

**MSDN manual curation is the SOLE method.** The reflection tool option is removed.

Jeff's directive: "I don't need a reflection tool — we can get the data from the docs."

## Rationale

1. **Immediately actionable:** No tooling prerequisites. The baselines can be curated right now without building anything.
2. **Verifiable:** Every property and event is listed by name in the JSON, traceable to a specific MSDN URL.
3. **No SDK dependency:** The reflection approach required .NET Framework 4.8 SDK, which may not be available in all environments.
4. **Sufficient accuracy:** For the ~55 controls we track, manual curation from official documentation is more than adequate. The first-pass baselines flag 24 complex controls as `needs-verification` — these can be refined incrementally.

## Counting Rules Applied

Baselines follow rules symmetric with the BWFC counting rules (PRD §3.2):

- **Stop-points (Web Forms):** WebControl, Control, BaseDataBoundControl, DataBoundControl
- **Include:** Properties declared between the leaf class and stop-points (the "immediate family")
- **Exclude:** Style sub-object properties (map to RenderFragment in BWFC), ITemplate properties (map to RenderFragment), inherited base class members
- **Events:** Counted separately from properties. Only declared events, not inherited lifecycle events.

## Artifacts

| File | Purpose |
|------|---------|
| `dev-docs/reference-baselines.json` | The baseline data (61 components, property/event lists) |
| `dev-docs/tracked-components.json` | Component → Web Forms type mapping |
| `dev-docs/prd-component-health-dashboard.md` | Updated §3.2 and §7.3 to reflect this decision |

## Impact

- The `tools/WebFormsPropertyCounter/` directory reference is removed from the PRD
- The `source` field in `reference-baselines.json` reads `"MSDN .NET Framework 4.8 API documentation"`
- Future baseline refinements should cite specific MSDN URLs for traceability
- 24 complex controls are flagged `needs-verification` — the team should spot-check these against MSDN before the dashboard goes live
