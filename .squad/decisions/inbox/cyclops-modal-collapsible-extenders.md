# Decision: ModalPopup & CollapsiblePanel Extender Patterns

**Author:** Cyclops  
**Date:** 2026-03-16  
**Status:** Implemented  

## Context

Implementing ModalPopupExtender (#446) and CollapsiblePanelExtender (#447) required decisions about JS interop patterns for more complex DOM manipulation than previous extenders.

## Decisions

1. **ModalPopup overlay is dynamically created/destroyed** — The backdrop div is appended to `document.body` on show and removed on hide. This avoids requiring the developer to pre-create an overlay element.

2. **Focus trapping via keydown Tab interception** — Rather than using `inert` attribute (limited browser support in older contexts), focus is trapped by intercepting Tab/Shift+Tab on first/last focusable elements.

3. **OnOkScript/OnCancelScript use `new Function()`** — Matches the original Web Forms pattern where these are inline JS strings. Wrapped in try/catch for safety.

4. **CollapsiblePanel uses CSS transitions, not JS animation** — `transition: height 0.3s ease` for smooth collapse/expand. Initial state is set with transitions disabled, then re-enabled via `requestAnimationFrame`.

5. **ExpandDirection enum added** — Simple Vertical=0, Horizontal=1 enum in the Enums/ folder, matching the original toolkit's ExpandDirection.

## Impact

These patterns should be followed by any future complex extenders that need overlays, focus management, or animated size changes.
