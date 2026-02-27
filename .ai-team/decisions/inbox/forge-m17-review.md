### 2026-02-28: M17 AJAX Controls Gate Review — APPROVE WITH NOTES
**By:** Forge
**What:** Reviewed all 6 M17 controls (Timer, ScriptManager, ScriptManagerProxy, UpdatePanel, UpdateProgress, Substitution) for Web Forms fidelity against .NET Framework 4.8 API. Verdict: **APPROVE WITH NOTES** — ready to PR with 4 minor follow-up items.

**Follow-up items (non-blocking, file as issues):**
1. ScriptManager `EnablePartialRendering` should default to `true` (Web Forms default), not `false`.
2. ScriptManager should accept a `Scripts` collection property (like ScriptManagerProxy does) for full markup compatibility.
3. UpdateProgress template should render `CssClass` on the `<div>` element — currently the property is accepted but silently ignored.
4. UpdateProgress non-dynamic layout should render `style="display:block;visibility:hidden;"` to match Web Forms byte-for-byte (currently omits `display:block`).

**Why:** None of these block migration. Items 1–2 affect no-op stubs with zero runtime behavior. Items 3–4 are cosmetic HTML gaps. All can be addressed in a patch follow-up without design changes.
