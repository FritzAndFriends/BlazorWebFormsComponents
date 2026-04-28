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


### 2026-04-28: First isolated semantic catalog entries (Bishop)

- Added `QueryDetailsSemanticPattern` and `ActionPagesSemanticPattern` under `src/BlazorWebFormsComponents.Cli/SemanticPatterns/` as the first real catalog entries on the isolated semantic runtime.
- `pattern-query-details` matches post-transform pages with exactly one `SelectMethod="..."` plus `[QueryString]` / `[RouteData]` parameters in the referenced code-behind method. It rewrites `SelectMethod` → `SelectItems`, emits `[SupplyParameterFromQuery]` / `[Parameter]` component properties, and leaves an explicit `TODO(bwfc-query-details)` stub that points developers at the quarantined code-behind artifact.
- `pattern-action-pages` matches inert action-only pages (for example `AddToCart.aspx`) with deterministic query-string inputs and one redirect target. It rewrites the output to a visible SSR handler scaffold with query-bound properties, `NavigationManager` injection, redirect target capture, and `TODO(bwfc-action-pages)` guidance instead of blank migrated HTML.
- Production wiring now registers semantic patterns via DI in `Program.cs`, and `MigrationPipeline` is created through an explicit factory to avoid constructor ambiguity between the full DI constructor and the lightweight transform-only constructor.
- Regression coverage lives in `tests/BlazorWebFormsComponents.Cli.Tests/SemanticPatternCatalogTests.cs` and `tests/BlazorWebFormsComponents.Cli.Tests/PipelineIntegrationTests.cs`. Verified commands: `dotnet test .\tests\BlazorWebFormsComponents.Cli.Tests\BlazorWebFormsComponents.Cli.Tests.csproj --no-restore` and `dotnet run --project .\src\BlazorWebFormsComponents.Cli --no-build -- migrate --input ... --output ... --skip-scaffold --overwrite`.


### 2026-04-28: Semantic Pattern Infrastructure Sprint - All Agents

**Task:** Complete semantic pattern infrastructure for BlazorWebFormsComponents semantic pattern catalog.

**Bishop:**
- Implemented pattern-query-details and pattern-action-pages infrastructure
- Wired production and test registration for all patterns
- Added isolated and pipeline regression tests

**Cyclops:**
- Implemented pattern-account-pages infrastructure
- Implemented pattern-master-content-contracts with helper logic
- Added focused concrete tests

**Forge:**
- Performed comprehensive reviewer safety pass
- Approved bounded semantics and manual TODO boundaries
- Special review of authentication and master/content section patterns

**Rogue:**
- QA audit identified missing default registration gap
- Recommended helper and integration test coverage
- Re-check confirmed gap was resolved by Bishop

**Coordinator:**
- Executed full test suite: 486 passed, 0 failed
- Verified all tests passing before archival

**Outcome:** All semantic pattern contracts approved and production-ready.