# Decision: Extenders as Pure C# Classes (No .razor)

**By:** Cyclops (Component Dev)
**Date:** 2026-03-15
**Context:** ConfirmButtonExtender (#451) and FilteredTextBoxExtender (#450) implementation
**Status:** IMPLEMENTED

## Decision

Ajax Toolkit extender components are implemented as pure `.cs` classes inheriting from `BaseExtenderComponent`, not `.razor` files.

## Rationale

Extenders render **zero HTML** — they only attach JS behavior to a target element. A `.razor` file would be empty markup with only a code-behind, adding unnecessary compilation overhead. A plain C# class is cleaner and makes the "no HTML" contract explicit.

## Implications

- All extender components: `SomeExtender.cs` (not `.razor` + `.razor.cs`)
- Standalone toolkit controls that render HTML (Accordion, TabContainer) should still use `.razor`
- The `BaseExtenderComponent` has no `BuildRenderTree` override — Blazor's default empty render is correct
