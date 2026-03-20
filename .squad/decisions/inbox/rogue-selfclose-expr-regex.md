### SelfClosingAspTagRegex breaks on expression placeholders in attribute values

**Found by:** Rogue (QA Analyst)  
**Date:** 2026-07-25  
**Branch:** `experiment/aspx-middleware`  
**Severity:** P3 (edge case, workaround exists)

## Problem

`SelfClosingAspTagRegex` uses `<(asp_\w+)([^>]*?)\s*/>` to expand self-closing asp tags into explicit open/close pairs before AngleSharp parsing. The `[^>]*?` character class cannot match the `>` character that appears inside expression placeholders in attribute values.

When `ReplaceExpressions` runs first, it turns:
```html
<asp:HyperLink NavigateUrl='<%# Eval("Url") %>' Text="Link" runat="server" />
```
into:
```html
<asp:HyperLink NavigateUrl='<!--___ASPX_EXPR_0___-->' Text="Link" runat="server" />
```

The `-->` in the placeholder contains `>`, which breaks the self-closing regex match. The tag is never expanded, AngleSharp treats it as an unclosed element, and subsequent sibling content gets swallowed as children.

## Impact

- Self-closing asp tags with expression-in-attribute values don't parse correctly
- Sibling nodes after the unclosed tag become children instead of siblings
- Only affects `<%# %>`, `<%= %>`, `<%$ %>` expressions inside attribute values on self-closing tags

## Workaround

Use explicit closing tags when expressions appear in attribute values:
```html
<asp:HyperLink NavigateUrl='<%# Eval("Url") %>' Text="Link" runat="server"></asp:HyperLink>
```

## Potential Fix

Change the expression placeholder format to avoid `>`:
- Option A: Use a non-HTML placeholder like `___ASPX_EXPR_0___` (no comment wrapper) inside attribute values
- Option B: Detect expressions in attribute values separately and store them in a lookup before the main `ReplaceExpressions` pass
- Option C: Use a regex for self-closing that is aware of quoted attribute values: `<(asp_\w+)((?:\s+\w+\s*=\s*(?:"[^"]*"|'[^']*'))*)\s*/>`

**Assigned to:** Cyclops (parser owner)
