# Decision: ToolTip promoted to BaseWebFormsComponent

**Date:** 2026-03-07
**Author:** Cyclops
**Context:** M20 Component Parity — Issues #15, #16, #17, #18

## Decision

Moved the `ToolTip` property from `BaseStyledComponent` up to `BaseWebFormsComponent`. This means every component in the library (not just styled ones) now has ToolTip support, matching Web Forms where `WebControl.ToolTip` is available on all controls.

## Rationale

- Web Forms `WebControl.ToolTip` is on the base class, not a styled subclass
- Components like `MenuItem` and `ChartSeries` that inherit `BaseWebFormsComponent` directly now get ToolTip from the base instead of needing their own duplicate declarations
- Removed 2 duplicate `ToolTip` properties (ChartSeries, MenuItem)
- `DataPoint` and `TreeNode` keep their own ToolTip — they don't inherit BaseWebFormsComponent

## Status of Issues #15, #17, #18

These were already implemented in the codebase:
- **#15 AccessKey** — already in BaseWebFormsComponent
- **#17 DataBoundComponent** — BaseDataBoundComponent already inherits BaseStyledComponent
- **#18 Image/Label** — both already inherit BaseStyledComponent

No further changes needed for those three issues.

## Impact

- 0 compilation errors, 1550 tests pass
- ~40 components now have ToolTip via inheritance
