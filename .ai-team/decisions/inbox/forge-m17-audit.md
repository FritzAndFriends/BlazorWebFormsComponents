# Decision: M17 Audit Findings — Follow-Up Required

**Author:** Forge
**Date:** 2026-02-28
**Scope:** M17 AJAX / Migration Helper Controls

## Context

Completed formal Web Forms fidelity audit of all 6 M17 controls against Microsoft Learn .NET Framework 4.8.1 API docs. Full report at `planning-docs/M17-CONTROL-AUDIT.md`.

## Decisions

### 1. UpdateProgress CssClass rendering is a fidelity bug (Medium priority)

`UpdateProgress` inherits `BaseStyledComponent` (which provides `CssClass`) but the `.razor` template does not apply `class="@CssClass"` to the output `<div>`. This means migrating `<asp:UpdateProgress CssClass="loading">` silently drops the class. **This should be fixed before M17 ships.**

### 2. ScriptManager `EnablePartialRendering` default should be `true` (Low priority)

Web Forms defaults `EnablePartialRendering` to `true`. Our stub defaults to `false` (bare `bool`). While the stub is a no-op, migrating code that reads this property gets the wrong value. Recommend changing to `= true`.

### 3. No-op stub property coverage is intentionally limited

ScriptManager at 41% and ScriptManagerProxy at 50% of Web Forms properties is acceptable. The missing properties are deep AJAX infrastructure (history, composite scripts, authentication service, etc.) with no Blazor equivalent. We intentionally only include properties commonly found in declarative markup.

### 4. UpdatePanel `Triggers` collection deliberately omitted

Web Forms' `Triggers` collection is for specifying which controls trigger partial updates. Blazor's rendering model makes this unnecessary — all Blazor rendering is already partial. This is a deliberate architectural decision, not a gap.

## For Team Awareness

- Cyclops: Items 1 and 2 above are code fixes for a follow-up PR
- Beast: Audit report is at `planning-docs/M17-CONTROL-AUDIT.md` — reference it in docs if needed
- All: The audit confirms all 6 controls pass for the M17 PR with the noted caveats
