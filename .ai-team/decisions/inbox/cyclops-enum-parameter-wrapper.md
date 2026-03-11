# Decision: EnumParameter<T> wrapping for all BWFC enum [Parameter] properties

**Author:** Cyclops  
**Date:** 2026-07-25  
**Status:** Implemented  
**Scope:** Library-wide

## What

All `[Parameter]` properties on BWFC components that use real enum types (not abstract class hierarchies) have been wrapped in `EnumParameter<T>`. This is a `readonly struct` with implicit conversions to/from both the enum type `T` and `string`.

## Why

Web Forms accepted bare string values in markup: `GridLines="None"`. Blazor requires Razor expressions: `GridLines="@(GridLines.None)"`. `EnumParameter<T>` bridges this gap — both syntaxes now work.

## Impact on other agents

### Rogue (test updates required)
4 test files have compilation errors from `ShouldBe()` calls on `EnumParameter<T>`. Fix: change `property.ShouldBe(EnumVal)` to `property.Value.ShouldBe(EnumVal)`.

### Beast (migration scripts)
L2 scripts that emit `@(EnumType.Value)` wrapping for enum attributes can now emit bare strings instead. This is a significant simplification.

### Forge (L2 automation analysis)
OPP-1 is now complete. The volume of L2 enum-escaping fixes should drop to near zero.

## Gotchas for everyone
- **Switch expressions** on `EnumParameter<T>` properties must use `.Value` — pattern matching ignores implicit conversions.
- **Shouldly/assertion libraries** need `.Value` — extension methods don't resolve through implicit struct conversions.
- **Abstract class hierarchies** (`DataListEnum`, `RepeatLayout`, `ButtonType`, `TreeViewImageSet`, `ValidationSummaryDisplayMode`) were NOT converted — they are not enums.
- **Nullable enum params** (`Docking?` on Chart components) were NOT converted — needs separate handling.
