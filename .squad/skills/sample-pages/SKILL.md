---
name: "sample-pages"
description: "Pattern for creating component sample/demo pages in the BlazorWebFormsComponents sample app"
domain: "sample-app"
confidence: "high"
source: "jubilee sprint-1"
---

## Context

Each component needs a sample page in the AfterBlazorServerSide sample app. This skill captures the conventions so new samples are consistent.

## Patterns

### File Location

```
samples/AfterBlazorServerSide/Components/Pages/ControlSamples/{ComponentName}/Index.razor
```

Do NOT use the older `Pages/ControlSamples/` path.

### Page Structure

```razor
@page "/ControlSamples/{ComponentName}"
@using BlazorWebFormsComponents
@using BlazorWebFormsComponents.Enums

<PageTitle>{ComponentName} Sample</PageTitle>

<h2>{ComponentName} Component Samples</h2>

<p>Brief description mentioning the Web Forms equivalent.</p>

<hr />

<h3>Feature Section</h3>
<p>Explanation of what this demo shows.</p>

<div class="demo-section">
    <!-- Live component demo -->
</div>

<p>Code:</p>
<pre><code><!-- Escaped markup showing usage --></code></pre>

<hr />

<!-- More sections... -->

@code {
    // State for demos
}
```

### Navigation Updates (REQUIRED)

When adding a new sample page, update BOTH files:

1. **`Components/Layout/NavMenu.razor`** — Add a `<TreeNode>` in the correct category, alphabetically ordered
2. **`Components/Pages/ComponentList.razor`** — Add an `<li><a>` link in the correct category, alphabetically ordered

### Content Guidelines

- Show the most common Web Forms migration scenario first (basic usage)
- Demonstrate multiple features: properties, events, styling, visibility
- Include `<pre><code>` blocks with escaped Blazor markup (use `@@` for `@`)
- Use `<hr />` between sections
- Mention the Web Forms equivalent where helpful (e.g., "In Web Forms this was `<asp:Calendar>`")
- Keep `@code {}` block at the bottom of the file

### Code Block Escaping

In `<pre><code>` blocks:
- `@` becomes `@@`
- `<` becomes `&lt;`
- `>` becomes `&gt;`

## Examples

See these files for reference:
- `Components/Pages/ControlSamples/Image/Index.razor` — good structure with demo sections
- `Components/Pages/ControlSamples/CheckBox/Index.razor` — good event handling demos
- `Components/Pages/ControlSamples/Calendar/Index.razor` — comprehensive styled demos

## Anti-Patterns

- **Missing PageTitle** — Every page should have `<PageTitle>`
- **No code snippets** — Always include `<pre><code>` showing the markup
- **Forgetting navigation** — Must update BOTH NavMenu.razor AND ComponentList.razor
- **Using Pages/ instead of Components/Pages/** — Use the newer path only
