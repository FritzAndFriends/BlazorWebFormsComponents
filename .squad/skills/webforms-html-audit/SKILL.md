---
name: "webforms-html-audit"
description: "Provides patterns for automated HTML fidelity comparison between ASP.NET Web Forms controls and their BWFC Blazor equivalents using Playwright. Covers marker-based control isolation with data-audit attributes, HTML normalization rules for stripping ViewState and naming-container IDs, and control classification by audit feasibility (Clean, Structural, JS-coupled, Divergent). Use when validating migration output fidelity, setting up Playwright-based HTML comparison tests, building an intentional divergence registry, or auditing whether migrated Blazor components render equivalent DOM structure."
domain: "testing-and-validation"
confidence: "low"
source: "earned"
---

## Context

When migrating ASP.NET Web Forms controls to Blazor components, HTML output fidelity is critical — existing CSS and JavaScript targeting the original HTML structure must continue to work. Automated comparison of rendered HTML between Web Forms and Blazor requires careful normalization to avoid false positives from framework infrastructure differences.

## Patterns

### Marker Isolation for Control HTML

Wrap Web Forms controls in sample pages with unique marker elements to enable programmatic extraction of just the control's structural HTML:

```html
<!-- In the .aspx sample page -->
<div data-audit-control="Button" data-audit-id="styled-button">
    <asp:Button runat="server" ID="styleButton" BackColor="Blue" Text="Blue Button" />
</div>
```

Use `data-audit-control` and `data-audit-id` attributes rather than `<hr>` elements — they're invisible to the user, semantically clear, and easy to query via Playwright's `page.locator('[data-audit-control="Button"]').innerHTML()`.

### HTML Normalization Rules

Before comparing Web Forms HTML against Blazor HTML, apply these normalizations:

1. **Strip ID prefixes** — Web Forms generates `ctl00_MainContent_X` naming-container IDs. Either strip entirely or normalize to just the control's local ID.
2. **Normalize __doPostBack** — Replace `href="javascript:__doPostBack('...','...')"` with a placeholder like `href="#postback"`. Blazor uses its own event mechanism.
3. **Strip WebResource.axd URLs** — Replace `src="/WebResource.axd?d=...&t=..."` with a normalized placeholder like `src="[framework-resource]"`.
4. **Remove ViewState/EventValidation** — These are page-level hidden inputs, not control output. Strip `<input type="hidden" name="__VIEWSTATE" ...>` and `<input type="hidden" name="__EVENTVALIDATION" ...>`.
5. **Normalize whitespace** — Collapse multiple whitespace characters, trim, and normalize line endings before comparison.
6. **Strip auto-generated JavaScript blocks** — Web Forms controls like TreeView and Menu inject `<script>` blocks with framework data. These have no Blazor equivalent.

### Control Classification for Audit Feasibility

Not all controls are equally suitable for automated HTML comparison:

- **Clean** — Single-element controls (Button→`<input>`, Label→`<span>`, Image→`<img>`, HyperLink→`<a>`). Near-exact match expected.
- **Structural** — Multi-element controls with self-contained HTML (GridView→`<table>`, DataList→`<table>`, Calendar→`<table>`). Match expected after normalization.
- **JS-coupled** — Controls that inject JavaScript for behavior (TreeView, Menu). Structural HTML can be compared but JS behavior cannot.
- **Divergent** — Controls where Blazor intentionally uses different rendering (Chart: server-image vs. canvas). Document and exclude from audit.

### Intentional Divergence Registry

Maintain a registry of deliberate HTML differences between Web Forms and Blazor. Each entry should include:
- Control name
- What differs (element, attribute, structure)
- Why it differs (architectural constraint, improvement, framework limitation)
- Impact on migration (CSS affected? JS affected? Visual change?)

## Anti-patterns

- **Comparing raw HTML without normalization** — Will produce 100% false-positive rate due to IDs, __doPostBack, and resource URLs.
- **Screenshot-only comparison** — Misses structural differences that affect CSS/JS targeting. Screenshots verify visual appearance, not DOM structure.
- **Attempting to reproduce __doPostBack** — This is Web Forms infrastructure. Blazor has its own event system. Never try to emit `__doPostBack` from a Blazor component.
- **Matching Web Forms naming-container IDs** — The `ctl00$MainContent$` prefix is a Web Forms artifact. Blazor components should use clean IDs.

## Checklist

- [ ] Sample page wraps each control instance with `data-audit-control` marker
- [ ] Playwright script extracts innerHTML between markers
- [ ] Normalization pipeline applied before comparison
- [ ] Intentional divergences documented in registry
- [ ] Each control classified as Clean/Structural/JS-coupled/Divergent
