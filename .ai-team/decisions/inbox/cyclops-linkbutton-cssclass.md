# Decision: Issue #379 — LinkButton CssClass Already Fixed

**Author:** Cyclops (Component Dev)
**Date:** 2026-02-27
**Status:** Verified — no code change needed

## Context

Issue #379 reported that `CssClass` on `LinkButton` was not being passed through to the rendered `class` attribute on the `<a>` element.

## Finding

The fix was already applied during M15 (commit `65aedc0`). The `LinkButton.razor` file already contains `class="@GetCssClassOrNull()"` on both `<a>` elements, and the `GetCssClassOrNull()` method correctly handles enabled/disabled states with `aspNetDisabled` class appending.

Six bUnit tests in `LinkButton/Format.razor` already cover all CssClass scenarios. All 25 LinkButton tests pass.

## Recommendation

Issue #379 can be closed as already resolved. No additional code changes are required.
