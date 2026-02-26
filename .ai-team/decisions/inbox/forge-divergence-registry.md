# Decision: M11-02 — Intentional Divergence Registry Created

**Date:** 2026-02-26
**Decided by:** Forge
**Milestone:** M11-02
**Status:** Completed

## Context

The HTML fidelity audit (M11–M13) will compare Web Forms gold-standard HTML against Blazor component output. Without a pre-defined registry of intentional divergences, auditors would repeatedly investigate platform-level differences (ID mangling, PostBack mechanisms, ViewState) that can never be replicated in Blazor and should not be treated as bugs.

## Decision

Created `planning-docs/DIVERGENCE-REGISTRY.md` with 10 documented intentional divergences (D-01 through D-10) covering:

- **D-01:** ID Mangling — `ctl00_` prefixes vs. direct IDs (ALL controls)
- **D-02:** PostBack Link Mechanism — `__doPostBack` vs. `@onclick` (GridView, Calendar, DetailsView, FormView, Menu, TreeView)
- **D-03:** ViewState Hidden Fields — `__VIEWSTATE`/`__EVENTVALIDATION` absent in Blazor (page-level)
- **D-04:** WebResource.axd URLs — embedded resource delivery replaced by static files (TreeView, Menu)
- **D-05:** Chart Rendering Technology — `<img>` vs. `<canvas>` (Chart — excluded from audit)
- **D-06:** Menu RenderingMode — Table mode not implemented, List mode only (Menu)
- **D-07:** TreeView JavaScript — `TreeView_ToggleNode` replaced by `@onclick` (TreeView)
- **D-08:** Calendar Day Selection — `__doPostBack` day links replaced by `@onclick` (Calendar)
- **D-09:** Login Control Infrastructure — visual shells only, no Membership provider (all 7 login controls)
- **D-10:** Validator Client-Side Scripts — `WebUIValidation.js` replaced by component validation (all 6 validators)

Each entry documents: control(s) affected, divergence description, category, reason, CSS impact, JS impact, and normalization rules for the audit pipeline.

## Impact

- Audit reports (M11-09, M12-06, M13-06) will reference D-XX entries when classifying intentional divergences
- The normalization pipeline (M11-06) will implement the normalization rules from each entry
- The registry will be updated incrementally as new divergences are discovered in M12 and M13
- Final version locked in M13-09

## Alternatives Considered

None — this was a planned deliverable per the M11 milestone plan.
