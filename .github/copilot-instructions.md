# Copilot Instructions for BlazorWebFormsComponents

## Project Overview

BlazorWebFormsComponents is a library that provides Blazor components emulating ASP.NET Web Forms controls, enabling migration from Web Forms to Blazor with minimal markup changes. The project helps developers reuse their existing Web Forms markup in Blazor applications.

## Core Requirements

**ASP.NET Web Forms components that ship with .NET Framework 4.8 should be recreated as Blazor Components with:**

1. **Same Name** - The Blazor component must have the identical name as the original Web Forms control (e.g., `<asp:Button>` becomes `<Button>`, `<asp:GridView>` becomes `<GridView>`)

2. **Same Attributes and Properties** - Support the same attribute names and property signatures as the original control wherever possible. This enables developers to migrate markup with minimal changes (removing only `asp:` prefix and `runat="server"`)

3. **Identical HTML Output** - The rendered HTML must match what the original Web Forms control produces. This ensures:
   - Existing CSS styles continue to work
   - JavaScript that targets the HTML structure remains functional
   - Visual appearance is preserved after migration

### What This Means in Practice

```html
<!-- Original Web Forms -->
<asp:Button ID="btnSubmit" Text="Submit" CssClass="btn-primary" OnClick="Submit_Click" runat="server" />

<!-- Blazor Equivalent (should render identical HTML) -->
<Button Text="Submit" CssClass="btn-primary" OnClick="Submit_Click" />
```

Both should render:
```html
<button type="submit" class="btn-primary">Submit</button>
```

### Reference Documentation
- Original Web Forms controls: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols?view=netframework-4.8
- Use the .NET Framework 4.8 reference source to verify HTML output patterns

## Technology Stack

- **.NET Version**: .NET 10.0 (see `global.json` for SDK version)
- **Framework**: Blazor (Server-Side and WebAssembly)
- **Project Type**: Razor Class Library (`Microsoft.NET.Sdk.Razor`)
- **Testing Framework**: xUnit with bUnit for Blazor component testing
- **Assertion Library**: Shouldly
- **Mocking**: Moq
- **Build/Versioning**: Nerdbank.GitVersioning
- **CSS Utilities**: BlazorComponentUtilities

## Project Structure

```
/docs                                 -- User documentation (MkDocs)
/samples                              -- Usage samples
  BeforeWebForms/                     -- Original Web Forms sample (.NET Framework)
  AfterBlazorServerSide/              -- Blazor Server-Side samples
  AfterBlazorClientSide/              -- Blazor WebAssembly samples
  SharedSampleObjects/                -- Shared models/data for samples
/src
  BlazorWebFormsComponents/           -- Main component library
  BlazorWebFormsComponents.Test/      -- Unit tests with bUnit
```

## Component Architecture

### Base Classes Hierarchy

1. **`BaseWebFormsComponent`** - Root base class for all components
   - Inherits from `ComponentBase`
   - Implements `IAsyncDisposable`
   - Provides cascading parent component support
   - Defines obsolete Web Forms properties (`ID`, `ViewState`, `runat`)
   - Provides `Enabled`, `TabIndex`, `Visible` parameters

2. **`BaseStyledComponent`** - Adds styling support
   - Inherits from `BaseWebFormsComponent`
   - Implements `IStyle`
   - Provides `BackColor`, `ForeColor`, `BorderColor`, `CssClass`, `Font`, `Height`, `Width`

3. **`DataBoundComponent<TItemType>`** - For data-bound controls
   - Supports `DataSource`, `Items`, `SelectMethod`, `DataMember`
   - Handles `IEnumerable<T>` and `IListSource` (DataSet/DataTable)

### Component File Conventions

Components follow the code-behind pattern:
- `ComponentName.razor` - Markup/template file
- `ComponentName.razor.cs` - Partial class with logic and parameters

Example structure for a component:
```csharp
// Button.razor.cs
namespace BlazorWebFormsComponents
{
    public partial class Button : ButtonBaseComponent
    {
        [Parameter]
        public string ToolTip { get; set; }

        // Component-specific logic
    }
}
```

```razor
@* Button.razor *@
@inherits ButtonBaseComponent

@if (Visible)
{
    <button type="@CalculatedButtonType" @onclick="Click">@Text</button>
}
```

## Coding Conventions

### Naming
- Use PascalCase for public properties, methods, and parameters
- Parameters match original Web Forms attribute names for compatibility
- Internal/calculated properties prefixed with `Calculated` (e.g., `CalculatedCssClass`)

### Parameters
- All component attributes are decorated with `[Parameter]`
- Obsolete Web Forms features are marked with `[Obsolete]` attribute with migration guidance
- Use descriptive obsolete messages without emoji: `Use @ref instead of ID`

### Obsolete Pattern
```csharp
[Parameter, Obsolete("Use @ref instead of ID")]
public string ID { get; set; }

[Parameter, Obsolete("In Blazor PostbackURL is not supported")]
public string PostBackUrl { get; set; }
```

### Event Handling
- Use `EventCallback` and `EventCallback<T>` for component events
- Common events: `OnClick`, `OnCommand`, `OnDataBinding`
- Custom event args classes (e.g., `CommandEventArgs`, `DataListItemEventArgs`)

### Style Building
- Use `BlazorComponentUtilities` for CSS class building
- Implement `IStyle` interface for styled components
- Use `ToStyle().Build().NullIfEmpty()` pattern

## Testing Conventions

### Test Organization
Tests are organized by component in folders matching component names:
```
/src/BlazorWebFormsComponents.Test/
  Button/
    Click.razor
    Enabled.razor
    Style.razor
  GridView/
    ...
```

### bUnit Test Pattern (v2.x)

Tests inherit from `BunitContext` and use the `Render()` method with Razor syntax:

```razor
@inherits BunitContext

@code {
    [Fact]
    public void ComponentName_Scenario_ExpectedBehavior()
    {
        var cut = Render(@<Button OnClick="HandleClick">Submit</Button>);

        cut.Find("button").Click();

        ClickCount.ShouldBe(1);
    }

    private int ClickCount = 0;
    private void HandleClick() => ClickCount++;
}
```

### Test Method Naming

Use the pattern: `ComponentName_Scenario_ExpectedBehavior`

Examples:
- `Button_Click_InvokesHandler`
- `DataList_EmptySource_ShowsEmptyTemplate`
- `RequiredFieldValidator_BlankInput_DisplaysError`

### Service Registration

```razor
@code {
    [Fact]
    public void Component_WithService_BehavesCorrectly()
    {
        Services.AddSingleton<IMyService>(new FakeService());

        var cut = Render(@<MyComponent />);
    }
}
```

### Authentication Testing

```razor
@code {
    [Fact]
    public void SecureComponent_AuthenticatedUser_ShowsContent()
    {
        var auth = this.AddTestAuthorization();
        auth.SetAuthorized("testuser");
        auth.SetRoles("Admin", "User");

        var cut = Render(@<SecureComponent />);
    }
}
```

### Accessing Component Instance

When testing component properties or state, use `FindComponent<T>()`:

```razor
@code {
    [Fact]
    public void TreeView_StaticNodes_HasCorrectNodeCount()
    {
        var cut = Render(@<TreeView>...</TreeView>);

        cut.FindComponent<TreeView>().Instance.Nodes.Count.ShouldBe(4);
    }
}
```

### Assertions
- Use Shouldly for assertions (`value.ShouldBe(expected)`)
- Use `MarkupMatches()` for HTML comparison
- Follow Arrange-Act-Assert pattern in test methods

## Design Principles

1. **Markup Compatibility**: Components should accept the same attributes as Web Forms controls
2. **HTML Output**: Generated HTML should match Web Forms output where possible
3. **Minimal API Surface**: Focus on essential features, not full Web Forms API
4. **No DataSources**: Use repository pattern instead of Web Forms DataSource controls
5. **No ViewState**: Store state in private fields or session (ViewState property exists but is syntax-only)
6. **No Postback**: Blazor's event model replaces postback

## Components Requiring Implementation

When adding new components:
1. Create `.razor` and `.razor.cs` files in `/src/BlazorWebFormsComponents/`
2. Inherit from appropriate base class
3. Add unit tests in matching folder under `/src/BlazorWebFormsComponents.Test/`
4. Add sample page in `/samples/AfterBlazorServerSide/Pages/ControlSamples/`
5. Document in `/docs/` folder

## Building and Running

```bash
# Restore packages
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Run server-side samples
dotnet run --project samples/AfterBlazorServerSide
```

## Key Interfaces

- `IStyle` - Styling properties (BackColor, ForeColor, CssClass, etc.)
- `IColumn<T>` - Grid column definition
- `IRow` - Grid row definition
- `ITextComponent` - Components with Text property
- `IButtonComponent` - Button-like components
- `IImageComponent` - Image-like components

## Templates and RenderFragments

Data controls use templated components:
```csharp
[Parameter]
public RenderFragment<ItemType> ItemTemplate { get; set; }

[Parameter]
public RenderFragment HeaderTemplate { get; set; }

[Parameter]
public RenderFragment FooterTemplate { get; set; }
```

## Generic Type Parameters

Data-bound components use `@typeparam`:
```razor
@typeparam ItemType
@inherits DataBoundComponent<ItemType>
```

## Validation Components

Located in `/src/BlazorWebFormsComponents/Validations/`:
- Inherit from `BaseValidator`
- Support `ControlToValidate`, `ErrorMessage`, `ValidationGroup`
- Include `RequiredFieldValidator`, `RegularExpressionValidator`, `CustomValidator`, etc.

## Documentation System

### Overview
Documentation is built using **MkDocs** with the **Material** theme and deployed to GitHub Pages via GitHub Actions. The docs workflow (`.github/workflows/docs.yml`) triggers on changes to `docs/` or `mkdocs.yml`.

### Building Documentation Locally
```bash
# Build the mkdocs Docker image
docker build -t mkdocs -f ./docs/Dockerfile ./

# Build docs (from repository root)
docker run --rm -v "$(pwd):/docs" mkdocs build --strict

# Serve docs locally for preview
docker run --rm -p 8000:8000 -v "$(pwd):/docs" mkdocs serve
```

### Documentation Structure
```
/docs
  README.md                      -- Home page
  /EditorControls/               -- Simple UI components (Button, Label, etc.)
  /DataControls/                 -- Data-bound components (GridView, Repeater, etc.)
  /ValidationControls/           -- Form validation components
  /NavigationControls/           -- Navigation components (TreeView, Menu)
  /UtilityFeatures/              -- Helper features (DataBinder, ViewState)
  /Migration/                    -- Migration guides and strategies
  /assets/                       -- Images, CSS, logos
```

### Adding New Documentation
1. Create markdown file in appropriate category folder
2. Add entry to `mkdocs.yml` under `nav:` section
3. Follow the component documentation template below

### Component Documentation Template
All component documentation should follow this structure:

```markdown
# ComponentName

Brief description of what the component does and its purpose.

Original Microsoft documentation: [link to docs.microsoft.com]

## Features Supported in Blazor

- Feature 1
- Feature 2
- Event handlers supported

## Web Forms Features NOT Supported

- Feature not supported (with explanation why)
- PostBackUrl (not needed in Blazor)

## Web Forms Declarative Syntax

```html
<asp:ComponentName
    Attribute1="value"
    Attribute2="value"
    OnEvent="EventHandler"
    runat="server"
/>
```

## Blazor Syntax

```razor
<ComponentName
    Attribute1="value"
    Attribute2="value"
    OnEvent="EventHandler" />
```

## Usage Notes

Any special considerations, gotchas, or migration tips.

## Examples

### Basic Usage
[Code example]

### Advanced Scenario
[Code example]
```

### Migration Documentation Guidelines
- Start with acknowledgment that migration isn't 100% automated
- Provide step-by-step instructions
- Include before/after code comparisons
- Link to related component documentation
- Reference the live samples site for working examples

### MkDocs Configuration
Key extensions enabled in `mkdocs.yml`:
- `admonition` - Note/warning/tip boxes
- `codehilite` - Syntax highlighting
- `pymdownx.tabbed` - Tabbed code blocks
- `pymdownx.superfences` - Enhanced code fences
- `toc` - Table of contents with permalinks

### Documentation Admonitions
Use these for callouts:
```markdown
!!! note
    Informational note

!!! warning
    Important warning

!!! tip
    Helpful tip

!!! danger
    Critical information
```

## Contributing

- All PRs require unit tests
- Follow existing code patterns
- Update documentation for new features
- Reference issues in commit messages
