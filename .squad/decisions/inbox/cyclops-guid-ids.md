# Decision: No GUID-based IDs in rendered HTML

**Date:** 2026-03-17
**Author:** Cyclops
**Issue:** #471

## Context

CheckBox, RadioButton, and RadioButtonList generated `Guid.NewGuid().ToString("N")` as fallback HTML `id` attributes when no developer ID was set. This polluted the DOM with unpredictable 32-char hex strings that break CSS selectors and JavaScript targeting.

## Decision

**Components must use `ClientID` (from `ComponentIdGenerator`) as the sole source for HTML `id` attributes.** No component should generate its own GUID for `id` or `for` attributes.

- When developer sets `ID="X"`: render `id="X"` (or `id="X_0"`, `id="X_1"` for list items)
- When no ID is set: omit the `id` and `for` attributes entirely
- The only acceptable GUID fallback is for the radio button `name` attribute (required for mutual exclusion grouping when neither `ID` nor `GroupName` is set)

## Impact

- All new components should follow this pattern
- CheckBoxList already uses a similar pattern with `_baseId` — may need the same fix if flagged
- FileUpload was already correct (only renders id when ClientID present)

## Status

Implemented. 2105/2105 tests pass.
