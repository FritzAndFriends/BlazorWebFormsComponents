# Decision: M17 AJAX & Migration Helper Component Patterns

**By:** Cyclops
**Date:** 2026-02-28
**Issues:** #396, #397, #398, #399, #400, #401

## What

Six new components added for M17: Timer, ScriptManager, ScriptManagerProxy, UpdatePanel, UpdateProgress, Substitution.

### Key decisions:

1. **ScriptManager & ScriptManagerProxy are no-op stubs.** They render nothing and exist solely for migration compatibility — Blazor doesn't need AJAX script management. All properties are preserved as `[Parameter]` for markup compat.

2. **Timer shadows base `Enabled` with `new` keyword.** The base class `Enabled` property controls HTML `disabled` attribute rendering, but Timer renders no HTML. Timer.Enabled specifically controls whether the internal System.Threading.Timer is active.

3. **UpdatePanel uses ChildContent (not ContentTemplate).** Web Forms uses `<ContentTemplate>` inside UpdatePanel. In Blazor, the idiomatic pattern is `ChildContent` as a default `RenderFragment`. Users migrate by removing the `<ContentTemplate>` wrapper.

4. **UpdateProgress renders initially hidden.** Web Forms toggles visibility via JavaScript during async postbacks. In Blazor, users control visibility via their own state management. The component renders the initial hidden state matching Web Forms default output.

5. **Substitution uses `Func<HttpContext, string>` callback.** Web Forms uses a static method referenced by name. The Blazor equivalent accepts a delegate directly. `MethodName` is preserved as a string parameter for migration reference only.

6. **New categories in ComponentCatalog:** "AJAX" for Timer/UpdatePanel/UpdateProgress, "Migration Helpers" for ScriptManager/ScriptManagerProxy/Substitution.

## Why

These controls appear frequently in Web Forms applications. Even as stubs, they prevent compilation errors during migration and allow incremental removal of AJAX infrastructure.
