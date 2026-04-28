# WebForms to Blazor CLI Tool

The `webforms-to-blazor` CLI is a powerful command-line tool that automates the first phase of your Web Forms to Blazor migration. It performs deterministic, pattern-based transformations on your Web Forms markup and code-behind to produce Blazor-ready code and a **.NET 10 Blazor Web App scaffold configured for static server-side rendering (SSR)**.

## What It Does

This tool **reduces manual migration effort** by:

- Removing boilerplate Web Forms directives and syntax
- Converting ASP.NET server controls to BWFC components
- Replacing Web Forms expressions with Blazor syntax
- Applying semantic page-pattern rewrites after the core transform pass
- Extracting code patterns and flagging them with TODO comments for Copilot L2 automation
- Quarantining risky legacy bootstrap/source artifacts out of the generated SSR compile surface
- Scaffolding a new .NET 10 Blazor SSR project structure with shims and services

The tool processes `.aspx`, `.ascx`, and `.master` files in a fixed sequence, then applies a bounded semantic pattern catalog so each higher-level rewrite builds on a normalized page shape.

## Installation

### As a Global Tool

```bash
dotnet tool install --global Fritz.WebFormsToBlazor
```

### From Source

```bash
cd src/BlazorWebFormsComponents.Cli
dotnet pack
dotnet tool install --global --add-source ./bin/Release Fritz.WebFormsToBlazor
```

### Verify Installation

```bash
webforms-to-blazor --help
```

## Quick Start

### Convert a Single File

```bash
webforms-to-blazor convert --input ProductCard.ascx --output ./BlazorComponents
```

### Convert a Whole Project

```bash
webforms-to-blazor migrate --input ./MyWebFormsProject --output ./MyBlazorProject
```

The tool will:
1. Scan all `.aspx`, `.ascx`, and `.master` files
2. Apply the ordered markup and code-behind transform pipeline
3. Apply semantic page-pattern rewrites for known recurring Web Forms shapes
4. Generate a migration report
5. Scaffold supporting files for a .NET 10 Blazor SSR app (Program.cs, App.razor, shims, handlers)

## Two Commands

### `migrate` — Full Project Migration

Transforms an entire Web Forms project to **.NET 10 Blazor SSR** with scaffolding.

```bash
webforms-to-blazor migrate \
  --input ./MyWebFormsProject \
  --output ./MyBlazorProject
```

**Key Options:**

- `--input <path>` — Web Forms project root (required)
- `--output <path>` — .NET 10 Blazor SSR output directory (required)
- `--skip-scaffold` — Skip generating the .NET 10 Blazor SSR scaffold
- `--dry-run` — Preview changes without writing files
- `--verbose` / `-v` — Show detailed per-file transform logging
- `--overwrite` — Overwrite existing files in the output directory
- `--report <path>` — Write the JSON migration report to a specific file

**Output:**
- Converted `.razor` files
- Quarantined manual code-behind and risky legacy source artifacts under `migration-artifacts\`
- Generated `Program.cs` with shim registration for static SSR on .NET 10
- Migration report (`migration-report.json`)

### `convert` — File-Level Transformation

Converts individual files without scaffolding. Useful for incremental migrations.

```bash
webforms-to-blazor convert \
  --input ./Controls/MyControl.ascx \
  --output ./Components/MyControl.razor
```

**Key Options:**

- `--input <path>` — Single `.ascx` or `.aspx` file (required)
- `--output <path>` — Output file path
- `--overwrite` — Overwrite an existing generated file

## Transform Categories

The tool applies an ordered transform pipeline and then a semantic pattern catalog:

1. **Directives** (5) — Page, Master, Control, Register, Import directives
2. **Markup** (19) — Controls, expressions, templates, data binding
3. **Code-Behind** (9) — Using statements, base classes, lifecycle, event handlers

See **[Transform Reference](transforms.md)** for the flat transform list and **[Semantic Pattern Catalog](semantic-pattern-catalog.md)** for the bounded semantic pass that runs afterward.

## TODO Comments and L2 Automation

The tool inserts TODO comments with standardized category slugs so Copilot L2 skills can automatically follow up on migration work:

```csharp
// TODO(bwfc-lifecycle): Page_Load → OnInitializedAsync
// TODO(bwfc-ispostback): Review IsPostBack guard for Blazor patterns
// TODO(bwfc-session-state): SessionShim auto-wired via [Inject]
```

See **[TODO Categories](todo-conventions.md)** for the complete list of 13 categories and how L2 automation uses them.

## Migration Report

After migration, the tool generates a `migration-report.json` with:

- File-by-file transformation summary
- Manual work items flagged by category
- Severity levels (Info, Warning, Error)
- Precise file locations and line numbers

See **[Report Format](report.md)** for schema and examples.

## Limitations & Next Steps

**This tool handles Level 1 transformations only:**

- ✅ Markup and directive conversion
- ✅ Pattern detection and guidance
- ✅ Boilerplate removal
- ❌ Logic rewriting (use Copilot L2 skills for this)

After running the CLI:

1. **Review TODO comments** — each one points to a specific migration pattern
2. **Run Copilot L2 skills** — automated follow-up transforms for complex patterns
3. **Build and test** — verify your Blazor project compiles and runs
4. **Manual tweaks** — business logic, styling, third-party integrations

## Example: Full Migration Workflow

```bash
# 1. Scan and transform
webforms-to-blazor migrate \
  --input ./MyApp.Web \
  --output ./MyApp.Blazor

# 2. Review migration report
cat MyApp.Blazor/migration-report.json | jq '.manualItems[] | select(.severity == "Error")'

# 3. Build and identify missing pieces
cd MyApp.Blazor
dotnet build

# 4. Use Copilot CLI for L2 automation
copilot /webforms-migration
```

## Next Steps

- **[Transform Reference](transforms.md)** — See what each transform does with before/after examples
- **[Semantic Pattern Catalog](semantic-pattern-catalog.md)** — Understand when page-shape rewrites belong in the isolated semantic pass
- **[TODO Conventions](todo-conventions.md)** — Understand the TODO categories for L2 automation
- **[Report Schema](report.md)** — Interpret the migration report
- **[Migration Strategies](../Migration/Strategies.md)** — Learn the full migration approach
