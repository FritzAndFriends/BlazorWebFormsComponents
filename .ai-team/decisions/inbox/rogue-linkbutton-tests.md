# Decision: LinkButton CssClass Test Coverage Strategy

**By:** Rogue (QA Analyst)
**Date:** Issue #379
**Status:** Implemented

## Context

LinkButton has two render paths (PostBackUrl null vs non-null), both using `GetCssClassOrNull()`. Tests must cover both branches for CssClass behavior.

## Decision

Created dedicated `CssClass.razor` test file (8 tests) separate from `Format.razor` which already had some CssClass tests. Both files coexist — Format.razor tests are more integration-style (MarkupMatches), CssClass.razor tests are targeted attribute assertions covering edge cases.

## Edge Case Noted

`GetCssClassOrNull()` uses `string.IsNullOrEmpty()` not `string.IsNullOrWhiteSpace()`. Whitespace-only CssClass (e.g., `" "`) would render `class=" "` instead of being omitted. Low priority but worth fixing in a future cleanup pass.

## Impact

All team members should know: when testing any component that uses `CssClass`, verify both the "no class" case (`HasAttribute("class").ShouldBeFalse()`) and the disabled state (`aspNetDisabled` appended). AngleSharp omits `class` when the value is null.
