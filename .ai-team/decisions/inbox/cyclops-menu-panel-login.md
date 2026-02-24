# Decision: Menu Level Styles, Panel BackImageUrl, Login/ChangePassword Orientation

**Author:** Cyclops  
**Date:** 2026-02-24  
**Status:** Implemented  
**WIs:** WI-47, WI-48, WI-49

## WI-47 — Menu Level Styles

### Decision
Created `MenuLevelStyle` as a standalone class (not a ComponentBase sub-component) with public constructor implementing `IStyle`. Level style collections are `List<MenuLevelStyle>` parameters on Menu, not `RenderFragment` sub-components.

### Rationale
- Level styles are positional (index-based), unlike named sub-components (StaticMenuItemStyle, DynamicMenuItemStyle)
- A `List<T>` parameter is the natural API for ordered collections
- `MenuLevelStyle` needed a public constructor (unlike `Style`/`TableItemStyle` which have `internal` constructors) so users can instantiate them in code
- Follows the same `IStyle` contract so `ToStyle()` extension works for CSS generation

### Style Resolution Order
MenuItem applies styles in this priority:
1. LevelSelectedStyles (if item is selected and entry exists at depth index)
2. LevelMenuItemStyles (if entry exists at depth index)
3. Falls back to static/dynamic CSS class styles from `<style>` block

### Files Changed
- `MenuLevelStyle.cs` (new)
- `Menu.razor.cs` — added 3 List parameters + 3 internal getter helpers
- `MenuItem.razor` — added `GetItemStyle()`, `GetItemCssClass()`, `GetSubMenuStyle()` methods

## WI-48 — Panel BackImageUrl

### Decision
Added `BackImageUrl` string parameter to Panel. Renders as `background-image:url({value})` in the existing `BuildStyle()` method.

### Rationale
- Minimal change — one parameter, one style line in existing `BuildStyle()`
- Follows same pattern as other computed styles (HorizontalAlign, ScrollBars, Wrap)
- No new rendering elements needed

### Files Changed
- `Panel.razor.cs` — added parameter + style entry in `BuildStyle()`

## WI-49 — Login/ChangePassword Orientation + TextLayout

### Decision
- Created `LoginTextLayout` enum (TextOnLeft, TextOnTop) in `Enums/`
- Added `Orientation` and `TextLayout` parameters to both Login and ChangePassword
- Used `Enums.Orientation.Vertical` fully-qualified comparison in Razor `@code` blocks to avoid parameter/type name collision

### Razor Naming Collision
The parameter `Orientation` has the same name as the enum type `Orientation`. In Razor, this causes ambiguity. Resolution: helper properties `IsVertical`/`IsCpVertical` use `Enums.Orientation.Vertical` (namespace-qualified) to disambiguate. This follows the known M6 pattern documented by Jubilee.

### Layout Approach
- Vertical (default): fields in separate `<tr>` rows (original behavior)
- Horizontal: fields in `<td>` columns within same `<tr>`
- TextOnLeft (default): label beside input (original behavior)
- TextOnTop: label in separate row above input
- Dynamic `colspan` adjusts full-width rows (title, instructions, failure text, buttons)

### Files Changed
- `Enums/LoginTextLayout.cs` (new)
- `Login.razor.cs` — added Orientation + TextLayout parameters
- `Login.razor` — 4 layout branches + helper properties
- `ChangePassword.razor.cs` — added Orientation + TextLayout parameters
- `ChangePassword.razor` — 4 layout branches + helper properties
