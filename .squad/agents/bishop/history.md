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

## Learnings

### 2026-05-05T15:02:36-04:00: CLI compile-surface hardening for migrated apps (Bishop)

- `src\BlazorWebFormsComponents.Cli\Scaffolding\ProjectScaffolder.cs` now emits `<EnforceCodeStyleInBuild>false</EnforceCodeStyleInBuild>` so copied legacy C# files are not blocked by repo-level IDE style analyzers during generated-app builds.
- Validator generic arguments must match BWFC component generic parameter names, not generic Razor conventions: `RequiredFieldValidator` needs `Type="string"`, while `CompareValidator` and `RangeValidator` need `InputType="string"`. The deterministic transform lives in `src\BlazorWebFormsComponents.Cli\Transforms\Markup\ValidatorGenericTypeTransform.cs`.
- Compiled `.razor.cs` emission is safer when the pipeline scans transformed markup for unresolved `@Method()`, `@_field`, and `OnClick="@Handler"` references and appends fallback stubs after the main code-behind transforms. The implementation lives in `src\BlazorWebFormsComponents.Cli\Transforms\CodeBehind\MarkupReferencedMemberStubTransform.cs` and depends on `MigrationPipeline.TransformMarkup()` persisting `metadata.MarkupContent`.
- Regression coverage for these fixes lives in `tests\BlazorWebFormsComponents.Cli.Tests\TransformUnit\ValidatorGenericTypeTransformTests.cs`, `tests\BlazorWebFormsComponents.Cli.Tests\TransformUnit\MarkupReferencedMemberStubTransformTests.cs`, `tests\BlazorWebFormsComponents.Cli.Tests\TransformUnit\CompiledCodeBehindStubPipelineTests.cs`, and `tests\BlazorWebFormsComponents.Cli.Tests\ScaffoldingTests.cs`.
- Verified command: `dotnet test .\tests\BlazorWebFormsComponents.Cli.Tests --nologo` (499 passed).

### 2026-05-06T09:16:32-04:00: CLI-only deprecation audit for migration tooling (Bishop)

- `migration-toolkit\scripts\bwfc-migrate.ps1` is now only a thin wrapper: it resolves the CLI project, forwards to `dotnet run --project ... -- migrate|prescan`, and returns before the legacy PowerShell implementation executes.
- The C# `MigrationPipeline` already owns scaffold/config/markup/code-behind/static/source/App_Start/redirect orchestration, but it still bridges `Migrate-NugetStaticAssets.ps1` and `Convert-EdmxToEfCore.ps1` through `NuGetStaticAssetExtractor` and `EdmxConverterBridge`.
- `bwfc-scan.ps1` still has no CLI equivalent; `PrescanAnalyzer` only covers C# migration-pattern scanning and currently omits the old PowerShell `BWFC021` master-page rule.
- The deprecation roadmap is captured in `dev-docs\cli-migration-plan.md`, with P0 focus on removing internal PowerShell runtime dependencies and adding CLI-first replacements for scan/assets/edmx workflows.

### 2026-05-06T10:08:56-04:00: Native CLI replacements for assets and EDMX helpers (Bishop)

- Added native C# services at `src\BlazorWebFormsComponents.Cli\Services\NuGetStaticAssetExtractor.cs` and `src\BlazorWebFormsComponents.Cli\Services\EdmxToEfCoreConverter.cs`, and rewired `Program.cs` plus `MigrationPipeline` to use them instead of the PowerShell bridge classes.
- The asset extractor now supports `packages.config` and top-level `PackageReference`, searches legacy `packages\` folders plus the global NuGet cache, writes `asset-manifest.json` and `AssetReferences.html`, and honors a manifest-only mode without copying files.
- The EDMX converter now parses conceptual/mapping metadata with `System.Xml.Linq`, generates POCO entities plus a DbContext with relationship configuration, and returns graceful failures for missing or invalid EDMX input.
- Added first-class CLI entrypoints for `scan`, `assets extract`, and `edmx convert`, then updated all four migration-toolkit PowerShell scripts with deprecation banners that point to the CLI command to use instead.
- Regression coverage lives in `tests\BlazorWebFormsComponents.Cli.Tests\Services\NuGetStaticAssetExtractorTests.cs` and `tests\BlazorWebFormsComponents.Cli.Tests\Services\EdmxToEfCoreConverterTests.cs`; verified with `dotnet build src\BlazorWebFormsComponents.Cli\BlazorWebFormsComponents.Cli.csproj` and `dotnet test tests\BlazorWebFormsComponents.Cli.Tests --nologo`.

### 2026-05-06T14:00:00-04:00: ServerCodeBlockTransform and TemplateFieldChildComponentsTransform (Bishop)
- Implemented ServerCodeBlockTransform (Order 510). Converts Web Forms statement blocks to Razor control-flow.
- Implemented TemplateFieldChildComponentsTransform (Order 620). Wraps TemplateField style child elements in ChildComponents blocks.
- Full CLI test suite: 545 passed, 0 failed.
- Key lesson: Avoid C# interpolated brace-escaping in transform return strings. Use plain concatenation instead.
- Decision file: .squad\decisions\inbox\bishop-gap3-gap5-transforms.md

### 2026-05-06T18:00:00-04:00: WingtipToys Migration Run 34 — First Run With New Transforms (Bishop)
- Executed Run 34 benchmark validating ServerCodeBlockTransform (Order 510) and TemplateFieldChildComponentsTransform (Order 620).
- Both transforms confirmed working in generated output. Two manual fixes from Run 33 eliminated automatically.
- Initial build errors: 266 (vs. 252 in Run 33). Final: 0 errors. **25/25 acceptance tests passing.**
- Key architectural learning: Blazor Server scoped services are NOT safe for cart state across the SSR pre-render → interactive circuit transition. When a Blazor Server component renders, `OnInitializedAsync` runs twice: once during SSR pre-render (with a temporary scoped container) and once when the interactive circuit connects (with a new scoped container). A cart stored only in a scoped service is lost at that boundary. Fix: store cart state in a singleton keyed by ASP.NET Core session cookie value (`CartSessionStore`), making it survive both HTTP redirects and circuit reconnections.
- Key Playwright learning: `WaitForLoadStateAsync(NetworkIdle)` resolves when the HTTP response chain settles. The Blazor interactive circuit connects via WebSocket AFTER that. If a test clicks a Blazor `@onclick` button and immediately checks results after `NetworkIdle`, it may check before the Blazor event fires. Design server interactions as Minimal API HTTP endpoints (not Blazor event handlers) when they need to be testable with Playwright's `NetworkIdle` approach.
- Cart endpoints pattern: `MapGet("/AddToCart", ...)` and `MapGet("/RemoveFromCart", ...)` in Program.cs handle cart mutations as HTTP round-trips that update `CartSessionStore` and redirect. The Blazor `ShoppingCart` component reads from the same singleton at pre-render time.
- Ambiguous route conflict: Blazor `@page` directives and Minimal API `MapGet` both claim the same URL. Solution: remove `@page` from the Blazor component (keep the component for rendering use) and let the Minimal API own the route.
- Report: dev-docs\migration-tests\wingtiptoys\run34\report.md

### 2026-05-06T15:12:13-04:00: Run 34 gap shims and attribute-binding transform (Bishop)
- BaseStyledComponent now accepts Width/Height as plain Razor strings (`"500"`, `"500px"`, `"50%"`) by storing string parameters and parsing them back to `Unit` for the internal `IStyle` contract. Legacy quoted `Unit.Pixel(200)` strings are still parsed so existing samples/tests keep working.
- Added `BlazorWebFormsComponents.QueryStringExtensions.Get(string)` so migrated `Request.QueryString.Get("key")` code compiles against the existing `RequestShim.QueryString` collection without rewriting callers.
- Added `System.Web.HttpUtility` compatibility methods (`UrlEncode`, `UrlDecode`, `HtmlEncode`, `HtmlDecode`) backed by `System.Net.WebUtility`. Tests had to use reflection because the test graph also references the legacy `System.Web.HttpUtility` package, which makes direct symbol references ambiguous.
- Added CLI `DataBindingAttributeTransform` (Order 615) and registered it in production/test pipelines so attribute values like `NavigateUrl='<%# Item.GetUrl() %>'` become `NavigateUrl='@(Item.GetUrl())'` after prefix stripping.
- Validation: `dotnet build src\BlazorWebFormsComponents\BlazorWebFormsComponents.csproj --nologo`, `dotnet test src\BlazorWebFormsComponents.Test --nologo` (2895 passing per target), and `dotnet test tests\BlazorWebFormsComponents.Cli.Tests --nologo` (552 passing).

### 2026-05-06T16:54:14-0400 : WingtipToys Migration Run 35 � Benchmark after gap fixes (Bishop)
- Executed fresh Run 35 benchmark on commit `a6fa2455bec610b01127ab852886263f80f4bb1e` from branch `feature/wingtip-next-features-review` using `migration-toolkit\scripts\bwfc-migrate.ps1`.
- Final result: build succeeded and **25/25 Wingtip acceptance tests passed**; screenshots captured under `dev-docs\migration-tests\wingtiptoys\run35\images\`.
- Gap #3 (string Width/Height) and Gap #4 (attribute databinding transform) helped immediately in fresh output; no new `Request.QueryString.Get(...)` compile blocker surfaced.
- Gap #10 (`System.Web.HttpUtility`) still needs follow-up for real migrated apps that also reference the legacy `System.Web.HttpUtility` package; `Logic\PayPalFunctions.cs` required manual `WebUtility` substitutions to avoid symbol ambiguity.
- Biggest remaining automation gaps were invalid Razor syntax in generated markup, unsupported master-page script/bundle constructs, EF6-style `ProductContext` constructor emission, and a long tail of unresolved account/admin/mobile compile-surface pages that had to be stubbed or simplified manually.
- Report: `dev-docs\migration-tests\wingtiptoys\run35\report.md`

### 2026-05-06T17:30:00-04:00: Run 35 gap transforms G1/G3/G8/G10 (Bishop)
- Added CLI markup `DisplayExpressionTransform` (Order 490) so `<%#: expr %>`, `<%=: expr %>`, and broken `@(: expr)` output normalize to valid Razor `@(...)` before the broader expression pass.
- Added code-behind transforms `HttpUtilityRewriteTransform` (Order 104) and `EfContextConstructorTransform` (Order 106); they now rewrite `HttpUtility.*` calls to `WebUtility.*`, add `using System.Net`, modernize EF6 `DbContext` string constructors to EF Core `DbContextOptions<TContext>`, and are included in `SourceFileCopier` so copied Models/Logic files are fixed too.
- Extended `ServerShim` with `Transfer`, `GetLastError`, and `ClearError`, and updated `ServerShimTransform` guidance to treat those APIs as supported compatibility shims instead of dead-end TODOs.
- Added focused regression tests for the new transforms, source-file copying, and ServerShim behavior; updated CLI docs plus migration docs so HttpUtility guidance now points to inline `WebUtility` rewrites rather than relying on the legacy shim package.
- Validation: `dotnet build src\BlazorWebFormsComponents\BlazorWebFormsComponents.csproj --nologo`, `dotnet test src\BlazorWebFormsComponents.Test --nologo`, and `dotnet test tests\BlazorWebFormsComponents.Cli.Tests --nologo`.

### 2026-05-06T18:55:09-04:00: WingtipToys Migration Run 36 — benchmark after G1/G3/G8/G10 validation (Bishop)
- Executed fresh Run 36 benchmark on commit `aa8f70cbea53ed86734e6b550167248c63b86304` from branch `feature/wingtip-next-features-review` using `migration-toolkit\scripts\bwfc-migrate.ps1`; final result was **25/25 acceptance tests passing** with screenshots under `dev-docs\migration-tests\wingtiptoys\run36\images\`.
- Total runtime dropped to `00:10:12` and Layer 2/3 repair time dropped to `00:05:37`, a meaningful improvement over Run 35 (`00:17:34` total, `00:10:03` repair).
- G3 removed the old `ProductContext : base("WingtipToys")` constructor-definition blocker, but generated callers still emitted `new ProductContext()` and need a follow-up call-site transform.
- G8 removed missing-method compile errors for `Server.GetLastError()` / `Server.ClearError()`, but generated code still emits the old `Server.Transfer(path, true)` shape, so either an overload or a rewrite is still needed.
- G10 removed live compiled-source `HttpUtility` blockers from the benchmark app; remaining `HttpUtility` references were only left in quarantined `migration-artifacts` text files.
- G1 reduced the error surface by eliminating the broken `@(: expr)` class, but a raw `<%#:` expression still leaked into `ShoppingCart.razor`, so templated display-expression cleanup is not complete yet.
- Biggest remaining benchmark gaps were the unsupported master-page script/bundle shell, unresolved account/admin/checkout compile surfaces, and the need for a lightweight runtime scaffold (catalog/cart/auth) to validate the acceptance path quickly.
- Report: `dev-docs\migration-tests\wingtiptoys\run36\report.md`

### 2026-05-06T21:33:09-04:00: DisplayExpressionTransform emits idiomatic Razor for simple expressions (Bishop)
- `src\BlazorWebFormsComponents.Cli\Transforms\Markup\DisplayExpressionTransform.cs` now classifies dotted identifier chains with a dedicated regex helper and emits `@expr` for simple display expressions while preserving `@(expr)` for method calls, operators, and other complex expressions.
- Updated focused coverage in `tests\BlazorWebFormsComponents.Cli.Tests\TransformUnit\DisplayExpressionTransformTests.cs` for simple and complex cases, plus pipeline assertions for both shapes.
- Updated L1 expected fixtures `tests\BlazorWebFormsComponents.Cli.Tests\TestData\expected\TC06-Expressions.razor` and `tests\BlazorWebFormsComponents.Cli.Tests\TestData\expected\TC30-DataDrivenPage.razor` to reflect idiomatic Razor output in generated markup.
- Validation: `dotnet test tests\BlazorWebFormsComponents.Cli.Tests --nologo --filter "DisplayExpression"` (9 passing) and `dotnet test tests\BlazorWebFormsComponents.Cli.Tests --nologo` (573 passing).

### 2026-05-06T22:04:10-04:00: WingtipToys Migration Run 37 — benchmark after DisplayExpressionTransform idiomatic output (Bishop)
- Executed fresh Run 37 benchmark on commit `d35ff5c114bf834081cb38685a445599f86fe053` from branch `feature/wingtip-next-features-review` using `migration-toolkit\scripts\bwfc-migrate.ps1`; final result was **25/25 acceptance tests passing** with screenshots under `dev-docs\migration-tests\wingtiptoys\run37\images\`.
- The simple-expression `@expr` improvement showed up in fresh output (`@Item.ProductName` style markup was cleaner than earlier parenthesized output), but it did **not** reduce end-to-end repair time because invalid templated markup, master-page shell output, compile-surface pages, and unfinished auth/runtime scaffolding still dominated the benchmark.
- A raw `<%#:` display expression still leaked into `ShoppingCart.razor`, confirming the current transform does not yet cover all templated display-expression shapes.
- Generated `ListView` / `FormView` output still shipped malformed HTML, wrong item-context references, and unsupported child-content structure, so the benchmark path remained blocked until those pages were rewritten manually.
- Large `Account/*`, `Admin/*`, and `Checkout/*` routable surfaces still arrived as uncompilable compile-surface debt. The fastest repair path was still aggressive page simplification/stubbing around the acceptance-tested flow.

### 2026-05-06T22:13:18-04:00: Run 37 gap fixes G1/G2/G4 for CLI output hardening (Bishop)
- `DisplayExpressionTransform` now allows `String.Format(...)` through the display-expression path by excluding only `Bind(...)` and `Eval(...)`; `<%#: String.Format(...) %>` now becomes valid Razor `@(String.Format(...))` instead of leaking raw Web Forms syntax.
- Added markup `ScriptManagerStripTransform` (Order 255) immediately after `MasterPageTransform`. It strips `<asp:ScriptManager>...</asp:ScriptManager>`, `<webopt:bundlereference ... />`, and `Scripts.Render(...)` placeholders from `.master` output, replacing the ScriptManager block with a single Blazor guidance comment and collapsing excess blank lines.
- Added code-behind `CompileSurfaceStubTransform` (Order 850) plus `PageCodeBehindEmissionPlanner`/`MigrationPipeline` support for dual output: infrastructure-heavy pages now emit a build-safe `.razor`/`.razor.cs` stub into the compile surface while preserving the transformed pre-stub code-behind under `migration-artifacts\codebehind\` and recording a `bwfc-compile-surface` manual item.
- Login.aspx and Register.aspx remain explicitly exempt from compile-surface stubbing so the existing account semantic pattern and acceptance-path auth scaffolding continue to run.
- Updated CLI docs (`docs\cli\index.md`, `docs\cli\transforms.md`) and added focused regression coverage in `DisplayExpressionTransformTests`, `ScriptManagerStripTransformTests`, `CompileSurfaceStubTransformTests`, and `PipelineIntegrationTests`.
- Validation sequence: baseline `dotnet test tests\BlazorWebFormsComponents.Cli.Tests --no-restore --nologo` (573 passing), after G1 (577 passing), after G2 (582 passing), after G4/final docs/tests (588 passing).

### 2026-05-07T13:05:50-04:00: WingtipToys Migration Run 40 — fresh wrapper benchmark with runtime scaffold validation (Bishop)
- Executed fresh Run 40 benchmark from a cleared `samples\AfterWingtipToys` folder using `migration-toolkit\scripts\bwfc-migrate.ps1`; final result was **25/25 Wingtip acceptance tests passing** with screenshots under `dev-docs\migration-tests\wingtiptoys\run40\images\`.
- The generated `RuntimeDetector` / `ProgramCsEmitter` scaffold was a credible starting point (modern `Program.cs`, Razor Components shell, session wiring, launch settings), but it was still not benchmark-ready without manual catalog/cart/auth runtime simplification.
- Fresh output still arrived with heavy compile-surface debt: malformed `ListView` / `FormView` markup, many `.razor.cs` files explicitly inheriting `ComponentBase`, and a long tail of `Account/*`, `Checkout/*`, mobile, and payment surfaces that were faster to stub than fully migrate.
- The acceptance path stayed within BWFC controls: `ProductList` kept `ListView`, `ProductDetails` kept `FormView`, and `ShoppingCart` kept `GridView`; the last failing test was fixed by using an explicit session-backed `cart-key` instead of relying directly on `Session.Id`.
- Final saved artifacts: `dev-docs\migration-tests\wingtiptoys\run40\report.md`, `build-final.log`, `acceptance-final.log`, and the six screenshot captures.
