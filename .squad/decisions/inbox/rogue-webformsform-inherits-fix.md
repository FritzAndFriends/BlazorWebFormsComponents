# WebFormsForm must inherit ComponentBase explicitly

**Author:** Rogue (QA)  
**Date:** 2026-07  
**Scope:** WebFormsForm.razor, RequestShim.cs  
**Issue:** #533

## Decision

Any `.razor` component in the main project that should NOT be a Web Forms control must explicitly declare `@inherits ComponentBase` to override the project-level `_Imports.razor` (which specifies `@inherits BaseWebFormsComponent`).

## Bugs Found

1. **WebFormsForm.razor** — Missing `@inherits ComponentBase` caused it to inherit `BaseWebFormsComponent` via `_Imports.razor`. Both classes had `[Parameter(CaptureUnmatchedValues = true)]`, throwing `ThrowForMultipleCaptureUnmatchedValuesParameters` at render time. Fixed by adding `@inherits ComponentBase`.

2. **RequestShim.cs line 79** — `new FormShim(null)` was ambiguous between `FormShim(IFormCollection?)` and `FormShim(Dictionary<string, StringValues>)` after the dual-mode constructor was added. Fixed by casting to `(IFormCollection?)null`.

## Impact

Both fixes are required for the WebFormsForm component to render at all. Without them, any page using `<WebFormsForm>` crashes at component initialization.
