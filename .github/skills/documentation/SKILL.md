---
name: documentation
description: Guidance for writing MkDocs documentation for BlazorWebFormsComponents. Use this when creating or updating component documentation, migration guides, or utility feature docs in the /docs folder.
---

# Documentation Skill for BlazorWebFormsComponents

This skill provides guidance for writing documentation for the BlazorWebFormsComponents library. Use this when creating or updating component documentation, migration guides, or utility feature docs.

## Documentation Philosophy

The documentation serves developers migrating ASP.NET Web Forms applications to Blazor. Every document should:
1. Help developers understand what IS and ISN'T supported
2. Show side-by-side Web Forms → Blazor syntax comparisons
3. Provide practical, copy-paste-ready examples
4. Link to original Microsoft documentation for reference

## Component Documentation Template

When documenting a component, use this exact structure:

```markdown
# [ComponentName]

[One paragraph describing what this component does and why it exists in this library]

Original Microsoft implementation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.[componentname]?view=netframework-4.8

## Features Supported in Blazor

- [List each supported feature as a bullet point]
- [Include supported events with brief description]
- [Note any Blazor-specific enhancements]

### Blazor Notes

[Any Blazor-specific implementation details or behavioral differences]

## Web Forms Features NOT Supported

- **[FeatureName]** - [Brief explanation of why not supported or what to use instead]
- **PostBackUrl** - Not supported; Blazor uses component events instead
- **ViewState serialization** - Not needed; state is preserved in component fields

## Web Forms Declarative Syntax

```html
<asp:[ComponentName]
    [All attributes from original Web Forms control]
    [Alphabetically ordered]
    [Include runat="server" at the end]
/>
```

## Blazor Syntax

```razor
<[ComponentName]
    [Supported attributes only]
    [Same order as Web Forms section for easy comparison]
/>
```

## Usage Notes

[Practical advice for migration]
[Common pitfalls to avoid]
[Performance considerations if any]

## Examples

### Basic Usage

```razor
@* Description of what this example shows *@
<[ComponentName] Property="value" OnEvent="Handler" />

@code {
    void Handler()
    {
        // Implementation
    }
}
```

### [Scenario Name]

```razor
@* More complex example *@
```

## See Also

- [Link to related component]
- [Link to migration guide if relevant]
- [Link to live sample page]
```

## Writing Style Guidelines

### Tone
- Professional but approachable
- Acknowledge that migration is work, but this library helps
- Be direct about what's NOT supported rather than hiding it

### Code Examples
- Always show WORKING code that can be copy-pasted
- Include `@code` blocks with event handlers
- Use realistic property names and values
- Comment complex sections

### Web Forms Syntax Blocks
- Copy the FULL attribute list from Microsoft docs
- Include ALL attributes even if not supported (helps with migration)
- Keep attributes alphabetically ordered within logical groups
- Always include `runat="server"` at the end

### Blazor Syntax Blocks
- Show only SUPPORTED attributes
- Maintain same order as Web Forms for easy comparison
- Omit `runat` attribute (not used in Blazor)
- Use Blazor event syntax (`OnClick` not `OnClick="Handler"`)

## Migration Guide Template

```markdown
# Migrating [Feature/Pattern]

## Overview

[What this guide covers and who it's for]

## Prerequisites

- [Required knowledge]
- [Required tools/versions]

## Step-by-Step Migration

### Step 1: [Action]

**Before (Web Forms):**
```aspx
[Web Forms code]
```

**After (Blazor):**
```razor
[Blazor code]
```

[Explanation of changes]

### Step 2: [Action]

[Continue pattern...]

## Common Issues

### [Issue Name]
**Problem:** [Description]
**Solution:** [How to fix]

## Next Steps

- [What to migrate next]
- [Related guides]
```

## Utility Feature Documentation Template

```markdown
# [FeatureName]

## Background

[Why this feature existed in Web Forms]
[What problem it solved]

## Web Forms Usage

```csharp
// How it was used in Web Forms
```

## Blazor Implementation

[How this library implements the feature]
[Key differences from Web Forms]

```csharp
// How to use in Blazor
```

## Migration Path

[How to update existing code]
[Recommended Blazor alternatives if applicable]

## Moving On

[Recommendations for properly refactoring away from this legacy pattern]
```

## File Naming Conventions

- Use PascalCase for component docs: `Button.md`, `GridView.md`
- Use kebab-case for guides: `migration-readiness.md`, `master-pages.md`
- Exception: `readme.md` for index pages (lowercase)

## MkDocs Integration

After creating documentation:
1. Add entry to `mkdocs.yml` in appropriate `nav:` section
2. Maintain alphabetical order within categories
3. Use descriptive nav labels matching the component name

```yaml
nav:
  - Editor Controls:
    - Button: EditorControls/Button.md      # Format: "Label: path/file.md"
    - NewComponent: EditorControls/NewComponent.md
```

## Admonition Usage

Use these sparingly for important callouts:

```markdown
!!! note "Migration Tip"
    Helpful information for migration

!!! warning "Breaking Change"
    Something that differs significantly from Web Forms

!!! danger "Not Supported"
    Feature that cannot be migrated directly

!!! tip "Best Practice"
    Recommended Blazor pattern to use instead
```

## Cross-References

Link to related documentation:
- Other components: `[Button](../EditorControls/Button.md)`
- Migration guides: `[Getting Started](../Migration/readme.md)`
- Live samples: `[Live Demo](https://blazorwebformscomponents.azurewebsites.net)`
- Microsoft docs: Full URL to docs.microsoft.com

## Sample Page Template (AfterBlazorServerSide Project)

When creating sample pages in `samples/AfterBlazorServerSide/Components/Pages/ControlSamples/`, follow this structure to help developers see both the working demo AND the code that creates it:

```razor
@page "/samples/[componentname]/[scenario]"

<PageTitle>[ComponentName] - [Scenario]</PageTitle>

<h1>[ComponentName] Sample</h1>

<p>[Brief description of what this sample demonstrates]</p>

<div class="demo-container">
    <h2>Demo</h2>

    @* The actual working component demo *@
    <Button Text="Click Me" OnClick="HandleClick" />

    @if (ClickCount > 0)
    {
        <p>Button clicked @ClickCount times</p>
    }
</div>

<div class="code-container">
    <h2>Source Code</h2>

    <pre><code class="language-razor">@@page "/samples/[componentname]/[scenario]"

&lt;h1&gt;[ComponentName] Sample&lt;/h1&gt;

&lt;Button Text="Click Me" OnClick="HandleClick" /&gt;

@@if (ClickCount &gt; 0)
{
    &lt;p&gt;Button clicked @@ClickCount times&lt;/p&gt;
}

@@code {
    private int ClickCount = 0;

    private void HandleClick()
    {
        ClickCount++;
    }
}</code></pre>
</div>

@code {
    private int ClickCount = 0;

    private void HandleClick()
    {
        ClickCount++;
    }
}
```

### Sample Page Guidelines

1. **Two Sections Required:**
   - **Demo Section** - The working, interactive component
   - **Source Code Section** - A readable code block showing exactly what's in the demo

2. **Code Block Formatting:**
   - Use `<pre><code class="language-razor">` for syntax highlighting
   - HTML-encode special characters: `<` becomes `&lt;`, `>` becomes `&gt;`
   - Preserve `@` symbols by doubling them: `@@code`, `@@if`, `@@page`
   - Show the COMPLETE code including `@code` block with event handlers
   - Include all relevant markup from the demo section

3. **Keep Demo and Code in Sync:**
   - The code block must match the demo exactly
   - If you change the demo, update the code block
   - Don't simplify or abbreviate the code block

4. **Organization:**
   - Use clear headings: "Demo" and "Source Code"
   - Add brief description at the top explaining what the sample demonstrates
   - Use CSS classes `demo-container` and `code-container` for styling

5. **Complex Samples:**
   - For samples with multiple code files, show the main component code
   - Link to GitHub for complete source if needed
   - Use tabs or accordion for multiple code examples

### Example: Data-Bound Component Sample

```razor
@page "/samples/gridview/basic"

<PageTitle>GridView - Basic Example</PageTitle>

<h1>GridView Sample</h1>

<p>This sample demonstrates basic GridView usage with static data.</p>

<div class="demo-container">
    <h2>Demo</h2>

    <GridView DataSource="@Products" AutoGenerateColumns="true" />
</div>

<div class="code-container">
    <h2>Source Code</h2>

    <pre><code class="language-razor">@@page "/samples/gridview/basic"

&lt;GridView DataSource="@@Products" AutoGenerateColumns="true" /&gt;

@@code {
    private List&lt;Product&gt; Products = new()
    {
        new Product { Id = 1, Name = "Product A", Price = 10.99m },
        new Product { Id = 2, Name = "Product B", Price = 20.99m }
    };

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}</code></pre>
</div>

@code {
    private List<Product> Products = new()
    {
        new Product { Id = 1, Name = "Product A", Price = 10.99m },
        new Product { Id = 2, Name = "Product B", Price = 20.99m }
    };

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
```

## Quality Checklist

Before submitting documentation:
- [ ] Follows the appropriate template
- [ ] Includes Web Forms AND Blazor syntax
- [ ] Lists ALL supported features
- [ ] Explicitly states unsupported features
- [ ] Has working code examples
- [ ] Added to `mkdocs.yml` nav
- [ ] Links to Microsoft reference documentation
- [ ] No broken internal links
- [ ] Spell-checked

Before submitting sample pages:
- [ ] Includes both demo and source code sections
- [ ] Code block exactly matches the demo
- [ ] HTML entities properly encoded in code block
- [ ] `@` symbols doubled in code block
- [ ] Includes complete `@code` block with all handlers
- [ ] Brief description explains what sample demonstrates
- [ ] Sample is accessible from navigation or component list
