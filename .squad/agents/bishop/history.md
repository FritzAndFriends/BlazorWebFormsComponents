# Bishop - Migration Tooling Dev History

## Role
Bishop is the Migration Tooling Dev on the BlazorWebFormsComponents project, responsible for building migration tools and utilities that help developers move from ASP.NET Web Forms to Blazor.

## Project Context
BlazorWebFormsComponents is a library providing Blazor components that emulate ASP.NET Web Forms controls, enabling migration with minimal markup changes. The project aims to preserve the same component names, attributes, and HTML output as the original Web Forms controls.

## Core Historical Context

**2025-01-26 to 2026-03-29:** Built foundational migration infrastructure:
- WI-8 (2025-01-26): .skin file parser + SkinFileParser.cs runtime parser for theme migration
- Theme Migration SKILL.md (2025-01-27): Documented auto-discovery pattern (copy → SkinFileParser → ThemeProvider)
- Migration Automation Audit (2026-07-25): Identified 23 gaps, proposed solutions across 3 phases
- Phase 1 L1 Script Enhancements (2026-07-25): Implemented 6 GAPs (Web.config→appsettings, IsPostBack unwrap, App_Start copy, selective using retention, URL cleanup, Bind()→@bind)
- Phase 2 Lifecycle & Event Handlers (2026-03-29): Added Page_Load→OnInitializedAsync, event handler signature transforms
- Global Tool Pipeline + 16 Markup Transforms (2026-07-27): Built complete MigrationPipeline, 16 markup transforms ported from PowerShell script
- MasterPageTransform + GetRouteUrlTransform + TC12–TC23 (2026-04-03): Added MasterPage directive rewriting, GetRouteUrl conversion, 12 new acceptance tests

## Learnings

📌 Team update (2026-04-12): All migration transforms pipeline infrastructure complete — 17 transforms (added 3: ConfigurationManager, RequestForm, ServerShim), 373/373 tests passing, expected files regenerated. WingtipToys analysis shows WebFormsPageBase enables 31 pages to eliminate manual shim wiring. — decided by Psylocke, Forge, Bishop

📌 Team update (2026-04-12): CLI tool references were added by Coordinator to all 4 migration-toolkit docs missing them (METHODOLOGY.md, CHECKLIST.md, README.md), ensuring consistent tool naming and linking across all migration-toolkit documentation. — decided by Coordinator

### Shim Inventory & CLI Transform Update (2026-04-12)

**Task**: Build a runtime parser that reads ASP.NET Web Forms .skin files and converts them into ThemeConfiguration objects.

**Implementation Details**:
- Created `SkinFileParser.cs` with three public methods:
  - `ParseSkinFile(string, ThemeConfiguration)` - parses .skin content from string
  - `ParseSkinFileFromPath(string, ThemeConfiguration)` - parses single .skin file from disk
  - `ParseThemeFolder(string, ThemeConfiguration)` - parses all .skin files in a directory
  
- Parsing approach:
  - Strip ASP.NET comments (`<%-- ... --%>`)
  - Wrap content in root element and replace `<asp:` with `<asp_` for XML compatibility
  - Parse as XML using XDocument
  - Walk element tree to build ControlSkin and TableItemStyle objects

- Type conversions:
  - WebColor: use `WebColor.FromHtml(value)` for color attributes
  - Unit: use `new Unit(value)` for size/width attributes
  - FontUnit: use `FontUnit.Parse(value)` for font sizes
  - BorderStyle: use `Enum.TryParse<BorderStyle>()` for enum values
  - Font attributes: special handling for `Font-Bold`, `Font-Italic`, `Font-Size`, etc.

- Sub-styles: Nested elements like `<HeaderStyle>`, `<RowStyle>` become entries in `ControlSkin.SubStyles` dictionary as `TableItemStyle` objects

- Error handling: Defensive parsing with try-catch blocks and console warnings, never throws on parse errors

**Key Technical Decisions**:
1. Used XML parsing after preprocessing rather than custom parser - leverages proven XML infrastructure
2. Case-insensitive attribute and control name matching for robustness
3. Silently ignore unknown attributes to handle variations in .skin files
4. Console.WriteLine for warnings rather than throwing exceptions - allows partial parsing success

**Build Status**: ✅ Successfully builds with no errors

**Verification**: ✅ Tested with sample .skin content:
- Successfully parsed default Button skin with colors and font properties
- Successfully parsed named Button skin (SkinID="DangerButton")
- Successfully parsed GridView with nested HeaderStyle and RowStyle sub-components
- All color conversions, font attributes, and sub-styles worked correctly

### Web Forms Theme Migration SKILL.md (2025-01-27)

**Task**: Write a SKILL.md that teaches Copilot and Squad agents how to migrate Web Forms themes to Blazor using BWFC auto-discovery.

**Delivered**:
- Created `.squad/skills/theme-migration/SKILL.md` as authoritative reference for theme migration pattern
- Documented Web Forms theme structure (App_Themes/ folder with .skin, .css, images)
- Explained auto-discovery flow: copy → `AddBlazorWebFormsComponents()` → `SkinFileParser` → `ThemeProvider` injection
- Covered key concepts:
  - Theme folder identification and copy operation (preserve structure)
  - Default theme selection (first folder alphabetically)
  - CSS auto-discovery and injection via ThemeProvider
  - Named skins (SkinID parameter requirement in Blazor)
  - ThemeMode (StyleSheetTheme default vs. Theme override mode)
  - Multiple themes support and custom ThemesPath configuration
- Provided 3 detailed examples (simple, multiple themes, named skins)
- Documented edge cases (no themes, CSS-only themes, custom paths)
- Included anti-patterns with do's and don'ts (manual registration, missing ThemeProvider, image handling)

**Why This Matters**: This SKILL.md replaces the need for agent-specific migration scripts. Any agent (Copilot, Cyclops, Rogue, or future Squad members) doing Web Forms migration now has a standardized reference explaining the theme pattern. No more tribal knowledge — the pattern is documented, discoverable, and reusable across projects.

**Key Principle**: The SKILL teaches the "what" and "why" but delegates implementation details (SkinFileParser internals, ThemeProvider rendering) to library code. This keeps the SKILL maintainable as the implementation evolves.

### Migration Automation Audit (2026-07-25)

**Task**: Audit bwfc-migrate.ps1 (2,714 lines), all BWFC shims, and migration test results to identify manual gaps and propose automation solutions.

**Key Findings**:
- L1 script handles ~60% of migration mechanically (22 categories of transforms)
- BWFC library provides 20+ shims (WebFormsPageBase, ResponseShim, RequestShim, ViewStateDictionary, Handler framework, Middleware, Identity stubs, EF6 stubs)
- Identified 23 automation gaps with proposed solutions
- 9 quick wins shippable in single PRs (ConfigurationManager shim, Web.config→appsettings.json, IsPostBack unwrap, App_Themes auto-copy, BundleConfig stubs, etc.)
- Top 3 "sneaky wins": ConfigurationManager.AppSettings shim, Session["key"] shim, Web.config→appsettings.json extraction
- Missing shims: ConfigurationManager, HttpContext.Current, FormsAuthentication, Session (on WebFormsPageBase), BundleConfig/RouteConfig, Server.MapPath (outside handlers)
- Missing script transforms: Page_Load→OnInitializedAsync, event handler signatures, Bind() expressions, Global.asax extraction, Startup.cs/OWIN parsing
- WingtipToys Run 7: 14/14 tests pass, 366 transforms, 55 L2 files touched
- ContosoUniversity Run 22: 39/40 tests pass, EDMX parser handles all models

**Deliverable**: `dev-docs/migration-automation-opportunities.md` — structured report with gap inventory, proposed solutions, complexity estimates, and implementation order across 3 phases.

### Phase 1 L1 Script Enhancements (2026-07-25)

**Task**: Implement 6 GAP items from the migration automation audit as Phase 1 "Just Make It Compile" enhancements to `bwfc-migrate.ps1`.

**Implementation Details**:

- **GAP-12 (Web.config → appsettings.json)**: New `Convert-WebConfigToAppSettings` function. Parses `<appSettings>` and `<connectionStrings>` via XML, generates proper JSON structure, merges with existing appsettings.json. Placed in pipeline after scaffold generation, before code-behind copy. Skips built-in connection strings (LocalSqlServer).

- **GAP-06 (IsPostBack Guard Unwrapping)**: New `Remove-IsPostBackGuards` function with iterative brace-counting approach. Handles 6 pattern variants (`!IsPostBack`, `!Page.IsPostBack`, `!this.IsPostBack`, `== false` forms). Simple guards unwrapped with comment; complex guards (else clause) get TODO annotation. Max 50 iterations safety limit.

- **GAP-22 (App_Start/ Directory Copy)**: New `Copy-AppStart` function. Copies .cs files to output root (not App_Start/ subfolder). Strips `[assembly:]` attributes via multiline regex. Applies same selective using retention as code-behind. Flags WebApiConfig and FilterConfig for manual review.

- **GAP-09 (Selective Using Retention)**: Restructured Copy-CodeBehind using-stripping to preserve `System.Configuration`, `System.Web.Optimization`, `System.Web.Routing` as comments with BWFC equivalence notes. Then strips in specific order: UI → Security → remaining Web → AspNet → Owin. Prevents false-positive compile errors.

- **GAP-20 (.aspx URL Cleanup)**: 4-pattern transform in Copy-CodeBehind: tilde+query+aspx, tilde+aspx, relative+query in NavigateTo, relative in NavigateTo. Converts `"~/Page.aspx?q=v"` → `"/Page?q=v"`.

- **GAP-13 (Bind() → @bind Transform)**: Added to `ConvertFrom-Expressions` before Eval() transforms. Attribute-value Bind() → `@bind-Value="context.Prop"`. Standalone Bind() → `@context.Prop`. Handles both single-quoted and double-quoted attribute values.

**Key Technical Decisions**:
1. IsPostBack uses iterative brace-counting rather than regex for brace matching — regex can't reliably match nested braces
2. Using retention runs BEFORE blanket stripping — retained usings are converted to commented-out forms with BWFC notes so developers understand the equivalence
3. Bind() transforms run before Eval() in expression pipeline to avoid conflicts (Bind is more specific)
4. App_Start files go to output root, not a subdirectory — Blazor has no App_Start convention
5. URL cleanup handles NavigateTo context specifically to avoid false positives in non-URL strings

**Validation**: Script parses cleanly, WhatIf mode works, existing 15-test suite maintains 12/15 pass rate (3 pre-existing base-class-stripping failures unchanged). Script grew from 2,714 to ~3,000 lines.


## Phase 2: GAP-05 + GAP-07  Lifecycle and Event Handler Transforms (2026-03-29)

### GAP-05: Page Lifecycle Method Transform
Added `Convert-PageLifecycleMethods` function to bwfc-migrate.ps1:
- `Page_Load(object sender, EventArgs e)`  `protected override async Task OnInitializedAsync()` with `await base.OnInitializedAsync();` injection
- `Page_Init(object sender, EventArgs e)`  `protected override void OnInitialized()`
- `Page_PreRender(object sender, EventArgs e)`  `protected override async Task OnAfterRenderAsync(bool firstRender)` with `if (firstRender)` body guard
- Case-insensitive method name matching, handles all access modifier combinations
- TODO comments injected for developer review

### GAP-07: Event Handler Signature Transform  CRITICAL
Added `Convert-EventHandlerSignatures` function to bwfc-migrate.ps1:
- Standard `EventArgs`  strip both params: `Handler(object sender, EventArgs e)`  `Handler()`
- Specialized `*EventArgs` subtypes  strip sender only: `Handler(object sender, GridViewCommandEventArgs e)`  `Handler(GridViewCommandEventArgs e)`
- Decision logic: exact match on "EventArgs" type name determines strip-both vs keep-specialized
- Regex anchored to `\w*EventArgs` to avoid false positives on non-EventArgs parameter types
- Iterative processing handles multiple handlers per file (up to 200 safety limit)
- Access modifiers and async keywords preserved as-is

### Pipeline integration
Both functions called in `Copy-CodeBehind` after existing transforms, before file write:
1. Strip usings (existing)  2. IsPostBack unwrap (existing)  3. URL cleanup (existing)  4. Lifecycle convert (GAP-05)  5. Event handler signatures (GAP-07)  6. Write file

### Test results
Updated 6 expected test files (TC13, TC14, TC15, TC16, TC18, TC19) to reflect new transforms.
TC19 (lifecycle) and TC20/TC21 (event handlers) are dedicated test cases for these features.
**All 21 tests pass at 100% line accuracy.**


### Global Tool Pipeline Infrastructure + First 16 Markup Transforms (2026-07-27)

**Task**: Build the C# global tool pipeline from the architecture doc (`dev-docs/global-tool-architecture.md`), replacing the PR #328 single-converter approach with the full sequential pipeline.

**Implementation Details**:

- **Pipeline Infrastructure**: Created `MigrationPipeline.cs` (orchestrates IMarkupTransform + ICodeBehindTransform chains, sorted by Order), `MigrationContext.cs` (per-file + project state), `FileMetadata.cs` (per-file metadata with FileType enum), `TransformResult.cs` (immutable step result), `MigrationReport.cs` (summary metrics).

- **Transform Interfaces**: `IMarkupTransform` and `ICodeBehindTransform` with Name, Order, Apply(content, metadata) contract. All transforms are DI-registered singletons sorted by Order at pipeline construction time.

- **SourceScanner**: Discovers .aspx/.ascx/.master files, pairs with .cs/.vb code-behind, generates output paths with .razor extension.

- **16 Markup Transforms** (all regex patterns ported exactly from bwfc-migrate.ps1):
  - Directives (100-210): PageDirective, MasterDirective, ControlDirective, ImportDirective, RegisterDirective
  - Content/Form (300-310): ContentWrapper, FormWrapper
  - Expressions (500): ExpressionTransform (comments, Bind(), Eval(), Item., encoded/unencoded)
  - Tag Prefixes (600-610): AjaxToolkitPrefix, AspPrefix (+ ContentTemplate stripping, uc: prefix)
  - Attributes (700-720): AttributeStrip (runat, AutoEventWireup, etc. + ItemTypeTItem + IDid + ItemType="object" fallback), EventWiring, UrlReference
  - Normalization (800-820): TemplatePlaceholder, AttributeNormalize (booleans, enums, px units), DataSourceId

- **CLI Subcommands**: Replaced single root command with `migrate` (full project) and `convert` (single file) subcommands per architecture doc. Options: --input, --output, --skip-scaffold, --dry-run, --verbose, --overwrite, --use-ai, --report.

- **Deleted**: AscxToRazorConverter.cs (replaced by pipeline + transforms).

- **PackageId**: Changed from `WebformsToBlazor.Cli` to `Fritz.WebFormsToBlazor`.

**Validation**: All 12 test cases (TC01-TC12) produce exact expected output. Zero build errors.

**Key Learnings**:
1. Order of transforms matters critically  AjaxToolkitPrefix (600) MUST run before AspPrefix (610) to avoid treating `ajaxToolkit:` controls as `asp:` controls.
2. AttributeStrip's ItemType="object" fallback injects BEFORE other attributes in the tag, matching the PS script's behavior and test expectations.
3. Expression transforms must be ordered: Bind() before Eval() before encoded/unencoded, with comments first.
4. DataSourceId transform runs last (820) because it matches bare control names (asp: prefix already stripped).
5. ContentWrapperTransform strips asp:Content open+close tags using horizontal-whitespace-only patterns to avoid consuming indentation on the next line.

### MasterPageTransform + GetRouteUrlTransform + ManualItem + TC12–TC23 (2026-04-03)

**Commit:** `6824cbdc` on `feature/global-tool-port` — 41 files

**MasterPageTransform:** Rewrites `<%@ MasterPageFile="~/Site.Master" %>` directive into Blazor `@layout SiteMaster` reference. Handles filename → class name conversion (strip extension, PascalCase).

**GetRouteUrlTransform:** Converts `GetRouteUrl("routeName", new { key = val })` calls to Blazor `NavigationManager.GetUriWithQueryParameters()` equivalents. Registered in pipeline at Order 750 (after UrlReference, before normalization).

**ManualItem model:** Structured record for migration report entries flagged for manual developer review. Fields: `Category` (slug), `File`, `Line`, `Message`, `Severity`. Enables typed JSON output in migration report rather than raw strings.

**TC12–TC23 test data:** 12 new acceptance test input/expected-output pairs covering: MasterPage directives, GetRouteUrl calls, ManualItem annotation injection, ViewState attributes, ContentPlaceHolder, LoginView, Cache directives, SelectMethod, ValidationSummary, and mixed-transform scenarios.

**Key learnings:**
1. Master page filename → layout class name conversion must strip `~/`, directory path, and `.Master` extension, then PascalCase — a single regex replacement handles the common case but a helper method is cleaner for edge cases (spaces, hyphens).
2. GetRouteUrl route-parameter extraction uses named capture groups to map `new { k = v }` anonymous objects; iteration order of anonymous-object properties must be preserved (C# doesn't guarantee it at runtime, but test data uses single-param routes to avoid ordering issues).
3. ManualItem severity enum (`Info`, `Warning`, `Error`) maps to exit code: any `Error`-level item causes non-zero CLI exit, enabling CI gate integration.

📌 Team update (2026-04-12): All migration transforms pipeline infrastructure complete — 17 transforms (added 3: ConfigurationManager, RequestForm, ServerShim), 373/373 tests passing, expected files regenerated. WingtipToys analysis shows WebFormsPageBase enables 31 pages to eliminate manual shim wiring. — decided by Psylocke, Forge, Bishop

### Shim Inventory & CLI Transform Update (2026-04-12)

**Shims Identified (14 total)**:
- FormShim, ClientScriptShim, ResponseShim, RequestShim, SessionShim, ServerShim, CacheShim, ScriptManagerShim — auto-wired via WebFormsPageBase DI
- ConfigurationManager — static shim bridging `AppSettings`/`ConnectionStrings` to ASP.NET Core `IConfiguration`
- WebFormsPageBase — ComponentBase subclass with shim properties
- BundleConfig, RouteConfig — startup compatibility helpers
- PostBackEventArgs, FormSubmitEventArgs — event models for `<WebFormsForm>`

**New CLI Transforms Created**:
1. **ConfigurationManagerTransform** (Order 110) — strips `using System.Configuration;` (BWFC shim replaces it), detects `AppSettings`/`ConnectionStrings` usage, emits guidance block
2. **RequestFormTransform** (Order 320) — detects `Request.Form["key"]` patterns, emits FormShim + `<WebFormsForm>` guidance
3. **ServerShimTransform** (Order 330) — detects `Server.MapPath()`, `Server.HtmlEncode()`, `Server.UrlEncode()`, `Server.UrlDecode()`, emits ServerShim guidance

**Updated Files**:
- `TodoHeaderTransform`: header now references all 14 shims with tagged TODO markers
- `bwfc-migrate.ps1`: added BWFC015-018 scan patterns for Server utility, ConfigurationManager, ClientScript, Cache access
- `CONTROL-COVERAGE.md`: added "Migration Shims (14)" section
- `TestHelpers.cs`: registered 3 new transforms in test pipeline
- All 13 expected `.razor.cs` test files regenerated

**Key Learnings**:
1. New transforms must be registered in BOTH `Program.cs` (DI) AND `TestHelpers.CreateDefaultPipeline()` — forgetting the test pipeline causes L1 integration test failures
2. Transforms are sorted by `Order` property in `MigrationPipeline`, so list order in registration doesn't matter
3. When changing the TodoHeader, ALL 13 code-behind expected files must be regenerated — use a temp console project referencing the CLI to regenerate via the actual pipeline
4. `UsingStripTransform` already handles `System.Web.Optimization` and `System.Web.Routing` via its `WebUsingsRegex` pattern — no extra work needed for BundleConfig/RouteConfig namespaces
5. "Guidance-only" transforms (detect + TODO comment) are the right pattern when shims make original code compile unchanged on WebFormsPageBase

### Shim-First Documentation Update (2026-04-13)

**Task**: Update all 5 migration-toolkit docs (METHODOLOGY.md, CHECKLIST.md, QUICKSTART.md, README.md, CONTROL-COVERAGE.md) to reflect the "shim-first" migration paradigm.

**Key Changes**:
- Pipeline coverage percentages updated: L1 ~60% (was ~40%), L2 ~30% (was ~45%), L3 ~10% (was ~15%)
- METHODOLOGY.md: Added "Shim Infrastructure" subsection to Layer 1, "What Shims Handle Automatically" table to Layer 2, "Shim Path vs. Native Blazor Path" comparison box, removed Session from L3 decisions
- CHECKLIST.md: Added L1 shim setup items (AddBlazorWebFormsComponents, @inherits, WebFormsForm), marked Response.Redirect/Session/IsPostBack/Page.Title/Request.QueryString/Cache as "✅ works AS-IS", added "Optional: Refactor to Native Blazor" section
- QUICKSTART.md: Added shim callout in Step 4 and Step 6, updated transform table removing shim-handled items, added WebFormsForm guidance
- README.md: Updated coverage percentages, time estimates, added shim bullet in Quick Overview
- CONTROL-COVERAGE.md: Added "Infrastructure & Shim Components" section with full 15-row table, updated supporting component count to 96 (total 154)

**Key Decisions**:
1. Session["key"] moved from Layer 3 architecture decision to "works AS-IS" — SessionShim provides in-memory dictionary. Persistent/distributed session is still an architecture decision but basic usage compiles unchanged.
2. Response.Redirect removed from Layer 2 manual transforms — ResponseShim handles it automatically including ~/prefix and .aspx stripping.
3. Added "Optional: Refactor to Native Blazor" as a post-verification section in the checklist — acknowledges shims are a valid long-term choice, not just a crutch.
4. Time estimates reduced: Layer 2 with Copilot from 2-4 hours to 1-3 hours reflecting reduced manual work.

**Files Modified**: migration-toolkit/METHODOLOGY.md, CHECKLIST.md, QUICKSTART.md, README.md, CONTROL-COVERAGE.md