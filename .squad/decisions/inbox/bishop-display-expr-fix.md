# Decision: DisplayExpressionTransform emits idiomatic Razor for simple expressions

**Date:** 2026-05-06
**Author:** Bishop (Migration Tooling Dev)
**Status:** Implemented

## Decision

Update `DisplayExpressionTransform` so simple dotted identifier display expressions emit bare Razor `@expr`, while complex expressions continue to emit `@(expr)`.

## Scope

- Simple expressions match identifier chains such as `Item.ProductName`, `Item.UnitPrice`, `Model.Title`, and `user.Email`.
- Complex expressions keep parentheses, including method calls, operators, ternaries, indexers, and casts.
- Broken generated `@(: expr)` output is normalized through the same formatter so simple cases become idiomatic Razor too.

## Rationale

The old transform emitted `@(expr)` for every display expression. That output compiled, but it produced noisy, non-idiomatic Razor for the most common migrated shape: property access on the current item or model.

Using bare `@expr` for simple member chains keeps generated Razor closer to what a Blazor developer would naturally write, without sacrificing correctness for complex expressions that still require grouping.

## Files Updated

- `src\BlazorWebFormsComponents.Cli\Transforms\Markup\DisplayExpressionTransform.cs`
- `tests\BlazorWebFormsComponents.Cli.Tests\TransformUnit\DisplayExpressionTransformTests.cs`
- `tests\BlazorWebFormsComponents.Cli.Tests\TestData\expected\TC06-Expressions.razor`
- `tests\BlazorWebFormsComponents.Cli.Tests\TestData\expected\TC30-DataDrivenPage.razor`

## Validation

- `dotnet test tests\BlazorWebFormsComponents.Cli.Tests --nologo --filter "DisplayExpression"`
- `dotnet test tests\BlazorWebFormsComponents.Cli.Tests --nologo`
