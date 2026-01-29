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

## Creating Sample Pages for Components

**CRITICAL**: Every documented component MUST have a corresponding sample page in the AfterBlazorServerSide project.

### Sample Page Structure and Location

Samples are located in `samples/AfterBlazorServerSide/Components/Pages/ControlSamples/[ComponentName]/`

The folder structure mirrors the documentation categories:
- **Editor Controls**: `ControlSamples/[ComponentName]/`
- **Data Controls**: `ControlSamples/[ComponentName]/`
- **Validation Controls**: `ControlSamples/[ComponentName]/` (or `ControlSamples/Validations/`)
- **Navigation Controls**: `ControlSamples/[ComponentName]/`
- **Login Controls**: `ControlSamples/[ComponentName]/`

### Sample File Naming Conventions

1. **Basic Sample**: `Index.razor` - The main/default sample for the component
   - Route: `@page "/ControlSamples/[ComponentName]"`

2. **Additional Scenarios**: `[ScenarioName].razor` - Named after what they demonstrate
   - Route: `@page "/ControlSamples/[ComponentName]/[ScenarioName]"`
   - Examples: `Style.razor`, `Events.razor`, `JavaScript.razor`

3. **Navigation Helper**: `Nav.razor` - Optional component listing all samples for this component
   - Used to navigate between multiple samples for the same component

### Sample Page Required Structure

Every sample page MUST include:

1. **Route**: `@page "/ControlSamples/[ComponentName]"` or `@page "/ControlSamples/[ComponentName]/[Scenario]"`
2. **PageTitle**: `<PageTitle>[ComponentName] - [Scenario]</PageTitle>`
3. **Heading**: `<h1>[ComponentName] Sample</h1>` or `<h2>[ComponentName] - [Scenario]</h2>`
4. **Description**: Brief explanation of what the sample demonstrates
5. **Demo Section**: Working component implementation
6. **Code Display**: Showing the markup and code being used (optional but recommended)

### Sample Page Template

```razor
@page "/ControlSamples/[ComponentName]"
@page "/ControlSamples/[ComponentName]/[Scenario]"

<PageTitle>[ComponentName] Sample</PageTitle>

<h2>[ComponentName] - [Scenario Description]</h2>

<p>[Brief description of what this sample demonstrates]</p>

@* Optional: Navigation to other samples for this component *@
<Nav />

@* The working demo *@
<div class="demo-section">
    <[ComponentName]
        Property1="value"
        Property2="@someValue"
        OnEvent="HandleEvent" />
</div>

@* Optional but recommended: Show the results or state *@
@if (!string.IsNullOrEmpty(Message))
{
    <div class="result">@Message</div>
}

@code {
    private string Message = "";

    private void HandleEvent()
    {
        Message = "Event handled!";
    }
}

<hr />

@* Optional: Display the code being used *@
<p>Code:</p>
<code>
&lt;[ComponentName] Property1="value" OnEvent="HandleEvent" /&gt;
</code>
```

### Creating Samples for Different Scenarios

When a component has multiple features to demonstrate, create separate sample pages:

```
ControlSamples/
  Button/
    Index.razor           # Basic button usage
    Style.razor           # Styling examples
    JavaScript.razor      # JavaScript integration
    Nav.razor            # Navigation between samples
```

Each scenario page should focus on ONE specific feature or use case.

## Linking Samples in Navigation

**CRITICAL**: After creating sample pages, they MUST be added to the navigation tree.

### Update NavMenu.razor

Location: `samples/AfterBlazorServerSide/Components/Layout/NavMenu.razor`

Add `<TreeNode>` entries under the appropriate category section:

```razor
<TreeNode Text="[Category] Components">

    @* For a component with a single sample *@
    <TreeNode Text="[ComponentName]" NavigateUrl="/ControlSamples/[ComponentName]" />

    @* For a component with multiple samples *@
    <TreeNode Text="[ComponentName]" NavigateUrl="/ControlSamples/[ComponentName]" Expanded="false">
        <TreeNode Text="Basic Usage" NavigateUrl="/ControlSamples/[ComponentName]" />
        <TreeNode Text="[Scenario 1]" NavigateUrl="/ControlSamples/[ComponentName]/[Scenario1]" />
        <TreeNode Text="[Scenario 2]" NavigateUrl="/ControlSamples/[ComponentName]/[Scenario2]" />
    </TreeNode>

</TreeNode>
```

**Navigation Categories**:
- `<TreeNode Text="Editor Components">` - For editor controls
- `<TreeNode Text="Data Components">` - For data-bound controls
- `<TreeNode Text="Validation Components">` - For validators
- `<TreeNode Text="Navigation Components">` - For navigation controls
- `<TreeNode Text="Login Components">` - For login/authentication controls

**Ordering**: Add new components alphabetically within their category.

### Example NavMenu Updates

```razor
@* Adding a new editor control *@
<TreeNode Text="Editor Components">
    <TreeNode Text="Button" NavigateUrl="/ControlSamples/Button">
        <TreeNode Text="JavaScript Click" NavigateUrl="/ControlSamples/Button/JavaScript" />
        <TreeNode Text="Style" NavigateUrl="/ControlSamples/Button/Style" />
    </TreeNode>
    <TreeNode Text="Label" NavigateUrl="/ControlSamples/Label" />  @* New component *@
    <TreeNode Text="TextBox" NavigateUrl="/ControlSamples/TextBox" />
</TreeNode>

@* Adding a new data control with multiple samples *@
<TreeNode Text="Data Components">
    <TreeNode Text="GridView" NavigateUrl="/ControlSamples/GridView" Expanded="false">
        <TreeNode NavigateUrl="/ControlSamples/GridView" Text="Simple GridView" />
        <TreeNode NavigateUrl="/ControlSamples/GridView/AutoGeneratedColumns" Text="Autogenerated Columns" />
        <TreeNode NavigateUrl="/ControlSamples/GridView/TemplateFields" Text="Template Fields" />
    </TreeNode>
</TreeNode>
```

## Updating the Home Page Component List

**CRITICAL**: After creating samples, update the component list on the home page.

Location: `samples/AfterBlazorServerSide/Components/Pages/ComponentList.razor`

Add links to the appropriate category column:

```html
<div class="col-md-3">
    <h3>[Category] Controls</h3>
    <ul>
        <li><a href="/ControlSamples/ExistingComponent">ExistingComponent</a></li>
        <li><a href="/ControlSamples/NewComponent">NewComponent</a></li>  @* Add alphabetically *@
        <li><a href="/ControlSamples/OtherComponent">OtherComponent</a></li>
    </ul>
</div>
```

**Categories**:
- **Editor Controls** - Simple UI components (Button, Label, TextBox, etc.)
- **Data Controls** - Data-bound components (GridView, Repeater, DataList, etc.)
- **Validation Controls** - Form validators
- **Navigation Controls** - Menu, TreeView, SiteMapPath
- **Login Controls** - Authentication/authorization components

**Ordering**: Add links alphabetically within each category.

## Updating the Repository README

**CRITICAL**: After documenting a component, update the main README.md at the repository root.

Location: `README.md` (repository root)

### For New Components

If adding a component that isn't already listed, add it to the appropriate category:

```markdown
## Blazor Components for Controls

There are a significant number of controls in ASP.NET Web Forms, and we will focus on creating components in the following order:

  - Editor Controls
    - [AdRotator](docs/EditorControls/AdRotator.md)
    - [Button](docs/EditorControls/Button.md)
    - [NewComponent](docs/EditorControls/NewComponent.md)  <-- Add alphabetically with link
    - [TextBox](docs/EditorControls/TextBox.md)
```

### For Existing Components (Adding Documentation)

If a component exists in the list without a documentation link, add the link:

```markdown
Before:
    - CheckBoxList

After:
    - [CheckBoxList](docs/EditorControls/CheckBoxList.md)
```

### README Categories

Match the category in the README to where you placed the documentation:
- `Editor Controls` → `docs/EditorControls/`
- `Data Controls` → `docs/DataControls/`
- `Validation Controls` → `docs/ValidationControls/`
- `Navigation Controls` → `docs/NavigationControls/`
- `Login Controls` → `docs/LoginControls/`

**Ordering**: Keep components alphabetically sorted within each category.

## Complete Documentation Workflow

When documenting a new or existing component, follow this complete workflow:

### 1. Create Component Documentation
- Location: `docs/[Category]/[ComponentName].md`
- Follow the component documentation template
- Include Web Forms and Blazor syntax comparisons
- Document supported and unsupported features
- Provide working code examples

### 2. Add to MkDocs Navigation
- File: `mkdocs.yml`
- Add entry under appropriate `nav:` section
- Format: `- ComponentName: Category/ComponentName.md`
- Maintain alphabetical order within category

### 3. Create Sample Page(s)
- Location: `samples/AfterBlazorServerSide/Components/Pages/ControlSamples/[ComponentName]/`
- Create `Index.razor` for basic usage
- Create additional scenario pages as needed (Style.razor, Events.razor, etc.)
- Follow sample page template and structure
- Include `@page` directive with correct route
- Add working component demonstration

### 4. Update Navigation Menu
- File: `samples/AfterBlazorServerSide/Components/Layout/NavMenu.razor`
- Add `<TreeNode>` entries under appropriate category
- Include all sample pages for the component
- Maintain alphabetical order within category

### 5. Update Home Page Component List
- File: `samples/AfterBlazorServerSide/Components/Pages/ComponentList.razor`
- Add link in appropriate category column
- Link to the main sample page: `/ControlSamples/[ComponentName]`
- Maintain alphabetical order within category

### 6. Update Repository README
- File: `README.md` (root)
- Add component with documentation link if new
- Update existing component entry to include link if adding documentation
- Format: `- [ComponentName](docs/Category/ComponentName.md)`
- Maintain alphabetical order within category

### Example: Complete Workflow for "Label" Component

```bash
# 1. Create documentation
docs/EditorControls/Label.md

# 2. Update mkdocs.yml
nav:
  - Editor Controls:
    - Label: EditorControls/Label.md

# 3. Create sample page
samples/AfterBlazorServerSide/Components/Pages/ControlSamples/Label/Index.razor

# 4. Update NavMenu.razor
<TreeNode Text="Label" NavigateUrl="/ControlSamples/Label" />

# 5. Update ComponentList.razor
<li><a href="/ControlSamples/Label">Label</a></li>

# 6. Update README.md
- [Label](docs/EditorControls/Label.md)
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
- [ ] Created in correct folder: `ControlSamples/[ComponentName]/`
- [ ] Follows naming convention (Index.razor for main sample)
- [ ] Includes `@page` directive with correct route
- [ ] Includes `<PageTitle>` element
- [ ] Has clear heading and description
- [ ] Contains working component demonstration
- [ ] Added to NavMenu.razor navigation tree
- [ ] Added to ComponentList.razor home page
- [ ] All samples for component are linked in navigation
- [ ] Routes match navigation URLs exactly

Before submitting README updates:
- [ ] Component added to or updated in correct category
- [ ] Documentation link format: `[ComponentName](docs/Category/ComponentName.md)`
- [ ] Alphabetically ordered within category
- [ ] Link verified to work (file exists at that path)
- [ ] Category matches documentation location
