# Decision: Menu auto-ID generation pattern

**By:** Cyclops
**Date:** 2026-02-24

## What

Menu component now auto-generates an ID (`menu_{GetHashCode():x}`) in `OnParametersSet` when no explicit `ID` parameter is provided. This ensures JS interop via `Sys.WebForms.Menu` always has a valid DOM element ID to target.

Additionally, `Menu.js` now has null safety (early return if element not found) and a try/catch around the constructor to prevent unhandled JS exceptions from crashing the Blazor circuit.

## Why

The Menu component's JS interop depends on a DOM element ID to find and manipulate the menu element. Without an ID, `document.getElementById('')` returns null, causing `TypeError: Cannot read properties of null (reading 'tagName')`. This crashed the entire Blazor circuit in headless Chrome environments.

## Impact

Any component that uses JS interop via element IDs should consider auto-generating IDs when none are provided. This pattern (`$"componentname_{GetHashCode():x}"` in `OnParametersSet`) could be reused by other components with JS interop dependencies.
