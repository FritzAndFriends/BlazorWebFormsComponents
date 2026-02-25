# Decision: Menu styles use MenuItemStyle pattern, not UiTableItemStyle

**By:** Cyclops
**Date:** 2026-02-26
**Issue:** #360

## What

Menu style sub-components (DynamicMenuStyle, StaticMenuStyle, DynamicMenuItemStyle, StaticMenuItemStyle) inherit `MenuItemStyle` (which inherits `ComponentBase, IStyle`), NOT `UiTableItemStyle`. The `IMenuStyleContainer` interface exposes these as `MenuItemStyle` properties.

## Why

Menu styles produce CSS text rendered into an inline `<style>` block via `ToStyle()`. This is fundamentally different from GridView/Calendar styles which use `TableItemStyle` objects applied as HTML element attributes. Forcing Menu styles into the `UiTableItemStyle` hierarchy would require restructuring the entire Menu CSS rendering approach with no benefit.

## Impact

- Any new Menu style sub-components should inherit `MenuItemStyle`, not `UiTableItemStyle`
- When adding RenderFragment parameters to a component that already has `ChildContent`, tests mixing named RenderFragments with bare content must wrap the bare content in `<ChildContent>` tags
- The `CascadingValue` for `ParentMenu` now has `IsFixed="true"` for performance
