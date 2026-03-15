# Decision: BlazorAjaxToolkitComponents Project Structure

**Author:** Cyclops  
**Date:** 2026-03-15  
**Status:** Proposed  
**Issue:** #441  

## Context

We need a new project to house Blazor components emulating the ASP.NET Ajax Control Toolkit. The toolkit is a separate library from core Web Forms controls, so it warrants its own assembly.

## Decisions

1. **Separate project, same solution** — `BlazorAjaxToolkitComponents` lives in `src/` alongside the base library and is added to `BlazorMeetsWebForms.sln`.

2. **ProjectReference to BlazorWebFormsComponents** — The toolkit extends the base library (e.g., extenders may target BWFC controls). ProjectReference for now; will become a PackageReference when publishing.

3. **Package ID: `BlazorAjaxToolkitComponents`** — No `Fritz.` prefix. The base library uses `Fritz.BlazorWebFormsComponents` but the toolkit is a distinct package.

4. **BaseExtenderComponent extends ComponentBase, not BaseWebFormsComponent** — Toolkit extenders are behavioral attachments (they add JS behavior to a target control), not visual controls that render HTML. They don't need theming, visibility, or the Web Forms control lifecycle that BaseWebFormsComponent provides.

5. **Microsoft.JSInterop dependency** — Toolkit extenders need direct JS interop for client-side animations, collapsing, drag-and-drop, etc.

## Impact

- New NuGet package consumers will install
- Future components (CalendarExtender, CollapsiblePanelExtender, etc.) will inherit from BaseExtenderComponent
- Forge's architecture spec will finalize the full extender pattern
