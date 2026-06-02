# CLI Migration Plan

## Executive Summary

`migration-toolkit\scripts\bwfc-migrate.ps1` is already effectively deprecated in code: its live path now resolves the CLI project, forwards to `webforms-to-blazor migrate` or `webforms-to-blazor prescan`, and returns before the legacy PowerShell implementation runs. The real remaining gap is not the main orchestrator entrypoint; it is that the CLI still depends on PowerShell for two helper workflows (`Migrate-NugetStaticAssets.ps1` and `Convert-EdmxToEfCore.ps1`) and has no CLI replacement for `bwfc-scan.ps1`.

## Section 1: What `bwfc-migrate.ps1` does

### 1.1 Actual live behavior today

1. Resolves `src\BlazorWebFormsComponents.Cli\BlazorWebFormsComponents.Cli.csproj` relative to the repository.
2. Builds `dotnet run --project <cli-csproj> -- ...` arguments.
3. If `-Prescan` is supplied, forwards to `webforms-to-blazor prescan --input <Path>`.
4. Otherwise forwards to `webforms-to-blazor migrate --input <Path> --output <Output> --overwrite`.
5. Maps `-SkipProjectScaffold` to `--skip-scaffold`, `-WhatIf` to `--dry-run`, and `-Verbose` to `--verbose`.
6. Exits with the CLI exit code.
7. Returns immediately; the large PowerShell implementation that follows is dead code for current callers.

### 1.2 Legacy embedded migration pipeline still present below `return`

The unreachable legacy body shows the original end-to-end orchestration the CLI has been porting:

1. Resolve and validate source/output paths; derive and sanitize a project name.
2. Print migration banner and mode summary.
3. Create the output directory.
4. Generate project scaffold unless `-SkipProjectScaffold` is set:
   - `.csproj`
   - `Program.cs`
   - `_Imports.razor`
   - `Components\App.razor`
   - `Components\Routes.razor`
5. Extract `Web.config` `appSettings` and `connectionStrings` into `appsettings.json`.
6. Discover `.aspx`, `.ascx`, and `.master` files.
7. For each Web Forms markup file:
   1. Determine the `.razor` output path.
   2. Detect redirect-handler pages and record manual follow-up.
   3. Add TODO headers for checkout/auth-heavy pages.
   4. Apply directive transforms (`Page`, `Master`, `Control`, `Import`, `Register`).
   5. Apply structural markup transforms (`Content`, `form`, master-page conversion).
   6. Convert expressions, login views, route helpers, and select methods.
   7. Convert AJAX Toolkit and `asp:` prefixes.
   8. Remove Web Forms-only attributes.
   9. Rewrite `~/` URL references.
   10. Fix template placeholders.
   11. Normalize booleans, enums, and unit values.
   12. Flag/remove `DataSourceID` and data-source controls.
   13. Write the `.razor` file.
   14. Copy/transform code-behind into `.razor.cs` (or VB equivalent).
8. Copy static files into `wwwroot\`.
9. Auto-detect CSS and inject `<link>` tags into `Components\App.razor`.
10. Run `Migrate-NugetStaticAssets.ps1` to extract package static assets into `wwwroot\lib\` and emit `asset-manifest.json` plus `AssetReferences.html`.
11. Auto-detect JavaScript and inject `<script>` tags into `Components\App.razor`.
12. Detect `.edmx` files in `Models\` and run `Convert-EdmxToEfCore.ps1`.
13. Copy model `.cs` files, skipping EDMX-generated artifacts and applying EF-related cleanup.
14. Copy business-logic directories (`BLL`, `BusinessLogic`, `Logic`, `Services`) with light cleanup and TODO headers.
15. Copy `App_Start` files into migration artifacts with TODO guidance.
16. Annotate `Program.cs` with redirect-handler TODOs.
17. Conditionally add `@using BlazorAjaxToolkitComponents` if AJAX Toolkit controls were seen.
18. Print a summary, manual-attention list, verbose transform log, and next steps.

## Section 2: What the CLI already handles

### 2.1 `bwfc-migrate.ps1` step mapping

| Legacy PS step | CLI equivalent | Status | Notes |
|---|---|---|---|
| Resolve inputs / invoke orchestrator | `Program.cs` `migrate` / `prescan` commands | Handled | This is now the live execution path. |
| Project scaffold generation | `ProjectScaffolder`, `GlobalUsingsGenerator`, `ShimGenerator` | Handled | CLI scaffolds more than the old PS script: `MainLayout.razor`, `launchSettings.json`, `GlobalUsings.cs`, shim files. |
| Web.config extraction | `WebConfigTransformer` | Handled | Ported to C#. |
| Discover markup files | `SourceScanner` | Handled | Supports directory and single-file input. |
| Markup transform pipeline | `MigrationPipeline` + registered `IMarkupTransform` implementations | Handled | The CLI now owns the ordered transform pipeline. |
| Code-behind transform pipeline | `MigrationPipeline` + registered `ICodeBehindTransform` implementations | Handled | Includes additional compile-surface safeguards not present in the old PS version. |
| Semantic/manual follow-up shaping | `SemanticPatternCatalog` + `MigrationReport.ManualItems` | Handled | CLI has isolated semantic patterns beyond the original regex-only script. |
| Static file copy to `wwwroot` | `StaticFileCopier` | Handled | Broader extension list than the old PS script. |
| CSS/JS injection into App.razor | `AppAssetInjector` | Handled | Driven from copied assets plus `AssetReferences.html`. |
| NuGet static asset extraction | `NuGetStaticAssetExtractor` | Partial | Exposed only inside `migrate`; implementation still shells out to `Migrate-NugetStaticAssets.ps1`. |
| EDMX conversion | `EdmxConverterBridge` | Partial | Exposed only inside `migrate`; implementation still shells out to `Convert-EdmxToEfCore.ps1`. |
| Model / business-logic source copy | `SourceFileCopier` | Handled, but different | CLI generalizes this into compile-surface-aware `.cs` copying and quarantine artifacts rather than separate Models/BLL passes. |
| `App_Start` copy | `AppStartCopier` | Handled | CLI quarantines into `migration-artifacts\App_Start\`. |
| Redirect handler annotations | `RedirectHandlerAnnotator` | Handled | Ported to C#. |
| AJAX Toolkit import behavior | `AjaxToolkitPrefixTransform` + generated output | Partial | Detection/transform exists, but there is no explicit post-pass equivalent that mutates `_Imports.razor` only when needed. |
| JSON migration report | `MigrationReport` + `--report` | Handled | Output schema differs from the old ad-hoc summary. |
| `-Prescan` mode | `PrescanAnalyzer` + `prescan` command | Partial | Covers code-pattern prescan only; legacy BWFC021 is missing, and it is not the same feature as `bwfc-scan.ps1`. |

### 2.2 Other PowerShell scripts vs CLI

| Script | What it does | CLI status | Should it become a subcommand? |
|---|---|---|---|
| `Migrate-NugetStaticAssets.ps1` | Extracts package CSS/JS/fonts/images, writes `asset-manifest.json`, writes `AssetReferences.html`, supports `-PackagesPath` and `-ManifestOnly` | Partial via internal bridge only | **Yes.** Recommend `webforms-to-blazor assets extract` (plus `--manifest-only`). |
| `Convert-EdmxToEfCore.ps1` | Parses EDMX and emits entities + `DbContext` | Partial via internal bridge only | **Yes.** Recommend `webforms-to-blazor edmx convert`. |
| `bwfc-scan.ps1` | Produces migration-readiness inventory/report over markup files, control coverage, ViewState/Session/DataSource hotspots | Missing | **Yes.** Recommend `webforms-to-blazor scan` with `console/json/markdown` output modes. |

## Section 3: Gap list

### P0 — blocks PowerShell deprecation

1. **CLI still has runtime PowerShell dependencies for assets and EDMX conversion.**
   - `NuGetStaticAssetExtractor` calls `Migrate-NugetStaticAssets.ps1`.
   - `EdmxConverterBridge` calls `Convert-EdmxToEfCore.ps1`.
   - Result: consumers may invoke the CLI, but they are not actually off PowerShell yet.

2. **Deprecation messaging is not in place.**
   - `bwfc-migrate.ps1` forwards silently.
   - The helper scripts and `bwfc-scan.ps1` do not point users to a CLI-only replacement path.

3. **CLI user guidance is stale for direct adoption.**
   - `src\BlazorWebFormsComponents.Cli\README.md` and `EXAMPLES.md` document an older command surface.
   - Deprecating scripts before fixing docs will strand users.

### P1 — needed for functional parity

4. **No standalone CLI equivalent for `bwfc-scan.ps1`.**
   - `prescan` is not a migration-readiness report.
   - Missing inventory/readiness scoring, supported/unsupported control counts, and Markdown/console report formats.

5. **No public CLI subcommand for NuGet asset extraction.**
   - Current behavior is only an internal side effect of `migrate`.
   - Missing `-PackagesPath`, `-ManifestOnly`, and direct audit scenarios.

6. **No public CLI subcommand for EDMX conversion.**
   - Current behavior is only an internal side effect of `migrate`.
   - Users cannot run/verify EDMX conversion independently.

7. **`prescan` is not fully parity-compatible with the old PowerShell prescan.**
   - Legacy `BWFC021` master-page/content-placeholder rule is absent from `PrescanAnalyzer`.

8. **`migrate` parity still differs in a few edge behaviors.**
   - The old PowerShell script had an explicit `_Imports.razor` mutation for AJAX Toolkit namespaces.
   - The CLI uses broader source-file quarantine semantics rather than the old Models/BLL split; this is probably better, but it should be documented as an intentional behavior change.

### P2 — nice to have / cleanup

9. **Wrapper scripts should eventually move from warning wrappers to removal or hard-fail wrappers.**
10. **Command/report schemas should be stabilized and documented for automation consumers.**
11. **Consider merging `prescan` and `scan` under a clearer analysis command family (`analyze prescan`, `analyze readiness`).**

## Section 4: Migration plan

| Order | Work item | Why | Complexity |
|---|---|---|---|
| 1 | Define the CLI-only command surface (`migrate`, `convert`, `prescan`, `scan`, `assets extract`, `edmx convert`) | Needed before docs, wrappers, and tests can converge | S |
| 2 | Port `Migrate-NugetStaticAssets.ps1` into native C# service code and keep manifest/snippet outputs | Removes hidden PowerShell dependency and preserves current migration behavior | M |
| 3 | Expose the native asset extractor as a first-class CLI subcommand with `--packages-path` and `--manifest-only` | Replaces direct script usage and supports audit scenarios | M |
| 4 | Port `Convert-EdmxToEfCore.ps1` into native C# | Removes the second hidden PowerShell dependency | L |
| 5 | Expose EDMX conversion as `webforms-to-blazor edmx convert` | Gives users a direct replacement for the script and a way to validate generated EF Core output independently | M |
| 6 | Implement `webforms-to-blazor scan` for readiness reporting (`console/json/markdown`) | Replaces `bwfc-scan.ps1` rather than forcing users onto the unrelated `prescan` command | M |
| 7 | Close `prescan` parity gaps (at minimum restore `BWFC021`, then document intentional deltas) | Prevents silent behavior regression for current `bwfc-migrate.ps1 -Prescan` users | S |
| 8 | Refresh CLI README/examples and migration-toolkit docs to show CLI-first usage only | Makes deprecation survivable for real users | M |
| 9 | Add warning banners to all PowerShell scripts with exact CLI replacements | Starts the deprecation clock without breaking automation immediately | S |
| 10 | Update internal automation/tests/docs to call the CLI directly instead of scripts | Proves the repo itself has stopped depending on PowerShell entrypoints | M |
| 11 | After one or two releases, convert scripts into thin compatibility shims or remove them entirely | Final deprecation step | S |

### Recommended command replacements

| PowerShell script | CLI replacement |
|---|---|
| `bwfc-migrate.ps1 -Path <src> -Output <out>` | `webforms-to-blazor migrate --input <src> --output <out>` |
| `bwfc-migrate.ps1 -Path <src> -Output <out> -Prescan` | `webforms-to-blazor prescan --input <src>` |
| `bwfc-scan.ps1 -Path <src> -OutputFormat Json -OutputFile report.json` | `webforms-to-blazor scan --input <src> --format json --output report.json` |
| `Migrate-NugetStaticAssets.ps1 -SourcePath <src> -OutputPath <out>` | `webforms-to-blazor assets extract --input <src> --output <out>` |
| `Convert-EdmxToEfCore.ps1 -EdmxPath <file> -OutputPath <dir> -Namespace <ns>` | `webforms-to-blazor edmx convert --input <file> --output <dir> --namespace <ns>` |

## Section 5: Deprecation strategy

### Phase 1 — announce and redirect

- Add a warning at the top of each PowerShell script.
- `bwfc-migrate.ps1` warning should explicitly say the script is deprecated and that `webforms-to-blazor migrate` is the supported entrypoint.
- `bwfc-scan.ps1`, `Migrate-NugetStaticAssets.ps1`, and `Convert-EdmxToEfCore.ps1` should point to their new CLI subcommands once those exist.
- Update repo docs, quickstarts, benchmark instructions, and samples to use the CLI directly.

### Phase 2 — remove hidden dependencies

- Ship native C# implementations for asset extraction and EDMX conversion.
- Ensure the CLI no longer requires `powershell`/`pwsh` to complete a full migration.
- Add CLI tests covering the new command surface and parity-critical outputs.

### Phase 3 — compatibility wrapper window

- Keep the PS1 files for a short compatibility window, but make them warning wrappers only.
- Avoid adding any new functionality to the scripts.
- Any new migration capability should land in the CLI only.

### Phase 4 — retirement

- Remove the scripts from public docs and supported workflows.
- Either delete them or leave hard-fail wrappers that say the scripts are no longer supported and point to the CLI.

### Suggested timeline

1. **Release N:** add warnings, refresh docs, define CLI command surface.
2. **Release N+1:** ship native asset/EDMX support plus `scan`/`assets`/`edmx` CLI commands; switch internal automation to CLI.
3. **Release N+2:** remove PS1 scripts from supported usage and retire them.

### What “done” looks like

Deprecation is complete when all of the following are true:

- Upstream documentation tells users to use `webforms-to-blazor`, not PowerShell.
- `webforms-to-blazor migrate` performs a full migration without invoking any PowerShell script internally.
- `bwfc-scan.ps1`, `Migrate-NugetStaticAssets.ps1`, and `Convert-EdmxToEfCore.ps1` each have a supported CLI replacement.
- The repository’s own automation/tests/examples no longer call the PowerShell scripts.
- Any remaining PS1 files are clearly marked deprecated and contain no unique behavior.
