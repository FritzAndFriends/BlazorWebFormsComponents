# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Project Learnings (from import)

- The migration-toolkit lives at `migration-toolkit/` and contains: scripts/, skills/, METHODOLOGY.md, CHECKLIST.md, CONTROL-COVERAGE.md, QUICKSTART.md, README.md, copilot-instructions-template.md
- Primary migration script: `migration-toolkit/scripts/bwfc-migrate.ps1` — handles Layer 1 (automated transform)
- Layer 2 (agent-driven implementation) is where BWFC control replacement failures occur — agents replace asp: controls with plain HTML
- Test-BwfcControlPreservation in bwfc-migrate.ps1 validates that BWFC controls are preserved post-transform
- Test-UnconvertiblePage uses path-based patterns (Checkout\, Account\) and content patterns (SignInManager, UserManager, etc.)
- Sample migration targets: WingtipToys (before/after samples in samples/ directory)
- Run7 is the current gold standard: `samples/Run7WingtipToys/`
- The BWFC library has 110+ components covering Web Forms controls
- Migration must preserve all asp: controls as BWFC components — never flatten to raw HTML

## Core Context

<!-- ⚠ Summarized 2026-03-06 by Scribe — covers Run 9 through Run 11, Cycles 1-2. Originals in history-archive.md -->

**Run 9-11 benchmark progression:** Build attempts 7→3→4, Layer 2 time 45→25→20 min, BWFC instances 173→172→178, preservation 98.9%→92.7%→98.9%. All runs 0 errors, 0 warnings. 28 routable pages, ~32 .razor, ~79 wwwroot throughout.

**Cycle 1 fixes (5 items):** P0-1 ItemType→TItem regex (only list controls get TItem, data controls retain ItemType). P0-2 smart stubs (all markup gets L1 transforms, only code-behinds stubbed). P0-3 base class stripping (`: Page`/`: UserControl`/`: MasterPage` removed). P1-1 validator type params (Type="string"/InputType="string" auto-injected). P1-4 ImageButton warning in Test-BwfcControlPreservation.

**Cycle 2 fixes (6 items):** Convert-EnumAttributes (TextMode→TextBoxMode, Display→@ValidatorDisplay, GridLines→@GridLines). Boolean normalization (True/False → true/false). ControlToValidate/ValidationGroup stripping. ImageButton detection escalated to FAIL. Server-side expression cleanup (Request/Session/Server wrapped in TODO comments). Remove-ItemTypeWithDataSource (strips ItemType/TItem when SelectMethod present).

**Key Layer 2 patterns:** `@inherits WebFormsPageBase` conflicts with `: ComponentBase` — must remove explicit base. Layout files need `: LayoutComponentBase`. `AddHttpContextAccessor()` before `AddBlazorWebFormsComponents()`. Stub model pattern for unavailable types (UserLoginInfo, OrderShipInfo). Stub page cleanup ~60% of L2 effort.

## Learnings

### Cycle 3 Fixes Applied (Bishop) — Script + Auth Services

- **P0.3 (Mock Auth Service Generation):** New `Add-MockAuthService` function in bwfc-migrate.ps1 auto-generates `Services/MockAuthService.cs` (in-memory user store with email/password) and `Services/MockAuthenticationStateProvider.cs` (Blazor auth state provider with login/logout) when Account/Login.aspx is detected in source. Eliminates manual auth scaffolding in Layer 2.
- **P0.1+P0.2 (Program.cs Auth Injection):** `New-ProjectScaffold` now accepts `$HasIdentityPages` parameter and conditionally adds `AddScoped<MockAuthService>()`, `AddScoped<MockAuthenticationStateProvider>()`, `AddCascadingAuthenticationState()`, and `AddAuthorization()` to Program.cs template.
- **P1.4 (LogoutAction Enum):** Added LogoutAction enum conversions: Redirect, RedirectToLoginPage, Refresh → `@LogoutAction.{Value}`.
- **P1.4 (BorderStyle Enum):** Added BorderStyle enum conversions: None, NotSet, Dotted, Dashed, Solid, Double, Groove, Ridge, Inset, Outset → `@BorderStyle.{Value}`.
- **P1.3 (Visible Attribute):** New `Convert-VisibleAttribute` function strips `Visible="true"` (default value), preserves `Visible="false"` for BWFC components.
- **P2.2 (Hex Color Escaping):** New `Convert-HexColors` function escapes hex color values like `BorderColor="#efeeef"` to `BorderColor="@("#efeeef")"` to prevent C# preprocessor directive interpretation in Razor.

### 2026-03-05: Run 12 WingtipToys Migration Benchmark

- **Result:** Build succeeded — 0 errors, 0 warnings, 2 build attempts (best ever — down from 4 in Run 11)
- **Pipeline:** Layer 1 (~3.5s, 365 ops) → Layer 2 (~15min) → Build (~3s, 2 attempts)
- **Output:** 32 .razor, 32 .cs, 79 wwwroot, 28 routable pages, 184 BWFC control instances (27 unique types)
- **Auth services fully functional (P0 complete):**
  - Login.razor.cs: Injects MockAuthenticationStateProvider + MockAuthService, calls LoginAsync, NavigateTo "/" on success
  - Register.razor.cs: Calls CreateUserAsync, auto-login on success, NavigateTo "/Account/Login"
  - MockAuthService: In-memory user store (admin@wingtiptoys.com / Pass@word1 default)
  - LoginStatus: Preserved as BWFC component on MainLayout with `LogoutAction="@LogoutAction.Redirect"`
  - Login/Register markup uses `@bind-Text` for TextBox → code-behind field binding
- **Layer 1 script improvements (6 new features):**
  - Mock auth service generation (conditional on Account/Login.aspx)
  - Program.cs auth injection (conditional)
  - LogoutAction + BorderStyle enum conversions
  - Visible attribute handling (strip true, preserve false)
  - Hex color escaping for Razor safety
- **BWFC preservation rate:** ~100% (184 instances, 27 unique types — up from 178/26 in Run 11)
- **LinkButton newly preserved:** 3 instances (ManageLogins, Checkout pages) — previously flattened

📌 Team update (2026-03-05): Run 12 BENCHMARK complete. All quality gates passed: 0 build errors, ≤3 attempts (2), auth functional, 184 BWFC instances (≥178), ~100% preservation. First run with functional Login/Register. — decided by Bishop

### Fresh Layer 1 Migration Run (Bishop) — AfterWingtipToys

- **Date:** Run executed against cleared `samples/AfterWingtipToys/`
- **Script:** `bwfc-migrate.ps1` with `-Path samples/WingtipToys/WingtipToys -Output samples/AfterWingtipToys`
- **Execution time:** 3.33 seconds
- **Transforms applied:** 366
- **Files processed:** 32 Web Forms files
- **Static files copied:** 80
- **Items needing review:** 46 (14 CodeBlock, 1 ContentPlaceHolder, 2 ControlPreservation, 15 EventHandler, 1 LoginView-Auth, 4 RegisterDirective, 9 SelectMethod)
- **Output:**
  - 35 .razor files, 35 .cs files, 80 wwwroot files, 152 total files
  - Scaffold: WingtipToys.csproj ✓, Program.cs ✓, _Imports.razor ✓, Components/App.razor ✓, Components/Routes.razor ✓
  - Layout: MainLayout.razor + Site.MobileLayout.razor in Components/Layout/
  - Auth: Services/MockAuthService.cs + MockAuthenticationStateProvider.cs auto-generated (Account/Login.aspx detected)
  - Pages: 10 root pages, 15 Account pages, 6 Checkout pages, 1 Admin page
- **Observations:**
  - Task spec listed `-SourcePath`/`-DestinationPath`/`-ProjectName` params — actual params are `-Path`/`-Output` (no ProjectName). Script auto-detects project name from directory.
  - App.razor lives at `Components/App.razor` (Blazor Web App convention), not project root. Task verification should check there.
  - ControlPreservation flagged Site.Master: 1 `<PlaceHolder>` lost (5 asp: tags in → 4 BWFC tags out). Known issue from prior runs.
  - 0 errors during script execution. Clean run.

 Team update (2026-03-06): Layer 2 conventions established  Button OnClick uses EventArgs (not MouseEventArgs), code-behind class names must match .razor filenames exactly, use EF Core wildcard versions for .NET 10, CartStateService replaces Session, GridView needs explicit TItem  decided by Cyclops


 Team update (2026-03-06): Forge produced 8 script improvement recommendations (S1-S8) assigned to Bishop  S1: @inherits WebFormsPageBase in _Imports.razor, S2: AddHttpContextAccessor in Program.cs, S3: : Page  : WebFormsPageBase, S4: @using Enums, S5: Page_Load rename, S6: Cookie auth scaffold, S7: src~/action~ URL conversion, S8: Stub base class. Recommended Cycle 1: S1+S2+S3+S4  decided by Forge
 Team update (2026-03-06): LoginView is native BWFC  migration script must stop converting to AuthorizeView. Strip asp: prefix only, preserve template names  decided by Jeffrey T. Fritz, Forge
 Team update (2026-03-06): WebFormsPageBase is the canonical base class for all migrated pages (not ComponentBase). All agents must use WebFormsPageBase  decided by Jeffrey T. Fritz

### RouteData → [Parameter] Bug Fix (Bishop)

- **Bug:** `[RouteData]` on Web Forms method parameters was replaced with `[Parameter]` inline — but `[Parameter]` targets Properties only (CS0592). Also, using `//` TODO comments inline risked absorbing the closing `)` of method signatures.
- **Root cause:** `[RouteData]` is a Web Forms model-binding attribute for method parameters. `[Parameter]` is a Blazor component attribute valid only on properties. No inline Blazor equivalent exists for method parameter route binding.
- **Fix:** Changed regex to strip `[RouteData]` entirely from method parameters (not replace with `[Parameter]`). A `/* TODO */` block comment is placed on the preceding line directing Layer 2 to promote the value to a `[Parameter]` property on the component class. Block comment (`/* */`) used instead of line comment (`//`) to avoid absorbing trailing code.
- **Regex change:** `([ \t]*)\[RouteData\]` → `([ \t]*)\[RouteData\]\s*` (also consumes trailing whitespace to keep formatting clean).
- **L1 tests:** All 15 pass (100% pass rate, 100% line accuracy).
- **Key pattern:** When a Web Forms attribute has no inline Blazor equivalent, strip it and leave a TODO for Layer 2 — never substitute an attribute that targets a different declaration type.

## Phase 2: GAP-05 + GAP-07  Lifecycle and Event Handler Transforms (2026-03-29)

Added `Convert-PageLifecycleMethods` (GAP-05) and `Convert-EventHandlerSignatures` (GAP-07) to bwfc-migrate.ps1.
- GAP-05: Page_Load  OnInitializedAsync, Page_Init  OnInitialized, Page_PreRender  OnAfterRenderAsync(bool firstRender)
- GAP-07: Standard EventArgs handlers strip both params; specialized *EventArgs handlers keep the EventArgs param, strip sender
- Updated 6 expected test files. All 21 L1 tests pass at 100% line accuracy.

## Phase 3: Code-Behind C# Transforms — TC13-TC21 (2026-03-30)

Ported all 10 code-behind transforms from PowerShell regex patterns to C# classes implementing `ICodeBehindTransform`.

### Transforms Built (in pipeline order)
| Order | Class | Coverage |
|-------|-------|----------|
| 10 | `TodoHeaderTransform` | Injects migration guidance header |
| 100 | `UsingStripTransform` | Strips System.Web.*, Microsoft.AspNet.*, Microsoft.Owin.*, Owin usings |
| 200 | `BaseClassStripTransform` | Removes `: System.Web.UI.Page` etc. from partial classes |
| 300 | `ResponseRedirectTransform` | `Response.Redirect()` → `NavigationManager.NavigateTo()` + [Inject] injection |
| 400 | `SessionDetectTransform` | Detects `Session["key"]`, generates migration guidance block |
| 410 | `ViewStateDetectTransform` | Detects `ViewState["key"]`, generates field replacement suggestions |
| 500 | `IsPostBackTransform` | Unwraps simple `if (!IsPostBack)` guards (brace-counting), TODO for else clauses |
| 600 | `PageLifecycleTransform` | Page_Load→OnInitializedAsync, Page_Init→OnInitialized, Page_PreRender→OnAfterRenderAsync |
| 700 | `EventHandlerSignatureTransform` | Strips (object sender, EventArgs e); keeps specialized EventArgs |
| 800 | `DataBindTransform` | Cross-file DataSource/DataBind handling + InjectItemsAttributes for markup |
| 900 | `UrlCleanupTransform` | .aspx URL literals → clean routes |

### Infrastructure Changes
- Added `TransformCodeBehind()` public method to `MigrationPipeline` for test access
- Registered all 11 code-behind transforms in `Program.cs` DI container
- Activated real pipeline in `TestHelpers.CreateDefaultPipeline()` (replaced TODO stubs)
- Activated real assertions in `L1TransformTests` for both markup and code-behind tests
- Fixed TC20/TC21 expected markup files: `OnClick="@Handler"` matches EventWiringTransform output

### Key Learnings
- **IDE0007 enforcement:** Project .editorconfig treats `var` preference as error. Always use `var` over explicit types.
- **Transform ordering matters:** ResponseRedirect strips `~/` but preserves `.aspx`; UrlCleanup then handles `.aspx` patterns on `"~/..."` and relative NavigateTo forms. URLs like `/Products.aspx` survive because URL cleanup patterns don't match leading `/`.
- **TodoHeader as standalone transform (Order 10):** Splitting the TODO header into its own transform class keeps Session/ViewState detect transforms cleaner — they find the marker and insert after it.
- **Test discovery was previously placeholder:** The old tests only verified input ≠ expected. Wiring real pipeline exposed TC20/TC21 markup mismatches from EventWiringTransform `@` prefix.
- **All 72 tests pass:** 21 markup + 8 code-behind + 4 infrastructure + 39 unit tests.

### Phase 4: Scaffolding, Config Transforms, and Full Pipeline Wiring (Bishop)

Ported scaffolding, config transforms, and OutputWriter from bwfc-migrate.ps1 to C#. Wired the full `migrate` command pipeline.

#### New Files (9 total)
| File | Purpose |
|------|---------|
| `Config/DatabaseProviderDetector.cs` | 3-pass DB provider detection from Web.config (providerName → conn string pattern → EntityClient inner) |
| `Config/WebConfigTransformer.cs` | Parses Web.config appSettings + connectionStrings → appsettings.json (XDocument/LINQ to XML) |
| `Io/OutputWriter.cs` | Centralized file writer: dry-run support, UTF-8 no BOM, directory creation, file tracking |
| `Scaffolding/ProjectScaffolder.cs` | Generates .csproj, Program.cs, _Imports.razor, App.razor, Routes.razor, launchSettings.json |
| `Scaffolding/GlobalUsingsGenerator.cs` | Generates GlobalUsings.cs with Blazor infrastructure + conditional Identity usings |
| `Scaffolding/ShimGenerator.cs` | Generates WebFormsShims.cs + conditional IdentityShims.cs |

#### Pipeline Changes
- `MigrationPipeline.ExecuteAsync()` now runs: scaffold → config → per-file transforms → report
- `MigrationReport` enhanced: JSON serialization (`--report`), console summary, manual items tracking
- `Program.cs` DI wires all new services: ProjectScaffolder, GlobalUsingsGenerator, ShimGenerator, WebConfigTransformer, DatabaseProviderDetector, OutputWriter
- 2-param constructor preserved on `MigrationPipeline` for backward-compatible test usage

#### Key Patterns
- **ProjectScaffolder detects HasModels/HasIdentity** from source directory structure (Models/, Account/, Login.aspx, Register.aspx) — adjusts .csproj packages and Program.cs boilerplate accordingly
- **WebConfigTransformer skips built-in connection names** (LocalSqlServer, LocalMySqlServer) — matches PS behavior
- **DatabaseProviderDetector maps 4 providers:** SqlClient→SqlServer, SQLite→Sqlite, Npgsql→PostgreSQL, MySql→MySql
- **OutputWriter respects dry-run** — logs what would be written without touching disk
- **All templates are string literals** — no external template files, matching PS approach
- **Build clean:** 0 errors for both CLI and test projects

### Phase 5: ClientScriptTransform — Phase 1 of PRD ClientScript Migration (Bishop)

Added `ClientScriptTransform.cs` (Order 850) to the code-behind pipeline. Handles 6 ClientScript/ScriptManager patterns:

#### Automatable (transforms to IJSRuntime skeleton):
| Pattern | Action |
|---------|--------|
| `RegisterStartupScript()` with inline script | → `await JS.InvokeVoidAsync("eval", ...)` + TODO to refactor eval |
| `RegisterClientScriptInclude()` with URL | → `// TODO: Add <script src="url"/> to _Host.cshtml or App.razor` |
| `ScriptManager.RegisterStartupScript()` | → Same as RegisterStartupScript |

#### Non-automatable (TODO markers):
| Pattern | TODO |
|---------|------|
| `GetPostBackEventReference()` | Replace __doPostBack with @onclick or EventCallback |
| `RegisterClientScriptBlock()` | Move script block to IJSRuntime or .js file |
| `ScriptManager.GetCurrent()` | No Blazor equivalent — use IJSRuntime directly |

#### Infrastructure:
- Injects `[Inject] private IJSRuntime JS { get; set; }` when startup script conversions are made
- Registered in `Program.cs` and `TestHelpers.CreateDefaultPipeline()`
- Test data: TC33-ClientScript (input .aspx/.aspx.cs + expected .razor/.razor.cs)
- Updated L1TransformTests counts: 34 markup, 13 code-behind
- **All 330 tests pass** (0 failures, 0 regressions)

#### Key Regex Learnings:
- `[^)]*` fails inside method calls with nested parens (e.g., `this.GetType()`)
- `[^;]*` fails when string args contain semicolons (e.g., `"<script>var x = 1;</script>"`)
- **Safe pattern:** `(?:"[^"]*"|[^"])*?` — alternates quoted strings and non-quote chars, handles both issues

### ClientScriptTransform: Switched to Shim-Preserving Mode (Bishop)

- **Date:** 2026-07-31
- **What changed:** `ClientScriptTransform.cs` (Order 850) no longer rewrites ClientScript calls to IJSRuntime skeletons. Instead, it preserves calls for use with `ClientScriptShim`.
- **Shim-compatible patterns (prefix stripping, calls preserved):**
  - `Page.ClientScript.RegisterStartupScript(...)` → `ClientScript.RegisterStartupScript(...)` (strip prefix)
  - `Page.ClientScript.RegisterClientScriptInclude(...)` → `ClientScript.RegisterClientScriptInclude(...)` (strip prefix)
  - `Page.ClientScript.RegisterClientScriptBlock(...)` → `ClientScript.RegisterClientScriptBlock(...)` (strip prefix, shim now supports this)
  - `ScriptManager.RegisterStartupScript(control, type, key, script, bool)` → `ClientScript.RegisterStartupScript(type, key, script, bool)` (drops first param)
- **Still TODO-marked (no shim support):**
  - `GetPostBackEventReference(...)` → TODO with @onclick/EventCallback guidance (shim throws NotSupportedException)
  - `ScriptManager.GetCurrent(...)` → TODO with IJSRuntime guidance (no shim equivalent)
- **Removed:** IJSRuntime `[Inject]` injection logic. Replaced with single-line `ClientScriptShim` dependency comment at class level.
- **Key principle:** Jeff's directive — "Zero-rewrite shim approach is PRECISELY what we should be building." CLI preserves Web Forms API calls instead of rewriting them.
- **Tests:** All 349 tests pass (same count as before), updated 20 unit test assertions + TC33 expected output file.
- **Regex approach:** Single `PageOrThisPrefixRegex` with lookahead handles all three shim-compatible methods in one pass. Much simpler than the old per-pattern regexes with inline script extraction.

### ClientScriptTransform: Phase 2 — PostBack + ScriptManager Shim-Preserving (Bishop)

- **Date:** 2026-07-31
- **What changed:** `ClientScriptTransform.cs` (Order 850) now preserves ALL ClientScript/ScriptManager patterns for shim use. No more TODO markers.
- **Phase 2 patterns (newly shim-preserved):**
  - `Page.ClientScript.GetPostBackEventReference(...)` → `ClientScript.GetPostBackEventReference(...)` (prefix stripped)
  - `this.ClientScript.GetPostBackEventReference(...)` → `ClientScript.GetPostBackEventReference(...)` (prefix stripped)
  - `ScriptManager.GetCurrent(Page)` → `ScriptManager.GetCurrent(this)` (Page→this substitution)
  - `ScriptManager.GetCurrent(this.Page)` → `ScriptManager.GetCurrent(this)` (this.Page→this substitution)
  - `ScriptManager.GetCurrent(this)` → preserved as-is (already correct)
- **Regex changes:**
  - `PageOrThisPrefixRegex` lookahead expanded to include `GetPostBackEventReference`
  - Removed `GetPostBackEventRefRegex` and `ScriptManagerGetCurrentRegex` (TODO-emitting regexes)
  - Added `ScriptManagerGetCurrentPageRegex` for Page→this substitution
- **Shim comment:** Now conditionally mentions `ScriptManagerShim` when ScriptManager patterns detected (dual-shim comment)
- **Tests:** 353 total (was 349) — 4 new test cases for Phase 2 patterns, updated 6 existing assertions
- **Key design decision:** `hasScriptManagerCall` flag tracks ScriptManager presence separately from `hasShimCall` to conditionally generate the dual-shim comment

