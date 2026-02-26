# Decision: Conditional HTML attribute rendering pattern

**Author:** Cyclops
**Date:** M15
**Context:** Bug fixes #380, #379, #378

## Decision

For conditional HTML attribute rendering in Blazor components:

1. **Use helper methods returning null** to suppress attributes (e.g., `GetLongDesc()`, `GetCssClassOrNull()`). Blazor does not render attributes with null values.
2. **For ordered lists**, use CSS `list-style-type` only — do NOT use the HTML `type` attribute on `<ol>`. WebForms doesn't render `type`.
3. **Disabled state class handling** on button-like components should follow the Button pattern: append `aspNetDisabled` to CssClass when `Enabled=false`. This applies to LinkButton and any future button-like components.

## Rationale

WebForms audit HTML shows these are the exact patterns used by .NET Framework. Matching these patterns ensures CSS/JS compatibility after migration.
