# Decision: Divergence Registry D-11 through D-14

**Date:** 2026-02-28
**Author:** Forge (Lead / Web Forms Reviewer)
**Issue:** #388
**Status:** Recorded

## Context

The M15–M18 HTML fidelity audit identified 4 new divergence patterns not covered by D-01 through D-10. These were flagged in multiple audit reports (M11, M12, M15 data control analysis, post-fix capture results, HTML fidelity master report) and needed formal registry entries.

## Decisions Made

### D-11: GUID-Based IDs — Fix, don't register as permanent intentional
- **Affected:** CheckBox, RadioButton, RadioButtonList, FileUpload
- **Decision:** These controls should be fixed to use developer-provided ID + `_0`/`_1` suffix convention per Web Forms. GUIDs make HTML non-deterministic and untargetable by CSS/JS. This is a bug, not an architectural divergence.
- **Registered temporarily** so the audit pipeline can normalize GUIDs while the fix is pending.

### D-12: Boolean Attribute Format — Intentional, no fix
- **Affected:** Any control rendering boolean HTML attributes (selected, checked, disabled)
- **Decision:** `selected=""` vs `selected="selected"` is a platform-level difference. Both are valid HTML5. Register as intentional. Add normalizer rule to canonicalize before comparison.

### D-13: Calendar Previous-Month Day Padding — Fix recommended
- **Affected:** Calendar
- **Decision:** Web Forms renders a full 42-cell grid with adjacent-month day numbers. This is visible structural content, not infrastructure. Blazor Calendar should match this layout. Registered to track until fixed.

### D-14: Calendar Style Property Pass-Through — Fix progressively
- **Affected:** Calendar
- **Decision:** The Calendar's style sub-properties (TitleStyle, DayStyle, TodayDayStyle, etc.) are not fully applied to rendered HTML. This is a significant fidelity gap. Fix progressively, prioritizing TitleStyle, DayStyle, and TodayDayStyle first.

## Rationale

The divergence registry is the authoritative reference for classifying audit findings. Without these entries, auditors would repeatedly investigate these patterns as potential bugs. D-11, D-13, and D-14 are tracked as "fix recommended" rather than "intentional" — they are registered for visibility but should be resolved.
