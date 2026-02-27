# M17 Sample Pages Decision — Jubilee

**Date:** 2026-02-27
**Author:** Jubilee (Sample Writer)
**Requested by:** Jeffrey T. Fritz

## Decision

Created 5 sample pages for the M17 AJAX/Migration controls. ScriptManagerProxy sample was skipped per task spec (too similar to ScriptManager).

## Files Created

| Control | Path | Category |
|---------|------|----------|
| Timer | `Components/Pages/ControlSamples/Timer/Default.razor` | AJAX |
| UpdatePanel | `Components/Pages/ControlSamples/UpdatePanel/Default.razor` | AJAX |
| UpdateProgress | `Components/Pages/ControlSamples/UpdateProgress/Default.razor` | AJAX |
| ScriptManager | `Components/Pages/ControlSamples/ScriptManager/Default.razor` | Migration Helpers |
| Substitution | `Components/Pages/ControlSamples/Substitution/Default.razor` | Migration Helpers |

## Notes for Other Agents

- **ComponentCatalog.cs** already had entries for all 5 controls — no catalog updates were needed.
- Sample filenames use `Default.razor` (not `Index.razor`) per task spec. This is fine since routing is based on `@page` directives, not filenames.
- The ScriptManager sample includes a property reference table — if ScriptManager gains new properties, update the table.
- The Substitution sample uses `Func<HttpContext?, string>` callbacks — the `HttpContextAccessor` is injected in `BaseWebFormsComponent`.
- Timer sample uses `System.Threading.Timer` internally; the 2-second demo interval is fast enough to show it works without being annoying.
