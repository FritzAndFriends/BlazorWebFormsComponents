# AGENTS.md — BlazorWebFormsComponents

## Project Overview

BlazorWebFormsComponents is a library of Blazor components that are **drop-in replacements** for ASP.NET Web Forms controls. It enables migration from Web Forms to Blazor with minimal markup changes. The library also includes a CLI migration tool (`webforms-to-blazor`), Roslyn analyzers, compile-compatibility shims, and comprehensive documentation.

- **Package:** `Fritz.BlazorWebFormsComponents` on NuGet
- **Version:** Managed by Nerdbank.GitVersioning — see `version.json` for the current version
- **License:** MIT
- **Live samples:** https://blazorwebformscomponents.azurewebsites.net
- **Docs site:** https://fritzandfriends.github.io/BlazorWebFormsComponents/

## Repository Structure

```
/
├── BlazorMeetsWebForms.sln              -- Solution file (root entry point)
├── global.json                          -- .NET SDK version pin (10.0.100)
├── Directory.Build.props                -- Shared MSBuild properties, analyzers, versioning
├── version.json                         -- Nerdbank.GitVersioning config
├── nuget.config                         -- NuGet feed configuration
├── mkdocs.yml                           -- MkDocs documentation config
├── .editorconfig                        -- Code style (tabs, var usage, brace rules)
│
├── src/
│   ├── BlazorWebFormsComponents/        -- Main component library (Razor Class Library)
│   │   ├── *.razor / *.razor.cs         -- Component markup + code-behind pairs
│   │   ├── Validations/                 -- Validation components (RequiredFieldValidator, etc.)
│   │   ├── LoginControls/               -- Login/auth components
│   │   ├── Theming/                     -- Themes and skins support
│   │   ├── Enums/                       -- Shared enumerations
│   │   ├── Interfaces/                  -- IStyle, IColumn, IRow, etc.
│   │   ├── Extensions/                  -- Extension methods
│   │   ├── EntityFramework/             -- EF integration helpers
│   │   ├── Identity/                    -- ASP.NET Identity integration
│   │   ├── wwwroot/                     -- Static assets (JS interop files)
│   │   └── build/ / buildTransitive/    -- MSBuild targets for package consumers
│   ├── BlazorWebFormsComponents.Test/   -- Unit tests (xUnit + bUnit)
│   │   └── ComponentName/              -- Tests organized by component
│   ├── BlazorWebFormsComponents.Analyzers/  -- Roslyn analyzers (BWFC001, etc.)
│   ├── BlazorWebFormsComponents.Analyzers.Test/  -- Analyzer unit tests
│   ├── BlazorWebFormsComponents.Cli/    -- Migration CLI tool source
│   ├── BlazorAjaxToolkitComponents/     -- AJAX Toolkit component library
│   ├── ContosoUniversity.AcceptanceTests/  -- Acceptance tests
│   └── WingtipToys.AcceptanceTests/     -- Acceptance tests
│
├── tests/
│   └── BlazorWebFormsComponents.Cli.Tests/  -- CLI tool tests
│
├── samples/
│   ├── BeforeWebForms/                  -- Original Web Forms sample (.NET Framework)
│   ├── AfterBlazorServerSide/           -- Blazor Server-Side sample app
│   ├── AfterBlazorServerSide.Tests/     -- Playwright integration tests for sample app
│   ├── AfterBlazorClientSide/           -- Blazor WebAssembly sample app
│   ├── AfterContosoUniversity/          -- Migrated Contoso University sample
│   ├── AfterDepartmentPortal/           -- Migrated department portal
│   ├── AfterWingtipToys/                -- Migrated WingtipToys sample
│   ├── SharedSampleObjects/             -- Shared models and data for samples
│   ├── WingtipToys/                     -- Original WingtipToys Web Forms project
│   ├── ContosoUniversity/               -- Original Contoso University project
│   └── DepartmentPortal/               -- Original department portal
│
├── docs/                                -- MkDocs documentation source
│   ├── README.md                        -- Docs home page
│   ├── EditorControls/                  -- Button, Label, TextBox, etc.
│   ├── DataControls/                    -- GridView, Repeater, DataList, etc.
│   ├── ValidationControls/             -- RequiredFieldValidator, etc.
│   ├── NavigationControls/             -- Menu, TreeView, SiteMapPath
│   ├── LoginControls/                  -- Login, CreateUserWizard, etc.
│   ├── Migration/                       -- Migration guides and strategies
│   ├── AjaxToolkit/                     -- AJAX Toolkit components
│   ├── Analyzers/                       -- Roslyn analyzer docs
│   ├── cli/                             -- CLI tool docs
│   ├── UtilityFeatures/                -- DataBinder, ViewState, etc.
│   ├── assets/                          -- Images, CSS, logos
│   └── Dockerfile                       -- MkDocs build container
│
├── migration-toolkit/                   -- Migration automation scripts and tools
├── scripts/                             -- Release and build helper scripts
├── planning-docs/                       -- Internal planning documents
├── dev-docs/                            -- Developer documentation
│
├── .github/
│   ├── copilot-instructions.md          -- Copilot coding conventions
│   ├── agents/                          -- Custom Copilot agents
│   │   ├── migration.agent.md           -- Web Forms → Blazor migration guide agent
│   │   └── squad.agent.md               -- AI team orchestrator
│   ├── skills/                          -- Custom Copilot skills
│   │   ├── aspire/                      -- Aspire integration skill
│   │   ├── bunit-test-migration/        -- bUnit v2 migration skill
│   │   ├── component-development/       -- New component development skill
│   │   ├── contoso-migration-test/      -- Contoso migration testing skill
│   │   ├── documentation/               -- Documentation generation skill
│   │   └── webforms-migration/          -- Web Forms migration skill
│   └── workflows/                       -- GitHub Actions CI/CD
│       ├── build.yml                    -- Build + unit tests (PR + push)
│       ├── integration-tests.yml        -- Playwright integration tests
│       ├── docs.yml                     -- MkDocs build + deploy to GitHub Pages
│       ├── codeql.yml                   -- CodeQL security scanning
│       ├── nuget.yml                    -- NuGet package publish
│       ├── release.yml                  -- Release automation
│       ├── demo.yml                     -- Demo deployment
│       └── deploy-server-side.yml       -- Server-side sample deployment
```

## Tech Stack

| Technology | Details |
|---|---|
| Language | C# |
| Framework | Blazor (Server-Side + WebAssembly) |
| .NET SDK | 10.0.100 (`global.json`) |
| Target frameworks | `net8.0`, `net9.0`, `net10.0` (multi-targeted) |
| Project SDK | `Microsoft.NET.Sdk.Razor` (Razor Class Library) |
| Unit testing | xUnit + bUnit 2.5.x + Shouldly + Moq |
| Integration testing | Playwright (.NET) |
| Analyzers | Roslyn (`BlazorWebFormsComponents.Analyzers`) |
| CSS utilities | BlazorComponentUtilities |
| Versioning | Nerdbank.GitVersioning |
| Docs | MkDocs with Material theme, deployed to GitHub Pages |
| Package | NuGet (`Fritz.BlazorWebFormsComponents`) |

## Build & Run

```bash
# Restore all packages
dotnet restore

# Build the entire solution
dotnet build

# Build just the main library
dotnet build src/BlazorWebFormsComponents/BlazorWebFormsComponents.csproj

# Run the server-side sample app
dotnet run --project samples/AfterBlazorServerSide
```

## Testing

### Unit Tests (xUnit + bUnit)

```bash
# Run all unit tests
dotnet test src/BlazorWebFormsComponents.Test

# Run analyzer tests
dotnet test src/BlazorWebFormsComponents.Analyzers.Test

# Run CLI tests
dotnet test tests/BlazorWebFormsComponents.Cli.Tests
```

Tests are `.razor` files organized by component name under `src/BlazorWebFormsComponents.Test/ComponentName/`. They use bUnit v2's `Render()` method with inline Razor syntax and Shouldly assertions.

### Integration Tests (Playwright)

```bash
# First time: install Playwright browsers
pwsh samples/AfterBlazorServerSide.Tests/bin/Debug/net10.0/playwright.ps1 install

# Run integration tests
dotnet test samples/AfterBlazorServerSide.Tests
```

### Test Naming Convention

`ComponentName_Scenario_ExpectedBehavior` — e.g., `Button_Click_InvokesHandler`

## Key Patterns and Conventions

### Component Architecture

Components use the **code-behind pattern**:
- `ComponentName.razor` — Markup/template
- `ComponentName.razor.cs` — Partial class with parameters and logic

### Base Class Hierarchy

1. `BaseWebFormsComponent` → root base class (inherits `ComponentBase`)
2. `BaseStyledComponent` → adds CSS/styling support (`IStyle`)
3. `DataBoundComponent<TItemType>` → data-binding support

### Parameters and Naming

- All component attributes use `[Parameter]`
- Names match original Web Forms attribute names (e.g., `CssClass`, `BackColor`)
- Obsolete Web Forms features use `[Obsolete("migration guidance")]` — no emoji in messages
- Internal calculated properties use `Calculated` prefix (e.g., `CalculatedCssClass`)

### Migration Shims

`WebFormsPageBase` provides compile-compatibility shims:
- `Page.Request`, `Page.Response`, `Page.Session`, `Page.Server`, `Page.Cache`
- `Page.ClientScript`, `Page.ViewState`
- `ConfigurationManager.AppSettings` / `.ConnectionStrings`
- All registered via `builder.Services.AddBlazorWebFormsComponents()`

### Style Building

Use `BlazorComponentUtilities` with the pattern: `ToStyle().Build().NullIfEmpty()`

## CI/CD

| Workflow | Trigger | What it does |
|---|---|---|
| `build.yml` | PR + push to main/dev/v* | Build, unit tests, analyzer tests, health snapshot |
| `integration-tests.yml` | PR + push to main/dev/v* | Playwright integration tests against sample app |
| `docs.yml` | Push to main/v* + PRs | Build MkDocs, deploy to GitHub Pages |
| `codeql.yml` | PR + push | CodeQL security scanning |
| `nuget.yml` | Tag push | Publish NuGet package |
| `release.yml` | Release events | Release automation |

All code workflows use path filters — they only run when source code changes.

## Adding a New Component

1. **Create component files** in `/src/BlazorWebFormsComponents/`:
   - `ComponentName.razor` — markup using `@inherits BaseClass`
   - `ComponentName.razor.cs` — partial class with `[Parameter]` properties

2. **Add unit tests** in `/src/BlazorWebFormsComponents.Test/ComponentName/`:
   - Create `.razor` test files following the bUnit v2 pattern
   - Inherit from `BunitContext`, use `Render(@<Component />)` and Shouldly assertions
   - Name tests: `ComponentName_Scenario_ExpectedBehavior`

3. **Add sample page** in `/samples/AfterBlazorServerSide/Pages/ControlSamples/ComponentName/`:
   - Create a `.razor` page demonstrating the component

4. **Create documentation** in `/docs/Category/ComponentName.md`:
   - Follow the component documentation template in `copilot-instructions.md`
   - Include original Web Forms syntax, Blazor syntax, features supported/unsupported

5. **Register in `mkdocs.yml`**:
   - Add entry under the appropriate `nav:` section, alphabetically ordered
   - Format: `- ComponentName: Category/ComponentName.md`

6. **Update `README.md`**:
   - Add link to docs under the appropriate component category

7. **Update `status.md`**:
   - Mark the component status in the tracking table

## Documentation

- **Framework:** MkDocs with Material theme
- **Location:** `/docs/` directory, config in `mkdocs.yml`
- **Deployed to:** https://fritzandfriends.github.io/BlazorWebFormsComponents/
- **CI:** `.github/workflows/docs.yml` builds on PRs, deploys on push to `main`

### Building Docs Locally

```bash
# Build the Docker image
docker build -t mkdocs -f ./docs/Dockerfile ./

# Build docs (validates with --strict)
docker run --rm -v "$(pwd):/docs" mkdocs build --strict

# Serve locally at http://localhost:8000
docker run --rm -p 8000:8000 -v "$(pwd):/docs" mkdocs serve --dev-addr 0.0.0.0:8000
```

## Common Pitfalls

- **Multi-target builds:** The library targets `net8.0`, `net9.0`, and `net10.0`. Package versions vary per target framework (see `Directory.Build.props`). Always test against all targets before merging.
- **bUnit v2 syntax:** Tests use bUnit v2's `Render()` method with inline Razor — do NOT use the older `RenderComponent<T>()` API.
- **Fetch depth:** CI workflows require `fetch-depth: 0` for Nerdbank.GitVersioning to work correctly.
- **Obsolete messages:** Use plain text without emoji — e.g., `"Use @ref instead of ID"`.
- **Sample app must build:** The `build.yml` workflow builds both the library and `AfterBlazorServerSide` sample. Changes to components can break the sample app.
- **Docs must pass strict build:** MkDocs runs with `--strict` in CI — broken links or missing nav entries will fail the build.
- **Version is not hardcoded:** Version is managed by `version.json` + Nerdbank.GitVersioning. Never put version numbers in source files.
