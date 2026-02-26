# Session: HTML Audit Strategy Review

**Date:** 2026-02-25
**Requested by:** Jeffrey T. Fritz

## Who Worked

- **Forge** — evaluated Jeff's HTML audit strategy proposal

## What Was Done

Forge reviewed Jeff's Playwright-based HTML audit strategy for comparing Web Forms gold-standard HTML output against Blazor component output. The marker-isolation approach (wrap controls with marker HTML, extract via Playwright, compare structurally) was assessed for feasibility and completeness.

## Decisions Made

**Approved with 7 modifications:**

1. **Phase 0 for missing samples** — Only ~25% of controls have BeforeWebForms samples. A sample-writing sub-phase must precede capture.
2. **Normalization pipeline** — Raw Web Forms HTML contains infrastructure artifacts (`ctl00_` prefixes, `__doPostBack`, `WebResource.axd`, ViewState) that must be stripped before comparison.
3. **Divergence registry** — Create `planning-docs/intentional-divergences.md` documenting every case where Blazor HTML deliberately differs from Web Forms.
4. **`data-audit-control` attributes** — Use marker attributes for control isolation during capture.
5. **Exclude Chart** — Chart uses `<img src="ChartImg.axd">` in Web Forms vs Chart.js canvas in Blazor. HTML will never match; this is by design.
6. **Script CodeBehind→CodeFile conversion** — The dynamic compilation workaround must be committed or scripted so the pipeline runs on fresh clones.
7. **Start with Tier 1 controls** — Validate pipeline with Button, TextBox, HyperLink, DropDownList, Repeater, DataList first.

## Key Outcomes

- Strategy approved for implementation with the above modifications
- Intentional Divergence Registry required before audit begins
- Screenshot comparison deferred to a later phase (low value given structural comparison)
