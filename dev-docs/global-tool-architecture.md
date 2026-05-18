# Global Tool Architecture: `webforms-to-blazor`

> **Author:** Forge (Lead / Web Forms Reviewer)  
> **Date:** 2026-07-26  
> **Status:** PROPOSAL — awaiting Jeff's approval  
> **Context:** Replaces `bwfc-migrate.ps1` (3,600+ lines, 41 functions) with a compiled C# dotnet global tool.  
> **PR #328 reference:** `copilot/add-ascx-to-razor-tool` branch — thin prototype (~15% coverage)

---

## 1. Project Structure

### Location

Keep `src/BlazorWebFormsComponents.Cli/` from PR #328. The tool ships alongside the library.

```
src/
├── BlazorWebFormsComponents/                 # The BWFC library (existing)
├── BlazorWebFormsComponents.Cli/             # The global tool (this proposal)
│   ├── BlazorWebFormsComponents.Cli.csproj
│   ├── Program.cs                            # System.CommandLine entry point
│   ├── Pipeline/
│   │   ├── MigrationPipeline.cs              # Orchestrates the full migration
│   │   ├── MigrationContext.cs               # Per-file + project-wide shared state
│   │   └── TransformResult.cs                # Immutable result of each transform step
│   ├── Transforms/
│   │   ├── IMarkupTransform.cs               # Interface for markup transforms
│   │   ├── ICodeBehindTransform.cs           # Interface for code-behind transforms
│   │   ├── Directives/
│   │   │   ├── PageDirectiveTransform.cs
│   │   │   ├── MasterDirectiveTransform.cs
│   │   │   ├── ControlDirectiveTransform.cs
│   │   │   ├── RegisterDirectiveTransform.cs
│   │   │   └── ImportDirectiveTransform.cs
│   │   ├── Markup/
│   │   │   ├── ContentWrapperTransform.cs
│   │   │   ├── FormWrapperTransform.cs
│   │   │   ├── MasterPageTransform.cs
│   │   │   ├── ExpressionTransform.cs        # <%: %>, <%# %>, Eval(), Bind(), Item.
│   │   │   ├── AspPrefixTransform.cs
│   │   │   ├── AjaxToolkitPrefixTransform.cs
│   │   │   ├── AttributeStripTransform.cs    # runat, AutoEventWireup, etc.
│   │   │   ├── AttributeNormalizeTransform.cs # booleans, enums, units
│   │   │   ├── UrlReferenceTransform.cs      # ~/ → /
│   │   │   ├── LoginViewTransform.cs
│   │   │   ├── SelectMethodTransform.cs
│   │   │   ├── DataSourceIdTransform.cs
│   │   │   ├── EventWiringTransform.cs       # OnClick="X" → OnClick="@X"
│   │   │   ├── TemplatePlaceholderTransform.cs
│   │   │   └── GetRouteUrlTransform.cs
│   │   └── CodeBehind/
│   │       ├── UsingStripTransform.cs        # System.Web.*, Microsoft.AspNet.*
│   │       ├── BaseClassStripTransform.cs
│   │       ├── ResponseRedirectTransform.cs
│   │       ├── SessionDetectTransform.cs
│   │       ├── ViewStateDetectTransform.cs
│   │       ├── IsPostBackTransform.cs
│   │       ├── PageLifecycleTransform.cs
│   │       ├── EventHandlerSignatureTransform.cs
│   │       ├── DataBindTransform.cs          # Cross-file: code-behind + markup correlation
│   │       └── UrlCleanupTransform.cs        # .aspx string literals → clean routes
│   ├── Scaffolding/
│   │   ├── ProjectScaffolder.cs              # .csproj, Program.cs, _Imports.razor, App.razor, Routes.razor
│   │   ├── GlobalUsingsGenerator.cs
│   │   ├── ShimGenerator.cs                  # WebFormsShims.cs, IdentityShims.cs
│   │   └── Templates/                        # Embedded resource templates (csproj, Program.cs, etc.)
│   ├── Config/
│   │   ├── WebConfigTransformer.cs           # web.config → appsettings.json
│   │   └── DatabaseProviderDetector.cs
│   ├── Analysis/
│   │   ├── Prescanner.cs                     # BWFC001–BWFC014 pattern analysis
│   │   └── MigrationReport.cs               # JSON + human-readable report
│   ├── Io/
│   │   ├── SourceScanner.cs                  # Discovers .aspx/.ascx/.master files
│   │   └── OutputWriter.cs                   # Writes files, respects --dry-run
│   └── Services/
│       └── (reserved for future services)
```

### Project References

The `.csproj` **should reference the BWFC library**. PR #328 already does this correctly:

```xml
<ProjectReference Include="..\BlazorWebFormsComponents\BlazorWebFormsComponents.csproj" />
```

**Why:** The tool needs access to BWFC's type system for:
- Knowing which enum types exist (for `Normalize-AttributeValues` → `AttributeNormalizeTransform`)
- Validating component names during `asp:` prefix stripping
- Future: Roslyn-based analysis that resolves BWFC component parameters

### NuGet Packaging

```xml
<PropertyGroup>
    <PackAsTool>true</PackAsTool>
    <ToolCommandName>webforms-to-blazor</ToolCommandName>
    <PackageId>Fritz.WebFormsToBlazor</PackageId>
    <Version>$(VersionPrefix)</Version>  <!-- Tied to NBGV versioning -->
</PropertyGroup>
```

**Installation:** `dotnet tool install --global Fritz.WebFormsToBlazor`

**Command name:** `webforms-to-blazor` — matches PR #328's existing `ToolCommandName`. Clear, descriptive, no ambiguity.

---

## 2. Service Architecture

### Pipeline Design: Sequential Pipeline with Shared Context

**Decision: Sequential pipeline, not middleware or visitor.**

Rationale: The PowerShell script processes transforms in a fixed, carefully ordered sequence (directives first, then expressions, then prefixes, then attributes). Order matters — `ConvertFrom-AjaxToolkitPrefix` must run before `ConvertFrom-AspPrefix`. A middleware pattern adds unnecessary flexibility that invites ordering bugs. A visitor pattern is wrong because we're doing regex-based text transforms, not AST walking.

```csharp
public class MigrationPipeline
{
    private readonly IReadOnlyList<IMarkupTransform> _markupTransforms;
    private readonly IReadOnlyList<ICodeBehindTransform> _codeBehindTransforms;
    private readonly ProjectScaffolder _scaffolder;
    private readonly WebConfigTransformer _configTransformer;
    private readonly OutputWriter _writer;
    
    public async Task<MigrationReport> ExecuteAsync(MigrationContext context)
    {
        // Phase 0: Scaffold
        if (!context.Options.SkipScaffold)
            await _scaffolder.GenerateAsync(context);
        
        // Phase 0.5: Config transforms
        await _configTransformer.TransformAsync(context);
        
        // Phase 1: Discover and transform files
        foreach (var sourceFile in context.SourceFiles)
        {
            // Pre-scan code-behind for cross-file data (DataBind map)
            var filePair = sourceFile.WithCodeBehind();
            
            // Markup pipeline
            var markup = filePair.MarkupContent;
            foreach (var transform in _markupTransforms)
                markup = transform.Apply(markup, filePair.Metadata);
            
            // Code-behind pipeline
            if (filePair.HasCodeBehind)
            {
                var codeBehind = filePair.CodeBehindContent;
                foreach (var transform in _codeBehindTransforms)
                    codeBehind = transform.Apply(codeBehind, filePair.Metadata);
                filePair.UpdateCodeBehind(codeBehind);
            }
            
            // Cross-file correlation (DataBind Items injection)
            markup = DataBindTransform.InjectItemsAttributes(markup, filePair.DataBindMap);
            
            filePair.UpdateMarkup(markup);
            await _writer.WriteAsync(filePair, context);
        }
        
        return context.BuildReport();
    }
}
```

### Transform Interfaces

```csharp
public interface IMarkupTransform
{
    string Name { get; }
    int Order { get; }  // Explicit ordering — no ambiguity
    string Apply(string content, FileMetadata metadata);
}

public interface ICodeBehindTransform
{
    string Name { get; }
    int Order { get; }
    string Apply(string content, FileMetadata metadata);
}
```

### Transform Registry

**Transforms are registered in DI with explicit ordering.**

```csharp
services.AddTransform<PageDirectiveTransform>(order: 100);
services.AddTransform<MasterDirectiveTransform>(order: 110);
services.AddTransform<ControlDirectiveTransform>(order: 120);
services.AddTransform<ImportDirectiveTransform>(order: 200);
services.AddTransform<RegisterDirectiveTransform>(order: 210);
services.AddTransform<ContentWrapperTransform>(order: 300);
services.AddTransform<FormWrapperTransform>(order: 310);
services.AddTransform<GetRouteUrlTransform>(order: 400);
services.AddTransform<ExpressionTransform>(order: 500);
services.AddTransform<LoginViewTransform>(order: 510);
services.AddTransform<SelectMethodTransform>(order: 520);
services.AddTransform<AjaxToolkitPrefixTransform>(order: 600);  // MUST run before AspPrefix
services.AddTransform<AspPrefixTransform>(order: 610);
services.AddTransform<AttributeStripTransform>(order: 700);
services.AddTransform<EventWiringTransform>(order: 710);
services.AddTransform<UrlReferenceTransform>(order: 720);
services.AddTransform<TemplatePlaceholderTransform>(order: 800);
services.AddTransform<AttributeNormalizeTransform>(order: 810);
services.AddTransform<DataSourceIdTransform>(order: 820);
```

Gaps in numbering (100, 200, 300…) allow inserting new transforms without renumbering.

### Cross-File Correlation

The `DataBindTransform` is the only transform that spans markup + code-behind. It works in two phases:

1. **Pre-scan phase** (`Get-DataBindMap` equivalent): Before the markup pipeline runs, `DataBindTransform.PreScan(codeBehindContent)` returns a `Dictionary<string, string>` mapping control IDs to generated field names.
2. **Code-behind phase:** `Convert-DataBindPattern` equivalent — rewrites `ctrl.DataSource = expr` to `_ctrlData = expr`, removes `.DataBind()` calls, injects field declarations.
3. **Markup injection phase:** After all other markup transforms, `Add-DataBindItemsAttribute` equivalent adds `Items="@_ctrlData"` to matching tags.

This is modeled as a `DataBindTransform` that implements both `ICodeBehindTransform` and exposes a static `InjectItemsAttributes` method called by the pipeline after the markup loop.

### MigrationContext

```csharp
public class MigrationContext
{
    public MigrationOptions Options { get; }           // CLI flags
    public string SourcePath { get; }
    public string OutputPath { get; }
    public string ProjectName { get; }                 // Sanitized from folder name
    public IReadOnlyList<SourceFile> SourceFiles { get; }
    public TransformLog Log { get; }                   // Structured transform log
    public ManualItemLog ManualItems { get; }           // Items needing human review
    public DatabaseProviderInfo DatabaseProvider { get; }
    public bool HasIdentity { get; }
    public bool HasModels { get; }
    public bool HasAjaxToolkitControls { get; set; }   // Set during transform
}
```

### File I/O

- **`SourceScanner`**: Walks the input directory, discovers `.aspx`, `.ascx`, `.master` files. Pairs them with code-behind (`.aspx.cs`, `.aspx.vb`). Returns `IReadOnlyList<SourceFile>`.
- **`OutputWriter`**: Writes transformed files to output directory. Respects `--dry-run` (logs what it would write). Handles directory creation, encoding (UTF-8 no BOM).

---

## 3. Transform Porting Plan

| PS Function Category | C# Service/Class | Notes |
|---------------------|-------------------|-------|
| **Pre-scan** (`Invoke-BwfcPrescan`) | `Prescanner` | BWFC001–BWFC014 pattern detection. Returns `PrescanReport`. |
| **Directive conversion** (`ConvertFrom-PageDirective`, `-MasterDirective`, `-ControlDirective`, `-RegisterDirective`, `-ImportDirective`) | `Directives/PageDirectiveTransform`, `MasterDirectiveTransform`, `ControlDirectiveTransform`, `RegisterDirectiveTransform`, `ImportDirectiveTransform` | 5 classes, 1:1 mapping. Page directive includes home-page dual-route and `<PageTitle>` extraction. |
| **Content/Form transforms** (`ConvertFrom-ContentWrappers`, `-FormWrapper`) | `Markup/ContentWrapperTransform`, `FormWrapperTransform` | ContentWrapper has HeadContent logic and TitleContent extraction. FormWrapper preserves `id` for CSS. |
| **Master page transforms** (`ConvertFrom-MasterPage`) | `Markup/MasterPageTransform` | `@inherits LayoutComponentBase`, ContentPlaceHolder→`@Body`, CSS/JS extraction. |
| **Expression transforms** (`ConvertFrom-Expressions`) | `Markup/ExpressionTransform` | Comments, Bind(), Eval(), Item., encoded/unencoded expressions. Largest single transform. |
| **Tag prefix transforms** (`ConvertFrom-AspPrefix`, `-AjaxToolkitPrefix`) | `Markup/AspPrefixTransform`, `AjaxToolkitPrefixTransform` | Ajax must run first. ContentTemplate stripping, uc: prefix handling. |
| **Attribute transforms** (`Remove-WebFormsAttributes`, `Normalize-AttributeValues`) | `Markup/AttributeStripTransform`, `AttributeNormalizeTransform` | Strip runat, ItemType→TItem, ID→id, boolean/enum/unit normalization. |
| **URL transforms** (`ConvertFrom-UrlReferences`) | `Markup/UrlReferenceTransform` | `~/` → `/` for href, NavigateUrl, ImageUrl. |
| **LoginView** (`ConvertFrom-LoginView`) | `Markup/LoginViewTransform` | Strips attributes, flags RoleGroups. |
| **SelectMethod** (`ConvertFrom-SelectMethod`) | `Markup/SelectMethodTransform` | Preserves attribute, adds TODO for delegate conversion. |
| **GetRouteUrl** (`ConvertFrom-GetRouteUrl`) | `Markup/GetRouteUrlTransform` | Page.GetRouteUrl → GetRouteUrlHelper.GetRouteUrl. |
| **DataSourceID** (`Add-DataSourceIDWarning`) | `Markup/DataSourceIdTransform` | Removes DataSourceID attrs, replaces data source controls with TODOs. |
| **Event wiring** (`Convert-EventHandlerWiring`) | `Markup/EventWiringTransform` | `OnClick="X"` → `OnClick="@X"`. |
| **Template placeholders** (`Convert-TemplatePlaceholders`) | `Markup/TemplatePlaceholderTransform` | Placeholder elements → `@context`. |
| **Code-behind copy** (`Copy-CodeBehind`) | `CodeBehind/UsingStripTransform`, `BaseClassStripTransform` | TODO header injection, System.Web.* stripping, base class removal. |
| **Response.Redirect** (`Copy-CodeBehind` inline) | `CodeBehind/ResponseRedirectTransform` | 4 patterns → NavigationManager.NavigateTo. Injects `[Inject]`. |
| **Session/ViewState detection** (`Copy-CodeBehind` inline) | `CodeBehind/SessionDetectTransform`, `ViewStateDetectTransform` | Detects keys, generates migration guidance blocks. |
| **IsPostBack guards** (`Remove-IsPostBackGuards`) | `CodeBehind/IsPostBackTransform` | Brace-counting unwrap (simple) or TODO annotation (complex). |
| **Page lifecycle** (`Convert-PageLifecycleMethods`) | `CodeBehind/PageLifecycleTransform` | Page_Load→OnInitializedAsync, Page_Init→OnInitialized, Page_PreRender→OnAfterRenderAsync. |
| **Event handler signatures** (`Convert-EventHandlerSignatures`) | `CodeBehind/EventHandlerSignatureTransform` | Strip sender+EventArgs (standard), keep specialized EventArgs. |
| **DataBind pattern** (`Get-DataBindMap`, `Convert-DataBindPattern`, `Add-DataBindItemsAttribute`) | `CodeBehind/DataBindTransform` | Cross-file. Pre-scan → code-behind rewrite → markup injection. |
| **.aspx URL cleanup** (inline in `Copy-CodeBehind`) | `CodeBehind/UrlCleanupTransform` | `"~/X.aspx"` → `"/X"` in string literals. |
| **Project scaffolding** (`New-ProjectScaffold`, `New-AppRazorScaffold`) | `Scaffolding/ProjectScaffolder` | .csproj, Program.cs, _Imports.razor, App.razor, Routes.razor, GlobalUsings.cs, launchSettings.json. |
| **Config transforms** (`Convert-WebConfigToAppSettings`, `Find-DatabaseProvider`) | `Config/WebConfigTransformer`, `DatabaseProviderDetector` | web.config → appsettings.json. Database provider detection from connection strings. |
| **Shim generation** (various) | `Scaffolding/ShimGenerator` | GlobalUsings.cs, WebFormsShims.cs, IdentityShims.cs. |
| **CSS/Script detection** (`Invoke-CssAutoDetection`, `Invoke-ScriptAutoDetection`) | `Scaffolding/ProjectScaffolder` (integrated) | Detects CSS/JS files and adds to App.razor `<head>`. |
| **App_Start copy** (`Copy-AppStart`) | `Scaffolding/ProjectScaffolder` (integrated) | Copies RouteConfig.cs, BundleConfig.cs with TODO annotations. |
| **Redirect handler detection** (`Test-RedirectHandler`, `New-CompilableStub`) | `Analysis/Prescanner` (integrated) | Detect minimal markup + Response.Redirect code-behind. |
| **Logging** (`Write-TransformLog`, `Write-ManualItem`) | `MigrationContext.Log`, `MigrationContext.ManualItems` | Structured logging replaces script globals. |

---

## 4. CLI Interface Design

### Commands

```
webforms-to-blazor migrate     # Full project migration (primary command)
webforms-to-blazor convert     # Single file conversion
```

> **Note:** The `Prescanner` and `Analysis/` modules exist internally to power `migrate`'s
> pre-scan phase. There is no public `analyze` subcommand — analysis runs automatically
> as part of `migrate` and its results appear in the `--report` output.

### `migrate` — Full Project Migration

```
webforms-to-blazor migrate --input <path> --output <path> [options]

Options:
  -i, --input <path>         Source Web Forms project root (required)
  -o, --output <path>        Output Blazor project directory (required)
  --skip-scaffold            Skip .csproj, Program.cs, _Imports.razor generation
  --dry-run                  Show transforms without writing files
  -v, --verbose              Detailed per-file transform log
  --overwrite                Overwrite existing files in output directory
  --report <path>            Write JSON migration report to file
  --report-format <format>   Report format: json | markdown (default: json)
```

### `convert` — Single File

```
webforms-to-blazor convert --input <file> [options]

Options:
  -i, --input <file>         .aspx, .ascx, or .master file (required)
  -o, --output <path>        Output directory (default: same directory)
  --overwrite                Overwrite existing .razor file
```

### Design Decisions

- **Both project and single-file modes.** The `migrate` command is the primary workflow. `convert` exists for incremental migration and testing individual files. Analysis runs automatically as part of `migrate` — the pre-scan results feed into `--report` output without exposing a separate command.
- **Copilot orchestrates L2 transforms.** The CLI tool is a pure L1 deterministic engine. Items it cannot handle get structured `// TODO(bwfc-*)` comments that Copilot skills recognize and act on. Copilot calls the CLI tool (via skill-chaining), reads the migration report, and applies L2 contextual transforms. This keeps the tool self-contained with no AI dependencies while enabling the full 3-layer migration workflow.
- **`--dry-run`** is the replacement for PowerShell's `-WhatIf`. Logs all transforms to console without writing any files.
- **`--report`** generates a structured report (JSON by default) with pass/fail metrics, transform counts, and manual items. This enables CI integration and Copilot skill consumption.

---

## 5. Testing Strategy

### Port the 25 L1 Test Cases as xUnit Tests

The existing 25 test cases (`TC01-AspPrefix` through `TC25-DataBindAndEvents`) become parameterized xUnit tests:

```csharp
[Theory]
[MemberData(nameof(L1TestCases))]
public async Task L1Transform_ProducesExpectedOutput(string testCaseName)
{
    // Arrange
    var inputPath = Path.Combine(TestDataRoot, "inputs", $"{testCaseName}.aspx");
    var expectedPath = Path.Combine(TestDataRoot, "expected", $"{testCaseName}.razor");
    
    // Act
    var result = await _pipeline.TransformFileAsync(inputPath);
    
    // Assert
    var expected = NormalizeContent(await File.ReadAllTextAsync(expectedPath));
    var actual = NormalizeContent(result.MarkupContent);
    Assert.Equal(expected, actual);
    
    // Also verify code-behind if expected file exists
    var expectedCsPath = expectedPath + ".cs";
    if (File.Exists(expectedCsPath))
    {
        var expectedCs = NormalizeContent(await File.ReadAllTextAsync(expectedCsPath));
        var actualCs = NormalizeContent(result.CodeBehindContent!);
        Assert.Equal(expectedCs, actualCs);
    }
}
```

### Test Project Layout

```
tests/
├── BlazorWebFormsComponents.Cli.Tests/
│   ├── BlazorWebFormsComponents.Cli.Tests.csproj
│   ├── L1TransformTests.cs                  # 25 parameterized test cases
│   ├── TransformUnit/
│   │   ├── AspPrefixTransformTests.cs       # Unit tests per transform
│   │   ├── ExpressionTransformTests.cs
│   │   ├── IsPostBackTransformTests.cs
│   │   └── ...
│   ├── PipelineIntegrationTests.cs          # Full pipeline end-to-end
│   ├── ScaffoldingTests.cs                  # Project scaffold generation
│   ├── CliTests.cs                          # System.CommandLine argument parsing
│   └── TestData/                            # Copied from migration-toolkit/tests/
│       ├── inputs/                          # TC01–TC25 .aspx + .aspx.cs files
│       └── expected/                        # TC01–TC25 .razor + .razor.cs files
```

### Test Categories

1. **L1 acceptance tests** (25 cases): Exact output matching against the same expected files the PowerShell harness uses. These are the gate — the C# tool MUST pass all 25 before the PowerShell script is deprecated.
2. **Unit tests per transform**: Each `IMarkupTransform` and `ICodeBehindTransform` gets focused tests. Faster feedback, easier debugging.
3. **Integration tests**: Full `MigrationPipeline` end-to-end with realistic project structures.
4. **CLI parsing tests**: Verify `System.CommandLine` argument handling.
5. **Scaffold tests**: Verify generated `.csproj`, `Program.cs`, `_Imports.razor` content.

### How to Run

```bash
dotnet test tests/BlazorWebFormsComponents.Cli.Tests/
```

Integrate into the existing CI matrix alongside the 2,606 existing BWFC component tests.

---

## 6. Migration Path from PowerShell

### Incremental Porting Strategy

**Phase 1 — Scaffold + Directives + Prefixes (Week 1–2)**
Port the "easy wins" that cover TC01–TC09:
- `ProjectScaffolder` (New-ProjectScaffold, New-AppRazorScaffold)
- All 5 directive transforms
- `AspPrefixTransform`, `AjaxToolkitPrefixTransform`
- `AttributeStripTransform`
- `FormWrapperTransform`, `ContentWrapperTransform`
- `ExpressionTransform`
- `UrlReferenceTransform`

**Run the 25 L1 test cases after each phase.** Track pass rate.

**Phase 2 — Attribute Normalization + Markup Transforms (Week 3)**
Port TC10–TC12, TC17:
- `AttributeNormalizeTransform` (booleans, enums, units)
- `DataSourceIdTransform`
- `LoginViewTransform`, `SelectMethodTransform`, `GetRouteUrlTransform`
- `EventWiringTransform`, `TemplatePlaceholderTransform`

**Phase 3 — Code-Behind Transforms (Week 4–5)**
Port TC13–TC25:
- `UsingStripTransform`, `BaseClassStripTransform`
- `ResponseRedirectTransform`
- `SessionDetectTransform`, `ViewStateDetectTransform`
- `IsPostBackTransform`
- `PageLifecycleTransform`, `EventHandlerSignatureTransform`
- `DataBindTransform` (cross-file correlation)
- `UrlCleanupTransform`

**Phase 4 — Config + Scaffolding + Polish (Week 6)**
- `WebConfigTransformer`
- `DatabaseProviderDetector`
- `ShimGenerator`
- CSS/Script auto-detection
- Report generation

### Script Deprecation Timeline

| Milestone | Criteria | Action |
|-----------|----------|--------|
| **Parity** | C# tool passes all 25 L1 tests | Add deprecation notice to `bwfc-migrate.ps1` header |
| **Supersede** | C# tool passes + ships as NuGet tool | `bwfc-migrate.ps1` emits warning: "Use `webforms-to-blazor migrate` instead" |
| **Retire** | 2 releases after Supersede | Remove `bwfc-migrate.ps1` from repo, redirect docs |

### Existing Test Harness

`Run-L1Tests.ps1` stays as-is until the C# tool reaches parity. Once the xUnit tests pass all 25 cases, the PowerShell harness becomes redundant but can be kept as a cross-validation tool.

**No hybrid mode.** The C# tool should NOT shell out to the PowerShell script for unported transforms. That defeats the security goal. Accept partial coverage during porting and track it via test pass rate.

---

## 7. Copilot Orchestration Model

### Architecture

The CLI tool is a **pure L1 deterministic engine** — no AI dependencies, no API keys, no external calls. Copilot orchestrates the migration by calling the tool and processing its output.

### How It Works

1. **Copilot invokes the CLI tool** via skill-chaining (SKILL.md files reference `webforms-to-blazor` as a CLI tool)
2. **L1 transforms run** — deterministic regex transforms handle ~70% of conversion
3. **Structured TODO comments** are emitted for items requiring contextual understanding:

```csharp
// TODO(bwfc-session-state): Session["CartId"] detected — convert to scoped service
// TODO(bwfc-identity-migration): FormsAuthentication.SignOut() → SignInManager.SignOutAsync()
```

4. **`--report` generates a migration report** (JSON) listing all flagged items with categories matching skill names
5. **Copilot reads the report** and applies L2 transforms using the appropriate BWFC skills (session-state, identity-migration, middleware, etc.)
6. **Developer handles L3** — irreducible manual work (~10-15%)

### Benefits

- **Security:** No API keys stored in or passed to the CLI tool. No network calls. Pure compiled binary.
- **Deterministic:** Same input always produces same output. Testable, auditable, CI-friendly.
- **Composable:** Copilot can call `convert` for single files or `migrate` for full projects, then layer on contextual transforms.
- **Offline:** Works without internet. AI enhancement is additive via Copilot, not required.

---

## 8. Security Considerations

### Why C# Over PowerShell

Jeff's core motivation: **reduce injection surface.**

- **No `Invoke-Expression`**: PowerShell scripts can be tricked into evaluating user input. The C# tool uses compiled regex patterns — no dynamic code execution.
- **No environment variable interpolation in transforms**: All regex patterns are compile-time constants or `Regex.Escape()`d inputs.
- **Signed NuGet package**: The tool ships via NuGet with package signing, establishing provenance.
- **No shell-out**: The tool does not invoke any external processes. Everything is in-process C#.

### Input Validation

```csharp
public static class PathValidator
{
    public static string ValidateInputPath(string path)
    {
        var resolved = Path.GetFullPath(path);
        if (!Directory.Exists(resolved) && !File.Exists(resolved))
            throw new FileNotFoundException($"Input path not found: {path}");
        
        // Prevent path traversal
        if (resolved.Contains(".."))
            throw new ArgumentException("Path traversal not allowed");
        
        return resolved;
    }
    
    public static string ValidateOutputPath(string path, string inputPath)
    {
        var resolved = Path.GetFullPath(path);
        
        // Prevent writing outside intended directory
        // (no writing to system directories, etc.)
        if (resolved.StartsWith(Path.GetTempPath(), StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Cannot write to temp directory");
        
        return resolved;
    }
}
```

### Content Safety

- **No `eval` or `CSharpScript`**: All transforms are regex-based string operations.
- **No deserialization of untrusted data**: The tool reads `.aspx`/`.cs` files as plain text. No XML deserialization of user controls (we regex-match, not parse).
- **web.config XML parsing**: Uses `XDocument` (LINQ to XML) which is safe against XXE by default in .NET.
- **Output encoding**: All files written as UTF-8. No content injection via file names — output paths are sanitized through `Path.GetFileName()`.

### NuGet Signing

```xml
<PropertyGroup>
    <SignAssembly>true</SignAssembly>
    <PackageRequireLicenseAcceptance>false</PackageRequireLicenseAcceptance>
</PropertyGroup>
```

The CI pipeline should sign the NuGet package with a code signing certificate before publishing to nuget.org.

---

## Appendix A: Transform Ordering (Markup Pipeline)

The exact ordering from `Convert-WebFormsFile` in the PowerShell script, preserved in the C# pipeline:

| Order | Transform | Why This Position |
|-------|-----------|-------------------|
| 100 | PageDirective | Must run first — extracts route, emits @page |
| 110 | MasterDirective | Removes <%@ Master %> |
| 120 | ControlDirective | Removes <%@ Control %> |
| 200 | ImportDirective | <%@ Import %> → @using |
| 210 | RegisterDirective | Removes <%@ Register %> |
| 300 | ContentWrapper | asp:Content → strip/HeadContent |
| 310 | FormWrapper | `<form runat>` → `<div>` |
| 400 | GetRouteUrl | Page.GetRouteUrl → helper (before expressions) |
| 500 | Expression | <%: %> → @(), Eval/Bind/Item (central transform) |
| 510 | LoginView | asp:LoginView → LoginView |
| 520 | SelectMethod | Preserve + TODO |
| 600 | AjaxToolkitPrefix | ajaxToolkit: → bare name (BEFORE asp:) |
| 610 | AspPrefix | asp: → bare name |
| 700 | AttributeStrip | runat, ItemType→TItem, ID→id |
| 710 | EventWiring | OnClick="X" → OnClick="@X" |
| 720 | UrlReference | ~/ → / |
| 800 | TemplatePlaceholder | placeholder elements → @context |
| 810 | AttributeNormalize | bool/enum/unit normalization |
| 820 | DataSourceId | Remove DataSourceID, replace data source controls |

## Appendix B: Code-Behind Transform Ordering

| Order | Transform | Why This Position |
|-------|-----------|-------------------|
| 100 | UsingStrip | Strip System.Web.* first (reduces noise for later transforms) |
| 200 | BaseClassStrip | Remove `: Page` etc. |
| 300 | ResponseRedirect | Response.Redirect → NavigationManager.NavigateTo |
| 400 | SessionDetect | Detect Session["key"], inject guidance |
| 410 | ViewStateDetect | Detect ViewState["key"], inject guidance |
| 500 | IsPostBack | Unwrap simple guards, TODO complex ones |
| 600 | PageLifecycle | Page_Load → OnInitializedAsync etc. |
| 700 | EventHandlerSignature | Strip sender+EventArgs |
| 800 | DataBind | DataSource/DataBind → field assignment |
| 900 | UrlCleanup | .aspx URL literals → clean routes |
