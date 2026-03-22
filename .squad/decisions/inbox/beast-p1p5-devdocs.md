# Decision: P1-P5 Developer Documentation Scope

**Decided by:** Beast  
**Date:** 2026-03-21  
**Scope:** Documentation

## Decision

The P1–P5 Custom Controls framework documentation was placed in `dev-docs/proposals/p1-p5-custom-controls-framework.md` as **internal contributor documentation**, not in `docs/` (user-facing MkDocs). This is because:

1. The API reference covers `protected virtual` members and internal architecture — not relevant to library consumers who use the pre-built BWFC components.
2. The migration patterns target developers building **new custom controls** on top of the framework, which is a contributor activity.
3. User-facing migration guides (already in `docs/Migration/CustomControl-BaseClasses.md`) cover the end-user perspective.

## Impact

- Future documentation about the CustomControls namespace should go in `dev-docs/`, not `docs/`.
- If the framework becomes a public extensibility point (users building custom controls against BWFC base classes), the API reference sections should be promoted to `docs/`.
