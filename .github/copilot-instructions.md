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
- **Framework**: Blazor (migration target is .NET 10 static SSR; samples also include Server-Side and WebAssembly)
- **Project Type**: Razor Class Library (`Microsoft.NET.Sdk.Razor`)
- **Testing Framework**: xUnit with bUnit for Blazor component testing
- **Assertion Library**: Shouldly
- **Mocking**: Moq
- **Build/Versioning**: Nerdbank.GitVersioning
- **CSS Utilities**: BlazorComponentUtilities

## Project Structure

```
/docs                                 -- User documentation (MkDocs)
/samples                              -- Usage samples and migration benchmarks (ordered by complexity ↓)
  BeforeWebForms/                     -- Original Web Forms sample (.NET Framework)
  AfterBlazorServerSide/              -- Blazor Server-Side component samples
  AfterBlazorServerSide.Tests/        -- Playwright tests for the sample app
  AfterBlazorClientSide/              -- Blazor WebAssembly component samples
  SharedSampleObjects/                -- Shared models/data for samples
  WingtipToys/                        -- ★ Current benchmark focus — e-commerce app (products, cart, checkout, identity)
  AfterWingtipToys/                   -- Migrated WingtipToys output (cleared + regenerated each benchmark run)
  ContosoUniversity/                  -- Next benchmark — university CRUD app (EF, master-detail, search)
  AfterContosoUniversity/             -- Migrated Contoso University output
  DepartmentPortal/                   -- Most complex benchmark — multi-page portal (advanced data binding, nested controls)
  AfterDepartmentPortal/              -- Migrated DepartmentPortal output
/src
  BlazorWebFormsComponents/           -- Main component library
  BlazorWebFormsComponents.Test/      -- Unit tests with bUnit
  BlazorWebFormsComponents.Analyzers/ -- Roslyn analyzers
  BlazorWebFormsComponents.Analyzers.Test/ -- Analyzer tests
  BlazorWebFormsComponents.Cli/       -- Web Forms to Blazor CLI pipeline and scaffolding
  BlazorAjaxToolkitComponents/        -- AJAX Toolkit component library
  WingtipToys.AcceptanceTests/        -- WingtipToys Playwright acceptance suite
  ContosoUniversity.AcceptanceTests/  -- ContosoUniversity Playwright acceptance suite
/tests
  BlazorWebFormsComponents.Cli.Tests/ -- CLI transform and pipeline tests
/migration-toolkit                    -- Wrapper scripts, skills, methodology, and benchmark helpers
  scripts/                            -- bwfc-migrate.ps1, bwfc-scan.ps1, and compatibility wrappers
  skills/                             -- bwfc-migration, bwfc-identity-migration, bwfc-data-migration, migration-standards, l3-performance-optimization
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

### bUnit Test Quick Reference

Tests use bUnit v2 with `.razor` test files that inherit from `BlazorWebFormsTestContext` (which extends `BunitContext` with pre-registered BWFC services). Use `Render(@<Component />)` with inline Razor syntax and Shouldly assertions.

```razor
@inherits BlazorWebFormsTestContext

@code {
    [Fact]
    public void ComponentName_Scenario_ExpectedBehavior()
    {
        var cut = Render(@<Button OnClick="HandleClick">Submit</Button>);
        cut.Find("input").Click();
        ClickCount.ShouldBe(1);
    }

    private int ClickCount = 0;
    private void HandleClick() => ClickCount++;
}
```

**Test naming:** `ComponentName_Scenario_ExpectedBehavior` (e.g., `Button_Click_InvokesHandler`)

**Assertions:** Use Shouldly (`ShouldBe`, `ShouldBeTrue`, `ShouldContain`) and bUnit's `MarkupMatches()` for HTML comparison.

> **For comprehensive test authoring guidance** — data-bound components, validation, event callbacks, JS interop mocking, service registration, authentication, xUnit logger integration — see the **bunit-testing** skill (`.github/skills/bunit-testing/SKILL.md`).

## Design Principles

1. **Markup Compatibility**: Components should accept the same attributes as Web Forms controls
2. **HTML Output**: Generated HTML should match Web Forms output where possible
3. **Minimal API Surface**: Focus on essential features, not full Web Forms API
4. **No DataSources**: Use repository pattern instead of Web Forms DataSource controls
5. **No ViewState**: Store state in private fields or session (ViewState property exists but is syntax-only)
6. **PostBack Compatibility**: `WebFormsPageBase` provides `IsPostBack`, `PostBack` event, and `__doPostBack()` JS interop bridge

## Migration Rules

These are hard rules for migration work in this repository:

1. **Use the toolkit entry point**: Start project migrations with `migration-toolkit/scripts/bwfc-migrate.ps1` or the `webforms-to-blazor migrate` CLI it forwards to. Do not invent ad hoc regex converters or one-off migration scripts.
2. **Always target Blazor static SSR**: Generated apps target .NET 10 static server-side rendering. Do not switch the whole app to interactive Blazor Server; only opt into interactive render modes deliberately and per page.
3. **Trust the shims**: `WebFormsPageBase` preserves `Page.Request`, `Page.Response`, `Page.Session`, `Page.Server`, `Page.Cache`, `Page.ClientScript`, `IsPostBack`, and `ViewState`. Keep migrated code-behind compiling against those shims instead of rewriting to native ASP.NET Core services during Layer 1 or Layer 2 work.
4. **Preserve BWFC data controls**: Never replace `ListView`, `FormView`, `GridView`, `DataList`, or `Repeater` with manual HTML tables or divs. Fix the generated BWFC markup, templates, item types, or child components instead.
5. **Register transforms twice**: Every new CLI transform must be registered in both `src/BlazorWebFormsComponents.Cli/Program.cs` DI and `tests/BlazorWebFormsComponents.Cli.Tests/TestHelpers.cs` so the runtime pipeline and isolated test pipeline stay aligned.
6. **Respect quarantine boundaries**: `PageQuarantineDetector` should quarantine non-essential `Account/`, `Admin/`, `Checkout/`, mobile, payment, or compile-surface blocker pages, but benchmark-critical home, about, contact, product, catalog, and cart flows stay on the runnable path whenever possible.

### Migration Benchmark Progression

The `samples/` folder contains benchmark apps ordered by increasing complexity. Each app exercises more of the CLI and BWFC component surface:

| Sample | Complexity | Key Challenges | Status |
|--------|-----------|----------------|--------|
| **WingtipToys** | ★★☆ | E-commerce: products, cart, checkout, identity, EF data binding | **Current focus** — active benchmark runs |
| **ContosoUniversity** | ★★☆ | University CRUD: master-detail, search, EF migrations | Acceptance tests exist |
| **DepartmentPortal** | ★★★ | Multi-page portal: advanced data binding, nested controls, complex layouts | Future target — most sophisticated |

Always complete the current benchmark focus before moving to the next. Improvements to the CLI and toolkit should be validated against the current focus app first, then regression-tested against simpler samples.

## Migration Shims

The library provides compile-compatibility shims on `WebFormsPageBase` so migrated code-behind compiles unchanged:

- **`Page.Request`** — `RequestShim` wrapping QueryString, Cookies, Url, Form (via `FormShim`)
- **`Page.Response`** — `ResponseShim` wrapping `Redirect()` (auto-strips `~/` and `.aspx`) and Cookies
- **`Page.Session`** — `SessionShim` with `ISession` sync and in-memory fallback
- **`Page.Server`** — `ServerShim` wrapping `MapPath()`, `HtmlEncode()`, `UrlEncode()`
- **`Page.Cache`** — `CacheShim` wrapping `IMemoryCache` with absolute/sliding expiration
- **`Page.ClientScript`** — `ClientScriptShim` for `RegisterStartupScript()`, `RegisterClientScriptBlock()`, `GetPostBackEventReference()`
- **`Page.ViewState`** — `ViewStateDictionary` per-component dictionary
- **`<WebFormsForm>`** — Component enabling `Request.Form["key"]` in interactive mode via JS interop
- **`ScriptManagerShim`** — `ScriptManager.GetCurrent(page)` pattern delegating to `ClientScriptShim`
- **`ConfigurationManager`** — `AppSettings["key"]` and `ConnectionStrings["name"]` from `IConfiguration`

All shims are auto-registered by `builder.Services.AddBlazorWebFormsComponents()` in `Program.cs`.

## Components Requiring Implementation

When adding new components:
1. Create `.razor` and `.razor.cs` files in `/src/BlazorWebFormsComponents/`
2. Inherit from appropriate base class
3. Add unit tests in matching folder under `/src/BlazorWebFormsComponents.Test/`
4. Add sample page in `/samples/AfterBlazorServerSide/Pages/ControlSamples/`
5. Document in `/docs/` folder (see Documentation Requirements below)

## Migration CLI & Toolkit

### Primary Entry Points

- `src/BlazorWebFormsComponents.Cli/Program.cs` defines the `webforms-to-blazor` CLI entry point and command surface (`migrate`, `convert`, `prescan`, `scan`, `assets extract`, `edmx convert`).
- `migration-toolkit/scripts/bwfc-migrate.ps1` is the supported migration-toolkit wrapper. It resolves the CLI project and forwards to `migrate` or `prescan` instead of maintaining a separate regex pipeline.
- The migration-toolkit skill set currently includes `bwfc-migration`, `bwfc-identity-migration`, `bwfc-data-migration`, `migration-standards`, and `l3-performance-optimization`.

### Pipeline Overview

`MigrationPipeline` orchestrates the full migration sequence:

1. `ProjectScaffolder` generates the .NET 10 Blazor static SSR scaffold unless `--skip-scaffold` is used.
2. `WebConfigTransformer` converts `Web.config` settings into `appsettings.json`.
3. Ordered markup transforms run first.
4. Ordered code-behind transforms run second.
5. `SemanticPatternCatalog` applies higher-level page rewrites.
6. `PageQuarantineDetector` performs a late compile-surface pass and writes `migration-artifacts/quarantine-manifest.json` when pages must be stubbed.
7. Static assets, source files, App_Start artifacts, NuGet assets, EDMX output, and redirect annotations are copied or generated.

The CLI currently has **24 core markup transforms plus directive/infrastructure markup passes** and **27 code-behind transforms** wired through `Program.cs`.

### Transform Registration Pattern

When adding a transform:

1. Create the transform class in `src/BlazorWebFormsComponents.Cli/Transforms/`.
2. Register it in `BuildServiceProvider()` in `src/BlazorWebFormsComponents.Cli/Program.cs`.
3. Add it to `CreateDefaultPipeline()` in `tests/BlazorWebFormsComponents.Cli.Tests/TestHelpers.cs`.
4. Add or update focused transform tests and pipeline coverage.

```csharp
// Program.cs
services.AddSingleton<IMarkupTransform, MyNewTransform>();

// TestHelpers.cs
var markupTransforms = new List<IMarkupTransform>
{
    new MyNewTransform(),
};
```

### Scaffolding System

- `RuntimeDetector` aggregates runtime signal detectors for Entity Framework, session, identity, and `Global.asax` usage.
- `ProjectScaffolder` generates the project file, `_Imports.razor`, `Components/App.razor`, `Components/Routes.razor`, `Components/Layout/MainLayout.razor`, and launch settings.
- `ProgramCsEmitter` generates `Program.cs` for .NET 10 static SSR, including `AddRazorComponents()`, `AddBlazorWebFormsComponents()`, `UseStaticFiles()`, and `UseAntiforgery()`.
- `DatabaseProviderDetector` and `WebConfigTransformer` carry forward database provider and connection-string wiring.

### Quarantine System

- `PageQuarantineDetector` creates build-safe placeholder pages for compile-surface blockers and emits a manifest under `migration-artifacts/quarantine-manifest.json`.
- Essential benchmark pages (home, about, contact, product, catalog, shopping cart, add-to-cart flows) are exempt from incidental quarantine signals.
- Quarantined pages keep stub `.razor` and `.razor.cs` files plus the transformed original code-behind as an artifact for later Layer 2 or Layer 3 repair.

### Acceptance Test Suites

Use these suites as the migration quality gate for benchmark apps:

```bash
# WingtipToys migrated app
# Set WINGTIPTOYS_BASE_URL first if the app is not on https://localhost:5001

dotnet build src/WingtipToys.AcceptanceTests
pwsh src/WingtipToys.AcceptanceTests/bin/Debug/net10.0/playwright.ps1 install chromium
dotnet test src/WingtipToys.AcceptanceTests

# ContosoUniversity benchmark/original app
# Set CONTOSO_BASE_URL first if the app is not on http://localhost:44380

dotnet build src/ContosoUniversity.AcceptanceTests
pwsh src/ContosoUniversity.AcceptanceTests/bin/Debug/net10.0/playwright.ps1 install chromium
dotnet test src/ContosoUniversity.AcceptanceTests
```

## Documentation Requirements

**IMPORTANT**: All features, fixes, and new components MUST include documentation updates.

### When to Update Documentation

- **New Component**: Create documentation file and add to `mkdocs.yml`
- **New Feature**: Update the component's documentation to describe the feature
- **Bug Fix**: Update documentation if the fix changes behavior or usage
- **Breaking Change**: Document migration steps in the component's documentation

### Documentation Checklist

For every completed feature, fix, or new component:

1. **Create or update the markdown documentation file** in the appropriate `/docs/` subfolder:
   - Editor Controls → `/docs/EditorControls/`
   - Data Controls → `/docs/DataControls/`
   - Validation Controls → `/docs/ValidationControls/`
   - Navigation Controls → `/docs/NavigationControls/`
   - Login Controls → `/docs/LoginControls/`

2. **Add entry to `mkdocs.yml`** if the documentation file is new:
   - Add under the appropriate section in the `nav:` block
   - Follow alphabetical ordering within sections
   - Format: `- ComponentName: Category/ComponentName.md`

3. **Update README.md** if adding a new component:
   - Add link to documentation in the component list
   - Format: `[ComponentName](docs/Category/ComponentName.md)`

### Example: Adding a New Component

```bash
# 1. Create documentation file
docs/EditorControls/NewComponent.md

# 2. Add to mkdocs.yml nav section
nav:
  - Editor Controls:
    - NewComponent: EditorControls/NewComponent.md

# 3. Add link in README.md under Editor Controls
- [NewComponent](docs/EditorControls/NewComponent.md)
```

## Building and Running

```bash
# Restore packages
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Run CLI tests
dotnet test tests/BlazorWebFormsComponents.Cli.Tests

# Prescan a Web Forms app
dotnet run --project src/BlazorWebFormsComponents.Cli -- prescan -i samples/WingtipToys

# Run the migration CLI
dotnet run --project src/BlazorWebFormsComponents.Cli -- migrate -i samples/WingtipToys -o samples/AfterWingtipToys

# Run server-side samples
dotnet run --project samples/AfterBlazorServerSide
```

### Acceptance Test Commands

```bash
# WingtipToys migrated app (set WINGTIPTOYS_BASE_URL first if needed)
dotnet build src/WingtipToys.AcceptanceTests
pwsh src/WingtipToys.AcceptanceTests/bin/Debug/net10.0/playwright.ps1 install chromium
dotnet test src/WingtipToys.AcceptanceTests

# ContosoUniversity benchmark/original app (set CONTOSO_BASE_URL first if needed)
dotnet build src/ContosoUniversity.AcceptanceTests
pwsh src/ContosoUniversity.AcceptanceTests/bin/Debug/net10.0/playwright.ps1 install chromium
dotnet test src/ContosoUniversity.AcceptanceTests
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

## Maintenance Matrix

When making changes, ensure all related files are updated:

| Change Made | Files to Update |
|---|---|
| **New component added** | `src/BlazorWebFormsComponents/ComponentName.razor` + `.razor.cs`, `src/BlazorWebFormsComponents.Test/ComponentName/*.razor` (tests), `samples/AfterBlazorServerSide/Pages/ControlSamples/ComponentName/*.razor` (sample), `docs/Category/ComponentName.md` (docs), `mkdocs.yml` (`nav:` entry), `README.md` (component link), `status.md` (status table) |
| **Existing component modified** | Unit tests in `src/BlazorWebFormsComponents.Test/ComponentName/`, sample page if behavior changed, `docs/Category/ComponentName.md` if API changed |
| **New migration shim added** | Shim class in `src/BlazorWebFormsComponents/`, register in `ServiceCollectionExtensions.cs` via `AddBlazorWebFormsComponents()`, update `docs/Migration/` guide, update `copilot-instructions.md` Migration Shims section |
| **Base class changed** (`BaseWebFormsComponent`, `BaseStyledComponent`, `DataBoundComponent`) | Check all components inheriting from it — changes cascade to every component. Run full test suite. |
| **CLI transform added** | Transform in `src/BlazorWebFormsComponents.Cli/Transforms/`, register it in `src/BlazorWebFormsComponents.Cli/Program.cs` and `tests/BlazorWebFormsComponents.Cli.Tests/TestHelpers.cs`, add or update transform tests, update `docs/cli/transforms.md`, update `docs/cli/index.md` transform count |
| **CLI scaffolding/runtime changed** | Files in `src/BlazorWebFormsComponents.Cli/Scaffolding/` or `Pipeline/`, matching tests in `tests/BlazorWebFormsComponents.Cli.Tests/`, `migration-toolkit/README.md` or methodology docs if operator workflow changes, and `.github/copilot-instructions.md` if migration guidance changes |
| **Migration toolkit wrapper or skill changed** | Matching files in `migration-toolkit/scripts/` or `migration-toolkit/skills/`, relevant `docs/Migration/` or `docs/cli/` pages, and `.github/copilot-instructions.md` when the recommended migration workflow changes |
| **Analyzer rule added** | Rule in `src/BlazorWebFormsComponents.Analyzers/`, test in `src/BlazorWebFormsComponents.Analyzers.Test/`, update `docs/Analyzers/` |
| **Target framework changed** | `Directory.Build.props` (version properties), all `.csproj` files (`TargetFrameworks`), `global.json`, `.github/workflows/build.yml` (dotnet-version), `.github/copilot-setup-steps.yml` |
| **Documentation added/changed** | `docs/` markdown file, `mkdocs.yml` (`nav:` entry if new), verify local build passes with `docker run --rm -v "$(pwd):/docs" mkdocs build --strict` |
| **NuGet dependency changed** | `.csproj` file, `Directory.Build.props` if version is centralized there, test that all three target frameworks build |
| **Project structure changed** | `AGENTS.md` (repository structure), `README.md`, `CONTRIBUTING.md`, `.github/copilot-instructions.md` (project structure section) |

## Contributing

- All PRs require unit tests
- Follow existing code patterns
- Update documentation for new features
- Reference issues in commit messages
