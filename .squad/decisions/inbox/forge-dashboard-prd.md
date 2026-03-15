# Decision: Component Health Dashboard PRD Approved

**Date:** 2026-07-25
**Author:** Forge (Lead / Web Forms Reviewer)
**Scope:** Dashboard architecture, data model, scoring

## Decision

The Component Health Dashboard must follow the PRD at `dev-docs/prd-component-health-dashboard.md` before any implementation code is written. Key binding decisions:

1. **Property counting uses hierarchy-walking with stop-types** — DeclaredOnly reflection from leaf class upward, stopping at BaseWebFormsComponent / BaseStyledComponent / BaseDataBoundComponent / DataBoundComponent<T>. No other counting approach is acceptable.

2. **RenderFragment parameters are excluded from ALL counts** — They are Blazor template slots with no Web Forms property equivalent. This is non-negotiable; counting them broke the first implementation.

3. **EventCallback parameters appear in events column ONLY** — Never double-counted as properties.

4. **Reference baselines must come from .NET Framework 4.8 metadata** — Not estimates. Either reflection-based tooling or verified MSDN documentation. Baselines must be created BEFORE dashboard code.

5. **Tracked components list is curated, not auto-detected** — A maintained list of ~55 component names. Auto-classification failed catastrophically in v1.

6. **Generic type names must strip arity suffix** — `GridView`1` → `GridView` before any lookup.

## Rationale

The first dashboard attempt was reverted after 10 distinct data accuracy bugs cascaded through the scoring system. Every decision above directly prevents a specific bug that actually shipped. The PRD documents all 10 pitfalls in §8.

## Impact

- All implementers MUST read §8 (Known Pitfalls) before writing any dashboard code
- Phase 1 (reference baselines) is a hard prerequisite for Phase 2 (health service)
- No dashboard code ships without passing the 10 acceptance criteria in §10
