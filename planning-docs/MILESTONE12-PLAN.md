# Milestone 12 — Migration Analysis Tool (PoC)

**Created:** 2026-02-25
**Author:** Forge (Lead / Web Forms Reviewer)
**Branch:** `milestone12/migration-analysis-tool`
**Baseline:** dev (post-M9, 51/53 components, 1200+ tests)

---

## 1. Vision — The Full Picture

The **Migration Analysis Tool** is a CLI tool + Copilot agent extension that analyzes an existing ASP.NET Web Forms application and produces a detailed migration plan for moving to Blazor Server Interactive using BlazorWebFormsComponents.

At maturity, the tool:

1. **Scans** an entire Web Forms project: `.aspx` pages, `.aspx.cs` code-behind, `.ascx` user controls, `.master` master pages, `Web.config`, `Global.asax`, `App_Code/`, `.asmx` web services, `App_Themes/`
2. **Identifies** every `<asp:*>` control and server-side construct used in the project
3. **Maps** each control to its BlazorWebFormsComponents equivalent (or flags it as a gap)
4. **Analyzes** code-behind patterns: data binding (`Eval()`, `Bind()`, strongly-typed `ItemType`), event handlers (`Page_Load`, `Button_Click`, postback patterns), session/viewstate usage, `HttpContext` dependencies, server-side includes, response redirects
5. **Scores** each page by migration complexity: trivial (pure markup swap), moderate (needs code-behind refactoring), complex (architectural changes needed)
6. **Recommends** migration order: easiest pages first, dependency-aware (shared user controls before pages that reference them)
7. **Generates** scaffolded `.razor` files for trivial pages — strips `asp:` prefixes, converts directives, rewrites code-behind to partial class
8. **Produces** a structured report (Markdown + JSON) suitable for project planning

### Why This Matters

We built 51 components that match Web Forms controls. But the actual migration workflow is still manual: a developer reads each `.aspx`, mentally maps controls, assesses code-behind, creates `.razor` files by hand. The biggest value we can add now is not more components — it's **reducing the friction of using the components we already have.**

This tool is the natural next step for the BlazorWebFormsComponents ecosystem. It turns a library into a migration platform.

---

## 2. PoC Scope — Milestone 12 Deliverable

The PoC is **not** the full vision. It demonstrates the core analysis pipeline on a representative Web Forms project and produces a readable report. No Roslyn analysis. No scaffolding. No Copilot agent integration. Those are post-PoC.

### What the PoC DOES:

| Capability | Description |
|------------|-------------|
| **ASPX parsing** | Parse `.aspx` files to extract `<asp:*>` control declarations with attributes |
| **Control mapping** | Map each `<asp:*>` control to the BlazorWebFormsComponents equivalent using a hardcoded mapping table derived from our `status.md` |
| **Gap identification** | Flag controls that have NO equivalent (e.g., `<asp:Wizard>`, `<asp:SqlDataSource>`, `<asp:ObjectDataSource>`, `<asp:ScriptManager>`) |
| **Page inventory** | List every `.aspx`, `.ascx`, `.master` file in the project with control counts |
| **Complexity scoring** | Score each page: Green (all controls have equivalents), Yellow (some gaps), Red (major gaps or heavy server-side logic) |
| **Code-behind pattern detection** | Regex-based detection of common patterns in `.aspx.cs` files: `Page_Load`, `Session[`, `ViewState[`, `Response.Redirect`, `Server.Transfer`, `DataBind()`, `IsPostBack` |
| **Markdown report** | Structured Markdown report with summary, per-page breakdown, gap list, and recommended migration order |
| **JSON output** | Machine-readable JSON with the same data for downstream tooling |

### What the PoC does NOT do:

| Excluded | Reason |
|----------|--------|
| Roslyn/semantic analysis | Too heavy for a PoC — regex-based pattern detection is sufficient to demonstrate value |
| Scaffolded `.razor` generation | Requires robust ASP.NET markup parsing + transformation; Phase 2 |
| Copilot agent/extension | Requires VS Code / GitHub Copilot extension infrastructure; Phase 3 |
| `Web.config` deep analysis | Authentication, authorization, connection strings — useful but not core to the PoC |
| NuGet dependency analysis | Tracking which NuGet packages need .NET Core equivalents; Phase 2 |
| VB.NET code-behind | PoC targets C# only |
| Custom control analysis | Detecting `WebControl`/`CompositeControl` subclasses; Phase 2 |

### PoC Success Criteria:

1. Run the CLI against the `samples/BeforeWebForms` project (if it exists) or a synthetic test project
2. Produce a Markdown report listing all pages, controls used, gaps, and complexity scores
3. Produce a JSON report with the same data
4. Correctly identify ≥90% of `<asp:*>` controls in the test project
5. Correctly map controls to their BWFC equivalents per `status.md`
6. Complete analysis of a 20-page Web Forms project in <5 seconds

---

## 3. Architecture

### 3.1 Project Location

**New project in the same repo:** `src/BlazorWebFormsComponents.MigrationAnalysis/`

Rationale:
- The mapping table (which Web Forms controls → which BWFC components) is tightly coupled to the component library. Same repo means the mapping stays in sync.
- Shares the solution file, CI pipeline, and versioning (nbgv).
- Published as a separate NuGet tool package: `Fritz.BlazorWebFormsComponents.MigrationAnalysis`
- If/when we build a Copilot agent extension, it can reference this project for its analysis engine.

A separate test project: `src/BlazorWebFormsComponents.MigrationAnalysis.Tests/`

### 3.2 Technology Stack

| Layer | Technology | Why |
|-------|-----------|-----|
| **CLI framework** | `System.CommandLine` | Modern .NET CLI framework, supports `--help`, `--output`, `--format` etc. |
| **ASPX parsing** | Regex + custom tokenizer | Full HTML/ASPX parsing (e.g., HtmlAgilityPack) is overkill for the PoC. `<asp:ControlName` patterns are highly regular. A simple regex tokenizer that extracts tag name + attributes is sufficient. |
| **Code-behind analysis** | Regex pattern matching | Detect `Page_Load`, `Session[`, `ViewState[`, `IsPostBack`, `DataBind()`, `Response.Redirect`, `Server.Transfer` etc. No Roslyn needed for PoC. |
| **Control mapping** | Static dictionary | `Dictionary<string, ControlMapping>` where key is the Web Forms control name (e.g., `"GridView"`) and value contains the BWFC equivalent, supported attributes, and any migration notes. |
| **Report generation** | StringBuilder + System.Text.Json | Markdown via string templates. JSON via `System.Text.Json` serialization. |
| **Target framework** | `net10.0` (console app) | Matches the rest of the repo. Published as a .NET tool. |

### 3.3 Component Map

The control mapping is the heart of the tool. It encodes our 51 implemented components plus all known Web Forms controls we intentionally don't support:

```
SUPPORTED (51 components):
  asp:Button → <Button> ✅
  asp:TextBox → <TextBox> ✅
  asp:GridView → <GridView> ✅
  asp:Label → <Label> ✅
  ... (full map derived from status.md)

DEFERRED (2 components):
  asp:Substitution → ⏸️ No Blazor equivalent
  asp:Xml → ⏸️ XSLT rarely used

EXPLICITLY UNSUPPORTED (never planned):
  asp:SqlDataSource → ❌ Use DI + EF Core
  asp:ObjectDataSource → ❌ Use DI
  asp:EntityDataSource → ❌ Use DI + EF Core
  asp:LinqDataSource → ❌ Use LINQ in code
  asp:XmlDataSource → ❌ Use data services
  asp:AccessDataSource → ❌ Use EF Core
  asp:SiteMapDataSource → ❌ Use NavigationManager
  asp:ScriptManager → ❌ Use Blazor JS interop
  asp:UpdatePanel → ❌ Blazor is inherently SPA
  asp:UpdateProgress → ❌ Use Blazor loading patterns
  asp:Timer → ❌ Use System.Threading.Timer
  asp:Wizard → ❌ Manual multi-step in Blazor
  asp:DynamicData → ❌ Not applicable
```

### 3.4 CLI Interface

```bash
# Basic analysis
dotnet bwfc-migrate analyze --project "C:\path\to\WebFormsApp.csproj"

# With output options
dotnet bwfc-migrate analyze \
  --project "C:\path\to\WebFormsApp.csproj" \
  --output "./migration-report" \
  --format markdown json \
  --verbose

# Scan a directory (no .csproj required)
dotnet bwfc-migrate analyze --path "C:\path\to\WebFormsApp\"
```

### 3.5 Report Structure

```
Migration Analysis Report
═══════════════════════════

Project: MyWebFormsApp
Scanned: 2026-02-25
Pages: 24 (.aspx) | Controls: 8 (.ascx) | Master Pages: 2 (.master)

Summary
───────
✅ Ready for migration:     14 pages (58%)
⚠️  Needs some custom work:   7 pages (29%)
❌ Requires significant work: 3 pages (13%)

Control Coverage
────────────────
Controls used:       18 unique Web Forms controls
With BWFC equivalent: 15 (83%)
No equivalent:         3 (17%) — Wizard, SqlDataSource, ScriptManager

Migration Gaps
──────────────
| Control          | Pages Using It | Recommendation                    |
|------------------|---------------|-----------------------------------|
| asp:Wizard       | 2             | Rewrite as multi-step Blazor form |
| asp:SqlDataSource| 5             | Replace with EF Core + DI         |
| asp:ScriptManager| 8             | Remove — Blazor handles JS interop|

Code-Behind Patterns
────────────────────
| Pattern            | Pages | Migration Impact |
|--------------------|-------|------------------|
| Page_Load          | 22    | → OnInitialized  |
| IsPostBack         | 18    | → Remove (no postback in Blazor) |
| Session["..."]     | 9     | → Inject IHttpContextAccessor or use Blazor state |
| ViewState["..."]   | 6     | → Use component parameters or fields |
| Response.Redirect  | 11    | → NavigationManager.NavigateTo    |
| Server.Transfer    | 2     | → NavigationManager.NavigateTo    |
| DataBind()         | 14    | → Automatic with parameter binding |

Recommended Migration Order
────────────────────────────
1. About.aspx (Green — 3 controls, all supported, no code-behind patterns)
2. Contact.aspx (Green — 5 controls, all supported, simple Page_Load)
3. Products/List.aspx (Yellow — GridView + SqlDataSource, replace data source)
...

Per-Page Detail
───────────────
### Default.aspx (⚠️ Yellow)
Controls: Button, Label, TextBox, GridView, SqlDataSource
Code-behind: Page_Load, IsPostBack, Session["UserName"], DataBind()
Gaps: SqlDataSource (replace with EF Core service)
Complexity: Moderate — markup is drop-in, but data access needs refactoring
```

---

## 4. Work Items

### P0 — Core Analysis Engine (6 WIs)

| ID | Title | Description | Agent | Dependencies | Size | Priority |
|----|-------|-------------|-------|-------------|------|----------|
| WI-01 | **Create project skeleton** | Create `src/BlazorWebFormsComponents.MigrationAnalysis/` as a `net10.0` console app. Add `System.CommandLine` NuGet reference. Set up `dotnet tool` packaging in `.csproj` (`<PackAsTool>true</PackAsTool>`, `<ToolCommandName>bwfc-migrate</ToolCommandName>`). Add project to `BlazorMeetsWebForms.sln`. Create `src/BlazorWebFormsComponents.MigrationAnalysis.Tests/` xUnit test project. | Cyclops | — | S | P0 |
| WI-02 | **ASPX control extractor** | Implement `AspxControlExtractor` class that takes a file path (`.aspx`, `.ascx`, `.master`) and returns a list of `ControlReference` objects: `{ ControlName, Attributes (Dictionary), LineNumber, FilePath }`. Use regex to find `<asp:(\w+)` patterns and extract attribute key-value pairs. Handle self-closing tags (`/>`) and content tags (`</asp:...>`). Handle multi-line declarations. Write xUnit tests with sample `.aspx` content covering: single control, multiple controls, nested controls, self-closing, multi-line attributes, non-asp tags (should be ignored). | Cyclops | WI-01 | M | P0 |
| WI-03 | **Control mapping registry** | Implement `ControlMappingRegistry` class containing the mapping of all 53 planned Web Forms controls to their BWFC equivalents (51 supported, 2 deferred). Add entries for ~15 known unsupported controls (data sources, ScriptManager, UpdatePanel, Wizard, etc.) with migration notes. Each mapping entry: `{ WebFormsName, BwfcEquivalent (nullable), Status (Supported/Deferred/Unsupported), MigrationNote }`. Derive the supported list from `status.md`. Write tests verifying all 51 supported controls map correctly. | Cyclops | WI-01 | M | P0 |
| WI-04 | **Code-behind pattern analyzer** | Implement `CodeBehindAnalyzer` class that takes a `.cs` file path and returns detected patterns: `Page_Load`, `Page_Init`, `Page_PreRender`, `IsPostBack`, `Session["..."]`, `ViewState["..."]`, `Response.Redirect`, `Server.Transfer`, `Server.MapPath`, `DataBind()`, `FindControl()`, `Eval()`/`Bind()`, `RegisterStartupScript`, `RegisterClientScriptBlock`, `HttpContext.Current`. Each pattern includes migration guidance text. Regex-based — no Roslyn. Write tests with synthetic code-behind samples. | Cyclops | WI-01 | M | P0 |
| WI-05 | **Project scanner & page inventory** | Implement `ProjectScanner` class that takes a project directory or `.csproj` path and discovers all `.aspx`, `.aspx.cs`, `.aspx.designer.cs`, `.ascx`, `.ascx.cs`, `.master`, `.master.cs`, `Web.config`, `Global.asax`, `Global.asax.cs` files. Returns a `ProjectInventory` with file counts by type, list of pages with their code-behind associations (matching `Default.aspx` ↔ `Default.aspx.cs`), and directory structure. Write tests with a mock file system or temp directory. | Cyclops | WI-01 | M | P0 |
| WI-06 | **Complexity scorer** | Implement `ComplexityScorer` that takes a page's extracted controls + code-behind patterns and produces a complexity score: `Green` (all controls supported, ≤2 code-behind patterns, no data source controls), `Yellow` (≤2 unsupported controls OR 3-5 code-behind patterns), `Red` (≥3 unsupported controls OR session/viewstate heavy OR uses Server.Transfer/FindControl/RegisterScript). Returns `{ Score, Reasons[] }`. Write tests for each threshold. | Cyclops | WI-02, WI-03, WI-04 | S | P0 |

### P1 — Report Generation & CLI (4 WIs)

| ID | Title | Description | Agent | Dependencies | Size | Priority |
|----|-------|-------------|-------|-------------|------|----------|
| WI-07 | **Markdown report generator** | Implement `MarkdownReportGenerator` that takes a `MigrationAnalysisResult` (output of full pipeline) and produces a Markdown report matching the structure in Section 3.5: summary, control coverage, migration gaps table, code-behind patterns table, recommended migration order (sorted by complexity score ascending, then alphabetical), per-page detail. Write tests verifying report structure. | Cyclops | WI-06 | M | P1 |
| WI-08 | **JSON report generator** | Implement `JsonReportGenerator` that serializes `MigrationAnalysisResult` to indented JSON using `System.Text.Json`. The JSON schema should be documented in a `schema.md` or `report-schema.json` file. Write tests verifying JSON structure and roundtrip deserialization. | Cyclops | WI-06 | S | P1 |
| WI-09 | **CLI entry point** | Wire up `System.CommandLine` with an `analyze` command accepting `--project` (path to `.csproj`), `--path` (directory path, alternative to `--project`), `--output` (output directory, default: current dir), `--format` (markdown, json, or both; default: markdown), `--verbose` (detailed per-file output). Orchestrate: ProjectScanner → AspxControlExtractor (per file) → CodeBehindAnalyzer (per code-behind) → ComplexityScorer → ReportGenerator. | Cyclops | WI-07, WI-08 | M | P1 |
| WI-10 | **CLI tests & integration test** | Create a synthetic Web Forms project in the test project (`TestData/SampleWebFormsApp/`) with 5-10 `.aspx` files covering various control combinations, code-behind patterns, and complexity levels. Run the full CLI pipeline against it. Verify the Markdown output contains expected sections and the JSON output deserializes correctly. This is the end-to-end validation. | Rogue | WI-09 | M | P1 |

### P2 — Documentation & Samples (3 WIs)

| ID | Title | Description | Agent | Dependencies | Size | Priority |
|----|-------|-------------|-------|-------------|------|----------|
| WI-11 | **Tool documentation** | Write docs for the migration analysis tool in `docs/Migration/MigrationAnalysisTool.md`. Cover: installation (`dotnet tool install`), usage, CLI options, interpreting the report, limitations, and what to do with the results. Link from the main migration guide (`docs/Migration/readme.md`). | Beast | WI-09 | M | P2 |
| WI-12 | **Sample migration report** | Generate a sample migration report from the test data project and include it in `docs/Migration/SampleMigrationReport.md`. This serves as a reference for users to understand what the tool produces. | Beast | WI-10 | S | P2 |
| WI-13 | **README update** | Update the repo `README.md` to mention the migration analysis tool under a new "Migration Tooling" section. Brief description + link to full docs. Keep it concise — 3-4 lines. | Beast | WI-11 | XS | P2 |

---

## 5. Phase 2 & Phase 3 Roadmap (Post-PoC)

These are NOT in Milestone 12. Documenting them here for Jeff's visibility.

### Phase 2 — Enhanced Analysis + Scaffolding (Milestone 13 or 14)

| Capability | Description |
|------------|-------------|
| **Roslyn-based code-behind analysis** | Replace regex with Roslyn `SyntaxTree` analysis for code-behind. Detect event handler signatures, method call chains, class inheritance. Much higher fidelity. |
| **Razor scaffolding** | Generate `.razor` files from `.aspx` files: strip `asp:` prefixes, convert `<%@ Page %>` directives to `@page`/`@layout`, move code-behind to partial class. Handle simple cases automatically. |
| **Custom control detection** | Find classes inheriting `System.Web.UI.WebControl`, `CompositeControl`, `UserControl`. Map to our `CustomControls/WebControl` migration adapter. |
| **Web.config analysis** | Parse authentication config (`<authentication mode="Forms">`), connection strings, custom handlers, HTTP modules. Produce ASP.NET Core equivalents. |
| **NuGet dependency analysis** | Read `packages.config` or `PackageReference` items. Check which packages have .NET Core equivalents. |
| **VB.NET support** | Code-behind analysis for `.aspx.vb` files. |
| **Attribute-level mapping** | Not just "does the control exist?" but "are all the attributes used on this control supported by BWFC?" Per-attribute gap analysis. |

### Phase 3 — Copilot Agent Integration (Milestone 15+)

| Capability | Description |
|------------|-------------|
| **GitHub Copilot Extension** | Package the analysis engine as a Copilot agent that can be invoked via `@bwfc-migrate analyze` in Copilot Chat. |
| **VS Code extension** | Provide inline diagnostics in `.aspx` files showing migration status per control. |
| **Interactive migration** | Agent-guided migration: analyze → review report → approve page → generate scaffolded `.razor` → iterate. |
| **GitHub Actions integration** | Run migration analysis as a CI step on PRs that modify Web Forms projects. |

---

## 6. Open Questions for Jeff

These need answers before or during implementation:

| # | Question | Impact | Forge's Recommendation |
|---|----------|--------|----------------------|
| 1 | **Same repo or separate repo?** | Project structure. | Same repo — the mapping table is tightly coupled to our component list. A separate repo means the mapping drifts. |
| 2 | **Tool name: `bwfc-migrate` or something else?** | CLI ergonomics. | `bwfc-migrate` is descriptive. Alternative: `blazor-migrate`. |
| 3 | **Do we have a reference Web Forms app to test against?** | Test data quality. `samples/BeforeWebForms` exists in older branches but may be stale. | Build a synthetic test project in the test folder. Optionally test against `BeforeWebForms` if it's still around. |
| 4 | **Should the tool detect third-party controls (Telerik, DevExpress, Infragistics)?** | Scope. | Not in PoC. Phase 2 could add a plugin/extension model for third-party control mappings. |
| 5 | **JSON schema: should we publish a formal JSON Schema for the report?** | Downstream tooling. | Yes in Phase 2. For PoC, just document the structure in a markdown file. |
| 6 | **Should this be a `dotnet tool` (global/local) or a standalone executable?** | Distribution. | `dotnet tool` — matches the .NET ecosystem, easy install via `dotnet tool install -g Fritz.BlazorWebFormsComponents.MigrationAnalysis`. |
| 7 | **Copilot agent integration timeline?** | Phase 3 planning. | Not before the PoC proves the analysis engine works. Phase 3 at earliest. |

---

## 7. Summary

| Priority | Work Items | Theme |
|----------|-----------|-------|
| P0 | 6 | Core analysis engine (parsing, mapping, scoring) |
| P1 | 4 | Report generation + CLI wiring |
| P2 | 3 | Documentation |
| **Total** | **13** | |

### Agent Assignments

| Agent | Work Items | Load |
|-------|-----------|------|
| Cyclops | WI-01 through WI-09 | 9 WIs (S + 4M + S + 2M + M) — heavy load, but sequential pipeline |
| Rogue | WI-10 | 1 WI (M) — end-to-end test |
| Beast | WI-11, WI-12, WI-13 | 3 WIs (M + S + XS) — docs |

### Dependencies

```
WI-01 ──→ WI-02, WI-03, WI-04, WI-05  (project skeleton first)
WI-02 ──→ WI-06  (extractor before scorer)
WI-03 ──→ WI-06  (mapping before scorer)
WI-04 ──→ WI-06  (analyzer before scorer)
WI-06 ──→ WI-07, WI-08  (scorer before report generators)
WI-07 ──→ WI-09  (reports before CLI wiring)
WI-08 ──→ WI-09
WI-09 ──→ WI-10  (CLI before integration test)
WI-09 ──→ WI-11  (CLI before docs)
WI-10 ──→ WI-12  (test data before sample report)
WI-11 ──→ WI-13  (tool docs before README update)
```

### Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|-----------|
| Regex-based ASPX parsing misses edge cases | Medium | Low | PoC explicitly scopes to `<asp:*>` patterns only. Phase 2 adds proper parsing. |
| No real Web Forms test project available | Medium | Medium | Build a synthetic test project with representative patterns. |
| Control mapping drifts from actual component library | Low | Medium | Mapping lives in same repo. CI could validate mapping against actual component classes. |
| System.CommandLine learning curve | Low | Low | Well-documented library. Forge has reviewed similar tools. |
| Scope creep into Roslyn analysis | Medium | High | Hard boundary: PoC is regex-only. Roslyn is Phase 2. Period. |

### Exit Criteria

1. CLI tool builds and runs against a test Web Forms project
2. Markdown report produced with summary, per-page breakdown, gap list, complexity scores
3. JSON report produced with matching data
4. All controls in `status.md` correctly represented in the mapping registry
5. 10+ unit tests covering the analysis pipeline
6. 1 end-to-end integration test
7. Documentation published in `docs/Migration/`

---

## 8. Why Now — Strategic Context

We're at 51/53 components. The component library is mature. The remaining work (Milestones 9-11) is hardening and polish. The highest-value thing we can do for the migration story is not building the 52nd component — it's helping developers actually USE the 51 we already have.

Every Web Forms developer evaluating BlazorWebFormsComponents asks the same question: "Will this work for MY app?" This tool answers that question programmatically. It turns a week of manual analysis into a 5-second CLI invocation.

This is the tool that makes BlazorWebFormsComponents a migration *platform*, not just a component *library*.

— Forge
