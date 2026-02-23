# Decision: Menu Core Improvements (WI-19 + WI-21 + WI-23)

**Author:** Cyclops  
**Date:** 2026-02-24  
**Status:** Implemented  
**Branch:** milestone7/feature-implementation

## Context

Menu component needed three improvements: base class upgrade for styling, selection tracking with events, and missing core properties.

## Decisions

### WI-19: Menu inherits BaseStyledComponent

- Changed `Menu : BaseWebFormsComponent` → `Menu : BaseStyledComponent`
- Menu now gets BackColor, BorderColor, BorderStyle, BorderWidth, CssClass, Font, ForeColor, Height, Width from the base class
- `Menu.razor` root `<div>` renders `style="@Style"` and `class="@GetMenuCssClass()"` using the inherited `Style` property
- `GetMenuCssClass()` helper returns null when CssClass is empty (same pattern as Label)
- Existing sub-component styles (DynamicHoverStyle, StaticMenuItemStyle, etc.) remain unchanged — they are non-parameter properties set by child sub-components, completely independent from `[Parameter]` base class styles
- MenuItem.razor still inherits BaseWebFormsComponent (no styling needed on individual items)

### WI-21: Selection tracking and events

- `SelectedItem` (MenuItem, read-only) — set internally when a menu item is clicked
- `SelectedValue` (string, read-only) — computed from `SelectedItem?.Value`
- `MenuItemClick` (EventCallback\<MenuEventArgs\>) — fires when any menu item is clicked
- `MenuItemDataBound` (EventCallback\<MenuEventArgs\>) — fires after each data-bound MenuItem is created
- Created `MenuEventArgs` class with `Item` property (follows TreeNodeEventArgs pattern)
- MenuItem calls `ParentMenu.NotifyItemClicked(this)` via `@onclick` handler
- `@onclick:preventDefault` only applies when `NavigateUrl` is empty (preserves navigation for link items)

### WI-23: Core missing properties

- `MaximumDynamicDisplayLevels` (int, default 3) — limits depth of dynamic flyout menus
- `Target` (string) — default link target for menu items; MenuItem has its own `Target` that overrides via `EffectiveTarget`
- `SkipLinkText` (string, default "Skip Navigation Links") — rendered as `<a class="skip-link">` before the menu and an anchor `<a id="...SkipLink">` after; matches Web Forms pattern
- `PathSeparator` (char, default '/') — stored on Menu, used in MenuItem.ValuePath computation
- MenuItem gets `Value` (string) and `ValuePath` (string, computed) properties
- MenuItem `target` attribute changed from hardcoded `_blank` to `@EffectiveTarget` (item-level Target > Menu-level Target)

## Files Changed

- `src/BlazorWebFormsComponents/Menu.razor` — BaseStyledComponent inherits, style/class on root, skip link
- `src/BlazorWebFormsComponents/Menu.razor.cs` — Base class change, new properties, events, NotifyItemClicked
- `src/BlazorWebFormsComponents/MenuItem.razor` — Click handler, EffectiveTarget
- `src/BlazorWebFormsComponents/MenuItem.razor.cs` — Value, Target, ValuePath, EffectiveTarget, HandleClick
- `src/BlazorWebFormsComponents/MenuEventArgs.cs` — New file

## Risks

- `MenuItemDataBound` fires with `null` Item during RenderTreeBuilder execution (component isn't materialized yet). Consumers should use this for counting/logging, not item manipulation.
- `MaximumDynamicDisplayLevels` property is declared but not yet enforced in rendering logic — the JS interop and CSS-based flyout system would need updates to actually limit depth.
