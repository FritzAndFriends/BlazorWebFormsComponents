# M17 Audit Fixes — Cyclops

**Date:** 2026-02-27
**Author:** Cyclops
**Milestone:** M17

## Decisions

1. **ScriptManager.EnablePartialRendering defaults to `true`** — Matches Web Forms. Any test asserting `false` default should be updated.

2. **ScriptManager now has a `Scripts` collection** — Same `List<ScriptReference>` pattern as ScriptManagerProxy. Components consuming ScriptManager should be aware of this property.

3. **UpdateProgress renders `class` attribute conditionally** — Uses `string.IsNullOrEmpty(CssClass) ? null : CssClass` pattern so the `class` attribute is omitted entirely when no CssClass is set, keeping HTML clean.

4. **UpdateProgress non-dynamic mode uses `display:block;visibility:hidden;`** — Matches Web Forms explicit style rendering. Tests checking the non-dynamic div style should expect both properties.

5. **ScriptReference expanded with ScriptMode, NotifyScriptLoaded, ResourceUICultures** — These are migration-compatibility properties from `System.Web.UI.ScriptReference`. No behavioral impact since ScriptManager/ScriptManagerProxy are no-op stubs, but allows markup to transfer without errors.
