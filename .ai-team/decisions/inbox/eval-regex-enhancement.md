# Decision: Enhance bwfc-migrate.ps1 with Eval Format-String and Simple String.Format Regexes

**Date:** 2026-03-04
**By:** Forge (Lead / Web Forms Reviewer)
**Status:** Proposed

## Context

The Run 2 and Run 3 migration reports listed `<%#: Eval("Total", "{0:C}") %>` as an "unconverted pattern requiring Layer 2." This is inaccurate — BWFC's `DataBinder.Eval` fully supports format strings, and the script already converts single-arg `<%#: Eval("prop") %>`. Only the two-argument form was missed.

Additionally, simple `<%#: String.Format("{0:c}", Item.Property) %>` patterns are mechanically convertible.

## Recommendation: Add 2 regex transforms to `ConvertFrom-Expressions`

### Transform 1: Eval with format string
```
Pattern:  <%#:\s*Eval\("(\w+)",\s*"\{0:([^}]+)\}"\)\s*%>
Replace:  @context.$1.ToString("$2")
Example:  <%#: Eval("Total", "{0:C}") %>  →  @context.Total.ToString("C")
```

### Transform 2: Simple String.Format with Item.Property
```
Pattern:  <%#:\s*String\.Format\("\{0:([^}]+)\}",\s*Item\.(\w+)\)\s*%>
Replace:  @($"{context.$2:$1}")
Example:  <%#: String.Format("{0:c}", Item.UnitPrice) %>  →  @($"{context.UnitPrice:c}")
```

## What should NOT be added (too complex for regex)

1. **Complex String.Format with arithmetic** — e.g., `<%#: String.Format("{0:c}", ((Convert.ToDouble(Item.Quantity)) * Convert.ToDouble(Item.Product.UnitPrice)))%>`. The expression body contains nested parentheses and method calls. Regex cannot reliably extract this. → Layer 2 (Copilot skill).

2. **GetRouteUrl** — e.g., `<%#: GetRouteUrl("ProductByNameRoute", new {productName = Item.ProductName}) %>`. Requires understanding of route table configuration and converting to Blazor `@page` patterns. Semantic, not mechanical. → Layer 2.

3. **Inline code blocks** — `<% } %>` and similar. Structural C# that requires understanding the surrounding `if`/`foreach` context. → Layer 2.

## Impact

Adding these two regexes would convert ~9 of the 18 currently-flagged manual items in WingtipToys:
- 7× `Eval("Property")` — already handled ✅
- 1× `Eval("Total", "{0:C}")` — Transform 1
- 3× `String.Format("{0:c}", Item.UnitPrice)` — Transform 2 (2 in ProductList, 1 in ProductDetails)

This would reduce the manual item count from 18 to ~14, pushing Layer 1 coverage from ~40% to ~45%.

## Decision needed

Should Cyclops implement these two regexes in `bwfc-migrate.ps1`? The changes are ~10 lines of code in the `ConvertFrom-Expressions` function, with well-defined test cases from WingtipToys source files.
