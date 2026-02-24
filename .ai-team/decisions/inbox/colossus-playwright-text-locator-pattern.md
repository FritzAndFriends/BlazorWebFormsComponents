# Decision: Avoid bare `text=` locators in Playwright integration tests

**Author:** Colossus  
**Date:** 2025-07-24  
**Status:** Proposed  

## Context

Five integration tests failed in CI (PR #343) because `page.Locator("text=Label:")` matches the *innermost* element containing the text. When markup uses `<p><strong>Label:</strong> value</p>`, the locator returns the `<strong>`, excluding the sibling value text from `TextContentAsync()`. Additionally, bare `text=` locators cause strict-mode violations when the same text appears in both rendered output and code examples.

## Decision

All Playwright integration tests MUST use container-targeted locators instead of bare `text=` selectors when reading text content that includes a label and a value:

```csharp
// ❌ BAD — matches <strong>, returns only label text
var info = page.Locator("text=Selected index:");

// ✅ GOOD — matches the parent <p>, returns label + value
var info = page.Locator("p").Filter(new() { HasTextString = "Selected index:" });
```

For elements that might appear in multiple places (rendered output + code examples), target the specific rendered element type:

```csharp
// ❌ BAD — strict mode violation if text appears twice
var header = page.Locator("text=Widget Catalog");

// ✅ GOOD — targets only the rendered <td>
var header = page.Locator("td").Filter(new() { HasTextString = "Widget Catalog" }).First;
```

## Consequences

- Existing tests using bare `text=` locators for value extraction should be migrated.
- New tests must follow this pattern from the start.
- WaitForSelectorAsync calls should use specific selectors (e.g., `button:has-text('Edit')`) not generic element type selectors.
