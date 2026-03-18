# Decision: Strip [RouteData] instead of replacing with [Parameter]

**Date:** 2026-03-08
**Author:** Bishop
**Status:** Implemented

## Context

Run 15 revealed that the `[RouteData]` → `[Parameter]` conversion in `bwfc-migrate.ps1` caused build failures (`CS0592`) in `ProductDetails.razor.cs` and `ProductList.razor.cs`. The `[Parameter]` attribute targets `Property` declarations only, but `[RouteData]` appears on **method parameters** in Web Forms model-binding signatures.

## Decision

**Strip `[RouteData]` from method parameters entirely.** Do not replace with `[Parameter]` inline.

- A `/* TODO */` block comment is placed above the parameter directing Layer 2 to create a `[Parameter]` property on the component class.
- Block comment (`/* */`) is used instead of line comment (`//`) to prevent absorbing the closing `)` of the method signature.

## Rationale

- `[RouteData]` is a Web Forms model-binding attribute with no inline Blazor equivalent
- `[Parameter]` cannot decorate method parameters — it targets properties only
- Layer 2 is the right place to refactor the method signature (promote parameters to component properties)
- Block comments are safe inside method parameter lists; line comments risk absorbing trailing syntax

## Impact

- **bwfc-migrate.ps1**: RouteData regex updated (lines ~1724-1732)
- **All L1 tests pass**: 15/15, 100% line accuracy
- **Layer 2 agents** should look for `/* TODO: RouteData parameter */` comments as signals to create `[Parameter]` properties
