# Decision: Stable cart session key for CLI transforms

**Date:** 2026-05-07T13:58:11-04:00  
**Author:** Rogue (QA Analyst)  
**Requested by:** Jeffrey T. Fritz  
**Status:** Proposed

## Context

Run 40 exposed a migration gap in benchmark cart flows: generated code that used `Session.Id` directly for cart lookups was not stable enough under Blazor SSR. Cart and basket code paths need an explicit session-backed identifier that survives the benchmark flow more reliably than the raw session ID.

## Decision

Add a dedicated CLI code-behind transform named `CartSessionKeyTransform` that:

- targets cart/basket-oriented statements still using `Session.Id` or `HttpContext.Session.Id`
- injects a single `GetOrCreateCartKey()` helper into the generated partial class
- stores the stable value in `Session["cart-key"]`
- rewrites matching cart service calls and cart ID assignments to use that helper
- leaves unrelated `Session.Id` usage untouched to avoid over-matching

## QA Notes

- Added focused transform-unit coverage for cart assignment rewrites, cart service call rewrites, non-cart preservation, helper idempotence, and default-pipeline registration.
- Updated CLI docs so the transform catalog and overview mention the new cart session-key stabilization step.
- Full CLI test execution is currently blocked by unrelated workspace changes in `src\BlazorWebFormsComponents.Cli\Pipeline\PageQuarantineDetector.cs`, which fail the CLI build before the new tests can run.

## Files Affected

- `src/BlazorWebFormsComponents.Cli/Transforms/CodeBehind/CartSessionKeyTransform.cs`
- `src/BlazorWebFormsComponents.Cli/Program.cs`
- `tests/BlazorWebFormsComponents.Cli.Tests/TestHelpers.cs`
- `tests/BlazorWebFormsComponents.Cli.Tests/TransformUnit/CartSessionKeyTransformTests.cs`
- `docs/cli/index.md`
- `docs/cli/transforms.md`
