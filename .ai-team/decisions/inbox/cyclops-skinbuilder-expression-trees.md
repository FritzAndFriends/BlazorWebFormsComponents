# Decision: SkinBuilder uses expression trees for nested property access

**By:** Cyclops
**Date:** 2026-03-01
**Issues:** #364, #365

## What

The `SkinBuilder.Set<TValue>()` method uses `System.Linq.Expressions` to parse lambda expressions and set properties on `ControlSkin`. For nested properties like `s => s.Font.Bold`, it recursively navigates the expression tree, auto-creating intermediate objects (e.g. `FontInfo`) if null.

## Why

The fluent API spec requires `skin.Set(s => s.Font.Bold, true)` syntax. Expression tree parsing is the only way to support both direct and nested property setting with a single generic method. Alternative approaches (separate methods per property, string-based names) were rejected for type safety and API consistency.

## Impact

- Future properties added to `ControlSkin` are automatically supported without builder changes
- Adding sub-object properties (like a future `Padding` object) will just work
- Reflection-based, so slightly slower than direct assignment, but theme configuration happens once at startup
